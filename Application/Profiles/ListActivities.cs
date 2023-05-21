using Application.Core;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Profiles
{
    public class ListActivities
    {
        public class Query : IRequest<Result<PagedList<UserActivityDto>>>
        {
            public UserActivityParams Params { get; set; }
            public string Username { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<PagedList<UserActivityDto>>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                _context = context;

            }
            public async Task<Result<PagedList<UserActivityDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                // var query = _context.Activities
                // .Where(x => x.Attendees.Any(a => a.AppUser.UserName == request.Username))
                var query = _context.ActivityAttendees
                .Where(x => x.AppUser.UserName == request.Username)
                .ProjectTo<UserActivityDto>(_mapper.ConfigurationProvider)
                .AsQueryable();

                query = request.Params.Predicate.ToLower() switch
                {
                    "past" => query.Where(w => w.Date < DateTime.UtcNow).OrderByDescending(O => O.Date),
                    "hosting" => query.Where(w => w.Date >= DateTime.UtcNow && w.HostUsername == request.Username).OrderBy(O => O.Date),
                    _ => query = query.Where(w => w.Date >= DateTime.UtcNow).OrderBy(O => O.Date)
                };

                // switch (request.Params.Predicate.ToLower())
                // {
                //     case "past":
                //         query = query.Where(w => w.Date < DateTime.UtcNow)
                //         .OrderByDescending(O => O.Date);
                //         break;
                //     case "hosting":
                //         query = query.Where(w => w.Date >= DateTime.UtcNow && w.HostUsername == request.Username)
                //         .OrderBy(O => O.Date);
                //         break;
                //     case "future":
                //     default:
                //         query = query.Where(w => w.Date >= DateTime.UtcNow)
                //         .OrderBy(O => O.Date);
                //         break;
                // }
                //var activities = query.ToListAsync();
                return Result<PagedList<UserActivityDto>>.Success(await PagedList<UserActivityDto>.CreateAsync(query, request.Params.PageNumber, request.Params.PageSize));

            }
        }
    }
}
using Application.Core;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities
{
    public class List
    {
        //So we pass our Query, which in turn pass to the Handler, which handles the request and returns
        public class Query : IRequest<Result<PagedList<ActivityDto>>>
        {
            //public PagingParams Params { get; set; }
            public ActivityParams Params { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<PagedList<ActivityDto>>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            private readonly IUserAccessor _userAccessor;

            public Handler(DataContext context, IMapper mapper, IUserAccessor userAccessor)
            {
                _mapper = mapper;
                _userAccessor = userAccessor;
                _context = context;

            }
            public async Task<Result<PagedList<ActivityDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                //defer query execution 
                var query = _context.Activities
                            .Where(w => w.Date >= request.Params.StartDate)
                            .OrderBy(o => o.Date)
                            .ProjectTo<ActivityDto>(_mapper.ConfigurationProvider, new { currentUsername = _userAccessor.GetUserName() })
                            .AsQueryable();

                if (request.Params.IsGoing && !request.Params.IsHost)
                {
                    query = query.Where(x => x.Attendees.Any(a => a.Username == _userAccessor.GetUserName()));
                }

                if (request.Params.IsHost && !request.Params.IsGoing)
                {
                    query = query.Where(x => x.HostUsername == _userAccessor.GetUserName());
                }


                return Result<PagedList<ActivityDto>>.Success(await PagedList<ActivityDto>.CreateAsync(query, request.Params.PageNumber, request.Params.PageSize));

                #region Code from previous section
                // var activities = await _context.Activities
                //     //.Include(a => a.Attendees) //Eager Loading
                //     //.ThenInclude(u => u.AppUser)
                //     .ProjectTo<ActivityDto>(_mapper.ConfigurationProvider
                //         , new { currentUsername = _userAccessor.GetUserName() })
                //     //Alternatively we can use Select (System.Linq) instead of ProjectTo and add the fields manually. But automapper makes it super easy.
                //     .ToListAsync(cancellationToken);
                // //We need a mapper here to convert Activity to ActivityDto. Let's add this to Automapper class.
                // //var activitiesToReturn = _mapper.Map<List<ActivityDto>>(activities); //We dont need this anymore as we are receiving ActivityDto

                //return Result<PagedList<ActivityDto>>.Success(activities);
                #endregion Code from previous section
            }
        }
    }
}
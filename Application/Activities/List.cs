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
        public class Query : IRequest<Result<List<ActivityDto>>> { }

        public class Handler : IRequestHandler<Query, Result<List<ActivityDto>>>
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
            public async Task<Result<List<ActivityDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var activities = await _context.Activities
                    //.Include(a => a.Attendees) //Eager Loading
                    //.ThenInclude(u => u.AppUser)
                    .ProjectTo<ActivityDto>(_mapper.ConfigurationProvider
                        , new { currentUsername = _userAccessor.GetUserName() })
                    //Alternatively we can use Select (System.Linq) instead of ProjectTo and add the fields manually. But automapper makes it super easy.
                    .ToListAsync(cancellationToken);
                //We need a mapper here to convert Activity to ActivityDto. Let's add this to Automapper class.
                //var activitiesToReturn = _mapper.Map<List<ActivityDto>>(activities); //We dont need this anymore as we are receiving ActivityDto

                return Result<List<ActivityDto>>.Success(activities);
            }
        }
    }
}
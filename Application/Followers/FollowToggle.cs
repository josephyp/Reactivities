using Application.Core;
using Application.Interfaces;
using MediatR; //CQRS - Command Query Request Segregation
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Followers
{
    //This will handle Following flag to either follow or unfollow a user.
    //To Follow -> Add record to UserFollowing
    //To Unfollow -> Remove record from UserFollowing
    public class FollowToggle
    {
        #region Command Section
        public class Command : IRequest<Result<Unit>>
        {
            public string TargetUsername { get; set; }
        }
        #endregion Command Section

        #region Request Section
        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly IUserAccessor _userAccessor;

            public Handler(DataContext context, IUserAccessor userAccessor)
            {
                _context = context;
                _userAccessor = userAccessor;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                //1. Get our users. User doing this action is the observer.
                //2. Get the target from the command
                //3. Check the following tag and toggle it.
                var observerUsername = _userAccessor.GetUserName();
                var observer = await _context.Users.FirstOrDefaultAsync(o => o.UserName == observerUsername);

                if (observer == null) return null;


                var target = await _context.Users.FirstOrDefaultAsync(t => t.UserName == request.TargetUsername);
                if (target == null) return null;

                var following = await _context.UserFollowings.FindAsync(observer.Id, target.Id);

                if (following == null)
                {
                    following = new Domain.UserFollowing
                    {
                        Observer = observer,
                        Target = target
                    };
                    await _context.UserFollowings.AddAsync(following);
                }
                else
                {
                    _context.UserFollowings.Remove(following);
                }
                var success = await _context.SaveChangesAsync() > 0;

                if (success) return Result<Unit>.Success(Unit.Value);

                return Result<Unit>.Failure("Failed to update following");
            }
            #endregion Request Section
        }
    }
}
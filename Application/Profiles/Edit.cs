using Application.Core;
using Application.Interfaces;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Profiles
{
    public class Edit
    {
        public class Command : IRequest<Result<Unit>>
        {
            public string DisplayName { get; set; }
            public string Bio { get; set; }
        }

        //To validate fields of Profile before saving.
        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.DisplayName).NotEmpty();
            }

        }

        public class Request : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly IUserAccessor _userAccessor;

            public Request(DataContext context, IUserAccessor userAccessor)
            {
                _userAccessor = userAccessor;
                _context = context;

            }
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                //1. Find the User
                var user = await _context.Users
                    .FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUserName());

                if (user == null) Result<Unit>.Failure("User not found");

                //2. Update User Bio and Displayname
                user.Bio = request.Bio ?? user.Bio;
                user.DisplayName = request.DisplayName ?? user.DisplayName;

                var result = await _context.SaveChangesAsync() > 0;

                if (result) return Result<Unit>.Success(Unit.Value);

                return Result<Unit>.Failure("Problem updating profile");

            }
        }
    }
}
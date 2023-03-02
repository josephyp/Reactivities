using Application.Core;
using AutoMapper;
using Domain;
using FluentValidation;
using MediatR;
using Persistence;

namespace Application.Activities
{
    public class Edit
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Activity Activity { get; set; }

        }
        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Activity).SetValidator(new ActivityValidator());
            }

        }
        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                _context = context;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                //get our activity, update each of the fields
                var activity = await _context.Activities.FindAsync(request.Activity.Id);
                if (activity == null) return null;

                //Lets check with one property. Later we will see a different approach to update all fields
                //activity.Title = request.Activity.Title ?? activity.Title;
                _mapper.Map(request.Activity, activity); //Do not forget to add AutoMapper as Services in Program.cs

                var result = await _context.SaveChangesAsync() > 0;

                if (!result) return Result<Unit>.Failure("Failed to update the activity");

                return Result<Unit>.Success(Unit.Value);

            }
        }
    }
}
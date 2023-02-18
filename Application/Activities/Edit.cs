using AutoMapper;
using Domain;
using MediatR;
using Persistence;

namespace Application.Activities
{
    public class Edit
    {
        public class Command : IRequest
        {
            public Activity Activity { get; set; }

        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                _context = context;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                //get our activity, update each of the fields
                var activity = await _context.Activities.FindAsync(request.Activity.Id);
                //Lets check with one property. Later we will see a different approach to update all fields
                //activity.Title = request.Activity.Title ?? activity.Title;
                _mapper.Map(request.Activity, activity); //Do not forget to add AutoMapper as Services in Program.cs

                await _context.SaveChangesAsync();

                return Unit.Value;

            }
        }
    }
}
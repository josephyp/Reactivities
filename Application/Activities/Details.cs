using Domain;
using MediatR;
using Persistence;

namespace Application.Activities
{
    public class Details
    {
        //Add Query Class
        public class Query : IRequest<Activity>
        {
            public Guid Id { get; set; }
        }

        //Add Handler Class
        public class Handler : IRequestHandler<Query, Activity>
        {
            private readonly DataContext _context;
            public Handler(DataContext context)
            {
                _context = context;

            }
            public async Task<Activity> Handle(Query request, CancellationToken cancellationToken)
            {
                return await _context.Activities.FindAsync(request.Id);
            }
        }
    }
}
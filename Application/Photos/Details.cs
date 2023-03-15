using Application.Core;
using Application.Profiles;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Photos
{
    public class Details
    {
        public class Query : IRequest<Result<Profiles.Profile>>
        {
            public string Username { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<Profiles.Profile>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                _context = context;

            }
            public async Task<Result<Profiles.Profile>> Handle(Query request, CancellationToken cancellationToken)
            {
                //1. Get user
                var user = await _context.Users.ProjectTo<Profiles.Profile>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(x => x.Username == request.Username);

                return Result<Profiles.Profile>.Success(user);
            }
        }
    }
}
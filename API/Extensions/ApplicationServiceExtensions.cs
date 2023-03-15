using Application.Activities;
using Application.Core;
using Application.Interfaces;
using FluentValidation;
using FluentValidation.AspNetCore;
using Infrastructure.Photos;
using Infrastructure.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        //this > denotes the class that is being extended and will be the first parameter in the extension method. The method needs to be static. 
        public static IServiceCollection AddApplicationservices(this IServiceCollection services, IConfiguration config)
        {
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            //services.AddLogging();
            services.AddDbContext<DataContext>(opt =>
            {
                opt.UseSqlite(config.GetConnectionString("DefaultConnection")); //We need Connection string to connect to SQLite which we can provide in appsettings.[Development].json configuration file(s)
            });

            //CorsPolicy
            services.AddCors(opt =>
            {
                opt.AddPolicy("CorsPolicy", policy =>
                {
                    policy.AllowAnyMethod().AllowAnyHeader().WithOrigins("http://localhost:3000");
                });
            });

            //Add Mediator. We need to also tell where the handlers are located. Let's give a type so it know where to find that and all our types will be in that assembly.
            services.AddMediatR(typeof(List.Handler));
            //Add AutoMapper
            services.AddAutoMapper(typeof(MappingProfiles).Assembly);
            //Add validation service to do auto validation and the assebly to look into in the next statement.
            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<Create>();

            //HttpContext Services and Infra Services
            services.AddHttpContextAccessor();
            services.AddScoped<IUserAccessor, UserAccessor>();
            //Cloudinary settings
            services.Configure<CloudinarySettings>(config.GetSection("Cloudinary"));
            services.AddScoped<IPhotoAccessor, PhotoAccessor>();

            return services;

        }
    }
}
using System.Text;
using API.Services;
using Domain;
using Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Persistence;

namespace API.Extensions
{
    public static class IdentityServiceExtensions
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
        {
            //We will use AddIdentityCode for particular user
            services.AddIdentityCore<AppUser>(opt =>
            {
                //opt gives us options to work on different Identity properties
                opt.Password.RequireNonAlphanumeric = false;
            })
            //Allows us to query our users in our database
            .AddEntityFrameworkStores<DataContext>();

            //Let's update the authentication after jwt token
            //var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("super secret key")); //This key should be same as the one in Token Service. We will have a better solution later. but for now lets hardcode the key
            //updated above statement to get this from appsettings
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opt =>
                {
                    opt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = key,
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                    //For signalR
                    opt.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];//This needs to be access_token. Dont be creative here
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) && (path.StartsWithSegments("/chat")))
                            {
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        }
                    };
                }); //We also need middleware for this.



            services.AddAuthorization(opt =>
            {
                opt.AddPolicy("IsActivityHost", policy =>
                {
                    policy.Requirements.Add(new IsHostRequirement());
                });
            });
            services.AddTransient<IAuthorizationHandler, IsHostRequirementHandler>();
            //AddScoped - Will scope this to the request. and will be disposed when the request is finished. Generally used
            //AddTransient - Will scope the service to a method. Too Short
            //AddSingleton - Will scope the service for Application life. Too Long
            services.AddScoped<TokenService>();
            return services;
        }
    }
}
using API.Extensions;
using API.Middleware;
using API.SignalR;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//We will add auth policy here
builder.Services.AddControllers(opt =>
{
    //create policy
    var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
    //add policy to filters
    opt.Filters.Add(new AuthorizeFilter(policy));

});
//Moved the service additions to Extension class API>Extensions>ApplicationServiceExtensions.cs
builder.Services.AddApplicationservices(builder.Configuration);
//Add Identity services extension
builder.Services.AddIdentityServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection(); //Ours is a Single Page Application
//Cors Policy 
app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization(); //For Authentication / Authorization

app.MapControllers(); //Registers those endpoints 
//Map the ChatHub
app.MapHub<ChatHub>("/chat"); //API.SignalR and the root to direct the users to when they connect to our chat hub. Let's then authenticate

//Run migration 
using var scope = app.Services.CreateScope();
//we can see our services in the scope's service provider
var services = scope.ServiceProvider;

try
{
    var context = services.GetRequiredService<DataContext>();
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    //context.Database.Migrate(); //Lets use ascync method
    await context.Database.MigrateAsync();
    //Let's also seed the data
    await Seed.SeedDataAsync(context, userManager);
}
catch (Exception ex)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occured during migration");
}

app.Run(); //

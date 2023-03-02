using API.Extensions;
using API.Middleware;
using Microsoft.EntityFrameworkCore;
using Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
//Moved the service additions to Extension class API>Extensions>ApplicationServiceExtensions.cs
builder.Services.AddApplicationservices(builder.Configuration);

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

app.UseAuthorization(); //For Authentication / Authorization

app.MapControllers(); //Registers those endpoints 

//Run migration 
using var scope = app.Services.CreateScope();
//we can see our services in the scope's service provider
var services = scope.ServiceProvider;

try
{
    var context = services.GetRequiredService<DataContext>();
    //context.Database.Migrate(); //Lets use ascync method
    await context.Database.MigrateAsync();
    //Let's also seed the data
    await Seed.SeedDataAsync(context);
}
catch (Exception ex)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occured during migration");
}

app.Run(); //

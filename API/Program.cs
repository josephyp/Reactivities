using Microsoft.EntityFrameworkCore;
using Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Services.AddLogging();
builder.Services.AddDbContext<DataContext>(opt =>
{
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")); //We need Connection string to connect to SQLite which we can provide in appsettings.[Development].json configuration file(s)
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection(); //Ours is a Single Page Application

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

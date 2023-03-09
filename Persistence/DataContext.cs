using Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    public class DataContext : IdentityDbContext<AppUser>
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        //Create DbSets (Tables from Domain)
        public DbSet<Activity> Activities { get; set; } //Activity is from Domain project and not from System.Diagnostics

    }
}
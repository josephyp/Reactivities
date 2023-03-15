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

        public DbSet<ActivityAttendee> ActivityAttendees { get; set; }
        public DbSet<Photo> Photos { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //we have now access to entity configurations.
            //Lets set the primary key
            builder.Entity<ActivityAttendee>(x => x.HasKey(aa => new { aa.AppUserId, aa.ActivityId }));

            //Let's also set FK relation. 
            //For AppUserID
            builder.Entity<ActivityAttendee>()
            .HasOne(u => u.AppUser) //has one related record in AppUser
            .WithMany(a => a.Activities) //and many in Activities
            .HasForeignKey(aa => aa.AppUserId); //for this field

            //For ActivityId
            builder.Entity<ActivityAttendee>()
            .HasOne(a => a.Activity) //has one related record in Activity
            .WithMany(u => u.Attendees) //has many attendees
            .HasForeignKey(aa => aa.ActivityId); //for this field

        }

    }
}
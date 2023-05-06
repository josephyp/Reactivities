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
        public DbSet<Comment> Comments { get; set; }

        public DbSet<UserFollowing> UserFollowings { get; set; }

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

            //For Comments with Activity
            builder.Entity<Comment>()
            .HasOne(a => a.Activity)
            .WithMany(c => c.Comments)
            .OnDelete(DeleteBehavior.Cascade); //Cascade makes sure

            //For UserFollowing with AppUser(Section 20)
            //Key is the unique combination of ObserverId and TargetId
            //An Observer will have many Followings
            //A Target will have many Followers.
            builder.Entity<UserFollowing>(b =>
            {
                b.HasKey(k => new { k.ObserverId, k.TargetId });

                b.HasOne(o => o.Observer)
                    .WithMany(f => f.Followings)
                    .HasForeignKey(o => o.ObserverId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(t => t.Target)
                    .WithMany(f => f.Followers)
                    .HasForeignKey(t => t.TargetId)
                    .OnDelete(DeleteBehavior.Cascade);

            });


        }

    }
}
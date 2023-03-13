using Application.Profiles;

namespace Application.Activities
{
    //Shares almost the same properties as Domain.Activity, but with Profiles
    public class ActivityDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string City { get; set; }
        public string Venue { get; set; }  //If you are getting Nonnullable wiggly then set the Nullable property value to false in .csproj file
        public string HostUsername { get; set; } //To identify Host attendee (profile)
        public bool IsCancelled { get; set; }
        public ICollection<Profile> Attendees { get; set; } = new List<Profile>();
    }
}
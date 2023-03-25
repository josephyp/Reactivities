//Remove unecessary usings. Click Ctrl+. for bulb to popup and take the action.
using System.Collections;

namespace Domain
{
    public class Activity
    {
        //Add Properties    prop and tab
        public Guid Id { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string City { get; set; }
        public string Venue { get; set; }  //If you are getting Nonnullable wiggly then set the Nullable property value to false in Domain.csproj file
        public bool IsCancelled { get; set; }
        public ICollection<ActivityAttendee> Attendees { get; set; } = new List<ActivityAttendee>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
using System.Runtime.Serialization;

namespace Courses.Core.Models.LiveSessions
{
    public enum LiveSessionStatus
    {
        [EnumMember(Value = "Scheduled")]
        Scheduled,
        [EnumMember(Value = "Live")]
        Live,
        [EnumMember(Value = "Ended")]
        Ended
    }
}

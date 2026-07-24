using System.Runtime.Serialization;

namespace Courses.Core.Models.Instructors
{
    public enum InstructorStatus
    {
        [EnumMember(Value = "Pending")]
        Pending = 0,
        [EnumMember(Value = "Approved")]
        Approved = 1,
        [EnumMember(Value = "Rejected")]
        Rejected = 2,
        [EnumMember(Value = "Suspended")]
        Suspended = 3
    }
}

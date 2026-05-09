using System.Runtime.Serialization;

namespace Courses.Core.Models.Instructors
{
    public enum InstructorRequestStatus
    {
        [EnumMember(Value = "Pending")]
        Pending,

        [EnumMember(Value = "Approved")]
        Approved,

        [EnumMember(Value = "Rejected")]
        Rejected
    }
}

using System.Runtime.Serialization;

namespace Courses.Core.Models.Courses
{
    public enum CourseStatus
    {
        [EnumMember(Value = "Draft")]
        Draft,
        [EnumMember(Value = "Published")]
        Published,
        [EnumMember(Value = "Pending Review")]
        PendingReview,
        [EnumMember(Value = "Suspended")]
        Suspended
    }
}

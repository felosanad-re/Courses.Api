using System.Runtime.Serialization;

namespace Courses.Core.Models.Courses
{
    /// <summary>
    /// This Enum Show if course Is Online Or Recorded
    /// </summary>
    public enum CourseStatus
    {
        [EnumMember(Value = "Online Course") ]
        OnlineCourse,

        [EnumMember(Value = "Recorder Course") ]
        RecorderCourse
    }
}

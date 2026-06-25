using System.Runtime.Serialization;

namespace Courses.Core.Models.Courses
{
    /// <summary>
    /// This Enum Show if course Is Online Or Recorded
    /// </summary>
    public enum CourseType
    {
        [EnumMember(Value = "Online Course") ]
        OnlineCourse,

        [EnumMember(Value = "Recorder Course") ]
        RecorderCourse
    }
}

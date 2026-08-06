using System.Runtime.Serialization;

namespace Courses.Core.Models.ApplicationUsers
{
    public enum AccountStatus
    {
        [EnumMember(Value = "Active")]
        Active,
        [EnumMember(Value = "Suspended")]
        Suspended
    }
}

using System.Runtime.Serialization;

namespace Courses.Core.ModelsDTO.ResponseDTO.Enrollment
{
    public enum EnrollStatus
    {
        [EnumMember(Value = "Pending Payment")]
        PendingPayment = 0,

        [EnumMember(Value = "Course Active")]
        Active = 1,

        [EnumMember(Value = "Payment Succeeded")]
        PaymentSucceeded = 2,

        [EnumMember(Value = "Payment Failed")]
        PaymentFailed = 3,

        [EnumMember(Value = "Payment Cancelled")]
        PaymentCancelled = 4
    }
}

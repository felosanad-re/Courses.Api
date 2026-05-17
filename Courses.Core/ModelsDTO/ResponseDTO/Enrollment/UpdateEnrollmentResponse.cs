namespace Courses.Core.ModelsDTO.ResponseDTO.Enrollment
{
    public class UpdateEnrollmentResponse
    {
        public EnrollStatus Status { get; set; }
        public string? PaymentIntentId { get; set; }
        public int EnrollmentId { get; set; }
    }
}

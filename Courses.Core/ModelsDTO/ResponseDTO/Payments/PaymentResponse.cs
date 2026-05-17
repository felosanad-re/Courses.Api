namespace Courses.Core.ModelsDTO.ResponseDTO.Payments
{
    public class PaymentResponse
    {
        public string PaymentIntentId { get; set; } = string.Empty;

        public string? ClientSecret { get; set; }
        public int EnrollmentId { get; set; }
        public int CourseId { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}

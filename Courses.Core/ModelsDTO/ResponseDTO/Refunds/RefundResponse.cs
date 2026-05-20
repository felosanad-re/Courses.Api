namespace Courses.Core.ModelsDTO.ResponseDTO.Refunds
{
    public class RefundResponse
    {
        public string PaymentIntentId { get; set; } = string.Empty;

        public string? ClientSecret { get; set; }
        public int EnrollmentId { get; set; }
        public int CourseId { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? CancellationReason { get; set; }
    }
}

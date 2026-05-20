using System.ComponentModel.DataAnnotations;

namespace Courses.Core.ModelsDTO.RequestDTO.Refunds
{
    public class RefundRequest
    {
        [Required]
        public int EnrollmentId { get; set; }
        public string? CancellationReason { get; set; }
    }
}

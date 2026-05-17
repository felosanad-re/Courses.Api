using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO.Enrollments;
using Courses.Core.ModelsDTO.ResponseDTO.Enrollment;

namespace Courses.Core.Services.Contract.EnrollmentServices
{
    public interface IEnrollmentService
    {
        Task<ApplicationServiceResult<EnrollmentWithCourseResponse>> CreateEnrollmentAsync(EnrollmentRequest req);


        // Update Enrollment Status After Payment
        Task<ApplicationServiceResult<UpdateEnrollmentResponse>> UpdateEnrollmentStatusAsync(string paymentIntentId, EnrollStatus status);
    }
}

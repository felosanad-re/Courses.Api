using Courses.Api.ErrorHandler;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO.Enrollments;
using Courses.Core.ModelsDTO.ResponseDTO.Enrollment;
using Courses.Core.Services.Contract.EnrollmentServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Courses.Api.Controllers.Enrollment
{
    public class EnrollmentController : BaseController
    {
        #region
        protected readonly IEnrollmentService _enrollmentService;

        public EnrollmentController(IEnrollmentService enrollmentService)
        {
            _enrollmentService = enrollmentService;
        }
        #endregion

        #region Create Enrollment
        [HttpPost("CreateEnrollment")] // Post: /api/Enrollment/CreateEnrollment
        public async Task<ActionResult<ApplicationServiceResult<EnrollmentWithCourseResponse>>> CreateEnrollment(EnrollmentRequest req)
        {
            var result = await _enrollmentService.CreateEnrollmentAsync(req);
            if (!result.Succeed) return BadRequest(new ErrorResponse(400) { Message = [result.Message] });

            return Ok(result);
        }
        #endregion
    }
}

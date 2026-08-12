using Courses.Api.ErrorHandler;
using Courses.Core;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO.Instructors;
using Courses.Core.ModelsDTO.ResponseDTO.Instructors;
using Courses.Core.Services.Contract.InstructorServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Courses.Api.Controllers.Instructors
{
    [Authorize(Roles = Roles.Student)]
    public class InstructorRequestController : BaseController
    {
        #region Inject Services
        protected readonly IInstructorRequestService _instructorRequestService;

        public InstructorRequestController(IInstructorRequestService instructorRequestService)
        {
            _instructorRequestService = instructorRequestService;
        }
        #endregion

        #region Apply Instructor Request
        /// <summary>
        /// Apply to become an instructor (Student only)
        /// </summary>
        [HttpPost("Apply")] // POST: /api/InstructorRequest/Apply/{instructorId}
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApplicationServiceResult<ApplyInstructorResponse>>> Apply([FromBody] ApplyInstructorRequest req)
        {
            var result = await _instructorRequestService.ApplyInstructorRequest(req);
            if (!result.Succeed)
                return BadRequest(new ErrorResponse(400) { Message = [result.Message] });
            return Ok(result);
        }
        #endregion
    }
}
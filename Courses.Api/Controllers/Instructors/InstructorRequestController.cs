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
    [Authorize(Roles = Roles.Admin)]
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
        [HttpPost("Apply")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<ApplicationServiceResult<ApplyInstructorResponse>>> Apply([FromBody] ApplyInstructorRequest req)
        {
            var result = await _instructorRequestService.ApplyInstructorRequest(req);
            if (!result.Succeed)
            {
                return BadRequest(new ErrorResponse(400)
                {
                    Message = [result.Message]
                });
            }
            return Ok(result);
        }
        #endregion

        #region Get All Requests
        /// <summary>
        /// Get all instructor requests (Admin only)
        /// </summary>
        [HttpGet("All")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApplicationServiceResult<IReadOnlyList<ApplyInstructorResponse>>>> GetAll()
        {
            var result = await _instructorRequestService.GetAllRequests();
            if (!result.Succeed)
            {
                return BadRequest(new { message = result.Message });
            }
            return Ok(result);
        }
        #endregion

        #region Approve Request
        /// <summary>
        /// Approve an instructor request (Admin only)
        /// </summary>
        [HttpPut("Approve/{reqId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApplicationServiceResult<ApplyInstructorResponse>>> Approve(int reqId)
        {
            var result = await _instructorRequestService.GetApproveRequest(reqId);
            if (!result.Succeed)
            {
                return BadRequest(new ErrorResponse(400)
                {
                    Message = [result.Message]
                });
            }
            return Ok(result);
        }
        #endregion

        #region Reject Request
        /// <summary>
        /// Reject an instructor request (Admin only)
        /// </summary>
        [HttpPut("Reject/{reqId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApplicationServiceResult<ApplyInstructorResponse>>> Reject(int reqId)
        {
            var result = await _instructorRequestService.GetRejectRequest(reqId);
            if (!result.Succeed)
            {
                return BadRequest(new ErrorResponse(400)
                {
                    Message = [result.Message]
                });
            }
            return Ok(result);
        }
        #endregion
    }
}
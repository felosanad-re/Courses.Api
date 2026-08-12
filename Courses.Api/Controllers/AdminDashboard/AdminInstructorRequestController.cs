using Courses.Api.ErrorHandler;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.ResponseDTO.Instructors;
using Courses.Core.Services.Contract.AdminDashboardServices;
using Courses.Core.Specifications.InstructorRequestSpecifications;
using Microsoft.AspNetCore.Mvc;

namespace Courses.Api.Controllers.AdminDashboard
{
    public class AdminInstructorRequestController : BaseController
    {
        protected readonly IAdminInstructorRequest _adminInstructorRequest;

        public AdminInstructorRequestController(IAdminInstructorRequest adminInstructorRequest)
        {
            _adminInstructorRequest = adminInstructorRequest;
        }

        #region Get All Requests
        [HttpGet] // GET: /api/AdminInstructorRequest
        public async Task<ActionResult<ApplicationServiceResult<ApplyInstructorResponse>>> GetAllRequests([FromQuery]InstructorRequestParams param)
        {
            var result = await _adminInstructorRequest.GetAllRequests(param);
            if (!result.Succeed)
                return BadRequest(new ErrorResponse(400) { Message = [result.Message] });
            return Ok(result);
        }
        #endregion

        #region Get Request
        [HttpGet("{reqId}")] // GET: /api/AdminInstructorRequest/{reqId}
        public async Task<ActionResult<ApplicationServiceResult<ApplyInstructorResponse>>> GetRequest(int reqId)
        {
            var result = await _adminInstructorRequest.GetRequestDetails(reqId);
            if (!result.Succeed)
                return BadRequest(new ErrorResponse(400) { Message = [result.Message] });
            return Ok(result);
        }
        #endregion

        #region Approve
        [HttpPost("Approve/{reqId}")] // Post: /api/AdminInstructorRequest/Approve/{reqId}
        public async Task<ActionResult<ApplicationServiceResult<bool>>> Approve(int reqId)
        {
            var result = await _adminInstructorRequest.ApproveRequest(reqId);
            if (!result.Succeed)
                return BadRequest(new ErrorResponse(400) { Message = [result.Message] });
            return Ok(result);
        }
        #endregion

        #region Rejected
        [HttpPost("Rejected/{reqId}")] // Post: /api/AdminInstructorRequest/Rejected/{reqId}
        public async Task<ActionResult<ApplicationServiceResult<bool>>> Rejected(int reqId)
        {
            var result = await _adminInstructorRequest.RejectRequest(reqId);
            if (!result.Succeed)
                return BadRequest(new ErrorResponse(400) { Message = [result.Message] });
            return Ok(result);
        }
        #endregion
    }
}

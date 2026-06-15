using Courses.Api.ErrorHandler;
using Courses.Core;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO.Enrollments;
using Courses.Core.ModelsDTO.ResponseDTO.Instructors;
using Courses.Core.Services.Contract.EarningServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Courses.Api.Controllers.Earning
{
    [Authorize(Roles = Roles.Instructor)]
    public class EarningController : BaseController
    {
        protected readonly IEarningService _earningService;

        public EarningController(IEarningService earningService)
        {
            _earningService = earningService;
        }

        [HttpGet("Stats")] // GET: /api/Earning/Stats
        public async Task<ActionResult<ApplicationServiceResult<InstructorEarningStatsResponse>>> GetEarningStats([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            var result = await _earningService.GetEarningStatsAsync(fromDate, toDate);
            if (!result.Succeed)
                return BadRequest(new ErrorResponse(400) { Message = [result.Message] });
            return Ok(result);
        }

        [HttpGet("enrollments")] // GET: /api/Earning/enrollments
        public async Task<ActionResult<ApplicationServiceResult<Pagination<InstructorWithEnrollmentsDetails>>>> GetInstructorEnrollment([FromQuery] EnrollmentsParams param)
        {
            var result = await _earningService.GetInstructorEnrollmentsAsync(param);
            if(!result.Succeed)
                return BadRequest(new ErrorResponse(400) { Message= [result.Message]});
            return Ok(result);
        }
    }
}

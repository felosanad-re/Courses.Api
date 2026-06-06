using Courses.Api.ErrorHandler;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.ResponseDTO.DashboardInstructor;
using Courses.Core.Services.Contract.DashboardServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Courses.Api.Controllers.DashboardStats
{
    public class InstructorDashboardController : BaseController
    {
        protected readonly IDashboardInstructorService _dashboardInstructorService;

        public InstructorDashboardController(IDashboardInstructorService dashboardInstructorService)
        {
            _dashboardInstructorService = dashboardInstructorService;
        }

        #region Get Stats
        [HttpGet("Stats")] // GET :/api/InstructorDashboard/Stats
        public async Task<ActionResult<ApplicationServiceResult<DashboardInstructorDTO>>> GetStats()
        {
            var result = await _dashboardInstructorService.GetDashboardInstructStatsAsync();
            if (!result.Succeed)
                return BadRequest(new ErrorResponse(400) { Message = [result.Message] });
            return Ok(result);
        }
        #endregion
    }
}

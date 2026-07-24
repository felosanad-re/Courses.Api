using Courses.Api.ErrorHandler;
using Courses.Core;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.ResponseDTO.AdminDashboard;
using Courses.Core.Services.Contract.AdminDashboardServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Courses.Api.Controllers.AdminDashboard
{
    [Authorize(Roles = Roles.Admin)]
    public class AdminDashboardController : BaseController
    {
        #region Services
        protected readonly IAdminDashboardService _adminDashboardService;

        public AdminDashboardController(IAdminDashboardService adminDashboardService)
        {
            _adminDashboardService = adminDashboardService;
        }
        #endregion

        #region Get Stats
        [HttpGet("Stats")] // GET: /api/AdminDashboard/Stats
        public async Task<ActionResult<ApplicationServiceResult<AdminDashboardStatsResponse>>> GetStats()
        {
            var res = await _adminDashboardService.GetStatsAsync();
            if(!res.Succeed)
                return BadRequest(new ErrorResponse(400) { Message = [res.Message] });

            return Ok(res);
        }
        #endregion

        #region Get Charts
        [HttpGet("Charts")] // GET: /api/AdminDashboard/Charts
        public async Task<ActionResult<ApplicationServiceResult<AdminDashboardStatsResponse>>> GetCharts()
        {
            var res = await _adminDashboardService.GetChartsAsync();
            if (!res.Succeed)
                return BadRequest(new ErrorResponse(400) { Message = [res.Message] });

            return Ok(res);
        }
        #endregion

        #region Get Lasted Reviews
        [HttpGet("Reviews")] // GET: /api/AdminDashboard/Reviews
        public async Task<ActionResult<List<AdminDashboardReviewsResponse>>> GetLastedReviews()
        {
            var res = await _adminDashboardService.GetLatestReviewsAsync();
            if (!res.Succeed)
                return BadRequest(new ErrorResponse(400) { Message = [res.Message] });

            return Ok(res);
        }
        #endregion

        [HttpGet("Actions")] // GET: /api/AdminDashboard/Actions
        public async Task<ActionResult<ApplicationServiceResult<AdminDashboardQuickActionsResponse>>> GetActions()
        {
            var res = await _adminDashboardService.GetQuickActionsAsync();
            if (!res.Succeed)
                return BadRequest(new ErrorResponse(400) { Message = [res.Message] });

            return Ok(res);
        }
    }
}

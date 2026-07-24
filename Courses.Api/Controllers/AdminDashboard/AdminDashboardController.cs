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
        protected readonly IAdminDashboardService _adminDashboardService;

        public AdminDashboardController(IAdminDashboardService adminDashboardService)
        {
            _adminDashboardService = adminDashboardService;
        }

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
    }
}

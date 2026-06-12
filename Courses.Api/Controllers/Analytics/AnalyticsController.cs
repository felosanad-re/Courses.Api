using Courses.Api.ErrorHandler;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO.Analyzer;
using Courses.Core.ModelsDTO.ResponseDTO.Analyses;
using Courses.Core.Services.Contract.AnalyticsServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Courses.Api.Controllers.Analytics
{
    public class AnalyticsController : BaseController
    {
        protected readonly IAnalyzeService _analyzeService;

        public AnalyticsController(IAnalyzeService analyzeService)
        {
            _analyzeService = analyzeService;
        }

        #region Get Analyzer
        [HttpGet("Analyze")] // GET: /api/Analytics/Analyze
        public async Task<ActionResult<ApplicationServiceResult<InstructorAnalyticsDto>>> GetAnalyzer()
        {
            var result = await _analyzeService.GetAnalyzeAsync();
            if (!result.Succeed)
                return BadRequest(new ErrorResponse(400) { Message = [result.Message] });
            return Ok(result);
        }
        #endregion

        #region Analyze Charts
        [HttpGet("AnalyzeCharts")] // GET: /api/Analytics/AnalyzeCharts
        public async Task<ActionResult<ApplicationServiceResult<IReadOnlyList<MonthlyAnalyticsDto>>>> GetAnalyzeCharts([FromQuery] ChartRequest req)
        {
            var result = await _analyzeService.GetInstructorChartsAnalyticsAsync(req);
            if(!result.Succeed)
                return BadRequest(new ErrorResponse(400) { Message = [result.Message] });

            return Ok(result);
        }
        #endregion
    }
}

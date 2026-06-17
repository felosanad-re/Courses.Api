using Courses.Core.ModelsDTO.RequestDTO.Zoom;
using Courses.Core.Services.Contract.ZoomServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Courses.Api.Controllers.Tests
{
    public class TestZoomController : BaseController
    {
        private readonly IZoomService _zoomService;

        public TestZoomController(IZoomService zoomService)
        {
            _zoomService = zoomService;
        }

        [HttpPost("test-meeting")] // POST: /api/TestZoom/test-meeting
        public async Task<IActionResult> TestMeeting()
        {
            var req = new ZoomRequest
            {
                Topic = "C#",
                Duration = 60,
                StartTime = DateTime.UtcNow.AddMinutes(3)
            };

            var result = await _zoomService.CreateMeetingAsync(req);

            return Ok(result);
        }
    }
}

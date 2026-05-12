using Courses.Api.ErrorHandler;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.ResponseDTO.CoursesTypes;
using Courses.Core.Services.Contract.CourseTypeServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Courses.Api.Controllers.CourseTypes
{
    public class CourseTypesController : BaseController
    {
        protected readonly ICourseTypeService _courseTypeService;

        public CourseTypesController(ICourseTypeService courseTypeService)
        {
            _courseTypeService = courseTypeService;
        }

        [HttpGet("Types")] // GET: api/courseTypes/Types
        public async Task<ActionResult<ApplicationServiceResult<IReadOnlyList<CourseTypeToReturnDTO>>>> GetAllAsync()
        {
            var result = await _courseTypeService.GetAllTypesAsync();
            if (!result.Succeed) return BadRequest(new ErrorResponse(400) { Message = [result.Message] });

            return Ok(result);
        }
    }
}

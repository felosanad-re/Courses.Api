using Courses.Api.ErrorHandler;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.ResponseDTO.CoursesCategories;
using Courses.Core.Services.Contract.CourseCategoriesServices;
using Microsoft.AspNetCore.Mvc;

namespace Courses.Api.Controllers.CourseCategories
{
    public class CourseCategoriesController : BaseController
    {
        protected readonly ICourseCategoryService _courseCategoryService;

        public CourseCategoriesController(ICourseCategoryService courseCategoryService)
        {
            _courseCategoryService = courseCategoryService;
        }

        [HttpGet("Categories")] // GET: api/CourseCategories/Categories
        public async Task<ActionResult<ApplicationServiceResult<IReadOnlyList<CourseCategoryToReturnDTO>>>> GetAllAsync()
        {
            var result = await _courseCategoryService.GetAllCategoriesAsync();
            if (!result.Succeed) return BadRequest(new ErrorResponse(400) { Message = [result.Message] });

            return Ok(result);
        }
    }
}

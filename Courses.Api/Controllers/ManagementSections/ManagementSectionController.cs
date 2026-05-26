using Courses.Api.ErrorHandler;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO.Sections;
using Courses.Core.ModelsDTO.ResponseDTO.Sections;
using Courses.Core.Services.Contract.ManagementCourses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Courses.Api.Controllers.ManagementSections
{
    public class ManagementSectionController : BaseController
    {
        protected readonly IManagementSection _managementSection;

        public ManagementSectionController(IManagementSection managementSection)
        {
            _managementSection = managementSection;
        }

        #region Create Section
        [HttpPost("Create")] // POST: /api/ManagementSection/Create
        public async Task<ActionResult<ApplicationServiceResult<SectionWithCourseResponse>>> CreateSection(CreateSectionRequest req)
        {
            var result = await _managementSection.CreateSectionAsync(req);
            if (!result.Succeed && result.Data == null) 
                return BadRequest(new ErrorResponse(400));

            return Ok(result);
        }
        #endregion

        #region Update Section
        [HttpPut("Update")] // POST: /api/ManagementSection/Update
        public async Task<ActionResult<ApplicationServiceResult<SectionWithCourseResponse>>> UpdateSection(UpdateSectionRequest req)
        {
            var result = await _managementSection.UpdateSectionAsync(req);
            if (!result.Succeed && result.Data == null) 
                return BadRequest(new ErrorResponse(400));

            return Ok(result);
        }
        #endregion

        #region Delete Section
        [HttpDelete("{id}")] // DELETE: /api/ManagementSection/Id
        public async Task<ActionResult<ApplicationServiceResult<DeleteSectionResponse>>> DeleteSection(int id)
        {
            var result = await _managementSection.DeleteSectionAsync(id);
            if (!result.Succeed)
                return BadRequest(new ErrorResponse(400) { Message = [result.Message] });

            return Ok(result);
        }
        #endregion

        #region
        [HttpPost("multi-delete")] // POST: /api/ManagementSection/multi-delete
        public async Task<ActionResult<ApplicationServiceResult<DeleteSectionResponse>>> MultiDeleteSections([FromBody]IEnumerable<int> ids)
        {
            var res = await _managementSection.DeleteMultiSections(ids);
            if(!res.Succeed) return BadRequest(new ErrorResponse(400) { Message = [res.Message] });

            return Ok(res);
        }
        #endregion
    }
}

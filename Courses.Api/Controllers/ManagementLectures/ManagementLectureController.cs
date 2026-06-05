using Courses.Api.ErrorHandler;
using Courses.Core;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO.Lectures;
using Courses.Core.ModelsDTO.ResponseDTO.Lectures;
using Courses.Core.Services.Contract.ManagementCourses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Courses.Api.Controllers.ManagementLectures
{
    [Authorize(Roles = Roles.Instructor)]
    public class ManagementLectureController : BaseController
    {
        #region DI Services
        protected readonly IManagementLecture _managementLecture;

        public ManagementLectureController(IManagementLecture managementLecture)
        {
            _managementLecture = managementLecture;
        }
        #endregion

        #region Get Lecture
        [HttpGet("{id}")] // GET: /api/ManagementLecture/id
        public async Task<ActionResult<ApplicationServiceResult<LectureWithInstructorResponse>>> GetLecture(int id)
        {
            var result = await _managementLecture.GetLectureAsync(id);
            if (!result.Succeed) return BadRequest(new ErrorResponse(400) { Message = [result.Message] });

            return Ok(result);
        }
        #endregion

        #region CreateLecture
        [HttpPost("Create")] // POST: /api/ManagementLecture/Create
        public async Task<ActionResult<ApplicationServiceResult<LectureWithInstructorResponse>>> CreateLecture(CreatedLectureRequest req)
        {
            var result = await _managementLecture.CreateLectureAsync(req);
            if (!result.Succeed)
                return BadRequest(new ErrorResponse(400));

            return Ok(result);
        }
        #endregion

        #region Update Lecture
        [HttpPut("Update")] // PUT: /api/ManagementLecture/Update
        public async Task<ActionResult<ApplicationServiceResult<LectureWithInstructorResponse>>> UpdateLecture(UpdatedLectureRequest req)
        {
            var result = await _managementLecture.UpdateLectureAsync(req);
            if (!result.Succeed)
                return BadRequest(new ErrorResponse(400));

            return Ok(result);
        }
        #endregion

        #region Delete Lecture
        [HttpDelete("{id}")] //DELETE: /api/ManagementLecture/id
        public async Task<ActionResult<ApplicationServiceResult<LectureDeletedResponse>>> DeleteLecture(int id)
        {
            var result = await _managementLecture.DeleteLectureAsync(id);
            if (!result.Succeed)
                return BadRequest(new ErrorResponse(400));

            return Ok(result);
        }
        #endregion

        #region Delete-Multi Lecture
        [HttpPost("Multi-Deleted")] // POST: /api/ManagementLecture/Multi-Deleted
        public async Task<ActionResult<ApplicationServiceResult<LectureDeletedResponse>>> DeleteMultiLecture([FromBody]IEnumerable<int> ids)
        {
            var result = await _managementLecture.DeleteMultiLectureAsync(ids);
            if (!result.Succeed)
                return BadRequest(new ErrorResponse(400));

            return Ok(result);
        }
        #endregion
    }
}

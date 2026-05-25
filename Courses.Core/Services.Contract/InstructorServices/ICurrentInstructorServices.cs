using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.ResponseDTO.Instructors;

namespace Courses.Core.Services.Contract.InstructorServices
{
    public interface ICurrentInstructorServices
    {
        Task<ApplicationServiceResult<InstructorWithApplicationUserResponse>> CurrentInstructor();
    }
}

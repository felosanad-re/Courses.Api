using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.ResponseDTO.Students;

namespace Courses.Core.Services.Contract.StudentServices
{
    public interface ICurrentStudentService
    {
        Task<ApplicationServiceResult<StudentWithApplicationUserToReturnDTO>> GetStudentWithApplicationUser();
    }
}

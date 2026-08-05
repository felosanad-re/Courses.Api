using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO.Students;
using Courses.Core.ModelsDTO.ResponseDTO.AdminDashboard;

namespace Courses.Core.Services.Contract.AdminDashboardServices
{
    public interface IAdminManagementStudents
    {
        // Get All Students
        Task<ApplicationServiceResult<Pagination<AdminWithStudentResponse>>> GetStudentsAsync(StudentParams param);

        // Get Student Details
        Task<ApplicationServiceResult<AdminWithStudentDetailsResponse>> GetStudentDetailsAsync(int studentId);

    }
}

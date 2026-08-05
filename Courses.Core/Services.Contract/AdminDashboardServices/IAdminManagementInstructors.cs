using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO.Instructors;
using Courses.Core.ModelsDTO.ResponseDTO.AdminDashboard;

namespace Courses.Core.Services.Contract.AdminDashboardServices
{
    public interface IAdminManagementInstructors
    {
        Task<ApplicationServiceResult<Pagination<AdminInstructorResponse>>> GetAllInstructorsAsync(InstructorParams param);

        Task<ApplicationServiceResult<AdminInstructorDetailsResponse>> GetInstructorDetailsAsync(int instructorId);
    }
}

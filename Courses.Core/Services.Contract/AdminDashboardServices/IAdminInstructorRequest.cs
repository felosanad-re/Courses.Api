using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.ResponseDTO.Instructors;
using Courses.Core.Specifications.InstructorRequestSpecifications;

namespace Courses.Core.Services.Contract.AdminDashboardServices
{
    public interface IAdminInstructorRequest
    {
        Task<ApplicationServiceResult<ApplyInstructorResponse>> GetApproveRequest(int reqId);
        Task<ApplicationServiceResult<ApplyInstructorResponse>> GetRejectRequest(int reqId);

        Task<ApplicationServiceResult<Pagination<ApplyInstructorResponse>>> GetAllRequests(InstructorRequestParams param);
    }
}

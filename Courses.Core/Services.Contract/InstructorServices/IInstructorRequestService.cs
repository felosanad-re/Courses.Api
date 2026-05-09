using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO.Instructors;
using Courses.Core.ModelsDTO.ResponseDTO.Instructors;

namespace Courses.Core.Services.Contract.InstructorServices
{
    // The request after user created a new account
    // the request send to admin to approve or reject this request
    public interface IInstructorRequestService
    {
        Task<ApplicationServiceResult<ApplyInstructorResponse>> ApplyInstructorRequest(ApplyInstructorRequest req);

        Task<ApplicationServiceResult<ApplyInstructorResponse>> GetApproveRequest(int reqId);
        Task<ApplicationServiceResult<ApplyInstructorResponse>> GetRejectRequest(int reqId);

        Task<ApplicationServiceResult<IReadOnlyList<ApplyInstructorResponse>>> GetAllRequests();
    }
}

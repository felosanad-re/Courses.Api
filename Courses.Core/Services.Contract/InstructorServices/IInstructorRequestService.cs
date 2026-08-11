using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO.Instructors;
using Courses.Core.ModelsDTO.ResponseDTO.Instructors;
using Courses.Core.Specifications.InstructorRequestSpecifications;

namespace Courses.Core.Services.Contract.InstructorServices
{
    // The request after user created a new account
    // the request send to admin to approve or reject this request
    public interface IInstructorRequestService
    {
        Task<ApplicationServiceResult<ApplyInstructorResponse>> ApplyInstructorRequest(ApplyInstructorRequest req);
    }
}

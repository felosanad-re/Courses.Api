using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO;
using Courses.Core.ModelsDTO.ResponseDTO;

namespace Courses.Core.Services.Contract.AccountServices
{
    public interface IAccountService
    {
        Task<ApplicationServiceResult<CreateAccountResponse>> RegisterAsync(CreateAccountRequest req);
        Task<ApplicationServiceResult<ConfirmAccountResponse>> ConfirmAccount(ConfirmAccountRequest req);
        Task<ApplicationServiceResult<LoginResponse>> LoginAsync(LoginRequest req);
        Task<ApplicationServiceResult<CheckAccountResponse>> CheckAccountAsync(CheckAccountRequest req);
        Task<ApplicationServiceResult<CheckOTPResponse>> CheckOTP(CheckOTPRequest req);
        Task<ApplicationServiceResult<ResetPasswordResponse>> ResetPasswordAsync(ResetPasswordRequest req);
    }
}

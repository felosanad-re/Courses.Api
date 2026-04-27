namespace Courses.Core.ModelsDTO.ResponseDTO
{
    public class CheckAccountResponse
    {
        public bool Exists { get; set; }
        public bool RequiresOTP { get; set; }
        public bool CanResetPassword { get; set; }
        public string Token { get; set; }
    }
}

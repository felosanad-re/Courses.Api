namespace Courses.Core.ModelsDTO.ResponseDTO.Account
{
    public class CheckOTPResponse
    {
        public bool IsValid { get; set; }
        public string Token { get; set; }
    }
}

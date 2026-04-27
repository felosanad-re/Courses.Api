namespace Courses.Core.ModelsDTO.ResponseDTO
{
    public class CheckOTPResponse
    {
        public bool IsValid { get; set; }
        public string Token { get; set; }
    }
}

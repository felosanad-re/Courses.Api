namespace Courses.Core.ModelsDTO.ResponseDTO.Account
{
    public class CheckEmailConfirmationResponse
    {
        public bool Exists { get; set; }
        public bool IsConfirmed { get; set; }
    }
}
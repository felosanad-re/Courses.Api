namespace Courses.Core.ModelsDTO.ResponseDTO.Account
{
    public class LoginResponse
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public bool IsAuthenticated { get; set; }
        public IList<string> Roles { get; set; }
    }
}

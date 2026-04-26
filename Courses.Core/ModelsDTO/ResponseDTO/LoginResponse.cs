namespace Courses.Core.ModelsDTO.ResponseDTO
{
    public class LoginResponse
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public bool IsAuthenticated { get; set; }
        public string[] Roles { get; set; }
    }
}

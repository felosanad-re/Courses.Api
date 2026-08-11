namespace Courses.Core.ModelsDTO.ResponseDTO.Profiles
{
    public class UserProfileResponse
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public DateTime Birthday { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public List<string> UserRoles { get; set; } = new();
    }
}

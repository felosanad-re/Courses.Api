namespace Courses.Core.ModelsDTO.RequestDTO.Profile
{
    public class EditProfileRequest
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public DateTime Birthday { get; set; }
    }
}

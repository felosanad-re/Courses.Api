namespace Courses.Core.Options
{
    public class SeedAdminOptions
    {
        public const string SectionName = "SeedAdmin";

        public bool Enabled { get; set; }
        public string FirstName { get; set; } = "Admin";
        public string LastName { get; set; } = "User";
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}

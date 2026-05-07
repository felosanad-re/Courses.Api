namespace Courses.Core.Services.Contract.UserServices
{
    public interface ICurrentUserService
    {
        public string? UserId { get; }
        public string? Email { get; }
        public string? UserName { get; }
        public IList<string>? Roles { get; }
    }
}

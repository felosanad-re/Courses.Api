namespace Courses.Core.Models.ApplicationUsers
{
    public class OTPUser
    {
        public int OTP { get; set; }
        public string Token { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedAt { get; set; }

        // Check OTP Is Valid or no
        public bool IsExpired(TimeSpan validateOTP)
        {
            return DateTime.UtcNow - CreatedAt > validateOTP;
        }

    }
}

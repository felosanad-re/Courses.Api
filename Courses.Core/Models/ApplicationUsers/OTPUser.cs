namespace Courses.Core.Models.ApplicationUsers
{
    /// <summary>
    /// Represents a One-Time Password sent to a user for verification
    /// (e.g., during registration, password reset, or account confirmation).
    /// Stored in Redis (not SQL) for fast access and automatic expiration.
    /// </summary>
    public class OTPUser
    {

        // The 4-6 digit OTP code sent to the user
        public int OTP { get; set; }

        // Token associated with this OTP request (for correlation)
        public string Token { get; set; }

        // The Identity user this OTP belongs to
        public string UserId { get; set; }

        // When this OTP was generated (used to check expiration)
        public DateTime CreatedAt { get; set; }

        // Whether the user has successfully verified this OTP
        public bool IsVerified { get; set; }

        // Checks if the OTP has exceeded the allowed time window
        public bool IsExpired(TimeSpan validateOTP)
        {
            return DateTime.UtcNow - CreatedAt > validateOTP;
        }
    }
}

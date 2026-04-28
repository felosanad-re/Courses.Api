namespace Courses.Core.Models
{
    /// <summary>
    /// Abstract base class for entities that represent real people (Student, Instructor).
    /// Inherits from BaseModel which provides Id, CreatedBy, CreatedAt, and IsDeleted audit fields.
    /// </summary>
    /// <remarks>
    /// Why this class exists:
    /// - Both Student and Instructor share personal identity fields (Name, Birthday, Age).
    /// - Instead of duplicating these properties in each class, we centralize them here.
    /// - This follows the DRY principle (Don't Repeat Yourself) and ensures consistent
    ///   age calculation logic across all person-based entities.
    /// - Age is computed dynamically from Birthday (not stored in DB) so it's always accurate.
    /// </remarks>
    public abstract class PersonalBase : BaseModel
    {

        // Full name of the person (e.g., "Ahmed Mohamed")
        public string Name { get; set; }

        // Date of birth used to calculate Age dynamically
        public DateTime Birthday { get; set; }

        // Computed age in years based on Birthday relative to today's date.
        // Not stored in the database — always calculated at runtime for accuracy.
        public int Age
        {
            get
            {
                var today = DateTime.UtcNow;
                var age = today.Year - Birthday.Year;
                // If the birthday hasn't occurred yet this year, subtract 1
                if (today.Month < Birthday.Month
                    || (today.Month == Birthday.Month && today.Day < Birthday.Day))
                {
                    age--;
                }
                return age;
            }
        }
    }
}

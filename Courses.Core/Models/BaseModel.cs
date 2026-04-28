namespace Courses.Core.Models
{
    /// <summary>
    /// Base entity for all domain models. Provides common audit fields
    /// that every entity in the system needs (soft delete, creation tracking).
    /// </summary>
    public class BaseModel
    {
        public int Id { get; set; }

        // The user who created this record (audit trail)
        public string CreatedBy { get; set; }
        // Timestamp of creation, defaults to UTC now
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


        // Soft delete flag — when true the record is considered deleted but stays in DB

        public bool IsDeleted { get; set; }
    }
}

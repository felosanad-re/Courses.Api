namespace Courses.Core.Models.Enrollments
{
    /// <summary>
    /// A single lecture/video within a Section. Belongs to one Section (many-to-one).
    /// </summary>
    public class Lecture
    {
        public int Id { get; set; }
        public string Title { get; set; }

        // URL to the lecture video (could be a streaming URL or file path)
        public string VideoUrl { get; set; }


        // The section this lecture belongs to (many-to-one)
        public Section Section { get; set; }
        public int SectionId { get; set; }

        // Tracks which students have completed this lecture
        public ICollection<StudentLectureProgress> StudentProgresses { get; set; } = new HashSet<StudentLectureProgress>();
    }
}

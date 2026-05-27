namespace Courses.Core.ModelsDTO.ResponseDTO.Lectures
{
    public class LectureDeletedResponse
    {
        public string Message { get; set; }

        public int LectureCount { get; set; } // For Multi-Lectures Delete
    }
}

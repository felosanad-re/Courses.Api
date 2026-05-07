namespace Courses.Core.ModelsDTO.ResponseDTO.Courses
{
    public class DeleteCoursesResult
    {
        public List<int> DeletedIds { get; set; }
        public List<int> NotFoundIds { get; set; }
        public List<int> UnauthorizedIds { get; set; }
    }
}

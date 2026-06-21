namespace Courses.Core.ModelsDTO.ResponseDTO.Sections
{
    public class SectionWithSessionsResponse
    {
        public int Id { get; set; } // SectionId
        public int CourseId { get; set; }
        public string Title { get; set; }
        public int Order { get; set; }

        public List<SessionsWithSectionResponse> Sessions { get; set; } = new();
    }
}

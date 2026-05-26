namespace Courses.Core.ModelsDTO.ResponseDTO.Sections
{
    public class DeleteSectionResponse
    {
        public string Message { get; set; }

        public int SectionsCount { get; set; } // For Multi-Sections Delete

        public int LecturesCount { get; set; } // For numbers of lectures Deleted With Courses
    }
}

namespace Courses.Core.ModelsDTO.RequestDTO.Courses
{
    public class CoursesParams
    {
        public string? Sort { get; set; } // Course Type
        public string? Search { get; set; }
        public int? Type { get; set; }
        public int MaxPageSize { get; set; } = 10;

        public int PageIndex { get; set; } = 1;

        private int pageSize = 5;

        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = value > MaxPageSize? MaxPageSize : value; }
        }

    }
}

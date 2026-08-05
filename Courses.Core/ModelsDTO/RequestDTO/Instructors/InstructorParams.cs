namespace Courses.Core.ModelsDTO.RequestDTO.Instructors
{
    public class InstructorParams
    {
        public string? Search { get; set; }
        public string? Sort { get; set; }

        private int pageSize = 5;

        public int PageIndex { get; set; } = 1;

        public int MaxPageSize { get; set; } = 10;

        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = value > MaxPageSize ? MaxPageSize : value; }
        }

    }
}

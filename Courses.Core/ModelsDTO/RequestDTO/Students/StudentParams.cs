namespace Courses.Core.ModelsDTO.RequestDTO.Students
{
    public class StudentParams
    {
        public string? Sort { get; set; }
        public string? Search { get; set; }

        public int MaxPageSize { get; set; } = 10;
        public int PageIndex { get; set; } = 1;

        private int pageSize = 5;

        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = value > MaxPageSize ? MaxPageSize : value; }
        }

    }
}

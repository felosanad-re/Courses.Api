namespace Courses.Core.ModelsDTO.RequestDTO.DashboardInstructor
{
    public class RatingParams
    {
        public string? Search { get; set; }
        public string? Sort { get; set; }
        public int PageIndex { get; set; } = 1;

        private int pageSize = 10;

        public int MaxPageSize { get; set; } = 30;
        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = value > MaxPageSize ? value : MaxPageSize; }
        }
    }
}

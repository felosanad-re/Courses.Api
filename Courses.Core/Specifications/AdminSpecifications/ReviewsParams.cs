namespace Courses.Core.Specifications.AdminSpecifications
{
    public class ReviewsParams
    {
        public string? Search { get; set; }
        public int Sort { get; set; }

        public int MaxPageSize { get; set; } = 10;
        public int PageIndex { get; set; } = 1;

        private int pageSize;

        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = value > MaxPageSize ? MaxPageSize : value; }
        }
    }
}

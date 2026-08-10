namespace Courses.Core.Specifications.InstructorRequestSpecifications
{
    public class InstructorRequestParams
    {
        public string? Search { get; set; }

        public int PageIndex { get; set; } = 1;

        private int pageSize = 5;
		public int MaxPageSize { get; set; } = 10;
        public int PageSize
		{
			get { return pageSize; }
			set { pageSize = value > MaxPageSize ? MaxPageSize : value; }
		}

	}
}

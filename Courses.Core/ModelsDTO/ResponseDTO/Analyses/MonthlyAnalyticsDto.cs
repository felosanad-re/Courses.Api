namespace Courses.Core.ModelsDTO.ResponseDTO.Analyses
{
    public class MonthlyAnalyticsDto
    {
        public int Years { get; set; }
        public int Month { get; set; }
        public string MonthLabel { get; set; } = string.Empty;
        public int Students { get; set; }
        public decimal Earnings { get; set; }
    }
}

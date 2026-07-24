namespace Courses.Core.ModelsDTO.ResponseDTO.Charts
{
    public class DashboardChartsResponse
    {
        public string Key { get; set; }
        public string Title { get; set; }
        public List<ChartPointResponse> Data { get; set; } = [];
    }
}

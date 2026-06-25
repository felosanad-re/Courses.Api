namespace Courses.Core.ModelsDTO.ResponseDTO.LiveSessions
{
    public class LiveSessionStatisticsResponse
    {
        public int TotalSessions { get; set; }
        public int UpcomingSessions { get; set; }
        public int CompletedSessions { get; set; }
        public int CancelledSessions { get; set; }
    }
}

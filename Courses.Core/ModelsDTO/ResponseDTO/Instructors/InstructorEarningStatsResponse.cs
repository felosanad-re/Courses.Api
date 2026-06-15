namespace Courses.Core.ModelsDTO.ResponseDTO.Instructors
{
    public class InstructorEarningStatsResponse
    {
        public decimal TotalEarnings { get; set; }
        public decimal PeriodEarnings { get; set; }
        public int PeriodEnrollments { get; set; }
        public decimal AverageRevenueEnrollments { get; set; }
    }
}

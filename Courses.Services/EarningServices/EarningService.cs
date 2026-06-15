using AutoMapper;
using Courses.Core.Models.Enrollments;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO.Enrollments;
using Courses.Core.ModelsDTO.ResponseDTO.Instructors;
using Courses.Core.Services.Contract.EarningServices;
using Courses.Core.Services.Contract.InstructorServices;
using Courses.Core.Specifications;
using Courses.Core.UnitOfWork;
using Microsoft.Extensions.Logging;

namespace Courses.Services.EarningServices
{
    public class EarningService : IEarningService
    {
        #region DI Services
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly ICurrentInstructorServices _currentInstructorServices;
        protected readonly IMapper _mapper;
        protected readonly ILogger<EarningService> _logger;
        public EarningService(IUnitOfWork unitOfWork, ILogger<EarningService> logger, ICurrentInstructorServices currentInstructorServices, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _currentInstructorServices = currentInstructorServices;
            _mapper = mapper;
        }
        #endregion

        #region Get Earning Stats
        public async Task<ApplicationServiceResult<InstructorEarningStatsResponse>> GetEarningStatsAsync(DateTime? fromDate, DateTime? toDate)
        {
            var userNotFoundMessage = "There is no instructor with this id";
            var succeededMessage = "Instructor earning statistics retrieved successfully";
            var loggerError = "There is a problem in database";
            int? instructorId = null;

            try
            {
                // Validation In Data
                if (fromDate.HasValue && toDate.HasValue && fromDate > toDate)
                    return ApplicationServiceResult<InstructorEarningStatsResponse>.Fail("From Date Can't be greater than to date");

                instructorId = await GetCurrentInstructorInfo();
                if (instructorId is null)
                    return ApplicationServiceResult<InstructorEarningStatsResponse>.Fail(userNotFoundMessage);

                // Spec For Get Life Time Earning
                var enrollmentSpec = new BaseSpecifications<Enrollment>(x =>
                    (x.Course.InstructorId == instructorId) &&
                    (x.IsPaid)
                 );

                // Set Date Range Value
                var startDate = fromDate ?? DateTime.UtcNow.AddMonths(-3); // From Last 3 Months
                var endDate = toDate ?? DateTime.UtcNow; // To Now

                // Spec for get Filter data
                var filterEnrollmentSpec = new BaseSpecifications<Enrollment>(x =>
                    (x.Course.InstructorId == instructorId) &&
                    (x.IsPaid) &&
                    (x.CreatedAt >= startDate) &&
                    (x.CreatedAt <= endDate)
                    );

                var enrollmentRepo = _unitOfWork.CreateRepository<Enrollment>();

                // Get Total Earning In Life Time
                var totalEarning = await enrollmentRepo.GetSumAsyncSpec(enrollmentSpec, x => x.Amount);

                // Get Enrollments In Period Time
                var periodEnrollments = await enrollmentRepo.GetCountAsyncSpec(filterEnrollmentSpec);

                // Get Period Earning In Time
                var periodEarning =
                    periodEnrollments == 0
                    ? 0 : await enrollmentRepo.GetSumAsyncSpec(filterEnrollmentSpec, x => x.Amount);

                // Get Average Earning
                var averageEarning = periodEnrollments == 0
                    ? 0
                    : periodEarning / periodEnrollments;

                var data = new InstructorEarningStatsResponse
                {
                    TotalEarnings = totalEarning,
                    PeriodEnrollments = periodEnrollments,
                    PeriodEarnings = periodEarning,
                    AverageRevenueEnrollments = averageEarning
                };

                return ApplicationServiceResult<InstructorEarningStatsResponse>.Success(data, succeededMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "there is a problem when try to retrieve Stats For Instructor Id {instructorId}", instructorId);
                return ApplicationServiceResult<InstructorEarningStatsResponse>.Fail(loggerError);
            }
        }
        #endregion

        #region Get Instructor Enrollments Async
        public async Task<ApplicationServiceResult<Pagination<InstructorWithEnrollmentsDetails>>> GetInstructorEnrollmentsAsync(EnrollmentsParams param)
        {
            var userNotFoundMessage = "There is no instructor with this id";
            var succeededMessage = "Instructor Enrollments retrieved successfully";
            var loggerError = "There is a problem in database";
            int? instructorId = null;

            try
            {
                instructorId = await GetCurrentInstructorInfo();
                if (instructorId is null)
                    return ApplicationServiceResult<Pagination<InstructorWithEnrollmentsDetails>>.Fail(userNotFoundMessage);

                var enrollmentRepo = _unitOfWork.CreateRepository<Enrollment>();

                var enrollmentSpec = BuildEnrollmentSpec(param, instructorId, isPagination: true);

                var countEnrollmentSpec = BuildEnrollmentSpec(param, instructorId, isPagination: false);

                var enrollmentCount = await enrollmentRepo.GetCountAsyncSpec(countEnrollmentSpec);
                if (enrollmentCount == 0)
                    return ApplicationServiceResult<Pagination<InstructorWithEnrollmentsDetails>>
                        .Success(new Pagination<InstructorWithEnrollmentsDetails>(
                            param.PageIndex,
                            param.PageSize,
                            0,
                            []
                         ), "there is no enrollments yet");

                var enrollments = await enrollmentRepo.GetAllAsyncSpec(enrollmentSpec);

                var data = _mapper.Map<IReadOnlyList<InstructorWithEnrollmentsDetails>>(enrollments);
                var pagination = new Pagination<InstructorWithEnrollmentsDetails>(
                        param.PageIndex,
                        param.PageSize,
                        enrollmentCount,
                        data
                    );

                return ApplicationServiceResult<Pagination<InstructorWithEnrollmentsDetails>>
                        .Success(pagination, succeededMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "there is a problem when try to retrieve Enrollments For Instructor Id {instructorId}", instructorId);
                return ApplicationServiceResult<Pagination<InstructorWithEnrollmentsDetails>>.Fail(loggerError);
            }
        }
        #endregion

        #region Helper Method
        private async Task<int?> GetCurrentInstructorInfo()
        {
            var instructorInfo = await _currentInstructorServices.GetCurrentInstructor();
            if (instructorInfo is null || instructorInfo.Data is null)
                return null;
            return instructorInfo.Data.Id;
        }

        private BaseSpecifications<Enrollment> BuildEnrollmentSpec(EnrollmentsParams param, int? instructorId, bool isPagination = false)
        {
            var spec = new BaseSpecifications<Enrollment>(x =>
                    (x.Course.InstructorId == instructorId) &&
                    (string.IsNullOrEmpty(param.Search) || x.Course.Name.Contains(param.Search)));
            spec.Includes.Add(x => x.Student);
            spec.Includes.Add(x => x.Course);

            if(isPagination)
            {
                spec.AddOrderByDesc(x => x.CreatedAt);
                spec.AddPagination(param.PageSize * (param.PageIndex - 1), param.PageSize);
            }
            return spec;
        }
        #endregion
    }
}

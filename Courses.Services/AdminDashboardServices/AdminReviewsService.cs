using AutoMapper;
using Courses.Core.Models.Courses;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.ResponseDTO.AdminDashboard;
using Courses.Core.Services.Contract.AdminDashboardServices;
using Courses.Core.Services.Contract.UserServices;
using Courses.Core.Specifications.AdminSpecifications;
using Courses.Core.UnitOfWork;
using Microsoft.Extensions.Logging;

namespace Courses.Services.AdminDashboardServices
{
    public class AdminReviewsService : IAdminReviewsService
    {
        private const string SuccessedMessage = "You Retrieve Your Data Succeeded";
        private const string ErrorMessage = "There is no user With this Id";
        private const string WarningMessage = "There is no data to retrieved";
        private const string LoggerMessage = "There is a problem in database";

        #region Services
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly ILogger<AdminReviewsService> _logger;
        protected readonly ICurrentUserService _currentUserService;
        protected readonly IMapper _mapper;

        public AdminReviewsService(IUnitOfWork unitOfWork, ILogger<AdminReviewsService> logger, ICurrentUserService currentUserService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }
        #endregion

        #region Get All Reviews Async
        public async Task<ApplicationServiceResult<Pagination<AdminCoursesReviewsResponse>>> GetAllReviewsAsync(ReviewsParams param)
        {
            string? userId = null;

            try
            {
                userId = _currentUserService.UserId;
                if (userId is null)
                    return ApplicationServiceResult<Pagination<AdminCoursesReviewsResponse>>.Fail(ErrorMessage);

                var reviewSpec = new AdminReviewSpec(param);
                var reviewCountSpec = new AdminReviewsCountSpec(param);
                var reviewRepo = _unitOfWork.CreateRepository<CourseRating>();

                var reviewCoursesCount = await reviewRepo.GetCountAsyncSpec(reviewCountSpec);
                if (reviewCoursesCount <= 0)
                    return ApplicationServiceResult<Pagination<AdminCoursesReviewsResponse>>.Success(new Pagination<AdminCoursesReviewsResponse>(param.PageIndex, param.PageSize, reviewCoursesCount, []), WarningMessage);

                var reviewCourses = await reviewRepo.GetAllAsyncSpec(reviewSpec);

                var data = _mapper.Map<IReadOnlyList<AdminCoursesReviewsResponse>>(reviewCourses);

                var paginagtion = new Pagination<AdminCoursesReviewsResponse>(
                        param.PageIndex,
                        param.PageSize,
                        reviewCoursesCount,
                        data
                    );

                return ApplicationServiceResult<Pagination<AdminCoursesReviewsResponse>>.Success(paginagtion, SuccessedMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "There is a problem when try to retrieve Data For User id {userId}", userId);
                return ApplicationServiceResult<Pagination<AdminCoursesReviewsResponse>>.Fail(LoggerMessage);
            }
        }
        #endregion

        #region Get Review Details Async
        public async Task<ApplicationServiceResult<AdminReviewDetailsResponse>> GetReviewDetailsAsync(int reviewId)
        {
            string? userId = null;

            try
            {
                userId = _currentUserService.UserId;
                if (userId is null)
                    return ApplicationServiceResult<AdminReviewDetailsResponse>.Fail(ErrorMessage);

                var reviewSpec = new AdminReviewSpec(reviewId);
                var reviewRepo = _unitOfWork.CreateRepository<CourseRating>();

                var reviewCourse = await reviewRepo.GetAsyncSpec(reviewSpec);

                if(reviewCourse is null)
                    return ApplicationServiceResult<AdminReviewDetailsResponse>.Fail(WarningMessage);

                var data = _mapper.Map<AdminReviewDetailsResponse>(reviewCourse);

                return ApplicationServiceResult<AdminReviewDetailsResponse>.Success(data, SuccessedMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "There is a problem when try to retrieve details review For User id {userId} and review id {reviewId}", userId, reviewId);
                return ApplicationServiceResult<AdminReviewDetailsResponse>.Fail(LoggerMessage);
            }
        }
        #endregion

        #region Delete Review Async
        public async Task<ApplicationServiceResult<bool>> DeleteReviewAsync(int reviewId)
        {
            const string DeletedMessage = "Review Deleted Successfully";
            string? userId = null;

            try
            {
                userId = _currentUserService.UserId;
                if (userId is null)
                    return ApplicationServiceResult<bool>.Fail(ErrorMessage);

                var reviewSpec = new AdminReviewSpec(reviewId);
                var reviewRepo = _unitOfWork.CreateRepository<CourseRating>();

                var reviewCourse = await reviewRepo.GetAsyncSpec(reviewSpec);

                if (reviewCourse is null)
                    return ApplicationServiceResult<bool>.Fail(WarningMessage);

                reviewCourse.IsDeleted = true;
                await _unitOfWork.CompleteAsync();

                return ApplicationServiceResult<bool>.Success(true, DeletedMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "There is a problem when try to retrieve details review For User id {userId} and review id {reviewId}", userId, reviewId);
                return ApplicationServiceResult<bool>.Fail(LoggerMessage);
            }
        }
        #endregion
    }
}

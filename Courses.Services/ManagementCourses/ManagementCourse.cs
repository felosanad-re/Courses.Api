using AutoMapper;
using Courses.Core.Models.Courses;
using Courses.Core.Models.Instructors;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO.Courses;
using Courses.Core.ModelsDTO.ResponseDTO.Courses;
using Courses.Core.Options;
using Courses.Core.Services.Contract.AttachmentServices;
using Courses.Core.Services.Contract.ManagementCourses;
using Courses.Core.Services.Contract.UserServices;
using Courses.Core.Specifications.CoursesSpecifications;
using Courses.Core.Specifications.InstructorsSpecifications;
using Courses.Core.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Courses.Services.ManagementCourses
{
    public class ManagementCourse : IManagementCourse
    {
        #region Inject Services
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly ILogger<ManagementCourse> _logger;
        protected readonly ICurrentUserService _currentUserService;
        protected readonly IMapper _mapper;
        protected readonly IAttachmentService _attachmentService;
        protected readonly FileSettingsOptions _fileSettings;

        public ManagementCourse(IUnitOfWork unitOfWork, ILogger<ManagementCourse> logger, ICurrentUserService currentUserService, IMapper mapper, IAttachmentService attachmentService, IOptions<FileSettingsOptions> fileSettings)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _currentUserService = currentUserService;
            _mapper = mapper;
            _attachmentService = attachmentService;
            _fileSettings = fileSettings.Value;
        }
        #endregion

        #region Get Course Details Async
        public async Task<ApplicationServiceResult<CourseResponseForInstructor>> GetCourseDetailsAsync(int id)
        {
            var userNotFoundMessage = "User Not Found";
            var errorMessage = "No Courses with this id";
            var succeededMessage = "Course details retrieved successfully";
            var loggerError = "There is a problem in database";

            try
            {
                if (id <= 0) return ApplicationServiceResult<CourseResponseForInstructor>.Fail("course id must be greater than zero");

                var userId = _currentUserService.UserId;
                if (userId is null) return ApplicationServiceResult<CourseResponseForInstructor>.Fail(userNotFoundMessage);

                var spec = new CourseWithInstructorSpec(id, userId);
                var course = await _unitOfWork.CreateRepository<Course>().GetAsyncSpec(spec);
                if (course is null) return ApplicationServiceResult<CourseResponseForInstructor>.Fail(errorMessage);

                var data = _mapper.Map<CourseResponseForInstructor>(course);
                return ApplicationServiceResult<CourseResponseForInstructor>.Success(data, succeededMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return ApplicationServiceResult<CourseResponseForInstructor>.Fail(loggerError);
            }
        }
        #endregion

        #region Create Course Async
        public async Task<ApplicationServiceResult<CourseResponseForInstructor>> CreateCourseAsync(CreatedCourseRequest req)
        {
            var userNotFoundMessage = "User Not Found";
            var errorMessage = "Instructor not found";
            var succeededMessage = "Course created succeeded";
            var loggerError = "There is a problem in database";
            string? uploadedImage = null;

            try
            {
                if (!ValidateCourseInput(req.Name, req.Description, req.Image, imageRequired: true, req.CourseCategoryId, req.IsPaid, req.Price, _fileSettings, out var normalizedCourse, out var validationError))
                    return ApplicationServiceResult<CourseResponseForInstructor>.Fail(validationError);

                var userId = _currentUserService.UserId;
                if (userId is null) return ApplicationServiceResult<CourseResponseForInstructor>.Fail(userNotFoundMessage);

                var courseType = await _unitOfWork.CreateRepository<CourseCategory>().GetAsync(req.CourseCategoryId);
                if (courseType is null)
                    return ApplicationServiceResult<CourseResponseForInstructor>.Fail("Course type not found");

                // Get Current Instructor
                var currentInstructor = await _unitOfWork.CreateRepository<Instructor>()
                    .GetAsyncSpec(new InstructorSpec(userId));
                if (currentInstructor is null)
                    return ApplicationServiceResult<CourseResponseForInstructor>.Fail(errorMessage);

                var newCourse = _mapper.Map<Course>(req);
                newCourse.InstructorId = currentInstructor.Id;
                ApplyNormalizedCourseValues(newCourse, normalizedCourse);
                uploadedImage = await _attachmentService.UploadAsync(req.Image, _fileSettings.FolderName, _fileSettings.AllowedExtensions, _fileSettings.MaxSize);
                newCourse.Image = uploadedImage;

                await _unitOfWork.CreateRepository<Course>().AddAsync(newCourse);
                await _unitOfWork.CompleteAsync();

                var data = _mapper.Map<CourseResponseForInstructor>(newCourse);

                return ApplicationServiceResult<CourseResponseForInstructor>.Success(data, succeededMessage);
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrWhiteSpace(uploadedImage))
                    await TryDeleteImageAsync(uploadedImage);

                _logger.LogError(ex, ex.Message);
                return ApplicationServiceResult<CourseResponseForInstructor>.Fail(loggerError);
            }
        }
        #endregion

        #region Update Course Async
        public async Task<ApplicationServiceResult<CourseResponseForInstructor>> UpdateCourseAsync(int id, UpdatedCourseRequest req)
        {
            var userNotFoundMessage = "User Not Found";
            var errorMessage = "No course Found with this id";
            var succeededMessage = "Course updated succeeded";
            var loggerError = "There is a problem in database";
            string? newImage = null;

            try
            {
                if (id <= 0) return ApplicationServiceResult<CourseResponseForInstructor>.Fail("course id must be greater than zero");

                if (!ValidateCourseInput(req.Name, req.Description, req.Image, imageRequired: false, req.CourseCategoryId, req.IsPaid, req.Price, _fileSettings, out var normalizedCourse, out var validationError))
                    return ApplicationServiceResult<CourseResponseForInstructor>.Fail(validationError);

                var userId = _currentUserService.UserId;
                if (userId is null) return ApplicationServiceResult<CourseResponseForInstructor>.Fail(userNotFoundMessage);

                var courseType = await _unitOfWork.CreateRepository<CourseCategory>().GetAsync(req.CourseCategoryId);
                if (courseType is null)
                    return ApplicationServiceResult<CourseResponseForInstructor>.Fail("Course type not found");

                var courseRepo = _unitOfWork.CreateRepository<Course>();

                // Get Course with Instructor
                var course = await courseRepo.GetAsyncSpec(new CourseWithInstructorSpec(id, userId));
                if (course is null) return ApplicationServiceResult<CourseResponseForInstructor>.Fail(errorMessage);

                var oldImage = course.Image;
                if (req.Image is not null)
                    newImage = await _attachmentService.UploadAsync(req.Image, _fileSettings.FolderName, _fileSettings.AllowedExtensions, _fileSettings.MaxSize);

                // Update Course Mapping
                _mapper.Map(req, course);
                ApplyNormalizedCourseValues(course, normalizedCourse);
                if (newImage is not null)
                    course.Image = newImage;

                courseRepo.Update(course);
                await _unitOfWork.CompleteAsync();

                if (newImage is not null && !string.IsNullOrWhiteSpace(oldImage))
                    await _attachmentService.DeleteImageAsync(oldImage, _fileSettings.FolderName);

                var data = _mapper.Map<CourseResponseForInstructor>(course);
                return ApplicationServiceResult<CourseResponseForInstructor>.Success(data, succeededMessage);
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrWhiteSpace(newImage))
                    await TryDeleteImageAsync(newImage);

                _logger.LogError(ex, ex.Message);
                return ApplicationServiceResult<CourseResponseForInstructor>.Fail(loggerError);
            }
        }
        #endregion

        #region Delete Course Async
        public async Task<ApplicationServiceResult<bool>> DeleteCourseAsync(int id)
        {
            var userNotFoundMessage = "User Not Found";
            var errorMessage = "No course Found with this id";
            var succeededMessage = "Course Deleted succeeded";
            var loggerError = "There is a problem in database";

            try
            {
                if (id <= 0) return ApplicationServiceResult<bool>.Fail("course id must be greater than zero");

                var userId = _currentUserService.UserId;
                if (userId is null) return ApplicationServiceResult<bool>
                        .Fail(userNotFoundMessage);

                var courseRepo = _unitOfWork.CreateRepository<Course>();
                // Get Course
                var course = await courseRepo.GetAsyncSpec(new CourseWithInstructorSpec(id, userId));
                if (course is null) return ApplicationServiceResult<bool>.Fail(errorMessage);

                SoftDeleteCourseWithChildren(course);
                courseRepo.Update(course);
                await _unitOfWork.CompleteAsync();
                return ApplicationServiceResult<bool>.Success(true, succeededMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return ApplicationServiceResult<bool>.Fail(loggerError);
            }
        }
        #endregion

        #region Delete Courses Async
        public async Task<ApplicationServiceResult<DeleteCoursesResult>> DeleteCoursesAsync(IEnumerable<int> courseIds)
        {
            var userNotFoundMessage = "User Not Found";
            var succeededMessage = "Course Deleted succeeded";
            var loggerError = "There is a problem in database";

            try
            {
                var ids = courseIds?.Distinct().ToArray() ?? Array.Empty<int>();
                if (ids.Length == 0)
                    return ApplicationServiceResult<DeleteCoursesResult>.Fail("course ids are required");

                if (ids.Any(id => id <= 0))
                    return ApplicationServiceResult<DeleteCoursesResult>.Fail("course ids must be greater than zero");

                var userId = _currentUserService.UserId;
                if (userId is null) return ApplicationServiceResult<DeleteCoursesResult>
                    .Fail(userNotFoundMessage);

                var courseRepo = _unitOfWork.CreateRepository<Course>();
                // All Courses For Specification Instructors
                var courses = await courseRepo.GetAllAsyncSpec(new CourseWithInstructorSpec(ids, userId));

                // Get All Courses With Ids
                var allCourses = await courseRepo.GetAllAsyncSpec(new CourseWithInstructorSpec(ids));

                // Existing Courses
                var existingIds = courses.Select(c => c.Id).ToList();
                // All Courses with Ids Request [authorization - unauthorized]
                var allIds = allCourses.Select(c => c.Id).ToList();

                var notFoundIds = ids.Except(allIds).ToList();    // Ids not found in database
                var unauthorizedIds = allIds.Except(existingIds).ToList(); // Ids not for this instructor

                if (notFoundIds.Any())
                    return ApplicationServiceResult<DeleteCoursesResult>.Fail($"Courses not found: {string.Join(", ", notFoundIds)}");

                if (unauthorizedIds.Any())
                    return ApplicationServiceResult<DeleteCoursesResult>.Fail($"You don't have access to courses: {string.Join(", ", unauthorizedIds)}");

                foreach (var course in courses)
                {
                    SoftDeleteCourseWithChildren(course);
                }

                await _unitOfWork.CompleteAsync();
                var result = new DeleteCoursesResult
                {
                    DeletedIds = existingIds,
                    NotFoundIds = notFoundIds,
                    UnauthorizedIds = unauthorizedIds
                };

                return ApplicationServiceResult<DeleteCoursesResult>.Success(result, succeededMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return ApplicationServiceResult<DeleteCoursesResult>.Fail(loggerError);
            }
        }
        #endregion

        #region Helper Methods
        private static bool ValidateCourseInput(
            string? name,
            string? description,
            IFormFile? image,
            bool imageRequired,
            int courseCategoryId,
            bool isPaid,
            decimal price,
            FileSettingsOptions fileSettings,
            out NormalizedCourseInput normalizedCourse,
            out string errorMessage)
        {
            normalizedCourse = new NormalizedCourseInput();
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(name))
            {
                errorMessage = "course name is required";
                return false;
            }

            if (string.IsNullOrWhiteSpace(description))
            {
                errorMessage = "course description is required";
                return false;
            }

            if (image is null)
            {
                if (imageRequired)
                {
                    errorMessage = "course image is required";
                    return false;
                }
            }
            else
            {
                if (!ValidateImage(image, fileSettings, out errorMessage))
                    return false;
            }

            var normalizedName = name.Trim();
            var normalizedDescription = description.Trim();

            if (normalizedName.Length > 300)
            {
                errorMessage = "course name can't be more than 300 characters";
                return false;
            }

            if (normalizedDescription.Length > 2000)
            {
                errorMessage = "course description can't be more than 2000 characters";
                return false;
            }

            if (courseCategoryId <= 0)
            {
                errorMessage = "course type id must be greater than zero";
                return false;
            }

            if (price < 0)
            {
                errorMessage = "course price can't be negative";
                return false;
            }

            if (isPaid && price <= 0)
            {
                errorMessage = "paid course price must be greater than zero";
                return false;
            }

            normalizedCourse = new NormalizedCourseInput
            {
                Name = normalizedName,
                Description = normalizedDescription,
                CourseCategoryId = courseCategoryId,
                IsPaid = isPaid,
                Price = isPaid ? price : 0m
            };

            return true;
        }

        private static void ApplyNormalizedCourseValues(Course course, NormalizedCourseInput normalizedCourse)
        {
            course.Name = normalizedCourse.Name;
            course.Description = normalizedCourse.Description;
            course.CourseCategoryId = normalizedCourse.CourseCategoryId;
            course.IsPaid = normalizedCourse.IsPaid;
            course.Price = normalizedCourse.Price;
        }

        private static void SoftDeleteCourseWithChildren(Course course)
        {
            course.IsDeleted = true;

            foreach (var section in course.Sections)
            {
                section.IsDeleted = true;

                foreach (var lecture in section.Lectures)
                {
                    lecture.IsDeleted = true;
                }
            }
        }

        private static bool ValidateImage(IFormFile image, FileSettingsOptions fileSettings, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (image.Length == 0)
            {
                errorMessage = "course image can't be empty";
                return false;
            }

            if (image.Length > fileSettings.MaxSize)
            {
                errorMessage = $"course image size can't be more than {fileSettings.MaxSize / 1024 / 1024} MB";
                return false;
            }

            var allowedExtensions = fileSettings.AllowedExtensions
                .Select(extension => extension.ToLowerInvariant())
                .ToHashSet();
            var imageExtension = Path.GetExtension(image.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(imageExtension))
            {
                errorMessage = $"course image extension must be one of: {string.Join(", ", fileSettings.AllowedExtensions)}";
                return false;
            }

            var allowedContentTypes = fileSettings.AllowedContentTypes
                .Select(contentType => contentType.ToLowerInvariant())
                .ToHashSet();
            if (string.IsNullOrWhiteSpace(image.ContentType) ||
                !allowedContentTypes.Contains(image.ContentType.ToLowerInvariant()))
            {
                errorMessage = $"course image content type must be one of: {string.Join(", ", fileSettings.AllowedContentTypes)}";
                return false;
            }

            return true;
        }

        private async Task TryDeleteImageAsync(string fileName)
        {
            try
            {
                await _attachmentService.DeleteImageAsync(fileName, _fileSettings.FolderName);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to delete course image {Image}", fileName);
            }
        }

        private sealed class NormalizedCourseInput
        {
            public string Name { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public int CourseCategoryId { get; set; }
            public bool IsPaid { get; set; }
            public decimal Price { get; set; }
        }
        #endregion
    }
}

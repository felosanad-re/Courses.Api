using AutoMapper;
using Courses.Api.Helper.Resolvers;
using Courses.Core.Models.ApplicationUsers;
using Courses.Core.Models.Courses;
using Courses.Core.Models.Enrollments;
using Courses.Core.Models.Instructors;
using Courses.Core.Models.LiveSessions;
using Courses.Core.Models.Students;
using Courses.Core.ModelsDTO.RequestDTO.Account;
using Courses.Core.ModelsDTO.RequestDTO.Courses;
using Courses.Core.ModelsDTO.RequestDTO.Lectures;
using Courses.Core.ModelsDTO.RequestDTO.Profile;
using Courses.Core.ModelsDTO.RequestDTO.Sections;
using Courses.Core.ModelsDTO.ResponseDTO.Account;
using Courses.Core.ModelsDTO.ResponseDTO.AdminDashboard;
using Courses.Core.ModelsDTO.ResponseDTO.Courses;
using Courses.Core.ModelsDTO.ResponseDTO.CoursesCategories;
using Courses.Core.ModelsDTO.ResponseDTO.DashboardInstructor;
using Courses.Core.ModelsDTO.ResponseDTO.Enrollment;
using Courses.Core.ModelsDTO.ResponseDTO.Instructors;
using Courses.Core.ModelsDTO.ResponseDTO.Lectures;
using Courses.Core.ModelsDTO.ResponseDTO.LiveSessions;
using Courses.Core.ModelsDTO.ResponseDTO.Payments;
using Courses.Core.ModelsDTO.ResponseDTO.Progress;
using Courses.Core.ModelsDTO.ResponseDTO.Refunds;
using Courses.Core.ModelsDTO.ResponseDTO.Sections;
using Courses.Core.ModelsDTO.ResponseDTO.StudentLectureProgress;
using Courses.Core.ModelsDTO.ResponseDTO.Students;

namespace Courses.Api.Helper.Mapping
{
    public class ProfileMapping : Profile
    {
        public ProfileMapping()
        {
            CreateMap<CreateAccountRequest, ApplicationUser>();
            CreateMap<ApplicationUser, CreateAccountResponse>();

            #region Edit Profile Request
            CreateMap<EditProfileRequest, ApplicationUser>();
            CreateMap<EditProfileRequest, Instructor>()
                .ForMember(d => d.Name, o => o.MapFrom(s => $"{s.FirstName} {s.LastName}"));
            CreateMap<EditProfileRequest, Student>()
                .ForMember(d => d.Name, o => o.MapFrom(s => $"{s.FirstName} {s.LastName}"));
            #endregion

            #region Courses With Instructors
            CreateMap<Course, CourseResponseForInstructor>()
                .ForMember(d => d.Image, o => o.MapFrom<ImageResolver<Course, CourseResponseForInstructor>, string>(s => s.Image));
            CreateMap<Course, InstructorWithCoursesResponse>()
                .ForMember(d => d.Image, o => o.MapFrom<ImageResolver<Course, InstructorWithCoursesResponse>, string>(s => s.Image));
            CreateMap<CreatedCourseRequest, Course>()
                .ForMember(d => d.Image, o => o.Ignore());
            CreateMap<UpdatedCourseRequest, Course>()
                .ForMember(d => d.Image, o => o.Ignore());
            CreateMap<Course, CourseAnalyticDTO>()
                .ForMember(d => d.Image, o => o.MapFrom<ImageResolver<Course, CourseAnalyticDTO>,string>(s => s.Image))
                .ForMember(d => d.Enrollments, o => o.Ignore())
                .ForMember(d => d.Revenue, o => o.Ignore());
            #endregion

            CreateMap<Instructor, InstructorResponse>();

            #region Courses
            CreateMap<Course, CourseResponse>()
                .ForMember(d => d.CourseCategory, o => o.MapFrom(s => s.CourseCategory.Name))
                .ForMember(d => d.InstructorName, o => o.MapFrom(s => s.Instructor.Name))
                .ForMember(d => d.Image, o => o.MapFrom<ImageResolver<Course, CourseResponse>, string>(s => s.Image));

            CreateMap<Course, CoursesToReturnDTO>()
                .ForMember(d => d.CourseCategory, o => o.MapFrom(s => s.CourseCategory.Name))
                .ForMember(d => d.Image, o => o.MapFrom<ImageResolver<Course, CoursesToReturnDTO>, string>(s => s.Image));

            CreateMap<Course, CourseDetailsToReturnDTO>()
                .ForMember(d => d.CourseCategory, o => o.MapFrom(s => s.CourseCategory.Name))
                .ForMember(d => d.InstructorName, o => o.MapFrom(s => s.Instructor.Name))
                .ForMember(d => d.Image, o => o.MapFrom<ImageResolver<Course, CourseDetailsToReturnDTO>, string>(s => s.Image));

            CreateMap<Course, CourseProgressResponse>()
                .ForMember(d => d.CourseName, o => o.MapFrom(s => s.Name));
            CreateMap<Course, CourseTypesResponse>();
            CreateMap<Course, CourseResponseForSubmit>();

            CreateMap<Course, AdminCoursesResponse>()
                .ForMember(d => d.CourseCategory, o => o.MapFrom(s => s.CourseCategory.Name));

            CreateMap<Course, AdminInstructorCoursesResponse>()
                .ForMember(d => d.CourseCategoryName, o => o.MapFrom(s => s.CourseCategory.Name))
                .ForMember(d => d.NumberOfSections, o => o.MapFrom(s => s.Sections.Count));
            #endregion

            CreateMap<CourseCategory, CourseCategoryToReturnDTO>();

            #region Enrollment
            CreateMap<Enrollment, EnrollmentResponse>()
                .ForMember(d => d.CourseName, o => o.MapFrom(s => s.Course.Name))
                .ForMember(d => d.StudentName, o => o.MapFrom(s => s.Student.Name));

            CreateMap<Enrollment, EnrollmentWithCoursesResponse>()
                .ForMember(d => d.CourseId, o => o.MapFrom(s => s.CourseId))
                .ForMember(d => d.Name, o => o.MapFrom(s => s.Course.Name))
                .ForMember(d => d.Description, o => o.MapFrom(s => s.Course.Description))
                .ForMember(d => d.Image, o => o.MapFrom<ImageResolver<Enrollment, EnrollmentWithCoursesResponse>, string>(s => s.Course.Image))
                .ForMember(d => d.InstructorId, o => o.MapFrom(s => s.Course.InstructorId))
                .ForMember(d => d.IsPaid, o => o.MapFrom(s => s.Course.IsPaid))
                .ForMember(d => d.CourseCategory, o => o.MapFrom(s => s.Course.CourseCategory.Name))
                .ForMember(d => d.Price, o => o.MapFrom(s => s.Course.Price))
                .ForMember(d => d.AverageRating, o => o.MapFrom(s => s.Course.AverageRating))
                .ForMember(d => d.RatingCount, o => o.MapFrom(s => s.Course.RatingCount));

            CreateMap<Enrollment, PaymentResponse>();

            CreateMap<Enrollment, RefundResponse>();

            CreateMap<Enrollment, InstructorWithEnrollmentsDetails>()
                .ForMember(d => d.EnrollmentId, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.CourseId, o => o.MapFrom(s => s.CourseId))
                .ForMember(d => d.StudentName, o => o.MapFrom(s => s.Student.Name))
                .ForMember(d => d.CourseTitle, o => o.MapFrom(s => s.Course.Name));

            CreateMap<Enrollment, AdminEnrollmentsWithStudentResponse>();
            #endregion

            #region Lecture
            CreateMap<Lecture, LectureResponse>()
                .ForMember(d => d.SectionName, o => o.MapFrom(s => s.Section.Title));

            CreateMap<Lecture, CourseContentItemDTO>()
                .ForMember(d => d.SectionName, o => o.MapFrom(s => s.Section.Title))
                .ForMember(d => d.Url, o => o.MapFrom(s => s.VideoUrl));

            CreateMap<Lecture, LectureWithSectionResponse>();

            CreateMap<Lecture, CourseWithLectureVideoResponse>();

            CreateMap<Lecture, LectureWithInstructorResponse>()
                .ForMember(d => d.SectionName, o=> o.MapFrom(s => s.Section.Title));

            CreateMap<CreatedLectureRequest, Lecture>();

            CreateMap<UpdatedLectureRequest, Lecture>();
            #endregion

            #region Sections
            CreateMap<Section, SectionWithCourseResponse>()
                .ForMember(d => d.AverageRating, o => o.MapFrom(s => s.Course.AverageRating))
                .ForMember(d => d.RatingCount, o => o.MapFrom(s => s.Course.RatingCount));

            CreateMap<Section, SectionToReturnDTO>()
                .ForMember(d => d.CourseName, o => o.MapFrom(s => s.Course.Name));

            CreateMap<CreateSectionRequest, Section>();

            CreateMap<UpdateSectionRequest, Section>();
            CreateMap<Section, SectionWithSessionsResponse>();
            CreateMap<Section, SectionListResponse>();
            #endregion

            #region Student Lecture Progress
            CreateMap<StudentLectureProgress, StudentLectureProgressResponse>()
                .ForMember(d => d.LectureName, o => o.MapFrom(s => s.Lecture.Title));

            CreateMap<StudentLectureProgress, ProgressWithLectureResponse>()
                .ForMember(d => d.LectureName, o => o.MapFrom(s => s.Lecture.Title))
                .ForMember(d => d.VideoDuration, o => o.MapFrom(s => s.Lecture.DurationInSeconds));
            #endregion

            #region Live Sessions
            CreateMap<LiveSession, LiveSessionResponse>();
            CreateMap<LiveSession, LiveSessionListResponse>()
                .ForMember(d => d.SectionName, o => o.MapFrom(s => s.Section.Title))
                .ForMember(d => d.CourseName, o => o.MapFrom(s => s.Section.Course.Name));
            CreateMap<LiveSession, LiveSessionDetailsResponse>()
                .ForMember(d => d.SectionName, o => o.MapFrom(s => s.Section.Title));

            CreateMap<LiveSession, SessionsWithSectionResponse>();

            CreateMap<LiveSession, CourseContentItemDTO>()
                .ForMember(d => d.Title, o => o.MapFrom(s => s.Topic))
                .ForMember(d => d.SectionName, o => o.MapFrom(s => s.Section.Title))
                .ForMember(d => d.Url, o => o.MapFrom(s => s.StudentJoinUrl));
            #endregion

            #region Students
            CreateMap<Student, StudentWithApplicationUserToReturnDTO>();

            CreateMap<Student, AdminWithStudentResponse>()
                .ForMember(d => d.NumberOfEnrollments, o => o.MapFrom(s => s.Enrollments.Count));

            CreateMap<Student, AdminWithStudentDetailsResponse>()
                .ForMember(d => d.Email, o => o.MapFrom(s => s.ApplicationUser.Email))
                .ForMember(d => d.PhoneNumber, o => o.MapFrom(s => s.ApplicationUser.PhoneNumber))
                .ForMember(d => d.Address, o => o.MapFrom(s => s.ApplicationUser.Address))
                .ForMember(d => d.UserName, o => o.MapFrom(s => s.ApplicationUser.UserName))
                .ForMember(d => d.Status, o => o.MapFrom(s => s.ApplicationUser.Status));
            #endregion

            #region Instructors
            CreateMap<Instructor, InstructorWithApplicationUserResponse>();
            CreateMap<Instructor, AdminInstructorResponse>()
                .ForMember(d => d.NumberOfCourses, o => o.MapFrom(s => s.Courses.Count));
            CreateMap<Instructor, AdminInstructorDetailsResponse>()
                .ForMember(d => d.NumberOfCourses, o => o.MapFrom(s => s.Courses.Count))
                 .ForMember(d => d.Email, o => o.MapFrom(s => s.ApplicationUser.Email))
                .ForMember(d => d.PhoneNumber, o => o.MapFrom(s => s.ApplicationUser.PhoneNumber))
                .ForMember(d => d.Address, o => o.MapFrom(s => s.ApplicationUser.Address))
                .ForMember(d => d.UserName, o => o.MapFrom(s => s.ApplicationUser.UserName))
                .ForMember(d => d.IsDeleted, o => o.MapFrom(s => s.ApplicationUser.IsDeleted));
            #endregion

            #region Course Rating
            CreateMap<CourseRating, DashboardInstructorReviewsDTO>()
                .ForMember(d => d.CourseName, o => o.MapFrom(s => s.Course.Name))
                .ForMember(d => d.StudentName, o => o.MapFrom(s => s.Student.Name));

            CreateMap<CourseRating, AdminDashboardReviewsResponse>()
                .ForMember(d => d.CourseName, o => o.MapFrom(s => s.Course.Name))
                .ForMember(d => d.StudentName, o => o.MapFrom(s => s.Student.Name));
            #endregion
        }
    }
}

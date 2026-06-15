using AutoMapper;
using Courses.Api.Helper.Resolvers;
using Courses.Core.Models.ApplicationUsers;
using Courses.Core.Models.Courses;
using Courses.Core.Models.Enrollments;
using Courses.Core.Models.Instructors;
using Courses.Core.Models.Students;
using Courses.Core.ModelsDTO.RequestDTO.Account;
using Courses.Core.ModelsDTO.RequestDTO.Courses;
using Courses.Core.ModelsDTO.RequestDTO.Lectures;
using Courses.Core.ModelsDTO.RequestDTO.Profile;
using Courses.Core.ModelsDTO.RequestDTO.Sections;
using Courses.Core.ModelsDTO.ResponseDTO.Account;
using Courses.Core.ModelsDTO.ResponseDTO.Courses;
using Courses.Core.ModelsDTO.ResponseDTO.CoursesTypes;
using Courses.Core.ModelsDTO.ResponseDTO.Enrollment;
using Courses.Core.ModelsDTO.ResponseDTO.Instructors;
using Courses.Core.ModelsDTO.ResponseDTO.Lectures;
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
                .ForMember(d => d.CourseType, o => o.MapFrom(s => s.CourseType.Name))
                .ForMember(d => d.InstructorName, o => o.MapFrom(s => s.Instructor.Name))
                .ForMember(d => d.Image, o => o.MapFrom<ImageResolver<Course, CourseResponse>, string>(s => s.Image));

            CreateMap<Course, CoursesToReturnDTO>()
                .ForMember(d => d.CourseType, o => o.MapFrom(s => s.CourseType.Name))
                .ForMember(d => d.Image, o => o.MapFrom<ImageResolver<Course, CoursesToReturnDTO>, string>(s => s.Image));

            CreateMap<Course, CourseDetailsToReturnDTO>()
                .ForMember(d => d.CourseType, o => o.MapFrom(s => s.CourseType.Name))
                .ForMember(d => d.InstructorName, o => o.MapFrom(s => s.Instructor.Name))
                .ForMember(d => d.Image, o => o.MapFrom<ImageResolver<Course, CourseDetailsToReturnDTO>, string>(s => s.Image));

            CreateMap<Course, CourseProgressResponse>()
                .ForMember(d => d.CourseName, o => o.MapFrom(s => s.Name));
            #endregion

            CreateMap<CourseType, CourseTypeToReturnDTO>();

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
                .ForMember(d => d.CourseType, o => o.MapFrom(s => s.Course.CourseType.Name))
                .ForMember(d => d.Price, o => o.MapFrom(s => s.Course.Price));

            CreateMap<Enrollment, PaymentResponse>();

            CreateMap<Enrollment, RefundResponse>();

            CreateMap<Enrollment, InstructorWithEnrollmentsDetails>()
                .ForMember(d => d.EnrollmentId, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.CourseId, o => o.MapFrom(s => s.CourseId))
                .ForMember(d => d.StudentName, o => o.MapFrom(s => s.Student.Name))
                .ForMember(d => d.CourseTitle, o => o.MapFrom(s => s.Course.Name));
            #endregion

            #region Lecture
            CreateMap<Lecture, LectureResponse>()
                .ForMember(d => d.SectionName, o => o.MapFrom(s => s.Section.Title));

            CreateMap<Lecture, LectureToReturnDTO>()
                .ForMember(d => d.SectionName, o => o.MapFrom(s => s.Section.Title));

            CreateMap<Lecture, LectureWithSectionResponse>();

            CreateMap<Lecture, CourseWithLectureVideoResponse>();

            CreateMap<Lecture, LectureWithInstructorResponse>()
                .ForMember(d => d.SectionName, o=> o.MapFrom(s => s.Section.Title));

            CreateMap<CreatedLectureRequest, Lecture>();

            CreateMap<UpdatedLectureRequest, Lecture>();
            #endregion

            #region Sections
            CreateMap<Section, SectionWithCourseResponse>();

            CreateMap<Section, SectionToReturnDTO>()
                .ForMember(d => d.CourseName, o => o.MapFrom(s => s.Course.Name));

            CreateMap<CreateSectionRequest, Section>();

            CreateMap<UpdateSectionRequest, Section>();

            #endregion

            #region Student Lecture Progress
            CreateMap<StudentLectureProgress, StudentLectureProgressResponse>()
                .ForMember(d => d.LectureName, o => o.MapFrom(s => s.Lecture.Title));

            CreateMap<StudentLectureProgress, ProgressWithLectureResponse>()
                .ForMember(d => d.LectureName, o => o.MapFrom(s => s.Lecture.Title))
                .ForMember(d => d.VideoDuration, o => o.MapFrom(s => s.Lecture.DurationInSeconds));
            #endregion

            CreateMap<Student, StudentWithApplicationUserToReturnDTO>();

            CreateMap<Instructor, InstructorWithApplicationUserResponse>();
        }
    }
}

using AutoMapper;
using Courses.Api.Helper.Resolvers;
using Courses.Core.Models.ApplicationUsers;
using Courses.Core.Models.Courses;
using Courses.Core.Models.Enrollments;
using Courses.Core.Models.Instructors;
using Courses.Core.Models.Students;
using Courses.Core.ModelsDTO.RequestDTO.Account;
using Courses.Core.ModelsDTO.RequestDTO.Courses;
using Courses.Core.ModelsDTO.RequestDTO.Profile;
using Courses.Core.ModelsDTO.ResponseDTO.Account;
using Courses.Core.ModelsDTO.ResponseDTO.Courses;
using Courses.Core.ModelsDTO.ResponseDTO.Enrollment;
using Courses.Core.ModelsDTO.ResponseDTO.Instructors;
using Courses.Core.ModelsDTO.ResponseDTO.Lectures;
using Courses.Core.ModelsDTO.ResponseDTO.Sections;
using Courses.Core.ModelsDTO.ResponseDTO.StudentLectureProgress;

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
                .ForMember(d => d.Image, o => o.MapFrom<ImageResolver<Course, CourseResponseForInstructor>, string>(s => $"Files/CoursesImages/{s.Image}"));
            CreateMap<CreatedCourseRequest, Course>();
            CreateMap<UpdatedCourseRequest, Course>();
            #endregion

            CreateMap<Instructor, InstructorResponse>();

            #region Courses
            CreateMap<Course, CourseResponse>()
                .ForMember(d => d.CourseType, o => o.MapFrom(s => s.CourseType.Name))
                .ForMember(d => d.InstructorName, o => o.MapFrom(s => s.Instructor.Name));
            #endregion

            #region Enrollment
            CreateMap<Enrollment, EnrollmentResponse>()
                .ForMember(d => d.CourseName, o => o.MapFrom(s => s.Course.Name))
                .ForMember(d => d.StudentName, o => o.MapFrom(s => s.Student.Name));
            #endregion

            #region Lecture
            CreateMap<Lecture, LectureResponse>()
                .ForMember(d => d.SectionName, o => o.MapFrom(s => s.Section.Title));
            #endregion

            #region Sections
            CreateMap<Section, SectionResponse>()
                .ForMember(d => d.CourseName, o => o.MapFrom(s => s.Course.Name));
            #endregion

            #region Student Lecture Progress
            CreateMap<StudentLectureProgress, StudentLectureProgressResponse>()
                .ForMember(d => d.LectureName, o => o.MapFrom(s => s.Lecture.Title));
            #endregion
        }
    }
}

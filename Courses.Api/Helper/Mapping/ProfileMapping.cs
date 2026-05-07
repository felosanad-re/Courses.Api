using AutoMapper;
using Courses.Api.Helper.Resolvers;
using Courses.Core.Models.ApplicationUsers;
using Courses.Core.Models.Courses;
using Courses.Core.Models.Instructors;
using Courses.Core.Models.Students;
using Courses.Core.ModelsDTO.RequestDTO.Account;
using Courses.Core.ModelsDTO.RequestDTO.Courses;
using Courses.Core.ModelsDTO.RequestDTO.Profile;
using Courses.Core.ModelsDTO.ResponseDTO.Account;
using Courses.Core.ModelsDTO.ResponseDTO.Courses;

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
        }
    }
}

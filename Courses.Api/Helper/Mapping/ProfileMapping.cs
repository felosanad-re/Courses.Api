using AutoMapper;
using Courses.Core.Models.ApplicationUsers;
using Courses.Core.Models.Instructors;
using Courses.Core.Models.Students;
using Courses.Core.ModelsDTO.RequestDTO.Account;
using Courses.Core.ModelsDTO.RequestDTO.Profile;
using Courses.Core.ModelsDTO.ResponseDTO.Account;

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
            CreateMap<EditProfileRequest, Instructor>();
            CreateMap<EditProfileRequest, Student>();
            #endregion
        }
    }
}

using AutoMapper;
using Courses.Core.Models.ApplicationUsers;
using Courses.Core.ModelsDTO.RequestDTO;
using Courses.Core.ModelsDTO.ResponseDTO;

namespace Courses.Api.Helper.Mapping
{
    public class ProfileMapping : Profile
    {
        public ProfileMapping()
        {
            CreateMap<CreateAccountRequest, ApplicationUser>();
            CreateMap<ApplicationUser, CreateAccountResponse>();
        }
    }
}

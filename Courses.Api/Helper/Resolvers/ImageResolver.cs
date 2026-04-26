using AutoMapper;

namespace Courses.Api.Helper.Resolvers
{
    public class ImageResolver<TSource, TDestination> : IMemberValueResolver<TSource, TDestination, string, string>
    {
        private readonly IConfiguration _config;

        public ImageResolver(IConfiguration configuration)
        {
            _config = configuration;
        }

        public string Resolve(TSource source, TDestination destination, string sourceMember, string destMember, ResolutionContext context)
        {
            if (!string.IsNullOrEmpty(sourceMember))
                return $"{_config["BasePictureUrl"]}/{sourceMember}";
            return string.Empty;
        }
    }
}

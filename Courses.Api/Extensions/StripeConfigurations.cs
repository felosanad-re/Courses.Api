using Courses.Core.Options;
using Microsoft.Extensions.Options;
using Stripe;

namespace Courses.Api.Extensions
{
    public static class StripeConfigurations
    {
        public static IServiceCollection AddStripeConfig(this IServiceCollection services, IConfiguration configuration)
        {
            // Configure StripeOptions from appsettings/UserSecrets
            services.Configure<StripeOptions>(configuration.GetSection(StripeOptions.SectionName));

            // Set Stripe API key globally
            var stripeOptions = configuration.GetSection(StripeOptions.SectionName).Get<StripeOptions>() ?? new StripeOptions();
            StripeConfiguration.ApiKey = stripeOptions.SecretKey ?? string.Empty;

            return services;
        }
    }
}

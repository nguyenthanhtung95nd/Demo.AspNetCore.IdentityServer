using IdentityServer4.AccessTokenValidation;

namespace Demo.API.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureAuthenticationHandler(this IServiceCollection services) =>
            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
            .AddIdentityServerAuthentication(opt =>
            {
                opt.Authority = "https://localhost:5005";
                opt.ApiName = "demoapi";
            });
    }
}

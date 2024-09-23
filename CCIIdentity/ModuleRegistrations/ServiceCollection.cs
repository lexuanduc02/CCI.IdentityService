using CCI.Domain.Entities;
using CCI.Service;
using CCI.Service.Contractors;
using CCIIdentity.Configurations;
using Microsoft.AspNetCore.Identity;

namespace CCIIdentity;

public static class ServiceCollection
{
    public static IServiceCollection AddServiceCollection(this IServiceCollection services)
    {
        return services
                .AddScoped<UserManager<User>>()
                .AddScoped<SignInManager<User>>()
                .AddScoped<ProfileService>()
                .AddScoped<ResourceOwnerPasswordValidator>()
                .AddScoped<IEmailService, EmailService>()
                .AddScoped<IUserService, UserService>()
                .AddScoped<IOAuthService, OAuthService>()
                .AddScoped<ITokenService, TokenService>()
                .AddScoped<IPhotoService, PhotoService>()
                .AddScoped<IRoleService, RoleService>();
    }
}

using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;

namespace CCIIdentity.Configurations
{
    public class ProfileService : IProfileService
    {
        public ProfileService()
        {
        }

        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var claims = context.Subject.Claims;
            context.IssuedClaims.AddRange(claims);
            return Task.CompletedTask;
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            string sub = context.Subject.GetSubjectId();
            context.IsActive = sub != null;
            return Task.CompletedTask;
        }
    }
}

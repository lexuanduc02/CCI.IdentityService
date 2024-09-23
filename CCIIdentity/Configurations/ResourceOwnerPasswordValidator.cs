using System.Security.Claims;
using CCI.Common.Extensions;
using CCI.Domain.Entities;
using CCI.Repository;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Validation;
using Microsoft.AspNetCore.Identity;
using Serilog.Events;

namespace CCIIdentity.Configurations
{
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly ILogger<ResourceOwnerPasswordValidator> _logger;
        private const string ClassName = nameof(ResourceOwnerPasswordValidator);

        public ResourceOwnerPasswordValidator(ILogger<ResourceOwnerPasswordValidator> logger)
        {
            _logger = logger;
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            var username = context.UserName;
            var role = context.Password;

            try
            {
                if (username != null && role != null)
                {
                    var claims = new List<Claim> {
                            new Claim("role", $"{role}")
                        };

                    //Set the result
                    context.Result = new GrantValidationResult(
                        subject: username,
                        authenticationMethod: "password",
                        claims: claims
                    );

                    _logger.LogInformation("Generate Token Successfully".GeneratedLog(ClassName, LogEventLevel.Information));
                    return;
                }
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "User does not exist.");

                _logger.LogError("Generate Token Failed".GeneratedLog(ClassName, LogEventLevel.Error));
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Generate Token Failed: {ex}".GeneratedLog(ClassName, LogEventLevel.Error));
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest, "Generate Token Failed");
            }
        }
    }
}

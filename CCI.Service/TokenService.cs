using CCI.Common.Extensions;
using CCI.Model;
using CCI.Model.CommonModels;
using CCI.Model.ResponseModel;
using Duende.IdentityServer.Stores;
using IdentityModel.Client;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog.Events;

namespace CCI.Service;

public class TokenService : ITokenService
{
    private readonly ILogger<OAuthService> _logger;
    private readonly IHostingEnvironment _env;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IClientStore _clientStore;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _configuration;
    private const string ClassName = nameof(OAuthService);

    public TokenService(
        ILogger<OAuthService> logger,
        IHostingEnvironment env,
        IHttpClientFactory httpClientFactory,
        IClientStore clientStore,
        IHttpContextAccessor httpContextAccessor,
        IConfiguration configuration)
    {
        _logger = logger;
        _env = env;
        _httpClientFactory = httpClientFactory;
        _clientStore = clientStore;
        _httpContextAccessor = httpContextAccessor;
        _configuration = configuration;
    }
    public async Task<BaseResponseModel<string>> IntrospectToken(IntrospectTokenRequest request)
    {
        var host = _httpContextAccessor.HttpContext.Request;
        var address = $"{host.Scheme}://{host.Host}/connect/token";
        var client = _httpClientFactory.CreateClient();

        try
        {

            var response = await client.IntrospectTokenAsync(new TokenIntrospectionRequest
            {
                Address = address,
                ClientId = request.ClientId,
                ClientSecret = request.ClientSecret,
                Token = request.AccessToken,
                TokenTypeHint = "access_token"
            });

            if (response.IsError)
            {
                return ErrorResponse<string>(response.Error, (int)response.HttpStatusCode);
            }

            _logger.LogInformation("Introspect Token Successfully".GeneratedLog(ClassName, LogEventLevel.Information));
            return new BaseResponseModel<string>
            {
                Message = "Introspect Token Successfully",
                Success = true,
                StatusCode = (int)response.HttpStatusCode,
                Data = response.Raw
            };

        }
        catch (System.Exception ex)
        {
            return ErrorResponse<string>($"Introspect Token failed", StatusCodes.Status400BadRequest, ex);
        }
    }

    public async Task<BaseResponseModel<LoginResponseModel>> RefreshAccessToken(RefreshAccessTokenRequest request)
    {
        var host = _httpContextAccessor.HttpContext.Request;
        var address = $"{host.Scheme}://{host.Host}/connect/token";
        var client = _httpClientFactory.CreateClient();

        try
        {

            var identityServerResponse = await client.RequestRefreshTokenAsync(new RefreshTokenRequest
            {
                Address = address,
                ClientId = request.ClientId,
                ClientSecret = request.ClientSecret,
                RefreshToken = request.RefreshToken
            });

            if (identityServerResponse.IsError)
            {
                return ErrorResponse<LoginResponseModel>(identityServerResponse.Error, (int)identityServerResponse.HttpStatusCode);
            }

            var clientStore = await _clientStore.FindClientByIdAsync(request.ClientId);

            _logger.LogInformation("Refresh AccessToken Successfully".GeneratedLog(ClassName, LogEventLevel.Information));
            return new BaseResponseModel<LoginResponseModel>
            {
                Message = "Đăng nhập thành công!",
                Success = true,
                StatusCode = (int)identityServerResponse.HttpStatusCode,
                Data = new LoginResponseModel()
                {
                    Token = new Token()
                    {
                        AccessToken = identityServerResponse.AccessToken,
                        RefreshToken = identityServerResponse.RefreshToken,
                        IdToken = identityServerResponse.IdentityToken,
                        ExpireTime = identityServerResponse.ExpiresIn,
                    },
                    RedirectUri = clientStore.RedirectUris.FirstOrDefault(),
                },
            };
        }
        catch (Exception ex)
        {
            return ErrorResponse<LoginResponseModel>($"Login failed", StatusCodes.Status400BadRequest, ex);
        }
    }

    private BaseResponseModel<T> ErrorResponse<T>(string message, int statusCode, Exception ex = null)
    {
        _logger.LogError($"{message}: {ex?.ToString() ?? ""}".GeneratedLog(ClassName, LogEventLevel.Error));

        return new BaseResponseModel<T>
        {
            Success = false,
            StatusCode = statusCode,
            Message = message,
        };
    }
}

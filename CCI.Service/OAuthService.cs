using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog.Events;
using CCI.Common.Extensions;
using CCI.Domain.Entities;
using CCI.Model.CommonModels;
using CCI.Model.OAuthModels;
using CCI.Model.ResponseModel;
using CCI.Service.Contractors;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Stores;
using IdentityModel.Client;
using MimeKit;
using Microsoft.AspNetCore.Hosting;
using CCI.Repository;

namespace CCI.Service
{
    public class OAuthService : IOAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<OAuthService> _logger;
        private readonly IEmailService _emailService;
        private readonly IHostingEnvironment _env;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IClientStore _clientStore;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private const string ClassName = nameof(OAuthService);

        public OAuthService(
            UserManager<User> userManager,
            ILogger<OAuthService> logger,
            IEmailService emailService,
            IHostingEnvironment env,
            IHttpClientFactory httpClientFactory,
            IClientStore clientStore,
            SignInManager<User> signInManager,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper,
            IConfiguration configuration,
            IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _logger = logger;
            _emailService = emailService;
            _env = env;
            _httpClientFactory = httpClientFactory;
            _clientStore = clientStore;
            _signInManager = signInManager;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _configuration = configuration;
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponseModel<string>> ChangePassword(ChangePasswordModel request)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(request.Id.ToString());

                if (user == null)
                {
                    return ErrorResponse<string>("User does not exist", StatusCodes.Status400BadRequest);
                }

                await _userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);
                _logger.LogInformation($"{user.UserName} change password successfully");

                return new BaseResponseModel<string>
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Password changed successfully",
                };
            }
            catch (Exception ex)
            {
                return ErrorResponse<string>($"Password change failed", StatusCodes.Status400BadRequest, ex);
            }
        }

        public async Task<BaseResponseModel<LoginResponseModel>> Login(LoginRequest request)
        {
            var host = _httpContextAccessor.HttpContext.Request;
            var address = $"{host.Scheme}://{host.Host}/connect/token";
            var client = _httpClientFactory.CreateClient();

            try
            {
                var user = await _userManager.FindByNameAsync(request.Username);
                if (user.IsActive == false)
                {
                    return ErrorResponse<LoginResponseModel>("Account has been disabled!", StatusCodes.Status423Locked);
                }

                var checkPassword = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
                if (!checkPassword.Succeeded)
                {
                    return ErrorResponse<LoginResponseModel>("Login failed!", StatusCodes.Status400BadRequest);
                }

                var role = await _unitOfWork.UserRepository.GetRoleAsync(user.Id.ToString());

                var checkPermission = await _unitOfWork.ClientRoleRepository
                                        .FindByAsync(x => x.ClientId == request.ClientId && x.RoleName == role);
                if (!checkPermission.Any())
                {
                    return ErrorResponse<LoginResponseModel>("You do not have permission to access this application!", StatusCodes.Status403Forbidden);
                }

                var identityServerResponse = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
                {
                    Address = address,
                    ClientId = request.ClientId,
                    ClientSecret = request.ClientSecret,
                    UserName = user.Id.ToString(),
                    Password = role
                });

                if (identityServerResponse.IsError)
                {
                    return ErrorResponse<LoginResponseModel>(identityServerResponse.Error, (int)identityServerResponse.HttpStatusCode);
                }

                var clientStore = await _clientStore.FindClientByIdAsync(request.ClientId);

                _logger.LogInformation($"User '{user.Id}' Authenticate Successfully".GeneratedLog(ClassName, LogEventLevel.Information));
                return new BaseResponseModel<LoginResponseModel>
                {
                    Success = true,
                    StatusCode = (int)identityServerResponse.HttpStatusCode,
                    Data = new LoginResponseModel()
                    {
                        UserProfile = new UserProfile()
                        {
                            UserId = user.Id,
                            Username = user.UserName,
                            Role = role,
                            Email = user.Email
                        },

                        Token = new Token()
                        {
                            AccessToken = identityServerResponse.AccessToken,
                            RefreshToken = identityServerResponse.RefreshToken,
                            ExpireTime = identityServerResponse.ExpiresIn,
                        },

                        RedirectUri = clientStore.RedirectUris.FirstOrDefault(),
                    },
                };
            }
            catch (Exception ex)
            {
                return ErrorResponse<LoginResponseModel>($"Login failed: {ex}", StatusCodes.Status400BadRequest);
            }
        }

        public async Task<BaseResponseModel<string>> Logout(LogoutRequest request)
        {
            var host = _httpContextAccessor.HttpContext.Request;
            var address = $"{host.Scheme}://{host.Host}/connect/revocation";
            var client = _httpClientFactory.CreateClient();

            try
            {
                var clientStore = await _clientStore.FindClientByIdAsync(request.ClientId);

                if (clientStore == null)
                {
                    return ErrorResponse<string>("Invalid Client", StatusCodes.Status400BadRequest);
                };

                var revokeAccessToken = await client.RevokeTokenAsync(new TokenRevocationRequest
                {
                    Address = address,
                    ClientId = request.ClientId,
                    ClientSecret = request.ClientSecret,
                    Token = request.AccessToken,
                });

                if (revokeAccessToken.IsError)
                {
                    return ErrorResponse<string>(revokeAccessToken.Error, (int)revokeAccessToken.HttpStatusCode);
                }

                var revokeRefreshToken = await client.RevokeTokenAsync(new TokenRevocationRequest
                {
                    Address = address,
                    ClientId = request.ClientId,
                    ClientSecret = request.ClientSecret,
                    Token = request.RefreshToken,
                });

                if (revokeRefreshToken.IsError)
                {
                    return ErrorResponse<string>(revokeRefreshToken.Error, (int)revokeRefreshToken.HttpStatusCode);
                }

                await _signInManager.SignOutAsync();

                _logger.LogInformation("Logout Successfully!");
                return new BaseResponseModel<string>()
                {
                    Success = true,
                    Message = "Logout Successfully!",
                    StatusCode = StatusCodes.Status200OK,
                    Data = clientStore.PostLogoutRedirectUris.FirstOrDefault(),
                };
            }
            catch (Exception ex)
            {
                return ErrorResponse<string>($"Logout Failed!: {ex}", StatusCodes.Status400BadRequest);
            }
        }

        public async Task<BaseResponseModel<string>> RecoveryPassword(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);

                if (user == null)
                {
                    return ErrorResponse<string>("Email is not registered", StatusCodes.Status400BadRequest);
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                if (token == null)
                {
                    return ErrorResponse<string>("Failed to generate password reset token", StatusCodes.Status503ServiceUnavailable);
                }

                var oauthClient = new Uri(_configuration["OauthClientUri"]);
                var resetLink = $"{oauthClient.AbsoluteUri}reset-password?token={token}&email={email}";
                var webRoot = _env.WebRootPath;
                var pathToFile = webRoot + Path.DirectorySeparatorChar.ToString() + "HtmlTemplates" + Path.DirectorySeparatorChar.ToString() + "ForgotPassword.html";
                var builder = new BodyBuilder();

                using (StreamReader SourceReader = File.OpenText(pathToFile))
                {
                    builder.HtmlBody = SourceReader.ReadToEnd();
                }

                builder.HtmlBody = builder.HtmlBody.Replace("{{reset-link}}", resetLink);

                var body = builder.HtmlBody;

                _emailService.SendEmail(email, "Reset mật khẩu", body, true);

                return new BaseResponseModel<string>
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "The link to change the new password has been sent to the email registered on the system"
                };
            }
            catch (Exception ex)
            {
                return ErrorResponse<string>($"Reset password failed: {ex}", StatusCodes.Status400BadRequest);
            }
        }

        public async Task<BaseResponseModel<User>> Register(RegisterModel request)
        {
            try
            {
                var checkUserName = await _userManager.FindByNameAsync(request.Username);
                if (checkUserName != null)
                {
                    return ErrorResponse<User>("Username already exists!", StatusCodes.Status400BadRequest);
                }

                var checkEmail = await _userManager.FindByEmailAsync(request.Email);
                if (checkEmail != null)
                {
                    return ErrorResponse<User>("The Email Was Registered!", StatusCodes.Status400BadRequest);
                }

                var user = new User();

                _mapper.Map(request, user);
                user.IsActive = true;
                var result = await _userManager.CreateAsync(user, request.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, request.Role.IsNullOrEmpty() ? "User" : request.Role);

                    _logger.LogInformation("Create New Account Successfully".GeneratedLog(ClassName, LogEventLevel.Information));
                    return new BaseResponseModel<User>
                    {
                        Success = true,
                        StatusCode = StatusCodes.Status201Created,
                        Message = "Sign Up Success!",
                        Data = user,
                    };
                }

                return ErrorResponse<User>($"Registration Failed", StatusCodes.Status400BadRequest);
            }
            catch (Exception ex)
            {
                return ErrorResponse<User>($"Registration Failed", StatusCodes.Status400BadRequest, ex);
            }
        }

        public async Task<BaseResponseModel<LoginResponseModel>> ReLogin(ReLoginRequest request)
        {
            try
            {
                var checkUser = false;

                var user = await _userManager.FindByNameAsync(request.Username);

                if (user != null && user.Email == request.Email)
                    checkUser = true;

                if (!checkUser)
                    return ErrorResponse<LoginResponseModel>("Login failed", StatusCodes.Status400BadRequest);

                var host = _httpContextAccessor.HttpContext.Request;
                var address = $"{host.Scheme}://{host.Host}/connect/token";
                var client = _httpClientFactory.CreateClient();

                var identityServerResponse = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
                {
                    Address = address,
                    ClientId = request.ClientId,
                    ClientSecret = request.ClientSecret,
                    UserName = request.Username,
                });

                if (identityServerResponse.IsError)
                {
                    return ErrorResponse<LoginResponseModel>(identityServerResponse.Error, (int)identityServerResponse.HttpStatusCode);
                }

                var clientStore = await _clientStore.FindClientByIdAsync(request.ClientId);

                var roles = await _userManager.GetRolesAsync(user);

                var checkPermission = await _unitOfWork.ClientRoleRepository.FindByAsync(x => x.ClientId == request.ClientId && x.RoleName == roles.FirstOrDefault());

                if (!checkPermission.Any())
                {
                    return ErrorResponse<LoginResponseModel>("You do not have permission to access this application!", StatusCodes.Status403Forbidden);
                }

                _logger.LogInformation($"User '{user.Id}' Authenticate Successfully".GeneratedLog(ClassName, LogEventLevel.Information));
                return new BaseResponseModel<LoginResponseModel>
                {
                    Success = true,
                    StatusCode = (int)identityServerResponse.HttpStatusCode,
                    Data = new LoginResponseModel()
                    {
                        UserProfile = new UserProfile()
                        {
                            UserId = user.Id,
                            Username = user.UserName,
                            Email = user.Email,
                            Role = roles.FirstOrDefault(),
                        },
                        Token = new Token()
                        {
                            AccessToken = identityServerResponse.AccessToken,
                            RefreshToken = identityServerResponse.RefreshToken,
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

        public async Task<BaseResponseModel<string>> ResetPassword(ResetPasswordModel request)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);

                if (user == null)
                {
                    return ErrorResponse<string>("Email has not been registered on the system!", StatusCodes.Status400BadRequest);
                }
                var result = await _userManager.ResetPasswordAsync(user, request.Token, request.Password);

                if (!result.Succeeded)
                {
                    return ErrorResponse<string>("Password change failed!", StatusCodes.Status400BadRequest);
                }

                _logger.LogInformation("Password changed successfully".GeneratedLog(ClassName, LogEventLevel.Information));
                return new BaseResponseModel<string>
                {
                    Success = true,
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Password changed successfully"
                };
            }
            catch (Exception ex)
            {
                return ErrorResponse<string>("Reset password failed", StatusCodes.Status400BadRequest, ex);
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
}

using AutoMapper;
using CCI.Common.Extensions;
using CCI.Domain.Entities;
using CCI.Model.CommonModels;
using CCI.Model.Users;
using CCI.Repository;
using CCI.Repository.Contractors;
using CCI.Service.Contractors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using MimeKit;
using Serilog.Events;

namespace CCI.Service
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<User> _roleManager;
        private readonly IPhotoService _photoService;
        private readonly ILogger<UserService> _logger;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;
        private readonly IHostingEnvironment _env;
        private const string ClassName = nameof(UserService);
        public UserService(IUnitOfWork unitOfWork,
            UserManager<User> userManager,
            ILogger<UserService> logger,
            IMapper mapper,
            IPhotoService photoService,
            IEmailService emailService,
            IHostingEnvironment env)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _logger = logger;
            _mapper = mapper;
            _photoService = photoService;
            _emailService = emailService;
            _env = env;
        }

        public async Task<BaseResponseModel<UserViewModel>> GetUser(Guid id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id.ToString());

                var roles = await _userManager.GetRolesAsync(user);

                if (user != null)
                {
                    var result = new UserViewModel()
                    {
                        Id = user.Id,
                        Username = user.UserName,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        ImageLink = user.ImageLink,
                        Gender = user.Gender,
                        Dob = user.Dob,
                        IsActive = user.IsActive,
                        PhoneNumber = user.PhoneNumber,
                        Email = user.Email,
                        Role = roles.FirstOrDefault(),
                    };

                    _logger.LogInformation("Get User Information Successfully!".GeneratedLog(ClassName, LogEventLevel.Information));
                    return new BaseResponseModel<UserViewModel>()
                    {
                        Message = "Get User Information Successfully!",
                        Success = true,
                        StatusCode = 200,
                        Data = result
                    };
                }

                return ErrorResponse<UserViewModel>("User Not Found", StatusCodes.Status400BadRequest);
            }
            catch (Exception ex)
            {
                return ErrorResponse<UserViewModel>("User Not Found!", StatusCodes.Status400BadRequest, ex);
            }
        }

        public async Task<BaseResponseModel<string>> UpdateUser(UpdateUserRequest request)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(request.Id.ToString());

                if (user == null)
                {
                    return ErrorResponse<string>("User Not Found", StatusCodes.Status400BadRequest);
                }

                if (user.Email != request.Email)
                {
                    var checkEmail = await _userManager.FindByEmailAsync(request.Email);
                    _logger.LogInformation("Checking Email".GeneratedLog(ClassName, LogEventLevel.Information));

                    if (checkEmail != null)
                    {
                        return ErrorResponse<string>("Email Already Exist!", StatusCodes.Status400BadRequest);
                    }
                }

                _mapper.Map(request, user);

                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    return ErrorResponse<string>("Cannot Update User Information", StatusCodes.Status400BadRequest);
                }

                _logger.LogInformation("Update User Information Successfully!".GeneratedLog(ClassName, LogEventLevel.Information));
                return new BaseResponseModel<string>()
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Update User Information Successfully!",
                };
            }
            catch (Exception ex)
            {
                return ErrorResponse<string>("Cannot Update User Information!", StatusCodes.Status400BadRequest, ex);
            }
        }

        public async Task<BaseResponseModel<bool>> LockUser(Guid userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());

                if (user == null)
                {
                    return ErrorResponse<bool>("User Not Found", StatusCodes.Status400BadRequest);
                }

                if (user.IsActive == true)
                {
                    var webRoot = _env.WebRootPath;

                    var pathToFile = webRoot + Path.DirectorySeparatorChar.ToString() + "HtmlTemplates"
                        + Path.DirectorySeparatorChar.ToString() + "LockAccountNotification.html";

                    var builder = new BodyBuilder();

                    using (StreamReader SourceReader = File.OpenText(pathToFile))
                    {
                        builder.HtmlBody = SourceReader.ReadToEnd();
                    }

                    var body = builder.HtmlBody;

                    _emailService.SendEmail(user.Email, "Tài khoản bị vô hiệu hóa", body, true);

                    user.IsActive = false;
                }

                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    return ErrorResponse<bool>("Cannot Lock User Account!", StatusCodes.Status400BadRequest);
                }

                _logger.LogInformation("Lock User Account Successfully!".GeneratedLog(ClassName, LogEventLevel.Information));
                return new BaseResponseModel<bool>()
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Lock User Account Successfully"
                };
            }
            catch (System.Exception ex)
            {
                return ErrorResponse<bool>("Cannot Lock User Account!", StatusCodes.Status400BadRequest, ex);
            }
        }

        public async Task<BaseResponseModel<bool>> UnLockUser(Guid userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());

                if (user == null)
                {
                    return ErrorResponse<bool>("User Not Found", StatusCodes.Status400BadRequest);
                }

                if (user.IsActive == false)
                {
                    var webRoot = _env.WebRootPath;

                    var pathToFile = webRoot + Path.DirectorySeparatorChar.ToString() + "HtmlTemplates"
                        + Path.DirectorySeparatorChar.ToString() + "UnlockAccountNotification.html";

                    var builder = new BodyBuilder();

                    using (StreamReader SourceReader = File.OpenText(pathToFile))
                    {
                        builder.HtmlBody = SourceReader.ReadToEnd();
                    }

                    var body = builder.HtmlBody;

                    _emailService.SendEmail(user.Email, "Tài khoản bị vô hiệu hóa", body, true);

                    user.IsActive = true;
                }

                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    return ErrorResponse<bool>("Cannot UnLock User Account!", StatusCodes.Status400BadRequest);
                }

                _logger.LogInformation("UnLock User Account Successfully!".GeneratedLog(ClassName, LogEventLevel.Information));
                return new BaseResponseModel<bool>()
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "UnLock User Account Succeed!"
                };
            }
            catch (System.Exception ex)
            {
                return ErrorResponse<bool>("Cannot UnLock User Account!", StatusCodes.Status400BadRequest, ex);
            }
        }

        public async Task<BaseResponseModel<string>> UploadImage(UploadAvatarModel model)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(model.UserId.ToString());

                if (user == null)
                {
                    return ErrorResponse<string>("User Not Found", StatusCodes.Status400BadRequest);
                }

                _photoService.DeletePhotoAsync($"profile_images/{user.Id}");

                var image = new UploadImageModel()
                {
                    File = model.File,
                    FileName = $"{DateTimeOffset.Now.ToUnixTimeMilliseconds()}{Path.GetExtension(model.File.FileName)}",
                    With = 180,
                    Height = 180,
                    PublicId = user.Id.ToString(),
                    Folder = "profile_images"
                };

                var uploadResult = await _photoService.AddPhotoAsync(image);

                if (uploadResult.Error != null)
                {
                    return ErrorResponse<string>("Cannot Upload Image", StatusCodes.Status400BadRequest);
                }

                user.ImageLink = uploadResult.SecureUrl.AbsoluteUri;
                await _userManager.UpdateAsync(user);

                _logger.LogInformation("Upload Image Successfully".GeneratedLog(ClassName, LogEventLevel.Information));
                return new BaseResponseModel<string>()
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Upload Image Successfully",
                    Data = user.ImageLink,
                };
            }
            catch (Exception ex)
            {
                return ErrorResponse<string>("Cannot Upload Image", StatusCodes.Status400BadRequest, ex);
            }
        }

        public async Task<BaseResponseModel<bool>> RemoveImage(string publicId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(publicId);

                if (user == null)
                {
                    return ErrorResponse<bool>("User Not Found", StatusCodes.Status400BadRequest);
                }

                user.ImageLink = null;

                await _userManager.UpdateAsync(user);

                _photoService.DeletePhotoAsync($"profile_images/{publicId}");

                _logger.LogInformation("Remove Avatar Successfully".GeneratedLog(ClassName, LogEventLevel.Information));

                return new BaseResponseModel<bool>
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Remove Avatar Successfully",
                };
            }
            catch (Exception ex)
            {
                return ErrorResponse<bool>("Cannot Remove Avatar", StatusCodes.Status400BadRequest, ex);
            }
        }

        public async Task<BaseResponseModel<List<UserViewModel>>> GetAllUser()
        {
            try
            {
                var result = await _unitOfWork.UserRepository.GetAllUserAsync();

                if (result == null)
                {
                    return ErrorResponse<List<UserViewModel>>("Cannot Get List User", StatusCodes.Status400BadRequest);
                }

                _logger.LogInformation("Get List User Successfully!".GeneratedLog(ClassName, LogEventLevel.Information));

                return new BaseResponseModel<List<UserViewModel>>()
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Get List User Successfully!",
                    Data = result,
                };
            }
            catch (Exception ex)
            {
                return ErrorResponse<List<UserViewModel>>("Cannot Get List User", StatusCodes.Status400BadRequest, ex);
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

using CCI.Common.Extensions;
using CCI.Domain.EF;
using CCI.Domain.Entities;
using CCI.Model;
using CCI.Model.CommonModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Serilog.Events;

namespace CCI.Service;

public class RoleService : IRoleService
{
    private readonly UserManager<User> _userManager;
    private readonly ILogger<RoleService> _logger;
    private DataContext _context;
    private const string ClassName = nameof(RoleService);

    public RoleService(UserManager<User> userManager,
            DataContext context,
            ILogger<RoleService> logger)
    {
        _userManager = userManager;
        _context = context;
        _logger = logger;
    }

    public async Task<BaseResponseModel<bool>> RoleAssign(RoleAssignRequest request)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(request.Id.ToString());
            if (user == null)
            {
                return ErrorResponse<bool>("User Not Found", StatusCodes.Status400BadRequest);
            }

            var roleIsExist = _context.Roles.Where(x => x.Name == request.Role).FirstOrDefault();

            if (roleIsExist == null)
            {
                return ErrorResponse<bool>("Role Not Found", StatusCodes.Status400BadRequest);
            }

            var isInRole = _context.UserRoles.Where(x => x.UserId == request.Id && x.RoleId == roleIsExist.Id).FirstOrDefault();

            if (isInRole != null)
            {
                return ErrorResponse<bool>("User Is In Role", StatusCodes.Status400BadRequest);
            }

            var removeRole = _context.UserRoles.Where(x => x.UserId == request.Id).FirstOrDefault();

            if (removeRole != null)
            {
                _context.UserRoles.Remove(removeRole);
                await _context.SaveChangesAsync();
            }

            var assignRole = await _userManager.AddToRoleAsync(user, request.Role);

            if (!assignRole.Succeeded)
            {
                return ErrorResponse<bool>("Failed To Assign User To Role", StatusCodes.Status400BadRequest);
            }

            _logger.LogInformation("Assign User To Role Successfully".GeneratedLog(ClassName, LogEventLevel.Information));
            return new BaseResponseModel<bool>()
            {
                Message = "Assign User To Role Successfully",
                Success = true,
                StatusCode = StatusCodes.Status200OK,
            };
        }
        catch (Exception ex)
        {
            return ErrorResponse<bool>("Failed To Assign User To Role", StatusCodes.Status400BadRequest, ex);
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

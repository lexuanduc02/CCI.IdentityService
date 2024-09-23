using CCI.Model.Users;
using CCI.Service.Contractors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CCIIdentity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("get-user")]
        public async Task<IActionResult> GetUser([FromQuery] Guid idUser)
        {
            var result = await _userService.GetUser(idUser);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        [Authorize("admin_recruiter")]
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _userService.GetAllUser();

            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("update-user")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest request)
        {
            var result = await _userService.UpdateUser(request);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("update-avatar")]
        public async Task<IActionResult> UpdateAvatar([FromForm] UploadAvatarModel model)
        {
            var result = await _userService.UploadImage(model);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("remove-avatar")]
        public async Task<IActionResult> RemoveAvatar(string publicId)
        {
            var result = await _userService.RemoveImage(publicId);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        [Authorize(Policy = "admin")]
        [HttpGet("lock-account")]
        public async Task<IActionResult> LockAccount([FromQuery] Guid userId)
        {
            var result = await _userService.LockUser(userId);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        [Authorize(Policy = "admin")]
        [HttpGet("unlock-account")]
        public async Task<IActionResult> UnLockAccount([FromQuery] Guid userId)
        {
            var result = await _userService.UnLockUser(userId);

            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}

using CCI.Model;
using CCI.Model.OAuthModels;
using CCI.Service;
using CCI.Service.Contractors;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CCIIdentity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OAuthsController : ControllerBase
    {
        private readonly IOAuthService _oauthService;
        private readonly ITokenService _tokenService;
        public OAuthsController(IOAuthService oauthService, ITokenService tokenService)
        {
            _oauthService = oauthService;
            _tokenService = tokenService;
        }

        [HttpPost("recovery")]
        public async Task<IActionResult> Recovery([FromBody] RecoveryPasswordModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _oauthService.RecoveryPassword(request.Email);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _oauthService.ResetPassword(request);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _oauthService.Register(request);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        [Authorize("user_profile")]
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _oauthService.ChangePassword(request);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _oauthService.Login(request);

            return result.Success ? Ok(result) : BadRequest(result);

        }

        [HttpPost("re-login")]
        public async Task<IActionResult> ReLogin([FromBody] ReLoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _oauthService.ReLogin(request);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("refresh-access-token")]
        public async Task<IActionResult> RefreshAccessTokenRequest([FromBody] RefreshAccessTokenRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _tokenService.RefreshAccessToken(request);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("introspect-token")]
        public async Task<IActionResult> IntrospectTokenRequest([FromBody] IntrospectTokenRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _tokenService.IntrospectToken(request);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await HttpContext.SignOutAsync();

            var result = await _oauthService.Logout(request);

            return result.Success ? Ok(result) : BadRequest(result);
        }
        // [HttpPost("tokens/cancel")]
        // public async Task<IActionResult> CancelAccessToken()
        // {
        //     await _tokenManager.DeactivateCurrentAsync();

        //     return NoContent();
        // }
    }
}

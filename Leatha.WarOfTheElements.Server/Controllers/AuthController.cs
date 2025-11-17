using Leatha.WarOfTheElements.Common.Communication.Messages.Requests;
using Leatha.WarOfTheElements.Common.Communication.Messages.Responses;
using Leatha.WarOfTheElements.Server.Objects.Validations;
using Leatha.WarOfTheElements.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Leatha.WarOfTheElements.Server.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("signup")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationFailureResponse), StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
        //[ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SignUp([FromBody] SignupRequest request)
        {
            var response = await _authService.CreatePlayerAsync(request);
            return response.Match<IActionResult>(
                Ok,
                failed => BadRequest(failed.MapToResponse()));
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationFailureResponse), StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
        //[ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var response = await _authService.AuthenticateAsync(request);
            return response.Match<IActionResult>(
                Ok,
                failed => BadRequest(failed.MapToResponse()));
        }

        [HttpPost("logout")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [Authorize] // Requires authentication
        public async Task<IActionResult> LogoutAsync([FromBody] LogoutRequest request)
        {
            var result = await _authService.RevokeRefreshTokenAsync(request.PlayerId, request.RefreshToken);
            return Ok(result);
        }


        [HttpPost("refresh-token")]
        [ProducesResponseType(typeof(RefreshTokenResponse), StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenRequest request)
        {
            var response = await _authService.RefreshTokenAsync(request.PlayerId, request.RefreshToken);
            return response.Match<IActionResult>(
                Ok,
                Unauthorized);
            //NotFound);
        }

        [HttpPost("validate-token")]
        [ProducesResponseType(typeof(ValidateTokenResponse), StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
        //[ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ValidateTokenAsync([FromBody] ValidateTokenRequest request)
        {
            var response = await _authService.ValidateTokenAsync(request.AccessToken);
            return Ok(response);
        }
    }
}

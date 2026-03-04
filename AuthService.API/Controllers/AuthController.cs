using AuthService.Application.DTOs;
using AuthService.Application.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController(RegisterUseCase registerUseCase, LoginUseCase loginUseCase) : ControllerBase
    {
        [HttpPost("register")]
        [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ApiResponse<AuthResponse>>> Register([FromBody] RegisterRequest request, CancellationToken ct)
        {
            var result = await registerUseCase.ExecuteAsync(request, ct);
            return result is null
                ? Conflict(ApiResponse<AuthResponse>.FailureResponse("User with the same email already exists.", "USER_ALREADY_EXISTS"))
                : Ok(ApiResponse<AuthResponse>.SuccessResponse(result, "User registered successfully."));
        }
        [HttpPost("login")]
        [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse<AuthResponse>>> Login([FromBody] LoginRequest request, CancellationToken ct)
        {
            var result = await loginUseCase.ExecuteAsync(request, ct);
            return result is null
                ? Unauthorized(ApiResponse<AuthResponse>.FailureResponse("Invalid email or password.", "INVALID_CREDENTIALS"))
                : Ok(ApiResponse<AuthResponse>.SuccessResponse(result, "Login successful."));
        }
        [HttpGet("health")]
        public IActionResult Health() => Ok(new { Service = "AuthService",Status = "Healthy", Timestamp = DateTime.UtcNow });
    }
}

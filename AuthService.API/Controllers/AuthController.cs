using AuthService.Application.DTOs;
using AuthService.Application.UseCases;
using AuthService.Domain.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AuthService.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController(
        RegisterUseCase registerUseCase,
        LoginUseCase loginUseCase,
        OAuthLoginUseCase oAuthLoginUseCase,
        IOAuthCodeStore codeStore,
        IConfiguration configuration) : ControllerBase
    {
        // ── Helpers ────────────────────────────────────────────────
        private string FrontendUrl =>
            configuration["Frontend:BaseUrl"] ?? "http://localhost:3000";

        private IActionResult RedirectToError() =>
            Redirect($"{FrontendUrl}/login?error=auth_failed");

        private IActionResult RedirectWithCode(string code) =>
            Redirect($"{FrontendUrl}/oauth/callback?code={code}");

        // ── Existing endpoints — unchanged ─────────────────────────
        [HttpPost("register")]
        [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ApiResponse<AuthResponse>>> Register(
            [FromBody] RegisterRequest request, CancellationToken ct)
        {
            var result = await registerUseCase.ExecuteAsync(request, ct);
            return result is null
                ? Conflict(ApiResponse<AuthResponse>.FailureResponse(
                    "User with the same email already exists.", "USER_ALREADY_EXISTS"))
                : Ok(ApiResponse<AuthResponse>.SuccessResponse(
                    result, "User registered successfully."));
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse<AuthResponse>>> Login(
            [FromBody] LoginRequest request, CancellationToken ct)
        {
            var result = await loginUseCase.ExecuteAsync(request, ct);
            return result is null
                ? Unauthorized(ApiResponse<AuthResponse>.FailureResponse(
                    "Invalid email or password.", "INVALID_CREDENTIALS"))
                : Ok(ApiResponse<AuthResponse>.SuccessResponse(
                    result, "Login successful."));
        }

        // ── OAuth — Google ─────────────────────────────────────────
        [HttpGet("google")]
        public IActionResult GoogleLogin()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(GoogleCallback))
            };
            return Challenge(properties, "Google");
        }

        [HttpGet("google/callback")]
        public async Task<IActionResult> GoogleCallback(CancellationToken ct)
        {
            var result = await HttpContext.AuthenticateAsync("Google");
            if (!result.Succeeded) return RedirectToError();

            try
            {
                var email = result.Principal.FindFirstValue(ClaimTypes.Email)!;
                var name = result.Principal.FindFirstValue(ClaimTypes.Name)
                                 ?? email.Split('@')[0];
                var providerId = result.Principal.FindFirstValue(ClaimTypes.NameIdentifier)!;

                var auth = await oAuthLoginUseCase.ExecuteAsync(
                    email, name, "Google", providerId, ct);

                var code = codeStore.GenerateCode(new AuthCodePayload
                {
                    Token = auth.Token,
                    Name = auth.Name,
                    Email = auth.Email,
                    Role = auth.Role,
                    UserId = auth.UserId.ToString()
                });

                return RedirectWithCode(code);
            }
            catch
            {
                return RedirectToError();
            }
        }

        // ── OAuth — GitHub ─────────────────────────────────────────
        [HttpGet("github")]
        public IActionResult GitHubLogin()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(GitHubCallback))
            };
            return Challenge(properties, "GitHub");
        }

        [HttpGet("github/callback")]
        public async Task<IActionResult> GitHubCallback(CancellationToken ct)
        {
            var result = await HttpContext.AuthenticateAsync("GitHub");
            if (!result.Succeeded) return RedirectToError();

            try
            {
                var email = result.Principal.FindFirstValue(ClaimTypes.Email)!;
                var name = result.Principal.FindFirstValue(ClaimTypes.Name)
                                 ?? result.Principal.FindFirstValue("urn:github:login")
                                 ?? email.Split('@')[0];
                var providerId = result.Principal.FindFirstValue(ClaimTypes.NameIdentifier)!;

                var auth = await oAuthLoginUseCase.ExecuteAsync(
                    email, name, "GitHub", providerId, ct);

                var code = codeStore.GenerateCode(new AuthCodePayload
                {
                    Token = auth.Token,
                    Name = auth.Name,
                    Email = auth.Email,
                    Role = auth.Role,
                    UserId = auth.UserId.ToString()
                });

                return RedirectWithCode(code);
            }
            catch
            {
                return RedirectToError();
            }
        }

        // ── Exchange code for JWT ──────────────────────────────────
        [HttpGet("oauth/token")]
        public IActionResult ExchangeCode([FromQuery] string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return BadRequest(ApiResponse<AuthResponse>.FailureResponse(
                    "Code is required.", "INVALID_CODE"));

            var payload = codeStore.ConsumeCode(code);

            if (payload is null)
                return Unauthorized(ApiResponse<AuthResponse>.FailureResponse(
                    "Code is invalid or expired.", "CODE_EXPIRED"));

            var response = new AuthResponse
            {
                Token = payload.Token,
                Name = payload.Name,
                Email = payload.Email,
                Role = payload.Role,
                UserId = Guid.Parse(payload.UserId)
            };

            return Ok(ApiResponse<AuthResponse>.SuccessResponse(
                response, "Token exchange successful."));
        }

        [HttpGet("health")]
        public IActionResult Health() => Ok(new
        {
            Service = "AuthService",
            Status = "Healthy",
            Timestamp = DateTime.UtcNow
        });
    }
}
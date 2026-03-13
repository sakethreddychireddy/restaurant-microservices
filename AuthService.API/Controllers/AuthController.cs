using AuthService.Application.DTOs;
using AuthService.Application.UseCases;
using AuthService.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http.Json;
using System.Text.Json;

namespace AuthService.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController(
        RegisterUseCase registerUseCase,
        LoginUseCase loginUseCase,
        OAuthLoginUseCase oAuthLoginUseCase,
        IOAuthCodeStore codeStore,
        IConfiguration configuration,
        IMemoryCache cache) : ControllerBase
    {
        // ── Helpers ────────────────────────────────────────────────
        private string FrontendUrl =>
            configuration["Frontend:BaseUrl"] ?? "http://localhost:3000";

        private IActionResult RedirectToError() =>
            Redirect($"{FrontendUrl}/login?error=auth_failed");

        private IActionResult RedirectWithCode(string code) =>
            Redirect($"{FrontendUrl}/oauth/callback?code={code}");

        private static string GenerateState() =>
            Convert.ToBase64String(
                System.Security.Cryptography.RandomNumberGenerator.GetBytes(16))
                .Replace("+", "-").Replace("/", "_").TrimEnd('=');

        // ── Existing endpoints ─────────────────────────────────────
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

        // ── GitHub OAuth — manual flow ─────────────────────────────
        [HttpGet("github")]
        public IActionResult GitHubLogin()
        {
            var state = GenerateState();
            var clientId = configuration["OAuth:GitHub:ClientId"];
            var redirectUri = $"{FrontendUrl}/api/auth/github/callback";

            // store state in memory (5 minutes)
            cache.Set($"oauth_state:{state}", true, TimeSpan.FromMinutes(5));

            var url = "https://github.com/login/oauth/authorize" +
                      $"?client_id={clientId}" +
                      $"&redirect_uri={Uri.EscapeDataString(redirectUri)}" +
                      $"&scope={Uri.EscapeDataString("user:email")}" +
                      $"&state={state}";

            return Redirect(url);
        }

        [HttpGet("github/callback")]
        public async Task<IActionResult> GitHubCallback(
            [FromQuery] string code,
            [FromQuery] string state,
            CancellationToken ct)
        {
            // verify state
            if (string.IsNullOrEmpty(state) ||
                !cache.TryGetValue($"oauth_state:{state}", out _))
                return RedirectToError();

            cache.Remove($"oauth_state:{state}");

            try
            {
                var accessToken = await ExchangeGitHubCode(code, ct);
                if (accessToken is null) return RedirectToError();

                var userInfo = await GetGitHubUserInfo(accessToken, ct);
                if (userInfo is null) return RedirectToError();

                var auth = await oAuthLoginUseCase.ExecuteAsync(
                    userInfo.Value.Email,
                    userInfo.Value.Name,
                    "GitHub",
                    userInfo.Value.Id.ToString(),
                    ct);

                var oneTimeCode = codeStore.GenerateCode(new AuthCodePayload
                {
                    Token = auth.Token,
                    Name = auth.Name,
                    Email = auth.Email,
                    Role = auth.Role,
                    UserId = auth.UserId.ToString()
                });

                return RedirectWithCode(oneTimeCode);
            }
            catch
            {
                return RedirectToError();
            }
        }

        // ── Google OAuth — manual flow ─────────────────────────────
        [HttpGet("google")]
        public IActionResult GoogleLogin()
        {
            var state = GenerateState();
            var clientId = configuration["OAuth:Google:ClientId"];
            var redirectUri = $"{FrontendUrl}/api/auth/google/callback";

            cache.Set($"oauth_state:{state}", true, TimeSpan.FromMinutes(5));

            var url = "https://accounts.google.com/o/oauth2/v2/auth" +
                      $"?client_id={clientId}" +
                      $"&redirect_uri={Uri.EscapeDataString(redirectUri)}" +
                      $"&response_type=code" +
                      $"&scope={Uri.EscapeDataString("openid email profile")}" +
                      $"&state={state}";

            return Redirect(url);
        }

        [HttpGet("google/callback")]
        public async Task<IActionResult> GoogleCallback(
            [FromQuery] string code,
            [FromQuery] string state,
            CancellationToken ct)
        {
            if (string.IsNullOrEmpty(state) ||
                !cache.TryGetValue($"oauth_state:{state}", out _))
                return RedirectToError();

            cache.Remove($"oauth_state:{state}");

            try
            {
                var accessToken = await ExchangeGoogleCode(code, ct);
                if (accessToken is null) return RedirectToError();

                var userInfo = await GetGoogleUserInfo(accessToken, ct);
                if (userInfo is null) return RedirectToError();

                var auth = await oAuthLoginUseCase.ExecuteAsync(
                    userInfo.Value.Email,
                    userInfo.Value.Name,
                    "Google",
                    userInfo.Value.Id,
                    ct);

                var oneTimeCode = codeStore.GenerateCode(new AuthCodePayload
                {
                    Token = auth.Token,
                    Name = auth.Name,
                    Email = auth.Email,
                    Role = auth.Role,
                    UserId = auth.UserId.ToString()
                });

                return RedirectWithCode(oneTimeCode);
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

            return Ok(ApiResponse<AuthResponse>.SuccessResponse(
                new AuthResponse
                {
                    Token = payload.Token,
                    Name = payload.Name,
                    Email = payload.Email,
                    Role = payload.Role,
                    UserId = Guid.Parse(payload.UserId)
                }, "Token exchange successful."));
        }

        [HttpGet("health")]
        public IActionResult Health() => Ok(new
        {
            Service = "AuthService",
            Status = "Healthy",
            Timestamp = DateTime.UtcNow
        });

        // ── GitHub helpers ─────────────────────────────────────────
        private async Task<string?> ExchangeGitHubCode(
            string code, CancellationToken ct)
        {
            var clientId = configuration["OAuth:GitHub:ClientId"];
            var clientSecret = configuration["OAuth:GitHub:ClientSecret"];
            var redirectUri = $"{FrontendUrl}/api/auth/github/callback";

            using var http = new HttpClient();
            http.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            var response = await http.PostAsJsonAsync(
                "https://github.com/login/oauth/access_token",
                new
                {
                    client_id = clientId,
                    client_secret = clientSecret,
                    code,
                    redirect_uri = redirectUri
                }, ct);

            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content
                .ReadFromJsonAsync<JsonElement>(cancellationToken: ct);
            return json.TryGetProperty("access_token", out var token)
                ? token.GetString() : null;
        }

        private async Task<(string Email, string Name, long Id)?> GetGitHubUserInfo(
            string accessToken, CancellationToken ct)
        {
            using var http = new HttpClient();
            http.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer", accessToken);
            http.DefaultRequestHeaders.UserAgent.Add(
                new System.Net.Http.Headers.ProductInfoHeaderValue("Savoria", "1.0"));

            var user = await http.GetFromJsonAsync<JsonElement>(
                "https://api.github.com/user", ct);

            var name = user.TryGetProperty("name", out var n) &&
                       n.ValueKind != JsonValueKind.Null
                ? n.GetString()!
                : user.TryGetProperty("login", out var l)
                    ? l.GetString()! : "GitHub User";

            var id = user.TryGetProperty("id", out var i) ? i.GetInt64() : 0;

            var emails = await http.GetFromJsonAsync<JsonElement[]>(
                "https://api.github.com/user/emails", ct);

            var email = emails?
                .FirstOrDefault(e =>
                    e.TryGetProperty("primary", out var p) && p.GetBoolean() &&
                    e.TryGetProperty("verified", out var v) && v.GetBoolean())
                .GetProperty("email").GetString();

            if (email is null) return null;
            return (email, name, id);
        }

        // ── Google helpers ─────────────────────────────────────────
        private async Task<string?> ExchangeGoogleCode(
            string code, CancellationToken ct)
        {
            var clientId = configuration["OAuth:Google:ClientId"];
            var clientSecret = configuration["OAuth:Google:ClientSecret"];
            var redirectUri = $"{FrontendUrl}/api/auth/google/callback";

            using var http = new HttpClient();
            var response = await http.PostAsJsonAsync(
                "https://oauth2.googleapis.com/token",
                new
                {
                    client_id = clientId,
                    client_secret = clientSecret,
                    code,
                    redirect_uri = redirectUri,
                    grant_type = "authorization_code"
                }, ct);

            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content
                .ReadFromJsonAsync<JsonElement>(cancellationToken: ct);
            return json.TryGetProperty("access_token", out var token)
                ? token.GetString() : null;
        }

        private async Task<(string Email, string Name, string Id)?> GetGoogleUserInfo(
            string accessToken, CancellationToken ct)
        {
            using var http = new HttpClient();
            http.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer", accessToken);

            var user = await http.GetFromJsonAsync<JsonElement>(
                "https://www.googleapis.com/oauth2/v2/userinfo", ct);

            var email = user.TryGetProperty("email", out var e)
                ? e.GetString() : null;
            var name = user.TryGetProperty("name", out var n)
                ? n.GetString() : email?.Split('@')[0];
            var id = user.TryGetProperty("id", out var i)
                ? i.GetString() : null;

            if (email is null || id is null) return null;
            return (email, name!, id);
        }
    }
}
using AuthService.Application.DTOs;
using AuthService.Domain.Interfaces;
using AuthService.Application.Templates;
using AuthService.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace AuthService.Application.UseCases
{
    public class OtpUseCase(
        IUserRepository userRepository,
        IOtpStore otpStore,
        IEmailService emailService,
        ITokenService tokenService,
        IRefreshTokenRepository refreshTokenRepository,
        ILogger<OtpUseCase> logger)
    {
        // ── Send OTP ───────────────────────────────────────────
        public async Task<SendOtpResponse> SendOtpAsync(
            SendOtpRequest request,
            CancellationToken ct = default)
        {
            var email = request.Email.ToLowerInvariant().Trim();
            var isNewUser = !await userRepository.ExistsByEmailAsync(email, ct);

            // generate and store OTP
            var otp = otpStore.GenerateOtp(email);

            // send email
            var subject = isNewUser
                ? "🎉 Welcome to Mithila — Your verification code"
                : "🔐 Your Mithila sign-in code";

            var htmlBody = OtpEmailTemplate.Generate(otp, isNewUser);

            await emailService.SendAsync(email, email, subject, htmlBody, ct);

            logger.LogInformation(
                "OTP sent to {Email} — IsNewUser: {IsNewUser}",
                email, isNewUser);

            return new SendOtpResponse(
                isNewUser,
                isNewUser
                    ? "Verification code sent! Please check your email."
                    : "Sign-in code sent! Please check your email.");
        }

        // ── Verify OTP ─────────────────────────────────────────
        public async Task<AuthResponse?> VerifyOtpAsync(
            VerifyOtpRequest request,
            CancellationToken ct = default)
        {
            var email = request.Email.ToLowerInvariant().Trim();

            // verify OTP
            if (!otpStore.VerifyOtp(email, request.Otp))
            {
                logger.LogWarning(
                    "Invalid OTP attempt for {Email}", email);
                return null;
            }

            // get or create user
            var user = await userRepository.GetByEmailAsync(email, ct);

            if (user is null)
            {
                // new user — create account
                var name = request.Name?.Trim();
                if (string.IsNullOrWhiteSpace(name))
                    name = email.Split('@')[0]; // fallback

                user = User.Create(
                    name, email,
                    passwordHash: string.Empty); // no password

                await userRepository.AddAsync(user, ct);
                await userRepository.SaveChangesAsync(ct);

                logger.LogInformation(
                    "New user created via OTP: {Email}", email);
            }
            else
            {
                logger.LogInformation(
                    "Existing user logged in via OTP: {Email}", email);
            }

            var token = tokenService.GenerateToken(user);
            var refreshToken = RefreshToken.Create(user.Id, expiryDays: 30);

            await refreshTokenRepository.AddAsync(refreshToken, ct);
            await refreshTokenRepository.SaveChangesAsync(ct);
            return new AuthResponse
            {
                Token = token,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                UserId = user.Id
            };
        }
    }
}
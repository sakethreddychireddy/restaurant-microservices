namespace AuthService.Application.Templates
{
    public static class OtpEmailTemplate
    {
        public static string Generate(
            string otp, bool isNewUser) => $@"
<!DOCTYPE html>
<html>
<head>
  <meta charset='utf-8'>
  <style>
    body {{ font-family: 'Segoe UI', Arial, sans-serif; background: #faf8f5; margin: 0; padding: 20px 0; }}
    .container {{ max-width: 480px; margin: 0 auto; background: #ffffff; border-radius: 20px; overflow: hidden; box-shadow: 0 4px 24px rgba(0,0,0,0.08); }}
    .header {{ background: linear-gradient(135deg, #c17f3d 0%, #8b5e2a 100%); padding: 36px 32px; text-align: center; }}
    .header h1 {{ color: #ffffff; font-size: 24px; font-weight: 700; margin: 0; }}
    .header p {{ color: #f5e6d3; font-size: 13px; margin: 6px 0 0; letter-spacing: 1px; }}
    .body {{ padding: 36px 32px; text-align: center; }}
    .greeting {{ font-size: 18px; color: #2c2c2c; font-weight: 600; margin-bottom: 8px; }}
    .message {{ font-size: 14px; color: #6b6b6b; line-height: 1.6; margin-bottom: 32px; }}
    .otp-box {{ background: #faf8f5; border-radius: 16px; padding: 24px; margin-bottom: 24px; border: 2px dashed #c17f3d; }}
    .otp-label {{ font-size: 12px; color: #9b8b7a; text-transform: uppercase; letter-spacing: 2px; margin-bottom: 12px; }}
    .otp-code {{ font-size: 48px; font-weight: 800; color: #c17f3d; letter-spacing: 12px; font-family: monospace; }}
    .expiry {{ font-size: 13px; color: #9b8b7a; margin-bottom: 24px; }}
    .warning {{ background: #fff8f0; border-radius: 12px; padding: 14px 18px; font-size: 12px; color: #9b8b7a; line-height: 1.6; border-left: 3px solid #c17f3d; text-align: left; }}
    .footer {{ background: #faf8f5; padding: 20px 32px; text-align: center; border-top: 1px solid #ede8e0; }}
    .footer p {{ font-size: 12px; color: #b0a090; margin: 0; line-height: 1.6; }}
  </style>
</head>
<body>
  <div class='container'>
    <div class='header'>
      <h1>🍽️ Mithila</h1>
      <p>CRAFTED WITH PASSION</p>
    </div>
    <div class='body'>
      <p class='greeting'>
        {(isNewUser ? "Welcome to Mithila! 🎉" : "Welcome back! 👋")}
      </p>
      <p class='message'>
        {(isNewUser
            ? "You're one step away from joining the Mithila family. Use the code below to complete your signup."
            : "Use the code below to securely sign in to your account.")}
      </p>
      <div class='otp-box'>
        <p class='otp-label'>Your verification code</p>
        <p class='otp-code'>{otp}</p>
      </div>
      <p class='expiry'>⏱️ This code expires in <strong>10 minutes</strong></p>
      <div class='warning'>
        <strong>🔒 Security tip:</strong> Never share this code with anyone.
        Mithila will never ask for your OTP via phone or chat.
      </div>
    </div>
    <div class='footer'>
      <p>If you didn't request this code, please ignore this email.<br/>
      This is an automated message — please do not reply.</p>
    </div>
  </div>
</body>
</html>";
    }
}
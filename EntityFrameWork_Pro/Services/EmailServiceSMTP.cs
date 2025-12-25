using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace EntityFrameWork_Pro.Services
{
    public class EmailServiceSMTP
    {
        private readonly string _smtpServer = "smtp-mail.outlook.com";
        private readonly int _smtpPort = 587;
        private readonly string _senderEmail;
        private readonly string _senderPassword;
        private readonly string _senderName;
        private static Dictionary<string, string> _verificationCodes = new Dictionary<string, string>();

        public EmailServiceSMTP(IConfiguration configuration)
        {
            _senderEmail = configuration["Email:Username"] ?? "ulaflostandfound@outlook.com";
            _senderPassword = configuration["Email:Password"] ?? "YY1289yy";
            _senderName = configuration["Email:DisplayName"] ?? "ULA Lost & Found";
            
            Console.WriteLine($"[EMAIL-SMTP] Initialized with: {_senderEmail}");
            Console.WriteLine($"[EMAIL-SMTP] Password length: {_senderPassword?.Length ?? 0}");
            Console.WriteLine($"[EMAIL-SMTP] Password starts with: {(_senderPassword?.Length > 0 ? _senderPassword.Substring(0, Math.Min(3, _senderPassword.Length)) : "EMPTY")}");
        }

        public string GenerateVerificationCode()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        public void StoreVerificationCode(string studentId, string code)
        {
            _verificationCodes[studentId] = code;
            Console.WriteLine($"[EMAIL] Verification code for {studentId}: {code}");
        }

        public bool VerifyCode(string studentId, string code)
        {
            if (_verificationCodes.TryGetValue(studentId, out string storedCode))
            {
                return storedCode == code;
            }
            return false;
        }

        public async Task<bool> SendVerificationEmailAsync(string toEmail, string code, string studentName)
        {
            try
            {
                Console.WriteLine($"[EMAIL-SMTP] Sending to {toEmail}...");

                using (var smtpClient = new SmtpClient(_smtpServer, _smtpPort))
                {
                    smtpClient.EnableSsl = true;
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = new NetworkCredential(_senderEmail, _senderPassword);
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtpClient.Timeout = 30000; // 30 seconds timeout

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(_senderEmail, _senderName),
                        Subject = "Verify Your Email - Zarqa University Lost & Found",
                        Body = $@"
                            <html>
                            <head>
                                <style>
                                    body {{ font-family: 'Segoe UI', Arial, sans-serif; background-color: #f5f5f5; margin: 0; padding: 20px; }}
                                    .container {{ max-width: 600px; margin: 0 auto; background: #ffffff; padding: 40px; border-radius: 12px; box-shadow: 0 4px 15px rgba(0,0,0,0.1); }}
                                    .header {{ color: #0078d4; text-align: center; margin-bottom: 30px; font-size: 32px; font-weight: 600; }}
                                    .welcome {{ color: #333333; font-size: 24px; margin-bottom: 20px; font-weight: 600; }}
                                    .text {{ color: #333333; font-size: 16px; line-height: 1.6; margin: 15px 0; }}
                                    .code-box {{ background: #f0f0f0; padding: 40px 30px; text-align: center; border-radius: 12px; margin: 30px 0; border: 3px solid #d0d0d0; }}
                                    .code {{ color: #4a4a4a; font-size: 56px; font-weight: bold; letter-spacing: 12px; font-family: 'Courier New', monospace; }}
                                    .footer {{ margin-top: 40px; padding-top: 25px; border-top: 2px solid #e0e0e0; color: #666666; font-size: 14px; text-align: center; }}
                                </style>
                            </head>
                            <body>
                                <div class='container'>
                                    <h1 class='header'>🎓 Zarqa University</h1>
                                    <h2 class='welcome'>مرحباً، {studentName}!</h2>
                                    <p class='text'>شكراً لتسجيلك في نظام المفقودات والموجودات بجامعة الزرقاء.</p>
                                    
                                    <div class='code-box'>
                                        <div class='code'>{code}</div>
                                    </div>
                                    
                                    <p class='text'>⏰ ينتهي خلال 10 دقائق / Expires in 10 minutes</p>
                                    
                                    <div class='footer'>
                                        <strong>نظام المفقودات - جامعة الزرقاء</strong><br>
                                        <strong>Lost & Found System - Zarqa University</strong>
                                    </div>
                                </div>
                            </body>
                            </html>
                        ",
                        IsBodyHtml = true
                    };

                    mailMessage.To.Add(toEmail);

                    Console.WriteLine($"[EMAIL-SMTP] Attempting to send via {_smtpServer}:{_smtpPort}...");
                    await smtpClient.SendMailAsync(mailMessage);
                    Console.WriteLine($"[EMAIL-SMTP] ✅ Email sent successfully to {toEmail}");
                    return true;
                }
            }
            catch (SmtpException smtpEx)
            {
                Console.WriteLine($"[EMAIL-SMTP] ❌ SMTP Error: {smtpEx.Message}");
                Console.WriteLine($"[EMAIL-SMTP] Status Code: {smtpEx.StatusCode}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EMAIL-SMTP] ❌ Error: {ex.Message}");
                Console.WriteLine($"[EMAIL-SMTP] Error Type: {ex.GetType().Name}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"[EMAIL-SMTP] Inner: {ex.InnerException.Message}");
                }
                return false;
            }
        }
    }
}

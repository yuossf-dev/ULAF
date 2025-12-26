using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace EntityFrameWork_Pro.Services
{
    public class EmailServiceResend
    {
        private readonly string _apiKey;
        private readonly string _senderEmail;
        private readonly string _senderName;
        private readonly HttpClient _httpClient;
        private static Dictionary<string, string> _verificationCodes = new Dictionary<string, string>();

        public EmailServiceResend(IConfiguration configuration)
        {
            _apiKey = configuration["Resend:ApiKey"] ?? "";
            _senderEmail = configuration["Resend:SenderEmail"] ?? "onboarding@resend.dev";
            _senderName = configuration["Resend:SenderName"] ?? "ULA Lost & Found";
            
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            
            Console.WriteLine($"[EMAIL-RESEND] Initialized");
            Console.WriteLine($"[EMAIL-RESEND] Sender: {_senderEmail}");
            Console.WriteLine($"[EMAIL-RESEND] API Key length: {_apiKey?.Length ?? 0}");
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
                Console.WriteLine($"[EMAIL-RESEND] Sending to {toEmail}...");

                var emailData = new
                {
                    from = $"{_senderName} <{_senderEmail}>",
                    to = new[] { toEmail },
                    subject = "Verify Your Email - Zarqa University Lost & Found",
                    html = $@"
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
                                <p class='text'>Thank you for registering with Zarqa University Lost & Found System.</p>
                                
                                <div class='code-box'>
                                    <div class='code'>{code}</div>
                                </div>
                                
                                <p class='text'>⏰ ينتهي خلال 10 دقائق / Expires in 10 minutes</p>
                                <p class='text'>🔒 لا تشارك هذا الرمز / Keep this code private</p>
                                
                                <div class='footer'>
                                    <strong>نظام المفقودات - جامعة الزرقاء</strong><br>
                                    <strong>Lost & Found System - Zarqa University</strong>
                                </div>
                            </div>
                        </body>
                        </html>
                    "
                };

                var json = JsonSerializer.Serialize(emailData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                Console.WriteLine($"[EMAIL-RESEND] Calling Resend API...");
                var response = await _httpClient.PostAsync("https://api.resend.com/emails", content);

                var responseBody = await response.Content.ReadAsStringAsync();
                
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"[EMAIL-RESEND] ✅ Email sent successfully to {toEmail}");
                    Console.WriteLine($"[EMAIL-RESEND] Response: {responseBody}");
                    return true;
                }
                else
                {
                    Console.WriteLine($"[EMAIL-RESEND] ❌ Failed: {response.StatusCode}");
                    Console.WriteLine($"[EMAIL-RESEND] Error: {responseBody}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EMAIL-RESEND] ❌ Exception: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"[EMAIL-RESEND] Inner: {ex.InnerException.Message}");
                }
                return false;
            }
        }
    }
}

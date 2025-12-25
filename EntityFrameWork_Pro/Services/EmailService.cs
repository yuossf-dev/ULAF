using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Extensions.Configuration;
using Azure.Core;
using Azure.Identity;

namespace EntityFrameWork_Pro.Services
{
    public class EmailService
    {
        private readonly GraphServiceClient _emailGraphClient; // Separate client for sending emails
        private static Dictionary<string, string> _verificationCodes = new Dictionary<string, string>();
        
        private string? _senderEmail = null;
        private string? _senderName = null;

        public EmailService(IConfiguration configuration)
        {
            // Create a separate Graph client specifically for SENDING EMAILS
            var emailAccessToken = configuration["MicrosoftGraph:EmailAccessToken"];
            if (!string.IsNullOrEmpty(emailAccessToken))
            {
                var tokenCredential = new StaticAccessTokenCredential(emailAccessToken);
                _emailGraphClient = new GraphServiceClient(tokenCredential);
                InitializeSenderInfoAsync().Wait();
            }
            else
            {
                throw new Exception("Email access token not configured!");
            }
        }

        // Helper class for static token
        private class StaticAccessTokenCredential : Azure.Core.TokenCredential
        {
            private readonly string _accessToken;

            public StaticAccessTokenCredential(string accessToken)
            {
                _accessToken = accessToken;
            }

            public override Azure.Core.AccessToken GetToken(Azure.Core.TokenRequestContext requestContext, CancellationToken cancellationToken)
            {
                return new Azure.Core.AccessToken(_accessToken, DateTimeOffset.MaxValue);
            }

            public override ValueTask<Azure.Core.AccessToken> GetTokenAsync(Azure.Core.TokenRequestContext requestContext, CancellationToken cancellationToken)
            {
                return new ValueTask<Azure.Core.AccessToken>(new Azure.Core.AccessToken(_accessToken, DateTimeOffset.MaxValue));
            }
        }
        
        private async Task InitializeSenderInfoAsync()
        {
            try
            {
                var me = await _emailGraphClient.Me.GetAsync();
                _senderEmail = me?.Mail ?? me?.UserPrincipalName;
                _senderName = me?.DisplayName ?? "Lost & Found System";
                Console.WriteLine($"[EMAIL] Sender initialized: {_senderEmail} ({_senderName})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EMAIL] Error getting sender info: {ex.Message}");
                _senderEmail = "ulaflostandfound@outlook.com";
                _senderName = "ULA Lost & Found";
            }
        }

        /// <summary>
        /// Generate a 6-digit verification code
        /// </summary>
        public string GenerateVerificationCode()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        /// <summary>
        /// Store verification code for a student ID
        /// </summary>
        public void StoreVerificationCode(string studentId, string code)
        {
            _verificationCodes[studentId] = code;
            Console.WriteLine($"[EMAIL] Verification code for {studentId}: {code}");
        }

        /// <summary>
        /// Verify if the code matches
        /// </summary>
        public bool VerifyCode(string studentId, string code)
        {
            if (_verificationCodes.TryGetValue(studentId, out string storedCode))
            {
                return storedCode == code;
            }
            return false;
        }

        /// <summary>
        /// Send verification email using Microsoft Graph API from a specific sender
        /// </summary>
        public async Task<bool> SendVerificationEmailAsync(string toEmail, string code, string studentName)
        {
            try
            {
                if (string.IsNullOrEmpty(_senderEmail))
                {
                    Console.WriteLine("[EMAIL] ❌ Sender email not initialized");
                    return false;
                }

                Console.WriteLine($"[EMAIL] Sending verification code to {toEmail} from {_senderEmail}...");

                var message = new Message
                {
                    Subject = "Verify Your Email - Zarqa University Lost & Found",
                    Body = new ItemBody
                    {
                        ContentType = BodyType.Html,
                        Content = $@"
                            <html>
                            <head>
                                <style>
                                    body {{ font-family: 'Segoe UI', Arial, sans-serif; background-color: #f5f5f5; margin: 0; padding: 20px; }}
                                    .container {{ max-width: 600px; margin: 0 auto; background: #ffffff; padding: 40px; border-radius: 12px; box-shadow: 0 4px 15px rgba(0,0,0,0.1); }}
                                    .header {{ color: #0078d4; text-align: center; margin-bottom: 30px; font-size: 32px; font-weight: 600; }}
                                    .welcome {{ color: #333333; font-size: 24px; margin-bottom: 20px; font-weight: 600; }}
                                    .text {{ color: #333333; font-size: 16px; line-height: 1.6; margin: 15px 0; }}
                                    .code-box {{ background: #f0f0f0; padding: 40px 30px; text-align: center; border-radius: 12px; margin: 30px 0; border: 3px solid #d0d0d0; }}
                                    .code-label {{ color: #333333 !important; margin: 0 0 15px 0; font-size: 18px; font-weight: 500; }}
                                    .code {{ color: #4a4a4a !important; font-size: 56px; font-weight: bold; letter-spacing: 12px; text-shadow: 1px 1px 2px rgba(0,0,0,0.1); font-family: 'Courier New', monospace; background-color: transparent !important; }}
                                    .info-box {{ background: #e3f2fd; border-left: 4px solid #2196f3; padding: 15px 20px; margin: 20px 0; border-radius: 6px; }}
                                    .info-text {{ color: #1565c0; font-size: 15px; margin: 5px 0; }}
                                    .footer {{ margin-top: 40px; padding-top: 25px; border-top: 2px solid #e0e0e0; color: #666666; font-size: 14px; text-align: center; line-height: 1.8; }}
                                    .footer strong {{ color: #333333; }}
                                    .highlight {{ color: #d32f2f; font-weight: bold; }}
                                </style>
                            </head>
                            <body>
                                <div class='container'>
                                    <h1 class='header'>🎓 Zarqa University</h1>
                                    <h2 class='welcome'>مرحباً، {studentName}!</h2>
                                    <p class='text'>شكراً لتسجيلك في نظام المفقودات والموجودات بجامعة الزرقاء. يرجى التحقق من بريدك الإلكتروني لإكمال التسجيل.</p>
                                    <p class='text'>Thank you for registering with Zarqa University Lost & Found System. Please verify your email to complete registration.</p>
                                    
                                    <div class='code-box'>
                                        <p class='code-label'>Your Verification Code:</p>
                                        <div class='code' style='color: #4a4a4a !important; background-color: transparent !important;'>{code}</div>
                                    </div>
                                    
                                    <div class='info-box'>
                                        <p class='info-text'>⏰ <strong>ينتهي خلال 10 دقائق</strong> / Expires in 10 minutes</p>
                                        <p class='info-text'>🔒 <strong>لا تشارك هذا الرمز مع أحد</strong> / Keep this code private</p>
                                    </div>
                                    
                                    <p class='text'>إذا لم تطلب هذا الرمز، يرجى تجاهل هذه الرسالة.<br>If you didn't request this, please ignore this email.</p>
                                    
                                    <div class='footer'>
                                        <strong>نظام المفقودات - جامعة الزرقاء</strong><br>
                                        <strong>Lost & Found System - Zarqa University</strong><br>
                                        <em>This is an automated message - هذه رسالة تلقائية</em><br>
                                        للدعم الفني: IT Help Desk
                                    </div>
                                </div>
                            </body>
                            </html>
                        "
                    },
                    ToRecipients = new List<Recipient>
                    {
                        new Recipient
                        {
                            EmailAddress = new EmailAddress
                            {
                                Address = toEmail
                            }
                        }
                    }
                };

                // ✅ Send email as Me (the token owner - ulaflostandfound@outlook.com)
                await _emailGraphClient.Me.SendMail.PostAsync(new Microsoft.Graph.Me.SendMail.SendMailPostRequestBody
                {
                    Message = message,
                    SaveToSentItems = false
                });

                Console.WriteLine($"[EMAIL] ✅ Verification code sent successfully from {_senderEmail} to {toEmail}");
                return true;
            }
            catch (Microsoft.Graph.Models.ODataErrors.ODataError odataError)
            {
                Console.WriteLine($"[EMAIL] ❌ ODataError: {odataError.Error?.Message}");
                Console.WriteLine($"[EMAIL] Error Code: {odataError.Error?.Code}");
                if (odataError.Error?.Details != null)
                {
                    foreach (var detail in odataError.Error.Details)
                    {
                        Console.WriteLine($"[EMAIL] Detail: {detail.Message}");
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EMAIL] ❌ Failed to send email: {ex.GetType().Name} - {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"[EMAIL] Inner exception: {ex.InnerException.GetType().Name} - {ex.InnerException.Message}");
                }
                Console.WriteLine($"[EMAIL] Stack trace: {ex.StackTrace}");
                return false;
            }
        }
    }
}

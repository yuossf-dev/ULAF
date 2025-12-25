using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Kiota.Abstractions.Authentication;

namespace EntityFrameWork_Pro.Services
{
    // Token-based authentication provider
    public class TokenAuthenticationProvider : IAuthenticationProvider
    {
        private readonly string _accessToken;

        public TokenAuthenticationProvider(string accessToken)
        {
            _accessToken = accessToken;
        }

        public Task AuthenticateRequestAsync(Microsoft.Kiota.Abstractions.RequestInformation request, Dictionary<string, object>? additionalAuthenticationContext = null, CancellationToken cancellationToken = default)
        {
            request.Headers.Add("Authorization", $"Bearer {_accessToken}");
            return Task.CompletedTask;
        }
    }

    public class MicrosoftGraphService
    {
        private readonly GraphServiceClient? _graphClient;
        private readonly bool _isConfigured;

        public MicrosoftGraphService(IConfiguration configuration)
        {
            try
            {
                // Try access token first (simpler)
                var accessToken = configuration["MicrosoftGraph:AccessToken"];
                
                System.Diagnostics.Debug.WriteLine($"==========================================");
                System.Diagnostics.Debug.WriteLine($"[GRAPH SERVICE] Access Token Present: {!string.IsNullOrEmpty(accessToken)}");
                System.Diagnostics.Debug.WriteLine($"[GRAPH SERVICE] Token Length: {accessToken?.Length ?? 0}");
                
                if (!string.IsNullOrEmpty(accessToken))
                {
                    System.Diagnostics.Debug.WriteLine("[GRAPH SERVICE] Initializing with Access Token...");
                    var authProvider = new TokenAuthenticationProvider(accessToken);
                    _graphClient = new GraphServiceClient(authProvider);
                    _isConfigured = true;
                    System.Diagnostics.Debug.WriteLine("[GRAPH SERVICE] ✅ Successfully configured with token!");
                }
                else
                {
                    // Fallback to client credentials (Azure app)
                    var tenantId = configuration["MicrosoftGraph:TenantId"];
                    var clientId = configuration["MicrosoftGraph:ClientId"];
                    var clientSecret = configuration["MicrosoftGraph:ClientSecret"];

                    if (!string.IsNullOrEmpty(tenantId) && !string.IsNullOrEmpty(clientId) && !string.IsNullOrEmpty(clientSecret))
                    {
                        System.Diagnostics.Debug.WriteLine("[GRAPH SERVICE] Initializing with Client Credentials...");
                        var credential = new Azure.Identity.ClientSecretCredential(tenantId, clientId, clientSecret);
                        _graphClient = new GraphServiceClient(credential);
                        _isConfigured = true;
                        System.Diagnostics.Debug.WriteLine("[GRAPH SERVICE] ✅ Successfully configured with client credentials!");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("[GRAPH SERVICE] ❌ No credentials found - Graph API disabled");
                        _isConfigured = false;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[GRAPH SERVICE] ❌ Error during initialization: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[GRAPH SERVICE] Stack: {ex.StackTrace}");
                _isConfigured = false;
            }
            
            System.Diagnostics.Debug.WriteLine($"[GRAPH SERVICE] Final IsConfigured: {_isConfigured}");
            System.Diagnostics.Debug.WriteLine($"==========================================");
        }

        public bool IsConfigured => _isConfigured;

        // Expose GraphServiceClient for other services
        public GraphServiceClient GetGraphClient()
        {
            if (!_isConfigured || _graphClient == null)
                throw new InvalidOperationException("Microsoft Graph is not configured. Please check your appsettings.json");
            
            return _graphClient;
        }

        // Search for student by ID - Optimized for zu.edu.jo format (202302150@zu.edu.jo)
        public async Task<StudentInfo?> SearchStudentByIdAsync(string studentId)
        {
            if (!_isConfigured || _graphClient == null)
                return null;

            try
            {
                // Try exact match on userPrincipalName first (most reliable)
                var upnSearch = $"{studentId}@zu.edu.jo";
                
                try
                {
                    var user = await _graphClient.Users[upnSearch].GetAsync(requestConfig =>
                    {
                        requestConfig.QueryParameters.Select = new[] { "id", "displayName", "mail", "userPrincipalName", "employeeId" };
                    });

                    if (user != null)
                    {
                        return new StudentInfo
                        {
                            StudentId = studentId,
                            Name = user.DisplayName,
                            Email = user.Mail ?? user.UserPrincipalName,
                            IsValid = true
                        };
                    }
                }
                catch
                {
                    // User not found with exact match, try filter search
                }

                // Fallback: Search with filter
                var users = await _graphClient.Users
                    .GetAsync(requestConfig =>
                    {
                        requestConfig.QueryParameters.Filter = 
                            $"startsWith(userPrincipalName,'{studentId}') or " +
                            $"startsWith(mail,'{studentId}') or " +
                            $"employeeId eq '{studentId}'";
                        requestConfig.QueryParameters.Select = new[] { "id", "displayName", "mail", "userPrincipalName", "employeeId" };
                        requestConfig.QueryParameters.Top = 1;
                    });

                if (users?.Value != null && users.Value.Count > 0)
                {
                    var user = users.Value[0];
                    return new StudentInfo
                    {
                        StudentId = user.EmployeeId ?? studentId,
                        Name = user.DisplayName,
                        Email = user.Mail ?? user.UserPrincipalName,
                        IsValid = true
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                // Log error for debugging
                Console.WriteLine($"Graph API Error: {ex.Message}");
                return null;
            }
        }

        // Verify if student ID exists
        public async Task<bool> ValidateStudentIdAsync(string studentId)
        {
            var student = await SearchStudentByIdAsync(studentId);
            return student != null && student.IsValid;
        }

        // Get student information
        public async Task<StudentInfo?> GetStudentInfoAsync(string studentId)
        {
            // 🔒 PRODUCTION MODE: Only return real university data
            var result = await SearchStudentByIdAsync(studentId);
            
            if (result == null)
            {
                Console.WriteLine($"[GRAPH] ❌ Student ID not found in university system: {studentId}");
            }
            else
            {
                Console.WriteLine($"[GRAPH] ✅ Student found: {result.Name} ({result.Email})");
            }
            
            return result;
        }
    }

    public class StudentInfo
    {
        public string? StudentId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public bool IsValid { get; set; }
    }
}

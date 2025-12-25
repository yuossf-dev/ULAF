# Get Microsoft Graph Token via PowerShell
# For ulaflostandfound@outlook.com

Write-Host "`n🔐 Getting Microsoft Graph Token...`n" -ForegroundColor Cyan

# Configuration
$clientId = "d3590ed6-52b3-4102-aeff-aad2292ab01c"  # Microsoft Graph Explorer client
$tenantId = "common"
$scope = "https://graph.microsoft.com/.default"
$username = "ulaflostandfound@outlook.com"
$password = "YY1289yy"

# Convert password to secure string
$securePassword = ConvertTo-SecureString $password -AsPlainText -Force
$credential = New-Object System.Management.Automation.PSCredential($username, $securePassword)

try {
    Write-Host "⏳ Attempting to get token..." -ForegroundColor Yellow
    
    # Build the token request
    $body = @{
        grant_type = "password"
        client_id = $clientId
        scope = "https://graph.microsoft.com/Mail.Send https://graph.microsoft.com/User.Read"
        username = $username
        password = $password
    }
    
    $tokenUrl = "https://login.microsoftonline.com/$tenantId/oauth2/v2.0/token"
    
    # Request token
    $response = Invoke-RestMethod -Uri $tokenUrl -Method Post -Body $body -ContentType "application/x-www-form-urlencoded"
    
    $token = $response.access_token
    
    Write-Host "`n✅ SUCCESS! Token retrieved!`n" -ForegroundColor Green
    Write-Host "📋 Your token (copy this ENTIRE thing):" -ForegroundColor Cyan
    Write-Host "`n$token`n" -ForegroundColor White
    
    # Save to file
    $token | Out-File -FilePath "email_token.txt" -Encoding UTF8
    Write-Host "💾 Token also saved to: email_token.txt`n" -ForegroundColor Green
    
    # Show token info
    Write-Host "📊 Token Info:" -ForegroundColor Cyan
    Write-Host "   Length: $($token.Length) characters" -ForegroundColor White
    Write-Host "   Starts with: $($token.Substring(0,20))..." -ForegroundColor White
    Write-Host "   Expires in: $($response.expires_in / 60) minutes`n" -ForegroundColor White
    
    Write-Host "🎯 NEXT STEPS:" -ForegroundColor Yellow
    Write-Host "1. Copy the token above (or from email_token.txt)" -ForegroundColor White
    Write-Host "2. Go to Render dashboard" -ForegroundColor White
    Write-Host "3. Update: MicrosoftGraph__EmailAccessToken" -ForegroundColor White
    Write-Host "4. Save and wait for restart" -ForegroundColor White
    Write-Host "5. 🔒 CHANGE PASSWORD at https://outlook.com`n" -ForegroundColor Red
    
} catch {
    Write-Host "`n❌ ERROR: $($_.Exception.Message)`n" -ForegroundColor Red
    
    if ($_.Exception.Message -match "AADSTS50126") {
        Write-Host "⚠️ Invalid username or password!" -ForegroundColor Yellow
        Write-Host "Please verify the credentials are correct.`n" -ForegroundColor White
    }
    elseif ($_.Exception.Message -match "AADSTS50076") {
        Write-Host "⚠️ Multi-factor authentication required!" -ForegroundColor Yellow
        Write-Host "The account has MFA enabled. You need to:" -ForegroundColor White
        Write-Host "1. Use Graph Explorer: https://developer.microsoft.com/graph/graph-explorer" -ForegroundColor Cyan
        Write-Host "2. Or create an App Registration with client secret`n" -ForegroundColor Cyan
    }
    else {
        Write-Host "Try using Graph Explorer instead:" -ForegroundColor Yellow
        Write-Host "https://developer.microsoft.com/graph/graph-explorer`n" -ForegroundColor Cyan
    }
}

# Interactive Token Retrieval - Opens browser for authentication
Write-Host "`n🔐 INTERACTIVE TOKEN RETRIEVAL`n" -ForegroundColor Cyan

Write-Host "This will open your browser to get the token...`n" -ForegroundColor Yellow

# Configuration
$clientId = "d3590ed6-52b3-4102-aeff-aad2292ab01c"
$redirectUri = "https://login.microsoftonline.com/common/oauth2/nativeclient"
$scope = "https://graph.microsoft.com/Mail.Send https://graph.microsoft.com/User.Read"

# Build auth URL
$authUrl = "https://login.microsoftonline.com/common/oauth2/v2.0/authorize?" +
    "client_id=$clientId" +
    "&response_type=token" +
    "&redirect_uri=$redirectUri" +
    "&scope=$scope" +
    "&response_mode=fragment"

Write-Host "📋 STEPS:" -ForegroundColor Cyan
Write-Host "1. Your browser will open" -ForegroundColor White
Write-Host "2. Sign in with: ulaflostandfound@outlook.com" -ForegroundColor White
Write-Host "3. Password: YY1289yy" -ForegroundColor White
Write-Host "4. After redirect, the URL will contain the token" -ForegroundColor White
Write-Host "5. Copy the part after 'access_token=' and before '&token_type'`n" -ForegroundColor White

Write-Host "Press ENTER to open browser..." -ForegroundColor Yellow
$null = Read-Host

# Open browser
Start-Process $authUrl

Write-Host "`n✅ Browser opened!" -ForegroundColor Green
Write-Host "`n📋 AFTER YOU SIGN IN:" -ForegroundColor Cyan
Write-Host "1. Look at the URL in your browser" -ForegroundColor White
Write-Host "2. It will look like:" -ForegroundColor White
Write-Host "   ...nativeclient#access_token=eyJ0eXA..." -ForegroundColor Gray
Write-Host "3. Copy ONLY the token part (starts with eyJ)" -ForegroundColor White
Write-Host "4. Paste it below`n" -ForegroundColor White

Write-Host "Paste your token here and press ENTER:" -ForegroundColor Yellow
$token = Read-Host

if ($token -and $token.StartsWith("eyJ")) {
    Write-Host "`n✅ Token looks valid!`n" -ForegroundColor Green
    
    # Save to file
    $token | Out-File -FilePath "email_token.txt" -Encoding UTF8 -NoNewline
    
    Write-Host "💾 Token saved to: email_token.txt" -ForegroundColor Green
    Write-Host "📏 Token length: $($token.Length) characters`n" -ForegroundColor White
    
    Write-Host "🎯 NEXT STEPS:" -ForegroundColor Yellow
    Write-Host "1. Go to: https://dashboard.render.com" -ForegroundColor White
    Write-Host "2. Select your ULAF service" -ForegroundColor White
    Write-Host "3. Environment → MicrosoftGraph__EmailAccessToken" -ForegroundColor White
    Write-Host "4. Paste the token from email_token.txt" -ForegroundColor White
    Write-Host "5. Save Changes" -ForegroundColor White
    Write-Host "6. 🔒 CHANGE PASSWORD at https://outlook.com`n" -ForegroundColor Red
    
} else {
    Write-Host "`n⚠️ Token doesn't look right!" -ForegroundColor Red
    Write-Host "It should start with 'eyJ' and be very long." -ForegroundColor Yellow
    Write-Host "Try again or use Graph Explorer instead.`n" -ForegroundColor White
}

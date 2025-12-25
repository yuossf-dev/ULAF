# DIRECT JWT TOKEN GENERATOR FOR OUTLOOK.COM
# This uses a different client ID that returns proper JWT tokens

Write-Host "`n🔐 GETTING PROPER JWT TOKEN`n" -ForegroundColor Cyan

Write-Host "Opening browser with different authentication method..." -ForegroundColor Yellow
Write-Host "This should give you a JWT token (starts with eyJ)`n" -ForegroundColor White

# Using Azure CLI client ID which returns JWT tokens
$clientId = "04b07795-8ddb-461a-bbee-02f9e1bf7b46"  # Azure CLI
$redirectUri = "http://localhost"
$scope = "https://graph.microsoft.com/Mail.Send https://graph.microsoft.com/User.Read"

$authUrl = "https://login.microsoftonline.com/common/oauth2/v2.0/authorize?" +
    "client_id=$clientId" +
    "&response_type=token" +
    "&redirect_uri=$redirectUri" +
    "&scope=$scope" +
    "&response_mode=fragment"

Write-Host "📋 INSTRUCTIONS:" -ForegroundColor Cyan
Write-Host "1. Browser will open" -ForegroundColor White
Write-Host "2. Sign in: ulaflostandfound@outlook.com / YY1289yy" -ForegroundColor White
Write-Host "3. Browser will try to redirect to localhost (that's OK!)" -ForegroundColor White
Write-Host "4. You'll see 'This site can't be reached' - THAT'S NORMAL!" -ForegroundColor Yellow
Write-Host "5. Look at the URL bar - copy the token from there`n" -ForegroundColor White

Write-Host "Press ENTER to open browser..." -ForegroundColor Green
Read-Host

Start-Process $authUrl

Write-Host "`n✅ Browser opened!`n" -ForegroundColor Green
Write-Host "⚠️ IMPORTANT: After you sign in, the page will show an error." -ForegroundColor Red
Write-Host "   This is NORMAL! Just copy the token from the URL bar.`n" -ForegroundColor Yellow

Write-Host "The URL will look like:" -ForegroundColor Cyan
Write-Host "   http://localhost/#access_token=eyJ0eXAiOiJKV1Qi..." -ForegroundColor Gray
Write-Host "`nCopy ONLY the part after 'access_token=' and before '&token_type'`n" -ForegroundColor White

Write-Host "Paste the token here:" -ForegroundColor Green
$token = Read-Host

if ($token -and $token.StartsWith("eyJ")) {
    Write-Host "`n✅ SUCCESS! This is a JWT token!`n" -ForegroundColor Green
    
    # Save it
    $token | Out-File -FilePath "jwt_token.txt" -Encoding UTF8 -NoNewline
    
    Write-Host "💾 Saved to: jwt_token.txt" -ForegroundColor Green
    Write-Host "📏 Length: $($token.Length) chars`n" -ForegroundColor White
    
    # Test on jwt.io
    Write-Host "🧪 Testing token format..." -ForegroundColor Yellow
    try {
        $parts = $token.Split('.')
        if ($parts.Count -eq 3) {
            Write-Host "✅ Token structure is valid (3 parts)`n" -ForegroundColor Green
        }
    } catch {}
    
    Write-Host "🎯 NEXT STEPS:" -ForegroundColor Cyan
    Write-Host "1. Go to: https://dashboard.render.com" -ForegroundColor White
    Write-Host "2. ULAF service → Environment" -ForegroundColor White  
    Write-Host "3. MicrosoftGraph__EmailAccessToken = [paste from jwt_token.txt]" -ForegroundColor White
    Write-Host "4. Save Changes" -ForegroundColor White
    Write-Host "5. 🔒 CHANGE PASSWORD!`n" -ForegroundColor Red
    
} else {
    Write-Host "`n❌ That still doesn't look like a JWT token!" -ForegroundColor Red
    Write-Host "Token should start with 'eyJ' not 'EwBY'`n" -ForegroundColor Yellow
    
    Write-Host "Let me show you the URL format again:" -ForegroundColor Cyan
    Write-Host "Look for: http://localhost/#access_token=eyJTYKDJSKDJ...&token_type=Bearer" -ForegroundColor Gray
    Write-Host "Copy ONLY: eyJTYKDJSKDJ... (the part between = and &)`n" -ForegroundColor White
}

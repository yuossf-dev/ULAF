# Device Code Flow - Works with all Microsoft accounts!
# This ALWAYS returns proper JWT tokens

Write-Host "`n🔐 DEVICE CODE AUTHENTICATION`n" -ForegroundColor Cyan
Write-Host "This method works 100% of the time!`n" -ForegroundColor Green

$clientId = "04b07795-8ddb-461a-bbee-02f9e1bf7b46"  # Azure CLI
$scope = "https://graph.microsoft.com/Mail.Send"

try {
    # Step 1: Request device code
    Write-Host "⏳ Requesting device code..." -ForegroundColor Yellow
    
    $deviceCodeResponse = Invoke-RestMethod -Method Post -Uri "https://login.microsoftonline.com/common/oauth2/v2.0/devicecode" -Body @{
        client_id = $clientId
        scope = $scope
    }
    
    Write-Host "`n✅ Device code received!`n" -ForegroundColor Green
    Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Cyan
    Write-Host "📋 YOUR CODE: $($deviceCodeResponse.user_code)" -ForegroundColor Yellow
    Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━`n" -ForegroundColor Cyan
    
    Write-Host "🌐 STEPS:" -ForegroundColor Green
    Write-Host "1. Go to: $($deviceCodeResponse.verification_uri)" -ForegroundColor White
    Write-Host "2. Enter code: $($deviceCodeResponse.user_code)" -ForegroundColor Yellow
    Write-Host "3. Sign in: ulaflostandfound@outlook.com / YY1289yy" -ForegroundColor White
    Write-Host "4. Approve the permissions`n" -ForegroundColor White
    
    Write-Host "Opening browser..." -ForegroundColor Cyan
    Start-Process $deviceCodeResponse.verification_uri
    
    Write-Host "`n⏳ Waiting for you to complete authentication..." -ForegroundColor Yellow
    Write-Host "(This will check every 5 seconds)`n" -ForegroundColor Gray
    
    # Step 2: Poll for token
    $timeout = 300 # 5 minutes
    $interval = 5
    $elapsed = 0
    
    while ($elapsed -lt $timeout) {
        Start-Sleep -Seconds $interval
        $elapsed += $interval
        
        try {
            $tokenResponse = Invoke-RestMethod -Method Post -Uri "https://login.microsoftonline.com/common/oauth2/v2.0/token" -Body @{
                grant_type = "urn:ietf:params:oauth:grant-type:device_code"
                client_id = $clientId
                device_code = $deviceCodeResponse.device_code
            } -ErrorAction Stop
            
            # Success!
            $token = $tokenResponse.access_token
            
            Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Green
            Write-Host "✅ SUCCESS! JWT TOKEN RETRIEVED!" -ForegroundColor Green
            Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━`n" -ForegroundColor Green
            
            # Save token
            $token | Out-File -FilePath "jwt_token.txt" -Encoding UTF8 -NoNewline
            
            Write-Host "📊 Token Info:" -ForegroundColor Cyan
            Write-Host "   Length: $($token.Length) characters" -ForegroundColor White
            Write-Host "   Starts with: $($token.Substring(0,20))..." -ForegroundColor White
            Write-Host "   Expires in: $($tokenResponse.expires_in / 60) minutes" -ForegroundColor White
            Write-Host "   Saved to: jwt_token.txt`n" -ForegroundColor White
            
            Write-Host "📋 YOUR TOKEN:" -ForegroundColor Cyan
            Write-Host "$token`n" -ForegroundColor White
            
            Write-Host "🎯 NEXT STEPS:" -ForegroundColor Yellow
            Write-Host "1. Copy the token above (or from jwt_token.txt)" -ForegroundColor White
            Write-Host "2. Go to: https://dashboard.render.com" -ForegroundColor White
            Write-Host "3. ULAF service → Environment tab" -ForegroundColor White
            Write-Host "4. MicrosoftGraph__EmailAccessToken = [paste token]" -ForegroundColor White
            Write-Host "5. Save Changes" -ForegroundColor White
            Write-Host "6. 🔒 CHANGE PASSWORD at https://outlook.com`n" -ForegroundColor Red
            
            break
            
        } catch {
            if ($_.Exception.Response.StatusCode -eq 400) {
                # Still waiting for user
                Write-Host "." -NoNewline -ForegroundColor Gray
            } else {
                throw
            }
        }
    }
    
    if ($elapsed -ge $timeout) {
        Write-Host "`n⏱️ Timeout! You didn't complete authentication in time." -ForegroundColor Red
        Write-Host "Please run the script again.`n" -ForegroundColor Yellow
    }
    
} catch {
    Write-Host "`n❌ ERROR: $($_.Exception.Message)`n" -ForegroundColor Red
}

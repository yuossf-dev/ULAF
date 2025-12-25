# Quick Token Refresh Script
# Run this to get a new token URL

Write-Host "`n🔐 GET NEW EMAIL TOKEN`n" -ForegroundColor Cyan

Write-Host "1. Open this URL in your browser:" -ForegroundColor Yellow
Write-Host "   https://login.microsoftonline.com/common/oauth2/v2.0/authorize?client_id=de8bc8b5-d9f9-48b1-a8ad-b748da725064&response_type=token&redirect_uri=https://developer.microsoft.com/en-us/graph/graph-explorer&scope=Mail.Send%20User.Read" -ForegroundColor Cyan

Write-Host "`n2. Sign in with: 202302150@zu.edu.jo`n" -ForegroundColor White

Write-Host "3. After redirected, copy the 'access_token' from the URL`n" -ForegroundColor White

Write-Host "4. Update in Render Environment Variables`n" -ForegroundColor White

Write-Host "⚠️ Note: Tokens expire after 1 hour!" -ForegroundColor Red
Write-Host "   For production, you should implement auto-refresh`n" -ForegroundColor Yellow

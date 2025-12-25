# 🚀 Quick Deployment Script for Render.com

Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "  Render.com Deployment Setup" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

# Check if git is initialized
if (-not (Test-Path .git)) {
    Write-Host "Initializing Git repository..." -ForegroundColor Yellow
    git init
    Write-Host "✅ Git initialized" -ForegroundColor Green
} else {
    Write-Host "✅ Git already initialized" -ForegroundColor Green
}

# Add TOKENS_BACKUP.txt to .gitignore if not already there
if (-not (Select-String -Path .gitignore -Pattern "TOKENS_BACKUP.txt" -Quiet)) {
    Add-Content -Path .gitignore -Value "`nTOKENS_BACKUP.txt"
    Write-Host "✅ Added TOKENS_BACKUP.txt to .gitignore" -ForegroundColor Green
}

Write-Host ""
Write-Host "📋 Next Steps:" -ForegroundColor Yellow
Write-Host ""
Write-Host "1. Create a new repository on GitHub:" -ForegroundColor White
Write-Host "   https://github.com/new" -ForegroundColor Cyan
Write-Host ""
Write-Host "2. Run these commands (replace YOUR_USERNAME and YOUR_REPO):" -ForegroundColor White
Write-Host "   git add ." -ForegroundColor Green
Write-Host "   git commit -m 'Initial commit for Render deployment'" -ForegroundColor Green
Write-Host "   git branch -M main" -ForegroundColor Green
Write-Host "   git remote add origin https://github.com/YOUR_USERNAME/YOUR_REPO.git" -ForegroundColor Green
Write-Host "   git push -u origin main" -ForegroundColor Green
Write-Host ""
Write-Host "3. Go to Render.com and deploy:" -ForegroundColor White
Write-Host "   https://render.com" -ForegroundColor Cyan
Write-Host ""
Write-Host "📖 Full guide available in: RENDER_DEPLOYMENT_GUIDE.md" -ForegroundColor Yellow
Write-Host ""
Write-Host "⚠️  IMPORTANT: Your tokens are saved in TOKENS_BACKUP.txt" -ForegroundColor Red
Write-Host "   This file is NOT committed to Git for security." -ForegroundColor Red
Write-Host "   You'll need to add these as environment variables in Render." -ForegroundColor Red
Write-Host ""

# 🚀 Deploy to Render.com - Step by Step Guide

## ✅ Prerequisites
1. GitHub account
2. Render.com account (free) - https://render.com
3. Your code pushed to GitHub

## 📋 Deployment Steps

### Step 1: Push Code to GitHub
```bash
cd C:\Users\Victus\Desktop\EntityFrameWork_Pro
git init
git add .
git commit -m "Initial commit for Render deployment"
# Create a new repo on GitHub, then:
git remote add origin https://github.com/YOUR_USERNAME/YOUR_REPO_NAME.git
git push -u origin main
```

### Step 2: Create Render Web Service
1. Go to https://render.com and sign in
2. Click "New +" → "Web Service"
3. Connect your GitHub repository
4. Configure:
   - **Name**: `lostandfound-app` (or your choice)
   - **Environment**: `Docker`
   - **Region**: Choose closest to you
   - **Branch**: `main`
   - **Plan**: `Free`

### Step 3: Add Environment Variables
In Render dashboard, add these environment variables:

#### Required:
- `ASPNETCORE_ENVIRONMENT` = `Production`
- `DatabaseProvider` = `Sqlite`
- `ConnectionStrings__DefaultConnection` = `Data Source=/app/data/LostAndFound.db`

#### For Firebase (if using):
- `Firebase__ProjectId` = `lostandfound-d22e2`
- `Firebase__CredentialsPath` = `/app/firebase-key.json`

**Important for Firebase**: You'll need to add the firebase-key.json as a secret file
- In Render dashboard → Secret Files
- Add file: `firebase-key.json`
- Paste your Firebase credentials JSON content

#### For Microsoft Graph (Email):
- `MicrosoftGraph__AccessToken` = `YOUR_ACCESS_TOKEN`
- `MicrosoftGraph__EmailAccessToken` = `YOUR_EMAIL_TOKEN`

⚠️ **Security Note**: The tokens in your appsettings.json will expire. You should:
1. Remove them from appsettings.json before pushing to GitHub
2. Add them as environment variables in Render instead

### Step 4: Deploy
1. Click "Create Web Service"
2. Render will automatically build and deploy your app
3. Wait 5-10 minutes for first deployment

### Step 5: Access Your App
- Your app will be available at: `https://your-app-name.onrender.com`

## 🔧 Important Notes

### Database Persistence
⚠️ **SQLite on Render Free Tier**: 
- Data will be lost on redeploys
- For permanent storage, consider:
  - PostgreSQL (Render offers free tier)
  - Or upgrade to paid plan with persistent disks

### Token Management
Your Microsoft Graph tokens expire. Consider:
1. Implementing token refresh logic
2. Using Azure Key Vault
3. Regularly updating environment variables

### Firebase Setup
If using Firebase:
1. Upload firebase-key.json as a Secret File in Render
2. Make sure Firebase__CredentialsPath points to `/app/firebase-key.json`

## 🐛 Troubleshooting

### Build Fails
- Check Dockerfile syntax
- Ensure all dependencies in .csproj

### App Crashes on Start
- Check environment variables are set correctly
- View logs in Render dashboard

### Database Errors
- Ensure ConnectionStrings__DefaultConnection is set
- Check DatabaseProvider is "Sqlite"

## 📝 Before Pushing to GitHub

### Clean sensitive data:
```json
// In appsettings.json, replace tokens with empty strings:
{
  "MicrosoftGraph": {
    "AccessToken": "",
    "EmailAccessToken": "",
    "TenantId": "",
    "ClientId": "",
    "ClientSecret": ""
  }
}
```

### Create .gitignore if not exists:
```
bin/
obj/
*.db
firebase-key.json
appsettings.Development.json
.vs/
```

## 🎉 Success!
Once deployed, your app will be live at your Render URL!

Free tier limitations:
- Spins down after 15 minutes of inactivity
- First request after spin-down takes 30-60 seconds
- 750 hours/month free

## 🔄 Updates
To update your app:
1. Push changes to GitHub
2. Render auto-deploys from main branch

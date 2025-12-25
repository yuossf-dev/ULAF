# 🎯 CREATE YOUR OWN APP REGISTRATION
# This is the ONLY way that works for Outlook.com accounts

## ✅ Step-by-Step Guide

### Step 1: Create App Registration (5 minutes)

1. **Go to Azure Portal:**
   https://portal.azure.com

2. **Sign in with:** `ulaflostandfound@outlook.com` / `YY1289yy`

3. **Search for "App registrations"** (in the search bar at top)

4. **Click "+ New registration"**

5. **Fill in the form:**
   - **Name:** `ULAF Email Service`
   - **Supported account types:** Select "Personal Microsoft accounts only"
   - **Redirect URI:** Leave blank for now
   - Click **Register**

6. **Copy your Client ID:**
   - You'll see "Application (client) ID" - COPY THIS!
   - Example: `abc12345-1234-1234-1234-123456789abc`

### Step 2: Create Client Secret

1. **In your app**, click **"Certificates & secrets"** (left menu)

2. **Click "+ New client secret"**

3. **Add description:** `ULAF Email Token`

4. **Expires:** Select "24 months"

5. **Click "Add"**

6. **⚠️ IMPORTANT: Copy the SECRET VALUE immediately!**
   - It looks like: `abc~123xyz...`
   - You can ONLY see this ONCE!
   - If you miss it, you'll need to create a new one

### Step 3: Add API Permissions

1. **Click "API permissions"** (left menu)

2. **Click "+ Add a permission"**

3. **Select "Microsoft Graph"**

4. **Select "Delegated permissions"**

5. **Search and check:**
   - `Mail.Send`
   - `User.Read`

6. **Click "Add permissions"**

7. **Click "Grant admin consent"** (the button with shield icon)
   - Click "Yes" to confirm

### Step 4: Get Token Using Your App

Run this PowerShell script with YOUR values:

```powershell
$tenantId = "common"
$clientId = "YOUR_CLIENT_ID_HERE"  # From Step 1
$clientSecret = "YOUR_SECRET_HERE"  # From Step 2
$scope = "https://graph.microsoft.com/.default"
$username = "ulaflostandfound@outlook.com"
$password = "YY1289yy"

$body = @{
    grant_type = "password"
    client_id = $clientId
    client_secret = $clientSecret
    scope = $scope
    username = $username
    password = $password
}

$response = Invoke-RestMethod -Method Post -Uri "https://login.microsoftonline.com/$tenantId/oauth2/v2.0/token" -Body $body

$token = $response.access_token
Write-Host "Token: $token"
$token | Out-File -FilePath "final_token.txt"
```

### Step 5: Update Render

1. Copy the token from `final_token.txt`
2. Go to Render dashboard → ULAF → Environment
3. Update: `MicrosoftGraph__EmailAccessToken`
4. Save

---

## 🔄 For Long-Term (Optional but Recommended):

Instead of using password grant (which will be deprecated), update your app to use Client Credentials:

1. In Azure Portal → Your app → API Permissions
2. Remove delegated permissions
3. Add **Application permissions** → `Mail.Send`
4. Grant admin consent
5. Use this in your code:

```csharp
var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
var graphClient = new GraphServiceClient(credential);
```

This way tokens refresh automatically and you don't need to manually update them!

---

## ⚠️ AFTER YOU GET IT WORKING:

1. **Change the password** for `ulaflostandfound@outlook.com`
2. **Store Client ID and Secret** securely (in Render environment variables)
3. **Delete password from any files/scripts**

---

## 📌 Summary of What You'll Have:

- **Client ID:** `abc12345-...` (not secret, goes in code)
- **Client Secret:** `abc~123xyz...` (SECRET! Store in Render env vars)
- **Token:** JWT starting with `eyJ` (refreshes automatically)

This is the professional way and will work forever! 🎉

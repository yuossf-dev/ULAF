# 🎓 Microsoft Graph API Setup - University Student Verification

## ✅ What This Does:
- Validates student IDs against your university's Microsoft 365
- Verifies students exist in Microsoft Teams
- Auto-fills student name and email from university records
- Works with your existing login system

---

## 🚀 Setup Steps:

### Step 1: Register App in Azure Portal

1. Go to: https://portal.azure.com/
2. Sign in with your **university Microsoft account**
3. Navigate to: **Azure Active Directory** → **App registrations** → **New registration**

**Fill in:**
- **Name:** `Lost and Found Student Verification`
- **Supported account types:** `Accounts in this organizational directory only (Your University)`
- **Redirect URI:** Leave blank for now
- Click **Register**

### Step 2: Configure API Permissions

1. In your app, go to: **API permissions** → **Add a permission**
2. Select: **Microsoft Graph** → **Application permissions**
3. Add these permissions:
   - `User.Read.All` - Read all users' basic profiles
   - `Directory.Read.All` - Read directory data
4. Click **Grant admin consent for [Your University]** (requires admin - see alternative below)

**⚠️ If you don't have admin rights:**
- Use **Delegated permissions** instead of Application permissions
- Select: `User.Read` and `User.ReadBasic.All`
- No admin consent needed, but you'll need to sign in each time

### Step 3: Create Client Secret

1. Go to: **Certificates & secrets** → **New client secret**
2. **Description:** `Student Verification Secret`
3. **Expires:** Choose duration (recommended: 6 months or 1 year)
4. Click **Add**
5. **⚠️ COPY THE SECRET VALUE NOW** - You won't see it again!

### Step 4: Get Your App Credentials

Copy these from the app **Overview** page:
- **Application (client) ID:** `xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx`
- **Directory (tenant) ID:** `xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx`
- **Client secret:** (from Step 3)

### Step 5: Update appsettings.json

Add this to your `appsettings.json`:

```json
{
  "MicrosoftGraph": {
    "TenantId": "your-tenant-id-here",
    "ClientId": "your-client-id-here",
    "ClientSecret": "your-client-secret-here"
  }
}
```

**Example:**
```json
{
  "MicrosoftGraph": {
    "TenantId": "abcd1234-ef56-gh78-ij90-klmnopqrstuv",
    "ClientId": "1234abcd-5678-efgh-9012-ijklmnopqrst",
    "ClientSecret": "abc~XYZ123-_456.789def"
  }
}
```

---

## 🎯 How It Works:

### Before (Old System):
1. Student enters username/password
2. System checks local database
3. ❌ No way to verify if student really exists

### After (With Microsoft Graph):
1. Student enters **Student ID** and password
2. System checks **Microsoft Teams/365** if ID exists
3. ✅ Auto-fills name and email from university records
4. ✅ Only real university students can register!

---

## 📱 Usage Example:

```csharp
// In your controller:
var isValid = await _graphService.ValidateStudentIdAsync("202412345");

if (isValid)
{
    var studentInfo = await _graphService.GetStudentInfoAsync("202412345");
    // studentInfo.Name = "Ahmed Ali"
    // studentInfo.Email = "202412345@university.edu"
}
```

---

## 🔒 Security Notes:

1. **Never commit secrets to Git:**
   - Add `appsettings.json` to `.gitignore`
   - Use User Secrets in development
   - Use Azure Key Vault in production

2. **Protect your credentials:**
   ```bash
   dotnet user-secrets init
   dotnet user-secrets set "MicrosoftGraph:ClientSecret" "your-secret"
   ```

3. **Fallback mode:**
   - If Graph API is not configured, system works with local validation
   - Great for testing without university connection

---

## 🧪 Testing:

1. **With Graph API:**
   - Enter a real student ID from your university
   - System verifies with Microsoft 365
   - Auto-fills name and email

2. **Without Graph API (Offline):**
   - System uses local validation only
   - Still works for development/demo

---

## ⚡ Alternative: Simpler Delegated Permissions Flow

If you can't get admin consent, use this simpler approach:

### In Azure Portal:
1. Use **Delegated permissions** instead of Application
2. Add: `User.Read` and `User.ReadBasic.All`
3. No admin consent needed!

### In Code:
The student will sign in with their Microsoft account once, then you can verify them.

---

## 💡 Pro Tips:

- **For Demo:** Test with your own student ID first
- **For Presentation:** Show live verification with real student IDs
- **For Production:** Use Azure Key Vault for secrets
- **Student ID Format:** Ask your IT department what format they use

---

## 🆘 Troubleshooting:

**"Insufficient privileges"**
→ Need admin consent OR switch to delegated permissions

**"Application not found"**
→ Check Tenant ID is correct

**"Invalid client secret"**
→ Secret expired or incorrect, create new one

**"User not found"**
→ Check student ID format matches university system

---

Your Lost & Found app now has **enterprise-level university integration**! 🎓🔥

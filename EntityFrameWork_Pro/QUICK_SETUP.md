# 🚀 Quick Setup Guide - Microsoft Graph Student Verification

## ⚠️ Important: Close Visual Studio First!

Before running commands, **close Visual Studio** to avoid file lock issues.

---

## 📋 Step-by-Step Implementation

### Step 1: Update Database

Run these commands in PowerShell (after closing VS):

```powershell
cd C:\Users\Victus\Desktop\EntityFrameWork_Pro\EntityFrameWork_Pro
dotnet ef migrations add AddStudentIdToUser
dotnet ef database update
```

This adds the `StudentId` field to your database.

---

### Step 2: Update Your Views

#### Update `Views/User/Register.cshtml`:

Find the username input and add StudentId field before it:

```html
<!-- Add this BEFORE username field -->
<div class="form-group">
    <label>الرقم الجامعي</label>
    <input type="text" name="StudentId" class="form-control" placeholder="مثال: 202412345" required />
</div>
```

#### Update `Views/User/Login.cshtml`:

Change username to studentId:

```html
<!-- REPLACE username with studentId -->
<div class="form-group">
    <label>الرقم الجامعي</label>
    <input type="text" name="studentId" class="form-control" placeholder="202412345" required />
</div>
```

---

### Step 3: Configure Microsoft Graph (Optional - for real verification)

Open `appsettings.json` and update with your Azure credentials:

```json
{
  "MicrosoftGraph": {
    "TenantId": "your-university-tenant-id-from-azure",
    "ClientId": "your-app-client-id-from-azure",
    "ClientSecret": "your-app-secret-from-azure"
  }
}
```

**To get these credentials:**
1. Follow instructions in `MICROSOFT_GRAPH_SETUP.md`
2. Or leave as is - system will work in offline mode

---

## 🎯 How It Works:

### When Microsoft Graph is NOT configured:
✅ System accepts any student ID format  
✅ Perfect for development and testing  
✅ No internet needed

### When Microsoft Graph IS configured:
✅ Validates student ID with Microsoft Teams  
✅ Auto-fills student name and email  
✅ Only real university students can register

---

## 🧪 Testing:

### 1. Without Microsoft Graph (Quick Test):
```
Student ID: 202412345
Password: Test1234
```

### 2. With Microsoft Graph (Real Test):
```
Student ID: <your-actual-student-id>
Password: <your-password>
```

System will verify with Microsoft Teams and auto-fill your info!

---

## 📝 What Changed:

**User Model:**
- ✅ Added `StudentId` field (required)
- ✅ Still keeps username, email, phone

**Login System:**
- ✅ Now uses Student ID instead of username
- ✅ Validates with Microsoft Graph if configured
- ✅ Falls back to local validation if not

**Registration:**
- ✅ Checks if student exists in university system
- ✅ Auto-fills name and email from Microsoft
- ✅ Prevents duplicate registrations

---

## 🔥 Benefits:

1. **Authentic Users**: Only real students can register
2. **Auto-Fill Data**: No manual entry of name/email
3. **University Integration**: Professional enterprise-level feature
4. **Flexible**: Works offline without Graph API too

---

## 🆘 Troubleshooting:

**Build error about locked files?**
→ Close Visual Studio and try again

**Can't find student ID?**
→ Check if Microsoft Graph is configured correctly
→ Or leave unconfigured to accept any ID (for testing)

**Migration failed?**
→ Make sure SQL Server is running
→ Check connection string in appsettings.json

---

## 💡 Next Steps:

1. Close Visual Studio
2. Run the migration commands
3. Update your views (Register.cshtml and Login.cshtml)
4. Test without Graph API first
5. Configure Graph API when ready for production

Your Lost & Found system now has **university-grade authentication**! 🎓✨

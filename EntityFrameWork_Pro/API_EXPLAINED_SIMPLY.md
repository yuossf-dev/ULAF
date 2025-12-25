# 🔌 Microsoft Graph API - Simple Explanation

## ❓ What is Microsoft Graph API?

Think of it as a **phone book for your university**. Instead of manually looking up students, your code can automatically ask Microsoft:

```
Your Code: "Does student 202302150 exist?"
Microsoft: "Yes! Name: يوسف حسين, Email: 202302150@zu.edu.jo"
```

---

## 🎯 Why Do We Need It?

### **Problem Without API:**
```
❌ Student types:
   ID: 123456789 (could be fake)
   Name: Ahmed (could be anyone)
   Email: ahmed@gmail.com (not university email)

❌ System has no way to verify if this is real
❌ Anyone can create account with fake info
```

### **Solution With API:**
```
✅ Student types only:
   ID: 202302150
   
✅ System asks Microsoft:
   "Is 202302150 a real student at zu.edu.jo?"
   
✅ Microsoft responds:
   "Yes! Here's their real name and university email"
   
✅ System sends code to university email
✅ Student must check university email to verify
```

---

## 🏢 Where Does This Data Come From?

Your university uses **Microsoft Teams** for students. Behind Teams is **Azure Active Directory** (like a database of all students).

```
┌──────────────────────────────────────┐
│  Zarqa University Microsoft System   │
│                                      │
│  ┌────────────────────────────────┐ │
│  │  Azure Active Directory        │ │
│  │  (Student Database)            │ │
│  │                                │ │
│  │  202302150 → يوسف حسين         │ │
│  │  202302151 → محمد علي          │ │
│  │  202302152 → فاطمة خالد        │ │
│  │  ...                           │ │
│  └────────────────────────────────┘ │
│                ↑                     │
│                │ Query student info  │
└────────────────┼─────────────────────┘
                 │
        ┌────────┴────────┐
        │  Your Website   │
        │  (Asks API)     │
        └─────────────────┘
```

---

## 📝 The Two Main API Calls

### **1️⃣ Validate Student (MicrosoftGraphService.cs)**

```csharp
// FILE: Services/MicrosoftGraphService.cs

public async Task<StudentInfo> GetStudentInfoAsync(string studentId)
{
    // Build email: 202302150 → 202302150@zu.edu.jo
    string email = $"{studentId}@zu.edu.jo";
    
    try
    {
        // Ask Microsoft: "Does this student exist?"
        var user = await _graphClient.Users[email].GetAsync();
        
        // ✅ Found! Return their info
        return new StudentInfo
        {
            Name = user.DisplayName,      // يوسف حسين
            Email = user.Mail             // 202302150@zu.edu.jo
        };
    }
    catch
    {
        // ❌ Not found
        return null;
    }
}
```

**What happens:**
- Input: `"202302150"`
- API URL: `https://graph.microsoft.com/v1.0/users/202302150@zu.edu.jo`
- Microsoft checks: "Is this a real student?"
- Response: Name + Email (if exists), or Error (if not)

### **2️⃣ Send Verification Email (EmailService.cs)**

```csharp
// FILE: Services/EmailService.cs

public async Task SendVerificationEmailAsync(string toEmail, string code, string name)
{
    // Create email message
    var message = new Message
    {
        Subject = "رمز تفعيل الحساب",
        Body = new ItemBody
        {
            ContentType = BodyType.Html,
            Content = $@"
                <h2>مرحباً {name}</h2>
                <p>رمز التحقق الخاص بك:</p>
                <h1>{code}</h1>
            "
        },
        ToRecipients = new List<Recipient>
        {
            new Recipient
            {
                EmailAddress = new EmailAddress { Address = toEmail }
            }
        }
    };
    
    // Send via Microsoft Graph
    await _graphClient.Me.SendMail(message, false).PostAsync();
}
```

**What happens:**
- Generates code: `"123456"`
- Sends email to: `202302150@zu.edu.jo`
- Student checks Outlook and enters code
- System verifies code matches

---

## 🔑 How Authentication Works

### **Step 1: Get Access Token (Graph Explorer)**

1. Go to: https://developer.microsoft.com/graph/graph-explorer
2. Sign in with your university account: `202302150@zu.edu.jo`
3. Grant permissions: `User.ReadBasic.All` + `Mail.Send`
4. Copy the **Access Token** (long string starting with `eyJ...`)

### **Step 2: Put Token in Your Code**

```json
// FILE: appsettings.json

{
  "MicrosoftGraph": {
    "AccessToken": "eyJ0eXAiOiJKV1QiLCJhbGc..."
  }
}
```

### **Step 3: Use Token in Service**

```csharp
// FILE: Services/MicrosoftGraphService.cs

public MicrosoftGraphService(IConfiguration configuration)
{
    string token = configuration["MicrosoftGraph:AccessToken"];
    
    _graphClient = new GraphServiceClient(
        new DelegateAuthenticationProvider((requestMessage) =>
        {
            requestMessage.Headers.Authorization = 
                new AuthenticationHeaderValue("Bearer", token);
            return Task.CompletedTask;
        })
    );
}
```

**What this does:**
- Reads token from config file
- Attaches token to every API request
- Microsoft knows: "This request is from 202302150@zu.edu.jo"

---

## 🎬 Complete Flow Example

```
1. Student visits registration page
   └─> Views/User/SignUp.cshtml
   
2. Student enters:
   ├─ Student ID: 202302150
   └─ Password: MyPass123
   
3. Form submits to Controller
   └─> UserController.cs (Register action)
   
4. Controller calls API service
   └─> MicrosoftGraphService.GetStudentInfoAsync("202302150")
   
5. Service queries Microsoft
   ├─ URL: https://graph.microsoft.com/v1.0/users/202302150@zu.edu.jo
   ├─ Headers: Authorization: Bearer eyJ0eXAi...
   └─ Microsoft checks Azure AD database
   
6. Microsoft responds
   ├─ DisplayName: "يوسف مروان عبد المجيد حسين"
   └─ Mail: "202302150@zu.edu.jo"
   
7. Service returns to Controller
   └─> StudentInfo object with Name & Email
   
8. Controller auto-fills user data
   ├─ UserName = "يوسف مروان عبد المجيد حسين"
   └─ Email = "202302150@zu.edu.jo"
   
9. Controller calls Email Service
   └─> EmailService.SendVerificationEmailAsync(...)
   
10. Email Service generates code
    ├─ Code: "485921"
    └─ Stores in memory: _verificationCodes["202302150"] = "485921"
    
11. Email Service sends via API
    ├─ URL: https://graph.microsoft.com/v1.0/me/sendMail
    ├─ Body: HTML email with code
    └─ To: 202302150@zu.edu.jo
    
12. Student checks Outlook
    └─> Email received with code: 485921
    
13. Student enters code on website
    └─> Views/User/VerifyEmail.cshtml
    
14. Form submits to Controller
    └─> UserController.cs (VerifyEmail action)
    
15. Controller validates code
    └─> EmailService.VerifyCode("202302150", "485921")
    
16. Code matches! ✅
    └─> Controller saves user to database
    
17. Account created successfully
    └─> Student can now login
```

---

## 🔍 Checking API Calls (Testing)

### **Test in Graph Explorer:**

1. **Validate Student:**
   ```
   GET https://graph.microsoft.com/v1.0/users/202302150@zu.edu.jo
   ```
   
   Response:
   ```json
   {
     "displayName": "يوسف مروان عبد المجيد حسين",
     "mail": "202302150@zu.edu.jo",
     "userPrincipalName": "202302150@zu.edu.jo"
   }
   ```

2. **Send Email:**
   ```
   POST https://graph.microsoft.com/v1.0/me/sendMail
   
   Body:
   {
     "message": {
       "subject": "Test",
       "body": { "content": "Hello" },
       "toRecipients": [
         { "emailAddress": { "address": "202302150@zu.edu.jo" }}
       ]
     }
   }
   ```

---

## ⚙️ Code Files Summary

| File | Purpose | Type |
|------|---------|------|
| `Services/MicrosoftGraphService.cs` | Validates student with Microsoft | **API CODE** |
| `Services/EmailService.cs` | Sends emails via Microsoft | **API CODE** |
| `Controllers/UserController.cs` | Uses both services | **MIXED** |
| `Models/User.cs` | Data structure | **BASIC** |
| `DataBaseB/DBBridge.cs` | Database | **BASIC** |
| `appsettings.json` | API token config | **CONFIG** |

---

## 🎓 Key Concepts

1. **RESTful API**: Making HTTP requests to external services
2. **Authentication**: Using bearer tokens to prove identity
3. **Async/Await**: Non-blocking code execution
4. **Service Layer**: Separating API logic from controllers
5. **External Validation**: Trusting external authority (Microsoft) over user input

---

## 🔒 Security Benefits

✅ **Student ID Validation**
   - Can't register with fake ID
   - Must exist in university system

✅ **Email Verification**
   - Must have access to university email
   - Proves student owns the ID

✅ **No Personal Emails**
   - Only university emails accepted
   - No @gmail.com or @yahoo.com

✅ **Real Name from Microsoft**
   - Can't enter fake name
   - Fetched from university database

---

## 🆚 With API vs Without API

### **Scenario: Fake Student Registration**

#### **WITHOUT API:**
```
❌ Fake student enters:
   ID: 999999999
   Name: Hacker
   Email: hacker@gmail.com
   
✅ ACCEPTED (No validation)
❌ Fake account created
```

#### **WITH API:**
```
❌ Fake student enters:
   ID: 999999999
   
⚠️ System queries Microsoft:
   GET /users/999999999@zu.edu.jo
   
❌ Microsoft responds: "404 Not Found"
   
🚫 Registration BLOCKED
   Error: "Student ID not found in university system"
```

---

## 📊 Performance

- **API Call Speed**: ~200-500ms per request
- **Token Expiry**: 1 hour (need to refresh)
- **Rate Limits**: Microsoft allows thousands of requests per hour
- **Caching**: Could cache student info to reduce API calls

---

## 🎯 Summary

**In Simple Terms:**

Instead of trusting what students type, your website **asks Microsoft directly**:
1. "Is this student ID real?" → Microsoft checks
2. "What's their real name?" → Microsoft provides
3. "Send them an email" → Microsoft delivers

This makes registration **secure and verified** without manual checking.

---

*API integration adds 200 lines of code but provides enterprise-level security*

# 📚 Project Code Explanation

## 🎯 Overview
This project implements a **secure student registration system** that validates student IDs against the university's Microsoft Teams/Azure AD system.

---

## 🏗️ Architecture & Code Structure

### **1️⃣ Basic MVC Components (Standard .NET)**

#### **Models (Models/User.cs)**
```csharp
public class User
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Phone { get; set; }
    public string StudentId { get; set; }
    public bool IsEmailVerified { get; set; }
}
```
- **Purpose**: Defines the structure of user data
- **Simple database table**: Stores student information

#### **Database Context (DataBaseB/DBBridge.cs)**
```csharp
public class DBBridge : DbContext
{
    public DbSet<User> Users { get; set; }
}
```
- **Purpose**: Entity Framework connection to SQL Server
- **Standard approach**: No API calls here, just database operations

#### **Controller (Controllers/UserController.cs)**
- **Routes**: Handles HTTP requests (SignUp, Login, Register, VerifyEmail)
- **Validation**: Basic input validation (password strength, required fields)
- **Session Management**: Login/Logout using sessions

---

### **2️⃣ Microsoft Graph API Integration (Advanced Feature)**

This is where we validate student IDs with the university system.

#### **Service Layer (Services/MicrosoftGraphService.cs)**
```csharp
public class MicrosoftGraphService
{
    private readonly GraphServiceClient _graphClient;
    
    public async Task<StudentInfo> GetStudentInfoAsync(string studentId)
    {
        // 🔍 Search for student in university Microsoft Teams
        var user = await _graphClient.Users[$"{studentId}@zu.edu.jo"].GetAsync();
        
        return new StudentInfo
        {
            Name = user.DisplayName,
            Email = user.Mail
        };
    }
}
```

**What it does:**
1. Takes student ID (e.g., `202302150`)
2. Queries Microsoft Graph API: `https://graph.microsoft.com/v1.0/users/202302150@zu.edu.jo`
3. Returns student name and email if exists
4. Returns `null` if student ID not found in university system

**Why separate service?**
- ✅ **Clean separation**: API logic separated from controller
- ✅ **Reusable**: Can be used in multiple controllers
- ✅ **Testable**: Easy to test independently
- ✅ **Maintainable**: Changes to API don't affect controller logic

---

#### **Email Service (Services/EmailService.cs)**
```csharp
public class EmailService
{
    private readonly GraphServiceClient _graphClient;
    
    public async Task SendVerificationEmailAsync(string toEmail, string code, string name)
    {
        // 📧 Send email via Microsoft Graph API
        var message = new Message
        {
            Subject = "رمز تفعيل الحساب",
            Body = new ItemBody { Content = $"رمز التحقق: {code}" },
            ToRecipients = new List<Recipient>
            {
                new Recipient { EmailAddress = new EmailAddress { Address = toEmail } }
            }
        };
        
        await _graphClient.Me.SendMail(message, false).PostAsync();
    }
}
```

**What it does:**
1. Generates 6-digit verification code
2. Sends email using Microsoft Graph Mail API
3. Stores code temporarily in memory
4. Validates code when user submits

---

### **3️⃣ Configuration (appsettings.json)**

```json
{
  "MicrosoftGraph": {
    "AccessToken": "eyJ0eXAiOiJKV1QiLC..."
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=..."
  }
}
```

**Two separate configurations:**
1. **Microsoft Graph**: API authentication token
2. **SQL Server**: Database connection string

---

## 🔐 Security Flow

### **Registration Process:**

```
1. Student enters Student ID + Password
   ↓
2. MicrosoftGraphService validates ID with university system
   ↓
3. If VALID → Fetch name & email from university
   ↓
4. EmailService sends verification code to student's university email
   ↓
5. Student enters code from email
   ↓
6. If CODE VALID → Account created in database
   ↓
7. Student can now login
```

### **Why This is Secure:**

✅ **Can't fake Student ID**: Validated against real university database
✅ **Can't fake Email**: Auto-filled from university, not user input
✅ **Email Verification**: Must access university email to complete registration
✅ **Password Protected**: Only student knows their chosen password

---

## 🔧 How API Integration Works (Simple Explanation)

### **Without API (Insecure):**
```
Student types:
  ID: 123456 ❌ (could be fake)
  Name: John ❌ (could be fake)
  Email: fake@example.com ❌ (not verified)
```

### **With API (Secure):**
```
Student types:
  ID: 202302150 ✅
  
System checks with Microsoft:
  "Does student 202302150 exist?"
  
Microsoft responds:
  ✅ "Yes! Name: يوسف مروان عبد المجيد حسين"
  ✅ "Email: 202302150@zu.edu.jo"
  
System sends code to 202302150@zu.edu.jo
Student enters code → Account created
```

---

## 📝 Key Differences Between Basic & API Code

### **Basic Controller Code:**
```csharp
// Simple validation
if (string.IsNullOrWhiteSpace(user.UserName))
{
    ViewBag.Error = "Username required";
    return View();
}

// Save to database
_db.Users.Add(user);
_db.SaveChanges();
```

### **API Integration Code:**
```csharp
// Call external API
var studentInfo = await _graphService.GetStudentInfoAsync(user.StudentId);

if (studentInfo == null)
{
    ViewBag.Error = "Student ID not found";
    return View();
}

// Auto-fill from API response
user.UserName = studentInfo.Name;
user.Email = studentInfo.Email;
```

---

## 🎓 For Your Professor

### **Project Demonstrates:**

1. **Standard .NET MVC** (Basic)
   - Entity Framework for database
   - MVC pattern (Model-View-Controller)
   - Session management
   - Input validation

2. **RESTful API Integration** (Advanced)
   - Microsoft Graph API consumption
   - OAuth2 authentication
   - Service layer pattern
   - Asynchronous programming

3. **Security Best Practices**
   - Email verification
   - External identity validation
   - Password strength requirements
   - Protected routes

### **Files to Review:**

**Basic Code (Standard .NET):**
- `Models/User.cs` - Data model
- `DataBaseB/DBBridge.cs` - Database context
- `Views/User/*.cshtml` - UI pages

**API Integration Code (Advanced):**
- `Services/MicrosoftGraphService.cs` - University validation
- `Services/EmailService.cs` - Email verification
- `Controllers/UserController.cs` (Lines 73-88, 113-115) - API usage

**Configuration:**
- `appsettings.json` - Settings
- `Program.cs` - Dependency injection setup

---

## 🚀 Running the Project

1. **Database**: Runs on SQL Server
2. **API**: Requires Microsoft Graph access token
3. **Email**: Sends via Microsoft Graph Mail API

**Token expires every ~1 hour**, so you'll need to refresh from Graph Explorer before demo.

---

## ❓ Common Questions

**Q: Why use Microsoft Graph API?**
A: To validate student IDs against the real university database (Microsoft Teams/Azure AD)

**Q: Can't we just trust user input?**
A: No - anyone could type any name/email. API ensures they're real students.

**Q: What happens if API is down?**
A: Registration is blocked. System shows "Service unavailable" message.

**Q: Why separate services?**
A: Clean code architecture - business logic separated from controller logic.

---

## 📊 Summary

| Component | Type | Purpose |
|-----------|------|---------|
| **User.cs** | Basic | Data model |
| **DBBridge.cs** | Basic | Database |
| **UserController.cs** | Mixed | Request handling + API calls |
| **MicrosoftGraphService.cs** | Advanced | University validation |
| **EmailService.cs** | Advanced | Email verification |
| **Views** | Basic | HTML pages |

**Total Lines of Code:**
- Basic MVC: ~60%
- API Integration: ~40%

---

*Generated for academic demonstration purposes*

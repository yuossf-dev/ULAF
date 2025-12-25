# QUICK FIX - Use SMTP Instead of Microsoft Graph

## What to do:

1. **Update appsettings.json** - Add SMTP config:
```json
"Email": {
  "Username": "ulaflostandfound@outlook.com",
  "Password": "YY1289yy",
  "DisplayName": "ULA Lost & Found"
}
```

2. **Replace EmailService.cs** with the new `EmailServiceSMTP.cs`
   - Or rename EmailServiceSMTP.cs to EmailService.cs

3. **Update UserController.cs** - Change from:
```csharp
private readonly EmailService _emailService;
```
To:
```csharp
private readonly EmailServiceSMTP _emailService;
```

4. **Update Program.cs** - Change registration from:
```csharp
builder.Services.AddSingleton<EmailService>();
```
To:
```csharp
builder.Services.AddSingleton<EmailServiceSMTP>();
```

## OR - Simpler: Just update appsettings and commit!

The EmailServiceSMTP.cs file is ready. Just:
1. Add Email config to appsettings.json  
2. Commit and push
3. Add to Render env vars
4. Done!

**No tokens, no OAuth, just works!** ✅

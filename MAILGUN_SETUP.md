# 🚀 MAILGUN SETUP (5 Minutes!)

## Step 1: Sign Up (2 mins)

1. Go to: https://signup.mailgun.com/new/signup
2. Sign up with any email (FREE - no credit card!)
3. Verify your email

## Step 2: Get API Key (1 min)

1. After login, go to: https://app.mailgun.com/app/sending/domains
2. Click on your sandbox domain (looks like: sandbox-abc123.mailgun.org)
3. Click "API Keys" tab
4. Copy the "Private API key"

## Step 3: Add to Render (2 mins)

Go to Render dashboard → ULAF → Environment

Add these variables:

```
Mailgun__ApiKey = your-api-key-here
Mailgun__Domain = sandbox-abc123.mailgun.org
Mailgun__SenderEmail = noreply@sandbox-abc123.mailgun.org
Mailgun__SenderName = ULA Lost & Found
```

## Step 4: Update Code

In `Program.cs`, change:

```csharp
// FROM:
builder.Services.AddSingleton<EmailServiceSMTP>();

// TO:
builder.Services.AddSingleton<EmailServiceMailgun>();
```

In `UserController.cs`, change:

```csharp
// FROM:
private readonly EmailServiceSMTP _emailService;
public UserController(DBBridge db, MicrosoftGraphService graphService, EmailServiceSMTP emailService)

// TO:
private readonly EmailServiceMailgun _emailService;
public UserController(DBBridge db, MicrosoftGraphService graphService, EmailServiceMailgun emailService)
```

## Step 5: Commit & Push

```bash
git add .
git commit -m "Switch to Mailgun email service"
git push
```

## ✅ DONE!

Mailgun works instantly, no waiting, no SMTP issues!

---

## 📝 Notes:

- **Sandbox domain** only sends to authorized recipients
- To authorize your email:
  1. Go to Mailgun dashboard
  2. Click your sandbox domain
  3. Add 202302150@zu.edu.jo to "Authorized Recipients"
  4. Verify it via email

- **For production** (send to anyone):
  1. Add your own domain
  2. Add DNS records
  3. Get verified (takes 1-2 days)

For testing, sandbox is perfect! ✅

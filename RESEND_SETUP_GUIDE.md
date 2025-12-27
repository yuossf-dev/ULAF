# 🚀 Resend Email Setup Guide for Render.com

## Why Resend?
- ✅ **No token expiration** - API keys don't expire like Microsoft Graph tokens
- ✅ **Better deliverability** - Emails land in inbox, not spam
- ✅ **Free tier**: 100 emails/day, 3,000/month
- ✅ **Simple setup** - Just need an API key
- ✅ **Reliable** - No authentication headaches

## Step 1: Get Resend API Key

1. **Go to Resend**: https://resend.com
2. **Sign up** with your email (free account)
3. **Verify your email** 
4. **Go to API Keys**: https://resend.com/api-keys
5. **Click "Create API Key"**
   - Name: `Zarqa Lost & Found Production`
   - Permission: `Sending access`
6. **Copy the API key** (starts with `re_...`)
   - ⚠️ Save it somewhere safe - you won't see it again!

## Step 2: Configure Render.com Environment Variables

1. **Go to your Render dashboard**: https://dashboard.render.com
2. **Select your web service** (EntityFrameWork_Pro)
3. **Go to "Environment"** tab
4. **Add these environment variables**:

```bash
Resend__ApiKey=re_your_actual_api_key_here
Resend__SenderEmail=onboarding@resend.dev
Resend__SenderName=Zarqa University Lost & Found
```

**Important Notes:**
- Use **double underscore** `__` (not single `_` or `:`)
- The free tier uses `onboarding@resend.dev` as sender
- For custom domain (e.g., `noreply@zu.edu.jo`), you need to verify your domain in Resend

## Step 3: Redeploy on Render

1. After adding environment variables, Render will automatically redeploy
2. OR manually click **"Manual Deploy"** → **"Deploy latest commit"**
3. Check logs for:
   ```
   [STARTUP] Resend API Key configured: True
   [EMAIL-RESEND] Initialized
   ```

## Step 4: Test Email Sending

1. Try to register a new user
2. Check Render logs for:
   ```
   [EMAIL-RESEND] Sending to student@zu.edu.jo...
   [EMAIL-RESEND] ✅ Email sent successfully to student@zu.edu.jo
   ```

## 🎯 Upgrade to Custom Domain (Optional)

If you want emails from `noreply@zu.edu.jo` instead of `onboarding@resend.dev`:

1. **Go to Resend Dashboard** → **Domains**
2. **Add your domain**: `zu.edu.jo`
3. **Add DNS records** (Resend provides exact records)
4. **Verify domain** (usually takes 5-15 minutes)
5. **Update Render environment variable**:
   ```
   Resend__SenderEmail=noreply@zu.edu.jo
   ```

## 📊 Monitor Email Delivery

- **Resend Dashboard**: https://resend.com/emails
- See all sent emails, delivery status, opens, clicks
- Debug failed emails with detailed error messages

## 🔍 Troubleshooting

### Emails not sending?

1. **Check Render logs**:
   ```bash
   [EMAIL-RESEND] ❌ Failed: 401
   ```
   - **Solution**: API key is wrong or expired

2. **Check Resend dashboard**:
   - Look for failed emails
   - Check error messages

3. **Verify environment variables**:
   ```bash
   Resend__ApiKey (must use double underscore)
   ```

### Emails going to spam?

1. **Use verified domain** (not `onboarding@resend.dev`)
2. **Add SPF/DKIM records** (Resend provides these)
3. **Warm up your domain** (send gradually increasing emails)

## 💡 Best Practices

1. **Monitor your quota**: Free tier = 100 emails/day
2. **Upgrade if needed**: Pro plan = $20/month for 50,000 emails
3. **Use production API key** on Render, development key locally
4. **Never commit API keys** to GitHub

## 🆚 Comparison

| Feature | Microsoft Graph | Resend |
|---------|----------------|--------|
| Token Expiration | ❌ Every 60-90 days | ✅ Never |
| Setup Complexity | ❌ Complex | ✅ Simple |
| Deliverability | ⚠️ Sometimes spam | ✅ Excellent |
| Free Tier | ❌ No free tier | ✅ 3,000/month |
| API Reliability | ⚠️ Can fail | ✅ Very reliable |

## 📝 What Changed in Your Code?

1. **Program.cs**: Switched from `EmailService` to `EmailServiceResend`
2. **UserController.cs**: Updated constructor to use `EmailServiceResend`
3. **appsettings.Production.json**: Added Resend configuration

## ✅ Done!

Your app now uses Resend for emails. No more token expiration issues! 🎉

---

**Need help?** 
- Resend Docs: https://resend.com/docs
- Resend Support: support@resend.com

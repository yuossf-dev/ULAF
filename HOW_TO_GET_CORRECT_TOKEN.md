# ✅ CORRECT WAY TO GET EMAIL TOKEN

## Method 1: Graph Explorer (RECOMMENDED)

1. Go to: https://developer.microsoft.com/graph/graph-explorer

2. Sign in with your university account: 202302150@zu.edu.jo

3. In the left panel, select a sample request OR type:
   ```
   https://graph.microsoft.com/v1.0/me
   ```

4. Click "Run Query"

5. **Get the token:**
   - Click "Access Token" tab (below the query box)
   - You'll see a long token starting with "eyJ..."
   - Click "Copy" button
   - This is your JWT token!

6. Update in Render:
   - Go to: https://dashboard.render.com
   - Select your ULAF service
   - Environment → MicrosoftGraph__EmailAccessToken
   - Paste the token (starts with eyJ...)
   - Save

---

## Method 2: Microsoft Authentication (Alternative)

If Graph Explorer doesn't work, use this direct auth:

1. Open this URL:
```
https://login.microsoftonline.com/common/oauth2/v2.0/authorize?client_id=d3590ed6-52b3-4102-aeff-aad2292ab01c&response_type=token&redirect_uri=https://office.com&scope=https://graph.microsoft.com/Mail.Send%20https://graph.microsoft.com/User.Read&response_mode=fragment
```

2. Sign in with: 202302150@zu.edu.jo

3. After redirect, look at the URL bar - it will have:
   ```
   ...#access_token=eyJ0eXAiOiJKV1QiLC...
   ```

4. Copy ONLY the part after "access_token=" and BEFORE "&token_type"
   - Should start with: eyJ
   - Should be VERY long (1000+ characters)

5. Paste this into Render environment variable

---

## ⚠️ IMPORTANT:
- Token MUST start with "eyJ" 
- Token expires in 1 hour
- Don't include "Bearer " prefix
- Copy the FULL token (very long!)

---

## 🔍 How to verify token is correct:
Go to: https://jwt.io
Paste your token - you should see:
- Header section
- Payload section with your email
- Signature section

If it shows "Invalid Token", get a new one!

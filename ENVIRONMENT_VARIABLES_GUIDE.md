# 🎯 How to Edit DatabaseProvider in Render - Screenshot Guide

## Problem: "I can't see the database provider menu"

**Solution**: It's not a menu - it's an environment variable you need to edit!

---

## 📍 Step-by-Step Instructions:

### Step 1: Navigate to Environment Variables
```
Render Dashboard
  ↓
Click on "lostandfound-app" (your web service)
  ↓
Look for "Environment" tab/link
  (Usually in left sidebar or top tabs)
  ↓
Click "Environment"
```

### Step 2: Find DatabaseProvider Variable

You'll see a page with a list of variables:

```
╔═══════════════════════════════════════════════════════════╗
║  Environment Variables                                    ║
╠═══════════════════════════════════════════════════════════╣
║                                                           ║
║  Key                           | Value                   ║
║  ─────────────────────────────────────────────────────── ║
║  ASPNETCORE_ENVIRONMENT        | Production              ║
║  DatabaseProvider              | Sqlite            ← HERE!║
║  ConnectionStrings__...        | Data Source=...         ║
║  Firebase__ProjectId           | [empty or value]        ║
║  Firebase__CredentialsPath     | /app/firebase-key.json  ║
║  MicrosoftGraph__AccessToken   | [your token]            ║
║                                                           ║
║  [+ Add Environment Variable]                            ║
║                                                           ║
╚═══════════════════════════════════════════════════════════╝
```

### Step 3: Edit the Value

#### Option A: If DatabaseProvider EXISTS
```
1. Find the row: "DatabaseProvider | Sqlite"
2. Click on "Sqlite" (the value field)
3. DELETE "Sqlite"
4. TYPE "PostgreSQL" (case-sensitive!)
5. Press Enter or Tab
```

#### Option B: If DatabaseProvider DOESN'T EXIST
```
1. Click button: "+ Add Environment Variable"
2. A new row appears:
   ┌────────────────┬─────────────────┐
   │ Key            │ Value           │
   ├────────────────┼─────────────────┤
   │ [type here]    │ [type here]     │
   └────────────────┴─────────────────┘
3. In Key field: DatabaseProvider
4. In Value field: PostgreSQL
5. Press Tab or click elsewhere
```

### Step 4: Update Connection String

Find `ConnectionStrings__DefaultConnection`:

**Current value (SQLite):**
```
Data Source=/app/data/LostAndFound.db
```

**Change to (PostgreSQL):**
```
[Your PostgreSQL Internal Database URL]

Example:
postgresql://lostandfounduser:abc123@dpg-xyz.oregon-postgres.render.com/lostandfounddb
```

⚠️ **Important**: Get this URL from your PostgreSQL database page!

### Step 5: Save Changes

```
1. Scroll to bottom of page
2. Click "Save Changes" button
3. Your service will automatically redeploy
4. Wait 3-5 minutes
5. Check logs for: "[STARTUP] ✅ Database initialized"
```

---

## 🎯 Complete Checklist - What Variables You Need:

```
Required for PostgreSQL:
✅ DatabaseProvider = PostgreSQL
✅ ConnectionStrings__DefaultConnection = [PostgreSQL URL]

Required for your app:
✅ ASPNETCORE_ENVIRONMENT = Production
✅ Firebase__ProjectId = lostandfound-d22e2
✅ Firebase__CredentialsPath = /app/firebase-key.json
✅ MicrosoftGraph__AccessToken = [your token]
✅ MicrosoftGraph__EmailAccessToken = [your token]

Secret Files:
✅ firebase-key.json (uploaded)
```

---

## 🔍 Where to Get PostgreSQL Connection String?

If you already created PostgreSQL database:

```
1. Go to Render Dashboard
2. Click on your PostgreSQL database
   (It might be named "lostandfound-db" or similar)
3. Look for section: "Connections"
4. Find: "Internal Database URL"
5. Click "Copy" button
6. Use THIS in ConnectionStrings__DefaultConnection
```

Example of what it looks like:
```
Internal Database URL:
postgresql://lostandfounduser:longpasswordhere@dpg-abc123xyz.oregon-postgres.render.com/lostandfounddb

⚠️ Use INTERNAL not EXTERNAL
```

---

## ❌ Common Mistakes to Avoid:

### Mistake 1: Wrong Spelling
```
❌ WRONG: PostgreSql
❌ WRONG: Postgresql  
❌ WRONG: postgres
✅ CORRECT: PostgreSQL
```

### Mistake 2: Using External URL
```
❌ External Database URL (costs bandwidth)
✅ Internal Database URL (free, faster)
```

### Mistake 3: Forgot to Save
```
After editing, you MUST click "Save Changes" button!
The service won't update until you save.
```

---

## 🆘 Still Can't Find It?

### If you DON'T see "Environment" tab:

Try these locations:
1. **Top navigation** - Look for tabs at the top
2. **Left sidebar** - Scroll down if needed
3. **Service Settings** - Sometimes under "Settings" → "Environment"

### If page looks completely different:

Render might have updated their UI. Look for:
- "Environment Variables"
- "Env Vars"
- "Configuration"
- "Settings"

The concept is the same - you're looking for KEY-VALUE pairs.

---

## 📞 Alternative: Use Render CLI

If you prefer command line:

```bash
# Install Render CLI
npm install -g @render-com/cli

# Set environment variable
render env set DatabaseProvider=PostgreSQL --service=lostandfound-app

render env set ConnectionStrings__DefaultConnection="postgresql://..." --service=lostandfound-app
```

---

## 🎉 Success Indicators:

After saving changes, check these:

```
✅ "Save Changes" button becomes grayed out
✅ You see "Deploying..." status
✅ After 3-5 minutes, check Logs tab
✅ Look for: "[STARTUP] ✅ Database initialized"
```

If you see this in logs, PostgreSQL is working! 🎊

---

**Still stuck? Take a screenshot of your Render dashboard and I can help you identify where the Environment section is!**

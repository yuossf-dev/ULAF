# 📘 How to Create PostgreSQL Database on Render - Step by Step

## 🎯 Two Ways to Set This Up

---

## 🚀 METHOD 1: AUTOMATIC (EASIEST - RECOMMENDED)

### Using Blueprint (render.yaml does everything!)

#### Step 1: Go to Render Dashboard
- Open: https://dashboard.render.com
- Sign in to your account

#### Step 2: Delete Old Service (if exists)
- Find your existing `lostandfound-app` service
- Click on it → Settings (scroll down)
- Click "Delete Web Service"
- Type the service name to confirm

#### Step 3: Create from Blueprint
1. Click **"New +"** button (top right)
2. Select **"Blueprint"**
3. Connect your GitHub repository: `yuossf-dev/ULAF`
4. Branch: `main`
5. Click **"Apply"**

#### Step 4: Render Creates Everything Automatically! 🎉
- ✅ Creates PostgreSQL database (`lostandfound-db`)
- ✅ Creates web service (`lostandfound-app`)
- ✅ Connects them together automatically
- ✅ Applies all migrations

#### Step 5: Add Your Secrets
After creation, go to your web service:
1. Click on `lostandfound-app`
2. Go to **"Environment"** tab
3. Add these secret values:
   - `Firebase__ProjectId` = `lostandfound-d22e2`
   - `MicrosoftGraph__AccessToken` = (your token)
   - `MicrosoftGraph__EmailAccessToken` = (your token)

4. Upload Secret File:
   - Click **"Secret Files"**
   - Add file: `firebase-key.json`
   - Paste your Firebase credentials JSON

5. Click **"Save Changes"**

#### ✅ DONE! 
Your app will auto-deploy with PostgreSQL!

---

## 🔧 METHOD 2: MANUAL (More Control)

### Step-by-Step Manual Setup

#### Step 1: Create PostgreSQL Database
1. Go to: https://dashboard.render.com
2. Click **"New +"** → **"PostgreSQL"**

3. Fill in the form:
   ```
   Name:          lostandfound-db
   Database:      lostandfounddb
   User:          lostandfounduser
   Region:        [Same as your web service - probably Oregon/Singapore]
   PostgreSQL Version: 16
   Instance Type: Free
   ```

4. Click **"Create Database"**

5. Wait 2-3 minutes for database to be ready

#### Step 2: Get Database Connection String
1. Once created, you'll see the database dashboard
2. Look for **"Internal Database URL"** (it looks like this):
   ```
   postgresql://lostandfounduser:password@hostname/lostandfounddb
   ```
3. **COPY THIS URL** - you'll need it!

⚠️ **Important**: Use **Internal URL** (not External)
   - Internal = Free, fast (within Render network)
   - External = Costs bandwidth

#### Step 3: Update Your Web Service
1. Go to your web service (`lostandfound-app`)
2. Click **"Environment"** tab
3. Find these variables and update:

   **Change this:**
   ```
   DatabaseProvider = Sqlite
   ```
   **To this:**
   ```
   DatabaseProvider = PostgreSQL
   ```

   **Change this:**
   ```
   ConnectionStrings__DefaultConnection = Data Source=/app/data/LostAndFound.db
   ```
   **To this:** (paste your Internal Database URL)
   ```
   ConnectionStrings__DefaultConnection = postgresql://lostandfounduser:password@hostname/lostandfounddb
   ```

4. Add other secrets:
   - `Firebase__ProjectId`
   - `MicrosoftGraph__AccessToken`
   - `MicrosoftGraph__EmailAccessToken`
   - Upload `firebase-key.json` as Secret File

5. Click **"Save Changes"**

#### Step 4: Verify Deployment
1. Your app will automatically redeploy
2. Wait 3-5 minutes
3. Check **"Logs"** tab for:
   ```
   [STARTUP] ✅ Database initialized
   ```

#### ✅ DONE!
Your app now uses PostgreSQL with persistent data!

---

## 🔍 How to Verify It's Working

### Check 1: Logs
Go to your web service → **"Logs"** tab

Look for:
```
[STARTUP] ✅ Database initialized
```

If you see errors, check:
- Connection string is correct
- DatabaseProvider is "PostgreSQL"
- Database is in same region

### Check 2: Test Your App
1. Open your app URL: `https://lostandfound-app.onrender.com`
2. Create a test user/item
3. Wait for service to spin down (15 min inactivity)
4. Access app again
5. **Check if data is still there** ✅

### Check 3: Database Dashboard
Go to PostgreSQL database in Render:
- Check **"Info"** tab
- Should show: Status = Available
- Connections = Active

---

## 💰 Cost Breakdown

### PostgreSQL Free Tier:
- ✅ **FREE for 90 days**
- ✅ 256 MB storage (enough for thousands of users/items)
- ✅ Shared CPU (good performance for small apps)

### After 90 Days:
- Option 1: **Upgrade to $7/month** (database stays active)
- Option 2: **Do nothing** (database suspended, but data kept)
  - Can reactivate later by paying
  - Your data is safe!

### Web Service:
- ✅ **FREE forever** (with limitations)
- Spins down after 15 min inactivity
- 750 hours/month free

---

## 🆘 Troubleshooting

### ❌ "Connection refused" error
**Problem**: Can't connect to PostgreSQL

**Solution**:
1. Check database is in **same region** as web service
2. Use **Internal Database URL** not External
3. Verify connection string format is correct

### ❌ "Database doesn't exist" error
**Problem**: Database not initialized

**Solution**:
1. Check logs for migration errors
2. Database will auto-create on first run
3. If stuck, delete and recreate database

### ❌ App won't start after changes
**Problem**: Wrong environment variables

**Solution**:
1. Double-check `DatabaseProvider` = `PostgreSQL` (exact spelling)
2. Verify connection string has no typos
3. Check all required env vars are set

### ❌ Still losing data
**Problem**: Still using SQLite

**Solution**:
1. Check environment variables were saved
2. Verify deployment happened after changes
3. Check logs for "Using Sqlite" vs "Using PostgreSQL"

---

## 📞 Need Help?

1. **Check Logs First**: Most issues show clear errors in logs
2. **Verify Environment Variables**: Print them in logs to debug
3. **Database Status**: Check if PostgreSQL is "Available"
4. **Region Mismatch**: Database and app must be in same region

---

## 🎉 Success Checklist

After setup, you should have:
- ✅ PostgreSQL database created and running
- ✅ Web service using PostgreSQL (not SQLite)
- ✅ Connection string configured correctly
- ✅ App starts without errors
- ✅ Data persists across restarts
- ✅ No more data loss!

**Your data is now safe and permanent!** 🎊

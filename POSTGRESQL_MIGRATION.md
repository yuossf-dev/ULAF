# 🐘 PostgreSQL Migration Guide for Render

## Why PostgreSQL?
✅ **Data persists** across deployments and restarts
✅ **Free tier available** on Render (90 days, then suspended but data retained)
✅ **Production-ready** database
✅ **Better performance** than SQLite for web apps

## 🚀 Deployment Steps

### Option A: Using render.yaml (Easiest - Infrastructure as Code)

1. **Push updated code to GitHub**:
   ```bash
   cd C:\Users\Victus\Desktop\EntityFrameWork_Pro
   git add .
   git commit -m "Add PostgreSQL support"
   git push
   ```

2. **In Render Dashboard**:
   - Go to your existing service
   - Click "Blueprint" → "Apply Blueprint"
   - Or delete existing service and create new from render.yaml
   - Render will automatically create both PostgreSQL database AND web service

3. **Done!** Render handles the connection string automatically

### Option B: Manual Setup (More Control)

1. **Create PostgreSQL Database**:
   - In Render Dashboard → "New +" → "PostgreSQL"
   - Name: `lostandfound-db`
   - Database: `lostandfounddb` 
   - User: `lostandfounduser`
   - Region: Same as your web service
   - Plan: **Free**
   - Click "Create Database"

2. **Get Connection String**:
   - After creation, copy the **Internal Database URL**
   - Format: `postgresql://user:password@host/database`

3. **Update Web Service Environment Variables**:
   - Go to your web service → "Environment"
   - Change `DatabaseProvider` to `PostgreSQL`
   - Update `ConnectionStrings__DefaultConnection` to the Internal Database URL
   - Click "Save Changes"

4. **Redeploy**:
   - Your app will restart automatically
   - Check logs to confirm PostgreSQL connection

## 🔍 Verify Migration

After deployment, check logs for:
```
[STARTUP] ✅ Database initialized
```

If you see errors, check:
- Connection string is correct
- DatabaseProvider is set to "PostgreSQL"
- PostgreSQL database is running

## 📊 Migration Notes

### Data Migration
⚠️ **Your existing SQLite data will NOT be migrated automatically**

If you need to preserve existing data:
1. Export data from SQLite (before migration)
2. After PostgreSQL is set up, manually re-create users/items
3. Or write a migration script

### Cost
- **PostgreSQL Free Tier**: 90 days free, then:
  - Database suspended but data retained
  - Upgrade to $7/month to reactivate
- **Alternative**: Keep SQLite if data loss is acceptable

## 🎯 What Changed

1. ✅ Added `Npgsql.EntityFrameworkCore.PostgreSQL` package
2. ✅ Updated `Program.cs` to support PostgreSQL
3. ✅ Updated `render.yaml` to provision PostgreSQL database
4. ✅ Changed DatabaseProvider from "Sqlite" to "PostgreSQL"

## 🔄 Rollback (if needed)

If something goes wrong, you can rollback:
1. Change `DatabaseProvider` back to `Sqlite`
2. Revert ConnectionString changes
3. Redeploy

## 📞 Troubleshooting

### Connection refused error
- Check PostgreSQL database is in same region as web service
- Use **Internal Database URL** not External

### Migration errors
- Delete and recreate database
- Clear Migrations folder and regenerate

### App won't start
- Check environment variables are set
- Verify connection string format

## 🎉 Success!

Once deployed with PostgreSQL:
- ✅ Data persists across deployments
- ✅ No more data loss on restarts
- ✅ Better performance
- ✅ Production-ready setup

# Database Configuration Guide

## Switch Between SQL Server and Firebase

Your app now supports **both SQL Server and Firebase**! 

### How to Switch:

Edit `appsettings.json` and change the `DatabaseProvider` value:

```json
{
  "DatabaseProvider": "SqlServer"  // or "Firebase"
}
```

---

## SQL Server Setup (Current Default)

**Configuration:**
```json
{
  "DatabaseProvider": "SqlServer",
  "ConnectionStrings": {
    "DefaultConnection": "Server=LAPTOP-2P4LCSEF\\SQLEXPRESS;Database=ItemsDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

**Requirements:**
- SQL Server installed
- Database migrations applied
- No additional packages needed

---

## Firebase Setup

### Step 1: Install Firebase Package

```bash
dotnet add package Google.Cloud.Firestore
```

### Step 2: Create Firebase Project

1. Go to [Firebase Console](https://console.firebase.google.com/)
2. Create new project
3. Enable Firestore Database
4. Go to Project Settings → Service Accounts
5. Generate new private key (downloads JSON file)
6. Save the JSON file to your project folder

### Step 3: Update Configuration

```json
{
  "DatabaseProvider": "Firebase",
  "Firebase": {
    "ProjectId": "your-firebase-project-id",
    "CredentialsPath": "path/to/your-firebase-credentials.json"
  }
}
```

### Step 4: Run the App

```bash
dotnet run
```

---

## Benefits of Each:

### SQL Server (Local)
✅ No internet required  
✅ Fast performance  
✅ Full control  
✅ Complex queries easy  
❌ Needs SQL Server installed  

### Firebase (Cloud)
✅ No server management  
✅ Real-time updates  
✅ Free tier available  
✅ Automatic scaling  
✅ Works from anywhere  
❌ Needs internet connection  

---

## Repository Pattern Benefits:

Your code uses the **Repository Pattern**, which means:

✅ Easy to switch databases (just change config)  
✅ Controllers don't know which database is used  
✅ Can test with mock data easily  
✅ Clean separation of concerns  
✅ Professional architecture  

Perfect for university projects! 🎓

using EntityFrameWork_Pro.DataBaseB;
using EntityFrameWork_Pro.Interfaces;
using EntityFrameWork_Pro.Repositories;
using EntityFrameWork_Pro.Services;
using Microsoft.EntityFrameworkCore;
using Google.Cloud.Firestore;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllersWithViews();

// Register Microsoft Graph Service (for student validation)
builder.Services.AddSingleton<MicrosoftGraphService>();

// Register Email Service (uses its own token)
builder.Services.AddSingleton<EmailService>();

// Debug: Check if tokens are configured
var validationToken = builder.Configuration["MicrosoftGraph:AccessToken"];
var emailToken = builder.Configuration["MicrosoftGraph:EmailAccessToken"];
Console.WriteLine($"==========================================");
Console.WriteLine($"[STARTUP] Validation Token configured: {!string.IsNullOrEmpty(validationToken)}");
Console.WriteLine($"[STARTUP] Email Token configured: {!string.IsNullOrEmpty(emailToken)}");
Console.WriteLine($"==========================================");

// Configure database based on provider setting
var dbProvider = builder.Configuration["DatabaseProvider"];
builder.Services.AddDbContext<DBBridge>(options =>
{
    if (dbProvider == "Sqlite")
    {
        options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
    }
    else
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    }
});

// Try to initialize Firebase (for online mode)
FirestoreDb firestore = null;
try
{
    var projectId = builder.Configuration["Firebase:ProjectId"];
    var credentialsPath = builder.Configuration["Firebase:CredentialsPath"];
    
    if (!string.IsNullOrEmpty(projectId) && !string.IsNullOrEmpty(credentialsPath) && File.Exists(credentialsPath))
    {
        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialsPath);
        firestore = FirestoreDb.Create(projectId);
        builder.Services.AddSingleton(firestore);
    }
}
catch (Exception)
{
    // Firebase not configured, will use SQL Server only
}

// Register repositories
builder.Services.AddScoped<SqlServerRepository>();
builder.Services.AddScoped<SqlServerItemRepository>();
builder.Services.AddScoped<SqlServerUserRepository>();

if (firestore != null)
{
    builder.Services.AddScoped<FirebaseItemRepository>();
    builder.Services.AddScoped<FirebaseUserRepository>();
}

// Register the combined repository for admin
builder.Services.AddScoped<IRepository>(provider =>
{
    return provider.GetRequiredService<SqlServerRepository>();
});

// Register the repository resolver based on DatabaseMode
builder.Services.AddScoped<IItemRepository>(provider =>
{
    if (DatabaseMode.IsOnline && firestore != null)
        return provider.GetRequiredService<FirebaseItemRepository>();
    return provider.GetRequiredService<SqlServerItemRepository>();
});

builder.Services.AddScoped<IUserRepository>(provider =>
{
    if (DatabaseMode.IsOnline && firestore != null)
        return provider.GetRequiredService<FirebaseUserRepository>();
    return provider.GetRequiredService<SqlServerUserRepository>();
});

// Add session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// ⚡ ONE-TIME UPDATE: Mark existing users as verified
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DBBridge>();
    var unverifiedUsers = db.Users.Where(u => !u.IsEmailVerified).ToList();
    
    if (unverifiedUsers.Any())
    {
        Console.WriteLine($"[UPDATE] Found {unverifiedUsers.Count} unverified users");
        foreach (var user in unverifiedUsers)
        {
            user.IsEmailVerified = true;
            Console.WriteLine($"[UPDATE] Marked {user.UserName} ({user.StudentId}) as verified");
        }
        db.SaveChanges();
        Console.WriteLine("[UPDATE] ✅ All existing users marked as verified!");
    }
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSession();
app.UseAuthorization();

// Default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Welcome}/{id?}");

app.Run();

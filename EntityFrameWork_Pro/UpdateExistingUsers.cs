using Microsoft.EntityFrameworkCore;
using EntityFrameWork_Pro.DataBaseB;

namespace EntityFrameWork_Pro
{
    public class UpdateExistingUsers
    {
        public static void Run(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<DBBridge>();
            
            var unverifiedUsers = db.Users.Where(u => !u.IsEmailVerified).ToList();
            
            Console.WriteLine($"Found {unverifiedUsers.Count} unverified users");
            
            foreach (var user in unverifiedUsers)
            {
                user.IsEmailVerified = true;
                Console.WriteLine($"Marked {user.UserName} ({user.StudentId}) as verified");
            }
            
            db.SaveChanges();
            Console.WriteLine("✅ All existing users marked as verified!");
        }
    }
}

using EntityFrameWork_Pro.Interfaces;
using EntityFrameWork_Pro.Models;

namespace EntityFrameWork_Pro.Repositories
{
    /// <summary>
    /// Dual repository that saves users to BOTH SQL Server and Firebase simultaneously
    /// </summary>
    public class DualUserRepository : IUserRepository
    {
        private readonly SqlServerUserRepository _sqlRepo;
        private readonly FirebaseUserRepository _firebaseRepo;
        private readonly bool _hasFirebase;

        public DualUserRepository(
            SqlServerUserRepository sqlRepo,
            IServiceProvider serviceProvider)
        {
            _sqlRepo = sqlRepo;
            
            // Try to get Firebase repository if available
            try
            {
                _firebaseRepo = serviceProvider.GetService<FirebaseUserRepository>();
                _hasFirebase = _firebaseRepo != null;
                Console.WriteLine($"[DUAL-USER-REPO] Firebase available: {_hasFirebase}");
            }
            catch
            {
                _hasFirebase = false;
                Console.WriteLine("[DUAL-USER-REPO] Firebase not available - SQL only mode");
            }
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            // SqlServerUserRepository doesn't have GetUserByIdAsync
            // This method is rarely used, so we'll return null or throw
            throw new NotImplementedException("GetUserByIdAsync not implemented in SQL repository");
        }

        public async Task<User> GetUserByStudentIdAsync(string studentId)
        {
            return await _sqlRepo.GetUserByStudentIdAsync(studentId);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _sqlRepo.GetUserByEmailAsync(email);
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await _sqlRepo.GetUserByUsernameAsync(username);
        }

        public async Task<bool> UserExistsAsync(string username, string email)
        {
            return await _sqlRepo.UserExistsAsync(username, email);
        }

        public async Task<bool> UserExistsByStudentIdAsync(string studentId)
        {
            return await _sqlRepo.UserExistsByStudentIdAsync(studentId);
        }

        public async Task<User> GetUserByStudentIdAndPasswordAsync(string studentId, string password)
        {
            return await _sqlRepo.GetUserByStudentIdAndPasswordAsync(studentId, password);
        }

        public async Task<User> AddUserAsync(User user)
        {
            Console.WriteLine($"[DUAL-USER-REPO] Adding user: {user.UserName}");
            
            // 1. Save to SQL first (primary storage)
            var sqlResult = await _sqlRepo.AddUserAsync(user);
            Console.WriteLine($"[DUAL-USER-REPO] ✅ Saved to SQL - ID: {sqlResult.Id}");

            // 2. Also save to Firebase if available (backup/cloud storage)
            if (_hasFirebase)
            {
                try
                {
                    await _firebaseRepo.AddUserAsync(sqlResult);
                    Console.WriteLine($"[DUAL-USER-REPO] ✅ Saved to Firebase - ID: {sqlResult.Id}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[DUAL-USER-REPO] ⚠️ Firebase save failed (SQL still saved): {ex.Message}");
                    // Continue - SQL save was successful
                }
            }

            return sqlResult;
        }

        // Synchronous methods for compatibility
        public User GetUserById(int id)
        {
            return GetUserByIdAsync(id).GetAwaiter().GetResult();
        }

        public User GetUserByStudentId(string studentId)
        {
            return GetUserByStudentIdAsync(studentId).GetAwaiter().GetResult();
        }

        public User GetUserByEmail(string email)
        {
            return GetUserByEmailAsync(email).GetAwaiter().GetResult();
        }

        public User GetUserByUsername(string username)
        {
            return GetUserByUsernameAsync(username).GetAwaiter().GetResult();
        }

        public bool UserExists(string username, string email)
        {
            return UserExistsAsync(username, email).GetAwaiter().GetResult();
        }

        public void AddUser(User user)
        {
            AddUserAsync(user).GetAwaiter().GetResult();
        }
    }
}

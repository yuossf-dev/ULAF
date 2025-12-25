using EntityFrameWork_Pro.DataBaseB;
using EntityFrameWork_Pro.Interfaces;
using EntityFrameWork_Pro.Models;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameWork_Pro.Repositories
{
    public class SqlServerRepository : IRepository
    {
        private readonly DBBridge _db;

        public SqlServerRepository(DBBridge db)
        {
            _db = db;
        }

        // Item methods
        public async Task<IEnumerable<Item>> GetAllItemsAsync()
        {
            return await _db.Items.Include(i => i.PostedBy).ToListAsync();
        }

        public async Task<IEnumerable<Item>> GetLostItemsAsync()
        {
            return await _db.Items.Include(i => i.PostedBy).Where(i => i.Status == "Lost").ToListAsync();
        }

        public async Task<IEnumerable<Item>> GetFoundItemsAsync()
        {
            return await _db.Items.Include(i => i.PostedBy).Where(i => i.Status == "Found").ToListAsync();
        }

        public async Task<Item> GetItemByIdAsync(int id)
        {
            return await _db.Items.Include(i => i.PostedBy).FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<Item> AddItemAsync(Item item)
        {
            _db.Items.Add(item);
            await _db.SaveChangesAsync();
            return item;
        }

        public async Task<Item> UpdateItemAsync(Item item)
        {
            _db.Items.Update(item);
            await _db.SaveChangesAsync();
            return item;
        }

        public async Task<bool> DeleteItemAsync(Item item)
        {
            if (item == null) return false;
            
            _db.Items.Remove(item);
            await _db.SaveChangesAsync();
            return true;
        }

        public IEnumerable<Item> GetAllItems()
        {
            return _db.Items.Include(i => i.PostedBy).ToList();
        }

        // User methods
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _db.Users.ToListAsync();
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await _db.Users.FirstOrDefaultAsync(u => u.UserName == username);
        }

        public async Task<User> GetUserByStudentIdAsync(string studentId)
        {
            return await _db.Users.FirstOrDefaultAsync(u => u.StudentId == studentId);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> AddUserAsync(User user)
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return user;
        }

        public async Task<bool> UserExistsAsync(string username, string email)
        {
            return await _db.Users.AnyAsync(u => u.UserName == username || u.Email == email);
        }

        public async Task<bool> DeleteUserAsync(User user)
        {
            if (user == null) return false;
            
            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
            return true;
        }

        public User GetUserByUsername(string username)
        {
            return _db.Users.FirstOrDefault(u => u.UserName == username);
        }
    }

    public class SqlServerItemRepository : IItemRepository
    {
        private readonly DBBridge _db;

        public SqlServerItemRepository(DBBridge db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Item>> GetAllItemsAsync()
        {
            return await _db.Items.Include(i => i.PostedBy).ToListAsync();
        }

        public async Task<IEnumerable<Item>> GetLostItemsAsync()
        {
            return await _db.Items.Include(i => i.PostedBy).Where(i => i.Status == "Lost").ToListAsync();
        }

        public async Task<IEnumerable<Item>> GetFoundItemsAsync()
        {
            return await _db.Items.Include(i => i.PostedBy).Where(i => i.Status == "Found").ToListAsync();
        }

        public async Task<Item> GetItemByIdAsync(int id)
        {
            return await _db.Items.Include(i => i.PostedBy).FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<Item> AddItemAsync(Item item)
        {
            _db.Items.Add(item);
            await _db.SaveChangesAsync();
            return item;
        }

        public async Task<Item> UpdateItemAsync(Item item)
        {
            _db.Items.Update(item);
            await _db.SaveChangesAsync();
            return item;
        }

        public async Task<bool> DeleteItemAsync(int id)
        {
            var item = await _db.Items.FindAsync(id);
            if (item == null) return false;
            
            _db.Items.Remove(item);
            await _db.SaveChangesAsync();
            return true;
        }

        public IEnumerable<Item> GetAllItems()
        {
            return _db.Items.Include(i => i.PostedBy).ToList();
        }
    }

    public class SqlServerUserRepository : IUserRepository
    {
        private readonly DBBridge _db;

        public SqlServerUserRepository(DBBridge db)
        {
            _db = db;
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await _db.Users.FirstOrDefaultAsync(u => u.UserName == username);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> AddUserAsync(User user)
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return user;
        }

        public async Task<bool> UserExistsAsync(string username, string email)
        {
            return await _db.Users.AnyAsync(u => u.UserName == username || u.Email == email);
        }

        public User GetUserByUsername(string username)
        {
            return _db.Users.FirstOrDefault(u => u.UserName == username);
        }
    }
}
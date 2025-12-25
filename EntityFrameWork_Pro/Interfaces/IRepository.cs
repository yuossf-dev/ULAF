using EntityFrameWork_Pro.Models;

namespace EntityFrameWork_Pro.Interfaces
{
    public interface IRepository
    {
        // Item methods
        Task<IEnumerable<Item>> GetAllItemsAsync();
        Task<IEnumerable<Item>> GetLostItemsAsync();
        Task<IEnumerable<Item>> GetFoundItemsAsync();
        Task<Item> GetItemByIdAsync(int id);
        Task<Item> AddItemAsync(Item item);
        Task<Item> UpdateItemAsync(Item item);
        Task<bool> DeleteItemAsync(Item item);
        IEnumerable<Item> GetAllItems();

        // User methods
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> GetUserByUsernameAsync(string username);
        Task<User> GetUserByStudentIdAsync(string studentId);
        Task<User> GetUserByEmailAsync(string email);
        Task<User> AddUserAsync(User user);
        Task<bool> UserExistsAsync(string username, string email);
        Task<bool> DeleteUserAsync(User user);
        User GetUserByUsername(string username);
    }

    public interface IItemRepository
    {
        Task<IEnumerable<Item>> GetAllItemsAsync();
        Task<IEnumerable<Item>> GetLostItemsAsync();
        Task<IEnumerable<Item>> GetFoundItemsAsync();
        Task<Item> GetItemByIdAsync(int id);
        Task<Item> AddItemAsync(Item item);
        Task<Item> UpdateItemAsync(Item item);
        Task<bool> DeleteItemAsync(int id);
        IEnumerable<Item> GetAllItems();
    }

    public interface IUserRepository
    {
        Task<User> GetUserByUsernameAsync(string username);
        Task<User> GetUserByEmailAsync(string email);
        Task<User> AddUserAsync(User user);
        Task<bool> UserExistsAsync(string username, string email);
        User GetUserByUsername(string username);
    }
}

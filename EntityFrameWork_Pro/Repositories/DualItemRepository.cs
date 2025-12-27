using EntityFrameWork_Pro.Interfaces;
using EntityFrameWork_Pro.Models;

namespace EntityFrameWork_Pro.Repositories
{
    /// <summary>
    /// Dual repository that saves items to BOTH SQL Server and Firebase simultaneously
    /// </summary>
    public class DualItemRepository : IItemRepository
    {
        private readonly SqlServerItemRepository _sqlRepo;
        private readonly FirebaseItemRepository _firebaseRepo;
        private readonly bool _hasFirebase;

        public DualItemRepository(
            SqlServerItemRepository sqlRepo,
            IServiceProvider serviceProvider)
        {
            _sqlRepo = sqlRepo;
            
            // Try to get Firebase repository if available
            try
            {
                _firebaseRepo = serviceProvider.GetService<FirebaseItemRepository>();
                _hasFirebase = _firebaseRepo != null;
                Console.WriteLine($"[DUAL-REPO] Firebase available: {_hasFirebase}");
            }
            catch
            {
                _hasFirebase = false;
                Console.WriteLine("[DUAL-REPO] Firebase not available - SQL only mode");
            }
        }

        public async Task<IEnumerable<Item>> GetAllItemsAsync()
        {
            // Read from SQL (faster and more reliable)
            return await _sqlRepo.GetAllItemsAsync();
        }

        public async Task<IEnumerable<Item>> GetLostItemsAsync()
        {
            return await _sqlRepo.GetLostItemsAsync();
        }

        public async Task<IEnumerable<Item>> GetFoundItemsAsync()
        {
            return await _sqlRepo.GetFoundItemsAsync();
        }

        public async Task<Item> GetItemByIdAsync(int id)
        {
            return await _sqlRepo.GetItemByIdAsync(id);
        }

        public async Task<Item> AddItemAsync(Item item)
        {
            Console.WriteLine($"[DUAL-REPO] Adding item: {item.Name}");
            
            // 1. Save to SQL first (primary storage)
            var sqlResult = await _sqlRepo.AddItemAsync(item);
            Console.WriteLine($"[DUAL-REPO] ✅ Saved to SQL - ID: {sqlResult.Id}");

            // 2. Also save to Firebase if available (backup/cloud storage)
            if (_hasFirebase)
            {
                try
                {
                    await _firebaseRepo.AddItemAsync(sqlResult);
                    Console.WriteLine($"[DUAL-REPO] ✅ Saved to Firebase - ID: {sqlResult.Id}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[DUAL-REPO] ⚠️ Firebase save failed (SQL still saved): {ex.Message}");
                    // Continue - SQL save was successful
                }
            }

            return sqlResult;
        }

        public async Task<Item> UpdateItemAsync(Item item)
        {
            Console.WriteLine($"[DUAL-REPO] Updating item: {item.Id}");
            
            // 1. Update in SQL
            var sqlResult = await _sqlRepo.UpdateItemAsync(item);
            Console.WriteLine($"[DUAL-REPO] ✅ Updated in SQL");

            // 2. Update in Firebase if available
            if (_hasFirebase)
            {
                try
                {
                    await _firebaseRepo.UpdateItemAsync(sqlResult);
                    Console.WriteLine($"[DUAL-REPO] ✅ Updated in Firebase");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[DUAL-REPO] ⚠️ Firebase update failed: {ex.Message}");
                }
            }

            return sqlResult;
        }

        public async Task<bool> DeleteItemAsync(int id)
        {
            Console.WriteLine($"[DUAL-REPO] Deleting item: {id}");
            
            // 1. Delete from SQL
            var sqlResult = await _sqlRepo.DeleteItemAsync(id);
            Console.WriteLine($"[DUAL-REPO] ✅ Deleted from SQL");

            // 2. Delete from Firebase if available
            if (_hasFirebase)
            {
                try
                {
                    await _firebaseRepo.DeleteItemAsync(id);
                    Console.WriteLine($"[DUAL-REPO] ✅ Deleted from Firebase");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[DUAL-REPO] ⚠️ Firebase delete failed: {ex.Message}");
                }
            }

            return sqlResult;
        }

        // Synchronous methods for compatibility
        public IEnumerable<Item> GetAllItems()
        {
            return _sqlRepo.GetAllItems();
        }

        public IEnumerable<Item> GetLostItems()
        {
            return _sqlRepo.GetLostItems();
        }

        public IEnumerable<Item> GetFoundItems()
        {
            return _sqlRepo.GetFoundItems();
        }

        public Item GetItemById(int id)
        {
            return _sqlRepo.GetItemById(id);
        }

        public void AddItem(Item item)
        {
            Console.WriteLine($"[DUAL-REPO] Adding item (sync): {item.Name}");
            
            // Save to SQL
            _sqlRepo.AddItem(item);
            Console.WriteLine($"[DUAL-REPO] ✅ Saved to SQL (sync)");

            // Save to Firebase if available
            if (_hasFirebase)
            {
                try
                {
                    _firebaseRepo.AddItem(item);
                    Console.WriteLine($"[DUAL-REPO] ✅ Saved to Firebase (sync)");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[DUAL-REPO] ⚠️ Firebase save failed (sync): {ex.Message}");
                }
            }
        }

        public void UpdateItem(Item item)
        {
            _sqlRepo.UpdateItem(item);
            
            if (_hasFirebase)
            {
                try
                {
                    _firebaseRepo.UpdateItem(item);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[DUAL-REPO] ⚠️ Firebase update failed (sync): {ex.Message}");
                }
            }
        }

        public void DeleteItem(int id)
        {
            _sqlRepo.DeleteItem(id);
            
            if (_hasFirebase)
            {
                try
                {
                    _firebaseRepo.DeleteItem(id);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[DUAL-REPO] ⚠️ Firebase delete failed (sync): {ex.Message}");
                }
            }
        }
    }
}

using EntityFrameWork_Pro.Interfaces;
using EntityFrameWork_Pro.Models;
using Google.Cloud.Firestore;

namespace EntityFrameWork_Pro.Repositories
{
    public class FirebaseItemRepository : IItemRepository
    {
        private readonly FirestoreDb _firestore;
        private readonly string _collectionName = "Items";

        public FirebaseItemRepository(FirestoreDb firestore)
        {
            _firestore = firestore;
        }

        public async Task<IEnumerable<Item>> GetAllItemsAsync()
        {
            var snapshot = await _firestore.Collection(_collectionName).GetSnapshotAsync();
            return snapshot.Documents.Select(d => ConvertToItem(d)).ToList();
        }

        public async Task<IEnumerable<Item>> GetLostItemsAsync()
        {
            var query = _firestore.Collection(_collectionName).WhereEqualTo("Status", "Lost");
            var snapshot = await query.GetSnapshotAsync();
            return snapshot.Documents.Select(d => ConvertToItem(d)).ToList();
        }

        public async Task<IEnumerable<Item>> GetFoundItemsAsync()
        {
            var query = _firestore.Collection(_collectionName).WhereEqualTo("Status", "Found");
            var snapshot = await query.GetSnapshotAsync();
            return snapshot.Documents.Select(d => ConvertToItem(d)).ToList();
        }

        public async Task<Item> GetItemByIdAsync(int id)
        {
            var query = _firestore.Collection(_collectionName).WhereEqualTo("Id", id);
            var snapshot = await query.GetSnapshotAsync();
            return snapshot.Documents.Count > 0 ? ConvertToItem(snapshot.Documents[0]) : null;
        }

        public async Task<Item> AddItemAsync(Item item)
        {
            var docRef = _firestore.Collection(_collectionName).Document();
            item.Id = Math.Abs(docRef.Id.GetHashCode());
            
            await docRef.SetAsync(new
            {
                item.Id,
                item.Name,
                item.Category,
                item.Description,
                item.Location,
                Date = item.Date.ToString("yyyy-MM-dd HH:mm:ss"),
                item.Status,
                item.ContactInfo,
                item.MediaPathsJson,
                item.UserId, // ✅ Save UserId
                PosterName = item.PostedBy?.UserName ?? item.PosterName // ✅ Save poster name
            });
            
            return item;
        }

        public async Task<Item> UpdateItemAsync(Item item)
        {
            var query = _firestore.Collection(_collectionName).WhereEqualTo("Id", item.Id);
            var snapshot = await query.GetSnapshotAsync();
            if (snapshot.Documents.Count > 0)
            {
                var docRef = snapshot.Documents[0].Reference;
                await docRef.SetAsync(new
                {
                    item.Id,
                    item.Name,
                    item.Category,
                    item.Description,
                    item.Location,
                    Date = item.Date.ToString("yyyy-MM-dd HH:mm:ss"),
                    item.Status,
                    item.ContactInfo,
                    item.MediaPathsJson,
                    item.UserId, // ✅ Save UserId
                    PosterName = item.PostedBy?.UserName ?? item.PosterName // ✅ Save poster name
                });
            }
            return item;
        }

        public async Task<bool> DeleteItemAsync(int id)
        {
            var query = _firestore.Collection(_collectionName).WhereEqualTo("Id", id);
            var snapshot = await query.GetSnapshotAsync();
            if (snapshot.Documents.Count > 0)
            {
                await snapshot.Documents[0].Reference.DeleteAsync();
                return true;
            }
            return false;
        }

        public IEnumerable<Item> GetAllItems()
        {
            return GetAllItemsAsync().GetAwaiter().GetResult();
        }

        private Item ConvertToItem(DocumentSnapshot doc)
        {
            var item = new Item
            {
                Id = doc.GetValue<int>("Id"),
                Name = doc.GetValue<string>("Name"),
                Category = doc.GetValue<string>("Category"),
                Description = doc.GetValue<string>("Description"),
                Location = doc.GetValue<string>("Location"),
                Date = DateTime.Parse(doc.GetValue<string>("Date")),
                Status = doc.GetValue<string>("Status"),
                ContactInfo = doc.GetValue<string>("ContactInfo"),
                MediaPathsJson = doc.GetValue<string>("MediaPathsJson")
            };
            
            // Try to get UserId
            try
            {
                item.UserId = doc.ContainsField("UserId") ? doc.GetValue<int?>("UserId") : null;
            }
            catch { }
            
            // ✅ Create a temporary User object with the poster name
            try
            {
                var posterName = doc.ContainsField("PosterName") ? doc.GetValue<string>("PosterName") : null;
                if (!string.IsNullOrEmpty(posterName))
                {
                    item.PostedBy = new User { UserName = posterName };
                }
            }
            catch { }
            
            return item;
        }
    }

    public class FirebaseUserRepository : IUserRepository
    {
        private readonly FirestoreDb _firestore;
        private readonly string _collectionName = "Users";

        public FirebaseUserRepository(FirestoreDb firestore)
        {
            _firestore = firestore;
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            var query = _firestore.Collection(_collectionName).WhereEqualTo("UserName", username);
            var snapshot = await query.GetSnapshotAsync();
            return snapshot.Documents.Count > 0 ? ConvertToUser(snapshot.Documents[0]) : null;
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            var query = _firestore.Collection(_collectionName).WhereEqualTo("Email", email);
            var snapshot = await query.GetSnapshotAsync();
            return snapshot.Documents.Count > 0 ? ConvertToUser(snapshot.Documents[0]) : null;
        }

        public async Task<User> AddUserAsync(User user)
        {
            var docRef = _firestore.Collection(_collectionName).Document();
            user.Id = Math.Abs(docRef.Id.GetHashCode());
            
            await docRef.SetAsync(new
            {
                user.Id,
                user.StudentId,
                user.UserName,
                user.Email,
                user.Password,
                user.Phone,
                user.IsEmailVerified
            });
            
            return user;
        }

        public async Task<bool> UserExistsAsync(string username, string email)
        {
            var usernameQuery = _firestore.Collection(_collectionName).WhereEqualTo("UserName", username);
            var emailQuery = _firestore.Collection(_collectionName).WhereEqualTo("Email", email);
            
            var usernameSnapshot = await usernameQuery.GetSnapshotAsync();
            var emailSnapshot = await emailQuery.GetSnapshotAsync();
            
            return usernameSnapshot.Documents.Count > 0 || emailSnapshot.Documents.Count > 0;
        }

        public User GetUserByUsername(string username)
        {
            return GetUserByUsernameAsync(username).GetAwaiter().GetResult();
        }

        public async Task<User> GetUserByStudentIdAsync(string studentId)
        {
            var query = _firestore.Collection(_collectionName).WhereEqualTo("StudentId", studentId);
            var snapshot = await query.GetSnapshotAsync();
            return snapshot.Documents.Count > 0 ? ConvertToUser(snapshot.Documents[0]) : null;
        }

        public async Task<bool> UserExistsByStudentIdAsync(string studentId)
        {
            var query = _firestore.Collection(_collectionName).WhereEqualTo("StudentId", studentId);
            var snapshot = await query.GetSnapshotAsync();
            return snapshot.Documents.Count > 0;
        }

        public async Task<User> GetUserByStudentIdAndPasswordAsync(string studentId, string password)
        {
            var query = _firestore.Collection(_collectionName)
                .WhereEqualTo("StudentId", studentId)
                .WhereEqualTo("Password", password);
            var snapshot = await query.GetSnapshotAsync();
            return snapshot.Documents.Count > 0 ? ConvertToUser(snapshot.Documents[0]) : null;
        }

        private User ConvertToUser(DocumentSnapshot doc)
        {
            return new User
            {
                Id = doc.GetValue<int>("Id"),
                StudentId = doc.ContainsField("StudentId") ? doc.GetValue<string>("StudentId") : null,
                UserName = doc.GetValue<string>("UserName"),
                Email = doc.GetValue<string>("Email"),
                Password = doc.GetValue<string>("Password"),
                Phone = doc.ContainsField("Phone") ? doc.GetValue<string>("Phone") : null,
                IsEmailVerified = doc.ContainsField("IsEmailVerified") ? doc.GetValue<bool>("IsEmailVerified") : false
            };
        }
    }
}

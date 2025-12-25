using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using EntityFrameWork_Pro.Helpers;

namespace EntityFrameWork_Pro.Models
{
    public class Item
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Category { get; set; } // Wallet, Phone, Keys, ID, Bag, Other

        public string Description { get; set; }

        [Required]
        public string Location { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

        [Required]
        public string Status { get; set; } // "Lost" or "Found"

        public string ContactInfo { get; set; }

        // ✅ Relationship with User (Who posted this item) - Nullable for existing data
        public int? UserId { get; set; }
        
        [ForeignKey("UserId")]
        public User PostedBy { get; set; }

        [NotMapped]
        public string PosterName => PostedBy?.UserName ?? "Anonymous";

        public string MediaPathsJson { get; set; }

        [NotMapped]
        public List<string> MediaPaths
        {
            get => string.IsNullOrEmpty(MediaPathsJson)
                    ? new List<string>()
                    : JsonSerializer.Deserialize<List<string>>(MediaPathsJson);
            set => MediaPathsJson = JsonSerializer.Serialize(value);
        }

        // Helper properties for displaying items
        [NotMapped]
        public string CategoryIcon => CategoryIconHelper.GetCategoryIcon(Category);

        [NotMapped]
        public string CategoryColor => CategoryIconHelper.GetCategoryColor(Category);

        [NotMapped]
        public bool HasImages => MediaPaths != null && MediaPaths.Any();

        [NotMapped]
        public string FirstImage => HasImages ? MediaPaths.First() : null;
    }
}

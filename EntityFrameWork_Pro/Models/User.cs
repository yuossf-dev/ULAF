using System.ComponentModel.DataAnnotations;

namespace EntityFrameWork_Pro.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string StudentId { get; set; }

        [Required, MaxLength(50)]
        public string UserName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, MinLength(6)]
        public string Password { get; set; }

        [Phone]
        public string Phone { get; set; }

        // ✅ Email verification status
        public bool IsEmailVerified { get; set; } = false;

        // ✅ Navigation property - items posted by this user
        public ICollection<Item> PostedItems { get; set; }
    }
}

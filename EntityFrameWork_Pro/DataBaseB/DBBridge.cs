using Microsoft.EntityFrameworkCore;
using EntityFrameWork_Pro.Models;

namespace EntityFrameWork_Pro.DataBaseB
{
    public class DBBridge : DbContext
    {
        public DBBridge(DbContextOptions<DBBridge> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Item> Items { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Item -> User relationship as optional (no foreign key constraint enforcement)
            modelBuilder.Entity<Item>()
                .HasOne(i => i.PostedBy)
                .WithMany(u => u.PostedItems)
                .HasForeignKey(i => i.UserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull); // If user deleted, set UserId to null
        }
    }
}

using Microsoft.EntityFrameworkCore;
using EntityFrameWork_Pro.Models;

namespace EntityFrameWork_Pro.DataBaseB
{
    public class DBBridge : DbContext
    {
        public DBBridge(DbContextOptions<DBBridge> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Item> Items { get; set; }
    }
}

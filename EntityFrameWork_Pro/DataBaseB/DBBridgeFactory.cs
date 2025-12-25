using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace EntityFrameWork_Pro.DataBaseB
{
    public class DBBridgeFactory : IDesignTimeDbContextFactory<DBBridge>
    {
        public DBBridge CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<DBBridge>();
            var dbProvider = configuration["DatabaseProvider"];
            
            if (dbProvider == "Sqlite")
            {
                optionsBuilder.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
            }
            else
            {
                optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            }

            return new DBBridge(optionsBuilder.Options);
        }
    }
}

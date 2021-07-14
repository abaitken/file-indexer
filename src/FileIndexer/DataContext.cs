using Microsoft.EntityFrameworkCore;

namespace FileIndexer
{
    internal class DataContext : DbContext
    {
        private readonly string connectionString;

        public DataContext(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public DataContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<IndexedFile> IndexedFiles { get; set; }
        public DbSet<ConfigurationValue> ConfigurationValues { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!string.IsNullOrEmpty(connectionString))
                optionsBuilder.UseSqlite(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public void InitialiseAndUpgrade()
        {
            Database.Migrate();
        }
    }
}

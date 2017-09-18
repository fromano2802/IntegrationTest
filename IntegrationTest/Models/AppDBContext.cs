using System.Data.Entity;

namespace IntegrationTest.Models
{
    public class AppKey
    {
        public int AppKeyID { get; set; }
        public string ApiKey { get; set; }
    }

    public class KeyToken
    {
        public int KeyTokenID { get; set; }
        public string ApiKey { get; set; }
        public string Token { get; set; }
    }

    public class AppDBContext : DbContext
    {
        public DbSet<AppKey> AppKeys { get; set; }
        public DbSet<KeyToken> KeyTokens { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<AppDBContext>(null);
        }
    }
}
using System.Data.Common;
using System.Data.Entity.Infrastructure;

namespace IntegrationTest.Tests.Infrastructure
{
    public class EffortProviderFactory : IDbConnectionFactory
    {
        private static DbConnection _connection;
        private static readonly object Lock = new object();
 
        public static void ResetDb()
        {
            lock (Lock)
            {
                _connection = null;
            }
        }
 
        public DbConnection CreateConnection(string nameOrConnectionString)
        {
            lock (Lock)
            {
                return _connection ?? (_connection = Effort.DbConnectionFactory.CreateTransient());
            }
        }
    }
}

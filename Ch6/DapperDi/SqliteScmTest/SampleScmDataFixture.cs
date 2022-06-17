using System;
using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection; 
using ScmDataAccess;

namespace SqliteScmTest
{
    public class SampleScmDataFixture
    {
        private const string PartTypeTable = 
            @"CREATE TABLE PartType(
                Id INTEGER PRIMARY KEY,
                Name VARCHAR(255) NOT NULL
            );";

        public IServiceProvider Services { get; private set; }

        public SampleScmDataFixture()
        {
            // var conn = new SqliteConnection("Data Source=:memory:");
            // conn.Open();

            // (new SqliteCommand(PartTypeTable, conn)).ExecuteNonQuery();

            // var serviceCollection = new ServiceCollection();
            // IScmContext context = new SqliteScmContext(conn);
            // serviceCollection.AddSingleton<IScmContext>(context);
            // Services = serviceCollection.BuildServiceProvider();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddTransient<IScmContext>(
                provider => {
                    var conn = new SqliteConnection("Data Source=:memory:");
                    conn.Open();
                    (new SqliteCommand(PartTypeTable, conn)).ExecuteNonQuery();
                    return new SqliteScmContext(conn);
                }
            );
            Services = serviceCollection.BuildServiceProvider();
        }
    }
}
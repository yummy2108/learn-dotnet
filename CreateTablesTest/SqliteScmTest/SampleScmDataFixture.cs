using System;
using System.Data.Common;
using Microsoft.Data.Sqlite;

namespace SqliteScmTest
{
    public class SampleScmDataFixture : IDisposable
    {
        public SqliteConnection Connection { get; private set; }

        public SampleScmDataFixture()
        {
            var conn = new SqliteConnection("Data Source=:memory:");
            Connection = conn;
            conn.Open();

            var command = new SqliteCommand(
                @"CREATE TABLE PartType(
                    Id INTEGER PRIMARY KEY,
                    Name VARCHAR(255) NOT NULL
                );", conn
            );
            command.ExecuteNonQuery();
            command = new SqliteCommand(
                @"INSERT INTO PartType
                    (Name)
                    VALUES
                    ('8289 L-shaped plate')
                ", conn
            );
            command.ExecuteNonQuery();
        }

        public void Dispose()
        {
            if (Connection != null)
                Connection.Dispose();
        }
    }
}
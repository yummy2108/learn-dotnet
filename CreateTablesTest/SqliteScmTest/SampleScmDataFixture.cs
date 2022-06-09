using System;
using System.Data.Common;
using Microsoft.Data.Sqlite;

namespace SqliteScmTest
{
    public class SampleScmDataFixture : IDisposable
    {
        private const string PartTypeTable = 
            @"CREATE TABLE PartType(
                Id INTEGER PRIMARY KEY,
                Name VARCHAR(255) NOT NULL
            );";

        private const string InventoryItemTable = 
            @"CREATE TABLE InventoryItem(
                PartTypeId INTEGER PRIMARY KEY,
                Count INTEGER NOT NULL,
                OrderThreshold INTEGER,
            FOREIGN KEY(PartTypeId) REFERENCES PartType(Id)
            );";

        public SqliteConnection Connection { get; private set; }

        public SampleScmDataFixture()
        {
            var conn = new SqliteConnection("Data Source=:memory:");
            Connection = conn;
            conn.Open();

            (new SqliteCommand(PartTypeTable, conn)).ExecuteNonQuery();
            (new SqliteCommand(InventoryItemTable, conn)).ExecuteNonQuery();
            (new SqliteCommand(
                @"INSERT INTO PartType
                    (Id, Name)
                    VALUES
                    (0, '8289 L-shaped plate')", conn
            )).ExecuteNonQuery();
            (new SqliteCommand(
                @"INSERT INTO InventoryItem
                    (PartTypeId, Count, OrderThreshold)
                    VALUES
                    (0, 100, 10)", conn
            )).ExecuteNonQuery();
        }

        public void Dispose()
        {
            if (Connection != null)
                Connection.Dispose();
        }
    }
}
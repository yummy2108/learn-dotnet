using System;
using System.Collections.Generic;
using System.Data.Common;

namespace WidgetScmDataAccess
{
    public class ScmContext
    {
        private DbConnection connection;

        public IEnumerable<PartType> Parts { get; private set; }

        public IEnumerable<InventoryItem> Inventory { get; private set; }

        public ScmContext(DbConnection conn)
        {
            connection = conn;
            ReadParts();
            ReadInventory();
        }

        private void ReadParts()
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"SELECT Id, Name FROM PartType";
                using (var reader = command.ExecuteReader())
                {
                    var parts = new List<PartType>();
                    Parts = parts;
                    while(reader.Read())
                    {
                        parts.Add(new PartType() {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1)
                        });
                    }
                }
            }
        }

        private void ReadInventory()
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"SELECT
                    PartTypeId, Count, OrderThreshold
                    FROM InventoryItem";
                using (var reader = command.ExecuteReader())
                {
                    var items = new List<InventoryItem>();
                    Inventory = items;
                    while (reader.Read())
                    {
                        items.Add(new InventoryItem() {
                            PartTypeId = reader.GetInt32(0),
                            Count = reader.GetInt32(1),
                            OrderThreshold = reader.GetInt32(2)
                        });
                    }
                }
            }
        }
    }
}

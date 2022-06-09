using System;
using System.Collections.Generic;
using System.Data.Common;

namespace WidgetScmDataAccess
{
    public class ScmContext
    {
        private DbConnection connection;

        public IEnumerable<PartType> Parts { get; private set; }

        public ScmContext(DbConnection conn)
        {
            connection = conn;
            ReadParts();
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
    }
}

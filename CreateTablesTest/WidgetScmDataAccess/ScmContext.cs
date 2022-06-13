﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace WidgetScmDataAccess
{
    public class ScmContext
    {
        private DbConnection connection;

        public IEnumerable<PartType> Parts { get; private set; }

        public IEnumerable<InventoryItem> Inventory { get; private set; }

        public IEnumerable<Supplier> Suppliers { get; private set; }

        public ScmContext(DbConnection conn)
        {
            connection = conn;
            ReadParts();
            ReadInventory();
            ReadSuppliers();
        }

        public DbTransaction BeginTransaction()
        {
            return connection.BeginTransaction();
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
                var reader = command.ExecuteReader();
                var items = new List<InventoryItem>();
                Inventory = items;
                while (reader.Read())
                {
                    var item = new InventoryItem() {
                        PartTypeId = reader.GetInt32(0),
                        Count = reader.GetInt32(1),
                        OrderThreshold = reader.GetInt32(2)
                    };
                    items.Add(item);
                    item.Part = Parts.Single(p => p.Id == item.PartTypeId);
                }
            }
        }

        public void ReadSuppliers()
        {
            var command = connection.CreateCommand();
            command.CommandText = @"SELECT 
                Id, Name, Email, PartTypeId 
                FROM Supplier";
            var reader = command.ExecuteReader();
            var suppliers = new List<Supplier>();
            Suppliers = suppliers;
            while (reader.Read())
            {
                var supplier = new Supplier() {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Email = reader.GetString(2),
                PartTypeId = reader.GetInt32(3)
                };
                suppliers.Add(supplier);
                supplier.Part = Parts.Single(p => p.Id == supplier.PartTypeId);
            }              
        }

        public void CreatePartCommand(PartCommand partCommand)
        {
            var command = connection.CreateCommand();
            command.CommandText = @"INSERT INTO PartCommand
                (PartTypeId, Count, Command)
                VALUES
                (@partTypeId, @partCount, @command);
                SELECT last_insert_rowid();";

            AddParameter(command, "@partTypeId", partCommand.PartTypeId);
            AddParameter(command, "@partCount", partCommand.PartCount);
            AddParameter(command, "@command", partCommand.Command.ToString());

            long partCommandId = (long)command.ExecuteScalar();
            partCommand.Id = (int)partCommandId;
        }

        public void AddParameter(DbCommand cmd, string name, object value)
        {
            var p = cmd.CreateParameter();
            if (value == null)
                throw new ArgumentNullException("value");
            Type t = value.GetType();
            if (t == typeof(int))
                p.DbType = DbType.Int32;
            else if (t == typeof(string))
                p.DbType = DbType.String;
            else if (t == typeof(DateTime))
                p.DbType = DbType.DateTime;
            else
                throw new ArgumentException($"Unrecognized type: {t.ToString()}", "value");
            p.Direction = ParameterDirection.Input;
            p.ParameterName = name;
            p.Value = value;
            cmd.Parameters.Add(p);
        }

        public IEnumerable<PartCommand> GetPartCommands()
        {
            var command = connection.CreateCommand();
            command.CommandText = @"SELECT 
                Id, PartTypeId, Count, Command
                FROM PartCommand
                ORDER BY Id";
            var reader = command.ExecuteReader();
            var partCommands = new List<PartCommand>();
            while(reader.Read())
            {
                var cmd = new PartCommand() {
                    Id = reader.GetInt32(0),
                    PartTypeId = reader.GetInt32(1),
                    PartCount = reader.GetInt32(2),
                    // Command = (PartCountOperation)Enum.Parse(
                    //     typeof(PartCountOperation),
                    //     reader.GetString(3)
                    // );
                };
                cmd.Part = Parts.Single(p => p.Id == cmd.PartTypeId);

                PartCountOperation operation;
                if (Enum.TryParse<PartCountOperation>(reader.GetString(3), out operation))
                    cmd.Command = operation;
                partCommands.Add(cmd);
            }
            return partCommands;
        }

        public void UpdateInventoryItem(int partTypeId, int count, DbTransaction transaction)
        {
            var command = connection.CreateCommand();
            if (transaction != null)
                command.Transaction = transaction;
            command.CommandText = @"UPDATE InventoryItem
                SET Count=@count
                WHERE PartTypeId=@partTypeId";
            AddParameter(command, "@count", count);
            AddParameter(command, "@partTypeId", partTypeId);
            command.ExecuteNonQuery();
        }

        public void DeletePartCommand(int id, DbTransaction transaction)
        {
            var command = connection.CreateCommand();
            if (transaction != null)
                command.Transaction = transaction;
            command.CommandText = @"DELETE FROM PartCommand
                WHERE Id=@id";
            AddParameter(command, "@id", id);
            command.ExecuteNonQuery();
        }

        public void CreateOrder(Order order)
        {
            var transaction = connection.BeginTransaction();
            try {
                var command = connection.CreateCommand();
                command.Transaction = transaction;
                command.CommandText = @"INSERT INTO [Order]
                    (SupplierId, PartTypeId, PartCount, PlacedDate)
                    VALUES
                    (@supplierId, @partTypeId, @partCount, @placedDate);
                    SELECT last_insert_rowid()";
                
                AddParameter(command, "@supplierId", order.SupplierId);
                AddParameter(command, "@partTypeId", order.PartTypeId);
                AddParameter(command, "@partCount", order.PartCount);
                AddParameter(command, "@placedDate", order.PlacedDate);
                long orderId = (long)command.ExecuteScalar();
                order.Id = (int)orderId;

                command = connection.CreateCommand();
                command.Transaction = transaction;
                command.CommandText = @"INSERT INTO SendEmailCommand
                    ([To], Subject, Body) VALUES
                    (@To, @Subject, @Body)";
                AddParameter(command, "@To", order.Supplier.Email);
                AddParameter(command, "@Subject", $"Order #{orderId} for {order.Part.Name}");
                AddParameter(command, "@Body", $"Please send {order.PartCount}" + $" items of {order.Part.Name} to Widget Corp");
                command.ExecuteNonQuery();
                transaction.Commit();
            }
            catch {
                transaction.Rollback();
                throw;
            }
        }
    }
}

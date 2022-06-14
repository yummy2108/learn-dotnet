using System;
using System.Linq;
using System.Reflection;

enum DatabaseType {
    Sqlite,
    SqlServer,
    PostgerSql
}

static class DataAccessFactory
{
    internal static string ConnectionString { get; set; }
    internal static IScmContext GetScmContext(DatabaseType dbType)
    {
        switch (dbType)
        {
            case DatabaseType.Sqlite:
                return new SqliteScmContext(ConnectionString);
            case DatabaseType.SqlServer:
                return new SqlServerScmContext(ConnectionString);
            case DatabaseType.PostgerSql:
                return new PostgreSqlScmContext(ConnectionString);
            default:
                throw new ArgumentException($"Unrecognized Database type {dbType}", "dbType");
        }
    }
}
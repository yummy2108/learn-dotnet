using System;
using System.Linq;
using System.Reflection;


static class DataAccessFactory
{
    internal static Type scmContextType = null;
    internal static Type ScmContextType{
        get { return scmContextType; }
        set {
            if (!value.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IScmContext))){
                throw new ArgumentException(
                    $"{value.GetTypeInfo().FullName} doesn't implement IScmContext"
                );
            }
            scmContextType = value;
        }
    }
    internal static IScmContext GetScmContext()
    {
        // switch (dbType)
        // {
        //     case DatabaseType.Sqlite:
        //         return new SqliteScmContext(ConnectionString);
        //     case DatabaseType.SqlServer:
        //         return new SqlServerScmContext(ConnectionString);
        //     case DatabaseType.PostgerSql:
        //         return new PostgreSqlScmContext(ConnectionString);
        //     default:
        //         throw new ArgumentException($"Unrecognized Database type {dbType}", "dbType");
        // }

        if (scmContextType == null) {
            throw new ArgumentNullException("ScmContextType not set");
        }
        return Activator.CreateInstance(scmContextType) as IScmContext;
    }
}
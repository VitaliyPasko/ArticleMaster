using System.Data;
using Microsoft.Data.SqlClient;

namespace ArticleMaster.Application.Interfaces;

public abstract class MsSqlDbConnectionProvider : DbConnection
{
    public MsSqlDbConnectionProvider(string connectionString) : base(connectionString)
    {
    }

    protected override IDbConnection OpenConnection()
    {
        var conn = new SqlConnection(ConnectionString);
        conn.Open();
        return conn;
    }
}
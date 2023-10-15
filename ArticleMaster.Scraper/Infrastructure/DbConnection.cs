using System.Data;
using Microsoft.Data.SqlClient;

namespace ArticleMaster.Scraper.Infrastructure;

public class DbConnection
{
    protected readonly string ConnectionString;

    protected DbConnection(string connectionString)
    {
        ConnectionString = connectionString;
    }

    protected virtual IDbConnection OpenConnection()
    {
        var conn = new SqlConnection(ConnectionString);
        conn.Open();
        return conn;
    }
}
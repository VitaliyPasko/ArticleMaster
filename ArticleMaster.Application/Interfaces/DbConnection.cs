using System.Data;

namespace ArticleMaster.Application.Interfaces;

public abstract class DbConnection
{
    protected readonly string ConnectionString;

    protected DbConnection(string connectionString)
    {
        ConnectionString = connectionString;
    }

    protected abstract IDbConnection OpenConnection();
}
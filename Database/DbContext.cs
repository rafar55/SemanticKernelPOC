using System.Data;
using Dapper;
using Npgsql;
using Pgvector.Dapper;

namespace SemanticKernel.Database;

public class DbContext(DbConfig dbConfig)
{
    private readonly DbConfig _dbConfig = dbConfig;

    public async Task Initialize()
    {
        await CreateDatabase();
        await ConfigureVectors();
    }
    
    private IDbConnection CreateConnection()
    {
        var dateSourceBuilder = new NpgsqlDataSourceBuilder(_dbConfig.ConnectionString);
        dateSourceBuilder.UseVector();
        var dataSource = dateSourceBuilder.Build();
        return dataSource.CreateConnection();
    }
    
    private async Task CreateDatabase()
    {
        using var connection = CreateConnection();
        var sql = $"""
                   DO $$
                   BEGIN
                       IF NOT EXISTS (SELECT 1 FROM pg_database WHERE datname = '{_dbConfig.DatabaseName}') THEN
                           EXECUTE 'CREATE DATABASE {_dbConfig.DatabaseName}';
                       END IF;
                   END
                   $$;
                   """;
        await connection.ExecuteAsync(sql);
    }

    private async Task ConfigureVectors()
    {
        await using var cnn = (NpgsqlConnection) CreateConnection();
        await cnn.ExecuteAsync("CREATE EXTENSION IF NOT EXISTS vector;");
        await cnn.ReloadTypesAsync();
    }
}
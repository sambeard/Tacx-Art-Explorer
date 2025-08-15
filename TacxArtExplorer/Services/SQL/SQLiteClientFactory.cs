using Dapper;
using Microsoft.Data.Sqlite;
using System;
using System.IO;
using System.Threading.Tasks;
using TacxArtExplorer.Services.SQL;

namespace TacxArtExplorer.Services.SQLClients
{
    public interface ISqliteConnectionFactory
    {
        SqliteConnection Create();
    }


    public sealed class SqliteConnectionFactory : ISqliteConnectionFactory
    {
        private readonly string _connectionString;

        public SqliteConnectionFactory(string databasePath)
        {
            var fullPath = Path.GetFullPath(databasePath);
            if (string.IsNullOrWhiteSpace(fullPath))
                throw new ArgumentException("Database path cannot be null or empty.", nameof(databasePath));
            if (!Path.Exists(fullPath))
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

            _connectionString = new SqliteConnectionStringBuilder
            {
                DataSource = fullPath,
                Mode = SqliteOpenMode.ReadWriteCreate,
                Cache = SqliteCacheMode.Shared
            }.ToString();

            // Register custom type handlers for Dapper

            SqlMapper.AddTypeHandler(new SizeOptionHandler());
            SqlMapper.AddTypeHandler(new ImageFormatHandler());

        }

        public SqliteConnection Create() => new SqliteConnection(_connectionString);
    
    }

}

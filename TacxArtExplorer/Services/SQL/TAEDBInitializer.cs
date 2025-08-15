using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TacxArtExplorer.Models;
using TacxArtExplorer.Services.SQLClients;

namespace TacxArtExplorer.Services.SQL
{
    public interface IDbInitializer
    {
        Task InitializeAsync(IEnumerable<Artist>? seedArtists = default);
    }

    public sealed class SqliteDbInitializer : IDbInitializer
    {
        private readonly ISqliteConnectionFactory _factory;
        private const string _createCommand = @"
PRAGMA journal_mode=WAL;
PRAGMA foreign_keys=ON;
-- Artists
CREATE TABLE IF NOT EXISTS Artists (
  Id   INTEGER PRIMARY KEY,
  Name TEXT NOT NULL UNIQUE
);

-- ArtPieces (Artist is embedded in domain, normalized here by ArtistId)
CREATE TABLE IF NOT EXISTS ArtPieces (
  Id              INTEGER PRIMARY KEY,
  ArtistId        INTEGER NOT NULL REFERENCES Artists(Id) ON DELETE CASCADE,
  Title           TEXT NOT NULL,
  LongDescription TEXT NULL,
  ShortDescription TEXT NULL,
  ImageID         TEXT NOT NULL,
  DisplayDate     TEXT NULL,
  ArtworkType     TEXT NULL,
  PlaceOfOrigin   TEXT NULL,
  ThumbLQIP       TEXT NOT NULL,
  ThumbAlt        TEXT NOT NULL,
  ThumbWidth      INTEGER NOT NULL,
  ThumbHeight     INTEGER NOT NULL
);

-- ArtPieceImages (unique per Id + Size + Format)
CREATE TABLE IF NOT EXISTS ArtPieceImages (
  Id        TEXT NOT NULL,
  Format    TEXT NOT NULL,            -- 'jpg' | 'gif' | 'png'
  SizeValue INTEGER NOT NULL,         -- -1=full, else pixel width
  Data      BLOB NOT NULL,
  PRIMARY KEY (Id, SizeValue, Format)
);

";


        public SqliteDbInitializer(ISqliteConnectionFactory factory) => _factory = factory;

        public async Task InitializeAsync(IEnumerable<Artist>? seedArtists = default)
        {
            using var conn = _factory.Create();
            await conn.OpenAsync();
            await InitializeAsync(conn, seedArtists);
        }

        public static async Task InitializeAsync(SqliteConnection conn, IEnumerable<Artist>? seedArtists = default)
        {
            using var cmd = conn.CreateCommand();

            cmd.CommandText = _createCommand;
            if (seedArtists != null && seedArtists.Any())
            {
                cmd.CommandText += seedCommand(seedArtists!);
            }
            await cmd.ExecuteNonQueryAsync();
        }

        private static string seedCommand(IEnumerable<Artist> seedArtists) {
            var entries= string.Join(",\n ",seedArtists.Select(seedArtists => $"SELECT {seedArtists.Id}, '{seedArtists.Name}'")); 
            var ids = string.Join(", ", seedArtists.Select(a => a.Id));
            return "INSERT INTO Artists(Id, Name)" + entries + $"\nWHERE NOT EXISTS(SELECT 1 FROM Artists WHERE Id IN ({ids})";
        }
    }
}

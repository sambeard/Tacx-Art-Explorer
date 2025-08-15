using Dapper;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TacxArtExplorer.Models;
using TacxArtExplorer.Services.SQL;

namespace TacxArtExplorer.Services
{
    public interface IArtCacheService : IArtService
    {

        Task<int> InsertOrUpdateArtistAsync(Artist artist);
        Task<int> InsertOrUpdateArtPiecesAsync(IEnumerable<ArtPiece> artPieces);
        Task<int> InsertOrUpdateImageAsync(ArtPieceImage image);

        Task<Artist?> GetArtistByIdAsync(int artistId);
        Task<IEnumerable<Artist>?> GetArtistsAsync();
        // is here for documenation purposes only, same as underlying API
        new Task<IEnumerable<ArtPiece>?> GetArtPiecesByArtistAsync(Artist artist);
        Task<IEnumerable<ArtPiece>?> GetArtPieceByIdAsync(int id);
        Task<IEnumerable<ArtPieceImage>> GetImagesByIdAsync(string imageId);
        Task<ArtPieceImage?> GetImageByIdAsync(string imageId, SizeOption size = default);

        Task<int> RemoveArtPiecesForArtistAsync(Artist artist);
        Task<int> RemoveImageAsync(ArtPieceImage image);
        Task<int> RemoveImagesByIdAsync(string image_id);
    }

    public class CacheService : IArtCacheService
    {
        private readonly SqliteConnection _db;
        public CacheService(SqliteConnection dbConnection) => _db = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));

        private async Task EnsureOpenAsync()
        {
            if (_db.State != ConnectionState.Open)
                await _db.OpenAsync();
        }

        #region SQL: Artists
        private const string UpsertArtistSql = @"
INSERT INTO Artists (Id, Name) VALUES (@Id, @Name)
ON CONFLICT(Id) DO UPDATE SET Name = excluded.Name;";

        private const string SelectArtistsSql = @"SELECT Id, Name FROM Artists;";

        private const string SelectArtistByIdSql = @"
SELECT Id, Name FROM Artists WHERE Id = @Id;";
        #endregion

        #region SQL: ArtPieces
        private const string UpsertArtPieceSql = @"
INSERT INTO ArtPieces
(Id, ArtistId, Title, LongDescription, ShortDescription, ImageID, DisplayDate, ArtworkType, PlaceOfOrigin,
 ThumbLQIP, ThumbAlt, ThumbWidth, ThumbHeight)
VALUES
(@Id, @ArtistId, @Title, @LongDescription, @ShortDescription, @ImageID, @DisplayDate, @ArtworkType, @PlaceOfOrigin,
 @ThumbLQIP, @ThumbAlt, @ThumbWidth, @ThumbHeight)
ON CONFLICT(Id) DO UPDATE SET
  ArtistId        = excluded.ArtistId,
  Title           = excluded.Title,
  LongDescription = excluded.LongDescription,
  ShortDescription= excluded.ShortDescription,
  ImageID         = excluded.ImageID,
  DisplayDate     = excluded.DisplayDate,
  ArtworkType     = excluded.ArtworkType,
  PlaceOfOrigin   = excluded.PlaceOfOrigin,
  ThumbLQIP       = excluded.ThumbLQIP,
  ThumbAlt        = excluded.ThumbAlt,
  ThumbWidth      = excluded.ThumbWidth,
  ThumbHeight     = excluded.ThumbHeight;";

        private const string SelectPiecesByArtistSql = @"
SELECT 
  ap.Id, ap.Title,  ap.LongDescription, ap.ShortDescription, ap.ImageID,
  ap.DisplayDate, ap.ArtworkType, ap.PlaceOfOrigin,
  ap.ThumbLQIP, ap.ThumbAlt, ap.ThumbWidth, ap.ThumbHeight,
  a.Id as ArtistId, a.Name as ArtistName
FROM ArtPieces ap
JOIN Artists a ON a.Id = ap.ArtistId
WHERE ap.ArtistId = @ArtistId
ORDER BY ap.DisplayDate;";

        private const string SelectPieceByIdSql = @"
SELECT 
  ap.Id, ap.Title, ap.LongDescription, ap.ShortDescription, ap.ImageID,
  ap.DisplayDate, ap.ArtworkType, ap.PlaceOfOrigin,
  ap.ThumbLQIP, ap.ThumbAlt, ap.ThumbWidth, ap.ThumbHeight,
  a.Id as ArtistId, a.Name as ArtistName
FROM ArtPieces ap
JOIN Artists a ON a.Id = ap.ArtistId
WHERE ap.Id = @Id;";

        private const string DeletePiecesForArtistSql = @"
DELETE FROM ArtPieces WHERE ArtistId = @ArtistId;";
        #endregion

        #region SQL: Images
        private const string UpsertImageSql = @"
INSERT INTO ArtPieceImages (Id, Format, SizeValue, Data)
VALUES (@Id, @Format, @SizeOption, @Data)
ON CONFLICT(Id, SizeValue, Format) DO UPDATE SET Data = excluded.Data;";

        private const string SelectImagesByIdSql = @"
SELECT Id, Format, SizeValue AS SizeOption, Data
FROM ArtPieceImages
WHERE Id = @Id
ORDER BY SizeValue;";

        private const string SelectImageByIdAndSizeSql = @"
SELECT Id, Format, SizeValue AS SizeOption, Data
FROM ArtPieceImages
WHERE Id = @Id AND SizeValue = @SizeOption
LIMIT 1;";

        private const string SelectImageByIdSizeFormatSql = @"
SELECT Id, Format, SizeValue AS SizeOption, Data
FROM ArtPieceImages
WHERE Id = @Id AND SizeValue = @SizeOption AND Format = @Format
LIMIT 1;";

        private const string SelectAnyImageForIdSql = @"
SELECT Id, Format, SizeValue AS SizeOption, Data
FROM ArtPieceImages
WHERE Id = @Id
ORDER BY SizeValue
LIMIT 1;";

        private const string SelectAnyImageForIdByFormatSql = @"
SELECT Id, Format, SizeValue AS SizeOption, Data
FROM ArtPieceImages
WHERE Id = @Id AND Format = @Format
ORDER BY SizeValue
LIMIT 1;";

        private const string DeleteImageSql = @"
DELETE FROM ArtPieceImages
WHERE Id = @Id AND SizeValue = @SizeOption AND Format = @Format;";

        private const string DeleteImagesByIdSql = @"
DELETE FROM ArtPieceImages
WHERE Id = @Id;";
        #endregion

        #region Artists
        public async Task<int> InsertOrUpdateArtistAsync(Artist artist)
        {
            if (artist is null) throw new ArgumentNullException(nameof(artist));
            await EnsureOpenAsync();
            return await _db.ExecuteAsync(UpsertArtistSql, new { artist.Id, artist.Name });
        }

        public async Task<IEnumerable<Artist>?> GetArtistsAsync()
        {
            await EnsureOpenAsync();
            return await _db.QueryAsync<Artist>(SelectArtistsSql);
        }

        public async Task<Artist?> GetArtistByIdAsync(int artistId)
        {
            await EnsureOpenAsync();
            return await _db.QueryFirstOrDefaultAsync<Artist>(SelectArtistByIdSql, new { Id = artistId });
        }
        #endregion

        #region ArtPieces
        public async Task<int> InsertOrUpdateArtPiecesAsync(IEnumerable<ArtPiece> pieces)
        {
            if (pieces is null) throw new ArgumentNullException(nameof(pieces));
            await EnsureOpenAsync();

            using var tx = _db.BeginTransaction();
            var total = 0;

            foreach (var p in pieces)
            {
                // 1) Ensure artist row exists/updated
                await _db.ExecuteAsync(UpsertArtistSql, new { Id = p.Artist.Id, p.Artist.Name }, tx);

                // 2) Upsert piece
                total += await _db.ExecuteAsync(UpsertArtPieceSql, new
                {
                    p.Id,
                    ArtistId = p.Artist.Id,
                    p.Title,
                    p.LongDescription,
                    p.ShortDescription,
                    p.ImageID,
                    p.DisplayDate,
                    p.ArtworkType,
                    p.PlaceOfOrigin,
                    ThumbLQIP = p.Thumbnail.LQIP,
                    ThumbAlt = p.Thumbnail.AltText,
                    ThumbWidth = p.Thumbnail.Dimensions.Item1,
                    ThumbHeight = p.Thumbnail.Dimensions.Item2
                }, tx);
            }

            tx.Commit();
            return total; // affected rows across inserts/updates
        }

        public async Task<IEnumerable<ArtPiece>?> GetArtPiecesByArtistAsync(Artist artist)
        {
            // simulate small delay
            await Task.Delay(300);
            if (artist is null) throw new ArgumentNullException(nameof(artist));
            await EnsureOpenAsync();

            var rows = await _db.QueryAsync(SelectPiecesByArtistSql, new { ArtistId = artist.Id });
            return rows.Select(r => new ArtPiece
            {
                Id = (int)r.Id,
                Title = (string)r.Title,
                Artist = new Artist((int)r.ArtistId, (string)r.ArtistName),
                LongDescription = (string?)r.LongDescription,
                ShortDescription = (string?)r.ShortDescription,
                ImageID = (string)r.ImageID,
                DisplayDate = (string?)r.DisplayDate,
                ArtworkType = (string?)r.ArtworkType,
                PlaceOfOrigin = (string?)r.PlaceOfOrigin,
                Thumbnail = new Thumbnail(
                    (string)r.ThumbLQIP,
                    (string)r.ThumbAlt,
                    ((int)r.ThumbWidth, (int)r.ThumbHeight))
            }).ToList();
        }

        public async Task<IEnumerable<ArtPiece>?> GetArtPieceByIdAsync(int id)
        {
            await EnsureOpenAsync();

            var rows = await _db.QueryAsync(SelectPieceByIdSql, new { Id = id });
            return rows.Select(r => new ArtPiece
            {
                Id = (int)r.Id,
                Title = (string)r.Title,
                Artist = new Artist((int)r.ArtistId, (string)r.ArtistName),
                LongDescription = (string?)r.LongDescription,
                ShortDescription = (string?)r.ShortDescription,
                ImageID = (string)r.ImageID,
                DisplayDate = (string?)r.DisplayDate,
                ArtworkType = (string?)r.ArtworkType,
                PlaceOfOrigin = (string?)r.PlaceOfOrigin,
                Thumbnail = new Thumbnail(
                    (string)r.ThumbLQIP,
                    (string)r.ThumbAlt,
                    ((int)r.ThumbWidth, (int)r.ThumbHeight))
            }).ToList();
        }

        public async Task<int> RemoveArtPiecesForArtistAsync(Artist artist)
        {
            if (artist is null) throw new ArgumentNullException(nameof(artist));
            await EnsureOpenAsync();
            return await _db.ExecuteAsync(DeletePiecesForArtistSql, new { ArtistId = artist.Id });
        }
        #endregion

        #region Images
        public async Task<int> InsertOrUpdateImageAsync(ArtPieceImage image)
        {
            if (image is null) throw new ArgumentNullException(nameof(image));
            await EnsureOpenAsync();
            // SizeOption + ImageFormat converted by type handlers
            return await _db.ExecuteAsync(UpsertImageSql, image);
        }

        public async Task<IEnumerable<ArtPieceImage>> GetImagesByIdAsync(string imageId)
        {
            // simulate small delay
            await Task.Delay(200);
            if (string.IsNullOrWhiteSpace(imageId)) throw new ArgumentException("Required", nameof(imageId));
            await EnsureOpenAsync();
            var items = await _db.QueryAsync<ArtPieceImage>(SelectImagesByIdSql, new { Id = imageId });
            return items.ToList();
        }

        public async Task<ArtPieceImage?> GetImageByIdAsync(string imageId, SizeOption size = default)
        {
            // simulate small delay
            await Task.Delay(200);
            if (string.IsNullOrWhiteSpace(imageId)) throw new ArgumentException("Required", nameof(imageId));
            await EnsureOpenAsync();

            if (size.Equals(default(SizeOption)))
                return await _db.QueryFirstOrDefaultAsync<ArtPieceImage>(SelectAnyImageForIdSql, new { Id = imageId });

            return await _db.QueryFirstOrDefaultAsync<ArtPieceImage>(SelectImageByIdAndSizeSql, new { Id = imageId, SizeOption = size });
        }

        public async Task<ArtPieceImage?> GetImageByIdAsync(string imageId, SizeOption? size = null, ImageFormat format = ImageFormat.JPG)
        {
            if (string.IsNullOrWhiteSpace(imageId)) throw new ArgumentException("Required", nameof(imageId));
            await EnsureOpenAsync();

            if (size is null)
                return await _db.QueryFirstOrDefaultAsync<ArtPieceImage>(SelectAnyImageForIdByFormatSql, new { Id = imageId, Format = format });

            return await _db.QueryFirstOrDefaultAsync<ArtPieceImage>(SelectImageByIdSizeFormatSql, new { Id = imageId, SizeOption = size.Value, Format = format });
        }

        public async Task<int> RemoveImageAsync(ArtPieceImage image)
        {
            if (image is null) throw new ArgumentNullException(nameof(image));
            await EnsureOpenAsync();
            return await _db.ExecuteAsync(DeleteImageSql, image);
        }

        public async Task<int> RemoveImagesByIdAsync(string image_id)
        {
            if (string.IsNullOrWhiteSpace(image_id)) throw new ArgumentException("Required", nameof(image_id));
            await EnsureOpenAsync();
            return await _db.ExecuteAsync(DeleteImagesByIdSql, new { Id = image_id });
        }
        #endregion
    }


}

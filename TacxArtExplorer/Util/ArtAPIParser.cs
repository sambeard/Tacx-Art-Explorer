//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Text.Json;
//using TacxArtExplorer.Models;

//namespace TacxArtExplorer.Util
//{
//    internal static class ArtAPIParser
//    {
//        readonly static JsonSerializerOptions opt = new()
//        {
//            PropertyNameCaseInsensitive = true,
//        };

//        /// <summary>
//        /// Parses a JSON string containing an array of art pieces into a list of ArtPiece objects.
//        /// </summary>
//        /// <param name="json">The json string containing the list of artpieces</param>
//        /// <returns></returns>
//        public static IEnumerable<ArtPiece>? ParseArtPieces(string json)
//        {
//            if (string.IsNullOrWhiteSpace(json))
//                return null;

//            var artPieces = new List<ArtPiece>();
//            using var doc = JsonDocument.Parse(json);

//            try
//            {
//                foreach (var element in doc.RootElement.GetProperty("data").EnumerateArray())
//                {
//                    try
//                    {
//                        int artist_id, id;
//                        string artist_title, title, description, imageUrl;
//                        JsonElement thumbnailElement;

//                        // Artist details
//                        element.TryGetProperty("artist_id", out artist_id);

//                        int artist_id = element.GetProperty("artist_id").GetInt32();
//                        string artist_title = element.GetProperty("artist_title").GetString() ?? "";

//                        // Art piece details
//                        int id = element.GetProperty("id").GetInt32();
//                        string title = element.GetProperty("title").GetString() ?? "";
//                        DateTime creationDate = element.GetProperty("creationDate").GetDateTime();
//                        string description = element.GetProperty("description").GetString() ?? "";
//                        string imageUrl = element.GetProperty("imageUrl").GetString() ?? "";

//                        // Thumbnail details
//                        var thumbnailElement = element.GetProperty("thumbnail");

//                        // Create and add objects
//                        var thumbnail = ParseThumbnail(thumbnailElement.GetRawText());
//                        var artist = new Artist(artist_id, artist_title);
//                        artPieces.Add(new ArtPiece(id, title, artist, creationDate, description, imageUrl, thumbnail!));
//                    }
//                    catch {
//                        continue;
//                    }
//                }
//            }
//            catch
//            {
//                artPieces = null;
//            }
//            finally
//            {
//                doc.Dispose();
//            }
//            return artPieces;
//        }

//        public static Thumbnail? ParseThumbnail(string json)
//        {
//            // Parse the JSON string into a JsonDocument using default extractor
//            if (string.IsNullOrWhiteSpace(json))
//                return null;

//            return JsonSerializer.Deserialize<Thumbnail>(json, opt);
//        }
//    }
//}

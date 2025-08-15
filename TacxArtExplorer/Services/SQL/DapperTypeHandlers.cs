using System;
using Dapper;
using System.Data;
using TacxArtExplorer.Models;

namespace TacxArtExplorer.Services.SQL
{
    public sealed class SizeOptionHandler : SqlMapper.TypeHandler<SizeOption>
    {
        public override void SetValue(IDbDataParameter parameter, SizeOption value)
        {
            // Persist as int: -1 for "full", else the numeric width
            // SizeOption.Value => "200," or "full"; we need the raw number
            // Use ToString() to check "full" cheaply
            var s = value.ToString();
            parameter.Value = s == "full" ? -1 : int.Parse(s.TrimEnd(','));
            parameter.DbType = DbType.Int32;
        }

        public override SizeOption Parse(object value)
        {
            var i = Convert.ToInt32(value);
            return i < 0 ? SizeOption.Full : new SizeOption(i);
        }
    }

    public sealed class ImageFormatHandler : SqlMapper.TypeHandler<ImageFormat>
    {
        public override void SetValue(IDbDataParameter parameter, ImageFormat value)
        {
            parameter.Value = value.ToFormatString(); // "jpg" | "gif" | "png"
            parameter.DbType = DbType.String;
        }

        public override ImageFormat Parse(object value)
        {
            return value?.ToString()?.ToLowerInvariant() switch
            {
                "jpg" => ImageFormat.JPG,
                "gif" => ImageFormat.GIF,
                "png" => ImageFormat.PNG,
                _ => throw new ArgumentOutOfRangeException(nameof(value), $"Unknown format: {value}")
            };
        }
    }

}

using System;
using MimeKit;

namespace TEDx.Infrastructure.Media;

internal static class ImageContentTypeSniffer
{
    // WebP needs the longest prefix: "RIFF" + 4 size bytes + "WEBP".
    private const int HeaderLength = 12;

    private static readonly byte[] Jpeg = [0xFF, 0xD8, 0xFF];
    private static readonly byte[] Png = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];
    private static readonly byte[] Gif87a = "GIF87a"u8.ToArray();
    private static readonly byte[] Gif89a = "GIF89a"u8.ToArray();
    private static readonly byte[] Riff = "RIFF"u8.ToArray();
    private static readonly byte[] Webp = "WEBP"u8.ToArray();

    public static async Task<string?> DetectAsync(
        Stream stream,
        CancellationToken cancellationToken = default)
    {
        if (!stream.CanRead || !stream.CanSeek)
            return null;

        var origin = stream.Position;

        try
        {
            // Creates a 12-byte buffer (HeaderLength) to store the first bytes read from the stream.
            // We only need the beginning of the file because image formats have known signatures
            // (magic numbers) in their leading bytes that allow us to detect the actual content type.
            var header = new byte[HeaderLength]; // [ ][ ][ ][ ][ ][ ][ ][ ][ ][ ][ ][ ]

            // Move the stream position to the beginning so we can read the file signature from its first byte.
            stream.Position = 0;

            // Read up to the first 12 bytes from the stream into the buffer.
            // 'read' tells us how many bytes were actually read (the file may contain fewer than 12 bytes).
            var read = await stream.ReadAtLeastAsync(
                header,                     // Stores the bytes read from the stream.
                HeaderLength,               // Tries to read at least 12 bytes, which is enough to detect the supported image signatures.
                throwOnEndOfStream: false,  // If the file is smaller than 12 bytes, don't throw; just return how many bytes were actually read.
                cancellationToken);         // Stops the read operation if the request is cancelled.

            // Only pass the bytes that were actually read to the detector,
            // allowing it to compare them against the known image signatures.
            return Detect(header.AsSpan(0, read));

        }
        finally
        {
            stream.Position = origin;
        }
    }

    private static string? Detect(ReadOnlySpan<byte> header)
    {
        if (header.StartsWith(Jpeg))
            return "image/jpeg";

        if (header.StartsWith(Png))
            return "image/png";

        if (header.StartsWith(Gif87a) || header.StartsWith(Gif89a))
            return "image/gif";

        // Both markers are required; "RIFF" alone is also WAV, AVI, and others.
        if (header.Length >= HeaderLength
            && header.StartsWith(Riff)
            && header[8..HeaderLength].SequenceEqual(Webp))
        {
            return "image/webp";
        }

        return null;
    }
}

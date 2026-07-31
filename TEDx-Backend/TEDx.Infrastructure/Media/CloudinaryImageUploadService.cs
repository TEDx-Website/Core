using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using TEDx.Application.Common.Interfaces;
using TEDx.Infrastructure.Configuration;

namespace TEDx.Infrastructure.Media;

internal sealed class CloudinaryImageUploadService : IImageUploadService
{
    private readonly Cloudinary _cloudinary;
    private readonly CloudinaryOptions _options;

    public CloudinaryImageUploadService(IOptions<CloudinaryOptions> options)
    {
        _options = options.Value;

        var account = new Account(
            _options.CloudName,
            _options.ApiKey,
            _options.ApiSecret);

        _cloudinary = new Cloudinary(account) { Api = { Secure = true } };
    }
    public async Task<string> UploadAsync(
        Stream stream,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        ValidateFile(stream, fileName);

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(fileName, stream),
            Folder = _options.UploadFolder,
            UniqueFilename = true,
            UseFilename = false,
        };

        var result = await _cloudinary.UploadAsync(uploadParams, cancellationToken);

        if (result.Error is not null)
            throw new InvalidOperationException(
                $"Cloudinary upload failed: {result.Error.Message}");

        return result.SecureUrl.AbsoluteUri;
    }

    private void ValidateFile(Stream stream, string fileName)
    {
        // Size guard
        if (stream.Length > _options.MaxFileSizeBytes)
            throw new InvalidOperationException(
                $"File exceeds the maximum allowed size of {_options.MaxFileSizeBytes / (1024 * 1024)} MB.");

        // MIME type guard (derived from extension — full content sniffing deferred to USER-03)
        var mimeType = ResolveMimeType(fileName);
        if (!_options.AllowedMimeTypes.Contains(mimeType, StringComparer.OrdinalIgnoreCase))
            throw new InvalidOperationException(
                $"File type '{mimeType}' is not allowed. Allowed types: {string.Join(", ", _options.AllowedMimeTypes)}.");
    }

    private static string ResolveMimeType(string fileName)
    {
        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        return ext switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png"            => "image/png",
            ".webp"           => "image/webp",
            _                 => $"application/octet-stream"
        };
    }
}

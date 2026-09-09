using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TEDx.Application.Common.Interfaces;
using TEDx.Infrastructure.Options;
using DomainResult = TEDx.Domain.Common.Result<string>;
using Errors = TEDx.Application.Common.Errors.MediaErrors;

namespace TEDx.Infrastructure.Media;

internal sealed class CloudinaryImageUploadService : IImageUploadService
{
    private readonly Cloudinary _cloudinary;
    private readonly CloudinaryOptions _options;
    private readonly ILogger<CloudinaryImageUploadService> _logger;

    public CloudinaryImageUploadService(
        Cloudinary cloudinary,
        IOptions<CloudinaryOptions> options,
        ILogger<CloudinaryImageUploadService> logger)
    {
        _cloudinary = cloudinary;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<DomainResult> UploadAsync(
        Stream stream,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        var rejection = await ValidateAsync(stream, cancellationToken);
        if (rejection is not null)
            return DomainResult.Failure(rejection.Value);

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(fileName, stream),
            Folder = _options.UploadFolder,
            UniqueFilename = true,
            UseFilename = false,
        };

        try
        {
            var result = await _cloudinary.UploadAsync(uploadParams, cancellationToken);

            if (result.Error is not null)
            {
                _logger.LogError(
                    "Cloudinary rejected an upload: {UpstreamMessage}",
                    result.Error.Message);

                return DomainResult.Failure(Errors.UploadFailed);
            }

            if (result.SecureUrl is null)
            {
                _logger.LogError(
                    "Cloudinary reported success but returned no secure URL (status {Status}).",
                    result.StatusCode);

                return DomainResult.Failure(Errors.UploadFailed);
            }

            return DomainResult.Success(result.SecureUrl.AbsoluteUri);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {

            return DomainResult.Failure(Errors.UploadFailed);
        }
    }

    private async Task<Domain.Common.Error?> ValidateAsync(
        Stream stream,
        CancellationToken cancellationToken)
    {
        if (stream.CanSeek && stream.Length == 0)
            return Errors.FileMissing;

        if (stream.CanSeek && stream.Length > _options.MaxFileSizeBytes)
        {
            _logger.LogInformation(
                "Rejected an upload of {Size} bytes; the configured ceiling is {Ceiling} bytes.",
                stream.Length,
                _options.MaxFileSizeBytes);

            return Errors.FileTooLarge;
        }

        var detected = await ImageContentTypeSniffer.DetectAsync(stream, cancellationToken);

        if (detected is null
            || !_options.AllowedMimeTypes.Contains(detected, StringComparer.OrdinalIgnoreCase))
        {
            _logger.LogInformation(
                "Rejected an upload whose sniffed content type was {Detected}.",
                detected ?? "unrecognised");

            return Errors.InvalidFileType;
        }

        return null;
    }
}

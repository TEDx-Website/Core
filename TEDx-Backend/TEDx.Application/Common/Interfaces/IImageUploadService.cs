using TEDx.Domain.Common;

namespace TEDx.Application.Common.Interfaces;

public interface IImageUploadService
{
    Task<Result<string>> UploadAsync(
        Stream stream,
        string fileName,
        CancellationToken cancellationToken = default);
}

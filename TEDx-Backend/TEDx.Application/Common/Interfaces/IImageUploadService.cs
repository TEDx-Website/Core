namespace TEDx.Application.Common.Interfaces;

public interface IImageUploadService
{
    Task<string> UploadAsync(
        Stream stream,
        string fileName,
        CancellationToken cancellationToken = default);
}

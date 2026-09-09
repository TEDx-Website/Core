using TEDx.Domain.Common;

namespace TEDx.Application.Common.Errors;

public static partial class MediaErrors
{
    // Business (not Validation) is deliberate: the API contract requires these codes
    // at the top level of the error envelope, never nested under fieldErrors.
    // Error.Business exposes no field parameter, so nesting is impossible here.
    public static readonly Error InvalidFileType =
        Error.Business(
            "INVALID_FILE_TYPE",
            "The uploaded file is not an allowed image type.");

    public static readonly Error FileTooLarge =
        Error.Business(
            "FILE_TOO_LARGE",
            "The uploaded file exceeds the maximum allowed size.");

    public static readonly Error FileMissing =
        Error.Business(
            "FILE_MISSING",
            "No file was supplied with the request.");

    public static readonly Error UploadFailed =
        Error.Unexpected(
            "IMAGE_UPLOAD_FAILED",
            "The image could not be stored. Please try again.");
}

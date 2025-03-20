using ErrorOr;

namespace Infrastructure.Errors;
internal static class ClientError
{
    internal static class AwsError
    {
        internal static Error FileNotFound => Error.NotFound("File.NotFound");
        internal static Error DownloadFailed => Error.Unexpected("File.DownloadFailed");
        internal static Error UploadFailed => Error.Unexpected("File.UploadFailed");
    }

    internal static class GeminiError
    {
        internal static Error Unexpected => Error.Unexpected("Gemini.Unexpected");
    }
}
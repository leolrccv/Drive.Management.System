using ErrorOr;

namespace Api.Errors;

internal static class RequestError
{
    public static Error Invalid => Error.Validation("Request.Required");
    public static Error Unexpected => Error.Unexpected("Exception.Unexpected");
}
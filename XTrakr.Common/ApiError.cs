using System.Text.Json.Serialization;

using XTrakr.Common.Enumerations;

namespace XTrakr.Common;

[Serializable]
public sealed class ApiError
{
    private static readonly int _seeMessage = (int)DalErrorCode.SeeMessage;

    public int Code { get; }
    public string? Message { get; }
    public string[]? Messages { get; }

    public ApiError(int code = 0, string? message = null, string[]? messages = null)
    {
        Code = code;
        Message = message ?? string.Empty;
        Messages = messages?.ArrayCopy() ?? Array.Empty<string>();
    }

    public ApiError(int code) : this(code, null, null) { }

    public ApiError(string message) : this(_seeMessage, message, null) { }

    public ApiError(string[] messages) : this(_seeMessage, null, messages) { }

    public ApiError(string message, string[] messages) : this(_seeMessage, message, messages) { }

    [JsonIgnore]
    public bool Successful => Code == (int)DalErrorCode.NoError && string.IsNullOrWhiteSpace(Message) && (Messages is null || !Messages.Any());

    public static ApiError FromDalResult(DalResult result) => new((int)result.ErrorCode, result.Exception?.Innermost());

    public static ApiError FromException(Exception ex) => new((int)DalErrorCode.Exception, ex.Innermost());

    public static ApiError Success => new();
}

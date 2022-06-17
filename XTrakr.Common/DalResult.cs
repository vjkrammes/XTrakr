
using XTrakr.Common.Enumerations;

namespace XTrakr.Common;
public sealed class DalResult
{
    public DalErrorCode ErrorCode { get; init; }
    public Exception? Exception { get; init; }

    public bool Successful => Exception is null && ErrorCode == DalErrorCode.NoError;

    public DalResult(DalErrorCode errorCode, Exception? exception = null)
    {
        ErrorCode = errorCode;
        Exception = exception;
    }

    public static DalResult Duplicate => new(DalErrorCode.Duplicate);

    public static DalResult NotFound => new(DalErrorCode.NotFound);

    public static DalResult Success => new(DalErrorCode.NoError);

    public static DalResult FromException(Exception ex) => new(DalErrorCode.Exception, ex);

    public string ErrorMessage => Exception is not null ? Exception.Innermost() : ErrorCode.GetDescriptionFromEnumValue();
}

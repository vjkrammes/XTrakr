namespace XTrakr.Common;
public class NotFailureException : ApiResultException
{
    public NotFailureException(string? message = null) : base(message ?? "Cannot retrieve failure payload from a success result") { }
}

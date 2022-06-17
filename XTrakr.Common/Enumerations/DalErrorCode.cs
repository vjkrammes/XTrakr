using System.ComponentModel;

namespace XTrakr.Common.Enumerations;
public enum DalErrorCode
{
    [Description("No error")]
    NoError = 0,
    [Description("Not found")]
    NotFound = 1,
    [Description("An exception occurred")]
    Exception = 2,
    [Description("Duplicate")]
    Duplicate = 3,
    [Description("Invalid parameter(s)")]
    Invalid = 4,
    [Description("See Message(s)")]
    SeeMessage = 5
}

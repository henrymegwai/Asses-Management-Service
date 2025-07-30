using System.ComponentModel;

namespace CloudWorks.Application.Common.Enumeration;

public enum AccessPointStatus
{
    [Description("Successful")]
    Successful = 1,
    [Description("NotSuccessful")]
    NotSuccessful = 2,
    [Description("Attempted")]
    Attempted = 3
}

namespace CloudWorks.Application.Common.Utilities;

public record Page<T>(List<T> Items, int PageNumber, int PageSize, int TotalCount);
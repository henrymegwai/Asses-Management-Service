namespace CloudWorks.Application.Common.Models;

public record QueryParamsModel(string? Name, string? SortBy, bool Desc, int PageNumber, int PageSize);
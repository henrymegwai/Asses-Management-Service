using Microsoft.EntityFrameworkCore;

namespace CloudWorks.Application.Common.Utilities;

public static class PageExtension<T> where T : class
{
    public static Page<T> ToPageListAsync(IEnumerable<T> query, int pageNumber, int pageSize)
    {
        var count = query.Count();
        var offset = (pageNumber - 1) * pageSize;
        var items =  query.Skip(offset).Take(pageSize).ToList();
        return new Page<T>(items, pageNumber, pageSize, count);
    }
}
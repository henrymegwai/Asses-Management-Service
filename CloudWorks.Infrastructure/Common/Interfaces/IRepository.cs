using System.Linq.Expressions;
using FluentResults;

namespace CloudWorks.Infrastructure.Common.Interfaces;

public interface IRepository<T> where T : class
{
    Task<Result<T>> AddAsync(T entity, CancellationToken cancellationToken);
    
    Task<Result<List<T>>> AddRangeAsync(List<T> entities, CancellationToken cancellationToken);
    Task<Result<T>> UpdateAsync(T entity, CancellationToken cancellationToken);
    
    Task<Result> DeleteAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken);
    
    Task<Result<T>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    
    Task<Result<T>> GetByAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken);

    Task<Result<List<T>>> GetAllAsync(
        Expression<Func<T, bool>>? predicate,
        Expression<Func<T, object>>[]? includeProperties,
        CancellationToken cancellationToken);

    IEnumerable<T> Query(Expression<Func<T, bool>>? predicate,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        params Expression<Func<T, object>>[]? includes);
}
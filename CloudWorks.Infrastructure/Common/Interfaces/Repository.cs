using System.Linq.Expressions;
using CloudWorks.Application.Common.Interfaces;
using CloudWorks.Infrastructure.Persistence;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CloudWorks.Infrastructure.Common.Interfaces;

public class Repository<T>(CloudWorksDbContext dbContext, IUnitOfWork unitOfWork, ILogger<Repository<T>> logger) :
    IRepository<T> where T : class
{
    private readonly DbSet<T> _dbSet = dbContext.Set<T>();
    
    public async Task<Result<T>> AddAsync(T entity, CancellationToken cancellationToken)
    {
        try
        {
            await _dbSet.AddAsync(entity, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok(entity);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Add operation failed: {Message}", ex.Message);
            return Result.Fail("Add operation failed.");
        }
    }

    public async Task<Result<List<T>>> AddRangeAsync(List<T> entities, CancellationToken cancellationToken)
    {
        try
        {
            await _dbSet.AddRangeAsync(entities, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok(entities);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "AddRange operation failed: {Message}", ex.Message);
            return Result.Fail("AddRange operation failed.");
        }
    }

    public async Task<Result<T>> UpdateAsync(T entity, CancellationToken cancellationToken)
    {
        try
        {
            _dbSet.Update(entity);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok(entity);
        }
        catch (Exception ex)
        {
            logger.LogError("An unexpected error occurred while updating the entity {Message}", ex.Message);
            return Result.Fail($"An unexpected error occurred, please try again later.");
        }
    }

    public async Task<Result> DeleteAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
    {
        try
        {
            var entity = await GetByAsync(predicate, cancellationToken);
            _dbSet.Remove(entity.Value!);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok();
        }
        catch (ArgumentNullException ex)
        {
            logger.LogError(ex, "Delete operation failed: {Message}", ex.Message);
            return Result.Fail("Delete operation failed.");
        }
    }

    public async Task<Result<T>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _dbSet.FindAsync(id, cancellationToken);
            return result is not null ? Result.Ok(result) : Result.Fail("Entity not found.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "GetById operation failed: {Message}", ex.Message);
            return Result.Fail("GetById operation failed.");
        }
    }

    public async Task<Result<T>> GetByAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _dbSet.FirstOrDefaultAsync(predicate, cancellationToken);
            return result is not null ? Result.Ok(result) : Result.Fail("Entity not found.");
        }
        catch(Exception ex)
        {
            logger.LogError(ex, "GetBy operation failed: {Message}", ex.Message);
            return Result.Fail("GetBy operation failed.");
        }
    }
    
    public async Task<Result<List<T>>> GetAllAsync(
        Expression<Func<T, bool>>? predicate,
        Expression<Func<T, object>>[]? includeProperties,
        CancellationToken cancellationToken)
    {
        try
        {
            var query = _dbSet.AsNoTracking();
        
            if (predicate != null)
            {
                query = query.Where(predicate);
            }
        
            if (includeProperties != null || includeProperties!.Length > 0)
            {
                query = includeProperties.
                    Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            }

            var result = await query.ToListAsync(cancellationToken);
        
            return result.Count != 0
                ? Result.Ok(result) 
                : Result.Fail("No entities found.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "GetAll operation failed: {Message}", ex.Message);
            return Result.Fail("GetAll operation failed.");
        }
    }
    
    public IEnumerable<T> Query(Expression<Func<T, bool>>? predicate,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        params Expression<Func<T, object>>[]? includes)
    {
        try
        {
            IQueryable<T> query = _dbSet;

            if (predicate is not null) query = query.Where(predicate);

            if (includes is not null) query 
                = includes.Aggregate(query, (current, include) => current.Include(include));

            if (orderBy is not null) query = orderBy(query);

            return query;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Query operation failed: {Message}", ex.Message);
            throw new InvalidOperationException("Query operation failed.", ex);
        }
    }
}
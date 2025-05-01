using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Scheduley.Core.Domain.RepositoryContracts;
using Scheduley.Infrastructure.DbContexts;

namespace Scheduley.Infrastructure.Repositories;

public class GenericRepository<T>(ApplicationDbContext context) : IGenericRepository<T>
    where T : class
{
    public async Task Create(T entity) => await context.Set<T>().AddAsync(entity);

    public async Task<T> CreateAndGet(T entity) => (await context.Set<T>().AddAsync(entity)).Entity;

    public void Delete(T entity) => context.Set<T>().Remove(entity);

    public async Task<IEnumerable<T>> GetAll() => await context.Set<T>().ToListAsync();

    public async Task<T?> GetOne(int id) => await context.Set<T>().FindAsync(id);

    public async Task<T?> GetOne(Guid id) => await context.Set<T>().FindAsync(id);

    public async Task SaveChangesAsync() => await context.SaveChangesAsync();

    public void Update(T entity) => context.Set<T>().Update(entity);

    public async Task<IEnumerable<T>> Find(Expression<Func<T, bool>> predicate) =>
        await context.Set<T>().Where(predicate).ToListAsync();

    public async Task<T?> FindOne(Expression<Func<T, bool>> predicate) =>
        await context.Set<T>().FirstOrDefaultAsync(predicate);

    public async Task<IEnumerable<T>> GetAllAndPopulateAsync(
        Expression<Func<T, object>> includeExpression
    ) => await context.Set<T>().Include(includeExpression).AsNoTracking().ToListAsync();

    public async Task<List<TResult>> GetFilteredAndProjectAsync<TResult>(
        Expression<Func<T, bool>> predicate,
        Expression<Func<T, object>> includeExpression,
        Expression<Func<T, TResult>> selector
    )
    {
        return await context
            .Set<T>()
            .Include(includeExpression)
            .AsNoTracking()
            .Where(predicate)
            .Select(selector)
            .ToListAsync();
    }

    public async Task<List<TResult>> GetFilteredAndProjectWithoutIncludeAsync<TResult>(
        Expression<Func<T, bool>> predicate,
        Expression<Func<T, TResult>> selector
    )
    {
        return await context
            .Set<T>()
            .AsNoTracking()
            .Where(predicate)
            .Select(selector)
            .ToListAsync();
    }

    public async Task<List<TResult>> GetDistinctAndProjectAsync<TResult>(
        Expression<Func<T, TResult>> selector
    )
    {
        return await context.Set<T>().AsNoTracking().Select(selector).Distinct().ToListAsync();
    }

    public async Task<T?> FindOneAndPopulateAsync(
        Expression<Func<T, bool>> predicate,
        Expression<Func<T, object>> includeExpression
    ) => await context.Set<T>().Include(includeExpression).FirstOrDefaultAsync(predicate);

    //$@"EXEC {procedureName} @{procedureParameterName} = {pageNumber}"
}

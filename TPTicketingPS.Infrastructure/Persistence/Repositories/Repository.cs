using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TPTicketingPS.Application.Common.Interfaces;

namespace TPTicketingPS.Infrastructure.Persistence.Repositories;

public class Repository<T>(AppDbContext context) : IRepository<T> where T : class
{

    protected readonly AppDbContext Context = context;
    protected DbSet<T> Set => Context.Set<T>();

    public virtual async Task<T?> GetByIdAsync(
        object id,
        CancellationToken cancellationToken = default)
        => await Set.FindAsync(new[] { id }, cancellationToken);

    public virtual async Task<List<T>> ListAsync(
        CancellationToken cancellationToken = default)
        => await Set.ToListAsync(cancellationToken);

    public virtual async Task<bool> ExistsAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
        => await Set.AnyAsync(predicate, cancellationToken);

    public virtual async Task AddAsync(
        T entity,
        CancellationToken cancellationToken = default)
        => await Set.AddAsync(entity, cancellationToken);

    public virtual void Remove(T entity)
        => Set.Remove(entity);
}
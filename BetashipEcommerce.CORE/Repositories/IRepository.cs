using BetashipEcommerce.CORE.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BetashipEcommerce.CORE.Repositories
{
    public interface IRepository<TEntity, TId>
      where TEntity : AggregateRoot<TId>
      where TId : notnull
    {
        Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<IReadOnlyList<TEntity>> FindAsync(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default);

        Task<TEntity?> FindOneAsync(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default);

        Task<bool> ExistsAsync(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default);

        Task<int> CountAsync(
            Expression<Func<TEntity, bool>>? predicate = null,
            CancellationToken cancellationToken = default);

        void Add(TEntity entity);

        void Update(TEntity entity);

        void Remove(TEntity entity);
    }

}

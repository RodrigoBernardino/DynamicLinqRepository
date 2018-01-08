using System;
using System.Data.Entity;

namespace DynamicRepository.Contract
{
    public interface IEntityRepository<TEntity, TContext> : IDataRepository<TEntity>
        where TEntity : class, IIdentifiableEntity, new()
        where TContext : DbContext, new()
    {
        Func<TContext> ContextCreator { get; }
    }
}

using System;
using System.Data.Entity;

namespace DynamicRepository.Contract
{
    public interface IEntityCounter<TEntity, TContext> : IDataCounter<TEntity>
        where TEntity : class, IIdentifiableEntity, new()
        where TContext : DbContext, new()
    {
        Func<TContext> ContextCreator { get; }
    }
}

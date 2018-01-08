using DynamicRepository.Utils;
using System.Collections.Generic;

namespace DynamicRepository.Contract
{
    public interface IStoreWriter<TEntity>
    {
        Maybe<TEntity> Add(TEntity entity);

        Maybe<TEntity> AddWithNestedProperties(TEntity entity);

        void AddRange(IEnumerable<TEntity> entityList);

        Maybe<TEntity> Remove(TEntity entity);

        Maybe<TEntity> Remove(int id);

        void RemoveRange(IEnumerable<TEntity> entityList);

        void RemoveRange(IEnumerable<string> clauses);

        Maybe<TEntity> Update(TEntity entity);

        void UpdateRange(IEnumerable<TEntity> entityList);
    }
}

using DynamicRepository.Utils;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DynamicRepository.Contract
{
    public interface IStoreReader<TEntity>
    {
        IEnumerable<TEntity> FindAll();

        IEnumerable<TEntity> FindAll(IEnumerable<string> clauses);

        IEnumerable<TEntity> FindAll(QueryLimits queryLimits);

        IEnumerable<TEntity> FindAll(QueryLimits queryLimits, IEnumerable<string> clauses);

        IEnumerable<TEntity> FindAllIncluding(params Expression<Func<TEntity, object>>[] includeProperties);

        IEnumerable<TEntity> FindAllIncluding(IEnumerable<string> clauses, params Expression<Func<TEntity, object>>[] includeProperties);

        IEnumerable<TEntity> FindAllIncluding(QueryLimits queryLimits, params Expression<Func<TEntity, object>>[] includeProperties);

        IEnumerable<TEntity> FindAllIncluding(QueryLimits queryLimits, IEnumerable<string> clauses, params Expression<Func<TEntity, object>>[] includeProperties);

        IEnumerable<TEntity> FindAllIncludingNestedProps(params string[] includeProperties);

        IEnumerable<TEntity> FindAllIncludingNestedProps(IEnumerable<string> clauses, params string[] includeProperties);

        IEnumerable<TEntity> FindAllIncludingNestedProps(QueryLimits queryLimits, params string[] includeProperties);

        IEnumerable<TEntity> FindAllIncludingNestedProps(QueryLimits queryLimits, IEnumerable<string> clauses, params string[] includeProperties);

        Maybe<TEntity> Find(int id);
    }
}

using DynamicRepository.Contract;
using DynamicRepository.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;

namespace DynamicRepository.Concrete
{
    public class EntityRepository<TEntity, TContext> : IEntityRepository<TEntity, TContext>
        where TEntity : class, IIdentifiableEntity, new()
        where TContext : DbContext, new()
    {
        public EntityRepository(bool lazyLoadingEnabled, bool proxyCreationEnabled)
        {
            ContextCreator = () =>
            {
                var context = new TContext();
                ConfigContext(context, lazyLoadingEnabled, proxyCreationEnabled);
                return context;
            };
        }

        public Func<TContext> ContextCreator { get; private set; }

        private void ConfigContext(TContext context, bool lazyLoadingEnabled, bool proxyCreationEnabled)
        {
            context.Configuration.ProxyCreationEnabled = proxyCreationEnabled;
            context.Configuration.LazyLoadingEnabled = lazyLoadingEnabled;
        }

        public virtual Maybe<TEntity> Add(TEntity entity)
        {
            entity = entity.DeleteNestedProperties();

            using (var entityContext = ContextCreator())
            {
                TEntity addedEntity = entityContext.Set<TEntity>().Add(entity);
                entityContext.SaveChanges();

                if (addedEntity.Id != 0)
                    return new Maybe<TEntity>(addedEntity);
                return new Maybe<TEntity>();
            }
        }

        public virtual Maybe<TEntity> AddWithNestedProperties(TEntity entity)
        {
            using (var entityContext = ContextCreator())
            {
                TEntity addedEntity = entityContext.Set<TEntity>().Add(entity);
                entityContext.SaveChanges();

                if (addedEntity.Id != 0)
                    return new Maybe<TEntity>(addedEntity);
                return new Maybe<TEntity>();
            }
        }

        public virtual void AddRange(IEnumerable<TEntity> entityList)
        {
            using (var entityContext = ContextCreator())
            {
                entityContext.Set<TEntity>().AddRange(entityList);
                entityContext.SaveChanges();
            }
        }

        public virtual Maybe<TEntity> Remove(TEntity entity)
        {
            using (var entityContext = ContextCreator())
            {
                entityContext.Entry(entity).State = EntityState.Deleted;
                entityContext.SaveChanges();
                return new Maybe<TEntity>(entity);
            }
        }

        public virtual Maybe<TEntity> Remove(int id)
        {
            using (var entityContext = ContextCreator())
            {
                TEntity entity = entityContext.Set<TEntity>().Find(id);
                if (entity == null)
                    return new Maybe<TEntity>();

                entityContext.Entry(entity).State = EntityState.Deleted;
                entityContext.SaveChanges();
                return new Maybe<TEntity>(entity);
            }
        }

        public virtual void RemoveRange(IEnumerable<TEntity> entityList)
        {
            using (var entityContext = ContextCreator())
            {
                foreach (var entity in entityList)
                {
                    entityContext.Entry(entity).State = EntityState.Deleted;
                }
                entityContext.SaveChanges();
            }
        }

        public virtual void RemoveRange(IEnumerable<string> clauses)
        {

            using (var entityContext = ContextCreator())
            {
                IQueryable<TEntity> temporaryQuery = entityContext.Set<TEntity>();

                foreach (var clause in clauses)
                    temporaryQuery = temporaryQuery.Where(clause);

                entityContext.Set<TEntity>().RemoveRange(temporaryQuery);
                entityContext.SaveChanges();
            }
        }

        public virtual Maybe<TEntity> Update(TEntity entity)
        {
            entity = entity.DeleteNestedProperties();

            using (var entityContext = ContextCreator())
            {
                TEntity existingEntity =
                    entityContext.Set<TEntity>().FirstOrDefault(e => e.Id == entity.Id);

                if (existingEntity == null)
                    return new Maybe<TEntity>();

                entity.InheritedPropertyMap(existingEntity);

                entityContext.SaveChanges();
                return new Maybe<TEntity>(existingEntity);
            }
        }

        public virtual void UpdateRange(IEnumerable<TEntity> entityList)
        {
            using (var entityContext = ContextCreator())
            {
                foreach (var entity in entityList)
                {
                    var pureEntity = entity.DeleteNestedProperties();

                    TEntity existingEntity =
                        entityContext.Set<TEntity>().FirstOrDefault(e => e.Id == pureEntity.Id);

                    if (existingEntity != null)
                        pureEntity.InheritedPropertyMap(existingEntity);
                }

                entityContext.SaveChanges();
            }
        }

        public virtual IEnumerable<TEntity> FindAll()
        {
            using (var entityContext = ContextCreator())
            {
                return entityContext.Set<TEntity>().ToList();
            }
        }

        public virtual IEnumerable<TEntity> FindAll(IEnumerable<string> clauses)
        {
            using (var entityContext = ContextCreator())
            {
                IQueryable<TEntity> temporaryQuery = entityContext.Set<TEntity>();
                foreach (var clause in clauses)
                    temporaryQuery = temporaryQuery.Where(clause);

                return temporaryQuery.ToList();
            }
        }

        public virtual IEnumerable<TEntity> FindAll(QueryLimits queryLimits)
        {
            using (var entityContext = ContextCreator())
            {
                return entityContext.Set<TEntity>().OrderBy(queryLimits.OrderBy + " " + queryLimits.Orientation)
                    .Skip(queryLimits.Limit * (queryLimits.Page - 1))
                    .Take(queryLimits.Limit)
                    .ToList();
            }
        }

        public virtual IEnumerable<TEntity> FindAll(QueryLimits queryLimits, IEnumerable<string> clauses)
        {
            using (var entityContext = ContextCreator())
            {
                IQueryable<TEntity> temporaryQuery = entityContext.Set<TEntity>();
                foreach (var clause in clauses)
                    temporaryQuery = temporaryQuery.Where(clause);
                return temporaryQuery.OrderBy(queryLimits.OrderBy + " " + queryLimits.Orientation)
                    .Skip(queryLimits.Limit * (queryLimits.Page - 1))
                    .Take(queryLimits.Limit)
                    .ToList();
            }
        }

        public virtual IEnumerable<TEntity> FindAllIncluding(
            params Expression<Func<TEntity, object>>[] includeProperties)
        {
            using (var entityContext = ContextCreator())
            {
                var query = entityContext.Set<TEntity>().AsQueryable();
                query = includeProperties.Aggregate(query,
                    (current, property) => current.Include(property));
                return query.ToList();
            }
        }


        public virtual IEnumerable<TEntity> FindAllIncluding(IEnumerable<string> clauses,
            params Expression<Func<TEntity, object>>[] includeProperties)
        {
            using (var entityContext = ContextCreator())
            {
                IQueryable<TEntity> temporaryQuery = entityContext.Set<TEntity>();
                foreach (var clause in clauses)
                    temporaryQuery = temporaryQuery.Where(clause);
                var query = includeProperties.Aggregate(temporaryQuery,
                    (current, property) => current.Include(property));
                return query.ToList();
            }
        }

        public virtual IEnumerable<TEntity> FindAllIncluding(QueryLimits queryLimits,
            params Expression<Func<TEntity, object>>[] includeProperties)
        {
            using (var entityContext = ContextCreator())
            {
                IQueryable<TEntity> query;

                query = entityContext.Set<TEntity>().OrderBy(queryLimits.OrderBy + " " + queryLimits.Orientation)
                        .Skip(queryLimits.Limit * (queryLimits.Page - 1))
                        .Take(queryLimits.Limit);

                query = includeProperties.Aggregate(query,
                    (current, property) => current.Include(property));
                return query.ToList();
            }
        }

        public virtual IEnumerable<TEntity> FindAllIncluding(QueryLimits queryLimits,
            IEnumerable<string> clauses,
            params Expression<Func<TEntity, object>>[] includeProperties)
        {
            using (var entityContext = ContextCreator())
            {
                IQueryable<TEntity> temporaryQuery = entityContext.Set<TEntity>();
                foreach (var clause in clauses)
                    temporaryQuery = temporaryQuery.Where(clause);

                var query = temporaryQuery.OrderBy(queryLimits.OrderBy + " " + queryLimits.Orientation)
                        .Skip(queryLimits.Limit * (queryLimits.Page - 1))
                        .Take(queryLimits.Limit);

                query = includeProperties.Aggregate(query,
                    (current, property) => current.Include(property));
                return query.ToList();
            }
        }

        public virtual IEnumerable<TEntity> FindAllIncludingNestedProps(
            params string[] includeProperties)
        {
            using (var entityContext = ContextCreator())
            {
                var query = entityContext.Set<TEntity>().AsQueryable();
                query = includeProperties.Aggregate(query,
                    (current, property) => current.Include(property));
                return query.ToList();
            }
        }

        public virtual IEnumerable<TEntity> FindAllIncludingNestedProps(IEnumerable<string> clauses,
            params string[] includeProperties)
        {
            using (var entityContext = ContextCreator())
            {
                IQueryable<TEntity> temporaryQuery = entityContext.Set<TEntity>();
                foreach (var clause in clauses)
                    temporaryQuery = temporaryQuery.Where(clause);
                var query = includeProperties.Aggregate(temporaryQuery,
                    (current, property) => current.Include(property));
                return query.ToList();
            }
        }

        public virtual IEnumerable<TEntity> FindAllIncludingNestedProps(QueryLimits queryLimits,
            params string[] includeProperties)
        {
            using (var entityContext = ContextCreator())
            {
                IQueryable<TEntity> query;

                query = entityContext.Set<TEntity>().OrderBy(queryLimits.OrderBy + " " + queryLimits.Orientation)
                        .Skip(queryLimits.Limit * (queryLimits.Page - 1))
                        .Take(queryLimits.Limit);

                query = includeProperties.Aggregate(query,
                    (current, property) => current.Include(property));
                return query.ToList();
            }
        }

        public virtual IEnumerable<TEntity> FindAllIncludingNestedProps(QueryLimits queryLimits,
            IEnumerable<string> clauses,
            params string[] includeProperties)
        {
            using (var entityContext = ContextCreator())
            {
                IQueryable<TEntity> temporaryQuery = entityContext.Set<TEntity>();
                foreach (var clause in clauses)
                    temporaryQuery = temporaryQuery.Where(clause);

                var query = temporaryQuery.OrderBy(queryLimits.OrderBy + " " + queryLimits.Orientation)
                        .Skip(queryLimits.Limit * (queryLimits.Page - 1))
                        .Take(queryLimits.Limit);

                query = includeProperties.Aggregate(query,
                    (current, property) => current.Include(property));
                return query.ToList();
            }
        }

        public Maybe<TEntity> Find(int id)
        {
            using (var entityContext = ContextCreator())
            {
                var entity = entityContext.Set<TEntity>().Find(id);

                if (entity == null)
                    return new Maybe<TEntity>();
                return new Maybe<TEntity>(entity);
            }
        }

        public virtual IEnumerable<TEntity> ExecuteQuery(string query)
        {
            using (var entityContext = ContextCreator())
            {
                var result = entityContext.Database.SqlQuery<TEntity>(query);

                return result.ToList();
            }
        }
    }
}

using DynamicRepository.Contract;
using DynamicRepository.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;

namespace DynamicRepository.Concrete
{
    public class EntityCounter<TEntity, TContext> : IEntityCounter<TEntity, TContext>
        where TEntity : class, IIdentifiableEntity, new()
        where TContext : DbContext, new()
    {
        public EntityCounter(bool lazyLoadingEnabled, bool proxyCreationEnabled)
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

        public Maybe<int> Count()
        {
            using (var context = ContextCreator())
            {
                return new Maybe<int>(context.Set<TEntity>().Count());
            }
        }

        public Maybe<int> Count(IEnumerable<string> clauses)
        {
            using (var context = ContextCreator())
            {
                IQueryable<TEntity> temporaryQuery = context.Set<TEntity>();
                foreach (var clause in clauses)
                    temporaryQuery = temporaryQuery.Where(clause);
                return new Maybe<int>(temporaryQuery.Count());
            }
        }
    }
}

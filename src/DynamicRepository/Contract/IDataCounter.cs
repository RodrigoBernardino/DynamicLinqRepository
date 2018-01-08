using DynamicRepository.Utils;
using System.Collections.Generic;

namespace DynamicRepository.Contract
{
    public interface IDataCounter
    { }

    public interface IDataCounter<TEntity> : IDataCounter
    {
        Maybe<int> Count();
        Maybe<int> Count(IEnumerable<string> clauses);
    }
}

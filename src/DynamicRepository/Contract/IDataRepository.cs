
namespace DynamicRepository.Contract
{
    public interface IDataRepository
    { }

    public interface IDataRepository<TEntity> : IStoreReader<TEntity>, IStoreWriter<TEntity>, IDataRepository
    { }
}

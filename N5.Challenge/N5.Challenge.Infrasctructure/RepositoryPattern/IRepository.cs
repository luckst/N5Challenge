using System.Linq.Expressions;

namespace N5.Challenge.Infrasctructure.RepositoryPattern
{
    public interface IRepository<T> : IDisposable 
    {
        Task CreateAsync(T entity);
        Task<T> GetAsync(Guid id);
        Task UpdateAsync(T entity);

    }
}

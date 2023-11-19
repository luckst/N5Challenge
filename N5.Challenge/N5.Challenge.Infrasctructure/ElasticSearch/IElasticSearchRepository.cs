namespace N5.Challenge.Infrasctructure.ElasticSearch
{
    public interface IElasticSearchRepository<T>
    {
        Task PersistAsync(T resgitry);
    }
}

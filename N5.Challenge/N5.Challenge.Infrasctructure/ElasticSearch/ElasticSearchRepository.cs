using Nest;

namespace N5.Challenge.Infrasctructure.ElasticSearch
{
    public class ElasticSearchRepository<T> : IElasticSearchRepository<T> where T : class
    {
        private readonly ElasticClient _client;

        public ElasticSearchRepository(ElasticClient client)
        {
            _client = client;
        }
            
        public async Task PersistAsync(T resgitry)
        {   
            var indexResponse = await _client.IndexDocumentAsync<T>(resgitry);
        }
    }
}

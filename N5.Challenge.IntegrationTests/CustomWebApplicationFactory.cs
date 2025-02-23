using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            services.AddScoped<IElasticSearchRepository<Permission>>(sp => 
                Mock.Of<IElasticSearchRepository<Permission>>());
            
            services.AddScoped<IOperationProducer>(sp => 
                Mock.Of<IOperationProducer>());
        });
    }
}
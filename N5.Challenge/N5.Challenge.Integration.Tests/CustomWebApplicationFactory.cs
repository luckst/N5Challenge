using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using N5.Challenge.API;
using N5.Challenge.Domain;
using N5.Challenge.Infrasctructure;
using N5.Challenge.Infrasctructure.ElasticSearch;
using N5.Challenge.Infrasctructure.KafkaConfig.Producers;
using Nest;
using System;

namespace N5.Challenge.Integration.Tests
{
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureServices((context, services) =>
            {
                // Remove the existing DbContext registration
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ServiceDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                Environment.SetEnvironmentVariable("DB_Server", "localhost");

                // Get the connection string from configuration
                var dbServer = Environment.GetEnvironmentVariable("DB_Server");
                if (string.IsNullOrEmpty(dbServer))
                {
                    throw new ArgumentException("DB_Server environment variable is not set.");
                }

                var dbConnectionString = string.Format(
                    context.Configuration.GetConnectionString("N5Challenge_db")!,
                    dbServer
                );

                // Add DbContext using the real database connection string
                services.AddDbContext<ServiceDbContext>(options =>
                {
                    options.UseSqlServer(dbConnectionString);
                });

                services.AddScoped<IElasticSearchRepository<Permission>>(sp =>
                    Mock.Of<IElasticSearchRepository<Permission>>());

                services.AddScoped<IOperationProducer>(sp =>
                    Mock.Of<IOperationProducer>());

                // Ensure the database is created
                using (var scope = services.BuildServiceProvider().CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<ServiceDbContext>();
                    db.Database.EnsureCreated();
                }
            });
        }
    }
}

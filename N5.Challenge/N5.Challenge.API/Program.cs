using Elasticsearch.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using N5.Challenge.Application.Commands;
using N5.Challenge.Application.Queries;
using N5.Challenge.Infrasctructure;
using N5.Challenge.Infrasctructure.ElasticSearch;
using N5.Challenge.Infrasctructure.KafkaConfig;
using N5.Challenge.Infrasctructure.RepositoryPattern;
using Nest;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.CustomSchemaIds(s => s.FullName.Replace("+", "."));
});

// ELastic search config
var pool = new SingleNodeConnectionPool(new Uri(builder.Configuration["ElasticSearch:Url"]!));
var settings = new ConnectionSettings(pool)
    .DefaultIndex(builder.Configuration["ElasticSearch:PermissionIndex"]!)
    .MaximumRetries(3);
var elasticClient = new ElasticClient(settings);

builder.Services.AddSingleton(elasticClient);

// serilog config
var logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
builder.Services.AddSingleton(logger);

builder.Services.AddSingleton<KafkaClientHandle>();
builder.Services.AddSingleton<IKafkaProducer<string, string>, KafkaProducer<string, string>>();

builder.Services.AddScoped(typeof(IElasticSearchRepository<>), typeof(ElasticSearchRepository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddMediatR(Assembly.GetAssembly(typeof(GetEmployeePermissionsQueryHandler)));

string dbConnectionString = string.Format(builder.Configuration.GetConnectionString("N5Challenge_db")!, Environment.GetEnvironmentVariable("DB_Server"));

builder.Services
    .AddDbContextPool<ServiceDbContext>(options =>
    {

        options.UseSqlServer(dbConnectionString,
        sqlServerOptionsAction: sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
        });
    },
        10
    );

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

SeedDatabase(app);

app.Run();

void SeedDatabase(IHost host)
{
    using (var scope = host.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<ServiceDbContext>();
            new ServiceSeeding().SeedAsync(context).Wait();
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError("An error occurred while seeding the service database");
            logger.LogError(ex.ToString());
        }
    }
}
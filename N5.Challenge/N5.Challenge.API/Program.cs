using Elasticsearch.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using N5.Challenge.Application.Commands;
using N5.Challenge.Application.Queries;
using N5.Challenge.Entities.Settings;
using N5.Challenge.Infrasctructure;
using N5.Challenge.Infrasctructure.ElasticSearch;
using N5.Challenge.Infrasctructure.KafkaConfig;
using N5.Challenge.Infrasctructure.KafkaConfig.Producers;
using N5.Challenge.Infrasctructure.RepositoryPattern;
using Nest;
using Serilog;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.CustomSchemaIds(s => s.FullName.Replace("+", "."));
});

// Elastic search config
var esSection = builder.Configuration.GetSection("ElasticSearch");
var esUrl = esSection["Url"] ?? "https://localhost:9200";
var esUser = esSection["UserName"] ?? "elastic";
var esPass = esSection["Password"] ?? "";
var disableCertValidation = esSection["DisableCertificateValidation"] ?? "false";

var pool = new SingleNodeConnectionPool(new Uri(esUrl));
var connectionSettings = new ConnectionSettings(pool)
    .BasicAuthentication(esUser, esPass)
    .DefaultIndex("my_index")
    // Habilita el modo debug
    .EnableDebugMode()  
    // Callback que se ejecuta al completar cada request
    .OnRequestCompleted(details =>
    {
        if (details.RequestBodyInBytes != null)
            Console.WriteLine("### ES REQUEST ###\n" + Encoding.UTF8.GetString(details.RequestBodyInBytes));

        if (details.ResponseBodyInBytes != null)
            Console.WriteLine("### ES RESPONSE ###\n" + Encoding.UTF8.GetString(details.ResponseBodyInBytes));
    });

if (disableCertValidation.Equals("true", StringComparison.OrdinalIgnoreCase))
{
    connectionSettings.ServerCertificateValidationCallback((sender, cert, chain, errors) => true);
}

var esClient = new ElasticClient(connectionSettings);

builder.Services.AddSingleton(esClient);

// Kafka config
var kafkaSection = builder.Configuration.GetSection("Kafka");
builder.Services.Configure<KafkaSettings>(kafkaSection);

var kafkaSettings = new KafkaSettings();
kafkaSection.Bind(kafkaSettings);
builder.Services.AddSingleton(kafkaSettings);

var producerConfig = new Confluent.Kafka.ProducerConfig
{
    BootstrapServers = kafkaSettings.ProducerSettings.BootstrapServers
};

builder.Services.AddSingleton(producerConfig);

// Serilog config
var logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
builder.Services.AddSingleton(logger);

builder.Services.AddSingleton<IKafkaProducer<string, string>, KafkaProducer<string, string>>();

builder.Services.AddSingleton<IOperationProducer, OperationProducer>();

builder.Services.AddScoped(typeof(IElasticSearchRepository<>), typeof(ElasticSearchRepository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddMediatR(Assembly.GetAssembly(typeof(GetEmployeePermissionsQueryHandler)));

string dbConnectionString = string.Format(builder.Configuration.GetConnectionString("N5Challenge_db")!, Environment.GetEnvironmentVariable("DB_Server"));

builder.Services.AddDbContext<ServiceDbContext>(options =>
{
    options.UseSqlServer(dbConnectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
    });
});


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
    using var scope = host.Services.CreateScope();
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

static WebApplicationBuilder CreateHostBuilder(string[]? args = null)
{
    return WebApplication.CreateBuilder(args ?? Array.Empty<string>());
}

public partial class Program { }
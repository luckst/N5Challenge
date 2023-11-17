using MediatR;
using Microsoft.EntityFrameworkCore;
using N5.Challenge.Application.Queries;
using N5.Challenge.Infrasctructure;
using N5.Challenge.Infrasctructure.RepositoryPattern;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
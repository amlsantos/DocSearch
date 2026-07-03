using DocSearch.WebApi.Application.Common.Interfaces;
using DocSearch.WebApi.Application.Features.Ingestion;
using DocSearch.WebApi.Infrastructure.Persistence;
using DocSearch.WebApi.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace DocSearch.WebApi;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        ConfigureServices(builder.Services, builder.Configuration);
        
        var app = builder.Build();
        
        ApplyMigrations(app);
        
        ConfigurePipeline(app);
        
        app.Run();
    }

    static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // A. Add Controllers Support
        services.AddControllers();
    
        // B. Add Swagger/OpenAPI
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        
        // C. Add Database Context (PostgreSQL)
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        
        // D. Add Application & Infrastructure Services (Dependency Injection)
        services.AddScoped<IDocumentReader, MarkdownDocumentReader>();
        services.AddScoped<IDocumentRepository, DocumentRepository>();
        services.AddScoped<DocumentIngestionService>();
    }

    static void ConfigurePipeline(WebApplication app)
    {
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
    }
    
    static void ApplyMigrations(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.Migrate();
    }
}
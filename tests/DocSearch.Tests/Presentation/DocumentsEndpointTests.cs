using System.Net;
using DocSearch.WebApi;
using DocSearch.WebApi.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DocSearch.Tests.Presentation;

public class DocumentsEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public DocumentsEndpointTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureServices(services =>
            {
                // Remove all EF Core DbContext registrations from Program.cs (Npgsql)
                var dbDescriptors = services
                    .Where(d => d.ServiceType.FullName?.Contains("DbContext") == true ||
                                d.ServiceType.FullName?.Contains("EntityFramework") == true)
                    .ToList();
                foreach (var d in dbDescriptors) services.Remove(d);

                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb_Docs_Integration");
                });
            });
        });
    }

    [Fact]
    public async Task GetAllDocuments_ReturnsOk()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/documents/docs");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}

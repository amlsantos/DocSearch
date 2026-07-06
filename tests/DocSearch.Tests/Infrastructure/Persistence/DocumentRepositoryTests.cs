using DocSearch.WebApi.Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DocSearch.Tests.Infrastructure.Persistence;

public class DocumentRepositoryTests : IDisposable
{
    private readonly TestDbContext _context;
    private readonly TestDocumentRepository _repository;

    public DocumentRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(databaseName: $"DocSearchTestDb_{Guid.NewGuid()}")
            .Options;

        _context = new TestDbContext(options);
        _repository = new TestDocumentRepository(_context);
    }

    [Fact]
    public async Task InsertDocumentsAsync_PersistsDocumentsAndChunks()
    {
        // Arrange
        var doc = new Document("test.md", "/docs/test.md", DateTime.UtcNow);
        doc.AddChunk("First chunk of content.");
        doc.AddChunk("Second chunk of content.");

        // Act
        await _repository.InsertDocumentsAsync(new[] { doc });

        // Assert
        var documents = await _context.Documents.Include(d => d.Chunks).ToListAsync();
        documents.Should().HaveCount(1);
        documents[0].FileName.Should().Be("test.md");
        documents[0].Chunks.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAllDocumentsAsync_ReturnsAllDocuments()
    {
        // Arrange
        var doc1 = new Document("doc1.md", "/docs/doc1.md", DateTime.UtcNow);
        doc1.AddChunk("Content 1");
        var doc2 = new Document("doc2.md", "/docs/doc2.md", DateTime.UtcNow);
        doc2.AddChunk("Content 2");

        await _context.Documents.AddRangeAsync(doc1, doc2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllDocumentsAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Select(d => d.FileName).Should().BeEquivalentTo("doc1.md", "doc2.md");
    }

    [Fact]
    public async Task GetAllDocumentsAsync_EmptyDatabase_ReturnsEmptyList()
    {
        // Act
        var result = await _repository.GetAllDocumentsAsync();

        // Assert
        result.Should().BeEmpty();
        result.Should().NotBeNull();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

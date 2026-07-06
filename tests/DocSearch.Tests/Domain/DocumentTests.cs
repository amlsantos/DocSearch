using DocSearch.WebApi.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace DocSearch.Tests.Domain;

public class DocumentTests
{
    [Fact]
    public void Constructor_ValidInput_CreatesDocument()
    {
        // Arrange
        var fileName = "test-doc.md";
        var sourcePath = "/docs/test-doc.md";
        var lastModified = new DateTime(2025, 1, 15, 10, 30, 0, DateTimeKind.Utc);

        // Act
        var document = new Document(fileName, sourcePath, lastModified);

        // Assert
        document.Id.Should().NotBeEmpty();
        document.FileName.Should().Be(fileName);
        document.SourcePath.Should().Be(sourcePath);
        document.LastModifiedUtc.Should().Be(lastModified);
        document.Chunks.Should().BeEmpty();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_EmptyFileName_ThrowsArgumentException(string? fileName)
    {
        // Act
        var act = () => new Document(fileName!, "/some/path", DateTime.UtcNow);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithParameterName("fileName");
    }

    [Fact]
    public void AddChunk_ValidContent_AddsChunkToCollection()
    {
        // Arrange
        var document = new Document("test.md", "/docs/test.md", DateTime.UtcNow);

        // Act
        document.AddChunk("This is a valid chunk of text.");

        // Assert
        document.Chunks.Should().HaveCount(1);
        document.Chunks.First().Content.Should().Be("This is a valid chunk of text.");
        document.Chunks.First().OrderIndex.Should().Be(0);
        document.Chunks.First().DocumentId.Should().Be(document.Id);
    }

    [Fact]
    public void AddChunk_MultipleChunks_IncrementsOrderIndex()
    {
        // Arrange
        var document = new Document("test.md", "/docs/test.md", DateTime.UtcNow);

        // Act
        document.AddChunk("First chunk");
        document.AddChunk("Second chunk");
        document.AddChunk("Third chunk");

        // Assert
        document.Chunks.Should().HaveCount(3);
        document.Chunks.Select(c => c.OrderIndex).Should().BeEquivalentTo(new[] { 0, 1, 2 });
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void AddChunk_EmptyContent_ThrowsInvalidOperationException(string? content)
    {
        // Arrange
        var document = new Document("test.md", "/docs/test.md", DateTime.UtcNow);

        // Act
        var act = () => document.AddChunk(content!);

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Key_ReturnsExpectedFormat()
    {
        // Arrange
        var lastModified = new DateTime(2025, 6, 15, 14, 30, 45, DateTimeKind.Utc);
        var document = new Document("my-doc.md", "/docs/my-doc.md", lastModified);

        // Act
        var key = document.Key();

        // Assert
        key.Should().Be("my-doc.md_2025-06-15 14:30:45");
    }
}

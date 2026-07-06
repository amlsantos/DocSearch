using DocSearch.WebApi.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace DocSearch.Tests.Domain;

public class DocumentChunkTests
{
    [Fact]
    public void AddChunk_ContentExceedsMaxLength_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var document = new Document("test.md", "/docs/test.md", DateTime.UtcNow);
        var oversizedContent = new string('A', 8001);

        // Act
        var act = () => document.AddChunk(oversizedContent);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("content");
    }

    [Fact]
    public void AddChunk_ContentAtMaxLength_Succeeds()
    {
        // Arrange
        var document = new Document("test.md", "/docs/test.md", DateTime.UtcNow);
        var maxContent = new string('A', 8000);

        // Act
        document.AddChunk(maxContent);

        // Assert
        document.Chunks.Should().HaveCount(1);
        document.Chunks.First().Content.Should().HaveLength(8000);
    }

    [Fact]
    public void AddChunk_ContentIsTrimmed()
    {
        // Arrange
        var document = new Document("test.md", "/docs/test.md", DateTime.UtcNow);

        // Act
        document.AddChunk("  some text with spaces  ");

        // Assert
        document.Chunks.First().Content.Should().Be("some text with spaces");
    }
}

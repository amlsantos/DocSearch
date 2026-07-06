using DocSearch.WebApi.Infrastructure.Services;
using FluentAssertions;
using Xunit;

namespace DocSearch.Tests.Infrastructure;

public class MarkdownDocumentReaderTests : IDisposable
{
    private readonly string _tempDir;
    private readonly MarkdownDocumentReader _reader;

    public MarkdownDocumentReaderTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"DocSearchTests_{Guid.NewGuid()}");
        Directory.CreateDirectory(_tempDir);
        _reader = new MarkdownDocumentReader();
    }

    [Fact]
    public async Task ReadFilesAsync_ValidFile_ReturnsDocumentWithChunks()
    {
        // Arrange
        var filePath = CreateTempFile("test.md", "This is a paragraph.\n\nThis is another paragraph.");

        // Act
        var documents = await CollectAsync(_reader.ReadFilesAsync(new[] { filePath }));

        // Assert
        documents.Should().HaveCount(1);
        documents[0].FileName.Should().Be("test.md");
        documents[0].Chunks.Should().HaveCount(2);
    }

    [Fact]
    public async Task ReadFilesAsync_FileWithDoubleNewlines_SplitsIntoMultipleChunks()
    {
        // Arrange
        var content = "Chapter 1\n\nChapter 2\n\nChapter 3\n\nChapter 4";
        var filePath = CreateTempFile("multi.md", content);

        // Act
        var documents = await CollectAsync(_reader.ReadFilesAsync(new[] { filePath }));

        // Assert
        documents[0].Chunks.Should().HaveCount(4);
        documents[0].Chunks.First().Content.Should().Be("Chapter 1");
        documents[0].Chunks.Last().Content.Should().Be("Chapter 4");
    }

    [Fact]
    public async Task ReadFilesAsync_EmptyFile_SkipsDocument()
    {
        // Arrange
        var filePath = CreateTempFile("empty.md", "   \n\n   ");

        // Act
        var documents = await CollectAsync(_reader.ReadFilesAsync(new[] { filePath }));

        // Assert
        documents.Should().BeEmpty();
    }

    [Fact]
    public async Task ReadFilesAsync_MultipleFiles_YieldsAllDocuments()
    {
        // Arrange
        var file1 = CreateTempFile("doc1.md", "Content of doc 1");
        var file2 = CreateTempFile("doc2.md", "Content of doc 2");
        var file3 = CreateTempFile("doc3.md", "Content of doc 3");

        // Act
        var documents = await CollectAsync(_reader.ReadFilesAsync(new[] { file1, file2, file3 }));

        // Assert
        documents.Should().HaveCount(3);
        documents.Select(d => d.FileName).Should().BeEquivalentTo("doc1.md", "doc2.md", "doc3.md");
    }

    // Helper: collect IAsyncEnumerable into a list
    private static async Task<List<T>> CollectAsync<T>(IAsyncEnumerable<T> source)
    {
        var list = new List<T>();
        await foreach (var item in source)
        {
            list.Add(item);
        }
        return list;
    }

    // Helper: create a temp .md file
    private string CreateTempFile(string fileName, string content)
    {
        var filePath = Path.Combine(_tempDir, fileName);
        File.WriteAllText(filePath, content);
        return filePath;
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
        {
            Directory.Delete(_tempDir, recursive: true);
        }
    }
}

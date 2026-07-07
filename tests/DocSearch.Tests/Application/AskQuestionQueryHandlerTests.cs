using DocSearch.WebApi.Application.Common.Interfaces;
using DocSearch.WebApi.Application.Features.Answer.Query;
using DocSearch.WebApi.Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace DocSearch.Tests.Application;

public class AskQuestionQueryHandlerTests
{
    private readonly Mock<IDocumentRepository> _repositoryMock;
    private readonly Mock<IQuestionAnswerer> _answererMock;
    private readonly AskQuestionQueryHandler _handler;

    public AskQuestionQueryHandlerTests()
    {
        _repositoryMock = new Mock<IDocumentRepository>();
        _answererMock = new Mock<IQuestionAnswerer>();
        _handler = new AskQuestionQueryHandler(_repositoryMock.Object, _answererMock.Object);
    }

    [Fact]
    public async Task Handle_NoChunksFound_ReturnsDefaultMessage()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.RetrieveChunksAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<(DocumentChunk Chunk, float Rank)>());

        var query = new AskQuestionQuery("What is LanguageWire?");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Answer.Should().Contain("couldn't find any relevant information");
        result.Sources.Should().BeEmpty();
        _answererMock.Verify(a => a.AnswerAsync(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ChunksFound_CallsAnswererWithContext()
    {
        // Arrange
        var doc = new Document("test.md", "/docs/test.md", DateTime.UtcNow);
        doc.AddChunk("LanguageWire is a translation company.");
        doc.AddChunk("Founded in Copenhagen, Denmark.");
        var chunks = doc.Chunks.Select((c, i) => (Chunk: c, Rank: 0.5f - (i * 0.1f))).ToList();

        _repositoryMock
            .Setup(r => r.RetrieveChunksAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(chunks);

        _answererMock
            .Setup(a => a.AnswerAsync(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("LanguageWire is a translation company founded in Copenhagen.");

        var query = new AskQuestionQuery("What is LanguageWire?");

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _answererMock.Verify(a => a.AnswerAsync(
            "What is LanguageWire?",
            It.Is<IEnumerable<string>>(ctx => ctx.Count() == 2),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ChunksFound_ReturnsAnswerWithSources()
    {
        // Arrange
        var doc = new Document("test.md", "/docs/test.md", DateTime.UtcNow);
        doc.AddChunk("Some relevant content.");
        var chunks = doc.Chunks.Select(c => (Chunk: c, Rank: 0.75f)).ToList();

        _repositoryMock
            .Setup(r => r.RetrieveChunksAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(chunks);

        _answererMock
            .Setup(a => a.AnswerAsync(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("The answer based on the documents.");

        var query = new AskQuestionQuery("Tell me about this.");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Answer.Should().Be("The answer based on the documents.");
        result.Sources.Should().HaveCount(1);
        result.Sources.First().Chunk.Content.Should().Be("Some relevant content.");
        result.Sources.First().Rank.Should().Be(0.75f);
    }
}

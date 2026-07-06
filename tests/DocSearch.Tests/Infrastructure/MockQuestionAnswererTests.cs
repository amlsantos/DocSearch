using DocSearch.WebApi.Infrastructure.Services;
using FluentAssertions;
using Xunit;

namespace DocSearch.Tests.Infrastructure;

public class MockQuestionAnswererTests
{
    private readonly MockQuestionAnswerer _answerer = new();

    [Fact]
    public async Task AnswerAsync_WithChunks_ReturnsFormattedResponse()
    {
        // Arrange
        var chunks = new[] { "LanguageWire is a translation company.", "Founded in Denmark." };

        // Act
        var result = await _answerer.AnswerAsync("What is LanguageWire?", chunks);

        // Assert
        result.Should().Contain("[MOCK ANSWER]");
        result.Should().Contain("2 provided chunks");
        result.Should().Contain("LanguageWire is a translation company");
    }

    [Fact]
    public async Task AnswerAsync_EmptyChunks_ReturnsFallbackMessage()
    {
        // Arrange
        var chunks = Enumerable.Empty<string>();

        // Act
        var result = await _answerer.AnswerAsync("What is LanguageWire?", chunks);

        // Assert
        result.Should().Contain("could not find an answer");
    }
}

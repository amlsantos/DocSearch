using DocSearch.WebApi.Application.Common.Interfaces;

namespace DocSearch.WebApi.Infrastructure.Services;

public class MockQuestionAnswerer : IQuestionAnswerer
{
    public Task<string> AnswerAsync(string question, IEnumerable<string> contextChunks, CancellationToken cancellationToken = default)
    {
        if (!contextChunks.Any())
        {
            return Task.FromResult("I could not find an answer in the provided documents.");
        }

        // A mock implementation that returns a fake response based on context size
        var response = $"[MOCK ANSWER]\nBased on {contextChunks.Count()} provided chunks, I conclude that the answer to '{question}' is found in the documents. " +
                       $"Here is a snippet from the first chunk: '{contextChunks.First().Substring(0, Math.Min(50, contextChunks.First().Length))}...'";

        return Task.FromResult(response);
    }
}

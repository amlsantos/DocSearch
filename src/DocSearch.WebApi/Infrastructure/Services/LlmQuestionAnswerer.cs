using DocSearch.WebApi.Application.Common.Interfaces;
using Google.GenAI;

namespace DocSearch.WebApi.Infrastructure.Services;

public class LlmQuestionAnswerer : IQuestionAnswerer
{
    private const string Model = "gemini-2.5-flash";
    private readonly Client _client;

    public LlmQuestionAnswerer(Client client)
    {
        _client = client;
    }
    
    public async Task<string> AnswerAsync(string question, IEnumerable<string> contextChunks, CancellationToken cancellationToken = default)
    {
        string context = string.Join("\n\n---\n\n", contextChunks);
        string prompt = $"Based on the following documents:\n\n{context}\n\nAnswer the question: {question}";
        
        var response = await _client.Models.GenerateContentAsync(
            model: Model, contents: prompt
        );
        
        return response.Candidates[0].Content.Parts[0].Text ?? "No answer generated.";
    }
}
using DocSearch.WebApi.Domain.Entities;

namespace DocSearch.WebApi.Application.Features.Answer.Query;

public class AskQuestionResult
{
    public string Answer { get; set; } = string.Empty;
    public IList<(DocumentChunk Chunk, float Rank)> Sources { get; set; } = new List<(DocumentChunk Chunk, float Rank)>();
}

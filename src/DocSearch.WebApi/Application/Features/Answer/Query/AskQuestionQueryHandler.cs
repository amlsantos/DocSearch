using DocSearch.WebApi.Application.Common.Interfaces;
using MediatR;

namespace DocSearch.WebApi.Application.Features.Answer.Query;

public class AskQuestionQueryHandler : IRequestHandler<AskQuestionQuery, AskQuestionResult>
{
    private readonly IDocumentRepository _repository;
    private readonly IQuestionAnswerer _answerer;

    public AskQuestionQueryHandler(IDocumentRepository repository, IQuestionAnswerer answerer)
    {
        _repository = repository;
        _answerer = answerer;
    }

    public async Task<AskQuestionResult> Handle(AskQuestionQuery request, CancellationToken cancellationToken)
    {
        // Convert natural language to a forgiving "OR" query for PostgreSQL Full-Text Search
        // e.g. "What is the size?" -> "What OR is OR the OR size?"
        var searchKeywords = string.Join(" OR ", request.Question
            .Split(new[] { ' ', '?', '.', ',' }, StringSplitOptions.RemoveEmptyEntries)
            .Where(w => w.Length > 2)); // Filter out very short words
            
        // If the question only had small words, fallback to the original
        var searchTerm = string.IsNullOrWhiteSpace(searchKeywords) ? request.Question : searchKeywords;

        var sources = await _repository.RetrieveChunksAsync(searchTerm, request.Limit, cancellationToken);
        
        // If no chunks found, skip the AI call to save tokens
        if (!sources.Any())
        {
            return new AskQuestionResult 
            {
                Answer = "I couldn't find any relevant information in the knowledge base to answer your question.",
                Sources = sources
            };
        }

        var contextChunks = sources.Select(s => s.Chunk.Content);

        var answer = await _answerer.AnswerAsync(request.Question, contextChunks, cancellationToken);

        return new AskQuestionResult
        {
            Answer = answer,
            Sources = sources
        };
    }
}

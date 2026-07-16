using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DocSearch.WebApi.Application.Common.Interfaces;
using DocSearch.WebApi.Domain.Entities;
using MediatR;

namespace DocSearch.WebApi.Application.Features.Retrieval.Queries;

public class GetDocumentQueryHandler : IRequestHandler<GetDocumentQuery, IList<(DocumentChunk Chunk, float Rank)>>
{
    private readonly IDocumentRepository _repository;

    public GetDocumentQueryHandler(IDocumentRepository repository)
    {
        _repository = repository;
    }

    public async Task<IList<(DocumentChunk Chunk, float Rank)>> Handle(GetDocumentQuery request, CancellationToken cancellationToken)
    {
        // Se a string de pesquisa estiver vazia, devolvemos lista vazia
        if (string.IsNullOrWhiteSpace(request.Term))
        {
            return new List<(DocumentChunk, float)>();
        }

        // Convert natural language to a forgiving "OR" query for PostgreSQL Full-Text Search
        var searchKeywords = string.Join(" OR ", request.Term
            .Split(new[] { ' ', '?', '.', ',' }, StringSplitOptions.RemoveEmptyEntries)
            .Where(w => w.Length > 2));
            
        var searchTerm = string.IsNullOrWhiteSpace(searchKeywords) ? request.Term : searchKeywords;

        return await _repository.RetrieveChunksAsync(searchTerm, request.Limit, cancellationToken);
    }
}

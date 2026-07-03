using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DocSearch.WebApi.Application.Common.Interfaces;
using DocSearch.WebApi.Domain.Entities;
using MediatR;

namespace DocSearch.WebApi.Application.Features.Retrieval.Queries;

public class RetrieveDocumentsQueryHandler : IRequestHandler<RetrieveDocumentsQuery, IList<(DocumentChunk Chunk, float Rank)>>
{
    private readonly IDocumentRepository _repository;

    public RetrieveDocumentsQueryHandler(IDocumentRepository repository)
    {
        _repository = repository;
    }

    public async Task<IList<(DocumentChunk Chunk, float Rank)>> Handle(RetrieveDocumentsQuery request, CancellationToken cancellationToken)
    {
        // Se a string de pesquisa estiver vazia, devolvemos lista vazia
        if (string.IsNullOrWhiteSpace(request.Term))
        {
            return new List<(DocumentChunk, float)>();
        }

        return await _repository.RetrieveChunksAsync(request.Term, cancellationToken);
    }
}

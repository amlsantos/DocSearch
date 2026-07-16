using System.Collections.Generic;
using DocSearch.WebApi.Domain.Entities;
using MediatR;

namespace DocSearch.WebApi.Application.Features.Retrieval.Queries;

public class GetDocumentQuery : IRequest<IList<(DocumentChunk Chunk, float Rank)>>
{
    public string Term { get; }
    public int Limit { get; }

    public GetDocumentQuery(string term, int limit = 5)
    {
        Term = term;
        Limit = limit;
    }
}

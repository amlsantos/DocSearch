using System.Collections.Generic;
using DocSearch.WebApi.Domain.Entities;
using MediatR;

namespace DocSearch.WebApi.Application.Features.Retrieval.Queries;

public class RetrieveDocumentsQuery : IRequest<IList<(DocumentChunk Chunk, float Rank)>>
{
    public string Term { get; }

    public RetrieveDocumentsQuery(string term)
    {
        Term = term;
    }
}

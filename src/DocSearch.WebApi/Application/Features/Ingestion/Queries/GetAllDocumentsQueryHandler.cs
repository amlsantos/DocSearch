using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DocSearch.WebApi.Application.Common.Interfaces;
using DocSearch.WebApi.Domain.Entities;
using MediatR;

namespace DocSearch.WebApi.Application.Features.Ingestion.Queries;

public class GetAllDocumentsQueryHandler : IRequestHandler<GetAllDocumentsQuery, IList<Document>>
{
    private readonly IDocumentRepository _documentRepository;

    public GetAllDocumentsQueryHandler(IDocumentRepository documentRepository)
    {
        _documentRepository = documentRepository;
    }

    public async Task<IList<Document>> Handle(GetAllDocumentsQuery request, CancellationToken cancellationToken)
    {
        return await _documentRepository.GetAllDocumentsAsync();
    }
}

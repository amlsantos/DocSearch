using DocSearch.WebApi.Application.Common.Interfaces;
using DocSearch.WebApi.Domain.Entities;
using MediatR;

namespace DocSearch.WebApi.Application.Features.Retrieval.Queries;

public class GetAllDocumentsQueryHandler : IRequestHandler<GetAllDocumentsQuery, IList<Document>>
{
    private readonly IDocumentRepository _repository;

    public GetAllDocumentsQueryHandler(IDocumentRepository repository)
    {
        _repository = repository;
    }

    public async Task<IList<Document>> Handle(GetAllDocumentsQuery request, CancellationToken cancellationToken)
    {
        // Reusing the existing GetAllDocumentsAsync method from the repository
        return await _repository.GetAllDocumentsAsync();
    }
}

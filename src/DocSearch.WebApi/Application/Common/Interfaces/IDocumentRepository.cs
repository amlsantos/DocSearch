using DocSearch.WebApi.Domain.Entities;

namespace DocSearch.WebApi.Application.Common.Interfaces;

public interface IDocumentRepository
{
    Task ReplaceAllDocumentsAsync(IEnumerable<Document> documents, CancellationToken cancellationToken = default);
    
    Task<IList<Document>> GetAllDocumentsAsync();
}
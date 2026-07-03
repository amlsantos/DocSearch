using DocSearch.WebApi.Domain.Entities;

namespace DocSearch.WebApi.Application.Common.Interfaces;

public interface IDocumentRepository
{
    Task<IList<Document>> GetAllDocumentsAsync();
    Task InsertDocumentsAsync(IEnumerable<Document> documents, CancellationToken cancellationToken = default);
    Task<IList<(DocumentChunk Chunk, float Rank)>> RetrieveChunksAsync(string searchTerm, CancellationToken cancellationToken = default);
}
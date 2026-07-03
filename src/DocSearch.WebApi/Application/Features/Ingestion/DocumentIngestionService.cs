using DocSearch.WebApi.Application.Common.Interfaces;
using DocSearch.WebApi.Domain.Entities;

namespace DocSearch.WebApi.Application.Features.Ingestion;

public class DocumentIngestionService
{
    private readonly IDocumentReader _documentReader;
    private readonly IDocumentRepository _documentRepository;
    
    
    public DocumentIngestionService(IDocumentReader documentReader, IDocumentRepository documentRepository)
    {
        _documentReader = documentReader;
        _documentRepository = documentRepository;
    }
    
    public async Task IngestDocsAsync(string directoryPath, CancellationToken cancellationToken = default)
    {
        var documents = await _documentReader.ReadFromDirectoryAsync(directoryPath, cancellationToken);
        
        await _documentRepository.ReplaceAllDocumentsAsync(documents, cancellationToken);
    }

    public async Task<IList<Document>> GetAllDocuments()
    {
        return await _documentRepository.GetAllDocumentsAsync();
    }
}
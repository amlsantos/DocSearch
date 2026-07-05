using DocSearch.WebApi.Application.Common.Interfaces;
using DocSearch.WebApi.Domain.Entities;
using MediatR;

namespace DocSearch.WebApi.Application.Features.Ingestion.Commands;

public class IngestDocumentsCommandHandler : IRequestHandler<IngestDocumentsCommand, IList<Document>>
{
    private readonly IDocumentReader _documentReader;
    private readonly IDocumentRepository _documentRepository;

    public IngestDocumentsCommandHandler(IDocumentReader documentReader, IDocumentRepository documentRepository)
    {
        _documentReader = documentReader;
        _documentRepository = documentRepository;
    }

    public async Task<IList<Document>> Handle(IngestDocumentsCommand request, CancellationToken cancellationToken)
    {
        var ingestedDocuments = new List<Document>();
        
        // Step 1: Get documents that already exist in the database
        var existingDocs = await _documentRepository.GetAllDocumentsAsync();
        var existingKeys = existingDocs
            .Select(d => d.Key())
            .ToHashSet();
        
        // Step 2: Get all .md files from disk and filter only the NEW ones
        var allFiles = Directory.GetFiles(request.DirectoryPath, "*.md", SearchOption.AllDirectories);
        var newFilePaths = allFiles
            .Where(filePath =>
            {
                var fileName = Path.GetFileName(filePath);
                var lastModified = File.GetLastWriteTimeUtc(filePath);
                var tempDoc = new Document(fileName, filePath, lastModified);
                return !existingKeys.Contains(tempDoc.Key());
            })
            .ToList();
        
        // Step 3: If there are no new files, we're done
        if (!newFilePaths.Any()) 
            return ingestedDocuments;
        
        // Step 4 & 5: Read the new files one by one (streaming) and persist in batches
        var batch = new List<Document>();
        const int batchSize = 100;
        
        await foreach (var document in _documentReader.ReadFilesAsync(newFilePaths, cancellationToken))
        {
            batch.Add(document);
            ingestedDocuments.Add(document);
            
            if (batch.Count >= batchSize)
            {
                await _documentRepository.InsertDocumentsAsync(batch, cancellationToken);
                batch.Clear();
            }
        }
        
        // Persist any remaining documents in the last batch
        if (batch.Any())
        {
            await _documentRepository.InsertDocumentsAsync(batch, cancellationToken);
        }
        
        return ingestedDocuments;
    }
}

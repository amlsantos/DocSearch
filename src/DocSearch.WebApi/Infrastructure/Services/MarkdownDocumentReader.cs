using DocSearch.WebApi.Application.Common.Interfaces;
using DocSearch.WebApi.Domain.Entities;

namespace DocSearch.WebApi.Infrastructure.Services;

public class MarkdownDocumentReader : IDocumentReader
{
    public async Task<IEnumerable<Document>> ReadFromDirectoryAsync(string directoryPath, CancellationToken cancellationToken = default)
    {
        var documents = new List<Document>();
        
        var files = Directory.GetFiles(directoryPath, "*.md", SearchOption.AllDirectories);
        foreach (var filePath in files)
        {
            var fileName = Path.GetFileName(filePath);
            var document = new Document(fileName, filePath);
            var content = await File.ReadAllTextAsync(filePath, cancellationToken);
            
            // Chunking strategy: Split the text by double newlines to separate paragraphs/sections
            // Remove empty entries to ensure we don't process blank chunks
            var chunks = content.Split(new[] { "\n\n", "\r\n\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var chunk in chunks)
            {
                // Clean up extra whitespaces and ignore if the chunk is empty after trimming
                var trimmedChunk = chunk.Trim();
                if (!string.IsNullOrEmpty(trimmedChunk))
                {
                    document.AddChunk(trimmedChunk);
                }
            }
            
            // Only add the document if it contains valid content chunks
            if (document.Chunks.Any())
            {
                documents.Add(document);
            }
        }
        
        return documents;
    }
}
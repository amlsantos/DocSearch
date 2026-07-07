using System.Runtime.CompilerServices;
using DocSearch.WebApi.Application.Common.Interfaces;
using DocSearch.WebApi.Domain.Entities;

namespace DocSearch.WebApi.Infrastructure.Services;

public class MarkdownDocumentReader : IDocumentReader
{
    public async IAsyncEnumerable<Document> ReadFilesAsync(IEnumerable<string> filePaths, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        foreach (var filePath in filePaths)
        {
            var fileName = Path.GetFileName(filePath);
            var lastModified = File.GetLastWriteTimeUtc(filePath);
            
            var document = new Document(fileName, filePath, lastModified);
            var content = await File.ReadAllTextAsync(filePath, cancellationToken);
            
            // Chunking strategy: Split the text by double newlines to separate paragraphs/sections
            var paragraphChunks = content.Split(new[] { "\n\n", "\r\n\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var paragraph in paragraphChunks)
            {
                var trimmedChunk = paragraph.Trim();
                if (!string.IsNullOrEmpty(trimmedChunk))
                {
                    document.AddChunk(trimmedChunk);
                }
            }
            
            if (document.Chunks.Any())
            {
                yield return document;
            }
        }
    }
}
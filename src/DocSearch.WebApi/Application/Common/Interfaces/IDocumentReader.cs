using DocSearch.WebApi.Domain.Entities;

namespace DocSearch.WebApi.Application.Common.Interfaces;

public interface IDocumentReader
{
    IAsyncEnumerable<Document> ReadFilesAsync(IEnumerable<string> filePaths, CancellationToken cancellationToken = default);
}
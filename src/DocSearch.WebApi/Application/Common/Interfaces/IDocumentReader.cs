using DocSearch.WebApi.Domain.Entities;

namespace DocSearch.WebApi.Application.Common.Interfaces;

public interface IDocumentReader
{
    Task<IEnumerable<Document>> ReadFromDirectoryAsync(string directoryPath, CancellationToken cancellationToken = default);
}
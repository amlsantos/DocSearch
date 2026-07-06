using DocSearch.WebApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DocSearch.Tests.Infrastructure.Persistence;

/// <summary>
/// A test-specific repository that uses TestDbContext instead of AppDbContext.
/// This avoids loading Npgsql-specific assemblies in the test environment.
/// </summary>
public class TestDocumentRepository
{
    private readonly TestDbContext _context;

    public TestDocumentRepository(TestDbContext context)
    {
        _context = context;
    }

    public async Task<IList<Document>> GetAllDocumentsAsync()
    {
        return await _context.Documents.ToListAsync();
    }

    public async Task InsertDocumentsAsync(IEnumerable<Document> documents, CancellationToken cancellationToken = default)
    {
        await _context.Documents.AddRangeAsync(documents, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
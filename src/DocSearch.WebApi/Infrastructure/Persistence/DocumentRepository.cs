using DocSearch.WebApi.Application.Common.Interfaces;
using DocSearch.WebApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DocSearch.WebApi.Infrastructure.Persistence;

public class DocumentRepository : IDocumentRepository
{
    private readonly AppDbContext _context;
    
    public DocumentRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task ReplaceAllDocumentsAsync(IEnumerable<Document> documents, CancellationToken cancellationToken = default)
    {
        // 1. Clear existing data to allow clean re-ingestion
        await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"DocumentChunks\", \"Documents\" CASCADE", cancellationToken);
        // 2. Insert new documents (EF Core automatically handles chunk insertion via navigation properties)
        await _context.Documents.AddRangeAsync(documents, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IList<Document>> GetAllDocumentsAsync()
    {
        return await _context.Documents.ToListAsync();
    }
}
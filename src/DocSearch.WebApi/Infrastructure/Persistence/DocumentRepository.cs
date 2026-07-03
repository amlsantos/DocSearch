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
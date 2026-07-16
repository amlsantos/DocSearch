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
    
    public async Task<IList<(DocumentChunk Chunk, float Rank)>> RetrieveChunksAsync(string searchTerm, int limit = 5, CancellationToken cancellationToken = default)
    {
        var results = await _context.DocumentChunks
            .Where(c => EF.Property<NpgsqlTypes.NpgsqlTsVector>(c, "SearchVector")
                         .Matches(EF.Functions.WebSearchToTsQuery("english", searchTerm)))
            .Select(c => new 
            {
                Chunk = c,
                Rank = EF.Property<NpgsqlTypes.NpgsqlTsVector>(c, "SearchVector")
                        .Rank(EF.Functions.WebSearchToTsQuery("english", searchTerm))
            })
            .OrderByDescending(x => x.Rank)
            .Take(limit)
            .ToListAsync(cancellationToken);

        // Convert the anonymous type to the required Tuple type
        return results.Select(x => (x.Chunk, x.Rank)).ToList();
    }
}
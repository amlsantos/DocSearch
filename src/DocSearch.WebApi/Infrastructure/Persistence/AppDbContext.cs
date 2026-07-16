using DocSearch.WebApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DocSearch.WebApi.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public DbSet<Document> Documents { get; set; }
    public DbSet<DocumentChunk> DocumentChunks { get; set; }
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Configure Document entity
        modelBuilder.Entity<Document>(entity =>
        {
            entity.ToTable("Documents");
            entity.HasKey(d => d.Id);
            entity.Property(d => d.FileName).IsRequired().HasMaxLength(255);
            
            // A Document has many Chunks. Configure one-to-many relationship.
            entity.HasMany(d => d.Chunks)
                .WithOne() // No navigation property required in DocumentChunk
                .HasForeignKey(c => c.DocumentId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete chunks if document is removed
        });
        
        // Configure DocumentChunk entity
        modelBuilder.Entity<DocumentChunk>(entity =>
        {
            entity.ToTable("DocumentChunks");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Content).IsRequired();
            entity.Property(c => c.OrderIndex).IsRequired();
            
            // Full-Text Search Vector configured as a Shadow Property
            // This keeps the Domain layer pure (no Npgsql dependencies in DocumentChunk.cs)
            if (Database.IsNpgsql())
            {
                entity.Property<NpgsqlTypes.NpgsqlTsVector>("SearchVector")
                    .HasComputedColumnSql("to_tsvector('english', \"Content\")", stored: true);

                entity.HasIndex("SearchVector")
                    .HasMethod("GIN");
            }
        });
    }
}
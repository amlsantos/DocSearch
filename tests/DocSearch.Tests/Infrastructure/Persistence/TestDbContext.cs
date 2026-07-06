using DocSearch.WebApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DocSearch.Tests.Infrastructure.Persistence;

/// <summary>
/// A test-specific DbContext that mirrors AppDbContext's entity configuration
/// but removes PostgreSQL-specific features (tsvector, GIN index) that are 
/// not supported by the InMemory provider.
/// </summary>
public class TestDbContext : DbContext
{
    public DbSet<Document> Documents { get; set; }
    public DbSet<DocumentChunk> DocumentChunks { get; set; }

    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Document>(entity =>
        {
            entity.ToTable("Documents");
            entity.HasKey(d => d.Id);
            entity.Property(d => d.FileName).IsRequired().HasMaxLength(255);

            entity.HasMany(d => d.Chunks)
                .WithOne()
                .HasForeignKey(c => c.DocumentId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<DocumentChunk>(entity =>
        {
            entity.ToTable("DocumentChunks");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Content).IsRequired();
            entity.Property(c => c.OrderIndex).IsRequired();
            // No SearchVector shadow property — InMemory does not support tsvector
        });
    }
}
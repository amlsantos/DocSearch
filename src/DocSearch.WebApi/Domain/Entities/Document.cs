namespace DocSearch.WebApi.Domain.Entities;

public class Document
{
    public Guid Id { get; private set; }
    public string FileName { get; private set; }
    public string SourcePath { get; private set; }
    public DateTime LastModifiedUtc { get; private set; }
    
    private readonly List<DocumentChunk> _chunks = new();
    public IReadOnlyCollection<DocumentChunk> Chunks => _chunks.AsReadOnly();
    
    public Document(string fileName, string sourcePath, DateTime lastModifiedUtc)
    {
        if (string.IsNullOrWhiteSpace(fileName)) 
            throw new ArgumentException("The name of the file is mandatory", nameof(fileName));
            
        Id = Guid.NewGuid();
        FileName = fileName;
        SourcePath = sourcePath;
        LastModifiedUtc = lastModifiedUtc;
    }
    
    public void AddChunk(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new InvalidOperationException($"The content of the chunk can not be empty");
            // Business rule
            
        var chunk = new DocumentChunk(this.Id, content.Trim(), _chunks.Count);
        _chunks.Add(chunk);
    }
}
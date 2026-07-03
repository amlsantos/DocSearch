namespace DocSearch.WebApi.Domain.Entities;

public class DocumentChunk
{
    public Guid Id { get; private set; }
    
    // the id of the .md file. One file can have multiple chunks
    public Guid DocumentId { get; private set; }
    
    public string Content { get; private set; }
    private const int MaxContentLenght = 8000;
    
    // the index order in which this specific chunk appears in the document
    public int OrderIndex { get; private set; } 
    
    internal DocumentChunk(Guid documentId, string content, int orderIndex)
    {
        // Rule 1: THe order needs to be positive
        if (orderIndex < 0)
            throw new ArgumentOutOfRangeException(nameof(orderIndex), "The OrderIndex needs to be positive");
        
        // Rule 2: THe content of the chunk can not be bigger than MAX_CONTENT
        if (content.Length > MaxContentLenght)
            throw new ArgumentOutOfRangeException(nameof(content), $"The content of the chunk can not be bigger than {MaxContentLenght}");
        
        Id = Guid.NewGuid();
        DocumentId = documentId;
        Content = content;
        OrderIndex = orderIndex;
    }
}
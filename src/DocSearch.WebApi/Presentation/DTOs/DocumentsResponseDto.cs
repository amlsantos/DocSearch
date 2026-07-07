using System;

namespace DocSearch.WebApi.Presentation.DTOs;

public class DocumentsResponseDto
{
    // A little subset of the Chunk
    public Guid ChunkId { get; set; }
    public string Content { get; set; } = string.Empty;
    public int OrderIndex { get; set; }
    
    // We need to fetch the Document FileName as well to know where it came from
    public Guid DocumentId { get; set; }
    
    // The ranking score calculated by PostgreSQL
    public float Rank { get; set; }
}

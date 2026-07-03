using System;

namespace DocSearch.WebApi.Presentation.DTOs;

public class DocumentDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string SourcePath { get; set; } = string.Empty;
    public DateTime LastModifiedUtc { get; set; }
}

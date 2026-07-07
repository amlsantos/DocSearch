namespace DocSearch.WebApi.Presentation.DTOs;

public class SearchResponseDto
{
    public string Answer { get; set; } = default!;
    public List<DocumentsResponseDto> Sources { get; set; } = new List<DocumentsResponseDto>();
}

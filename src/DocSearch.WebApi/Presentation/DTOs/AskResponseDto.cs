namespace DocSearch.WebApi.Presentation.DTOs;

public class AskResponseDto
{
    public string Answer { get; set; } = default!;
    public List<SearchResultDto> Sources { get; set; } = new List<SearchResultDto>();
}

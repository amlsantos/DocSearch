namespace DocSearch.WebApi.Presentation.DTOs;

public class SearchRequestDto
{
    public string Question { get; set; } = default!;
    public int Limit { get; set; } = 5;
}

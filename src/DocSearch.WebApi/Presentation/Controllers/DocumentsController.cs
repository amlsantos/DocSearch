using DocSearch.WebApi.Application.Features.Ingestion.Commands;
using DocSearch.WebApi.Application.Features.Ingestion.Queries;
using DocSearch.WebApi.Application.Features.Retrieval.Queries;
using DocSearch.WebApi.Presentation.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DocSearch.WebApi.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DocumentsController : ControllerBase
{
    private readonly ISender _sender;
    
    // Dependency Injection comes through the constructor
    public DocumentsController(ISender sender)
    {
        _sender = sender;
    }
    
    [HttpPost("ingest")]
    public async Task<IActionResult> IngestDocuments()
    {
        try
        {
            // Resolve the path to the docs folder (which is two levels up from the WebApi project root: src -> root -> docs)
            var docsPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "docs"); 
            var command = new IngestDocumentsCommand(docsPath);
            
            var response = await _sender.Send(command);
            
            var newDocumentsDto = response.Select(d => new DocSearch.WebApi.Presentation.DTOs.DocumentDto
            {
                Id = d.Id,
                FileName = d.FileName,
                SourcePath = d.SourcePath,
                LastModifiedUtc = d.LastModifiedUtc
            }).ToList();
            var dto = new { message = "Documents ingested successfully.", newDocuments = newDocumentsDto };
            
            return Ok(dto);
        }
        catch (Exception ex)
        {
            // Simple error handling for now
            return StatusCode(500, new { error = "Failed to ingest documents.", details = ex.Message });
        }
    }
    
    [HttpGet("retrieve")]
    public async Task<IActionResult> RetrieveDocuments([FromQuery] string question)
    {
        if (string.IsNullOrWhiteSpace(question))
        {
            return BadRequest(new { error = "Search term 'q' cannot be empty." });
        }

        var query = new RetrieveDocumentsQuery(question);
        var searchResults = await _sender.Send(query);

        var dto = searchResults.Select(result => new SearchResultDto
        {
            ChunkId = result.Chunk.Id,
            DocumentId = result.Chunk.DocumentId,
            Content = result.Chunk.Content,
            OrderIndex = result.Chunk.OrderIndex,
            Rank = result.Rank
        }).ToList();

        return Ok(dto);
    }
    
    [HttpPost("ask")]
    public async Task<IActionResult> Ask([FromBody] AskRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Question))
        {
            return BadRequest(new { error = "Question cannot be empty." });
        }

        var query = new AskQuestionQuery(request.Question);
        var result = await _sender.Send(query);

        var dto = new AskResponseDto
        {
            Answer = result.Answer,
            Sources = result.Sources.Select(s => new SearchResultDto
            {
                ChunkId = s.Chunk.Id,
                DocumentId = s.Chunk.DocumentId,
                Content = s.Chunk.Content,
                OrderIndex = s.Chunk.OrderIndex,
                Rank = s.Rank
            }).ToList()
        };

        return Ok(dto);
    }
}
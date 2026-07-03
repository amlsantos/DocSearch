using DocSearch.WebApi.Application.Features.Ingestion;
using Microsoft.AspNetCore.Mvc;

namespace DocSearch.WebApi.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DocumentsController : ControllerBase
{
    private readonly DocumentIngestionService _ingestionService;
    
    // Dependency Injection comes through the constructor
    public DocumentsController(DocumentIngestionService ingestionService)
    {
        _ingestionService = ingestionService;
    }
    
    [HttpPost("ingest")]
    public async Task<IActionResult> IngestDocuments()
    {
        // Resolve the path to the docs folder (which is two levels up from the WebApi project root: src -> root -> docs)
        var docsPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "docs"); 
        
        try
        {
            await _ingestionService.IngestDocsAsync(docsPath);
            return Ok(new { message = "Documents ingested successfully." });
        }
        catch (Exception ex)
        {
            // Simple error handling for now
            return StatusCode(500, new { error = "Failed to ingest documents.", details = ex.Message });
        }
    }

    [HttpGet("getAll")]
    public async Task<IActionResult> GetAllDocuments()
    {
        var existingDocuments = await _ingestionService.GetAllDocuments();
        
        return Ok(existingDocuments);
    }
}
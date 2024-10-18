using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;
using ComplianceCalendar.Models.DTO;
using ComplianceCalendar.Services;
using ComplianceCalendar.Models;
using ComplianceCalendar.models;
using ComplianceCalendar.Repository.IRepository;
using Microsoft.Extensions.Logging;

namespace ComplianceCalendar.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentService _documentService;
        private readonly IDocumentRepository _documentRepository;
        private readonly ILogger<DocumentController> _logger;

        public DocumentController(IDocumentService documentService, IDocumentRepository documentRepository, ILogger<DocumentController> logger)
        {
            _documentService = documentService;
            _documentRepository = documentRepository;
            _logger = logger;
        }

        [HttpPost("/Document/AddDocument")]
        public async Task<IActionResult> AddDocument([FromForm] AddDocumentDTO addDocumentDTO)
        {
            _logger.LogInformation("AddDocument endpoint called");

            var response = await _documentService.AddDocumentAsync(addDocumentDTO);
            if (response.isSuccess)
            {
                _logger.LogInformation("Document added successfully");
                return Ok(response);
            }

            _logger.LogWarning("Failed to add document");
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("/Document/download/{filingId}")]
        public async Task<IActionResult> DownloadDocument(int filingId)
        {
            _logger.LogInformation($"DownloadDocument endpoint called with filingId: {filingId}");

            try
            {
                var fileBytes = await _documentService.DownloadDocumentAsync(filingId);
                string fileName = Path.GetFileName(await GetDocumentPathFromDatabase(filingId)); // Use the file name from the path
                _logger.LogInformation("Document downloaded successfully");
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error downloading document: {ex.Message}");
                return StatusCode(500, new APIResponse { isSuccess = false, Result = $"Error downloading document: {ex.Message}", StatusCode = HttpStatusCode.InternalServerError });
            }
        }

        [HttpDelete("/Document/DeleteDocument/{documentId}")]
        public async Task<IActionResult> DeleteDocument(int documentId)
        {
            _logger.LogInformation($"DeleteDocument endpoint called with documentId: {documentId}");

            var response = await _documentService.DeleteDocumentAsync(documentId);
            if (response.isSuccess)
            {
                _logger.LogInformation("Document deleted successfully");
                return Ok(response);
            }

            _logger.LogWarning("Failed to delete document");
            return StatusCode((int)response.StatusCode, response);
        }

        private async Task<string> GetDocumentPathFromDatabase(int documentId)
        {
            var document = await _documentRepository.GetAsync(documentId);
            return document.DocumentLink;
        }
    }
}

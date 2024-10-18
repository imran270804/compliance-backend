using ComplianceCalendar.Data;
using ComplianceCalendar.Models.DTO;
using ComplianceCalendar.Repositories;
using ComplianceCalendar.Repository.IRepository;
using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ComplianceCalendar.Controllers
{
    [Route("Filings/[controller]")]
    [ApiController]
    public class UploadFilingsController : ControllerBase
    {
        private readonly IUploadFilingsRepository _repository;
        private readonly ILogger<UploadFilingsController> _logger;

        public UploadFilingsController(IUploadFilingsRepository repository, ILogger<UploadFilingsController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpPost("upload")]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> UploadExcel(CancellationToken ct)
        {
            if (!Request.HasFormContentType || Request.Form.Files.Count == 0)
            {
                return BadRequest(new { message = "No file uploaded or invalid content type. Please ensure the Content-Type is multipart/form-data." });
            }

            var file = Request.Form.Files[0];

            // Retrieve createdById from the form data
            var createdByIdStr = Request.Form["createdById"].ToString();
            if (!int.TryParse(createdByIdStr, out int createdById))
            {
                return BadRequest(new { message = "Invalid createdById value." });
            }

            try
            {
                var result = await _repository.ProcessUploadAsync(file, createdById, ct);

                return result ? Ok(new { message = "File uploaded successfully and data updated." }) :
                                BadRequest(new { message = "Error uploading file." });
            }
            catch (DbUpdateException dbEx)
            {
                if (dbEx.InnerException is PostgresException pgEx && pgEx.SqlState == "23503")
                {
                    _logger.LogError("Foreign key constraint violation: {Message}", pgEx.Message);
                    return StatusCode(StatusCodes.Status400BadRequest, new { message = "Error uploading file: Foreign key constraint violation." });
                }
                else
                {
                    _logger.LogError(dbEx, "Error uploading file: Database update exception");
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Error uploading file: {ex.Message}" });
            }
        }
    }
}

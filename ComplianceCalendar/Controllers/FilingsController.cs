using AutoMapper;
using ComplianceCalendar.Models;
using ComplianceCalendar.Models.DTO;
using ComplianceCalendar.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ComplianceCalendar.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class FilingsController : ControllerBase
    {
        private readonly IFilingsService _filingsService;
        private readonly IFilingsRepository _filingsRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<FilingsController> _logger;

        public FilingsController(IFilingsService filingsService, IMapper mapper, ILogger<FilingsController> logger, IFilingsRepository filingsRepository)
        {
            _filingsService = filingsService;
            _mapper = mapper;
            _logger = logger;
            _filingsRepository = filingsRepository;
        }

        [HttpGet("GetAdminFilings/{employeeId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetAdminFilings(int employeeId, int year)
        {
            _logger.LogInformation($"GetAdminFilings called with employeeId: {employeeId}, year: {year}");
            var filingsOfAdmin = await _filingsService.GetAdminFilingsAsync(employeeId, year);
            return Ok(filingsOfAdmin);
        }

        [HttpGet("GetUserFilings/{employeeId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetUserFilings(int employeeId, int year)
        {
            _logger.LogInformation($"GetUserFilings called with employeeId: {employeeId}, year: {year}");

            if (year < 2000 || year > 2100)
            {
                _logger.LogWarning($"Invalid year specified: {year}");
                return BadRequest("Invalid year specified.");
            }

            var filingsOfUser = await _filingsService.GetUserFilingsAsync(employeeId, year);
            return Ok(filingsOfUser);
        }

        [HttpPost("AddFilings")]
        public async Task<IActionResult> AddFilings(AddFilingsDTO addFilingsDTO)
        {
            try
            {
                bool result = await _filingsService.AddFilingsAsync(addFilingsDTO);
                if (result)
                {
                    return Ok("Filings added successfully");
                }
                else
                {
                    return StatusCode(500, "Failed to add filings");
                }
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (DbUpdateException dbEx)
            {
                var innerMessage = dbEx.InnerException != null ? dbEx.InnerException.Message : dbEx.Message;
                return StatusCode(500, $"Database update error: {innerMessage}");
            }
            catch (Exception ex)
            {
                // Log generic errors
                return StatusCode(500, $"An error occurred while adding user filings: {ex.Message}");
            }
        }

        [HttpGet("GetDepartmentEmployeeDTO")]
        public async Task<ActionResult<IEnumerable<DepartmentEmployeeDTO>>> GetDepartmentEmployeeDTO()
        {
            _logger.LogInformation("GetDepartmentEmployeeDTO called");

            try
            {
                var departmentWithEmployees = await _filingsService.GetDepartmentEmployeeDTOAsync();
                if (departmentWithEmployees == null || !departmentWithEmployees.Any())
                {
                    _logger.LogWarning("No departments found");
                    return NotFound("No departments found");
                }

                return Ok(departmentWithEmployees);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while retrieving department employees: {ex.Message}");
                return StatusCode(500, $"An error occurred while retrieving department employees: {ex.Message}");
            }
        }

        [HttpPost("review/{filingId}")]
        public async Task<IActionResult> ReviewFiling(int filingId, [FromBody] ReviewDTO reviewDTO)
        {
            _logger.LogInformation($"ReviewFiling called with filingId: {filingId}");

            if (reviewDTO == null)
            {
                _logger.LogWarning("Review data is required.");
                return BadRequest("Review data is required.");
            }

            try
            {
                var result = await _filingsService.ReviewFilingAsync(filingId, reviewDTO);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning($"Filing not found: {ex.Message}");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal server error: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("reassign/{filingId}")]
        public async Task<IActionResult> ReassignFiling(int filingId, [FromBody] ReassignFilingsDTO reassignFilingsDTO, int employeeId)
        {
            _logger.LogInformation($"ReassignFiling called with filing Id : {filingId}");
            if (reassignFilingsDTO == null)
            {
                _logger.LogWarning("Data is required.");
                return BadRequest("Data is required.");
            }

            try
            {
                var result = await _filingsRepository.ReassignFilingAsync(filingId, reassignFilingsDTO, employeeId);
                return Ok(result); 
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning($"Filing not found: {ex.Message}");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal server error: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}

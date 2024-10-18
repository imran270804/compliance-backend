using AutoMapper;
using ComplianceCalendar.Data;
using ComplianceCalendar.Models;
using ComplianceCalendar.Models.DTO;
using ComplianceCalendar.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComplianceCalendar.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class AddFilingsController : ControllerBase
    {
        private readonly APIContext _context;
        private readonly IMapper _mapper;
        private readonly IFilingsService _filingsService;

        public AddFilingsController(APIContext context, IMapper mapper, IFilingsService filingsService)
        {
            _context = context;
            _mapper = mapper;
            _filingsService = filingsService;
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
            try
            {
                var departmentWithEmployees = await _context.Departments
                    .Select(d => new DepartmentEmployeeDTO
                    {
                        Id = d.Id,
                        DepName = d.DepName,
                        Employees = _context.Employees
                            .Where(e => e.DepId == d.Id)
                            .Select(e => new Employee
                            {
                                EmployeeId = e.EmployeeId,
                                EmpName = e.EmpName
                            })
                            .ToList()
                    })
                    .ToListAsync();

                if (departmentWithEmployees == null || !departmentWithEmployees.Any())
                {
                    return NotFound("No departments found");
                }

                return Ok(departmentWithEmployees);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving department employees: {ex.Message}");
            }
        }
    }
}

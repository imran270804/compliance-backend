using ComplianceCalendar.Data;
using ComplianceCalendar.Models;
using ComplianceCalendar.Models.DTO;
using ComplianceCalendar.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace ComplianceCalendar.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class ManageUsersController : ControllerBase
    {
        private readonly APIContext _context;
        private readonly ManageUserContext _manageUserContext;
        private readonly IUsersRepository _repository;
        private readonly ILogger<ManageUsersController> _logger; 

        public ManageUsersController(APIContext context, ManageUserContext manageUserContext,IUsersRepository repository, ILogger<ManageUsersController> logger)
        {
            _context = context;
            _manageUserContext = manageUserContext;
            _repository = repository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetUsers()
        {
            _logger.LogInformation("GetUsers called");
            var users = _repository.GetUsersAsync();
            if (users == null)
            {
                _logger.LogWarning("No users found.");
                return NotFound("No users found.");
            }

            return Ok(users.Value);
        }

        [HttpGet("{id}")]
        public ActionResult<IEnumerable<object>> GetUsers(int id)
        {
            _logger.LogInformation($"GetUsers called with id: {id}");
            var users = _repository.GetUsersAsync(id);
            if (User == null)
            {
                _logger.LogWarning($"No Users found with id: {id}");
                return NotFound("No Users found");
            }
            return Ok(users.Value);
        }

        [HttpPut("{id}")]
        public IResult UpdateStatus(int id, int adminId)
        {
            _logger.LogInformation($"UpdateStatus called with id: {id}");
            return _repository.UpdateStatusAsync(id, adminId);
            
        }

        // Reserved API for Delete user from table if needed. For future reference.

        [HttpDelete("{id}")]
        public IResult Delete(int id)
        {
            _logger.LogInformation($"Delete called with id: {id}");
            return _repository.DeleteAsync(id);
        }


        [HttpGet("GetUsersByDepartment/{depName}")]
        public async Task<ActionResult<IEnumerable<object>>> GetUsersByDepartment(string depName)
        {
            _logger.LogInformation($"GetUsersByDepartment called with department name: {depName}");
            var depUsers = await _repository.GetUsersByDepartmentAsync(depName);
            if (depUsers==null)
            {
                _logger.LogWarning($"No users found for department: {depName}");
                return NotFound("No users found");
            }
            return Ok(depUsers.Value);

        }



        [HttpPost("AddUserToEmployeeTable")]
        public async Task<IActionResult> AddUserToEmployeeTable([FromBody] ActiveDirectoryUserDto userDto)
        {
            _logger.LogInformation("AddUserToEmployeeTable called");
            var result = await _repository.AddUserToEmployeeTableAsync(userDto);

            if (result.Result is NotFoundObjectResult)
            {
                _logger.LogWarning("Department not found.");
                return NotFound("Department not found");
            }
            _logger.LogInformation("User added to employee table successfully");
            return Ok(result.Value);
        }
    }
}

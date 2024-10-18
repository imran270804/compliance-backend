using AutoMapper;
using ComplianceCalendar.Data;
using ComplianceCalendar.Models;
using ComplianceCalendar.Models.DTO;
using ComplianceCalendar.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class UsersRepository : IUsersRepository
{
    private readonly APIContext _context;
    private readonly ManageUserContext _manageUserContext;
    private readonly IMapper _mapper;
    public UsersRepository(APIContext context, ManageUserContext manageUserContext,IMapper mapper)
    {
        _context = context;
        _manageUserContext = manageUserContext;
        _mapper = mapper;
    }
    
    public IResult DeleteAsync(int id)
    {
        var userToDelete = _manageUserContext.Employee.FirstOrDefault(user => user.EmployeeId == id);
        if (userToDelete != null)
        {
            _manageUserContext.Employee.Remove(userToDelete);
            _manageUserContext.SaveChanges();
            return Results.Ok(userToDelete);
        }
        else
        {
            return Results.NotFound("No user with Id:");
        }
    }

    public ActionResult<IEnumerable<object>> GetUsersAsync()
    {
        var employeeTableData = _context.Employees.Select(e => new
        {
            e.EmployeeId,
            e.EmpName,
            e.Email,
            e.DepId,
            e.RoleId,
            e.IsEnabled,
            DepartmentName = _context.Departments.Where(d => d.Id == e.DepId).Select(d => d.DepName).ToList(),
            Role = _context.Roles.Where(r => r.Id == e.RoleId).Select(e => e.Rolename).FirstOrDefault()
        }).ToList();

        return employeeTableData;
    }

    public ActionResult<IEnumerable<object>> GetUsersAsync(int id)
    {
        var departmentIds = _context.AssignedToDept
                                        .Where(e => e.EmpId == id)
                                        .Select(e => e.DepartmentId)
                                        .ToList();
        var employees = _context.Employees
                                .Where(e => departmentIds.Contains(e.DepId) && e.RoleId != 3)
                                .Select(e => new
                                {
                                    e.EmployeeId,
                                    e.EmpName,
                                    e.Email,
                                    e.DepId,
                                    e.RoleId,
                                    e.IsEnabled,
                                    DepartmentName = _context.Departments.Where(d => d.Id == e.DepId).Select(d => d.DepName).ToList(),
                                    Role = _context.Roles.Where(r => r.Id == e.RoleId).Select(e => e.Rolename).FirstOrDefault()
                                })
                                .ToList();
        return employees;
    }

    public async Task<ActionResult<IEnumerable<object>>> GetUsersByDepartmentAsync(string depName)
    {
        var existingEmployeeEmails = await _context.Employees
                .Select(e => e.Email)
                .ToListAsync();

        // Get the users from Active Directory that are in the selected department and not in the Employee table
        var depUsers = await _context.ActiveDirectory
            .Where(user => user.Department == depName && !existingEmployeeEmails.Contains(user.Email))
            .ToListAsync();

        return depUsers;
    }

    public IResult UpdateStatusAsync(int id, int adminId)
    {
        var userToUpdate = _manageUserContext.Employee.FirstOrDefault(user => user.EmployeeId == id);
        if (userToUpdate != null)
        {
            userToUpdate.IsEnabled = !userToUpdate.IsEnabled;
            _manageUserContext.SaveChangesAsync();

            var employeeFilings = _context.UserFilings.Where(f => f.EmployeeId == id).ToList();
            foreach (var filing in employeeFilings)
            {
                filing.EmployeeId = adminId;
                _context.SaveChangesAsync();
            }
            return Results.Ok(userToUpdate);
        }
        else
        {
            return Results.NotFound("User not found..!");
        }
    }

    public async Task<ActionResult<Employee>> AddUserToEmployeeTableAsync(ActiveDirectoryUserDto userDto)
    {
        // Find the department by name
        var department = await _context.Departments
            .FirstOrDefaultAsync(d => d.DepName == userDto.DepartmentName);

        if (department == null)
        {
            return new NotFoundObjectResult("Department not found");
        }

        // Get the maximum EmployeeId currently in the table
        var maxEmployeeId = await _context.Employees
            .DefaultIfEmpty()
            .MaxAsync(e => e != null ? e.EmployeeId : 0);

        // Create a new Employee entity with the next available EmployeeId
        var newEmployee = new Employee
        {
            EmployeeId = maxEmployeeId + 1,
            EmpName = userDto.EmpName,
            Email = userDto.Email,
            DepId = department.Id,
            RoleId = 2, // Role ID is always 2
            CreatedDate = DateTime.UtcNow
        };

        // Add the new employee to the Employees table
        _context.Employees.Add(newEmployee);

        // Save changes to the database
        await _context.SaveChangesAsync();

        return new OkObjectResult(newEmployee);
    }


}
using AutoMapper;
using ComplianceCalendar.Data;
using ComplianceCalendar.Models;
using ComplianceCalendar.Models.DTO;
using Microsoft.EntityFrameworkCore;

public class AdminRepository : IAdminRepository
{
    private readonly AdminContext _context;
    private readonly IMapper _mapper;

    public AdminRepository(AdminContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<AdminDTO>> GetAdminsAsync()
    {
        return await _context.Employees
            .Include(e => e.Departments)
            .Where(e => e.RoleId == 1)
            .Select(e => new AdminDTO
            {
                EmployeeId = e.EmployeeId,
                EmpName = e.EmpName,
                Email = e.Email,
                DepartmentName = e.Departments.DepName,
                IsEnabled = e.IsEnabled
            })
            .ToListAsync();
    }
    public async Task<IEnumerable<Department>> GetDepartmentNamesByAdminIdAsync(int employeeId)
    {
        // Get the department IDs assigned to the employee
        var departmentIds = await _context.AssignedTo
            .Where(at => at.EmpId == employeeId)
            .Select(at => at.DepartmentId)
            .ToListAsync();

        // Get the departments based on the department IDs
        var departments = await _context.Departments
            .Where(d => departmentIds.Contains(d.Id))
            .Select(d => new Department
            { 
                Id = d.Id,
                DepName = d.DepName,
            })
            .ToListAsync();

        return departments;
    }


    public async Task<AdminDTO> DeleteAdminAsync(int id)
    {
        var employee = await _context.Employees.FindAsync(id);

        if (employee == null)
        {
            return null;
        }

        // Retrieve and delete AssignedTo record manually
        var assignedTo = await _context.AssignedTo.SingleOrDefaultAsync(at => at.EmpId == id);
        if (assignedTo != null)
        {
            _context.AssignedTo.Remove(assignedTo);
        }

        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();

        return new AdminDTO
        {
            EmployeeId = employee.EmployeeId,
            EmpName = employee.EmpName,
            Email = employee.Email,
            DepartmentName = employee.Departments?.DepName
        };
    }

    public async Task<AdminDTO> AddAdminAsync(AddAdminDTO model)
    {
        var maxEmployeeId = await _context.Employees.MaxAsync(e => (int?)e.EmployeeId) ?? 0;
        var nextEmployeeId = maxEmployeeId + 1;

        var department = await _context.Departments.FirstOrDefaultAsync(d => d.DepName == model.DepartmentName);
        if (department == null)
        {
            throw new Exception($"Department '{model.DepartmentName}' not found.");
        }

        var employee = _mapper.Map<Employee>(model);
        employee.EmployeeId = nextEmployeeId;
        employee.RoleId = 1;
        employee.DepId = department.Id;
        employee.CreatedDate = DateTime.UtcNow;

        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        var maxAssignedToId = await _context.AssignedTo.MaxAsync(a => (int?)a.Id) ?? 0;
        var nextAssignedToId = maxAssignedToId + 1;

        var assignedTo = new ComplianceCalendar.Models.AssignedTo
        {
            Id = nextAssignedToId,
            EmpId = employee.EmployeeId,
            DepartmentId = department.Id
        };
        _context.AssignedTo.Add(assignedTo);
        await _context.SaveChangesAsync();

        return new AdminDTO
        {
            EmployeeId = employee.EmployeeId,
            EmpName = employee.EmpName,
            Email = employee.Email,
            DepartmentName = department.DepName
        };
    }


    public async Task<IEnumerable<AdminDTO>> GetEmployeesByDepartmentIdAsync(int departmentId)
    {
        return await _context.Employees
            .Where(e => e.DepId == departmentId && e.RoleId != 3)
            .Select(e => new AdminDTO
            {
                EmployeeId = e.EmployeeId,
                EmpName = e.EmpName,
                Email = e.Email,
                DepartmentName = e.Departments.DepName
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<AdminDTO>> GetEmployeesByDepartmentNameAsync(string departmentName)
    {
        return await _context.Employees
            .Where(e => e.Departments.DepName == departmentName && e.RoleId != 3)
            .Select(e => new AdminDTO
            {
                EmployeeId = e.EmployeeId,
                EmpName = e.EmpName,
                Email = e.Email,
                DepartmentName = e.Departments.DepName
            })
            .ToListAsync();
    }

    public IResult UpdateAdminStatusAsync(int id)
    {
        var adminToUpdate = _context.Employees.FirstOrDefault(admin => admin.EmployeeId == id);
        if (adminToUpdate != null)
        {
            adminToUpdate.IsEnabled = !adminToUpdate.IsEnabled;
            _context.SaveChangesAsync();
            return Results.Ok(adminToUpdate);
        }
        else
        {
            return Results.NotFound("Admin not found..!");
        }
    }
}

using ComplianceCalendar.Data;
using ComplianceCalendar.Models;
using ComplianceCalendar.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ComplianceCalendar.Repository;

public class DepartmentRepository : IDepartmentRepository
{
    private readonly AdminContext _context;

    public DepartmentRepository(AdminContext context)
    {
        _context = context;
    }

    public async Task<List<Department>> GetAllDepartmentsAsync()
    {
        return await _context.Departments.ToListAsync();
    }

    public async Task<Department> GetDepartmentByIdAsync(int id)
    {
        return await _context.Departments.FindAsync(id);
    }
}

using ComplianceCalendar.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ComplianceCalendar.Repository.IRepository
{
    public interface IDepartmentRepository
    {
        Task<List<Department>> GetAllDepartmentsAsync();
        Task<Department> GetDepartmentByIdAsync(int id);
    }
}

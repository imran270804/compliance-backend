using ComplianceCalendar.Models;
using ComplianceCalendar.Models.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ComplianceCalendar.Services
{
    public interface IFilingsService
    {
        Task<IEnumerable<object>> GetAdminFilingsAsync(int employeeId, int year);
        Task<IEnumerable<object>> GetUserFilingsAsync(int employeeId, int year);
        Task<bool> AddFilingsAsync(AddFilingsDTO addFilingsDTO);
        Task<IEnumerable<DepartmentEmployeeDTO>> GetDepartmentEmployeeDTOAsync();
        Task<string> ReviewFilingAsync(int filingId, ReviewDTO reviewDTO);
    }
}

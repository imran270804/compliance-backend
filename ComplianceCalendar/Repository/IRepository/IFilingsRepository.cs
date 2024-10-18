using ComplianceCalendar.Models.DTO;
using ComplianceCalendar.Models;

public interface IFilingsRepository
{
    Task<IEnumerable<object>> GetAdminFilingsAsync(int employeeId, int year);
    Task<IEnumerable<object>> GetUserFilingsAsync(int employeeId, int year);
    Task<Filings> GetFilingByIdAsync(int id);
    Task<bool> UpdateFilingStatusAsync(int id, UpdateFilingStatusDTO updateDTO);
    Task<bool> FilingExistsAsync(int id);
    Task<bool> AddUserFilingsAsync(IEnumerable<UserFilings> userFilings);
    Task<IEnumerable<DepartmentEmployeeDTO>> GetDepartmentEmployeeDTOsAsync(CancellationToken ct);
    Task<string> ReviewFilingAsync(int filingId,ReviewDTO reviewDTO);
    Task<bool> ReassignFilingAsync(int filingId, ReassignFilingsDTO reassignDTO, int employeeId);
}

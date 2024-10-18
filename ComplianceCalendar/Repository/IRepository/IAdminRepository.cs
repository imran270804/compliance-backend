using ComplianceCalendar.Models;
using ComplianceCalendar.Models.DTO;

public interface IAdminRepository
{
    Task<IEnumerable<AdminDTO>> GetAdminsAsync();
    Task<IEnumerable<Department>> GetDepartmentNamesByAdminIdAsync(int employeeId);
    Task<AdminDTO> DeleteAdminAsync(int id);
    Task<AdminDTO> AddAdminAsync(AddAdminDTO model);
    Task<IEnumerable<AdminDTO>> GetEmployeesByDepartmentIdAsync(int departmentId);
    Task<IEnumerable<AdminDTO>> GetEmployeesByDepartmentNameAsync(string departmentName);
    public IResult UpdateAdminStatusAsync(int id);

}

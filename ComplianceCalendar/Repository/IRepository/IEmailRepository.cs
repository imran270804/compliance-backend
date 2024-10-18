using ComplianceCalendar.Models;

namespace ComplianceCalendar.Repository.IRepository
{
    public interface IEmailRepository
    {
        Task<List<UserFilings>> GetUserFilingsByFilingIdAsync(int filingId);
        Task<List<Employee>> GetEmployeesByIdsAsync(List<int> employeeIds);
        Task<List<Filings>> GetPendingFilingsAsync();
        Task<List<Filings>> GetFilingsDueSoonAsync(DateTimeOffset startDate, DateTimeOffset endDate, string status);
    }
}

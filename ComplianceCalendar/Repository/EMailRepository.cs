using ComplianceCalendar.Data;
using ComplianceCalendar.Models;
using ComplianceCalendar.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace ComplianceCalendar.Repository
{
    public class EMailRepository : IEmailRepository
    {
        private readonly APIContext _dbContext;

        public EMailRepository(APIContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<UserFilings>> GetUserFilingsByFilingIdAsync(int filingId)
        {
            return await _dbContext.Set<UserFilings>()
                                   .Where(uf => uf.FilingId == filingId)
                                   .ToListAsync();
        }

        public async Task<List<Employee>> GetEmployeesByIdsAsync(List<int> employeeIds)
        {
            return await _dbContext.Set<Employee>()
                                   .Where(e => employeeIds.Contains(e.EmployeeId))
                                   .ToListAsync();
        }
        public async Task<List<Filings>> GetPendingFilingsAsync()
    {
        return await _dbContext.Set<Filings>()
                               .Where(f => f.Status == "Closed" && f.DocIsUploaded == false)
                               .ToListAsync();
    }
        public async Task<List<Filings>> GetFilingsDueSoonAsync(DateTimeOffset startDate, DateTimeOffset endDate, string status)
        {
            return await _dbContext.Set<Filings>()
                .Where(f => f.DueDate >= startDate && f.DueDate <= endDate && f.Status == status)
                .ToListAsync();
        }
    }
}

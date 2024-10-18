using ComplianceCalendar.Data;
using ComplianceCalendar.Models;
using ComplianceCalendar.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComplianceCalendar.Repository
{
    public class FilingSchedulerRepository : IFilingSchedulerRepository
    {
        private readonly APIContext _dbContext;

        public FilingSchedulerRepository(APIContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Filings>> GetOpenFilingsPastDueDateAsync(DateTime dueDate)
        {
            return await _dbContext.Filings
                .Where(f => f.DueDate < dueDate && f.Status == "Open")
                .ToListAsync();
        }

        public async Task UpdateFilingStatusAsync(Filings filing, string status)
        {
            filing.Status = status;
            _dbContext.Filings.Update(filing);
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}

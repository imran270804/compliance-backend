using ComplianceCalendar.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ComplianceCalendar.Repository.IRepository
{
    public interface IFilingSchedulerRepository
    {
        Task<List<Filings>> GetOpenFilingsPastDueDateAsync(DateTime dueDate);
        Task UpdateFilingStatusAsync(Filings filing, string status);
        Task SaveChangesAsync();
    }
}


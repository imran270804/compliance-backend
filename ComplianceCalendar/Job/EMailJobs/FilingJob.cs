using Quartz;
using System.Threading.Tasks;
using ComplianceCalendar.Services;

namespace ComplianceCalendar.Services
{
    public class FilingJob : IJob
    {
        private readonly IFilingMailService _filingService;

        public FilingJob(IFilingMailService filingService)
        {
            _filingService = filingService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await _filingService.ProcessUpcomingFilingsAsync();
        }
    }
}

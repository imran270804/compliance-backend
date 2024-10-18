using Quartz;
using System.Threading.Tasks;
using ComplianceCalendar.Services;

namespace ComplianceCalendar.Services
{
    public class PendingFilingsJob : IJob
    {
        private readonly IPendingFilingsService _pendingFilingsService;

        public PendingFilingsJob(IPendingFilingsService pendingFilingsService)
        {
            _pendingFilingsService = pendingFilingsService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await _pendingFilingsService.ProcessPendingFilingsAsync();
        }
    }
}

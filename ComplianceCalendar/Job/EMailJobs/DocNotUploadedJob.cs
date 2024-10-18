using Quartz;
using System.Threading.Tasks;
using ComplianceCalendar.Services;

namespace ComplianceCalendar.Services
{
    public class DocNotUploadedJob : IJob
    {
        private readonly IDocNotUploadedService _docNotUploadedService;

        public DocNotUploadedJob(IDocNotUploadedService docNotUploadedService)
        {
            _docNotUploadedService = docNotUploadedService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await _docNotUploadedService.ProcessDocNotUploadedAsync();
        }
    }
}

using Quartz;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ComplianceCalendar.Repository.IRepository;
using ComplianceCalendar.Repository;

namespace ComplianceCalendar.Services.Scheduler
{
    public class UpdateFilingStatusJob : IJob
    {
        private readonly IServiceProvider _serviceProvider;

        public UpdateFilingStatusJob(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var filingRepository = scope.ServiceProvider.GetRequiredService<IFilingSchedulerRepository>();

                var filings = await filingRepository.GetOpenFilingsPastDueDateAsync(DateTime.UtcNow);

                foreach (var filing in filings)
                {
                    await filingRepository.UpdateFilingStatusAsync(filing, "Pending");
                }

                await filingRepository.SaveChangesAsync();
            }
        }
    }
}

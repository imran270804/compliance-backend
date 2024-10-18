using Quartz.Spi;
using Quartz;
using Microsoft.Extensions.DependencyInjection;

namespace ComplianceCalendar.Services.Scheduler
{
    public class SchedulerJobFactory : IJobFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public SchedulerJobFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        { 

            var scope = _serviceProvider.CreateScope();
            var serviceProvider = scope.ServiceProvider;
            return serviceProvider.GetRequiredService(bundle.JobDetail.JobType) as IJob;
        }

        public void ReturnJob(IJob job) 
        { 
            
        }
    }
}
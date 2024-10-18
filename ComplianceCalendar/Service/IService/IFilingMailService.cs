using System.Threading.Tasks;

namespace ComplianceCalendar.Services
{
    public interface IFilingMailService
    {
        Task ProcessUpcomingFilingsAsync();
    }
}

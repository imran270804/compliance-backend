using System.Threading.Tasks;

namespace ComplianceCalendar.Services
{
    public interface IPendingFilingsService
    {
        Task ProcessPendingFilingsAsync();
    }
}

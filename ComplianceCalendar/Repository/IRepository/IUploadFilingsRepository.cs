using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ComplianceCalendar.Repositories
{
    public interface IUploadFilingsRepository
    {
        Task<bool> ProcessUploadAsync(IFormFile file, int createdById, CancellationToken ct);
    }
}

using ComplianceCalendar.models;
using ComplianceCalendar.Models.DTO;
using System.Threading.Tasks;

namespace ComplianceCalendar.Services
{
    public interface IDocumentService
    {
        Task<APIResponse> AddDocumentAsync(AddDocumentDTO addDocumentDTO);
        Task<byte[]> DownloadDocumentAsync(int filingId);
        Task<APIResponse> DeleteDocumentAsync(int documentId);
    }
}

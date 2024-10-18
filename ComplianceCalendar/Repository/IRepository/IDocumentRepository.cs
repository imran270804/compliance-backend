using ComplianceCalendar.Models;

namespace ComplianceCalendar.Repository.IRepository
{
    public interface IDocumentRepository
    {
        Task<Documents> GetAsync(int id);
        void Add(Documents document);
        void Delete(int id); // Add this method signature
        void Save();
    }
}

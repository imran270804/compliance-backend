using ComplianceCalendar.Data;
using ComplianceCalendar.Models;
using ComplianceCalendar.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ComplianceCalendar.Repository
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly APIContext _context;

        public DocumentRepository(APIContext context)
        {
            _context = context;
        }

        public async Task<Documents> GetAsync(int id)
        {
            return await _context.Documents
                                 .Where(f => f.FilingId == id)
                                 .OrderBy(d => d.Id) // Add a deterministic sort order here
                                 .LastOrDefaultAsync();
        }

        public void Add(Documents document)
        {
            _context.Documents.Add(document);
        }

        public void Delete(int id)
        {
            // Fetch the document to be deleted
            var document = _context.Documents.FirstOrDefault(d => d.FilingId == id);
            if (document != null)
            {
                // Fetch the corresponding filing record
                var filing = _context.Filings.FirstOrDefault(f => f.Id == document.FilingId);
                if (filing != null)
                {
                    // Set DocIsUploaded to false
                    filing.DocIsUploaded = false;
                }

                // Remove the document
                _context.Documents.Remove(document);

                // Save changes to the database
                _context.SaveChanges();
            }
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}

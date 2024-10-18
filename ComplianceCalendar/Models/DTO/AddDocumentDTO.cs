using System.ComponentModel.DataAnnotations.Schema;

namespace ComplianceCalendar.Models.DTO
{
    public class AddDocumentDTO
    {
        private int _filingId;
        private string _documentLink;
        [ForeignKey("Filings")]
        public int FilingId {
            get { return _filingId; }
            set { _filingId = value; } 
        }
        public IFormFile File { get; set; }
        public string DocumentLink {
            get { return _documentLink; }
            set { _documentLink = value; } 
        }
    }
}

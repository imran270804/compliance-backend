using System.ComponentModel.DataAnnotations.Schema;

namespace ComplianceCalendar.Models
{
    public class Documents
    {
        public int Id { get; set; }
        private int _filingId;
        private int _uploadedBy;
        private DateTime _uploadedDate;
        private string _documentLink;

        [ForeignKey("Filings")]
        public int FilingId
        {
            get { return _filingId; }
            set { _filingId = value; }
        }
        public Filings Filings { get; set; }

        [ForeignKey("Employee")]
        public int UploadedBy
        {
            get { return _uploadedBy; }
            set { _uploadedBy = value; }
        }

        public Employee Employee { get; set; }

        public DateTime UploadedDate
        {
            get { return _uploadedDate; }
            set { _uploadedDate = value; }
        }

        public string DocumentLink
        {
            get { return _documentLink; }
            set { _documentLink = value; }
        }
    }
}


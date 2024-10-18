using System.ComponentModel.DataAnnotations.Schema;

namespace ComplianceCalendar.Models.DTO
{
    public class UpdateFilingStatusDTO
    {
        private string _status;
        private bool _docIsUploaded;
        private string _remarks;

        [Column(TypeName = "text")]
        public string Status
        {
            get { return _status; }
            set { _status = value; }
        }

        public bool DocIsUploaded
        {
            get { return _docIsUploaded; }
            set { _docIsUploaded = value; }
        }

        public string Remarks
        {
            get { return _remarks; }
            set { _remarks = value; }
        }
    }
}

using System.ComponentModel.DataAnnotations.Schema;

namespace ComplianceCalendar.Models
{
    public class UserFilings
    {
        public int Id { get; set; }
        private int _filingId;
        private int _empId;

        [ForeignKey("Filings")]
        public int FilingId
        {
            get { return _filingId; }
            set { _filingId = value; } 
        }
        public Filings Filings { get; set; }
        [ForeignKey("Employees")]
        public int EmployeeId
        {
            get { return _empId; }
            set { _empId = value; }
        }
        public Employee Employee { get; set; }
    }
}

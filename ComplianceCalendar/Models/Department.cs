using System.ComponentModel.DataAnnotations.Schema;

namespace ComplianceCalendar.Models
{
    public class Department
    {
        public int Id { get; set; }

        private string _depName;
        [Column(TypeName = "text")]
        public string DepName
        {
            get { return _depName; }
            set { _depName = value; }
        }
    }
}

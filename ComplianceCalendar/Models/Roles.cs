using System.ComponentModel.DataAnnotations.Schema;

namespace ComplianceCalendar.Models
{
    public class Roles
    {
        public int Id { get; set; }
        private string rolename;
        [Column(TypeName = "text")]
        public string Rolename
        { 
            get { return rolename; }
            set { rolename = value; }
        }
    }
}

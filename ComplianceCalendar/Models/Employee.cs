using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComplianceCalendar.Models
{
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }
        private string _empname;
        private string _email;
        private int _depId;
        private int _roleId;

        [Column(TypeName = "text")]
        public string EmpName
        {
            get { return _empname; }
            set { _empname = value; }
        }
        [Column(TypeName = "text")]
        public string Email
        {
            get { return _email; }
            set { _email = value; }
        }
        [ForeignKey("Departments")]
        public int DepId
        {
            get { return _depId; }
            set { _depId = value; }
        }
        public Department Departments { get; set; }
        [ForeignKey("Roles")]
        public int RoleId
        {
            get { return _roleId; }
            set { _roleId = value; }
        }
        public Roles Role { get; set; }

        public DateTime CreatedDate { get; set; }
        public bool IsEnabled { get; set; }
    }
}

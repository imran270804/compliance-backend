using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComplianceCalendar.Models
{
    public class AssignedTo
    {
        public int Id { get; set; }
        private int _empId;
        private int _departmentId;
        [ForeignKey ("Employee")]
        public int EmpId { get { return _empId; } set { _empId = value; } }
        public Employee Employees { get; set; }
        [ForeignKey("DepartmentId")]
        public int DepartmentId { get { return _departmentId; } set { _departmentId = value; } }
        public Department Departments { get; set; }

    }
}

namespace ComplianceCalendar.Models.DTO
{
    public class DepartmentEmployeeDTO
    {
        public int Id { get; set; }
        public string DepName { get; set; }
        public List<Employee> Employees { get; set; }
    }
}

namespace ComplianceCalendar.Models.DTO
{
    public class AddFilingsDTO
    {
        public DateTime DueDate { get; set; }
        public string StatuteOrAct { get; set; }
        public string FormChallan { get; set; }
        public string Particulars { get; set; }
        public string DepName { get; set; }
        public int CreatedById { get; set; } 
        public string Recurrence { get; set; }

        public List<AssignedTo> AssignedToList { get; set; } = new List<AssignedTo>();
    }

    public class AssignedTo
    {
        public string EmpName { get; set; }
    }
}

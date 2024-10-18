namespace ComplianceCalendar.Models.DTO
{
    public class ExcelUploadDTO
    {
        public DateTime DueDate { get; set; }
        public string StatuteOrAct { get; set; }
        public string FormChallan { get; set; }
        public string Particulars { get; set; }
        public string Status { get; set; }
        public string DepName { get; set; }
        public bool DocIsUploaded { get; set; }
        public string Remarks { get; set; }
        public int CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<string> TaskOwnerNames { get; set; }
    }
}





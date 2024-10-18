namespace ComplianceCalendar.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string NotificationType { get; set; }
        public string NotificationBody { get; set; }
        public bool IsRead { get; set; }
        public int EmpId { get; set; }
        public Employee Employee { get; set; }
    }
}

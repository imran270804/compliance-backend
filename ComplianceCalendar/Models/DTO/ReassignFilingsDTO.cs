using System.ComponentModel.DataAnnotations.Schema;

namespace ComplianceCalendar.Models.DTO
{
    public class ReassignFilingsDTO
    {
        [Column(TypeName = "text")]
        public string StatuteOrAct { get; set; }
        [Column(TypeName = "text")]
        public string FormChallan { get; set; }
        [Column(TypeName = "text")]
        public string Particulars  { get; set; }
        [Column(TypeName = "text")]
        public string DepName { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComplianceCalendar.Models
{
    public class Filings
    {
        public int Id { get; set; }
        private DateTime _dueDate;
        private string _statuteOrAct;
        private string _formChallan;
        private string _particulars;
        private string _status;
        private string _depName;
        private bool _docIsUploaded;
        private string _remarks;
        private int _createdById;
        private string _review;
        private bool _isRecurring;

        public DateTime CreatedDate { get; set; }
        public DateTime? ClosedDate { get; set; }
        public DateTime DueDate
        {
            get { return _dueDate; }
            set { _dueDate = value; }
        }
        [Column(TypeName = "text")]
        public string StatuteOrAct
        {
            get { return _statuteOrAct; }
            set { _statuteOrAct = value; }
        }
        [Column(TypeName = "text")]
        public string FormChallan
        {
            get { return _formChallan; }
            set { _formChallan = value; }
        }

        [Column(TypeName = "text")]
        public string Particulars
        {
            get { return _particulars; }
            set { _particulars = value; }
        }

        [Column(TypeName = "text")]
        public string Status
        {
            get { return _status; }
            set { _status = value; }
        }
        [Column(TypeName = "text")]
        public string DepName
        {
            get { return _depName; }
            set { _depName = value; }
        }
        public int CreatedById
        {
            get { return _createdById; }
            set { _createdById = value; }
        }
        public Employee CreatedBy { get; set; }
        public bool DocIsUploaded
        {
            get { return _docIsUploaded; }
            set { _docIsUploaded = value; }
        }
        [Column(TypeName = "text")]
        public string Remarks
        {
            get { return _remarks; }
            set { _remarks = value; }
        }

        [Column(TypeName = "text")]
        public string Review
        {
            get { return _review; }
            set { _review = value; }
        }
        public bool IsRecurring
        {
            get { return _isRecurring; }
            set { _isRecurring = value; }
        }
        public Filings()
        {
            _dueDate = DateTime.UtcNow;
            CreatedDate = DateTime.UtcNow;
            ClosedDate = DateTime.UtcNow;
        }
    }
}

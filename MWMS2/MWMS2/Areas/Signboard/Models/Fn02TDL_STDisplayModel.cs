using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Areas.Signboard.Models
{
    public class Fn02TDL_STDisplayModel : DisplayGrid
    {
        public string Uuid;
        public List<TaskModel> ValidationTaskList { get; set; }
        public List<TaskModel> AuditTaskList { get; set; }
        public int ValidationTaskListSize { get; set; }
        public int AuditTaskListSize { get; set; }
    }

    public class TaskModel : DisplayGrid
    {
        public string Uuid { get; set; } // B_SV_AUDIT_RECORD: UUID
        public string Task { get; set; }
        public string SubmissionNo { get; set; }
        public string FormCode { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public DateTime? AssignmentDate { get; set; }
        public string Status { get; set; }
    }
}
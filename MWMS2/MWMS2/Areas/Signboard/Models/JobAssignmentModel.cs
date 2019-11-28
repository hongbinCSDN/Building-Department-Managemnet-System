using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Areas.Signboard.Models
{
    public class JobAssignmentModel // SpoAssignmentObject
    {
        public string Uuid { get; set; } // B_SV_RECORD.UUID
        public string FileRefNo { get; set; }

        public string FormCode { get; set; }

        public string ReceivedDate { get; set; }
        public string PawName { get; set; }
        public string SignBoardDescription { get; set; }
        public string ToUserID { get; set; }
        public string PoUserID { get; set; }
        public string SpoUserID { get; set; }
        public string SpoAssignment { get; set; }

    }
}
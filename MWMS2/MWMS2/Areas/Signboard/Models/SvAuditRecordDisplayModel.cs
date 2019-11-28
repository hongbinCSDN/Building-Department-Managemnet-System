using MWMS2.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Areas.Signboard.Models
{
    public class SvAuditRecordDisplayModel
    {
        public string Uuid;
        public string FileRefNo;
        public string ReceivedDate;
        public List<B_SV_AUDIT_RECORD> SvAuditRecord { get; set; }
    }
}
using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Areas.Signboard.Models
{
    public class ProcessHandlingDisplayModel : DisplayGrid
    {
        public string Uuid; // B_SV_VALIDATION: UUID
        public string FormCode;
        public DateTime? ReceivedDate;
        public string ValidationResult;
        public DateTime? ExpiryDate;
    }
}
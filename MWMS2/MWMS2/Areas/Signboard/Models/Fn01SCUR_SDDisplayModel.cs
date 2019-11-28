using MWMS2.Entity;
using MWMS2.Services.Signborad.SignboardDAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Areas.Signboard.Models
{
    public class Fn01SCUR_SDDisplayModel
    {

       public string TargetDSNUUID { get; set; }
       public string SubmissionNo { get; set; }
 
       public List<B_SV_SCANNED_DOCUMENT> B_SV_SCANNED_DOCUMENT { get; set; }
    }
}
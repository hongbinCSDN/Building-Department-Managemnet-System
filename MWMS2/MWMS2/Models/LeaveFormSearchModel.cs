using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Models
{
    public class LeaveFormSearchModel : DisplayGrid
    {
       public string RegNo { get; set; }
       public string CompName { get; set; }
       public string SurName { get; set; }
       public string GivenName { get; set; }
       public string ChiName { get; set; }
       public DateTime? StartDate { get; set; }
       public DateTime? EndDate { get; set; }
    }
}
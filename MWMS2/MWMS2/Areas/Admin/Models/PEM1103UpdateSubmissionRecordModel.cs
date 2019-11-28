using MWMS2.Entity;
using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Areas.Admin.Models
{
    public class PEM1103UpdateSubmissionRecordModel : DisplayGrid
    {
        public string UUID { get; set; }
        public List<P_MW_RECORD_ITEM> MwRecordMinorWorkItemList { get; set; }
        public P_MW_RECORD_ITEM MwRecordMinorWorkItem { get; set; }
        public List<P_MW_APPOINTED_PROFESSIONAL> AppointedProfessionalList { get; set; }
    }

    //public class MinorWorkItemDispalyModel
    //{
    //    public string Uuid { get; set; }
    //    public string ItemUuid { get; set; }
    //    public string ItemNo { get; set; }
    //    public string Description { get; set; }
    //    public string ReferenceNo { get; set; }
    //}

}
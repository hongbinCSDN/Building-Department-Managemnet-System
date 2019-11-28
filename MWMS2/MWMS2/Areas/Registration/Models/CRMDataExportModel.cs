using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.Registration.Models
{

    public class CRMDataExportModel : CRMCommonDropDownList
    {

        public IList<string> SelectedCategoryList { get; set; }

        public String Category { get; set; }
        public String ApplicantRole { get; set; }
        public String PNRC { get; set; }

        public bool exportOfficeInfo { get; set; }
        public bool exportCorrAddress { get; set; }
        public bool exportBuildingSafety { get; set; }

        public bool exportEmergencyNo { get; set; }
        public bool exportPRBQualification { get; set; }


    }
}
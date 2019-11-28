using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Utility;

namespace MWMS2.Areas.Signboard.Models
{


    public class IssueLetterDisplayModel : SignboardRecordCommonDisplayModel
    {
        public string PBP_CODE_AP { get{ return SignboardConstant.PBP_CODE_AP; } set { } }

        public string MODIFIED_DATE { get; set; }

        //public SignboardRecordCommonDisplayModel SBCommonDisplayModel { get; set; }
        public string id { get { return B_SV_VALIDATION.UUID; } }
      //  public B_SV_RECORD B_SV_RECORD { get; set; }


        public B_SV_APPOINTED_PROFESSIONAL AP_AP { get; set; }
        public B_SV_APPOINTED_PROFESSIONAL AP_RSE { get; set; }
        public B_SV_APPOINTED_PROFESSIONAL AP_RGE { get; set; }
        public B_SV_APPOINTED_PROFESSIONAL AP_PRC { get; set; }
        
        

        public List<B_SV_SCANNED_DOCUMENT> SDList { get; set; }

        public List<B_SV_PHOTO_LIBRARY> SV_PL_List { get; set; }



        //public B_SV_VALIDATION B_SV_VALIDATION { get { return SBCommonDisplayModel?.B_SV_VALIDATION; }
        //    set { if(this.SBCommonDisplayModel == null) this.SBCommonDisplayModel = new SBCommonDisplayModel();
        //        this.SBCommonDisplayModel.B_SV_VALIDATION = value; }
        //}



        //public string SelectedLetterType { get; set; }
        //public string SelectedLetter { get; set; }

        public List<B_S_LETTER_TEMPLATE> B_S_LETTER_TEMPLATE_List { get; set; }

        public string ExportLetterFlag { get; set; }



    }
}
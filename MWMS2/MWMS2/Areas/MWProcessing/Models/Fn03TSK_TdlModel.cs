using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.MWProcessing.Models
{
    public class Fn03TSK_TdlModel
    {
    }

    public class Fn03TSK_TdlSearchModel : DisplayGrid, IValidatableObject
    {
        public string RefNo { get; set; }
        public string PostCode { get; set; }
        public P_MW_DIRECT_RETURN DR { get; set; }

        public List<Fn03TSK_DRCheckboxList> ListCheckbox { get; set; }

        public P_S_SYSTEM_VALUE DWRemark2 { get; set; }
        public Fn03TSK_DRCheckboxList DWRemark3 { get; set; }

        public List<P_S_SYSTEM_VALUE> Languages { get; set; }

        public string HandlingUnit { get; set; }

        public string FileType { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield return null;// ValidationUtil.Validate_Mandatory(this, "RefNo");
        }

        public V_CRM_INFO V_CRM_INFO { get; set; }
        public P_S_TO_DETAILS P_S_TO_DETAILS { get; set; }
    }

    public class Fn03TSK_DRCheckboxList
    {
        public Fn03TSK_DRCheckboxList()
        {
            this.IsChecked = false;
        }
        public bool IsChecked { get; set; }
        public string UUID { get; set; }
        public string SystemTypeID { get; set; }
        public string Code { get; set; }
        public string ParentID { get; set; }
        public string Description_C { get; set; }
        public string Description_E { get; set; }
        public string IsActive { get; set; }
    }

    public class Fn03TSK_TdlPrintDRModel
    {
        public string Dsn { get; set; }
        public string ReceiveDate { get; set; }
        public string Fax { get; set; }
        public string Language { get; set; }
        public string FormType { get; set; }
        public string Address { get; set; }
        public string CodeImage0 { get; set; }
        public string CodeImage1 { get; set; }
        public string CodeImage2 { get; set; }
        public string CodeImage3 { get; set; }
        public string CodeImage4 { get; set; }
        public string CodeImage5 { get; set; }
        public string CodeImage6 { get; set; }
        public string CodeImage7 { get; set; }
        public string CodeImage8 { get; set; }
        public string CodeImage9 { get; set; }
        public string CodeImage10 { get; set; }
        public string CodeImage11 { get; set; }
        public string CodeImage12 { get; set; }
        public string CodeImage13 { get; set; }
        public string CodeDecription0 { get; set; }
        public string CodeDecription1 { get; set; }
        public string CodeDecription2 { get; set; }
        public string CodeDecription3 { get; set; }
        public string CodeDecription4 { get; set; }
        public string CodeDecription5 { get; set; }
        public string CodeDecription6 { get; set; }
        public string CodeDecription7 { get; set; }
        public string CodeDecription8 { get; set; }
        public string CodeDecription9 { get; set; }
        public string CodeDecription10 { get; set; }
        public string CodeDecription11 { get; set; }
        public string CodeDecription12 { get; set; }
        public string CodeDecription13 { get; set; }
        public string DWRemark2 { get; set; }
        public string DWRemark3 { get; set; }
        public string GivenName { get; set; }
        public string Surname { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string AddressLine4 { get; set; }
        public string AddressLine5 { get; set; }
        public string SpoName { get; set; }
        public string SpoPost { get; set; }
        public string TOContactNo { get; set; }
        public string TOName { get; set; }
        public string POName { get; set; }
    }
}
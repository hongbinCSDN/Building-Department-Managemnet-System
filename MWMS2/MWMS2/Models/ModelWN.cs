using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using MWMS2.Entity;


namespace MWMS2.Models
{
    public class ModelWN : DisplayGrid
    {

        public string UUID { get; set; }

        [Required]
        public string SUBJECT { get; set; }
        public string CATEGORY { get; set; }
        [Required]
        public string REGISTRATION_NO { get; set; }
        public string MW_SUBMISSION_NO { get; set; }
        public string MW_ITEMS { get; set; }
        public string COMP_CONTRACTOR_NAME_ENG { get; set; }
        public string COMP_CONTRACTOR_NAME_CHI { get; set; }
        public string AUTHORIZED_SIGNATORY_NAME_ENG { get; set; }
        public string AUTHORIZED_SIGNATORY_NAME_CHI { get; set; }
        public string WLContStatus { get; set; }
        //[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        //public Nullable<System.DateTime> EXPIRY_DATE { get; set; }
        public string SECTION_UNIT { get; set; }
        //public string AUTHOR { get; set; }
        public string FILE_REF_FOUR { get; set; }
        public string FILE_REF_TWO { get; set; }
        public string WL_ISSUED_BY { get; set; }
        public string POST { get; set; }
        public string CASE_OFFICER { get; set; }
        public string RELATED_TO { get; set; }
        public string SOURCE { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public Nullable<System.DateTime> LETTER_ISSUE_DATE { get; set; }
        public string LETTER_FILE_PATH { get; set; }
 
        public string STATUS { get; set; }
        public string REMARK { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public Nullable<System.DateTime> CREATED_DATE { get; set; }
        public string CREATED_BY { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public Nullable<System.DateTime> MODIFIED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public string NOTICE_NO { get; set; }



        public string OFFENSE_DESCRIPTION { get; set; }
        public string[] v_Offense_Type_CheckBox { get; set; }
        public string[] v_MWItems_Type_CheckBox { get; set; }
        public string[] v_Cat_Checkbox { get; set; }
        public string[] v_Section_checkbox { get; set; }
        public string[] v_Related_checkbox { get; set; }
        public string[] v_Source_checkbox { get; set; }
        public string[] v_Status_checkbox { get; set; }

        public string[] COM_Creation_Status { get; set; }
        public string[] AS_Creation_Status  { get; set; }
        public string[] COM_Current_Status  { get; set; }
        public string[] AS_Current_Status   { get; set; }

      
        public List<Tuple<byte[],string>> v_Temp_file { get; set; }
        //public string v_ExpiryDate { get; set; }
        [Required]
        [Display(Name = "Issue Date")]
        public string v_IssuedDate { get; set; }
  


        public DateTime? SearchString_CreateStartDate { get; set; }
        public DateTime? SearchString_CreateEndDate { get; set; }
        public DateTime? SearchString_IssuedStartDate { get; set; }
        public DateTime? SearchString_IssuedEndDate { get; set; }
        //public string SearchString_ExpiryStartDate { get; set; }
        //public string SearchString_ExpiryEndDate { get; set; }
       
        public List<CheckBox> OffenseTypeChkb { get; set; }


        public W_WL M_WL { get; set; }
        public List<W_WL_FILE> M_WL_FILE { get; set; }
        public List<W_WL_MW_ITEM> M_WL_MW_ITEM { get; set; }
        public List<W_WL_TYPE_OF_OFFENSE> M_WL_TYPE_OF_OFFENSE { get; set; }

     


    }
}
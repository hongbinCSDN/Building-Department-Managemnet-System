using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.Registration.Models
{
    public class EformModel
    {
        public string FileRefNo { get; set; }

        public string RegistrationType { get; set; }
        public string FileRefSuffix { get; set; }
        public string SelectedEformApplication { get; set; } // table name
        public string EditMode { get; set; }
        public string ErrMSg { get; set; }

        public List<Dictionary<string, string>> EformitemList { get; set; }
        public string EformSelectFunc { get; set; }
        public string EformSaveFunc { get; set; }

        public List<List<object>> SelectedEformList { get; set; }

        public Dictionary<string, string> statusMap { get; set; } // EFSS UUID, STATUS
        public string status { get; set; }

        // ----- for view -----
        public List<string> Fields { get; set; }
        public List<string> Displays { get; set; }
        public List<string> DisplayFields { get; set; }

        public IEnumerable<SelectListItem> statusList
        {
            get
            {
                return (new List<SelectListItem>()
                {   new SelectListItem{ Value = "- select -" , Text = "- select -" },
                    new SelectListItem{ Value = "received" , Text = "received" },
                    new SelectListItem{ Value = "processed" , Text = "processed" },
                    new SelectListItem{ Value = "approved" , Text = "approved" },

                });
            }
        }

        // ----- variables -----
        public string[] EFSS_COMPANY_FIELD = new string[] {
            "ID","FORMTYPE","CHNCOMPNAME","ENGCOMPNAME","FILEREFTYPE","FILEREF",
            "ADDRESS_LINE1","ADDRESS_LINE2","ADDRESS_LINE3","ADDRESS_LINE4","ADDRESS_LINE5",
            "C_ADDRESS_LINE1","C_ADDRESS_LINE2","C_ADDRESS_LINE3","C_ADDRESS_LINE4","C_ADDRESS_LINE5",
            "TELNO","FAXNO","EMAIL","BRNO","COMTYPE","APPNAME","LASTUPDDATE","CREATEDATE","APPLICATIONDATE","TELNO2","TELNO3","FAXNO2", "EFSS_NO", "STATUS"};
        public string[] EFSS_COMPANY_DISPLAY = new string[] {
            "Latest Modified Date","Application Date","Form Type","Br No.","Company Name (Chinese)",
            "Company Name (English)","File Reference Type","Address(Eng)","Address(Chi)","Tel No.",
            "Fax No.","Email","Company Type","Applicant Name", "EFSS No.", "Status"};
        public string[] EFSS_COMPANY_DISPLAYFIELD = new string[] {
            "LASTUPDDATE","APPLICATIONDATE","FORMTYPE","BRNO",
            "CHNCOMPNAME","ENGCOMPNAME","FILEREFTYPE",
            "ADDRESS_LINE1","ADDRESS_LINE2","ADDRESS_LINE3","ADDRESS_LINE4","ADDRESS_LINE5",
            "C_ADDRESS_LINE1","C_ADDRESS_LINE2","C_ADDRESS_LINE3","C_ADDRESS_LINE4","C_ADDRESS_LINE5",
            "TELNO","FAXNO","EMAIL","COMTYPE","APPNAME", "EFSS_NO", "STATUS"};

        public string[] EFSS_PROFESSIONAL_FIELD = new string[] {
            "ID","FORMTYPE","CATCODE","FILEREF","CERTNO","ADDRESS_LINE1","ADDRESS_LINE2",
            "ADDRESS_LINE3","ADDRESS_LINE4","ADDRESS_LINE5",
            "C_ADDRESS_LINE1","C_ADDRESS_LINE2",
            "C_ADDRESS_LINE3","C_ADDRESS_LINE4","C_ADDRESS_LINE5","FAXNO","EMAIL",
            "OFFICE_ADDRESS_LINE1","OFFICE_ADDRESS_LINE2","OFFICE_ADDRESS_LINE3",
            "OFFICE_ADDRESS_LINE4","OFFICE_ADDRESS_LINE5",
            "C_OFFICE_ADDRESS_LINE1","C_OFFICE_ADDRESS_LINE2","C_OFFICE_ADDRESS_LINE3",
            "C_OFFICE_ADDRESS_LINE4","C_OFFICE_ADDRESS_LINE5",
            "TELNO1","TELNO2","EMERGNO1","EMERGNO2","SPECSIGN",
            "BSCODE1","BSCODE2","BSCODE3","BSCODE4",
            "ISINTERESTQP","LANG","CREATEDATE","LASTUPDDATE", "APPLICATIONDATE", "TELNO3", "FAXNO2", "EFSS_NO", "STATUS"};
        public string[] EFSS_PROFESSIONAL_DISPLAY = new string[] {
            "Latest Modified Date","Application Date","Form Type","Category","Certificate of Registration No.",
            "Address(Eng)","Address(Chi)",
            "Fax No.","Email","Office Address(Eng)","Office Address(Chi)","Tel. No.1","Tel. No.2",
            "Emerg No.1","Emerg No.2","Building Safety 1","Building Safety 2","Building Safety 3","Building Safety 4","Interested QP?","Language",
            "Created Date", "EFSS No.", "Status"};
        public string[] EFSS_PROFESSIONAL_DISPLAYFIELD = new string[] {
            "LASTUPDDATE","APPLICATIONDATE","FORMTYPE","CATCODE","CERTNO",
            "ADDRESS_LINE1","ADDRESS_LINE2","ADDRESS_LINE3","ADDRESS_LINE4","ADDRESS_LINE5",
            "C_ADDRESS_LINE1","C_ADDRESS_LINE2","C_ADDRESS_LINE3","C_ADDRESS_LINE4","C_ADDRESS_LINE5",
            "FAXNO","EMAIL",
            "OFFICE_ADDRESS_LINE1","OFFICE_ADDRESS_LINE2",
            "OFFICE_ADDRESS_LINE3","OFFICE_ADDRESS_LINE4",
            "OFFICE_ADDRESS_LINE5",
            "C_OFFICE_ADDRESS_LINE1","C_OFFICE_ADDRESS_LINE2",
            "C_OFFICE_ADDRESS_LINE3","C_OFFICE_ADDRESS_LINE4",
            "C_OFFICE_ADDRESS_LINE5",
            "TELNO1","TELNO2","EMERGNO1","EMERGNO2","BSCODE1","BSCODE2","BSCODE3","BSCODE4",
            "ISINTERESTQP","LANG","CREATEDATE", "EFSS_NO", "STATUS"};

        public string[] EFSS_MWC_FIELD = new string[] {
            "ID","FORMTYPE","FILEREF","BRNO","ENGCOMNAME","CHNCOMNAME",
            "COMTYPE","APPNAME","E_ADDRESS_LINE1","E_ADDRESS_LINE2",
            "E_ADDRESS_LINE3","E_ADDRESS_LINE4","E_ADDRESS_LINE5","C_ADDRESS_LINE1",
            "C_ADDRESS_LINE2","C_ADDRESS_LINE3","C_ADDRESS_LINE4","C_ADDRESS_LINE5",
            "TELNO1","TELNO2","TELNO3","FAXNO1","FAXNO2","EMAIL",
            "LASTUPDDATE", "CREATEDATE", "APPLICATIONDATE", "EFSS_NO", "STATUS"};
        public string[] EFSS_MWC_DISPLAY = new string[] {
            "Latest Modified Date","Application Date","Form Type","Br No.", "Company Name (English)"
            ,"Company Name (Chinese)","Address (Eng)","Address (Chi)"
            ,"Company Type", "Applicant Name","Tel No.1","Tel No.2","Tel No.3"
            ,"Fax No. 1" ,"Fax No. 2","Email", "EFSS No.", "Status"};
        public string[] EFSS_MWC_DISPLAYFIELD = new string[] {
            "LASTUPDDATE","APPLICATIONDATE","FORMTYPE","BRNO","ENGCOMNAME","CHNCOMNAME"
            ,"E_ADDRESS_LINE1","E_ADDRESS_LINE2","E_ADDRESS_LINE3"
            ,"E_ADDRESS_LINE4","E_ADDRESS_LINE5"
            ,"C_ADDRESS_LINE1","C_ADDRESS_LINE2","C_ADDRESS_LINE3"
            ,"C_ADDRESS_LINE4","C_ADDRESS_LINE5","COMTYPE"
            ,"APPNAME","TELNO1","TELNO2","TELNO3","FAXNO1","FAXNO2","EMAIL", "EFSS_NO", "STATUS"
            };

        public string[] EFSS_MWCW_FIELD = new string[] {
            "ID","FORMTYPE","FILEREF","CORR_E_ADDRESS_LINE1","CORR_E_ADDRESS_LINE2",
            "CORR_E_ADDRESS_LINE3","CORR_E_ADDRESS_LINE4","CORR_E_ADDRESS_LINE5","CORR_C_ADDRESS_LINE1",
            "CORR_C_ADDRESS_LINE2","CORR_C_ADDRESS_LINE3","CORR_C_ADDRESS_LINE4",
            "CORR_C_ADDRESS_LINE5","COMP_E_ADDRESS_LINE1","COMP_E_ADDRESS_LINE2",
            "COMP_E_ADDRESS_LINE3","COMP_E_ADDRESS_LINE4","COMP_E_ADDRESS_LINE5",
            "COMP_C_ADDRESS_LINE1","COMP_C_ADDRESS_LINE2","COMP_C_ADDRESS_LINE3",
            "COMP_C_ADDRESS_LINE4","COMP_C_ADDRESS_LINE5","TELNO1","TELNO2",
            "TELNO3","FAXNO1","FAXNO2","EMAIL",
            "LASTUPDDATE","CREATEDATE","APPLICATIONDATE", "EFSS_NO", "STATUS"};
        public string[] EFSS_MWCW_DISPLAY = new string[] {
            "Latest Modified Date","Application Date","Form Type", "Correspondence Address(Eng)", "Correspondence Address(Chi)"
            ,"Office Address(Eng)", "Office Address(Chi)", "Tel No.1"
            , "Tel No.2", "Tel No.3", "Fax No.1", "Fax No.2", "Email", "EFSS No.", "Status"};
        public string[] EFSS_MWCW_DISPLAYFIELD = new string[] {
            "LASTUPDDATE","APPLICATIONDATE","FORMTYPE"
            ,"CORR_E_ADDRESS_LINE1","CORR_E_ADDRESS_LINE2"
            ,"CORR_E_ADDRESS_LINE3","CORR_E_ADDRESS_LINE4"
            ,"CORR_E_ADDRESS_LINE5"
            ,"CORR_C_ADDRESS_LINE1","CORR_C_ADDRESS_LINE2"
            ,"CORR_C_ADDRESS_LINE3","CORR_C_ADDRESS_LINE4"
            ,"CORR_C_ADDRESS_LINE5"
            ,"COMP_E_ADDRESS_LINE1","COMP_E_ADDRESS_LINE2"
            ,"COMP_E_ADDRESS_LINE3","COMP_E_ADDRESS_LINE4"
            ,"COMP_E_ADDRESS_LINE5"
            ,"COMP_C_ADDRESS_LINE1","COMP_C_ADDRESS_LINE2"
            ,"COMP_C_ADDRESS_LINE3","COMP_C_ADDRESS_LINE4"
            ,"COMP_C_ADDRESS_LINE5"
            ,"TELNO1","TELNO2","TELNO3","FAXNO1","FAXNO2", "EMAIL", "EFSS_NO", "STATUS"};
    }
}
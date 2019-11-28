using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Utility;



namespace MWMS2.Areas.Registration.Models
{
    public class Fn02GCA_ExportLetterSearchModel : DisplayGrid
    {
        public string FileRef { get; set; }
        public string LetterNumber { get; set; }
        public string AuthName { get; set; }
        public string RegType { get; set; }
        public string selectedLetterUuid { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string AS { get; set; }
        public string TD { get; set; }
        public string PRB { get; set; }
        public string Category { get; set; }
        public string Committee { get; set; }
        public string DIA { get; set; }
       

        public C_COMP_APPLICATION C_COMP_APPLICATION { get; set; }
        public C_IND_APPLICATION C_IND_APPLICATION { get; set; }
        public List<C_S_EXPORT_LETTER> C_S_EXPORT_LETTER  { get; set; }
        public C_COMP_APPLICANT_INFO C_COMP_APPLICANT_INFO { get; set; }
        public C_IND_CERTIFICATE C_IND_CERTIFICATE { get; set; }

        public bool AS_Select { get; set; }
        public bool TD_Select { get; set; }
        public bool PRB_Select { get; set; }
        public bool CERT_Select { get; set; }
        public bool AUTHORITY_Select { get; set; }
        public bool DIA_Select { get; set; }
        public bool COMMITTEE_Select { get; set; }

        public List<C_S_EXPORT_LETTER> LetterNumberList
        {
            get
            {
                return SystemListUtil.LetterNumber(RegType);
            }
        } 
         

       

        public IEnumerable<SelectListItem> LetterNumberCGCList
        {
            get { return SystemListUtil.GetICType_IP(); }
        }
        public IEnumerable<SelectListItem> LetterNumberIPList
        {
            get { return SystemListUtil.GetICType_IP(); }
        }
        public IEnumerable<SelectListItem> LetterNumberCMWList
        {
            get { return SystemListUtil.GetICType_IP(); }
        }
        public IEnumerable<SelectListItem> LetterNumberIWMList
        {
            get { return SystemListUtil.GetICType_IP(); }
        }
        public IEnumerable<SelectListItem> AuthorityNameList
        {
            get { return SystemListUtil.SignatureName(); }
        }
        public List<SelectListItem> SelectASList { get; set; }
        public List<SelectListItem> SelectTDList { get; set; }
        public List<SelectListItem> SelectPRBList { get; set; }
        public List<SelectListItem> SelectCERTList { get; set; }
        public List<SelectListItem> SelectCOMMITTEEList { get; set; }
        public List<SelectListItem> SelectDIAList { get; set; }


    }
}
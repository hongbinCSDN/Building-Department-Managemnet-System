using MWMS2.Entity;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.WarningLetter.Models
{
    public class WLDisplayModel
    {

        public string ErrorMessage { get; set; }
        
        public W_WL W_WL { get; set; }



        public string DISPLAY_CATEGORY { get; set; }
        public string DISPLAY_COMP_CONTRACTOR_STATUS { get; set; }
        
        public string DISPLAY_AS_STATUS { get; set; }


        public List<W_WL_FILE> W_WL_FILE_LIST { get; set; }

        public List<W_WL_TYPE_OF_OFFENSE> SelectedOffense { get; set; }
        public List<W_S_SYSTEM_VALUE> OffenseType { get; set; }

        public List<W_S_SYSTEM_VALUE> IrrTechType { get; set; }
        public List<W_S_SYSTEM_VALUE> IrrProType { get; set; }
        public List<W_S_SYSTEM_VALUE> IrrMisType { get; set; }
        
        public string ComCurrentStatus { get; set; }
        public string COM_Current_Status { get; set; }
        
        public string ASCurrentStatus { get; set; }
        public string AS_Current_Status { get; set; }
        public List<string> AS_ENG_NAME_List { get; set; }

        public string IssuedDate { get; set; }
        public string CreatedDate { get; set; }

        public List<W_S_SYSTEM_VALUE> GetWLOffenseType()
        {
            return SystemListUtil.GetWLOffenseType();
        }
        public List<SelectListItem> GetSections()
        {
            return SystemListUtil.GetSections();
        }
    }
}
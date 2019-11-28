using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Models;
using MWMS2.Utility;

namespace MWMS2.Areas.Registration.Models
{
    public class Fn01Search_ExportQPSearchModel : DisplayGrid
    {
       
        public bool AP      { get; set; }
        public bool RI      { get; set; }
        public bool RSE     { get; set; }
        public bool GBC     { get; set; }
        public bool MWCInd  { get; set; }
        public bool MWCComp { get; set; }
        public string RegType { get; set; }
        public string check1
        { get
            {
                string r = (AP ? "AP" : "");
                 return r;
            }
        }
        public string check2
        {
            get
            {
                string r = (RI ? "RI" : "");
                return r;
            }
        }
        public string check3
        {
            get
            {
                string r = (RSE ? "RSE" : "");
                return r;
            }
        }
        public string check4
        {
            get
            {
                string r = (GBC ? "GBC" : "");
                return r;
            }
        }
        public string check5
        {
            get
            {
                string r = (MWCInd ? "MWC(C)" : "");
                return r;
            }
        }
        public string check6
        {
            get
            {
                string r = (MWCComp ? "MWC" : "");
                return r;
            }
        }


        //public string FileRef { get; set; }
        //public string RegNo { get; set; }
        //public string HKID { get; set; }
        //public string PassportNo { get; set; }
        //public string SurnName { get; set; }
        //public string GivenName { get; set; }
        //public string ChiName { get; set; }


        //public string ComName { get; set; }
        //public string BRNo { get; set; }



        //public string SerialNo { get; set; }
        //public Nullable<DateTime> IssueDate { get; set; }
        //public Nullable<DateTime> ExpiryDate { get; set; }
        //public Nullable<DateTime> ReturnDate { get; set; }



        //[Display(Name = "AP")]
        //public bool Ap { get; set; }

        //[Display(Name = "RI")]
        //public bool Ri { get; set; }

        //[Display(Name = "RSE")]
        //public bool Rse { get; set; }

        //[Display(Name = "RGBC")]
        //public bool Rgbc { get; set; }

        //[Display(Name = "RMWC(Company) - Type A")]
        //public bool TypeA { get; set; }

        //[Display(Name = "RMWC(Company) - Type G")]
        //public bool TypeG { get; set; }

        //[Display(Name = "RMWC(Individual) - Item 3.6")]
        //public bool Item36 { get; set; }

        //[Display(Name = "RMWC(Individual) - Item 3.7")]
        //public bool Item37 { get; set; }


        ////public List<string> QPTypeSelectedList { get; set; }
        ////public IEnumerable<SelectListItem> QPTypeList { get; set; } = SystemListUtil.RetrieveQPService();

        //public string QPService { get; set; }
        //public IEnumerable<SelectListItem> QPServiceList { get; set; } = SystemListUtil.RetrieveQPService();












    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Models;
namespace MWMS2.Areas.Registration.Models
{
    public class Fn01Search_DFRSearchModel : DisplayGrid
    {
        public string FileRef { get; set; }
        public string CompName { get; set; }
        public string SurnName { get; set; }
        public string GivenName { get; set; }
        public string ChineseName { get; set; }
        public string DeferralPeriodSymbol { get; set; }
        public string DeferralValue { get; set; }
        public string Date { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public string ChkProf { get; set; }
        public string ChkGBC { get; set; }
        public string ChkMWC { get; set; }
        public string ChkMWCI { get; set; }

        public List<SelectListItem> DeferralPeriodSymbolList
        {
            get
            {
                return new List<SelectListItem>()
                {
                    new SelectListItem() { Text = ">=", Value = ">=" },
                    new SelectListItem() { Text = ">", Value = ">" },
                    new SelectListItem() { Text = "=", Value = "=" },
                    new SelectListItem() { Text = "<=", Value = "<=" },
                    new SelectListItem() { Text = "<", Value = "<" },

                };
            }
        }
    }
}
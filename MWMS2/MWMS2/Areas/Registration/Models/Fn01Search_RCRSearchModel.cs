using MWMS2.Constant;
using MWMS2.Models;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.Registration.Models
{
    public class Fn01Search_RCRSearchModel : DisplayGrid
    {
        public string Logic { get; set; }
        public string rc_orderBy { get; set; }
        public string rc_form { get; set; }
        public string rc_groupType1 { get; set; }
        public string rc_groupType2 { get; set; }
        public string rc_groupType3 { get; set; }
        public string rc_groupType4 { get; set; }
        public string rc_groupType5 { get; set; }
        public string rc_groupType6 { get; set; }
        public string rc_groupType7 { get; set; }
        public string rc_applicationType1 { get; set; }
        public string rc_applicationType2 { get; set; }
        public string rc_applicationType3 { get; set; }
        public string rc_applicationType4 { get; set; }
        public string rc_applicationType5 { get; set; }
        public string rc_applicationType6 { get; set; }
        public string rc_applicationType7 { get; set; }
        public string rc_subType1 { get; set; }
        public string rc_subType2 { get; set; }
        public string rc_subType3 { get; set; }
        public string rc_subType4 { get; set; }
        public string rc_subType5 { get; set; }
        public string rc_subType6 { get; set; }
        public string rc_subType7 { get; set; }
        public string[] getRc_groupTypes()
        {
            return new string[]
            {
                rc_groupType1,
                rc_groupType2,
                rc_groupType3,
                rc_groupType4,
                rc_groupType5,
                rc_groupType6,
                rc_groupType7,
            };
        }
        public string[] getRc_applcationTypes()
        {
            return new string[]
            {
                rc_applicationType1,
                rc_applicationType2,
                rc_applicationType3,
                rc_applicationType4,
                rc_applicationType5,
                rc_applicationType6,
                rc_applicationType7,
            };
        }
        public string[] getRc_subTypes()
        {
            return new string[] {
            "All(And)".Equals(rc_subType1, StringComparison.InvariantCultureIgnoreCase) ? "and" : rc_subType1 != null &&
                rc_subType1.Equals("", StringComparison.InvariantCultureIgnoreCase) ? "" : rc_subType1,
            "All(And)".Equals(rc_subType2, StringComparison.InvariantCultureIgnoreCase) ? "and" : rc_subType2 != null &&
                rc_subType2.Equals("", StringComparison.InvariantCultureIgnoreCase) ? "" : rc_subType2,
            "All(And)".Equals(rc_subType3, StringComparison.InvariantCultureIgnoreCase) ? "and" : rc_subType3 != null &&
                rc_subType3.Equals("", StringComparison.InvariantCultureIgnoreCase) ? "" : rc_subType3,
            "All(And)".Equals(rc_subType4, StringComparison.InvariantCultureIgnoreCase) ? "and" : rc_subType3 != null &&
                rc_subType4.Equals("", StringComparison.InvariantCultureIgnoreCase) ? "" : rc_subType4,
            "All(And)".Equals(rc_subType5, StringComparison.InvariantCultureIgnoreCase) ? "and" : rc_subType5 != null &&
                rc_subType5.Equals("", StringComparison.InvariantCultureIgnoreCase) ? "" : rc_subType5,
            "All(And)".Equals(rc_subType6, StringComparison.InvariantCultureIgnoreCase) ? "and" : rc_subType6 != null &&
                rc_subType6.Equals("", StringComparison.InvariantCultureIgnoreCase) ? "" : rc_subType6,
            "All(And)".Equals(rc_subType7, StringComparison.InvariantCultureIgnoreCase) ? "and" : rc_subType7 != null &&
                rc_subType7.Equals("", StringComparison.InvariantCultureIgnoreCase) ? "" : rc_subType7
            };
        }
        public string excelReportType { get; set; }
        public DateTime? specifiedDate { get; set; }
    }
}
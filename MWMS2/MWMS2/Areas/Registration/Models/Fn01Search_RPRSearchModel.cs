using MWMS2.Constant;
using MWMS2.Models;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Entity;

namespace MWMS2.Areas.Registration.Models
{
    public class Fn01Search_RPRSearchModel : DisplayGrid
    {
        public string Logic { get; set; }
        public string rc_applicationType1 { get; set; }
        public string rc_applicationType2 { get; set; }
        public string rc_applicationType3 { get; set; }
        public string rc_applicationType4 { get; set; }
        public string rc_applicationType5 { get; set; }
        public string rc_applicationType6 { get; set; }
        public string rc_applicationType7 { get; set; }
        public string subType1 { get; set; }
        public string subType2 { get; set; }
        public string subType3 { get; set; }
        public string subType4 { get; set; }
        public string subType5 { get; set; }
        public string subType6 { get; set; }
        public string subType7 { get; set; }
        public string div_mwItemList1 { set; get; }
        public string div_mwItemList2 { set; get; }
        public string div_mwItemList3 { set; get; }
        public string div_mwItemList4 { set; get; }
        public string div_mwItemList5 { set; get; }
        public string div_mwItemList6 { set; get; }
        public string div_mwItemList7 { set; get; }
        public string div_mwitem1 { set; get; }
        public string div_mwType1 { set; get; }
        public string div_scType1 { set; get; }
        public string div_apType1 { set; get; }
        public string div_mwitem2 { set; get; }
        public string div_mwType2 { set; get; }
        public string div_scType2 { set; get; }
        public string div_apType2 { set; get; }

        public string div_mwitem3 { set; get; }
        public string div_mwType3 { set; get; }
        public string div_scType3 { set; get; }
        public string div_apType3 { set; get; }

        public string div_mwitem4 { set; get; }
        public string div_mwType4 { set; get; }
        public string div_scType4 { set; get; }
        public string div_apType4 { set; get; }

        public string div_mwitem5 { set; get; }
        public string div_mwType5 { set; get; }
        public string div_scType5 { set; get; }
        public string div_apType5 { set; get; }

        public string div_mwitem6 { set; get; }
        public string div_mwType6 { set; get; }
        public string div_scType6 { set; get; }
        public string div_apType6 { set; get; }

        public string div_mwitem7 { set; get; }
        public string div_mwType7 { set; get; }
        public string div_scType7 { set; get; }
        public string div_apType7 { set; get; }
        public string rc_orderBy { get; set; }
        public string rc_suborderBy { get; set; }
        public string rc_subType1 { get; set; }
        public string rc_subType2 { get; set; }
        public string rc_subType3 { get; set; }
        public string rc_subType4 { get; set; }
        public string rc_subType5 { get; set; }
        public string rc_subType6 { get; set; }
        public string rc_subType7 { get; set; }
        public string[] getRc_subTypes {
            get {
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
        }
        public string mwItemStr
        {
            get
            {
                List<C_S_SYSTEM_VALUE> mwItemSystemList = SystemListUtil.GetMWItemFullListByClass(RegistrationConstant.MW_CLASS_III);
                var mwItemStr = "";
                for (int i = 0; i < mwItemSystemList.Count; i++)
                {
                    if (i > 0)
                    {
                        mwItemStr += ", ";
                    }
                    mwItemStr += "'" + mwItemSystemList[i].CODE + "'";
                }
                return mwItemStr = "var list4 = [" + mwItemStr + "]";
            }
        }
    }
}
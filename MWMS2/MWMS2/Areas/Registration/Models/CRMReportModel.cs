using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Linq;
using System.Web.Mvc;

namespace MWMS2.Areas.Registration.Models
{

    public class CRMReportModel : CRMCommonDropDownList
    {
        //GCA Report
        public String Testing { get; set; }
        //public string RegType { get; set; }
        public string txtFileRef { get; set; }
        public string txtExpFileRef { get; set; }
        public string txtExpNameOfContractor { get; set; }
        public string InfoSheetCnvSource { get; set; }
        public DateTime? InfoSheetJDateFrom { get; set; }
        public DateTime? InfoSheetJDateTo { get; set; }
        public DateTime? InfoSheetODateFrom { get; set; }
        public DateTime? InfoSheetODateTo { get; set; }
        public string InfoSheetCompanyName { get; set; }
        public bool InfoSheetKeyword { get; set; }
        public bool InfoSheetPrint { get; set; }
        public string InfoSheetOrdinanceOrRegulation { get; set; }
        public string InfoSheetSuspensionReason { get; set; }
        public string InfoSheetBRNo { get; set; }
        //Label
        public DateTime? LabelGazDateFrom { get; set; }
        public DateTime? LabelGazDateTo { get; set; }
        public string cat_gp { get; set; }
        public string acting { get; set; }
        public DateTime? LabelExpiryDateFrom { get; set; }
        public DateTime? LabelExpiryDateTo { get; set; }
        public DateTime? MRDate { get; set; }
        public DateTime? PMdate { get; set; }
        public List<SelectListItem> RetrieveCNVSORUCE()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem
            {
                Text = "- All -",
                Value = "",
                Selected = true
            });

            selectListItems.AddRange(from sv in SystemListUtil.GetSVListByType(RegistrationConstant.SYSTEM_TYPE_CONVICTION_SOURCE)
                                     select new SelectListItem()
                                     {
                                         Text = sv.ENGLISH_DESCRIPTION,
                                         Value = sv.UUID,
                                     }

             );

            return selectListItems;
        }
        public string HighClass { get; set; }
        public List<SelectListItem> HighClassList()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem
            {
                Text = "- All -",
                Value = "",
                Selected = true
            });

            var query = from sv in SystemListUtil.GetSVListByType(RegistrationConstant.SYSTEM_TYPE_MWCLASS)
                        select sv;
            foreach (var item in query)
            {
                string tempText = "";
                if (item.CODE.Substring(6) == "1")
                {
                    tempText = "I";
                }
                else if (item.CODE.Substring(6) == "2")
                {
                    tempText = "II";

                }
                else if (item.CODE.Substring(6) == "3")
                {
                    tempText = "III";

                }

                selectListItems.Add(new SelectListItem() { Text = tempText, Value = item.CODE });

            }


            //selectListItems.AddRange(from sv in SystemListUtil.GetSVListByType(RegistrationConstant.SYSTEM_TYPE_MWCLASS)
            //                         select new SelectListItem()
            //                         {
            //                             Text = sv.CODE,
            //                             Value = sv.UUID,
            //                         }

             //);
            return selectListItems;
        }
        public DateTime? Gaz_date { get; set; }
        public string NameOfSignture { get; set; }
        public IEnumerable<SelectListItem> SignatureNameList { get { return SystemListUtil.SignatureName(); } }
        public string PN { get; set; }
        public List<SelectListItem> PNList()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem
            {
                Text = "- All -",
                Value = "",
                Selected = true
            });

            selectListItems.AddRange(from sv in SystemListUtil.GetSVListByType(RegistrationConstant.SYSTEM_TYPE_PRACTICE_NOTE)
                                     select new SelectListItem()
                                     {
                                        
                                         Text = sv.ENGLISH_DESCRIPTION,
                                         Value = sv.UUID,
                                     }

             );

            return selectListItems;
        }
        public string MonitorReportsYear { get; set; }
        public string AttendancYear { get; set; }
        public string AttendancMonth { get; set; }
        public string AttendancFromDate { get; set; }
        public string AttendancFromTo { get; set; }
        public string CategoryCode { get; set; }
        public List<SelectListItem> CategoryCodeList()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem
            {
                Text = "- All -",
                Value = "",
                Selected = true
            });
            selectListItems.Add(new SelectListItem
            {
                Text = "- All Register -",
                Value = "All",
            });
            selectListItems.AddRange(from sv in SystemListUtil.GetCatCodeByRegType(RegistrationConstant.REGISTRATION_TYPE_IP)
                                     select new SelectListItem()
                                     {
                                         Text = sv.ENGLISH_DESCRIPTION,
                                         Value = sv.UUID,
                                     }

             );

            return selectListItems;
        }
        public IEnumerable<SelectListItem> CategoryList
        {

            get
            {
                return SystemListUtil.GetCatList();
            }
        }
        public string CompName { get; set; }
        public bool chkExpiryDate { get; set; }
        public IEnumerable<SelectListItem> MeetingGroupList { get { return SystemListUtil.GetMeetingGroup(); } }
        public string MeetingGroup { get; set; }
        public string StatusId { get; set; }
        public string PeriodOfValidity { get; set; }
        public IEnumerable<SelectListItem> PeriodOfValidityList()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem
            {
                Text = "- All -",
                Value = "",
                Selected = true
            });

            selectListItems.AddRange(from sv in SystemListUtil.GetSVListByType(RegistrationConstant.SYSTEM_TYPE_PERIOD_OF_VALIDITY)
                                     select new SelectListItem()
                                     {
                                         Text = sv.CODE,
                                         Value = sv.UUID,
                                     }
             );
            return selectListItems;
        }
        public string AuthorityName { get; set; }

        public IEnumerable<SelectListItem> AuthorityNameList
        {
            get { return SystemListUtil.RetrieveSAuthorityList(); }
        }
        public string ddlExpPNRC { get; set; }
        public List<SelectListItem> ddlExpPNRCList()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem
            {
                Text = "- All -",
                Value = "ALL",
                Selected = true
            });

            selectListItems.AddRange(from sv in SystemListUtil.GetSVListByType(RegistrationConstant.SYSTEM_TYPE_PRACTICE_NOTE)
                                     select new SelectListItem()
                                     {
                                         Text = sv.ENGLISH_DESCRIPTION,
                                         Value = sv.UUID,
                                     }

             );

            return selectListItems;
        }
        public List<string> ddlExpCatCode { get; set; }
        public IEnumerable<SelectListItem> IPExpCatCode
        {
            get { return IPExpCatCodeList(); }
        }
        public List<SelectListItem> IPExpCatCodeList()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem
            {
                Text = "- All -",
                Value = "ALL",
                Selected = true
            });

            selectListItems.AddRange(from sv in SystemListUtil.GetCatCodeByRegType(RegistrationConstant.REGISTRATION_TYPE_IP)
                                     select new SelectListItem()
                                     {
                                         Text = sv.CODE,
                                         Value = sv.UUID,
                                     }

             );
            return selectListItems;
        }
        public List<SelectListItem> CGAExpCatCodeList()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem
            {
                Text = "- All -",
                Value = "ALL",
                Selected = true
            });

            selectListItems.AddRange(from sv in SystemListUtil.GetCatCodeByRegType(RegistrationConstant.REGISTRATION_TYPE_CGA)
                                     select new SelectListItem()
                                     {
                                         Text = sv.CODE,
                                         Value = sv.UUID,
                                     }

             );
            return selectListItems;
        }
        public List<SelectListItem> MWCAExpCatCodeList()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem
            {
                Text = "- All -",
                Value = "ALL",
                Selected = true
            });

            selectListItems.AddRange(from sv in SystemListUtil.GetCatCodeByRegType(RegistrationConstant.REGISTRATION_TYPE_MWCA)
                                     select new SelectListItem()
                                     {
                                         Text = sv.CODE,
                                         Value = sv.UUID,
                                     }

             );
            return selectListItems;
        }
        public List<SelectListItem> MWIAExpCatCodeList()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem
            {
                Text = "- All -",
                Value = "ALL",
                Selected = true
            });

            selectListItems.AddRange(from sv in SystemListUtil.GetCatCodeByRegType(RegistrationConstant.REGISTRATION_TYPE_MWIA)
                                     select new SelectListItem()
                                     {
                                         Text = sv.CODE,
                                         Value = sv.UUID,
                                     }

             );
            return selectListItems;
        }
        public string willingnessQp { set; get; }
        public string interestedFSS { set; get; }
        public string as_ddCtrCode { set; get; }
        public string Surname { set; get; }
        public string GivenName { set; get; }
        public string ChineseName { set; get; }
        public string HKID { get; set; }
        public string PASSPORT { get; set; }
        //UR
        public string Pooling { get; set; }
        public IEnumerable<SelectListItem> PoolingList
        {
            get{return SystemListUtil.GetPoolingList();}
        }
        public string HKID_PASSPORT_DISPLAY { get { return string.IsNullOrWhiteSpace(HKID) || string.IsNullOrWhiteSpace(PASSPORT) ? HKID + PASSPORT : HKID + "/ " + PASSPORT; } }
        public IEnumerable<SelectListItem> roleCodeList
        {
            get
            {
                return
                    (new List<SelectListItem>() { new SelectListItem() { Text = "- ALL -", Value = "" } })
                        .Concat(SystemListUtil.GetSVListByType(RegistrationConstant.SYSTEM_TYPE_ROLE_AS_TD_OO)
                            .Select(o => new SelectListItem() { Text = o.CODE, Value = o.CODE }));
            }
        }
        public IEnumerable<SelectListItem> AsTdOoStatusList
        {
            get
            {
                return
                    (new List<SelectListItem>() { new SelectListItem() { Text = "- ALL -", Value = "" } })
                        .Concat(SystemListUtil.GetSVListByType(RegistrationConstant.SYSTEM_TYPE_AS_TD_OO_STATUS)
                            .Select(o => new SelectListItem() { Text = o.ENGLISH_DESCRIPTION, Value = o.ENGLISH_DESCRIPTION }));
            }
        }
        public string RoleCode { get; set; }
        public string StatusDesc { get; set; }
        public string QpSerialNo { get; set; }
        public string StatusStr { get; set; }
        public IEnumerable<SelectListItem> DivStatusList
        {
            get { return SystemListUtil.GetDivStatus(""); }
        }
        public string PrsHeader { get; set; }
        public string Short_name { get; set; }
        
        public IEnumerable<SelectListItem> GCACategoryGroupList
        {
            get
            {
                return
                    new List<SelectListItem>().Concat(SystemListUtil.GetSVListByRegTypeNType(
                            RegistrationConstant.REGISTRATION_TYPE_CGA
                            , RegistrationConstant.SYSTEM_TYPE_CATEGORY_GROUP)
                            .Select(o => new SelectListItem() { Text = o.ENGLISH_DESCRIPTION, Value = o.UUID }));
            }
        }
        public IEnumerable<SelectListItem> MWIACategoryGroupList
        {
            get
            {
                return
                    new List<SelectListItem>().Concat(SystemListUtil.GetSVListByRegTypeNType(
                            RegistrationConstant.REGISTRATION_TYPE_MWIA
                            , RegistrationConstant.SYSTEM_TYPE_CATEGORY_GROUP)
                            .Select(o => new SelectListItem() { Text = o.ENGLISH_DESCRIPTION, Value = o.UUID }));
            }
        }
        public IEnumerable<SelectListItem> MWCACategoryGroupList
        {
            get
            {
                return
                    new List<SelectListItem>().Concat(SystemListUtil.GetSVListByRegTypeNType(
                            RegistrationConstant.REGISTRATION_TYPE_MWCA
                            , RegistrationConstant.SYSTEM_TYPE_CATEGORY_GROUP)
                            .Select(o => new SelectListItem() { Text = o.ENGLISH_DESCRIPTION, Value = o.UUID }));
            }
        }
        public IEnumerable<SelectListItem> IpCategoryGroupList
        {
            get
            {
                return
                    new List<SelectListItem>().Concat(SystemListUtil.GetSVListByRegTypeNType(
                            RegistrationConstant.REGISTRATION_TYPE_IP
                            , RegistrationConstant.SYSTEM_TYPE_CATEGORY_GROUP)
                            .Select(o => new SelectListItem() { Text = o.ENGLISH_DESCRIPTION, Value = o.UUID }));
            }
        }
        public string CategoryGroup { get; set; }
        public string AnnualGazetteCtrUUID { get; set; }

        public string RegisteredContractorSummaryComp { get; set; }
        public string ExpiredCtrUUID { get; set; }
        public string CountForWillingnessQp { get; set; }

        public IEnumerable<SelectListItem> QpAsATList { get { return SystemListUtil.GetQpAsATList(); } }
        public string QpAsAT { get; set; }
        public string MWAnnualGazetteCtrUUID { get; set; }
        public string MWCategoryGroup { get; set; }
        public string MWCRegisteredContractorSummaryComp { get; set; }
        public DateTime? txtExpApprovalDtFm { get; set; }
        public DateTime? txtExpApprovalDtTo { get; set; }
        public DateTime? txtExpRegDtFm { get; set; }
        public DateTime? txtExpRegDtTo { get; set; }
        public DateTime? txtExpRetDtFm { get; set; }
        public DateTime? txtExpRetDtTo { get; set; }
        public DateTime? txtExpRestDtFm { get; set; }
        public DateTime? txtExpRestDtTo { get; set; }
        public DateTime? txtExpGazDtFm { get; set; }
        public DateTime? txtExpGazDtTo { get; set; }
        public DateTime? txtExpExpiryDtFm { get; set; }
        public DateTime? txtExpExpiryDtTo { get; set; }
        public DateTime? txtExpRemovalDtFm { get; set; }
        public DateTime? txtExpRemovalDtTo { get; set; }
        public DateTime? CompExpiryFrDate { get; set; }
        public DateTime? CompExpiryToDate { get; set; }
        public DateTime? AcceptFrDate { get; set; }
        public DateTime? AcceptToDate { get; set; }
        public DateTime? RemovalFrDate { get; set; }
        public DateTime? RemovalToDate { get; set; }
        public DateTime? QpIssueFrDate { get; set; }
        public DateTime? QpIssueToDate { get; set; }
        public DateTime? QpExpiryFrDate { get; set; }
        public DateTime? QpExpiryToDate { get; set; }
        public DateTime? QpReturnFrDate { get; set; }
        public DateTime? QpReturnToDate { get; set; }
        public DateTime? AGFrDate { get; set; }
        public DateTime? AGToDate { get; set; }
        public DateTime? ExpiryFrDate { get; set; }
        public DateTime? ExpiryToDate { get; set; }
        public DateTime? ReceivedFrDate { get; set; }
        public DateTime? ReceivedToDate { get; set; }
        public DateTime? offence_fr_date { get; set; }
        public DateTime? offence_to_date { get; set; }
        public DateTime? Expired_fr_date { get; set; }
        public DateTime? Expired_to_date { get; set; }
        public DateTime? from_date { get; set; }
        public DateTime? to_date { get; set; }
        public DateTime? asAtDate { get; set; }
        public DateTime? ReportGazDateFrom { get; set; }
        public DateTime? ReportGazDateTo { get; set; }
        public DateTime? ReportChnGazDateFrom { get; set; }
        public DateTime? ReportChnGazDateTo { get; set; }
        public DateTime? asAtDate2 { get; set; }
        public DateTime? ir_search_date_from { get; set; }
        public DateTime? ir_search_date_to { get; set; }
        public string cat_gp2 { get; set; }
        public string Chn_cat_gp { get; set; }
        public string dd2Cols { get; set; }
        public string dd2Rows { get; set; }
        public string dd3Cols { get; set; }
        public string dd3Rows { get; set; }
        public string pmon_type { get; set; }
        public string exportType { get; set; }
        public string AppStat { get; set; }
        public string FileRef { get; set; }
        public string ExpASTOOFileRef { get; set; }
        public string dd2RowsInt { get; set; }
        public string dd2ColsInt { get; set; }
        public string dd3ColsInt { get; set; }
        public string dd3RowsInt { get; set; }
        public string InfoSurname { get; set; }
        public string InfoGivenName { get; set; }
        public string CheckSurname { get; set; }
        public string CheckGivenName { get; set; }
        public string txtExpSurname { get; set; }
        public string txtExpGName { get; set; }
        public string qp_serial_no { get; set; }
        // ----------------------------
        public string trade { get; set; }
        public string qualification { get; set; }
        public string ageFrom { get; set; }
        public string ageTo { get; set; }
        public IEnumerable<SelectListItem> tradeList
        {
            get
            {
                return SystemListUtil.GetTradeListItem();
            }
        }
        public IEnumerable<SelectListItem> qualificationList
        {
            get
            {
                return new List<SelectListItem>()
                {
                    new SelectListItem { Value = "" , Text = "- Select Support Type -" },
                    new SelectListItem { Value = "E" , Text = "Experience Only" },
                    new SelectListItem { Value = "Q" , Text = "Qualification Only" },
                    new SelectListItem { Value = "A" , Text = "Qualification + Experience Only" },
                    new SelectListItem { Value = "EQA" , Text = "Exp./Quali./Quali + Exp." },
                    new SelectListItem { Value = "EQ" , Text = "Exp./Quali." },
                    new SelectListItem { Value = "EA" , Text = "Exp./Quali.+ Exp." },
                    new SelectListItem { Value = "QA" , Text = "Quali./Quali.+ Exp." }

                };
            }
        }
        public bool checkboxAddr { get; set; }
        public bool checkboxTel { get; set; }
        public bool checkboxEmail { get; set; }
        public bool checkboxFax { get; set; }
        public bool checkboxEmergency { get; set; }
        // ----------------------------
        public IEnumerable<SelectListItem> AppStatList
        {
            get
            {
                return
                    (new List<SelectListItem>())
                        .Concat(SystemListUtil.GetSVListByType(
                             RegistrationConstant.SYSTEM_TYPE_APPLICANT_STATUS)
                            .Select(o => new SelectListItem() { Text = o.ENGLISH_DESCRIPTION, Value = o.UUID }));
            }
        }

        public string MWIHeaderDesc
        {
            get
            {
                return @"Pursuant to section 8A(3) of the Buildings Ordinance, a list of the following names in the register of minor works contractors (individual) kept by the Building Authority is hereby published below, as at[DD][MM][YYYY].";

            }
        }
        public string MWIHeaderChnDesc
        {
            get
            {
                return @"現根據《建築物條例》第8A(3)條，將截至[YYYY]年[MM]月[DD]日為止建築事務監督所備存的小型工程承建商（個 人）名冊內的名單刊載如下：";
            }
        }

        public string PAHeaderDesc
        {
            get {
                return "In pursuance to section 3(4) of the Buildings Ordinance, the Authorized Persons' Register kept by the Buliding Authority under " +
                    " section 3(1) of the Buildings Ordinance is published below, as at [DD][MM][YYYY]"
                    ;
            }
        }

        public string PAHeaderChnDesc {
            get {
                return "現依據《建築物條例》第3(4)條，將截至[YYYY]年[MM]月[DD]日為止建築物事務監督根據《建築物條例》第3(1)條而保存的認可人士名冊刊載如下:";
            }
        }
        public string HeaderDesc
        {
            get
            {
                return "Pursuant to section 8A(3) of the Buildings Ordinance, a list of the names of" +
                    " the contractors in each register kept by the Building Authority is hereby" +
                    " published below, as at[DD][MM][YYYY]";
            }
        }
        public string HeaderChnDesc
        {
            get
            {
                return "現依據《建築物條例》第8A(3)條，將截至[YYYY]年[MM]月[DD]日為止建築事務監督所備存的每份名冊內的承建商名單刊載如下:";
            }
        }
        public bool chkExpInfo { get; set; }
        public IEnumerable<SelectListItem> CatCodeList
        {
            get
            {
                return
                    (new List<SelectListItem>()
                    {
                        new SelectListItem() { Text = "- ALL -", Value = "ALL_GBC" }
                    }).Concat(
                        SystemListUtil.GetSVListByType(RegistrationConstant.SYSTEM_TYPE_COMP_CATEGORY_CODE)
                         .Select(o => new SelectListItem() { Text = o.ENGLISH_DESCRIPTION, Value = o.ENGLISH_DESCRIPTION }));
            }
        }
        public DateTime? inputFirstApplicationDate { get; set; }
        public DateTime? inputOutstandingDate { get; set; }
        public DateTime? inputResultDate { get; set; }
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
        public string rc_applicationType1 { get; set; }
        public string subType1 { get; set; }
        public string div_mwitem1 { get; set; }
        public string div_mwType1 { get; set; }
        public string div_scType1 { get; set; }
        public string div_apType1 { get; set; }
        public string div_mwItemList1 { get; set; }
        public DateTime? Disposal { get; set; }
        public List<SelectListItem> GetYNIOption()
        {
            return SystemListUtil.IpRetrieveYNIOption();
        }
        public string INTERESTED_FSS { get; set; }
        public string SerInMWIS { get; set; }
        public List<SelectListItem> GetMWISOption()
        {
            return SystemListUtil.RetrieveMWISOption();
        }
        public string rc_orderBy { get; set; }
        public string rc_suborderBy { get; set; }
        public string rc_subType1 { get; set; }
        public string[] getRc_subTypes
        {
            get
            {
                return new string[] {
                "All(And)".Equals(rc_subType1, StringComparison.InvariantCultureIgnoreCase) ? "and" : rc_subType1 != null &&
                    rc_subType1.Equals("", StringComparison.InvariantCultureIgnoreCase) ? "" : rc_subType1
                };
            }
        }
        public string today { get { return DateTime.Now.ToString("ddMMyyyy"); } }
        public string reg_type { get; set; }
        public string rptId { get; set; }
        public string IpOrderBy { get; set; }

        public List<SelectListItem> IpOderByList()
        {
            return SystemListUtil.getIpOderByList();
        }
        public List<SelectListItem> MwcCategoryGroupList()
        {
            return SystemListUtil.getMWCList();
        }
    }
}
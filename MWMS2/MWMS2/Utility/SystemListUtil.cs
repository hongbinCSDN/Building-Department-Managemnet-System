
using MWMS2.Constant;
using MWMS2.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace MWMS2.Utility
{
    public class SystemListUtil
    {
        static public List<SelectListItem> RetrieveCategoryCodeByRegType(string RegType)
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem
            {
                Text = "- All -",
                Value = "",
                Selected = true
            });
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                selectListItems.AddRange((from catcode in db.C_S_CATEGORY_CODE
                                          where catcode.REGISTRATION_TYPE == RegType
                                          orderby catcode.CODE
                                          select new SelectListItem()
                                          {
                                              Text = catcode.CODE,
                                              Value = catcode.UUID,
                                          }
                             ).ToList());

            }
            return selectListItems;

        }

        static public List<SelectListItem> GetCompanyTypeList()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem
            {
                Text = "- All -",
                Value = "",
                Selected = true
            });
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                selectListItems.AddRange((from sv in db.C_S_SYSTEM_VALUE
                                          join st in db.C_S_SYSTEM_TYPE on sv.SYSTEM_TYPE_ID equals st.UUID
                                          where st.TYPE == "COMPANY_TYPE"

                                          select new SelectListItem()
                                          {
                                              Text = sv.ENGLISH_DESCRIPTION,
                                              Value = sv.UUID,
                                          }
                             ).ToList());

            }
            return selectListItems;

        }

        static public List<SelectListItem> GetConvictionSourceList()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem
            {
                Text = "- All -",
                Value = "",
                Selected = true
            });
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                selectListItems.AddRange((from sv in db.C_S_SYSTEM_VALUE
                                          join st in db.C_S_SYSTEM_TYPE on sv.SYSTEM_TYPE_ID equals st.UUID
                                          where st.TYPE == "CONVICTION_SOURCE"

                                          select new SelectListItem()
                                          {
                                              Text = sv.ENGLISH_DESCRIPTION,
                                              Value = sv.UUID,
                                          }
                             ).ToList());

            }
            return selectListItems;

        }

        static public List<SelectListItem> GetPanelTypeList()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();

            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                selectListItems.AddRange((from sv in db.C_S_SYSTEM_VALUE
                                          join st in db.C_S_SYSTEM_TYPE on sv.SYSTEM_TYPE_ID equals st.UUID
                                          where st.TYPE == "PANEL_TYPE"

                                          select new SelectListItem()
                                          {
                                              Text = sv.CODE,
                                              // Text = sv.CODE + "- " + sv.UUID,
                                              Value = sv.UUID,
                                          }
                             ).ToList());

            }
            return selectListItems;

        }

        static public List<SelectListItem> RetrieveTitle()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem
            {
                Text = "- All -",
                Value = "",
                Selected = true
            });
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                selectListItems.AddRange((from sv in db.C_S_SYSTEM_VALUE
                                          join st in db.C_S_SYSTEM_TYPE on sv.SYSTEM_TYPE_ID equals st.UUID
                                          where st.TYPE == RegistrationConstant.SYSTEM_TYPE_TITLE
                                          orderby sv.ORDERING
                                          select new SelectListItem()
                                          {
                                              Text = sv.ENGLISH_DESCRIPTION,
                                              Value = sv.UUID,
                                          }
                             ).ToList());
            }
            return selectListItems;

        }

        static public List<SelectListItem> RetrieveResultType()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem
            {
                Text = "- All -",
                Value = "",
                Selected = true
            });
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                selectListItems.AddRange((from sv in db.C_S_SYSTEM_VALUE
                                          join st in db.C_S_SYSTEM_TYPE on sv.SYSTEM_TYPE_ID equals st.UUID
                                          where st.TYPE == RegistrationConstant.SYSTEM_TYPE_INTERVIEW_RESULT

                                          select new SelectListItem()
                                          {
                                              Text = sv.ENGLISH_DESCRIPTION,
                                              Value = sv.UUID,
                                          }
                             ).ToList());
            }
            return selectListItems;

        }
        static public List<SelectListItem> GetSTStatusList()
        {
            return new List<SelectListItem>{
                    new SelectListItem { Text = "Open", Value = SignboardConstant.WF_STATUS_OPEN}
                    ,new SelectListItem { Text = "Done", Value = SignboardConstant.WF_STATUS_DONE}
            };

        }
        static public List<SelectListItem> GetSortList()
        {
            return new List<SelectListItem>{
                    new SelectListItem { Text = "Date/Time", Value = "ST"}
                    ,new SelectListItem { Text = "Submission No.", Value = "REFERENCE_NO"}
                    ,new SelectListItem { Text = "Type", Value = "RECORD_TYPE"}
            };
            //List<SelectListItem> selectListItems = new List<SelectListItem>();
            //selectListItems.Add(new SelectListItem
            //{
            //    Text = "Date/Time",
            //    Value = "ST",

            //});
            //selectListItems.Add(new SelectListItem
            //{
            //    Text = "Submission No.",
            //    Value = "REFERENCE_NO",

            //});
            //selectListItems.Add(new SelectListItem
            //{
            //    Text = "Type",
            //    Value = "RECORD_TYPE",

            //});
            //return selectListItems;
        }
        static public List<SelectListItem> RetrieveGender()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem
            {
                Text = "- Select -",
                Value = "",

            });
            selectListItems.Add(new SelectListItem
            {
                Text = "M",
                Value = "M",

            });
            selectListItems.Add(new SelectListItem
            {
                Text = "F",
                Value = "F",

            });

            return selectListItems;

        }

        static public List<SelectListItem> RetrieveCompInd()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();

            selectListItems.Add(new SelectListItem
            {
                Text = "General Contractor Application",
                Value = "CGC",

            });
            selectListItems.Add(new SelectListItem
            {
                Text = "Professional Application",
                Value = "IP",

            });
            selectListItems.Add(new SelectListItem
            {
                Text = "MW Company Application",
                Value = "CMW",

            });
            selectListItems.Add(new SelectListItem
            {
                Text = "MW Individual Application",
                Value = "IMW",

            });

            return selectListItems;

        }

        static public List<SelectListItem> RetrieveYesNo()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();

            selectListItems.Add(new SelectListItem
            {
                Text = "No",
                Value = "N",

            });
            selectListItems.Add(new SelectListItem
            {
                Text = "Yes",
                Value = "Y",

            });

            return selectListItems;

        }
        static public List<SelectListItem> RetrieveYesNoExtra()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem
            {
                Text = "No Indication",
                Value = "",

            });
            selectListItems.Add(new SelectListItem
            {
                Text = "No",
                Value = "N",

            });
            selectListItems.Add(new SelectListItem
            {
                Text = "Yes",
                Value = "Y",

            });

            return selectListItems;

        }
        static public List<SelectListItem> RetrieveCommunicationLanguage()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();

            selectListItems.Add(new SelectListItem
            {
                Text = "English",
                Value = "E",

            });
            selectListItems.Add(new SelectListItem
            {
                Text = "Chinese",
                Value = "C",

            });

            return selectListItems;

        }
        static public List<SelectListItem> RetrieveYNOption()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();

            selectListItems.Add(new SelectListItem
            {
                Text = "Yes",
                Value = "Y",

            });
            selectListItems.Add(new SelectListItem
            {
                Text = "No",
                Value = "N",

            });

            return selectListItems;

        }

        static public List<SelectListItem> IpRetrieveYNIOption()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem
            {
                Text = "All",
                Value = "",

            });
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                selectListItems.AddRange((
                                        from sv in GetSVListByType(RegistrationConstant.SYSTEM_TYPE_FSS_DROPDOWN)
                                            //where sv.REGISTRATION_TYPE == RegType
                                        select new SelectListItem()
                                        {
                                            Text = sv.ENGLISH_DESCRIPTION,
                                            Value = sv.CODE,
                                        }
                             ).ToList());

            }
            return selectListItems;

        }
        static public List<SelectListItem> RetrieveYNIOption()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();

            //selectListItems.Add(new SelectListItem
            //{
            //    Text = "Yes",
            //    Value = "Y",

            //});
            //selectListItems.Add(new SelectListItem
            //{
            //    Text = "No",
            //    Value = "N",

            //});
            //selectListItems.Add(new SelectListItem
            //{
            //    Text = "No Indication",
            //    Value = "I",

            //});
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                selectListItems.AddRange((
                                        from sv in GetSVListByType(RegistrationConstant.SYSTEM_TYPE_FSS_DROPDOWN)
                                            //where sv.REGISTRATION_TYPE == RegType
                                        select new SelectListItem()
                                        {
                                            Text = sv.ENGLISH_DESCRIPTION,
                                            Value = sv.CODE,
                                        }
                             ).ToList());

            }
            return selectListItems;

        }
        static public List<SelectListItem> RetrieveMWISOption()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                selectListItems.AddRange((
                                        from sv in GetSVListByType(RegistrationConstant.SYSTEM_TYPE_MWIS_DROPDOWN)
                                            //where sv.REGISTRATION_TYPE == RegType
                                        select new SelectListItem()
                                        {
                                            Text = sv.ENGLISH_DESCRIPTION,
                                            Value = sv.CODE,
                                        }
                             ).ToList());

            }
            return selectListItems;
        }

        static public List<SelectListItem> RetrieveServiceInBSByRegType(string RegType)
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem
            {
                Text = "- All -",
                Value = "",
                Selected = true
            });
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                selectListItems.AddRange((
                                        from sv in GetSVListByType(RegistrationConstant.SYSTEM_TYPE_BUILDING_SAFETY_CODE)
                                        where sv.REGISTRATION_TYPE == RegType
                                        select new SelectListItem()
                                        {
                                            Text = sv.CODE,
                                            Value = sv.UUID,
                                        }
                             ).ToList());

            }
            return selectListItems;


        }

        static public List<SelectListItem> GetICType(string RegType)
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem
            {
                Text = "- All -",
                Value = "",
                Selected = true
            });
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                selectListItems.AddRange((
                                        from sv in db.C_S_SYSTEM_VALUE
                                        join st in db.C_S_SYSTEM_TYPE on sv.SYSTEM_TYPE_ID equals st.UUID
                                        where sv.REGISTRATION_TYPE == RegType && st.TYPE == "COMMITTEE_TYPE"
                                        select new SelectListItem()
                                        {
                                            Text = sv.CODE,
                                            Value = sv.CODE,
                                        }
                             ).ToList());

            }
            return selectListItems;

            //return new List<SelectListItem>{
            //        new SelectListItem { Text = "- All -", Value = ""}
            //        , new SelectListItem { Text = "GBC", Value = "GBC"}
            //        , new SelectListItem { Text = "SC", Value = "SC"}
            //        , new SelectListItem { Text = "SC(D)", Value = "SC(D)"}
            //        , new SelectListItem { Text = "SC(F)", Value = "SC(F)"}
            //        , new SelectListItem { Text = "SC(GI)", Value = "SC(GI)"}
            //        , new SelectListItem { Text = "SC(SF)", Value = "SC(SF)"}
            //        , new SelectListItem { Text = "SC(V)", Value = "SC(V)"}

            //};

        }

        static public List<SelectListItem> GetInterviewTypeList()
        {
            return new List<SelectListItem>{
                     new SelectListItem { Text = "- All -", Value = ""     }
                    , new SelectListItem { Text = "Interview", Value = "I" }
                    , new SelectListItem { Text = "Assessment", Value = "A"}


                };

        }
        static public List<SelectListItem> GetMonthList()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();

            int currentMonth = CommonUtil.getCurrentMonth();

            var dt = new DateTime(2019, 1, 1);

            for (int i = 1; i <= 12; i++)
            {
                selectListItems.Add(new SelectListItem
                {
                    Text = dt.AddMonths(i - 1).ToString("MMM", new CultureInfo("en-US")),
                    Value = i.ToString(),
                });
                //selectListItems.Add(new SelectListItem
                //{
                //    Text = DateTimeFormatInfo.CurrentInfo.GetMonthName(i).Substring(0, 3),
                //    Value = i.ToString(),
                //});
            }
            return selectListItems;
        }
        static public List<SelectListItem> GetSMMYearList()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            int currentYear = CommonUtil.getCurrentYear();

            for (int i = currentYear - 10; i < currentYear + 10; i++)
            {
                selectListItems.Add(new SelectListItem
                {
                    Text = i.ToString(),
                    Value = i.ToString(),
                });
            }
            return selectListItems;
        }
        static public List<SelectListItem> GetStatusList()
        {
            return new List<SelectListItem>{
                new SelectListItem { Text = "-All-", Value = ""}
                    ,new SelectListItem { Text = "Active", Value = "Active AS"}
                    , new SelectListItem { Text = "Inactive", Value = "Inactive AS"}
                };

        }
        static public List<SelectListItem> GetTypeList()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();

            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                selectListItems.AddRange((
                                        from sv in db.C_S_SYSTEM_VALUE
                                        join sv1 in db.C_S_SYSTEM_VALUE on sv.PARENT_ID equals sv1.UUID
                                        join st in db.C_S_SYSTEM_TYPE on sv.SYSTEM_TYPE_ID equals st.UUID
                                        where st.TYPE == "MINOR_WORKS_TYPE"
                                        select new SelectListItem()
                                        {
                                            Text = sv.CODE,
                                            Value = sv.UUID,
                                        }
                             ).ToList());

            }
            return selectListItems;

        }

        static public List<SelectListItem> GetCGCSelectionTypeList()
        {
            return new List<SelectListItem>{

                    new SelectListItem { Text = "Company Name, Authority Name", Value = "1"}  //CGC: 1-5
                    , new SelectListItem { Text = "Company Name, AS, Authority Name", Value = "2"}
                    , new SelectListItem { Text = "Company Name, TD, Authority Name", Value = "3"}
                    , new SelectListItem { Text = "Company Name, AS, TD, Authority Name", Value = "4"}
                    , new SelectListItem { Text = "Company Name, AS, TD, Interview Candidate, Authority Name", Value = "5"}

                };

        }

        static public List<SelectListItem> GetIPSelectionTypeList()
        {
            return new List<SelectListItem>{

                      new SelectListItem { Text = "Applicant Name, Category, Authority Name", Value = "7"}  //IP: 7-13
                    , new SelectListItem { Text = "Applicant Name, Category, PRB,  Authority Name", Value = "8"}
                    , new SelectListItem { Text = "Applicant Name, Category, PRB, Committee, Authority Name", Value = "9"}
                    , new SelectListItem { Text = "Applicant Name, Category, Committee, Authority Name", Value = "10"}
                    , new SelectListItem { Text = "Applicant Name, Category, Committee, Date of Interview/Assessment, Authority Name", Value = "11"}
                    , new SelectListItem { Text = "Applicant Name, Category, Date of Interview/Assessment, Authority Name", Value = "12"}
                    , new SelectListItem { Text = "Applicant Name, Category, PRB, Committee, Date of Interview/Assessment, Authority Name", Value = "13"}


                };

        }

        static public List<SelectListItem> GetCMWSelectionTypeList()
        {
            return new List<SelectListItem>{

                    new SelectListItem { Text = "Company Name, Authority Name", Value = "1"}  //cmw: 1-6
                    , new SelectListItem { Text = "Company Name, AS, Authority Name", Value = "2"}
                    , new SelectListItem { Text = "Company Name, TD, Authority Name", Value = "3"}
                    , new SelectListItem { Text = "Company Name, AS, TD, Authority Name", Value = "4"}
                    , new SelectListItem { Text = "Company Name, AS, TD, Interview Candidate, Authority Name", Value = "5"}
                    , new SelectListItem { Text = "Company Name, Authority Name, Approved Letter", Value = "6"}


                };

        }

        static public List<SelectListItem> GetIMWSelectionTypeList()
        {
            return new List<SelectListItem>{

                      new SelectListItem { Text = "Applicant Name, Authority Name, New Registration", Value = "14"}   //IMW: 14-19
                    , new SelectListItem { Text = "Applicant Name, Authority Name, Retention", Value = "15"}
                    , new SelectListItem { Text = "Applicant Name, Authority Name, Restoration", Value = "16"}
                    , new SelectListItem { Text = "Applicant Name, Date of Interview/Assessment, Authority Name, New Registration", Value = "17"}
                    , new SelectListItem { Text = "Applicant Name, Date of Interview/Assessment, Authority Name, Retention", Value = "18"}
                    , new SelectListItem { Text = "Applicant Name, Date of Interview/Assessment, Authority Name, Restoration", Value = "19"}

                };

        }



        static public List<SelectListItem> GetYearList()
        {
            List<SelectListItem> resultList = new List<SelectListItem>();

            resultList.Add(new SelectListItem { Text = "- Select -", Value = "- Select -" });

            int currentYear = CommonUtil.getCurrentYear();
            for (int i = -5; i < 5; i++)
            {
                resultList.Add(new SelectListItem { Text = currentYear + i + "", Value = currentYear + i + "" });

            }
            return resultList;
        }

        static public List<SelectListItem> GetICType_IP()
        {
            return new List<SelectListItem>{
                      new SelectListItem { Text = "- All -", Value = ""     }
                    , new SelectListItem { Text = "SERC"   , Value = "SERC" }
                    , new SelectListItem { Text = "GERC"   , Value = "GERC" }
                    , new SelectListItem { Text = "IRC"    , Value = "IRC"  }
                    , new SelectListItem { Text = "APRC"   , Value = "APRC" }

             };

        }

        // FirstOption
        // 0: no first option
        // 1: blank option
        static public List<SelectListItem> GetSystemValueBySystemType(string SystemType, int FirstOption)
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            string firstOptionDesc = "";
            switch (FirstOption)
            {
                case 2:
                    firstOptionDesc = "All";
                    break;
                case 3:
                    firstOptionDesc = "- All -";
                    break;
                case 4:
                    firstOptionDesc = "- Select - ";
                    break;
                case 5:
                    firstOptionDesc = "- Please Select -";
                    break;
                case 6:
                    firstOptionDesc = "-";
                    break;
            }
            if (FirstOption != 0)
            {
                selectListItems.Add(new SelectListItem { Text = firstOptionDesc, Value = "" });
            }
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                selectListItems.AddRange((
                                        from sv in db.B_S_SYSTEM_VALUE
                                        join st in db.B_S_SYSTEM_TYPE on sv.SYSTEM_TYPE_ID equals st.UUID
                                        where st.TYPE == SystemType && sv.IS_ACTIVE == "Y"
                                        orderby sv.ORDERING
                                        select new SelectListItem()
                                        {
                                            Text = sv.DESCRIPTION,
                                            Value = SystemType.Equals("LetterResult") ? sv.DESCRIPTION : sv.CODE
                                        }
                             ).ToList());

            }
            return selectListItems;

        }

        static public List<SelectListItem> GetClassTypeList()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            // selectListItems.Add(new SelectListItem { Text = "- All -", Value = "- All -" });
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                selectListItems.AddRange((
                                        from sv in db.B_S_SYSTEM_VALUE
                                        join st in db.B_S_SYSTEM_TYPE on sv.SYSTEM_TYPE_ID equals st.UUID
                                        where st.TYPE == "Class"
                                        select new SelectListItem()
                                        {
                                            Text = sv.DESCRIPTION,
                                            Value = sv.CODE,
                                        }
                             ).ToList());

            }
            return selectListItems;

        }

        static public List<SelectListItem> Get_C_ClassTypeList()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            // selectListItems.Add(new SelectListItem { Text = "- All -", Value = "- All -" });
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                selectListItems.AddRange((
                                        from sv in db.C_S_SYSTEM_VALUE
                                        join st in db.C_S_SYSTEM_TYPE on sv.SYSTEM_TYPE_ID equals st.UUID
                                        where st.TYPE == "MINOR_WORKS_CLASS"
                                        select new SelectListItem()
                                        {
                                            Text = sv.CODE,
                                            Value = sv.UUID,
                                        }
                             ).ToList());

            }
            return selectListItems;
        }

        static public List<SelectListItem> GetSUserAccountList(string SecurityLevel)
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem { Text = "", Value = "" });
            using (EntitiesAuth db = new EntitiesAuth())
            {



                var q = db.SYS_POST
                    .Where(o => o.SYS_UNIT.CODE == "SU")
                    .Where(o => o.IS_ACTIVE == "Y")
                    .OrderBy(o => o.CODE)
                    .ToList();
                foreach (var item in q)
                {
                    selectListItems.Add(new SelectListItem { Text = item.CODE, Value = item.UUID });
                }

            }
            return selectListItems;

        }

        static public List<SelectListItem> GetStatusList(int firstOption)
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            if (firstOption == 1)
            {
                selectListItems.Add(new SelectListItem { Text = "- Select -", Value = "" });
            }
            selectListItems.Add(new SelectListItem { Text = "Open", Value = "Open" });
            selectListItems.Add(new SelectListItem { Text = "Close", Value = "Close" });
            return selectListItems;
        }

        static public List<SelectListItem> GetRelatedPartyList()
        {
            return new List<SelectListItem>{
                      new SelectListItem { Text = "- Please Select -", Value = "" }
                    , new SelectListItem { Text = "Authorized person", Value = "AP" }
                    , new SelectListItem { Text = "Registered structural engineer", Value = "RSE" }
                    , new SelectListItem { Text = "Registered geotechnical engineer", Value = "RGE" }
                    , new SelectListItem { Text = "Prescribed registered contractor", Value = "PRC" }
             };

        }

        static public List<SelectListItem> GetBcisAreaCodeList()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem { Text = "All", Value = "" });
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                selectListItems.AddRange((
                                        from sv in db.B_S_SYSTEM_VALUE
                                        join st in db.B_S_SYSTEM_TYPE on sv.SYSTEM_TYPE_ID equals st.UUID
                                        where st.TYPE == "BcisDistrict"
                                        orderby sv.CODE
                                        select new SelectListItem()
                                        {
                                            Text = sv.CODE + " - " + sv.DESCRIPTION,
                                            Value = sv.CODE,
                                        }
                             ).ToList());

            }
            return selectListItems;

        }

        static public List<SelectListItem> GetLedOrTvList()
        {
            return new List<SelectListItem>{
                      new SelectListItem { Text = "", Value = "" }
                    , new SelectListItem { Text = SignboardConstant.LED_YES, Value = SignboardConstant.DATA_ENTRY_LED_YES }
                    , new SelectListItem { Text = SignboardConstant.LED_NO, Value = SignboardConstant.DATA_ENTRY_LED_NO }
                    , new SelectListItem { Text = SignboardConstant.LED_NA, Value = SignboardConstant.DATA_ENTRY_LED_NA }
             };

        }

        static public List<SelectListItem> GetRecordTypeList()
        {
            return new List<SelectListItem>{
                      new SelectListItem { Text = "", Value = "" }
                    , new SelectListItem { Text = "Validation", Value = SignboardConstant.VALIDATED }
                    , new SelectListItem { Text = "New Item", Value = SignboardConstant.NOT_VALIDATED }
             };

        }

        static public List<SelectListItem> GetICType_MW()
        {
            return new List<SelectListItem>{
                      new SelectListItem { Text = "- All -", Value = "     " }
                    , new SelectListItem { Text = "MWCRC  ", Value = "MWCRC" }
             };

        }

        static public List<SelectListItem> GetDateTypeList()
        {
            return new List<SelectListItem>{
                    new SelectListItem { Text = "- Select Type of Date -", Value = "0"}
                    , new SelectListItem { Text = "Date of Application", Value = "1"}
                    , new SelectListItem { Text = "Date of Registration", Value = "2"}
                    , new SelectListItem { Text = "Date of Gazette", Value = "3"}
                    , new SelectListItem { Text = "Date of Expiry", Value = "4"}
                    , new SelectListItem { Text = "Date of Removal", Value = "5"}
                    , new SelectListItem { Text = "Date of Renewal", Value = "6"}
                    , new SelectListItem { Text = "Date of Restoration", Value = "7"}
                    , new SelectListItem { Text = "Date of Approval", Value = "8"}
                };

        }
        static public List<SelectListItem> GetProcessList()
        {
            return new List<SelectListItem>{
                    new SelectListItem { Text = "- Select Type of Process -", Value = "0"}
                    , new SelectListItem { Text = "New Registration", Value = "1"}
                    , new SelectListItem { Text = "New Registration (confirm issue of cert)", Value = "6"}
                    , new SelectListItem { Text = "Retention ", Value = "2"}
                    , new SelectListItem { Text = "Retention (confirm issue of cert)", Value = "7"}
                    , new SelectListItem { Text = "Restoration", Value = "3"}
                    , new SelectListItem { Text = "Restoration (confirm issue of cert)", Value = "8"}
                    , new SelectListItem { Text = "Re-registration", Value = "4"}
                    , new SelectListItem { Text = "Removal", Value = "5"}
                };
        }
        static public List<SelectListItem> GetIndProcessList()
        {
            return new List<SelectListItem>{
                    new SelectListItem { Text = "- Select Type of Process -", Value = "0"}
                    , new SelectListItem { Text = "New Registration", Value = "1"}
                    , new SelectListItem { Text = "New Registration (confirm issue of cert)", Value = "6"}
                    , new SelectListItem { Text = "Retention ", Value = "2"}
                    , new SelectListItem { Text = "Retention (confirm issue of cert)", Value = "7"}
                    , new SelectListItem { Text = "Restoration", Value = "3"}
                    , new SelectListItem { Text = "Restoration (confirm issue of cert)", Value = "8"}
                    , new SelectListItem { Text = "Re-registration", Value = "4"}
                    , new SelectListItem { Text = "Removal", Value = "5"}
                    , new SelectListItem { Text = "QP Card Application", Value = "9"}
                };
        }
        static public List<SelectListItem> CaseStatusList()
        {
            return new List<SelectListItem>{
                    new SelectListItem { Text = "-All-", Value = ""}
                    , new SelectListItem { Text = "In Progress", Value = "In Progress"}
                    , new SelectListItem { Text = "Completed", Value = "Completed"}
                    , new SelectListItem { Text = "Overdue ", Value = "Overdue"}
                };
        }
        static public List<SelectListItem> arrCategoryList()
        {
            return new List<SelectListItem>{
                    new SelectListItem { Text = "-All-", Value = ""}
                    , new SelectListItem { Text = "Professionals (AP, RSE, RGE, RI)", Value = RegistrationConstant.REGISTRATION_TYPE_IP}
                    , new SelectListItem { Text = "Contractors (RGBC, RSC)", Value = RegistrationConstant.REGISTRATION_TYPE_CGA}
                    , new SelectListItem { Text = "RMWC (Company) ", Value = RegistrationConstant.REGISTRATION_TYPE_MWCA}
                    , new SelectListItem { Text = "RMWC (Company) ", Value = RegistrationConstant.REGISTRATION_TYPE_MWIA}
                };
        }
        static public List<SelectListItem> LetterTypeResultList()
        {
            return new List<SelectListItem>{
                    new SelectListItem { Text = "-All-", Value = ""}
                    , new SelectListItem { Text = "Latest-Date-of-Supp.Doc.", Value = RegistrationConstant.REGISTRATION_TYPE_IP}
                    , new SelectListItem { Text = "Date-of.Accept", Value = RegistrationConstant.REGISTRATION_TYPE_CGA}
                    , new SelectListItem { Text = "Date-of-O/S", Value = RegistrationConstant.REGISTRATION_TYPE_MWCA}
                    , new SelectListItem { Text = "Date-of-Refuse", Value = RegistrationConstant.REGISTRATION_TYPE_MWIA}
                    , new SelectListItem { Text = "Date-of-Withdrawal", Value = RegistrationConstant.REGISTRATION_TYPE_MWIA}
                };
        }
        static public List<SelectListItem> AdmTypeOfApplicationList(string RegType)
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem
            {
                Text = "- All -",
                Value = "ALL",
                Selected = true
            });

            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                var AppSQL = from sv in GetSVListByType(RegistrationConstant.SYSTEM_TYPE_APPLICATION_FORM)
                             where sv.REGISTRATION_TYPE == RegType
                             select sv;

                foreach (var item in AppSQL)
                {
                    if (item.REGISTRATION_TYPE == RegType)
                    {

                    }
                    string temp = item.ENGLISH_DESCRIPTION;

                    var FormList = new SelectListItem();
                    FormList.Text = item.CODE;
                    FormList.Value = item.UUID;
                    selectListItems.Add(FormList);
                }
                //selectListItems.AddRange((
                //                        from sv in GetSVListByType(RegistrationConstant.SYSTEM_TYPE_APPLICATION_FORM)
                //                        where sv.REGISTRATION_TYPE == RegType
                //                        select new SelectListItem()
                //                        {
                //                            Text = sv.CODE,
                //                            Value = sv.UUID,
                //                        }
                //             ).ToList());

            }
            return selectListItems;
        }
        static public List<SelectListItem> AdmInterviewResultList()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem
            {
                Text = "- All -",
                Value = "ALL",
                Selected = true
            });
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                var irSQL = from sv in GetSVListByType(RegistrationConstant.SYSTEM_TYPE_INTERVIEW_RESULT)
                            select sv;

                foreach (var item in irSQL)
                {
                    var irList = new SelectListItem();

                    if (item.CODE.Equals("A") || item.CODE.Equals("D") || item.CODE.Equals("W") || item.CODE.Equals("R"))
                    {
                        selectListItems.Add(irList);
                    }
                    else
                    {
                        selectListItems.Add(irList);
                    }
                    irList.Text = item.ENGLISH_DESCRIPTION;
                    irList.Value = item.CODE;
                    selectListItems.Add(irList);
                }
            }
            return selectListItems;
        }
        static public List<SelectListItem> AdmReportTypeOfApplicationList()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem
            {
                Text = "- All -",
                Value = "",
                Selected = true
            });
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                var AppSQL = (from sv in GetSVListByType(RegistrationConstant.SYSTEM_TYPE_APPLICATION_FORM)
                                  //where sv.REGISTRATION_TYPE == RegType
                              select sv).ToList();

                //foreach (var item in AppSQL)
                //{
                //    st
                //    var AdmAppList = new SelectListItem();
                //    AdmAppList.Text = item.CODE;
                //    AdmAppList.Value = item.UUID;
                //    selectListItems.Add(AdmAppList);
                //}
                selectListItems.AddRange((
                                            from sv in GetSVListByType(RegistrationConstant.SYSTEM_TYPE_APPLICATION_FORM)
                                            select new SelectListItem()
                                            {
                                                Text = sv.CODE,
                                                Value = sv.UUID,
                                            }
                                ).ToList().Distinct());

            }
            return selectListItems;
        }
        static public List<SelectListItem> arrTypeOfApplicationList()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem
            {
                Text = "- All -",
                Value = "",
                Selected = true
            });
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                selectListItems.AddRange((
                                        from sv in GetSVListByType(RegistrationConstant.SYSTEM_TYPE_APPLICATION_FORM)
                                            //where sv.REGISTRATION_TYPE == RegType
                                        select new SelectListItem()
                                        {
                                            Text = sv.CODE,
                                            Value = sv.UUID,
                                        }
                             ).ToList());

            }
            return selectListItems;
        }

        static public List<SelectListItem> arrInterviewResultList()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem
            {
                Text = "- All -",
                Value = "",
                Selected = true
            });
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                selectListItems.AddRange((
                                        from sv in GetSVListByType(RegistrationConstant.SYSTEM_TYPE_INTERVIEW_RESULT)
                                            //where sv.REGISTRATION_TYPE == RegType
                                        select new SelectListItem()
                                        {
                                            Text = sv.ENGLISH_DESCRIPTION,
                                            Value = sv.UUID,
                                        }
                             ).ToList());

            }
            return selectListItems;
        }
        static public List<SelectListItem> SignatureName()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem
            {
                Text = "- Select Authority -",
                Value = "",
                Selected = true
            });
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                selectListItems.AddRange((from sv in db.C_S_AUTHORITY
                                          orderby sv.ENGLISH_NAME
                                          select new SelectListItem()
                                          {
                                              Text = sv.ENGLISH_NAME,
                                              Value = sv.UUID,
                                          }
                             ).ToList());
            }
            return selectListItems;
        }
        static public List<SelectListItem> UserAccountName()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            //selectListItems.Add(new SelectListItem
            //{
            //    Text = "- All -",
            //    Value = "",
            //    Selected = true
            //});

            //using (EntitiesRegistration db = new EntitiesRegistration())
            //{
            //    var UserAccountSQL = from sv in db.C_S_USER_ACCOUNT
            //                         orderby sv.USERNAME
            //                         select sv;
            //    foreach (var item in UserAccountSQL)
            //    {
            //        string temp = item.USERNAME + " || " + item.FULLNAME;
            //        var UserAccountList = new SelectListItem();
            //        UserAccountList.Text = temp;
            //        UserAccountList.Value = item.UUID;
            //        selectListItems.Add(UserAccountList);
            //    }

            //}
            return selectListItems;
        }
        // static public List<SelectListItem> GetSystemCategoryCodeList()
        static public List<SelectListItem> GetASList(String FileRef)
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem
            {
                Text = "- Select -",
                Value = "",
                Selected = true
            });
            using (EntitiesRegistration db = new EntitiesRegistration())
            {

                var query = from info in db.C_COMP_APPLICANT_INFO
                            join comp in db.C_COMP_APPLICATION on info.MASTER_ID equals comp.UUID
                            join apnt in db.C_APPLICANT on info.APPLICANT_ID equals apnt.UUID
                            //join sv in db.C_S_SYSTEM_VALUE on comp1.APPLICATION_STATUS_ID equals sv.UUID
                            where comp.FILE_REFERENCE_NO == FileRef
                            select apnt;
                foreach (var item in query)
                {
                    selectListItems.Add(new SelectListItem { Text = item.SURNAME + " " + item.GIVEN_NAME_ON_ID, Value = item.UUID });

                }

            }
            return selectListItems;
        }
        static public List<SelectListItem> GetTDList(String FileRef)
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem
            {
                Text = "- Select -",
                Value = "",
                Selected = true
            });
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                selectListItems.AddRange((from info1 in db.C_COMP_APPLICANT_INFO
                                          join comp in db.C_COMP_APPLICATION on info1.MASTER_ID equals comp.UUID
                                          join apnt in db.C_APPLICANT on info1.APPLICANT_ID equals apnt.UUID
                                          join sv in db.C_S_SYSTEM_VALUE on comp.APPLICATION_STATUS_ID equals sv.UUID
                                          where comp.FILE_REFERENCE_NO == FileRef

                                          select new SelectListItem()
                                          {
                                              Text = apnt.SURNAME + apnt.GIVEN_NAME_ON_ID,
                                              Value = info1.UUID,
                                          }
                             ).ToList());
            }
            return selectListItems;
        }
        static public List<C_S_EXPORT_LETTER> LetterNumber(String RegType)

        {
            List<C_S_EXPORT_LETTER> selectListItems = new List<C_S_EXPORT_LETTER>();

            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                return db.C_S_EXPORT_LETTER.Where(o => o.REGISTRATION_TYPE == RegType).ToList();
            }

        }
        static public List<SelectListItem> GetMeetingGroup()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                int CurrentYear = DateTime.Now.Year;
                var MeetingSQL = from sv in db.C_MEETING
                                 where sv.YEAR == CurrentYear
                                 select sv;

                foreach (var item in MeetingSQL)
                {
                    string temp = item.YEAR.ToString() + "-" + item.MEETING_GROUP + "-" + item.MEETING_NO;
                    var MeetingList = new SelectListItem();
                    MeetingList.Text = temp;
                    MeetingList.Value = item.UUID;
                    selectListItems.Add(MeetingList);
                }
            }
            return selectListItems;
        }

        static public List<SelectListItem> GetValidityPeriodListByType()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem
            {
                Text = "- Select -",
                Value = "",
                Selected = true
            });
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                selectListItems.AddRange((
                                        from sv in GetSVListByType(RegistrationConstant.SYSTEM_TYPE_PERIOD_OF_VALIDITY)
                                        select new SelectListItem()
                                        {
                                            Text = sv.CODE,
                                            Value = sv.UUID,
                                        }
                             ).ToList());

            }
            return selectListItems;
        }
        static public List<SelectListItem> GetFormList(string RegType)
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem
            {
                Text = "- All -",
                Value = "",
                Selected = true
            });
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                selectListItems.AddRange((
                                        from sv in GetSVListByType(RegistrationConstant.SYSTEM_TYPE_APPLICATION_FORM)
                                        where sv.REGISTRATION_TYPE == RegType
                                        select new SelectListItem()
                                        {
                                            Text = sv.CODE,
                                            Value = sv.UUID,
                                        }
                             ).ToList());

            }
            return selectListItems;
        }
        static public List<SelectListItem> GetSignatureNameList()
        {
            return null;
        }
        static public List<SelectListItem> RetrievePNAPByType()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem
            {
                Text = "- All -",
                Value = "",
                Selected = true
            });
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                selectListItems.AddRange((
                    from sv in GetSVListByType(RegistrationConstant.SYSTEM_TYPE_PRACTICE_NOTE)
                    select new SelectListItem()
                    {
                        Text = sv.ENGLISH_DESCRIPTION,
                        Value = sv.UUID,
                    }
                             ).ToList());

            }
            return selectListItems;


        }
        static public List<C_S_SYSTEM_VALUE> GetSVListByParentUUID(string pUUID)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                return db.C_S_SYSTEM_VALUE.Where(o => o.PARENT_ID == pUUID && o.IS_ACTIVE == "Y").OrderBy(o => o.ORDERING).ToList();
            }
        }
        static public List<C_S_SYSTEM_VALUE> GetSVListByRegTypeNType(string regType, string type)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                return db.C_S_SYSTEM_VALUE.Where(o => o.C_S_SYSTEM_TYPE.TYPE == type && o.REGISTRATION_TYPE == regType && o.IS_ACTIVE == "Y").OrderBy(o => o.ORDERING).ToList();
            }
        }
        static public C_S_SYSTEM_VALUE GetSVByUUID(string uuid)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                return db.C_S_SYSTEM_VALUE.Where(x => x.UUID == uuid).FirstOrDefault();
            }
        }
        static public void AddSerialNoOrdering(string type)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                var query = (from sv in db.C_S_SYSTEM_VALUE
                             join st in db.C_S_SYSTEM_TYPE on sv.SYSTEM_TYPE_ID equals st.UUID
                             where st.TYPE == type && sv.IS_ACTIVE == "Y"
                             orderby sv.ORDERING
                             select sv).FirstOrDefault();

                query.ORDERING += 1;
                db.SaveChanges();

            }

        }
        static public List<C_S_SYSTEM_VALUE> GetSVListByType(string type)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                var query = from sv in db.C_S_SYSTEM_VALUE
                            join st in db.C_S_SYSTEM_TYPE on sv.SYSTEM_TYPE_ID equals st.UUID
                            where st.TYPE == type && sv.IS_ACTIVE == "Y"
                            orderby sv.ORDERING
                            select sv;

                return query.ToList();

            }
        }

        static public List<SelectListItem> GetTradeListItem()
        {
            using(EntitiesRegistration db = new EntitiesRegistration())
            {
                List<SelectListItem> result = new List<SelectListItem>();
                var query = db.C_S_MW_IND_CAPA_MAIN.OrderBy(x => x.ORDERING).ToList();
                result.Add(new SelectListItem() { Text = " - Select Trade - ", Value = "" });
                foreach(var item in query)
                {
                    result.Add(new SelectListItem() { Text = item.DESCRIPTION, Value = item.UUID });
                }
                return result;
            }
        }

        //static public List<C_S_USER_GROUP_CONV_INFO> GetCNVListbySysPostId(string SysRoleId)
        //{

        //    using (EntitiesRegistration db = new EntitiesRegistration())
        //    {
        //        var query = from gci in db.C_S_USER_GROUP_CONV_INFO
        //                    join sv in db.C_S_SYSTEM_VALUE on gci.CONVICTION_ID equals sv.UUID
        //                    where gci.SYS_ROLE_ID == SysRoleId
        //                    select gci;

        //        return query.ToList();

        //    }
        //}
        static public C_S_CATEGORY_CODE GetCatCodeByUUID(string uuid)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                return db.C_S_CATEGORY_CODE.Where(x => x.UUID == uuid).FirstOrDefault();
            }
        }
        static public List<C_S_CATEGORY_CODE> GetCatCodeByRegType(string regType)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                return db.C_S_CATEGORY_CODE.Where(x => x.REGISTRATION_TYPE == regType && x.ACTIVE == "Y").ToList();
            }
        }
        //static public List<SelectListItem> RetrievePRBByType()
        //{


        //    return (from sv in GetSVListByType(RegistrationConstant.SYSTEM_TYPE_PROF_REGISTRATION_BOARD)
        //            select new SelectListItem()
        //            {
        //                Text = sv.ENGLISH_DESCRIPTION,
        //                Value = sv.UUID,
        //            }
        //            ).ToList();




        //}
        static public List<C_S_CATEGORY_CODE> RetrieveCatCodeByType(string RegType)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                return db.C_S_CATEGORY_CODE.Where(x => x.REGISTRATION_TYPE == RegType && x.ACTIVE == "Y").OrderBy(x => x.CODE).ToList();
            }

        }

        static public List<SelectListItem> RetrieveQCByType(string RegType)
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();

            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                selectListItems.AddRange((from catCode in db.C_S_CATEGORY_CODE

                                          where catCode.REGISTRATION_TYPE == RegType && catCode.ACTIVE == "Y"
                                          orderby catCode.CODE
                                          select new SelectListItem()
                                          {
                                              Text = catCode.CODE,
                                              Value = catCode.UUID,
                                          }
                             ).ToList());

            }
            return selectListItems;


        }
        static public List<SelectListItem> RetrieveDisDivByType()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();

            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                selectListItems.AddRange((from catCodeDetail in db.C_S_CATEGORY_CODE_DETAIL
                                          orderby catCodeDetail.CODE
                                          select new SelectListItem()
                                          {
                                              Text = catCodeDetail.ENGLISH_DESCRIPTION,
                                              Value = catCodeDetail.UUID,
                                          }
                             ).ToList());

            }
            return selectListItems;


        }
        static public List<C_S_CATEGORY_CODE_DETAIL> GetCatCodeDetailByCode(string code)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                return (from catCodeDetail in db.C_S_CATEGORY_CODE_DETAIL
                        where catCodeDetail.C_S_CATEGORY_CODE.CODE == code
                        orderby catCodeDetail.CODE
                        select catCodeDetail
                              ).ToList();

            }


        }
        //static public List<SelectListItem> RetrieveCNVSOURCE()
        //{
        //    List<SelectListItem> selectListItems = new List<SelectListItem>();
        //    selectListItems.Add(new SelectListItem
        //    {
        //        Text = "- All -",
        //        Value = "",
        //        Selected = true
        //    });
        //    using (EntitiesRegistration db = new EntitiesRegistration())
        //    {
        //        selectListItems.AddRange((from sv in db.C_S_SYSTEM_VALUE
        //                                  join st in db.C_S_SYSTEM_TYPE on sv.SYSTEM_TYPE_ID equals st.UUID
        //                                  where st.TYPE == RegistrationConstant.SYSTEM_TYPE_CONVICTION_SOURCE && sv.IS_ACTIVE == "Y"
        //                                  orderby sv.ORDERING
        //                                  select new SelectListItem()
        //                                  {
        //                                      Text = sv.ENGLISH_DESCRIPTION,
        //                                      Value = sv.UUID,
        //                                  }
        //                       ).ToList());


        //    }
        //    return selectListItems;


        //}
        static public List<SelectListItem> RetrieveMWCap()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem
            {
                Text = "- Select Minor Work Class -",
                Value = "",
                Selected = true
            });

            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                selectListItems.AddRange((from sv in db.C_S_SYSTEM_VALUE
                                          join st in db.C_S_SYSTEM_TYPE on sv.SYSTEM_TYPE_ID equals st.UUID
                                          where st.TYPE == RegistrationConstant.SYSTEM_TYPE_MWCLASS
                                          orderby sv.ORDERING
                                          select new SelectListItem()
                                          {
                                              Text = sv.CODE,
                                              Value = sv.CODE,
                                          }
                               ).ToList());


            }
            return selectListItems;


        }
        static public List<SelectListItem> RetrieveMWTypeByClass(string mwclass)
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem
            {
                Text = "- Select Minor Work Type -",
                Value = "",
                Selected = true
            });

            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                selectListItems.AddRange((from sv in GetSVListByType(RegistrationConstant.SYSTEM_TYPE_MWTYPE)
                                          where sv.PARENT_ID == ((from sv1 in GetSVListByType(RegistrationConstant.SYSTEM_TYPE_MWCLASS) select sv1.UUID).FirstOrDefault())
                                          orderby sv.ORDERING
                                          select new SelectListItem()
                                          {
                                              Text = sv.CODE,

                                              Value = sv.UUID,

                                          }
                               ).ToList());


            }
            return selectListItems;


        }
        static public List<SelectListItem> RetrieveQPService()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem
            {
                Text = "- All -",
                Value = "",
                Selected = true
            });
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                selectListItems.AddRange((from sv in GetSVListByType(RegistrationConstant.SYSTEM_TYPE_QPSERVICES)
                                          select new SelectListItem()
                                          {
                                              Text = sv.ENGLISH_DESCRIPTION,
                                              Value = sv.CODE,
                                          }
                               ).ToList());


            }
            return selectListItems;
        }
        static public List<SelectListItem> RetrieveQPServiceInMWIS()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem
            {
                Text = "- All -",
                Value = "",
                Selected = true
            });
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                selectListItems.AddRange((from sv in GetSVListByType(RegistrationConstant.SYSTEM_TYPE_MWIS_DROPDOWN)
                                          select new SelectListItem()
                                          {
                                              Text = sv.ENGLISH_DESCRIPTION,
                                              Value = sv.CODE,
                                          }
                               ).ToList());


            }
            return selectListItems;
        }

        static public List<SelectListItem> RetrieveQCType()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem
            {
                Text = "- Select -",
                Value = "",
                Selected = true
            });
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                selectListItems.AddRange((from sv in GetSVListByType(RegistrationConstant.SYSTEM_TYPE_QUALIFICATION_TYPE)
                                          select new SelectListItem()
                                          {
                                              Text = sv.CODE,
                                              Value = sv.CODE,
                                          }
                               ).ToList());


            }
            return selectListItems;


        }
        static public List<C_S_ROOM> GetRoomLists()
        {

            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                return (from sr in db.C_S_ROOM select sr).ToList();
            }



        }
        static public C_S_ROOM GetRoomByUUID(string id)
        {

            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                return (from sr in db.C_S_ROOM where sr.UUID == id select sr).FirstOrDefault();
            }



        }
        static public List<SelectListItem> GetRoomList()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem
            {
                Text = "- All -",
                Value = "",
                Selected = true
            });

            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                selectListItems.AddRange((from sr in db.C_S_ROOM


                                          select new SelectListItem()
                                          {
                                              Text = sr.ROOM,
                                              Value = sr.ROOM,

                                          }
                               ).ToList());


            }
            return selectListItems;


        }


        static public List<SelectListItem> RetrieveRegType()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem
            {
                Text = "- All -",
                Value = "ALL",
                Selected = true
            });

            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                selectListItems.AddRange((from sv in GetSVListByType(RegistrationConstant.SYSTEM_TYPE_REGISTRATION_TYPE)

                                          select new SelectListItem()
                                          {
                                              Text = sv.ENGLISH_DESCRIPTION,
                                              Value = sv.CODE,

                                          }
                               ).ToList());


            }
            return selectListItems;


        }

        static public List<SelectListItem> GetGroupList()
        {
            return new List<SelectListItem>{
                    new SelectListItem { Text = "- All -", Value = ""}
                    , new SelectListItem { Text = "A", Value = "A"}
                    , new SelectListItem { Text = "B", Value = "B"}
                    , new SelectListItem { Text = "C", Value = "C"}
                    , new SelectListItem { Text = "D", Value = "D"}
                    , new SelectListItem { Text = "E", Value = "E"}
                    , new SelectListItem { Text = "F", Value = "F"}
                    , new SelectListItem { Text = "G", Value = "G"}
                    , new SelectListItem { Text = "H", Value = "H"}
                    , new SelectListItem { Text = "I", Value = "I"}
                    , new SelectListItem { Text = "J", Value = "J"}
                    , new SelectListItem { Text = "K", Value = "K"}
                    , new SelectListItem { Text = "L", Value = "L"}
                    , new SelectListItem { Text = "M", Value = "M"}
                    , new SelectListItem { Text = "N", Value = "N"}
                    , new SelectListItem { Text = "O", Value = "O"}
                    , new SelectListItem { Text = "P", Value = "P"}
                    , new SelectListItem { Text = "Q", Value = "Q"}
                    , new SelectListItem { Text = "R", Value = "R"}
                    , new SelectListItem { Text = "S", Value = "S"}
                    , new SelectListItem { Text = "T", Value = "T"}
                    , new SelectListItem { Text = "U", Value = "U"}
                    , new SelectListItem { Text = "V", Value = "V"}
                    , new SelectListItem { Text = "W", Value = "W"}
                    , new SelectListItem { Text = "X", Value = "X"}
                    , new SelectListItem { Text = "Y", Value = "Y"}
                    , new SelectListItem { Text = "Z", Value = "Z"}
                };
        }


        static public List<SelectListItem> RetrieveCategoryListByRegType(string RegType)
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem
            {
                Text = "- All -",
                Value = "",
                Selected = true
            });
            using (EntitiesRegistration db = new EntitiesRegistration())
            {   /*
                selectListItems.AddRange((from sCatCode in db.C_S_CATEGORY_CODE
                                          join sv in db.C_S_SYSTEM_VALUE on sCatCode.CATEGORY_GROUP_ID equals sv.UUID
                                          join sType in db.C_S_SYSTEM_TYPE on sv.SYSTEM_TYPE_ID equals sType.UUID
                                          where 1 == 1
                                          && sv.REGISTRATION_TYPE == RegType
                                          && sv.IS_ACTIVE == "Y"
                                          orderby sv.ORDERING, sCatCode.CODE
                                          select new SelectListItem()
                                          {
                                              Text = sCatCode.CODE,
                                              Value = sv.UUID,
                                          }
                             ).ToList());
                             */
                selectListItems.AddRange((
                                          from sv in db.C_S_SYSTEM_VALUE
                                          join sType in db.C_S_SYSTEM_TYPE on sv.SYSTEM_TYPE_ID equals sType.UUID
                                          where 1 == 1
                                          && sv.REGISTRATION_TYPE == RegType
                                          && sType.TYPE == "COMMITTEE_TYPE"
                                          && sv.IS_ACTIVE == "Y"
                                          orderby sv.ORDERING
                                          select new SelectListItem()
                                          {
                                              Text = sv.CODE,
                                              Value = sv.UUID,
                                          }
                   ).ToList());
            }
            return selectListItems;
        }


        static public List<SelectListItem> RetrieveTypeByCommitteeTypeID(string RegType)
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
                Text = "MWCRC",
                Value = "MWCRC",
                //Selected = true
            });
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                selectListItems.AddRange((
                                            //                               //from t1 in db.C_INTERVIEW_SCHEDULE
                                            //                               //join t2 in db.C_MEETING on t1.MEETING_ID equals t2.UUID
                                            //                               //join t3 in db.C_S_SYSTEM_VALUE on t2.COMMITTEE_TYPE_ID equals t3.UUID
                                            //                               //join t4 in db.C_S_SYSTEM_TYPE on t3.SYSTEM_TYPE_ID equals t4.UUID
                                            //                               //where 1 == 1
                                            //                               //&& t3.REGISTRATION_TYPE == RegType
                                            //                               //&& t3.UUID == "MWCRC"

                                            //select new SelectListItem()
                                            //{
                                            //    Text = t3.CODE,
                                            //    Value = t3.UUID,
                                            //}
                                            from sv in db.C_S_SYSTEM_VALUE
                                            join sType in db.C_S_SYSTEM_TYPE on sv.SYSTEM_TYPE_ID equals sType.UUID
                                            where 1 == 1
                                            && sv.REGISTRATION_TYPE == RegType
                                            && sType.TYPE == "COMMITTEE_TYPE"
                                            //&& sv.CODE == "MWCRC"
                                            && sv.IS_ACTIVE == "Y"
                                            orderby sv.ORDERING
                                            select new SelectListItem()
                                            {
                                                Text = sv.CODE,
                                                Value = sv.UUID,
                                            }
                    ).ToList());
            }
            return selectListItems;
        }

        static public List<SelectListItem> RetrieveYearList()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem
            {
                Text = "- All -",
                Value = "",
                Selected = true
            });

            int currYear = int.Parse(DateTime.Now.Year.ToString());
            int nextYear = currYear + 1;

            for (int year = nextYear; year >= nextYear - 11; year--)
            {
                selectListItems.Add(new SelectListItem
                {
                    Text = year.ToString(),
                    Value = year.ToString(),
                });
            }
            return selectListItems;
        }
        static public List<SelectListItem> RetrieveSAuthorityList()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem
            {
                Text = "- All -",
                Value = "",
                Selected = true
            });
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                selectListItems.AddRange((from sa in db.C_S_AUTHORITY
                                          orderby sa.ENGLISH_NAME
                                          select new SelectListItem()
                                          {
                                              Text = sa.ENGLISH_NAME,
                                              Value = sa.UUID,
                                          }
                             ).ToList());

            }
            return selectListItems;

        }
        static public List<SelectListItem> GetMissItem(string missingValue)
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>{

                    new SelectListItem { Text = "Null", Value = "0"}
                    , new SelectListItem { Text = "Cheque", Value = "1"}
                    , new SelectListItem { Text = "Professional Registration Certificate", Value = "2"}
                    , new SelectListItem { Text = "Others", Value = "3"}
                    , new SelectListItem { Text = "Incomplete Form", Value = "4"}
                };

            foreach (var item in selectListItems)
            {
                if (string.IsNullOrWhiteSpace(missingValue))
                { }
                else if (missingValue.Contains(item.Value))
                {
                    item.Selected = true;
                }
            }
            return selectListItems;
        }//new add
        static public List<SelectListItem> InterviewResultList(string missingValue)
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>{

                    new SelectListItem { Text = "Null", Value = "0"}
                    , new SelectListItem { Text = "Cheque", Value = "1"}
                    , new SelectListItem { Text = "Professional Registration Certificate", Value = "2"}
                    , new SelectListItem { Text = "Others", Value = "3"}
                    , new SelectListItem { Text = "Incomplete Form", Value = "4"}
                };

            foreach (var item in selectListItems)
            {
                if (string.IsNullOrWhiteSpace(missingValue))
                { }
                else if (missingValue.Contains(item.Value))
                {
                    item.Selected = true;
                }
            }
            return selectListItems;
        }
        static public List<SelectListItem> GetSecretaryList()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem
            {
                Text = " ",
                Value = "",
                Selected = true
            });
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                selectListItems.AddRange((
                                        from sv in GetSVListByType(RegistrationConstant.SYSTEM_TYPE_SECRETARY)
                                        select new SelectListItem()
                                        {
                                            Text = sv.CODE,
                                            Value = sv.CODE,
                                        }
                             ).ToList());

            }
            return selectListItems;
        }
        static public List<SelectListItem> GetAssistantList()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem
            {
                Text = " ",
                Value = "",
                Selected = true
            });
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                selectListItems.AddRange((
                                        from sv in GetSVListByType(RegistrationConstant.SYSTEM_TYPE_ASSISTANT)
                                        select new SelectListItem()
                                        {
                                            Text = sv.CODE,
                                            Value = sv.CODE,
                                        }
                             ).ToList());

            }
            return selectListItems;
        }
        static public List<SelectListItem> GetVetOfficer()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem
            {
                Text = " ",
                Value = "",
                Selected = true
            });
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                selectListItems.AddRange((
                                        from sv in GetSVListByType(RegistrationConstant.SYSTEM_TYPE_VETTING_OFFICER)
                                        select new SelectListItem()
                                        {
                                            Text = sv.CODE,
                                            Value = sv.CODE,
                                        }
                             ).ToList());

            }
            return selectListItems;
        }
        static public List<SelectListItem> GetVetCIHROfficer()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem
            {
                Text = "- All -",
                Value = "",
                Selected = true
            });
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                selectListItems.AddRange((
                                        from sv in GetSVListByType(RegistrationConstant.SYSTEM_TYPE_VETTING_OFFICER)
                                        select new SelectListItem()
                                        {
                                            Text = sv.CODE,
                                            Value = sv.CODE,
                                        }
                             ).ToList());

            }
            return selectListItems;
        }
        static public List<SelectListItem> GetEditVetOfficer()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem
            {
                Text = " ",
                Value = "",
                Selected = true
            });
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                selectListItems.AddRange((
                                        from sv in GetSVListByType(RegistrationConstant.SYSTEM_TYPE_VETTING_OFFICER)
                                        select new SelectListItem()
                                        {
                                            Text = sv.CODE,
                                            Value = sv.CODE,
                                        }
                             ).ToList());

            }
            return selectListItems;
        }
        static public List<SelectListItem> GetAppApplyType(string ApplyValue)
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>{

                    new SelectListItem { Text = "Addition", Value = "A"}
                    , new SelectListItem { Text = "Inclusion", Value = "I"}
                    , new SelectListItem { Text = "Renewal", Value = "N"}
                    , new SelectListItem { Text = "Restoration", Value = "S"}
                    , new SelectListItem { Text = "Application after Deferral", Value = "D"}
                    , new SelectListItem { Text = "QP Card", Value = "Q"}
                };

            foreach (var item in selectListItems)
            {
                if (string.IsNullOrWhiteSpace(ApplyValue))
                { }
                else if (ApplyValue.Contains(item.Value))
                {
                    item.Selected = true;
                }
            }
            return selectListItems;
        }
        static public List<SelectListItem> GetCMWAppApplyType(string ApplyValue)
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>{

                    new SelectListItem { Text = "Addition", Value = "A"}
                    , new SelectListItem { Text = "Inclusion", Value = "I"}
                    , new SelectListItem { Text = "Renewal", Value = "N"}
                    , new SelectListItem { Text = "Restoration", Value = "S"}
                    , new SelectListItem { Text = "Application after Deferral", Value = "D"}
                    , new SelectListItem { Text = "Review", Value = "R"}
                    , new SelectListItem { Text = "QP Card", Value = "Q"}
                };

            foreach (var item in selectListItems)
            {
                if (string.IsNullOrWhiteSpace(ApplyValue))
                { }
                else if (ApplyValue.Contains(item.Value))
                {
                    item.Selected = true;
                }
            }
            return selectListItems;
        }
        static public List<SelectListItem> GetIndAppApplyType(string ApplyValue)
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>{
                    new SelectListItem { Text = "Inclusion", Value = "I"}
                    , new SelectListItem { Text = "Renewal", Value = "N"}
                    , new SelectListItem { Text = "Restoration", Value = "E"}
                    , new SelectListItem { Text = "Addition", Value = "A"}
                    , new SelectListItem { Text = "Review", Value = "R"}
                    , new SelectListItem { Text = "QP Card", Value = "Q"}
                };

            foreach (var item in selectListItems)
            {
                if (string.IsNullOrWhiteSpace(ApplyValue))
                { }
                else if (ApplyValue.Contains(item.Value))
                {
                    item.Selected = true;
                }
            }
            return selectListItems;
        }
        static public List<C_S_SYSTEM_VALUE> GetSystemValueByRegistrationTypeAndCode(string type, string regType, string code)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                var query = from sv in db.C_S_SYSTEM_VALUE
                            join st in db.C_S_SYSTEM_TYPE on sv.SYSTEM_TYPE_ID equals st.UUID
                            where st.TYPE == type && sv.CODE == code && sv.REGISTRATION_TYPE == regType
                            orderby sv.ORDERING
                            select sv;

                return query.ToList();

            }

        }
        static public List<SelectListItem> GetTypeOfAppList()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                selectListItems.AddRange((
                                        from sv in GetSystemValueByRegistrationTypeAndCode(RegistrationConstant.SYSTEM_TYPE_APPLICATION_FORM, RegistrationConstant.REGISTRATION_TYPE_CGA, "BA2C")
                                        select new SelectListItem()
                                        {
                                            Text = sv.CODE,
                                            Value = sv.UUID,
                                        }
                             ).ToList());

            }
            return selectListItems;
        }
        static public List<SelectListItem> GetNewTypeOfAppList(string RegType)
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                selectListItems.AddRange((
                                        from sv in GetSVListByType(RegistrationConstant.SYSTEM_TYPE_APPLICATION_FORM)
                                        where sv.REGISTRATION_TYPE == RegType
                                        select new SelectListItem()
                                        {
                                            Text = sv.CODE,
                                            Value = sv.UUID,
                                        }
                             ).ToList());

            }
            return selectListItems;
        }
        static public List<SelectListItem> GetInterResult(string RegType)
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                selectListItems.AddRange((
                                        from sv in GetSVListByType(RegistrationConstant.SYSTEM_TYPE_INTERVIEW_RESULT)
                                        where sv.REGISTRATION_TYPE == RegType
                                        select new SelectListItem()
                                        {
                                            Text = sv.ENGLISH_DESCRIPTION,
                                            Value = sv.UUID,
                                        }
                             ).ToList());

            }
            return selectListItems;
        }
        static public List<SelectListItem> GetTypeOfPM(string NatureValue)
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>{

                    new SelectListItem { Text = "Interview", Value = "Interview"}
                    , new SelectListItem { Text = "Assessment", Value = "Assessment"}
                };

            foreach (var item in selectListItems)
            {
                if (string.IsNullOrWhiteSpace(NatureValue))
                { }
                else if (NatureValue.Contains(item.Value))
                {
                    item.Selected = true;
                }
            }
            return selectListItems;
        }
        static public List<SelectListItem> GetNature(string NatureValue)
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>{

                    new SelectListItem { Text = "N", Value = "N"}
                    , new SelectListItem { Text = "D", Value = "D"}
                };

            foreach (var item in selectListItems)
            {
                if (string.IsNullOrWhiteSpace(NatureValue))
                { }
                else if (NatureValue.Contains(item.Value))
                {
                    item.Selected = true;
                }
            }
            return selectListItems;
        }
        static public List<SelectListItem> GetTypeOfRegisters()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>{

                    new SelectListItem { Text = "AP", Value = "N"}
                    , new SelectListItem { Text = "RSE", Value = "RSE"}
                    , new SelectListItem { Text = "RGE", Value = "RGE"}
                    , new SelectListItem { Text = "RI", Value = "RI"}
                    , new SelectListItem { Text = "GBC", Value = "GBC"}
                    , new SelectListItem { Text = "SC(D)", Value = "SC(D)"}
                    , new SelectListItem { Text = "SC(F)", Value = "SC(F)"}
                    , new SelectListItem { Text = "SC(GI)", Value = "SC(GI)"}
                    , new SelectListItem { Text = "SC(SF)", Value = "SC(SF)"}
                    , new SelectListItem { Text = "SC(V)", Value = "SC(V)"}
                    , new SelectListItem { Text = "MWC", Value = "MWC"}
                    , new SelectListItem { Text = "MWC(P)", Value = "MWC(P)"}
                    , new SelectListItem { Text = "MWC(W)", Value = "MWC(W)"}
                };
            return selectListItems;
        }
        static public List<SelectListItem> GetTypeOfCategory()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>{
                    new SelectListItem { Text = "AP/RSE/RGE/RI", Value = "IP"}
                    , new SelectListItem { Text = "GBC", Value = "GBC"}
                    , new SelectListItem { Text = "SC", Value = "SC"}
                    , new SelectListItem { Text = "MWC", Value = "MWC"}
                    , new SelectListItem { Text = "MWC(P)", Value = "MWC(P)"}
                    , new SelectListItem { Text = "MWC(W)", Value = "MWC(W)"}
                };
            return selectListItems;
        }
        static public List<SelectListItem> GetTwoMonthCase(string TwoMonthCaseValue)
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>{

                    new SelectListItem { Text = "Y", Value = "Y"}
                    , new SelectListItem { Text = "N", Value = "N"}
                };

            foreach (var item in selectListItems)
            {
                if (string.IsNullOrWhiteSpace(TwoMonthCaseValue))
                { }
                else if (TwoMonthCaseValue.Contains(item.Value))
                {
                    item.Selected = true;
                }
            }
            return selectListItems;
        }
        static public List<SelectListItem> GetFastTrack(string FastTrackValue)
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>{

                    new SelectListItem { Text = "Y", Value = "Y"}
                    , new SelectListItem { Text = "N", Value = "N"}
                };

            foreach (var item in selectListItems)
            {
                if (string.IsNullOrWhiteSpace(FastTrackValue))
                { }
                else if (FastTrackValue.Contains(item.Value))
                {
                    item.Selected = true;
                }
            }
            return selectListItems;
        }



        static public string GetMWItemDetailList(string id, string status)
        {

            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                string MWItem = "";
                var query = from mwd in db.C_IND_APPLICATION_MW_ITEM_DETAIL
                            join sv in db.C_S_SYSTEM_VALUE on mwd.ITEM_DETAILS_ID equals sv.UUID
                            where mwd.STATUS_CODE == status && mwd.IND_APP_MW_ITEM_MASTER_ID == id
                            select sv.CODE;
                foreach (var item in query)
                {
                    MWItem += item.Substring(4) + ",";
                }
                MWItem = MWItem.Substring(0, MWItem.Length - 1);
                return MWItem;
            }

        }

        static public List<SelectListItem> GetCategoryCodeList()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();

            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                selectListItems.AddRange((from catcode in db.C_S_CATEGORY_CODE
                                          join sv in db.C_S_SYSTEM_VALUE on catcode.CATEGORY_GROUP_ID equals sv.UUID
                                          select new SelectListItem()
                                          {
                                              Text = catcode.REGISTRATION_TYPE + " - " + catcode.CODE,
                                              Value = catcode.UUID,

                                          }
                             ).ToList());

            }
            return selectListItems;

        }

        static public List<SelectListItem> GetSystemCategoryCodeList()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();

            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                selectListItems.AddRange((from sv in db.C_S_SYSTEM_VALUE
                                          join st in db.C_S_SYSTEM_TYPE on sv.SYSTEM_TYPE_ID equals st.UUID
                                          where st.TYPE == "CATEGORY_GROUP"
                                          select new SelectListItem()
                                          {
                                              Text = sv.REGISTRATION_TYPE + " - " + sv.CODE,
                                              Value = sv.UUID,
                                          }
                             ).ToList());

            }
            return selectListItems;

        }

        static public List<SelectListItem> GetRegistrationTypeList()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();

            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                selectListItems.AddRange((from sv in db.C_S_SYSTEM_VALUE
                                          join st in db.C_S_SYSTEM_TYPE on sv.SYSTEM_TYPE_ID equals st.UUID
                                          where st.TYPE == "REGISTRATION_TYPE"
                                          select new SelectListItem()
                                          {
                                              Text = sv.ENGLISH_DESCRIPTION,
                                              Value = sv.CODE,
                                          }
                             ).ToList());

            }
            return selectListItems;

        }

        //static public List<SelectListItem> GetCompanyTypeList()
        //{
        //    List<SelectListItem> selectListItems = new List<SelectListItem>();
        //    selectListItems.Add(new SelectListItem
        //    {
        //        Text = "- All -",
        //        Value = "",
        //        Selected = true
        //    });
        //    using (EntitiesRegistration db = new EntitiesRegistration())
        //    {
        //        selectListItems.AddRange((from sv in db.C_S_SYSTEM_VALUE
        //                                  join st in db.C_S_SYSTEM_TYPE on sv.SYSTEM_TYPE_ID equals st.UUID
        //                                  where st.TYPE == "COMPANY_TYPE"

        //                                  select new SelectListItem()
        //                                  {
        //                                      Text = sv.ENGLISH_DESCRIPTION,
        //                                      Value = sv.UUID,
        //                                  }
        //                     ).ToList());

        //    }

        static public List<SelectListItem> GetSystemValueCodeList()
        {
            return new List<SelectListItem>{
                    new SelectListItem { Text = "1", Value = "1"}
                    , new SelectListItem { Text = "2", Value = "2"}
                    , new SelectListItem { Text = "3", Value = "3"}
                    , new SelectListItem { Text = "4", Value = "4"}
                    , new SelectListItem { Text = "-", Value = ""}


                };
        }
        static public List<SelectListItem> GetPoolingList()
        {
            return new List<SelectListItem>{
                    new SelectListItem { Text = "All", Value = ""}
                    , new SelectListItem { Text = "Yes", Value = "1"}
                    , new SelectListItem { Text = "No", Value = "2"}
            };
        }










        static public List<C_S_SYSTEM_VALUE> GetMWItemApplyApprovedDetailList(string id, string status)
        {

            using (EntitiesRegistration db = new EntitiesRegistration())
            {

                var query = (from mwd in db.C_IND_APPLICATION_MW_ITEM_DETAIL
                             join sv in db.C_S_SYSTEM_VALUE on mwd.ITEM_DETAILS_ID equals sv.UUID
                             where mwd.STATUS_CODE == status && mwd.IND_APP_MW_ITEM_MASTER_ID == id
                             select sv).ToList();
                //string MWItem = "";
                //foreach (var item in query)
                //{
                //    MWItem += item.Substring(4)+",";
                //}
                //MWItem= MWItem.Substring(0, MWItem.Length - 1);
                //return MWItem;
                query = query.OrderBy(x => int.Parse(x.CODE.Substring(7))).ToList();
                return query;
            }

        }
        static public List<C_IND_APPLICATION_MW_ITEM> GetLatesetMWItemApprovedDetailList(string id)
        {

            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                var query = (from mw in db.C_IND_APPLICATION_MW_ITEM
                             join sv in db.C_S_SYSTEM_VALUE on mw.ITEM_DETAILS_ID equals sv.UUID
                             where mw.MASTER_ID == id
                             select mw).ToList();
                //var query = (from mwd in db.C_IND_APPLICATION_MW_ITEM_DETAIL
                //             join sv in db.C_S_SYSTEM_VALUE on mwd.ITEM_DETAILS_ID equals sv.UUID
                //             where mwd.STATUS_CODE == RegistrationConstant.MWITEM_APPROVED 
                //             && mwd.IND_APP_MW_ITEM_MASTER_ID == id
                //             select mwd).ToList();

                //query = query.OrderBy(x => int.Parse(x.CODE.Substring(7))).ToList();
                return query;
            }

        }
        static public List<C_IND_APPLICATION_MW_ITEM_DETAIL> GetSelectedMWItemApprovedDetailList(string id)
        {

            using (EntitiesRegistration db = new EntitiesRegistration())
            {

                var query = (from mwd in db.C_IND_APPLICATION_MW_ITEM_DETAIL
                             join sv in db.C_S_SYSTEM_VALUE on mwd.ITEM_DETAILS_ID equals sv.UUID
                             where mwd.STATUS_CODE == RegistrationConstant.MWITEM_APPROVED
                             && mwd.IND_APP_MW_ITEM_MASTER_ID == id
                             select mwd).ToList();

                //query = query.OrderBy(x => int.Parse(x.CODE.Substring(7))).ToList();
                return query;
            }

        }

        static public List<C_IND_APPLICATION_MW_ITEM_DETAIL> GetSelectedMWItemApplyDetailList(string id)
        {

            using (EntitiesRegistration db = new EntitiesRegistration())
            {

                var query = (from mwd in db.C_IND_APPLICATION_MW_ITEM_DETAIL
                             join sv in db.C_S_SYSTEM_VALUE on mwd.ITEM_DETAILS_ID equals sv.UUID
                             where mwd.STATUS_CODE == RegistrationConstant.MWITEM_APPLY
                             && mwd.IND_APP_MW_ITEM_MASTER_ID == id
                             select mwd).ToList();

                //query = query.OrderBy(x => int.Parse(x.CODE.Substring(7))).ToList();
                return query;
            }

        }

        static public List<C_S_SYSTEM_VALUE> GetMWItemFullListByClass(string classType)
        {

            using (EntitiesRegistration db = new EntitiesRegistration())
            {

                var query = from sv in GetSVListByType(RegistrationConstant.SYSTEM_TYPE_MWITEM)
                            join sv2 in db.C_S_SYSTEM_VALUE on sv.PARENT_ID equals sv2.UUID
                            where sv.CODE.Contains("Item 3.")
                            orderby sv2.CODE
                            select sv;
                /// var OrderedQuery = query.ToList().OrderBy(o => int.Parse(o.CODE.Substring(7))).OrderBy(o => o.CODE);
                //var query = from sv in db.C_S_SYSTEM_VALUE
                //            join st in db.C_S_SYSTEM_TYPE on sv.SYSTEM_TYPE_ID equals st.UUID
                //            where st.TYPE == RegistrationConstant.SYSTEM_TYPE_MWITEM
                //            && sv.IS_ACTIVE == "Y"
                //            && sv.CODE.Contains("Item 3.")
                //            orderby sv.CODE
                //            //orderby int.Parse(sv.CODE.Substring(7)) ,sv.CODE
                //            select sv;
                List<C_S_SYSTEM_VALUE> svlist = new List<C_S_SYSTEM_VALUE>();
                List<C_S_SYSTEM_VALUE> tmpsvlist = query.ToList();
                tmpsvlist = tmpsvlist.OrderBy(o => int.Parse(o.CODE.Substring(7))).ToList();

                string tempItem = "";
                foreach (var item in tmpsvlist)
                {
                    string test = item.UUID;
                    if (tempItem != item.CODE)
                    {
                        tempItem = item.CODE;
                        svlist.Add(item);

                    }
                }

                // svlist = svlist.OrderBy(o => int.Parse(o.CODE.Substring(7))).ToList();
                return svlist;
            }

        }
        static public List<C_S_MW_IND_CAPA_MAIN> GetNewMWItemFullList()
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                return db.C_S_MW_IND_CAPA_MAIN.Where(x => x.UUID != null).OrderBy(x => x.CHECKBOX_CODE).Include(x => x.C_S_MW_IND_CAPA).ToList();

            }

        }
        static public C_S_MW_IND_CAPA_MAIN GetNewMWItemByUUID(string uuid)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                return db.C_S_MW_IND_CAPA_MAIN.Where(x => x.UUID == uuid).Include(x => x.C_S_MW_IND_CAPA).FirstOrDefault();

            }

        }
        static public C_COMMITTEE_GROUP GetCommitteeGroupByUUID(string id)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                var q = db.C_COMMITTEE_GROUP.Where(x => x.UUID == id)
                    .Include(x => x.C_COMMITTEE)
                    .Include(x => x.C_COMMITTEE.C_S_SYSTEM_VALUE)
                    .Include(x => x.C_COMMITTEE.C_COMMITTEE_PANEL)
                    .Include(x => x.C_COMMITTEE.C_COMMITTEE_PANEL.C_S_SYSTEM_VALUE)
                    .FirstOrDefault();



                return q;

            }
        }
        static public List<int> getNextYearAndLastTenYear()
        {
            List<int> list = new List<int>();

            for (int i = DateTime.Now.Year + 1; i >= DateTime.Now.Year - 10; i--)
            {
                list.Add(i);
            }
            return list;
        }
        static public List<int> getMonth()
        {
            List<int> list = new List<int>();

            for (int i = 1; i <= 12; i++)
            {
                list.Add(i);
            }
            return list;
        }

        public static string MonthEnglishName(string value)
        {
            string month = null;
            switch (value)
            {
                case "01":
                case "1":
                    month = "January";
                    break;
                case "02":
                case "2":
                    month = "February";
                    break;
                case "03":
                case "3":
                    month = "March";
                    break;
                case "04":
                case "4":
                    month = "April";
                    break;
                case "05":
                case "5":
                    month = "May";
                    break;
                case "06":
                case "6":
                    month = "June";
                    break;
                case "07":
                case "7":
                    month = "July";
                    break;
                case "08":
                case "8":
                    month = "August";
                    break;
                case "09":
                case "9":
                    month = "September";
                    break;
                case "10":
                    month = "October";
                    break;
                case "11":
                    month = "November";
                    break;
                case "12":
                    month = "December";
                    break;
            }
            return month;
        }

        static public List<string> getAtoZ()
        {
            List<string> list = new List<string>();

            for (char c = 'A'; c <= 'Z'; c++)
            {
                list.Add(c.ToString());
                //do something with letter 
            }
            return list;
        }
        static public List<C_COMMITTEE_GROUP> GetCommitteeGroupByRegTypeAndYear(string registrationType, int searchYear)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                if (registrationType == RegistrationConstant.REGISTRATION_TYPE_MWCA || registrationType == RegistrationConstant.REGISTRATION_TYPE_MWIA)
                {
                    registrationType = "MW";
                }
                bool tmpResult = string.IsNullOrWhiteSpace(registrationType);
                var q = db.C_COMMITTEE_GROUP.Where(x => x.YEAR == searchYear)
                    .Include(x => x.C_COMMITTEE)
                    .Include(x => x.C_COMMITTEE.C_S_SYSTEM_VALUE)
                    .Include(x => x.C_COMMITTEE.C_COMMITTEE_PANEL)
                    .Include(x => x.C_COMMITTEE.C_COMMITTEE_PANEL.C_S_SYSTEM_VALUE)
                    .Where(x => (tmpResult ? 1 == 1 : x.C_S_SYSTEM_VALUE.REGISTRATION_TYPE == registrationType))
                    .OrderBy(x => x.C_COMMITTEE.C_S_SYSTEM_VALUE.CODE)
                    .ThenBy(x => x.NAME);



                return q.ToList();
                //var query = from cg in db.C_COMMITTEE_GROUP
                //            join c in db.C_COMMITTEE on cg.COMMITTEE_ID equals c.UUID
                //            join sv in db.C_S_SYSTEM_VALUE on c.COMMITTEE_TYPE_ID equals sv.UUID
                //            join cp in db.C_COMMITTEE_PANEL on c.COMMITTEE_PANEL_ID equals cp.UUID
                //            join svp in db.C_S_SYSTEM_VALUE on cp.PANEL_TYPE_ID equals svp.UUID
                //            where cg.YEAR == searchYear
                //            && (string.IsNullOrWhiteSpace(registrationType) ? 1 == 1 : sv.REGISTRATION_TYPE == registrationType)
                //            orderby svp.CODE, cg.NAME
                //            select new { cg, c, sv, cp, svp };

            }
        }
        static public List<SelectListItem> GetLeaveFormNature(string ApplyValue)
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>{

                    new SelectListItem { Text = "On Leave", Value = "L"}
                    , new SelectListItem { Text = "Replcaement", Value = "R"}
                };

            foreach (var item in selectListItems)
            {
                if (string.IsNullOrWhiteSpace(ApplyValue))
                { }
                else if (ApplyValue.Contains(item.Value))
                {
                    item.Selected = true;
                }
            }
            return selectListItems;
        }
        static public List<SelectListItem> GetSVListByRegType(string RegType)
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem
            {
                Text = "- All -",
                Value = "",
                Selected = true
            });
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                selectListItems.AddRange(
                        GetSVListByRegTypeNType(RegType,
                                RegistrationConstant.SYSTEM_TYPE_APPLICATION_FORM)
                                .Select(o => new SelectListItem() { Text = o.CODE, Value = o.UUID }));

            }
            return selectListItems;
        }
        static public List<SelectListItem> GetSVClassListByRegType(string RegType)
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem
            {
                Text = "- Select Minor Work Class -",
                Value = "",
                Selected = true
            });
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                selectListItems.AddRange(
                        GetSVListByType(RegType)
                                .Select(o => new SelectListItem() { Text = o.CODE, Value = o.CODE }));

            }

            return selectListItems;
        }
        static public List<SelectListItem> GetSVConsentListByRegType(string RegType)
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();

            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                selectListItems.AddRange(
                        GetSVListByType(RegType)
                                .Select(o => new SelectListItem() { Text = o.CODE, Value = o.CODE }));

            }
            return selectListItems;
        }
        //regComReport
        static public List<SelectListItem> GetStatusListItem()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem
            {
                Text = "- All -",
                Value = "",
                Selected = true
            });
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                selectListItems.AddRange((from sv in GetSVListByType(RegistrationConstant.SYSTEM_TYPE_APPLICANT_STATUS)
                                          select new SelectListItem()
                                          {
                                              Text = sv.ENGLISH_DESCRIPTION,
                                              Value = sv.UUID,
                                          }
                                            ).ToList());

            }
            return selectListItems;
        }
        static public List<SelectListItem> GetDivStatus(string Status)
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                selectListItems.AddRange((from sv in GetSVListByType(RegistrationConstant.SYSTEM_TYPE_APPLICANT_STATUS)
                                          select new SelectListItem()
                                          {
                                              Text = sv.ENGLISH_DESCRIPTION,
                                              Value = sv.UUID,
                                          }
                            ).ToList());
            }
            foreach (var item in selectListItems)
            {
                if (string.IsNullOrWhiteSpace(Status))
                { }
                else if (Status.Contains(item.Value))
                {
                    item.Selected = true;
                }
            }
            return selectListItems;
        }
        static public List<SelectListItem> GetQpAsATList()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem()
            {
                Text = "- Now -"
    ,
                Value = ""
            });
            using (EntitiesRegistration db = new EntitiesRegistration())
            {


                selectListItems.AddRange((from sv in GetQpAsATListSql()
                                          select new SelectListItem()
                                          {
                                              Text = sv,
                                              Value = sv,
                                          }
                            ).ToList());
            }
            return selectListItems;
        }
        static public List<string> GetQpAsATListSql()
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                var query = db.C_QP_COUNT.Select(x => x.COUNT_DATE).Distinct();
                //var query = (from qp in db.C_QP_COUNT
                //             orderby qp.COUNT_DATE descending
                //             select qp).Distinct();
                List<string> s = new List<string>();
                query = query.OrderByDescending(x => x);
                foreach (var item in query)
                {
                    if(!s.Contains(item.ToString("yyyy/MM")))
                        s.Add(item.ToString("yyyy/MM"));
                    
                }
               // s.OrderByDescending(x => x).Distinct();
                return s;
            }
        }

        static public List<SelectListItem> GetSysRank()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem()
            {
                Text = "- Select -"
                ,
                Value = ""
            });
            using (EntitiesAuth db = new EntitiesAuth())
            {
                selectListItems.AddRange((from r in db.SYS_RANK
                                          select new SelectListItem()
                                          {
                                              Text = r.CODE
                                              ,
                                              Value = r.UUID
                                          }).ToList());
                return selectListItems;
            }
        }
        static public List<SelectListItem> GetHandlingOfficerList()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem()
            {
                Text = "- Select -"
                ,
                Value = ""
            });
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                var currUser = db.B_S_SCU_TEAM.ToList();

                foreach (var item in currUser)
                {
                    string username = "";
                    using (EntitiesAuth auth = new EntitiesAuth())
                    {
                        username = auth.SYS_POST.Find(item.SYS_POST_ID).BD_PORTAL_LOGIN;
                    }
                    selectListItems.Add(new SelectListItem { Text = username, Value = item.SYS_POST_ID });
                }
            }
            //using (EntitiesSignboard db = new EntitiesSignboard())
            //{
            //    selectListItems.AddRange((from sua in db.B_S_USER_ACCOUNT
            //                              join sst in db.B_S_SCU_TEAM on sua.UUID equals sst.USER_ACCOUNT_ID
            //                              where sst.POSITION == SignboardConstant.LOOK_UP_NAME_RANK_PO
            //                              orderby sua.USERNAME
            //                              select new SelectListItem()
            //                              {
            //                                  Text = sua.USERNAME
            //                                  ,
            //                                  Value = sua.UUID
            //                              }).ToList());
            //    selectListItems.AddRange((from sua in db.B_S_USER_ACCOUNT
            //                              join sst in db.B_S_SCU_TEAM on sua.UUID equals sst.USER_ACCOUNT_ID
            //                              where sst.POSITION == SignboardConstant.LOOK_UP_NAME_RANK_TO
            //                              orderby sua.USERNAME
            //                              select new SelectListItem()
            //                              {
            //                                  Text = sua.USERNAME
            //                                  ,
            //                                  Value = sua.UUID
            //                              }).ToList());
            //    selectListItems.AddRange((from sua in db.B_S_USER_ACCOUNT
            //                              join sst in db.B_S_SCU_TEAM on sua.UUID equals sst.USER_ACCOUNT_ID
            //                              where sst.POSITION == SignboardConstant.LOOK_UP_NAME_RANK_SPO
            //                              orderby sua.USERNAME
            //                              select new SelectListItem()
            //                              {
            //                                  Text = sua.USERNAME
            //                                  ,
            //                                  Value = sua.UUID
            //                              }).ToList());
            return selectListItems;
            //}   
        }
        static public List<SelectListItem> GetSearchFormCodeList()
        {
            return new List<SelectListItem>{
                    new SelectListItem { Text = "- Select -", Value = ""}
                    , new SelectListItem { Text = "SC01", Value = "SC01"}
                    , new SelectListItem { Text = "SC02", Value = "SC02"}
                    , new SelectListItem { Text = "SC03", Value = "SC03"}
            };
        }
        static public List<SelectListItem> GetSearchStatusList()
        {
            return new List<SelectListItem>{
                    new SelectListItem { Text = "- Select -", Value = ""}
                    , new SelectListItem { Text = SignboardConstant.RECOMMEND_ACK_STR, Value = SignboardConstant.RECOMMEND_ACK_STR}
                    , new SelectListItem { Text = SignboardConstant.RECOMMEND_REF_STR, Value = SignboardConstant.RECOMMEND_REF_STR}
                    , new SelectListItem { Text = SignboardConstant.RECOMMEND_COND_STR, Value = SignboardConstant.RECOMMEND_COND_STR}
            };
        }
        static public List<SelectListItem> GetEndorsedByList()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem()
            {
                Text = "- Select -"
                ,
                Value = ""
            });
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                var currUser = db.B_S_SCU_TEAM.ToList();

                foreach (var item in currUser)
                {
                    string username = "";
                    using (EntitiesAuth auth = new EntitiesAuth())
                    {
                        username = auth.SYS_POST.Find(item.SYS_POST_ID).BD_PORTAL_LOGIN;
                    }
                    selectListItems.Add(new SelectListItem { Text = username, Value = item.SYS_POST_ID });
                }
            }
            //using (EntitiesSignboard db = new EntitiesSignboard())
            //{
            //    selectListItems.AddRange((from sua in db.B_S_USER_ACCOUNT
            //                              join sst in db.B_S_SCU_TEAM on sua.UUID equals sst.USER_ACCOUNT_ID
            //                              where sst.POSITION == SignboardConstant.LOOK_UP_NAME_RANK_PO
            //                              orderby sua.USERNAME
            //                              select new SelectListItem()
            //                              {
            //                                  Text = sua.USERNAME
            //                                  ,
            //                                  Value = sua.UUID
            //                              }).ToList());
            //    selectListItems.AddRange((from sua in db.B_S_USER_ACCOUNT
            //                              join sst in db.B_S_SCU_TEAM on sua.UUID equals sst.USER_ACCOUNT_ID
            //                              where sst.POSITION == SignboardConstant.LOOK_UP_NAME_RANK_TO
            //                              orderby sua.USERNAME
            //                              select new SelectListItem()
            //                              {
            //                                  Text = sua.USERNAME
            //                                  ,
            //                                  Value = sua.UUID
            //                              }).ToList());
            //    selectListItems.AddRange((from sua in db.B_S_USER_ACCOUNT
            //                              join sst in db.B_S_SCU_TEAM on sua.UUID equals sst.USER_ACCOUNT_ID
            //                              where sst.POSITION == SignboardConstant.LOOK_UP_NAME_RANK_SPO
            //                              orderby sua.USERNAME
            //                              select new SelectListItem()
            //                              {
            //                                  Text = sua.USERNAME
            //                                  ,
            //                                  Value = sua.UUID
            //                              }).ToList());
            return selectListItems;
            //  }   
        }
        static public List<SelectListItem> GetDistrictList()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem
            {
                Text = "- All -",
                Value = "ALL",
                Selected = true
            });

            //using (EntitiesRegistration db = new EntitiesRegistration())
            //{
            //    var AppSQL = from sv in GetSVListByType(RegistrationConstant.SYSTEM_TYPE_APPLICATION_FORM)
            //                 where sv.REGISTRATION_TYPE == RegType
            //                 select sv;

            //    foreach (var item in AppSQL)
            //    {
            //        if (item.REGISTRATION_TYPE == RegType)
            //        {

            //        }
            //        string temp = item.ENGLISH_DESCRIPTION;

            //        var FormList = new SelectListItem();
            //        FormList.Text = item.CODE;
            //        FormList.Value = item.UUID;
            //        selectListItems.Add(FormList);
            //    }
            //    selectListItems.AddRange((
            //                            from sv in GetSVListByType(RegistrationConstant.SYSTEM_TYPE_APPLICATION_FORM)
            //                            where sv.REGISTRATION_TYPE == RegType
            //                            select new SelectListItem()
            //                            {
            //                                Text = sv.CODE,
            //                                Value = sv.UUID,
            //                            }
            //                 ).ToList());

            //}
            return selectListItems;
        }
        static public List<SelectListItem> GetSeniorOfficerList()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem
            {
                Text = "- All -",
                Value = "ALL",
                Selected = true
            });

            //using (EntitiesRegistration db = new EntitiesRegistration())
            //{
            //    var AppSQL = from sv in GetSVListByType(RegistrationConstant.SYSTEM_TYPE_APPLICATION_FORM)
            //                 where sv.REGISTRATION_TYPE == RegType
            //                 select sv;

            //    foreach (var item in AppSQL)
            //    {
            //        if (item.REGISTRATION_TYPE == RegType)
            //        {

            //        }
            //        string temp = item.ENGLISH_DESCRIPTION;

            //        var FormList = new SelectListItem();
            //        FormList.Text = item.CODE;
            //        FormList.Value = item.UUID;
            //        selectListItems.Add(FormList);
            //    }
            //    selectListItems.AddRange((
            //                            from sv in GetSVListByType(RegistrationConstant.SYSTEM_TYPE_APPLICATION_FORM)
            //                            where sv.REGISTRATION_TYPE == RegType
            //                            select new SelectListItem()
            //                            {
            //                                Text = sv.CODE,
            //                                Value = sv.UUID,
            //                            }
            //                 ).ToList());

            //}
            return selectListItems;
        }
        static public List<SelectListItem> GetOfficerList()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem
            {
                Text = "- All -",
                Value = "ALL",
                Selected = true
            });

            //using (EntitiesRegistration db = new EntitiesRegistration())
            //{
            //    var AppSQL = from sv in GetSVListByType(RegistrationConstant.SYSTEM_TYPE_APPLICATION_FORM)
            //                 where sv.REGISTRATION_TYPE == RegType
            //                 select sv;

            //    foreach (var item in AppSQL)
            //    {
            //        if (item.REGISTRATION_TYPE == RegType)
            //        {

            //        }
            //        string temp = item.ENGLISH_DESCRIPTION;

            //        var FormList = new SelectListItem();
            //        FormList.Text = item.CODE;
            //        FormList.Value = item.UUID;
            //        selectListItems.Add(FormList);
            //    }
            //    selectListItems.AddRange((
            //                            from sv in GetSVListByType(RegistrationConstant.SYSTEM_TYPE_APPLICATION_FORM)
            //                            where sv.REGISTRATION_TYPE == RegType
            //                            select new SelectListItem()
            //                            {
            //                                Text = sv.CODE,
            //                                Value = sv.UUID,
            //                            }
            //                 ).ToList());

            //}
            return selectListItems;
        }
        static public List<SelectListItem> GetLSOList()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem
            {
                Text = "- All -",
                Value = "ALL",
                Selected = true
            });

            //using (EntitiesRegistration db = new EntitiesRegistration())
            //{
            //    var AppSQL = from sv in GetSVListByType(RegistrationConstant.SYSTEM_TYPE_APPLICATION_FORM)
            //                 where sv.REGISTRATION_TYPE == RegType
            //                 select sv;

            //    foreach (var item in AppSQL)
            //    {
            //        if (item.REGISTRATION_TYPE == RegType)
            //        {

            //        }
            //        string temp = item.ENGLISH_DESCRIPTION;

            //        var FormList = new SelectListItem();
            //        FormList.Text = item.CODE;
            //        FormList.Value = item.UUID;
            //        selectListItems.Add(FormList);
            //    }
            //    selectListItems.AddRange((
            //                            from sv in GetSVListByType(RegistrationConstant.SYSTEM_TYPE_APPLICATION_FORM)
            //                            where sv.REGISTRATION_TYPE == RegType
            //                            select new SelectListItem()
            //                            {
            //                                Text = sv.CODE,
            //                                Value = sv.UUID,
            //                            }
            //                 ).ToList());

            //}
            return selectListItems;
        }
        static public List<SelectListItem> GetLetter()
        {
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                List<SelectListItem> selectListItems = new List<SelectListItem>();
                selectListItems.AddRange((
                                        from lt in db.B_S_LETTER_TEMPLATE
                                        orderby lt.LETTER_NAME
                                        select new SelectListItem()
                                        {
                                            Text = lt.LETTER_NAME,
                                            Value = lt.UUID,
                                        }
                             ).ToList());
                return selectListItems;

            }


        }
        static public List<SelectListItem> GetFolderType()
        {
            return new List<SelectListItem> {
                new SelectListItem{ Text = SignboardConstant.SCANNED_DOC_FOLDER_TYPE_PUBLIC,Value =SignboardConstant.SCANNED_DOC_FOLDER_TYPE_PUBLIC},
                 new SelectListItem{ Text = SignboardConstant.SCANNED_DOC_FOLDER_TYPE_PRIVATE_SCU,Value =SignboardConstant.SCANNED_DOC_FOLDER_TYPE_PRIVATE_SCU},
                  new SelectListItem{ Text = SignboardConstant.SCANNED_DOC_FOLDER_TYPE_PRIVATE_BD,Value =SignboardConstant.SCANNED_DOC_FOLDER_TYPE_PRIVATE_BD}
            };

        }
        static public List<SelectListItem> GetSMMStatusList()
        {
            return new List<SelectListItem>{
                new SelectListItem { Text = "-All-", Value = ""}
                    ,new SelectListItem { Text = "Delivered", Value = SignboardConstant.SV_SUBMISSION_STATUS_SCU_RECEIVED}
                    , new SelectListItem { Text = "Draft", Value = SignboardConstant.SV_SUBMISSION_STATUS_SCU_DATA_ENTRY}
                };
        }
        static public List<SelectListItem> GetTYPEList()
        {
            return new List<SelectListItem>{
                new SelectListItem { Text = SignboardConstant.SIGNBOARD_TYPE_SHOPFRONT, Value = SignboardConstant.SIGNBOARD_TYPE_SHOPFRONT}
                    ,new SelectListItem { Text = SignboardConstant.SIGNBOARD_TYPE_PROJECTING, Value = SignboardConstant.SIGNBOARD_TYPE_PROJECTING}
                    , new SelectListItem { Text = SignboardConstant.SIGNBOARD_TYPE_WALL, Value = SignboardConstant.SIGNBOARD_TYPE_WALL}
                    , new SelectListItem { Text = SignboardConstant.SIGNBOARD_TYPE_CANOPY, Value = SignboardConstant.SIGNBOARD_TYPE_CANOPY}
                    , new SelectListItem { Text = SignboardConstant.SIGNBOARD_TYPE_BALCONY, Value = SignboardConstant.SIGNBOARD_TYPE_BALCONY}
                    , new SelectListItem { Text = SignboardConstant.SIGNBOARD_TYPE_ONGRADE, Value = SignboardConstant.SIGNBOARD_TYPE_ONGRADE}
                    , new SelectListItem { Text = SignboardConstant.SIGNBOARD_TYPE_ROOF, Value = SignboardConstant.SIGNBOARD_TYPE_ROOF
                    }
                };
        }
        static public List<SelectListItem> GetLEDList()
        {
            return new List<SelectListItem>{
                new SelectListItem { Text = "Y", Value = "Y"}
                    ,new SelectListItem { Text = "N", Value = "N"}
                    , new SelectListItem { Text = "N/A", Value = "N/A"}
                };
        }
        static public List<B_S_SYSTEM_VALUE> GetSMMSVListByRegTypeNType(string type)
        {
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                return db.B_S_SYSTEM_VALUE.Where(o => o.B_S_SYSTEM_TYPE.TYPE == type && o.IS_ACTIVE == "Y").OrderBy(o => o.ORDERING).ToList();
            }
        }
        static public B_S_SYSTEM_VALUE GetSMMSVByUUID(string uuid)
        {
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                return db.B_S_SYSTEM_VALUE.Where(x => x.UUID == uuid).FirstOrDefault();
            }
        }
        static public List<B_S_SYSTEM_VALUE> GetSMMSVListByRegTypeNTypeNPid(string type, string Pid)
        {
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                return db.B_S_SYSTEM_VALUE.Where(o => o.B_S_SYSTEM_TYPE.TYPE == type && o.IS_ACTIVE == "Y" && o.PARENT_ID == Pid).OrderBy(o => o.ORDERING).ToList();
            }
        }
        //SMM
        static public List<SelectListItem> GetPawSameAsList()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                selectListItems.AddRange(
                    GetSMMSVListByRegTypeNType("PawSameAs").Select
                    (o => new SelectListItem()
                    { Text = o.CODE, Value = o.CODE }));
                return selectListItems;
            }
        }
        static public List<SelectListItem> GetBcisDistrictList()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                var Description = db.B_S_SYSTEM_VALUE.Where(x => x.DESCRIPTION == "Hong Kong").FirstOrDefault();

                var BCISItem = GetSMMSVListByRegTypeNTypeNPid("BcisDistrict", Description.UUID);
                string displayValue = "";
                string Code = "";
                foreach (var item in BCISItem)
                {
                    displayValue = item.CODE + "-" + item.DESCRIPTION;
                    Code = item.CODE;
                    var BCISItemList = new SelectListItem();
                    BCISItemList.Text = displayValue;
                    BCISItemList.Value = Code;
                    selectListItems.Add(BCISItemList);
                }
            }
            return selectListItems;
        }


        //SMM
        static public List<SelectListItem> RetrieveNewHandler()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();

            using (EntitiesAuth db = new EntitiesAuth())
            {
                selectListItems.AddRange((from sv in db.SYS_POST
                                          join u in db.SYS_UNIT on sv.SYS_UNIT_ID equals u.UUID
                                          where u.CODE == "SU"
                                          orderby sv.CODE
                                          select new SelectListItem()
                                          {
                                              Text = sv.CODE,
                                              Value = sv.UUID,
                                          }
                               ).ToList());


            }
            return selectListItems;


        }
        // PEM
        static public List<SelectListItem> RetrieveNewHandlerPEM()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();

            using (EntitiesAuth db = new EntitiesAuth())
            {
                string query = " \r\n SELECT T1.*"
                    + " \r\n FROM SYS_POST T1 "
                    + " \r\n\t INNER JOIN P_S_SCU_TEAM T2 ON T1.UUID = T2.CHILD_SYS_POST_ID "
                    + " \r\n\t INNER JOIN P_S_SCU_TEAM T3 ON T2.SYS_POST_ID = T3.CHILD_SYS_POST_ID "
                    + " \r\n\t INNER JOIN SYS_RANK T4 ON T1.SYS_RANK_ID = T4.UUID "
                    + " \r\n WHERE T1.IS_ACTIVE = 'Y' AND T4.RANK_GROUP IN('TO', 'PO', 'SPO') ";
                List<SYS_POST> scus = db.SYS_POST.SqlQuery(query).ToList();
                foreach(var scu in scus)
                {
                    selectListItems.Add(new SelectListItem { Text = scu.CODE, Value = scu.UUID });
                }
            }
            return selectListItems;


        }

        // Begin add by Chester 2019-07-26
        static public List<SelectListItem> GetActiveSToDetails()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>() {
                new SelectListItem()
                {
                    Text=""
                    ,Value=""
                }
            };
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                var list = (from d in db.P_S_TO_DETAILS
                            where d.IS_ACTIVE == "Y"
                            select d).ToList();

                foreach (var item in list)
                {
                    selectListItems.Add(new SelectListItem()
                    {
                        Text = (item.PO_POST + (string.IsNullOrEmpty(item.PO_POST_ENG) ? "" : "-" + item.PO_POST_ENG) + (string.IsNullOrEmpty(item.PO_POST_CHI) ? "" : "-" + item.PO_POST_CHI)),
                        Value = item.UUID
                    });
                }
            }
            return selectListItems;
        }

        static public List<SelectListItem> GetGeneralSubmissionStatusList()
        {
            return new List<SelectListItem>{
                      new SelectListItem { Text = "- Select -", Value = ""}  //IP: 7-13
                    , new SelectListItem { Text = "Scanned", Value = ProcessingConstant.DSN_SCANNED}
                    , new SelectListItem { Text = "Confirmed", Value = ProcessingConstant.DSN_CONFIRMED}
                    , new SelectListItem { Text = "Draft", Value = ProcessingConstant.DSN_GENERAL_ENTRY + "," +
                    ProcessingConstant.DSN_GENERAL_ENTRY_WILL_SCAN}
                };

        }

        static public List<SelectListItem> GetDSNStatus()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>() {
                new SelectListItem()
                {
                    Text="- Select -"
                    ,Value=""
                }
            };
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                P_S_SYSTEM_TYPE systype = db.P_S_SYSTEM_TYPE.Where(m => m.TYPE == "DSN_STATUS").FirstOrDefault();
                selectListItems.AddRange((from sysvalue in db.P_S_SYSTEM_VALUE
                                          where sysvalue.SYSTEM_TYPE_ID == systype.UUID
                                          select new SelectListItem()
                                          {
                                              Text = sysvalue.DESCRIPTION
                                              ,
                                              Value = sysvalue.UUID
                                          }).Distinct().OrderBy(m => m.Text).ToList());
            }
            return selectListItems;
        }


        static public List<SelectListItem> GetValidMWFormNos()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem
            {
                Text = "- Select -",
                Value = "",
                Selected = true
            });
            selectListItems.AddRange((from m in ProcessingConstant.validMWFormNos
                                      select new SelectListItem()
                                      {
                                          Text = m
                                          ,
                                          Value = m
                                      }).ToList());
            return selectListItems;

        }


        static public List<SelectListItem> GetVERTStatus()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem
            {
                Text = "- Select -",
                Value = "",
                Selected = true
            });
            selectListItems.Add(new SelectListItem
            {
                Text = ProcessingConstant.STATUS_OPEN
                ,
                Value = ProcessingConstant.MW_VERT_STATUS_OPEN
            });
            selectListItems.Add(new SelectListItem()
            {
                Text = ProcessingConstant.STATUS_COMPLETED
                ,
                Value = ProcessingConstant.MW_ACKN_STATUS_OPEN
            });
            return selectListItems;

        }


        static public List<SelectListItem> GetVERTSubmissionType()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem
            {
                Text = "- Select -",
                Value = "",
                Selected = true
            });
            selectListItems.Add(new SelectListItem
            {
                Text = "Commencement"
                ,
                Value = ProcessingConstant.COMMENCEMENT
            });
            selectListItems.Add(new SelectListItem
            {
                Text = "Completion"
                ,
                Value = ProcessingConstant.COMPLETION
            });
            selectListItems.Add(new SelectListItem
            {
                Text = "Validation Scheme"
                ,
                Value = ProcessingConstant.VALIDATION_SCHEME
            });
            return selectListItems;

        }

        static public List<SelectListItem> GetACKNStatus()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem
            {
                Text = "- Select -",
                Value = "",
                Selected = true
            });
            selectListItems.Add(new SelectListItem
            {
                Text = ProcessingConstant.STATUS_OPEN
                ,
                Value = ProcessingConstant.STATUS_OPEN
            });
            selectListItems.Add(new SelectListItem()
            {
                Text = ProcessingConstant.STATUS_COMPLETED
                ,
                Value = ProcessingConstant.STATUS_COMPLETED
            });
            return selectListItems;

        }


        static public List<SelectListItem> GetACKNClasses()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem
            {
                Text = "- Select -",
                Value = "",
                Selected = true
            });
            selectListItems.Add(new SelectListItem
            {
                Text = ProcessingConstant.DISPLAY_CLASS_I
               ,
                Value = ProcessingConstant.DB_CLASS_I
            });
            selectListItems.Add(new SelectListItem
            {
                Text = ProcessingConstant.DISPLAY_CLASS_II
               ,
                Value = ProcessingConstant.DB_CLASS_II
            });
            selectListItems.Add(new SelectListItem
            {
                Text = ProcessingConstant.DISPLAY_CLASS_III
               ,
                Value = ProcessingConstant.DB_CLASS_III
            });
            return selectListItems;

        }

        static public List<SelectListItem> GetACKNSubmissionType()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem
            {
                Text = "- Select -",
                Value = "",
                Selected = true
            });
            selectListItems.Add(new SelectListItem
            {
                Text = "Commencement"
                ,
                Value = ProcessingConstant.ACKNOWLEDGEMENT_SUBMISSION_TYPE_COMMENCEMENT
            });
            selectListItems.Add(new SelectListItem
            {
                Text = "Completion"
                ,
                Value = ProcessingConstant.ACKNOWLEDGEMENT_SUBMISSION_TYPE_COMPLETION
            });
            selectListItems.Add(new SelectListItem
            {
                Text = "Validation Scheme"
                ,
                Value = ProcessingConstant.ACKNOWLEDGEMENT_SUBMISSION_TYPE_VALIDATION_SCHEME
            });
            return selectListItems;

        }


        static public List<SelectListItem> GetTSKGSSubmissionType()
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
                Text = ProcessingConstant.ENQ
                ,
                Value = ProcessingConstant.SUBMIT_TYPE_ENQ
            });
            selectListItems.Add(new SelectListItem
            {
                Text = ProcessingConstant.COM
                ,
                Value = ProcessingConstant.SUBMIT_TYPE_COM
            });
            return selectListItems;
        }


        static public List<SelectListItem> GetTSKGSStatuses()
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
                Text = ProcessingConstant.ENQUIRY_STATUS_IN_PROGRESS_DISPALY
                ,
                Value = ProcessingConstant.ENQUIRY_STATUS_IN_PROGRESS_DISPALY
            });
            selectListItems.Add(new SelectListItem
            {
                Text = ProcessingConstant.ENQUIRY_STATUS_CLOSED
                ,
                Value = ProcessingConstant.ENQUIRY_STATUS_CLOSED
            });

            return selectListItems;
        }


        static public List<SelectListItem> GetTSKGSSource()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem
            {
                Text = "- Select -",
                Value = "",
                Selected = true
            });
            selectListItems.Add(new SelectListItem
            {
                Text = ProcessingConstant.SOURCE_GENERAL_PUBLIC
                ,
                Value = ProcessingConstant.SOURCE_GENERAL_PUBLIC
            });
            selectListItems.Add(new SelectListItem
            {
                Text = ProcessingConstant.SOURCE_ICC
                ,
                Value = ProcessingConstant.SOURCE_ICC
            });
            selectListItems.Add(new SelectListItem
            {
                Text = ProcessingConstant.SOURCE_INTERNAL_REFERRAL
                ,
                Value = ProcessingConstant.SOURCE_INTERNAL_REFERRAL
            });
            selectListItems.Add(new SelectListItem
            {
                Text = ProcessingConstant.SOURCE_OTHERS
                ,
                Value = ProcessingConstant.SOURCE_OTHERS
            });

            return selectListItems;
        }


        static public List<SelectListItem> GetTSKProgress()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem
            {
                Text = "- Show All -",
                Value = "",
                Selected = true
            });
            selectListItems.Add(new SelectListItem
            {
                Text = ProcessingConstant.ENQUIRY_PROGRESS_SHOW_ALL
                ,
                Value = ProcessingConstant.ENQUIRY_PROGRESS_SHOW_ALL
            });
            selectListItems.Add(new SelectListItem
            {
                Text = ProcessingConstant.ENQUIRY_PROGRESS_SHOW_OVERDUE
                ,
                Value = ProcessingConstant.ENQUIRY_PROGRESS_SHOW_OVERDUE
            });
            selectListItems.Add(new SelectListItem
            {
                Text = ProcessingConstant.ENQUIRY_PROGRESS_SHOW_ALERT
                ,
                Value = ProcessingConstant.ENQUIRY_PROGRESS_SHOW_ALERT
            });
            return selectListItems;
        }

        // End add by Chester 2019-07-26

        static public List<SelectListItem> GetAuditStatus()
        {
            return new List<SelectListItem>{
                new SelectListItem { Text = "Assigned", Value = "Assigned"}
                    ,new SelectListItem { Text = "Not yet assign", Value = "Not yet assign"}
                    , new SelectListItem { Text = "Completed (In-order or final action taken)", Value = "Completed (In-order or final action taken)"}
                };
        }

        static public List<SelectListItem> GetSelectedLetterTypeList()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                selectListItems.AddRange(
                    GetSMMSVListByRegTypeNType("LetterType").Select
                    (o => new SelectListItem()
                    { Text = o.DESCRIPTION, Value = o.CODE }));
                return selectListItems;
            }
        }
        static public List<string> GetSubUser(string uuid)
        {
            List<string> list = new List<string>();
            //list.Add(uuid);
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                var q = db.B_S_SCU_TEAM.Where(x => x.SYS_POST_ID == uuid);
               
                foreach (var item in q)
                {
                    list.Add(item.CHILD_SYS_POST_ID);
                    var childQ = db.B_S_SCU_TEAM.Where(x => x.SYS_POST_ID == item.CHILD_SYS_POST_ID).ToList();
                    int i = 0;
                    while (i != childQ.Count())
                    {
                        if (!list.Contains(childQ[i].CHILD_SYS_POST_ID))
                        {
                            list.Add(childQ[i].CHILD_SYS_POST_ID);

                            foreach (var x in GetSubUser(childQ[i].CHILD_SYS_POST_ID))
                            {
                                if (!list.Contains(x))
                                    list.Add(x);

                            }

                        }
                        i++;
                        //var CchildQ = db.B_S_SCU_TEAM.Where(x => x.SYS_POST_ID == item.CHILD_SYS_POST_ID).ToList();
                        //while (CchildQ.Count() > 0 || i != CchildQ.Count())
                        //{
                        //    if(!list.Contains())
                        //    list.Add(childQ[i].SYS_POST_ID);
                        //}

                    }
                }


            }

            return list;
        }

        static public List<SelectListItem> GetScuTeamChildList(string uuid)
        {

            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem
            {
                Text = "-Please Select-",
                Value = "",
                Selected = true
            });
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                var currUser = db.B_S_SCU_TEAM.Where(x => x.SYS_POST_ID == uuid);
                List<B_S_SCU_TEAM> childList = new List<B_S_SCU_TEAM>();
                List<string> list = new List<string>();
                if (currUser != null)
                {
                    // childList = db.B_S_SCU_TEAM.Where(x => x.LEFT > currUser.LEFT).Where(x => x.RIGHT < currUser.RIGHT).ToList();
                    list = GetSubUser(uuid).Distinct().ToList();
                    //childList = db.B_S_SCU_TEAM.Where(x => list.Contains(x.SYS_POST_ID)).Distinct().ToList();
                    



                }
                if (list != null && list.Count() > 0)
                {
                    foreach (var item in list)
                    {
                        if (item != null)
                        {
                            string sys_post_id = item;
                            string username = "";
                            using (EntitiesAuth auth = new EntitiesAuth())
                            {

                                username = auth.SYS_POST.Find(sys_post_id).BD_PORTAL_LOGIN;
                            }
                            if (selectListItems.Where(x => x.Text == username).Count() == 0)
                                selectListItems.Add(new SelectListItem { Text = username, Value = sys_post_id });

                        }
                    }
                }



                //var currUser = db.B_S_SCU_TEAM.Where(x => x.SYS_POST_ID == uuid).FirstOrDefault();
                //List<B_S_SCU_TEAM> childList = new List<B_S_SCU_TEAM>();
                //if (currUser != null)
                //{
                //   childList = db.B_S_SCU_TEAM.Where(x => x.LEFT > currUser.LEFT).Where(x => x.RIGHT < currUser.RIGHT).ToList();



                //}
                //if (childList != null && childList.Count() > 0)
                //{
                //    foreach (var item in childList)
                //    {
                //        string sys_post_id = item.SYS_POST_ID;
                //        string username = "";
                //        using (EntitiesAuth auth = new EntitiesAuth())
                //        {
                //            username = auth.SYS_POST.Find(sys_post_id).BD_PORTAL_LOGIN;
                //        }
                //        selectListItems.Add(new SelectListItem { Text = username, Value = sys_post_id });
                //    }
                //}
            }
            return selectListItems;
        }
        static public List<C_S_SYSTEM_VALUE> GetMWItemVersion(string code)
        {

            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                var query = (from sv in db.C_S_SYSTEM_VALUE
                             join st in db.C_S_SYSTEM_TYPE on sv.SYSTEM_TYPE_ID equals st.UUID
                             where st.TYPE == SignboardConstant.MWITEM_VERSION
                             && sv.CODE == code
                             select sv).ToList();
                return query;
            }
        }

        static public List<SelectListItem> GetRecList()
        {
            return new List<SelectListItem>{
                new SelectListItem { Text = "All", Value = ""}
                ,new SelectListItem { Text = "Withdraw", Value = "Withdraw"}
                    ,new SelectListItem { Text = "Expired", Value = "Expired"}
                    ,new SelectListItem { Text = "Accept", Value = "Accept"}
                    ,new SelectListItem { Text = "Refuse", Value = "Refuse"}
            };
        }
        static public List<SelectListItem> GetCatList()
        {
            return new List<SelectListItem>{
                new SelectListItem { Text = "Registered General Building Contractors", Value = "GBC"}
                    ,new SelectListItem { Text = "RC", Value = "RC" }
                    ,new SelectListItem { Text = "RVC", Value = "RVC" }
                    ,new SelectListItem { Text = "Specialist Contractor", Value = "SC" }
                    ,new SelectListItem { Text = "Registered Specialist Contractor (Demolition Works) ", Value = "SC(D)"}
                    ,new SelectListItem { Text = "Registered Specialist Contractor (Foundation works) ", Value = "SC(F)"}
                    ,new SelectListItem { Text = "Registered Specialist Contractor (Ground Investigation Field Works) ", Value = "SC(GI)"}
                    ,new SelectListItem { Text = "Registered Specialist Contractor (Site Formation Works) ", Value = "SC(SF)"}
                    ,new SelectListItem { Text = "Registered Specialist Contractor (Ventilation Works) ", Value = "SC(V)"}
            };
        }

        static public List<SelectListItem> getIpOderByList()
        {
            return new List<SelectListItem>{
                new SelectListItem { Text = "Name", Value = "SURNAME"}
                    ,new SelectListItem { Text = "Date of Expiry", Value = "EXPDSURNAME" }
                    ,new SelectListItem { Text = "File Reference", Value = "FILE_REF" }
            };
        }
        static public List<SelectListItem> getMWCList()
        {
            return new List<SelectListItem>{
                new SelectListItem { Text = "All", Value = ""}
                    ,new SelectListItem { Text = "Registered Minor Works Contractors (Company)", Value = "MWC" }
                    ,new SelectListItem { Text = "Registered Minor Works Contractors (Provisional)", Value = "MWC(P)" }
            };
        }

        static public List<SelectListItem> GetSubmissionNatureList()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();

            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                return new List<SelectListItem>{
                     new SelectListItem { Text = "Submission", Value = ApplicationConstant.SUBMISSION}
                    ,new SelectListItem { Text = "eSubmission", Value = ApplicationConstant.ESUBMISSION}
                    ,new SelectListItem { Text = "ICU", Value = ApplicationConstant.ICU}
                    ,new SelectListItem { Text = "Direct Return", Value = ApplicationConstant.DIRECT_RETURN}
                    ,new SelectListItem { Text = "Revised Case", Value = ApplicationConstant.REVISED_CASE}
                    ,new SelectListItem { Text = "Withdrawal", Value = ApplicationConstant.WITHDRAWAL}
                };
            }

        }

        static public List<SelectListItem> GetPEMOfficerUserListBy(string rank)
        {   string sql =
                " SELECT T1.*  FROM SYS_POST T1 " +
                " INNER JOIN SYS_RANK T4 ON T1.SYS_RANK_ID = T4.UUID " +
                " WHERE " +
                " T1.IS_ACTIVE = 'Y'     AND T4.RANK_GROUP = '" + rank + "'  AND " +
                " T1.UUID IN (SELECT S.CHILD_SYS_POST_ID FROM P_S_SCU_TEAM S " +
                " UNION ALL SELECT P.SYS_POST_ID FROM P_S_SCU_TEAM P) ORDER BY T1.CODE ";
            List<SelectListItem> result = new List<SelectListItem>();
            using (EntitiesAuth db = new EntitiesAuth())
            {

                var query = db.SYS_POST.SqlQuery(sql).ToList();

                for(int i = 0; i< query.Count; i++)
                {

                    result.Add(new SelectListItem()
                    {
                        Text = query[i].CODE,
                        Value = query[i].CODE
                    });

                }
                       
                
            }
            return result;
        }

        static public List<SelectListItem> RetrieveStatusCode()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem
            {
                Text = "- Select -",
                Value = "",
                Selected = true
            });

            selectListItems.Add(new SelectListItem
            {
                Text = "New",
                Value = "REGISTRY_RECEIVED",

            });
            selectListItems.Add(new SelectListItem
            {
                Text = "Rescan",
                Value = "RESCANNED",

            });
            selectListItems.Add(new SelectListItem
            {
                Text = "Incoming",
                Value = "MWU_RD_INCOMING_NEW",

            });


            //using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            //{
            //    //selectListItems.AddRange((from sv in db.P_S_SYSTEM_VALUE
            //    //                          where  sv. == RegType
            //    //                          orderby catcode.CODE
            //    //                          select new SelectListItem()
            //    //                          {
            //    //                              Text = catcode.CODE,
            //    //                              Value = catcode.UUID,
            //    //                          }
            //    //             ).ToList());

            //}
            return selectListItems;

        }


        static public List<W_S_SYSTEM_VALUE> GetWLOffenseType()
        {
           
            using (EntitiesWarningLetter db = new EntitiesWarningLetter())
            {
                var query = db.W_S_SYSTEM_VALUE.Where(x => x.W_S_SYSTEM_TYPE.TYPE == "Type_Of_Offense");

               
                return query.ToList();
            }

        }
        static public List<SelectListItem> GetSections()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem
            {
                Text = "- Select -",
                Value = "",
                Selected = true
            });
            using (EntitiesAuth dbr = new EntitiesAuth())
            {
               var query = dbr.SYS_SECTION.ToList();
                foreach (var item in query)
                {
                    selectListItems.Add(new SelectListItem
                    {
                        Text = item.DESCRIPTION,
                        Value = item.CODE,

                    });
                }
            }
            return selectListItems;
        }
    }
}
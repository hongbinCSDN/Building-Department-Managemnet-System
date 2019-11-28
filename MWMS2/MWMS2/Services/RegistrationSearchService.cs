
using MWMS2.Entity;
using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Utility;
using MWMS2.Areas.Registration.Models;
using MWMS2.Constant;
using System.Data.Entity;

namespace MWMS2.Services
{
    public class RegistrationSearchService
    {
        //public static string LoginRole
        //{
        //    get {
        //        string result ="";
        //        var role = SessionUtil.LoginPost.SYS_POST_ROLE.Select(x => x.SYS_ROLE_ID).ToList();
        //        foreach (var item in role)
        //        {
        //            result += item +","; 
        //        }
        //        return result.Substring(0,result.Length-1);
        //    }
            

        //}
        public string ExportAS(Fn06AS_DBSearchModel model)
        {
            model.Query = SearchAS_q(model);
            //model.QueryWhere = SearchAS_whereQ(model);
            model.Columns = new List<Dictionary<string, string>>()
                .Append(new Dictionary<string, string>{["columnName"]="UUID",["displayName"]="UUID"})
                .Append(new Dictionary<string, string>{["columnName"]= "ID", ["displayName"]= "hkid/passport" })
                .Append(new Dictionary<string, string>{["columnName"]= "ENGNAME", ["displayName"]= "name" })
                .Append(new Dictionary<string, string>{["columnName"]= "CHINAME", ["displayName"]= "chinese_name" })
                .Append(new Dictionary<string, string>{["columnName"]= "CONSENT", ["displayName"]= "consent" })
                .Append(new Dictionary<string, string>{["columnName"]= "CONTACTTELNO", ["displayName"]= "contact_tel_no" })
                .Append(new Dictionary<string, string>{["columnName"]= "STATUS", ["displayName"]= "status" })
                .Append(new Dictionary<string, string>{["columnName"]= "GBC_RDT", ["displayName"]= "GBC" })
                .Append(new Dictionary<string, string>{["columnName"]= "SCD_RDT", ["displayName"]= "SC(D)" })
                .Append(new Dictionary<string, string>{["columnName"] = "SCF_RDT", ["displayName"] = "SC(F)" })
                .Append(new Dictionary<string, string>{["columnName"] = "SCGI_RDT", ["displayName"] = "SC(GI)" })
                .Append(new Dictionary<string, string>{["columnName"] = "SCSF_RDT", ["displayName"] = "SC(SF)" })
                .Append(new Dictionary<string, string>{["columnName"] = "SCV_RDT", ["displayName"] = "SC(V)" })
                .Append(new Dictionary<string, string>{["columnName"] = "MWC1_RDT", ["displayName"] = "MWC - Class I,II,III" })
                .Append(new Dictionary<string, string>{["columnName"] = "MWC2_RDT", ["displayName"] = "MWC - Class II,III" })
                .Append(new Dictionary<string, string>{["columnName"] = "MWC3_RDT", ["displayName"] = "MWC - Class III" })
                .Append(new Dictionary<string, string>{["columnName"] = "RESADDRESSE1", ["displayName"] = "e_residential_addr1" })
                .Append(new Dictionary<string, string>{["columnName"] = "RESADDRESSE2", ["displayName"] = "e_residential_addr2" })
                .Append(new Dictionary<string, string>{["columnName"] = "RESADDRESSE3", ["displayName"] = "e_residential_addr3" })
                .Append(new Dictionary<string, string>{["columnName"] = "RESADDRESSE4", ["displayName"] = "e_residential_addr4" })
                .Append(new Dictionary<string, string>{["columnName"] = "RESADDRESSE5", ["displayName"] = "e_residential_addr5" })
                .Append(new Dictionary<string, string>{["columnName"] = "RESADDRESSC1", ["displayName"] = "c_residential_addr1" })
                .Append(new Dictionary<string, string>{["columnName"] = "RESADDRESSC2", ["displayName"] = "c_residential_addr2" })
                .Append(new Dictionary<string, string>{["columnName"] = "RESADDRESSC3", ["displayName"] = "c_residential_addr3" })
                .Append(new Dictionary<string, string>{["columnName"] = "RESADDRESSC4", ["displayName"] = "c_residential_addr4" })
                .Append(new Dictionary<string, string>{["columnName"] = "RESADDRESSC5", ["displayName"] = "c_residential_addr5" })
                .Append(new Dictionary<string, string>{["columnName"]= "OFFICETEL", ["displayName"]= "office_tel" })
                .Append(new Dictionary<string, string>{["columnName"]= "MOBILETEL", ["displayName"]= "mobile_tel" })
                .Append(new Dictionary<string, string>{["columnName"]= "RESTEL", ["displayName"]= "residential_tel" })
                .Append(new Dictionary<string, string>{["columnName"]= "EMAIL1", ["displayName"]= "email1" })
                .Append(new Dictionary<string, string>{["columnName"]= "EMAIL2", ["displayName"]= "email2" }).ToArray();

            return model.Export("ExportData");
        }
        private string SearchAS_q(Fn06AS_DBSearchModel model)
        {
            //string whereQ = "";
            //string whereQ2 = "";
            string outerWhereClause = " where 1=1 ";
            string innerWhereClause = " where 1=1 ";
            string appIdWhereClause = "";
            List<string> mwcClassList = new List<string>();
            List<string> selectedCatList = new List<string>();
            List<string> classList = new List<string>();
            List<string> consentList = new List<string>();
            if (model.CAT_GBC == true) { selectedCatList.Add(RegistrationConstant.CAT_GBC); }
            if (model.CAT_SC_D == true) { selectedCatList.Add(RegistrationConstant.CAT_SC_D); }
            if (model.CAT_SC_F == true) { selectedCatList.Add(RegistrationConstant.CAT_SC_F); }
            if (model.CAT_SC_GI == true) { selectedCatList.Add(RegistrationConstant.CAT_SC_GI); }
            if (model.CAT_SC_SF == true) { selectedCatList.Add(RegistrationConstant.CAT_SC_SF); }
            if (model.CAT_SC_V == true) { selectedCatList.Add(RegistrationConstant.CAT_ALL); }
            if (model.CAT_MWC_CLASS_I_II_III == true) { classList.Add(RegistrationConstant.MW_CLASS_I); }
            if (model.CAT_MWC_CLASS_II_III == true) { classList.Add(RegistrationConstant.MW_CLASS_II); }
            if (model.CAT_MWC_CLASS_III == true) { classList.Add(RegistrationConstant.MW_CLASS_III); }
            if (model.ConsetToPublish==true){consentList.Add(RegistrationConstant.CONSET_TO_PUBLISH);}
            if (model.RefusedToPublish == true){ consentList.Add(RegistrationConstant.REFUSED_TO_PUBLISH); }
            if (model.NotIndicated == true){ consentList.Add(RegistrationConstant.NOT_INDICATED); }

            if (consentList.Count() > 0)
            {
                if (model.NotIndicated==true)
                {
                    outerWhereClause += " and (as1.CONSENT in (:consentList) or as1.CONSENT is null)";
                    model.QueryParameters.Add("consentList", consentList);
                }
                else
                {
                    outerWhereClause += " and as1.CONSENT in (:consentList)";
                    model.QueryParameters.Add("consentList", consentList);
                }
            }

            if (string.IsNullOrWhiteSpace(model.Status))
            {
                outerWhereClause += "\r\n\t" + "AND T.status in ('Active AS','Inactive AS') ";
            }
            else
            {
                outerWhereClause += "\r\n\t" + "AND T.status = :status ";
                model.QueryParameters.Add("status", model.Status);
            }
            if (!string.IsNullOrWhiteSpace(model.HKID))
            {
                innerWhereClause += "\r\n\t" + "AND " + EncryptDecryptUtil.getDecryptSQL("app.hkid") + " LIKE :HKID";
                model.QueryParameters.Add("HKID", "%" + model.HKID.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.PassportNo))
            {
                innerWhereClause += "\r\n\t" + "AND " + EncryptDecryptUtil.getDecryptSQL("app.passport_no") + " LIKE :PassportNo";
                model.QueryParameters.Add("PassportNo", "%" + model.PassportNo.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.SurName))
            {
                innerWhereClause += "\r\n\t" + "AND app.SURNAME LIKE :SurnName";
                model.QueryParameters.Add("SurnName", "%" + model.SurName.Trim().ToUpper() + "%");

            }
            if (!string.IsNullOrWhiteSpace(model.GivenName))
            {
                innerWhereClause += "\r\n\t" + "AND upper(app.GIVEN_NAME_ON_ID) LIKE :GivenName";
                model.QueryParameters.Add("GivenName", "%" + model.GivenName.Trim().ToUpper() + "%");

            }
            if (!string.IsNullOrWhiteSpace(model.ChiName))
            {
                innerWhereClause += "\r\n\t" + "AND app.CHINESE_NAME LIKE :ChiName";
                model.QueryParameters.Add("ChiName", "%" + model.ChiName.Trim().ToUpper() + "%");
            }

            // format criteria for company -> get list of AS
            if (!string.IsNullOrWhiteSpace(model.FileRef) || (selectedCatList != null && selectedCatList.Count() > 0) ||
                    (mwcClassList != null && mwcClassList.Count() > 0)){

                appIdWhereClause += ""
                                  + "\r\n" + " and app.uuid in (select cai.APPLICANT_ID "
                                  + "\r\n" +  " from c_comp_applicant_info cai "
                                  + "\r\n" +  " inner join c_comp_application ca on ca.uuid = cai.master_id "
                                  + "\r\n" +  " inner join c_s_system_value svrole on svrole.uuid = cai.APPLICANT_ROLE_ID and svrole.CODE='AS' "
                                  + "\r\n" +  " inner join c_s_category_code scc on scc.uuid = ca.CATEGORY_ID "
                                  + "\r\n" +  " left join c_comp_applicant_mw_item cami on cami.COMPANY_APPLICANTS_ID = cai.uuid "
                                  + "\r\n" +  " left join c_s_system_value sv3 on sv3.uuid = cami.ITEM_CLASS_ID "
                                  + "\r\n" + " where 1=1 ";

                if (!string.IsNullOrWhiteSpace(model.FileRef))
                {
                    appIdWhereClause += "\r\n\t" + "AND UPPER(ca.FILE_REFERENCE_NO) LIKE :FileRef";
                    model.QueryParameters.Add("FileRef", "%" + model.FileRef.Trim().ToUpper() + "%");
                }
                if ((selectedCatList != null && selectedCatList.Count() > 0) ||
                (mwcClassList != null && mwcClassList.Count() > 0))
                {
                    string combinedCondition = " and (<CAT> or <CLASS>) ";
                    string selectedCatListCondition = "1=2";
                    string mwcClassListCondition = "1=2";
                    if (selectedCatList != null && selectedCatList.Count() > 0)
                    {
                        selectedCatListCondition = "scc.CODE in (:categoryList)";
                        model.QueryParameters.Add("categoryList", selectedCatList);
                    }
                    if (mwcClassList != null && mwcClassList.Count() > 0)
                    {
                        mwcClassListCondition = "(scc.CODE = '" + RegistrationConstant.CAT_MWC + "' and sv3.CODE in (:mwcClassList))";
                        model.QueryParameters.Add("mwcClassList", mwcClassList);
                    }
                    combinedCondition = combinedCondition.Replace("<CAT>", selectedCatListCondition)
                            .Replace("<CLASS>", mwcClassListCondition);
                    appIdWhereClause += combinedCondition;
                }
                // close blacket
                appIdWhereClause += ")";
            }

            return ""
                + "\r\n" + " with latestCai as (  "
                + "\r\n" + " select distinct app.uuid as APPUUID, "
                + "\r\n" + " FIRST_VALUE(cai.uuid) OVER (PARTITION BY app.uuid ORDER BY cai.modified_date desc NULLS LAST) AS caiUuid "
                + "\r\n" + " from c_applicant app "
                + "\r\n" + " inner join c_comp_applicant_info cai on cai.APPLICANT_ID = app.uuid) "
                + "\r\n" + " select T.uuid, "
                + "\r\n" + " case when " + EncryptDecryptUtil.getDecryptSQL("app.hkid") + " is not null then "
                + EncryptDecryptUtil.getDecryptSQL("app.hkid")
                + "when " + EncryptDecryptUtil.getDecryptSQL("app.passport_no") + "is not null then "
                + EncryptDecryptUtil.getDecryptSQL("app.passport_no") + "end as ID, "
                + "\r\n" + " concat(concat( app.SURNAME ,' '), app.GIVEN_NAME_ON_ID) ENGNAME, "
                + "\r\n" + " app.CHINESE_NAME as CHINAME, "
                //+ "\r\n" + " as1.CONSENT  as CONSENT, "
                + "\r\n" + " decode(as1.CONSENT,null,'Not Indicated',as1.CONSENT  )as CONSENT, "
                + "\r\n" + " as1.CONTACT_TEL_NO  as contactTelNo,  "
                + "\r\n" + " T.status,  "
                + "\r\n" + " T.gbc_rdt, T.scd_rdt, T.scf_rdt, T.scgi_rdt, T.scsf_rdt, T.scv_rdt, T.mwc1_rdt, T.mwc2_rdt, T.mwc3_rdt, "
                + "\r\n" + " cai.RES_ADDRESS_E1 as resAddressE1, "
                + "\r\n" + " cai.RES_ADDRESS_E2 as resAddressE2, "
                + "\r\n" + " cai.RES_ADDRESS_E3 as resAddressE3, "
                + "\r\n" + " cai.RES_ADDRESS_E4 as resAddressE4, "
                + "\r\n" + " cai.RES_ADDRESS_E5 as resAddressE5, "
                + "\r\n" + " cai.RES_ADDRESS_C1 as resAddressC1, "
                + "\r\n" + " cai.RES_ADDRESS_C2 as resAddressC2, "
                + "\r\n" + " cai.RES_ADDRESS_C2 as resAddressC3, "
                + "\r\n" + " cai.RES_ADDRESS_C2 as resAddressC4, "
                + "\r\n" + " cai.RES_ADDRESS_C2 as resAddressC5, "
                + "\r\n" + " cai.OFFICE_TEL as officeTel, "
                + "\r\n" + " cai.MOBILE_TEL as mobileTel, "
                + "\r\n" + " cai.RES_TEL as resTel, "
                + "\r\n" + " cai.EMAIL1 as email1, "
                + "\r\n" + " cai.EMAIL2 as email2 "
                + "\r\n" + " from ( "
                + "\r\n" + " select uuid, "
                + "\r\n" + " case when sum(activeCnt) > 0 then 'Active AS' else 'Inactive AS' end as status, "
                + "\r\n" + " to_char(max(gbc), 'dd/MM/yyyy') as gbc_rdt, "
                + "\r\n" + " to_char(max(scd), 'dd/MM/yyyy') as scd_rdt, "
                + "\r\n" + " to_char(max(scf), 'dd/MM/yyyy') as scf_rdt, "
                + "\r\n" + " to_char(max(scgi), 'dd/MM/yyyy') as scgi_rdt, "
                + "\r\n" + " to_char(max(scsf), 'dd/MM/yyyy') as scsf_rdt, "
                + "\r\n" + " to_char(max(scv), 'dd/MM/yyyy') as scv_rdt, "
                + "\r\n" + " to_char(max(mwc1), 'dd/MM/yyyy') as mwc1_rdt, "
                + "\r\n" + " to_char(max(mwc2), 'dd/MM/yyyy') as mwc2_rdt, "
                + "\r\n" + " to_char(max(mwc3), 'dd/MM/yyyy') as mwc3_rdt "
                + "\r\n" + " from ( "
                + "\r\n" + " select "
                + "\r\n" + " app.uuid as uuid, "
                + "\r\n" + " case when sv.english_description = 'Active' then 1 end as activeCnt, "
                + "\r\n" + " case when scc.code = 'GBC' then cai.REMOVAL_DATE end as gbc, "
                + "\r\n" + " case when scc.code = 'SC(D)' then cai.REMOVAL_DATE end as scd, "
                + "\r\n" + " case when scc.code = 'SC(F)' then cai.REMOVAL_DATE end as scf, "
                + "\r\n" + " case when scc.code = 'SC(GI)' then cai.REMOVAL_DATE end as scgi, "
                + "\r\n" + " case when scc.code = 'SC(SF)' then cai.REMOVAL_DATE end as scsf, "
                + "\r\n" + " case when scc.code = 'SC(V)' then cai.REMOVAL_DATE end as scv, "
                + "\r\n" + " case when scc.code = 'MWC' and sv3.code = 'Class 1' then cai.REMOVAL_DATE end as mwc1, "
                + "\r\n" + " case when scc.code = 'MWC' and sv3.code = 'Class 2' then cai.REMOVAL_DATE end as mwc2, "
                + "\r\n" + " case when scc.code = 'MWC' and sv3.code = 'Class 3' then cai.REMOVAL_DATE end as mwc3 "
                + "\r\n" + " from c_applicant app "
                + "\r\n" + " inner join c_comp_applicant_info cai on cai.APPLICANT_ID = app.uuid "
                + "\r\n" + " inner join c_comp_application ca on ca.uuid = cai.master_id "
                + "\r\n" + " inner join c_s_system_value sv on sv.uuid = cai.APPLICANT_STATUS_ID "
                + "\r\n" + " inner join c_s_system_value sv2 on sv2.uuid = cai.APPLICANT_ROLE_ID and sv2.CODE='AS' "
                + "\r\n" + " inner join c_s_category_code scc on scc.uuid = ca.CATEGORY_ID "
                + "\r\n" + " left join c_comp_applicant_mw_item cami on cami.COMPANY_APPLICANTS_ID = cai.uuid "
                + "\r\n" + " left join c_s_system_value sv3 on sv3.uuid = cami.ITEM_CLASS_ID "
                //+ "\r\n" + " where 1=1 "
                + "\r\n" + innerWhereClause
                + "\r\n" + appIdWhereClause
                + "\r\n" + " ) "
                + "\r\n" + " group by uuid "
                + "\r\n" + " ) T "
                + "\r\n" + " inner join c_applicant app on T.uuid = app.uuid "
                + "\r\n" + " left join c_as_consent as1 on as1.hkid = app.hkid or as1.passport_no = app.passport_no "
                + "\r\n" + " left join latestCai lc on app.uuid = lc.appUuid "
                + "\r\n" + " left join c_comp_applicant_info cai on cai.uuid = lc.caiUuid "
                //+ "\r\n" + " where 1=1 "
                + "\r\n" + outerWhereClause;
        }

        private string SearchQP_q(Fn01Search_QPSearchModel model)
        {
            bool all = !(model.Ap || model.Rse || model.Ri || model.Rgbc || model.TypeA || model.TypeG || model.Item36 || model.Item37);

            string whereQ = "";
            string whereQ2 = "";
            if (!string.IsNullOrWhiteSpace(model.FileRef))
            {//+ "\r\n" + "\t" + " , FileRef
                whereQ += "\r\n\t" + "AND T1.FILE_REFERENCE_NO LIKE :FileRef";
                model.QueryParameters.Add("FileRef", "%" + model.FileRef.Trim().ToUpper() + "%");
                whereQ2 += "\r\n\t" + "AND T1.FILE_REFERENCE_NO LIKE :FileRef2";
                model.QueryParameters.Add("FileRef2", "%" + model.FileRef.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.RegNo))
            {//+ "\r\n" + "\t" + " , RegNo  
                whereQ += "\r\n\t" + "AND T1.CERTIFICATION_NO LIKE :CerNo";
                model.QueryParameters.Add("CerNo", "%" + model.RegNo.Trim().ToUpper() + "%");
                whereQ2 += "\r\n\t" + "AND T2.CERTIFICATION_NO LIKE :CerNo2";
                model.QueryParameters.Add("CerNo2", "%" + model.RegNo.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.HKID))
            {//+ "\r\n" + "\t" + " , HKID  
                whereQ += "\r\n\t" + "AND " + EncryptDecryptUtil.getDecryptSQL("T4.HKID") + " LIKE :HKID";
                model.QueryParameters.Add("HKID", "%" + model.HKID.Trim().ToUpper() + "%");
                whereQ2 += "\r\n\t" + "AND " + EncryptDecryptUtil.getDecryptSQL("T3.HKID") + " LIKE :HKID2";
                model.QueryParameters.Add("HKID2", "%" + model.HKID.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.PassportNo))
            {//+ "\r\n" + "\t" + " , PASSPORT_NO   
                whereQ += "\r\n\t" + "AND " + EncryptDecryptUtil.getDecryptSQL("T4.PASSPORT_NO") + " LIKE :PassportNo";
                model.QueryParameters.Add("PassportNo", "%" + model.PassportNo.Trim().ToUpper() + "%");
                whereQ2 += "\r\n\t" + "AND " + EncryptDecryptUtil.getDecryptSQL("T3.PASSPORT_NO") + " LIKE :PassportNo2";
                model.QueryParameters.Add("PassportNo2", "%" + model.PassportNo.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.SurnName))
            {//+ "\r\n" + "\t" + " , SURNAME, CATEGORY  
                whereQ += "\r\n\t" + "AND UPPER(T4.SURNAME) LIKE :SurnName";
                model.QueryParameters.Add("SurnName",  model.SurnName.Trim().ToUpper() + "%");
                whereQ2 += "\r\n\t" + "AND UPPER(T3.SURNAME) LIKE :SurnName2";
                model.QueryParameters.Add("SurnName2", model.SurnName.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.GivenName))
            {//+ "\r\n" + "\t" + " , GivenName, CATEGORY  
                whereQ += "\r\n\t" + "AND UPPER(T4.GIVEN_NAME_ON_ID) LIKE :GivenName";
                model.QueryParameters.Add("GivenName", "%" + model.GivenName.Trim().ToUpper() + "%");
                whereQ2 += "\r\n\t" + "AND UPPER(T3.GIVEN_NAME_ON_ID) LIKE :GivenName2";
                model.QueryParameters.Add("GivenName2", "%" + model.GivenName.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.ChiName))
            {//+ "\r\n" + "\t" + " , CHINESE_NAME 
                whereQ += "\r\n\t" + "AND UPPER(T4.CHINESE_NAME) LIKE :ChiName";
                model.QueryParameters.Add("ChiName", "%" + model.ChiName.Trim().ToUpper() + "%");
                whereQ2 += "\r\n\t" + "AND UPPER(T3.CHINESE_NAME) LIKE :ChiName2";
                model.QueryParameters.Add("ChiName2", "%" + model.ChiName.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.ComName))
            {//+ "\r\n" + "\t" + " , COMPANY_NAME  
                whereQ += "\r\n\t" + " AND UPPER(T1.ENGLISH_COMPANY_NAME) LIKE :ComName ";
                whereQ += "\r\n\t" + " OR UPPER(T1.CHINESE_COMPANY_NAME) LIKE :ComName  ";
                model.QueryParameters.Add("ComName", "%" + model.ComName.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.BRNo))
            {//+ "\r\n" + "\t" + " , BR_NO  
                whereQ += "\r\n\t" + "AND UPPER(T1.BR_NO) LIKE :BR_NO";
                model.QueryParameters.Add("BR_NO", "%" + model.BRNo.Trim().ToUpper() + "%");
            }

            if (!string.IsNullOrWhiteSpace(model.SerialNo))
            {//+ "\r\n" + "\t" + " , QP_CARD_SERIAL_NO   
                whereQ += "\r\n\t" + "AND UPPER(T2.CARD_SERIAL_NO) LIKE :SerialNo";
                model.QueryParameters.Add("SerialNo", "%" + model.SerialNo.Trim().ToUpper() + "%");
                whereQ2 += "\r\n\t" + "AND UPPER(T2.CARD_SERIAL_NO) LIKE :SerialNo2";
                model.QueryParameters.Add("SerialNo2", "%" + model.SerialNo.Trim().ToUpper() + "%");
            }

            if (model.IssueDate != null)
            {//+ "\r\n" + "\t" + " , QP_CARD_ISSUE_DATE   
                whereQ += "\r\n\t" + "AND T2.CARD_ISSUE_DATE = :IssueDate";
                model.QueryParameters.Add("IssueDate", model.IssueDate);
                whereQ2 += "\r\n\t" + "AND T2.CARD_ISSUE_DATE = :IssueDate2";
                model.QueryParameters.Add("IssueDate2", model.IssueDate);
            }
            if (model.ExpiryDate != null)
            {//+ "\r\n" + "\t" + " , QP_CARD_EXPIRY_DATE   
                whereQ += "\r\n\t" + "AND T2.CARD_EXPIRY_DATE = :ExpiryDate";
                model.QueryParameters.Add("ExpiryDate", model.ExpiryDate);
                whereQ2 += "\r\n\t" + "AND T2.CARD_EXPIRY_DATE = :ExpiryDate2";
                model.QueryParameters.Add("ExpiryDate2", model.ExpiryDate);
            }
            if (model.ReturnDate != null)
            {//+ "\r\n" + "\t" + " , QP_CARD_RETURN_DATE
                whereQ += "\r\n\t" + "AND T2.CARD_RETURN_DATE = :ReturnDate";
                model.QueryParameters.Add("ReturnDate", model.ReturnDate);
                whereQ2 += "\r\n\t" + "AND T2.CARD_RETURN_DATE = :ReturnDate2";
                model.QueryParameters.Add("ReturnDate2", model.ReturnDate);
            }

            if (!string.IsNullOrWhiteSpace(model.QPService))
            {//+ "\r\n" + "\t" + " , WILLINGNESS_QP   
                whereQ += "\r\n\t" + "AND T1.WILLINGNESS_QP = :QPService";
                model.QueryParameters.Add("QPService", model.QPService);
                whereQ2 += "\r\n\t" + "AND T1.WILLINGNESS_QP = :QPService2";
                model.QueryParameters.Add("QPService2", model.QPService);
            }
            if (!string.IsNullOrWhiteSpace(model.ServiceInMWIS))
            {
                if (RegistrationConstant.SERVICE_IN_MWIS_NO_INDICATION.Equals(model.ServiceInMWIS))
                {
                    whereQ += "\r\n\t" + "AND (T1.SERVICE_IN_MWIS = :ServiceInMWIS or T1.SERVICE_IN_MWIS is null)";
                    model.QueryParameters.Add("ServiceInMWIS", model.ServiceInMWIS);

                    whereQ2 += "\r\n\t" + "AND (T1.SERVICE_IN_MWIS = :ServiceInMWIS2 or T1.SERVICE_IN_MWIS is null)";
                    model.QueryParameters.Add("ServiceInMWIS2", model.ServiceInMWIS);
                }
                else
                {
                    whereQ += "\r\n\t" + "AND T1.SERVICE_IN_MWIS = :ServiceInMWIS";
                    model.QueryParameters.Add("ServiceInMWIS", model.ServiceInMWIS);

                    whereQ2 += "\r\n\t" + "AND T1.SERVICE_IN_MWIS = :ServiceInMWIS2";
                    model.QueryParameters.Add("ServiceInMWIS2", model.ServiceInMWIS);
                }

            }
            return ""
                  + "\r\n" + "\t" + " SELECT                                                                                                               "
                  + "\r\n" + "\t" + " FILE_REFERENCE_NO                                                                                                    "
                  + "\r\n" + "\t" + " , CERTIFICATION_NO                                                                                                   "
                  + "\r\n" + "\t" + " , SURNAME, CATEGORY                                                                                                  "
                  + "\r\n" + "\t" + " , ENGLISH_DESCRIPTION                                                                                                "
                  + "\r\n" + "\t" + " , UUID, REGISTRATION_TYPE                                                                                            "
                  + "\r\n" + "\t" + " , HKID                                                                                                               "
                  + "\r\n" + "\t" + " , PASSPORT_NO                                                                                                        "
                  + "\r\n" + "\t" + " , NAME                                                                                                               "
                  + "\r\n" + "\t" + " , CATGRP                                                                                                             "
                  + "\r\n" + "\t" + " , WILLINGNESS_QP                                                                                                     "
                  + "\r\n" + "\t" + " , COMPANY_NAME                                                                                                       "
                  + "\r\n" + "\t" + " , BR_NO                                                                                                              "
                  + "\r\n" + "\t" + " , CHINESE_NAME                                                                                                       "
                  + "\r\n" + "\t" + " , CHINESE_COMPANY_NAME                                                                                               "
                  + "\r\n" + "\t" + " , EXPIRY_DATE                                                                                                        "
                  + "\r\n" + "\t" + " , QP_CARD_ISSUE_DATE                                                                                                 "
                  + "\r\n" + "\t" + " , QP_CARD_EXPIRY_DATE                                                                                                "
                  + "\r\n" + "\t" + " , QP_CARD_SERIAL_NO                                                                                                  "
                  + "\r\n" + "\t" + " , QP_CARD_RETURN_DATE                                                                                                "
                  //new add
                  + "\r\n" + "\t" + " , QP_SERVICE_IN_MWIS                                                                                                "
                  + "\r\n" + "\t" + " FROM (                                                                                                               "

                  + (all || model.Rgbc || model.TypeA || model.TypeG ? ""
                  + "\r\n" + "\t" + " 	SELECT                                                                                                           "
                  + "\r\n" + "\t" + " 		T1.FILE_REFERENCE_NO AS FILE_REFERENCE_NO                                                                    "
                  + "\r\n" + "\t" + " 		, T1.CERTIFICATION_NO AS CERTIFICATION_NO                                                                    "
                  + "\r\n" + "\t" + " 		, T4.SURNAME||' '||T4.GIVEN_NAME_ON_ID AS SURNAME                                                            "
                  + "\r\n" + "\t" + " 		, T3.CODE AS CATEGORY                                                                                        "
                  + "\r\n" + "\t" + " 		, T5.ENGLISH_DESCRIPTION AS ENGLISH_DESCRIPTION                                                              "
                  + "\r\n" + "\t" + " 		, T1.UUID AS UUID                                                                                            "
                  + "\r\n" + "\t" + " 		, T1.REGISTRATION_TYPE AS REGISTRATION_TYPE                                                                  "
                  + "\r\n" + "\t" + "       , " + EncryptDecryptUtil.getDecryptSQL("T4.HKID") + " as HKID                                                    "
                  + "\r\n" + "\t" + "       , " + EncryptDecryptUtil.getDecryptSQL("T4.PASSPORT_NO") + " as PASSPORT_NO                                     "
                  //+ "\r\n" + "\t" + " 		, C_DECRYPT(T4.HKID, 'XXXXXXXXXXXXXXXXXXXXXXX') AS HKID                                                      "
                  //+ "\r\n" + "\t" + " 		, C_DECRYPT(T4.PASSPORT_NO, 'XXXXXXXXXXXXXXXXXXXXXXX') AS PASSPORT_NO                                        "
                  + "\r\n" + "\t" + " 		, T1.ENGLISH_COMPANY_NAME AS NAME                                                                            "
                  + "\r\n" + "\t" + " 		, T7.CODE AS CATGRP                                                                                          "
                  + "\r\n" + "\t" + " 		,decode(T1.WILLINGNESS_QP,'Y','Yes','N','No','I','No Indication',T1.WILLINGNESS_QP) AS WILLINGNESS_QP        "
                //+ "\r\n" + "\t" + " 		, T1.WILLINGNESS_QP AS WILLINGNESS_QP                                                                        "
                  + "\r\n" + "\t" + " 		, T1.ENGLISH_COMPANY_NAME AS COMPANY_NAME                                                                    "
                  + "\r\n" + "\t" + " 		, T1.BR_NO AS BR_NO                                                                                          "
                  + "\r\n" + "\t" + " 		, TO_CHAR(T4.CHINESE_NAME) AS CHINESE_NAME                                                                   "
                  + "\r\n" + "\t" + " 		, TO_CHAR(T1.CHINESE_COMPANY_NAME) AS CHINESE_COMPANY_NAME                                                   "
                  + "\r\n" + "\t" + " 		, T1.EXPIRY_DATE AS EXPIRY_DATE                                                                              "
                  + "\r\n" + "\t" + " 		, T2.CARD_ISSUE_DATE AS QP_CARD_ISSUE_DATE                                                                   "
                  + "\r\n" + "\t" + " 		, T2.CARD_EXPIRY_DATE AS QP_CARD_EXPIRY_DATE                                                                 "
                  + "\r\n" + "\t" + " 		, T2.CARD_SERIAL_NO AS QP_CARD_SERIAL_NO                                                                     "
                  + "\r\n" + "\t" + " 		, T2.CARD_RETURN_DATE AS QP_CARD_RETURN_DATE                                                                 "
                  //new add
//                  + "\r\n" + "\t" + " 		, T1.SERVICE_IN_MWIS AS QP_SERVICE_IN_MWIS                                                                 "
                  + "\r\n" + "\t" + "       ,decode(T1.SERVICE_IN_MWIS, 'Y','Yes',null,'-',T1.SERVICE_IN_MWIS) AS QP_SERVICE_IN_MWIS                     "
                  + "\r\n" + "\t" + " 	FROM C_COMP_APPLICATION T1                                                                                       "
                  + "\r\n" + "\t" + " 	INNER JOIN C_COMP_APPLICANT_INFO T2 ON T1.UUID = T2.MASTER_ID                                                    "
                  + "\r\n" + "\t" + " 	INNER JOIN C_S_CATEGORY_CODE T3 ON T1.CATEGORY_ID = T3.UUID                                                      "
                  + "\r\n" + "\t" + " 	INNER JOIN C_APPLICANT T4 ON T2.APPLICANT_ID = T4.UUID                                                           "
                  + "\r\n" + "\t" + " 	INNER JOIN C_S_SYSTEM_VALUE T5 ON T1.APPLICATION_STATUS_ID = T5.UUID                                             "
                  + "\r\n" + "\t" + " 	INNER JOIN C_S_SYSTEM_VALUE T7 ON T7.UUID = T3.CATEGORY_GROUP_ID                                                 "
                  + "\r\n" + "\t" + " 	WHERE                                                                                                            "
                  + "\r\n" + "\t" + " 	(                                                                                                                "


                  + (all || model.Rgbc ? ""
                  + "\r\n" + "\t" + " 		 (                                                                                                            "
                  + "\r\n" + "\t" + " 			T1.REGISTRATION_TYPE = 'CGC'                                                                             "
                  + "\r\n" + "\t" + " 			AND T3.CODE = 'GBC'                                                                                      "
                  + "\r\n" + "\t" + " 			AND EXISTS (                                                                                             "
                  + "\r\n" + "\t" + " 				SELECT 1 FROM C_S_SYSTEM_VALUE Q2                                                                    "
                  + "\r\n" + "\t" + " 				WHERE Q2.CODE LIKE 'A%'                                                                              "
                  + "\r\n" + "\t" + " 				AND T2.APPLICANT_ROLE_ID = Q2.UUID                                                                   "
                  + "\r\n" + "\t" + " 			)                                                                                                        "
                  + "\r\n" + "\t" + " 			AND EXISTS (                                                                                             "
                  + "\r\n" + "\t" + " 				SELECT 1 FROM C_S_SYSTEM_VALUE Q2                                                                    "
                  + "\r\n" + "\t" + " 				WHERE Q2.CODE = '1'                                                                                  "
                  + "\r\n" + "\t" + " 				AND T2.APPLICANT_STATUS_ID = Q2.UUID                                                                 "
                  + "\r\n" + "\t" + " 			)                                                                                                        "
                  + "\r\n" + "\t" + " 		)                                                                                                            "
                  : "")


                  ///////////////////////////
                  + (all || (model.Rgbc && (model.TypeA || model.TypeG)) ? ""
                  + "\r\n" + "\t" + " 		  OR                                                                                                         "
                  : "")

                  ///////////////////////////
                  + (all || (model.TypeA || model.TypeG) ? ""
                  + "\r\n" + "\t" + " 		     (                                                                                                       "
                  + "\r\n" + "\t" + " 			T1.REGISTRATION_TYPE = 'CMW'                                                                             "
                  + "\r\n" + "\t" + " 			AND T1.CERTIFICATION_NO IS NOT NULL                                                                      "
                  + "\r\n" + "\t" + " 			AND T2.ACCEPT_DATE IS NOT NULL                                                                           "
                  + "\r\n" + "\t" + " 			AND ((T1.REMOVAL_DATE IS NULL) OR (T1.REMOVAL_DATE < CURRENT_DATE))                                      "
                  + "\r\n" + "\t" + " 			                                                                                                         "
                  + "\r\n" + "\t" + " 			AND EXISTS (                                                                                             "
                  + "\r\n" + "\t" + " 				SELECT 1 FROM C_COMP_APPLICANT_MW_ITEM Q2                                                            "
                  + "\r\n" + "\t" + " 				WHERE EXISTS (                                                                                       "
                  + "\r\n" + "\t" + " 					SELECT 1 FROM C_S_SYSTEM_VALUE Q3                                                                "

                  + "\r\n" + "\t" + " 					WHERE Q3.CODE IN (" + (all || model.TypeA ? "'Type A'" : "''") + "," + (all || model.TypeG ? "'Type G'" : "''") + ")                                                            "

                  + "\r\n" + "\t" + " 					AND Q2.ITEM_TYPE_ID = Q3.UUID                                                                    "
                  + "\r\n" + "\t" + " 				)                                                                                                    "
                  + "\r\n" + "\t" + " 				AND Q2.COMPANY_APPLICANTS_ID = T2.UUID                                                               "
                  + "\r\n" + "\t" + " 			)                                                                                                        "
                  + "\r\n" + "\t" + " 			AND EXISTS (                                                                                             "
                  + "\r\n" + "\t" + " 				SELECT 1 FROM C_S_SYSTEM_VALUE Q2                                                                    "
                  + "\r\n" + "\t" + " 				WHERE Q2.CODE = '1'                                                                                  "
                  + "\r\n" + "\t" + " 				AND T2.APPLICANT_STATUS_ID = Q2.UUID                                                                 "
                  + "\r\n" + "\t" + " 			)                                                                                                        "
                  + "\r\n" + "\t" + " 			AND EXISTS (                                                                                             "
                  + "\r\n" + "\t" + " 				SELECT 1 FROM C_S_SYSTEM_VALUE Q2                                                                    "
                  + "\r\n" + "\t" + " 				WHERE Q2.CODE LIKE 'A%'                                                                              "
                  + "\r\n" + "\t" + " 				AND T2.APPLICANT_ROLE_ID = Q2.UUID                                                                   "
                  + "\r\n" + "\t" + " 			)                                                                                                        "
                  + "\r\n" + "\t" + " 		)                                                                                                            "
                  : "")



                  + "\r\n" + "\t" + " 	)                                                                                                                "
                  + "\r\n" + "\t" + " 	AND (                                                                                                            "
                  + "\r\n" + "\t" + " 		(T1.EXPIRY_DATE IS NOT NULL AND T1.EXPIRY_DATE >= CURRENT_DATE)                                              "
                  + "\r\n" + "\t" + " 		OR                                                                                                           "
                  + "\r\n" + "\t" + " 		(T1.RETENTION_APPLICATION_DATE > ADD_MONTHS(CURRENT_DATE, -12) AND T1.EXPIRY_DATE < CURRENT_DATE)            "
                  + "\r\n" + "\t" + " 	)                                                                                                                "
                  + "\r\n" + "\t" + " 	AND (T1.REMOVAL_DATE IS NULL OR T1.REMOVAL_DATE > CURRENT_DATE)                                                  "
                   : "")
                  + "\r\n" + "\t" + whereQ
                  + (all || ((model.Rgbc || model.TypeA || model.TypeG) && (model.Ap || model.Ri || model.Rse || model.Item36 || model.Item37)) ? ""
                  + "\r\n" + "\t" + " 	UNION                                                                                                            "
                  : "")
               + (all || model.Ap || model.Ri || model.Rse || model.Item36 || model.Item37 ? ""
              + "\r\n" + "\t" + " 	SELECT                                                                                                           "
                  + "\r\n" + "\t" + " 		T1.FILE_REFERENCE_NO AS FILE_REFERENCE_NO                                                                    "
                  + "\r\n" + "\t" + " 		, T2.CERTIFICATION_NO AS CERTIFICATION_NO                                                                    "
                  + "\r\n" + "\t" + " 		, T3.SURNAME ||' '||T3.GIVEN_NAME_ON_ID AS SURNAME                                                            "
                  + "\r\n" + "\t" + " 		, T4.CODE AS CATEGORY                                                                                        "
                  + "\r\n" + "\t" + " 		, T6.ENGLISH_DESCRIPTION AS ENGLISH_DESCRIPTION                                                              "
                  + "\r\n" + "\t" + " 		, T1.UUID AS UUID                                                                                            "
                  + "\r\n" + "\t" + " 		, T1.REGISTRATION_TYPE AS REGISTRATION_TYPE                                                                  "
                  + "\r\n" + "\t" + "       , "+EncryptDecryptUtil.getDecryptSQL("T3.HKID")+" as HKID                                                    "
                  + "\r\n" + "\t" + "       , "+EncryptDecryptUtil.getDecryptSQL("T3.PASSPORT_NO") + " as PASSPORT_NO                                     "
                  //+ "\r\n" + "\t" + " 		, C_DECRYPT( T3.HKID , 'XXXXXXXXXXXXXXXXXXXXXXX' )  AS HKID                                                  "
                  //+ "\r\n" + "\t" + " 		, C_DECRYPT( T3.PASSPORT_NO , 'XXXXXXXXXXXXXXXXXXXXXXX' )  AS PASSPORT_NO                                    "
                  + "\r\n" + "\t" + " 		, T3.SURNAME || ' ' || T3.GIVEN_NAME_ON_ID AS NAME                                                           "
                  + "\r\n" + "\t" + " 		, T5.CODE AS CATGRP                                                                                          "
                  + "\r\n" + "\t" + " 		,decode(T1.WILLINGNESS_QP,'Y','Yes','N','No','I','No Indication',T1.WILLINGNESS_QP) AS WILLINGNESS_QP        "
                  //+ "\r\n" + "\t" + " 		, T1.WILLINGNESS_QP AS WILLINGNESS_QP                                                                        "
                  + "\r\n" + "\t" + " 		, '' AS COMPANY_NAME                                                                                         "
                  + "\r\n" + "\t" + " 		, '' AS BR_NO                                                                                                "
                  + "\r\n" + "\t" + " 		, TO_CHAR(T3.CHINESE_NAME) AS CHINESE_NAME                                                                   "
                  + "\r\n" + "\t" + " 		, '' AS CHINESE_COMPANY_NAME                                                                                 "
                  + "\r\n" + "\t" + " 		, T2.EXPIRY_DATE AS EXPIRY_DATE                                                                              "
                  + "\r\n" + "\t" + " 		, T2.CARD_ISSUE_DATE AS QP_CARD_ISSUE_DATE                                                                   "
                  + "\r\n" + "\t" + " 		, T2.CARD_EXPIRY_DATE AS QP_CARD_EXPIRY_DATE                                                                 "
                  + "\r\n" + "\t" + " 		, T2.CARD_SERIAL_NO AS QP_CARD_SERIAL_NO                                                                     "
                  + "\r\n" + "\t" + " 		, T2.CARD_RETURN_DATE AS QP_CARD_RETURN_DATE                                                                 "
                  //new update
                  + "\r\n" + "\t" + "       ,decode(T1.SERVICE_IN_MWIS, 'Y','Yes',null,'-',T1.SERVICE_IN_MWIS) as QP_SERVICE_IN_MWIS                     "
                  //+ "\r\n" + "\t" + " 		, T1.SERVICE_IN_MWIS AS QP_SERVICE_IN_MWIS                                                                   "
                  + "\r\n" + "\t" + " 	FROM C_IND_APPLICATION T1                                                                                        "
                  + "\r\n" + "\t" + " 	INNER JOIN C_IND_CERTIFICATE T2 ON T1.UUID = T2.MASTER_ID                                                        "
                  + "\r\n" + "\t" + " 	INNER JOIN C_APPLICANT T3 ON T1.APPLICANT_ID = T3.UUID                                                           "
                  + "\r\n" + "\t" + " 	INNER JOIN C_S_CATEGORY_CODE T4 ON T2.CATEGORY_ID = T4.UUID                                                      "
                  + "\r\n" + "\t" + " 	INNER JOIN C_S_SYSTEM_VALUE T5 ON T5.UUID = T4.CATEGORY_GROUP_ID                                                 "
                  + "\r\n" + "\t" + " 	INNER JOIN C_S_SYSTEM_VALUE T6 ON T6.UUID = T2.APPLICATION_STATUS_ID                                             "
                  + "\r\n" + "\t" + " 	WHERE (                                                                                                          "

                  + (all || model.Ap || model.Ri || model.Rse ? ""
                  + "\r\n" + "\t" + " 		T5.CODE IN (" + (all || model.Ap ? "'AP'" : "''") + ", " + (all || model.Ri ? "'RI'" : "''") + ", " + (all || model.Rse ? "'RSE'" : "''") + ")                                                                               "
                  : "")
                  + (all || ((model.Ap || model.Ri || model.Rse) && (model.Item36 || model.Item37)) ? ""
                  + "\r\n" + "\t" + " 		OR                                                                                                          "
                  : "")

                  + (all || model.Item36 || model.Item37 ? ""
                  + "\r\n" + "\t" + " 		 (                                                                                                         "
                  + "\r\n" + "\t" + " 			T1.REGISTRATION_TYPE = 'IMW'                                                                             "
                  + "\r\n" + "\t" + " 			AND EXISTS (                                                                                             "
                  + "\r\n" + "\t" + " 				SELECT 1 FROM C_IND_APPLICATION_MW_ITEM Q1                                                           "
                  + "\r\n" + "\t" + " 				WHERE EXISTS (                                                                                       "
                  + "\r\n" + "\t" + " 					SELECT 1 FROM C_S_SYSTEM_VALUE Q2                                                                "
                  + "\r\n" + "\t" + " 					WHERE Q2.CODE IN (" + (all || model.Item36 ? "'Item 3.6'" : "''") + ", " + (all || model.Item37 ? "'Item 3.7'" : "''") + ")                                "
                  + "\r\n" + "\t" + " 					AND Q1.ITEM_DETAILS_ID = Q2.UUID                                                                 "
                  + "\r\n" + "\t" + " 				)                                                                                                    "
                  + "\r\n" + "\t" + " 				AND Q1.MASTER_ID = T1.UUID                                                                           "
                  + "\r\n" + "\t" + " 			)                                                                                                        "
                  + "\r\n" + "\t" + " 		)                                                                                                            "
                  
                  : "")

                  + "\r\n" + "\t" + " 	)                                                                                                                "
                  + "\r\n" + "\t" + " 	AND (T6.CODE = '1' OR (T2.RETENTION_APPLICATION_DATE IS NOT NULL))                                               "
                  + "\r\n" + "\t" + " 	AND (                                                                                                            "
                  + "\r\n" + "\t" + " 		(T2.EXPIRY_DATE IS NOT NULL AND T2.EXPIRY_DATE >= CURRENT_DATE)                                              "
                  + "\r\n" + "\t" + " 		OR                                                                                                           "
                  + "\r\n" + "\t" + " 		(T2.RETENTION_APPLICATION_DATE > ADD_MONTHS(CURRENT_DATE, -12) AND T2.EXPIRY_DATE < CURRENT_DATE)            "
                  + "\r\n" + "\t" + " 	)                                                                                                                "
                  + "\r\n" + "\t" + " 	AND (T2.REMOVAL_DATE IS NULL OR T2.REMOVAL_DATE > CURRENT_DATE)                                                  "
                  + "\r\n" + "\t" + whereQ2
                  : "")
                  + "\r\n" + "\t" + " ) BASE                                                                                                             "
                  + "\r\n\t where registration_type in ('CGC', 'IMW', 'IP', 'CMW') "
                  ;
        }





        //+ "\r\n" + "\t" + "WHERE T1.REGISTRATION_TYPE   ='CGC'                                  ";

        //String SearchPA_q = ""
        //       + "\r\n" + "\t" + "SELECT indApp.UUID, indApp.FILE_REFERENCE_NO, concat(concat( app.SURNAME ,' '), app.GIVEN_NAME_ON_ID) SURNAME"
        //       + "\r\n" + "\t" + ",sCat.CODE, indCert.CERTIFICATION_NO, svAppStatus.ENGLISH_DESCRIPTION ,"
        //       + "\r\n" + "\t" +    EncryptDecryptUtil.getDecryptSQL("app.hkid") + " as hkid ,"
        //       + "\r\n" + "\t" +    EncryptDecryptUtil.getDecryptSQL("app.passport_no") + " as passport_no "
        //       + "\r\n" + "\t" + "FROM C_ind_application indApp                                   "
        //       + "\r\n" + "\t" + "LEFT JOIN C_applicant app ON app.uuid = indApp.APPLICANT_ID            "
        //       + "\r\n" + "\t" + "LEFT JOIN C_Ind_Certificate indCert ON indCert.master_id = indApp.uuid  "
        //       + "\r\n" + "\t" + "LEFT JOIN C_s_system_value svAppStatus ON svAppStatus.uuid = indCert.APPLICATION_STATUS_ID   "
        //       + "\r\n" + "\t" + "LEFT JOIN C_s_category_code sCat ON sCat.uuid = indCert.CATEGORY_ID                   "
        //       + "\r\n" + "\t" + "where 1=1                                         "
        //       + "\r\n" + "\t" + "and indApp.REGISTRATION_TYPE = 'IP' ";



        String SearchPA_q = ""
           + "\r\n" + "\t" + "SELECT distinct indApp.UUID, indApp.FILE_REFERENCE_NO, concat(concat( app.SURNAME ,' '), app.GIVEN_NAME_ON_ID) SURNAME"
           + "\r\n" + "\t" + ",(select sCat.CODE from C_s_category_code sCat where sCat.uuid = indCert.CATEGORY_ID   ) as CODE, indCert.CERTIFICATION_NO,  (select  svAppStatus.ENGLISH_DESCRIPTION  from  C_s_system_value svAppStatus where svAppStatus.uuid = indCert.APPLICATION_STATUS_ID) as ENGLISH_DESCRIPTION,"
           + "\r\n" + "\t" + EncryptDecryptUtil.getDecryptSQL("app.hkid") + " as hkid ,"
           + "\r\n" + "\t" + EncryptDecryptUtil.getDecryptSQL("app.passport_no") + " as passport_no "
           + "\r\n" + "\t" + "FROM C_Ind_Certificate indCert                                   "
           + "\r\n" + "\t" + "INNER JOIN C_ind_application indApp  ON indApp.uuid =  indCert.master_id  "
           + "\r\n" + "\t" + "INNER JOIN C_applicant app ON app.uuid = indApp.APPLICANT_ID            "
           + "\r\n" + "\t" + " LEFT JOIN c_ind_Qualification iq on iq.MASTER_ID =indApp.uuid"
           + "\r\n" + "\t" + " left join c_ind_Qualification_Detail iqd on iqd.IND_QUALIFICATION_ID = iq.uuid"
           + "\r\n" + "\t" + "where 1=1                                         "
           + "\r\n" + "\t" + "and indApp.REGISTRATION_TYPE = 'IP' ";




        //  + "\r\n" + "\t" + "ORDER BY indCert.certification_No asc                    ";

        string SearchCNV_q = ""
+ "\r\n" + "\t" + "select * from (                                                                                                                                                      "
+ "\r\n" + "\t" + "(select con.UUID  ,case when con.REGISTRATION_TYPE ='CGC' then 'Company' end as ConvictionType ,con.BR_NO ,con.CR_SECTION , con.CR_OFFENCE_DATE as CR_OFFENCE_DATE, con.CR_JUDGE_DATE as CR_JUDGE_DATE, con.srr_Approval_Date as srr_Approval_Date, con.srr_Effective_Date as srr_Effective_Date, con.da_Decision_Date as da_Decision_Date, con.misc_Receiving_Date as  misc_Receiving_Date ,                      "
+ "\r\n" + "\t" + "case when con.CR_ACCIDENT= 'N' and con.CR_FATAL='N' then ''                                                                                                                                  "
+ "\r\n" + "\t" + "when con.CR_ACCIDENT= 'Y' and con.CR_FATAL='N' then 'A'                                                                                                                                      "
+ "\r\n" + "\t" + "when con.CR_ACCIDENT= 'N' and con.CR_FATAL='Y' then 'F'                                                                                                                                      "
+ "\r\n" + "\t" + "when con.CR_ACCIDENT= 'Y' and con.CR_FATAL='Y' then 'A / F' else '' end as AccidentFatal,                                                                                                    "
+ "\r\n" + "\t" + "con.REFERENCE,con.RECORD_TYPE as RECORD_TYPE  , cast(con.ENGLISH_NAME as varchar2(2000) )as name ,                                                                                                           "
+ "\r\n" + "\t" + "con.ENGLISH_NAME,null as HKID                                                                                                                                                                "
+ "\r\n" + "\t" + ",null as PASSPORT                                                                                                                                                                "
+ "\r\n" + "\t" + ", null as SURNAME"
+ "\r\n" + "\t" + ", null as GIVENAME"
+ "\r\n" + "\t" + ", null as CHINESE_NAME"
+ "\r\n" + "\t" + ", SRR_SUSPENSION_DETAILS"
+ "\r\n" + "\t" + ", PROPRI_NAME"
+ "\r\n" + "\t" + ", CONVICTION_SOURCE_ID"

+ "\r\n" + "\t" + "FROM C_Comp_Conviction con                                                                                                                                                                   "
+ "\r\n" + "\t" + "WHERE con.registration_Type = 'CGC'                                                                                                                                                          "
+ "\r\n" + "\t" + "and (                                                                                                                                                                                        "
+ "\r\n" + "\t" + "(con.Conviction_Source_Id                                                                                                                                                                    "
+ "\r\n" + "\t" + "in                                                                                                                                                                                           "
+ "\r\n" + "\t" + "(  select gc.CONVICTION_ID                                                                                                                                                                   "
+ "\r\n" + "\t" + "from C_S_User_Group_Conv_Info gc, SYS_ROLE ug                                                                                                                             "
+ "\r\n" + "\t" + "where gc.sys_role_id=ug.uuid and ug.uuid = '1' ) )                                                                                       "
+ "\r\n" + "\t" + "or  con.Conviction_Source_Id is null )                                                                                                                                                       "
+ "\r\n" + "\t" + ")                                                                                                                                                                                            "
+ "\r\n" + "\t" + "union                                                                                                                                                                                        "
+ "\r\n" + "\t" + "(                                                                                                                                                                                            "
+ "\r\n" + "\t" + "select Indcon.UUID, case when Indcon.REGISTRATION_TYPE ='IP' then 'Individual' end ,null,Indcon.CR_SECTION, Indcon.CR_OFFENCE_DATE as CR_OFFENCE_DATE, Indcon.CR_JUDGEMENT_DATE as CR_JUDGE_DATE,  Indcon.srr_Approval_Date as srr_Approval_Date, Indcon.SRR_EFFECT_DATE as srr_Effective_Date, Indcon.da_Decision_Date as da_Decision_Date, Indcon.misc_Receiving_Date as  misc_Receiving_Date ,                               "
+ "\r\n" + "\t" + "case when Indcon.CR_ACCIDENT= 'N' and Indcon.CR_FATAL='N' then ''                                                                                                                            "
+ "\r\n" + "\t" + "when Indcon.CR_ACCIDENT= 'Y' and Indcon.CR_FATAL='N' then 'A'                                                                                                                                "
+ "\r\n" + "\t" + "when Indcon.CR_ACCIDENT= 'N' and Indcon.CR_FATAL='Y' then 'F'                                                                                                                                "
+ "\r\n" + "\t" + "when Indcon.CR_ACCIDENT= 'Y' and Indcon.CR_FATAL='Y' then 'A / F' else '' end as AccidentFatal,                                                                                              "
+ "\r\n" + "\t" + "null,Indcon.RECORD_TYPE as RECORD_TYPE , cast(Indcon.SURNAME|| ' ' ||Indcon.GIVEN_NAME as varchar2(2000) ) as name,                                                                                         "
 + "\r\n" + "\t" + "Indcon.ENGLISH_COMPANY_NAME," + EncryptDecryptUtil.getDecryptSQL("Indcon.hkid") + " as HKID ,"
 + "\r\n" + "\t" + EncryptDecryptUtil.getDecryptSQL("Indcon.PASSPORT_NO") + " as PASSPORT "
            + "\r\n" + "\t" + ",Indcon.SURNAME"
              + "\r\n" + "\t" + ",Indcon.GIVEN_NAME"
                + "\r\n" + "\t" + ",Indcon.CHINESE_NAME"
                       + "\r\n" + "\t" + ",Indcon.SRR_DETAILS"
                           + "\r\n" + "\t" + ",null as PROPRI_NAME "
            + "\r\n" + "\t" + ",null as CONVICTION_SOURCE_ID"


 + "\r\n" + "\t" + "FROM C_ind_Conviction Indcon                                                                                                                                                                 "
+ "\r\n" + "\t" + "WHERE Indcon.registration_Type = 'IP'                                                                                                                                                        "
+ "\r\n" + "\t" + "and (                                                                                                                                                                                        "
+ "\r\n" + "\t" + "(Indcon.Conviction_Source_Id                                                                                                                                                                 "
+ "\r\n" + "\t" + "in                                                                                                                                                                                           "
+ "\r\n" + "\t" + "(  select gc.CONVICTION_ID                                                                                                                                                                   "
+ "\r\n" + "\t" + "from C_S_User_Group_Conv_Info gc, SYS_ROLE ug                                                                                                                             "
+ "\r\n" + "\t" + "where gc.sys_role_id=ug.uuid and ug.uuid = '1' ) )                                                                                       "
+ "\r\n" + "\t" + "or  Indcon.Conviction_Source_Id is null )                                                                                                                                                    "
+ "\r\n" + "\t" + ")                                                                                                                                                                                            "
+ "\r\n" + "\t" + ")                                                                                                                                                                                            "
+ "\r\n" + "\t" + "where 1=1                                                                                                                                                                                    ";



        /*
        private const string SearchMWCA_q = ""
         + "\r\n" + "\t" + "SELECT                                                               "
         + "\r\n" + "\t" + "T1.UUID                                                              "
         + "\r\n" + "\t" + ", T1.FILE_REFERENCE_NO                                               "
         + "\r\n" + "\t" + ", T1.ENGLISH_COMPANY_NAME                                            "
         + "\r\n" + "\t" + ", T1.CERTIFICATION_NO                                                "

         + "\r\n" + "\t" + ", T1.BR_NO                                                             "
         + "\r\n" + "\t" + ", T1.APPLICATION_DATE                                                  "
         + "\r\n" + "\t" + ", T1.GAZETTE_DATE                                                      "
         + "\r\n" + "\t" + ", T1.REGISTRATION_DATE                                                 "
         + "\r\n" + "\t" + ", T1.EXPIRY_DATE                                                       "
         + "\r\n" + "\t" + ", T1.REMOVAL_DATE                                                      "
         + "\r\n" + "\t" + ", T1.RETENTION_DATE                                                    "
         + "\r\n" + "\t" + ", T1.RESTORE_DATE                                                      "

         + "\r\n" + "\t" + ", T2.ENGLISH_DESCRIPTION                                             "
         + "\r\n" + "\t" + "FROM C_COMP_APPLICATION T1                                           "
         + "\r\n" + "\t" + "LEFT JOIN C_S_SYSTEM_VALUE T2 ON T1.APPLICATION_STATUS_ID = T2.UUID  "
         + "\r\n" + "\t" + "WHERE T1.REGISTRATION_TYPE   ='CMW'                                  ";
         */
        // sql for MWC(W)
        String SearchMWIA_q = ""
 + "\r\n" + "\t" + "SELECT distinct indApp.UUID, indApp.FILE_REFERENCE_NO, concat(concat( app.SURNAME ,' '), app.GIVEN_NAME_ON_ID) NAME"
 + "\r\n" + "\t" + ",(select sCat.CODE from C_s_category_code sCat where sCat.uuid = indCert.CATEGORY_ID   ) as CODE, indCert.CERTIFICATION_NO,  (select  svAppStatus.ENGLISH_DESCRIPTION  from  C_s_system_value svAppStatus where svAppStatus.uuid = indCert.APPLICATION_STATUS_ID) as ENGLISH_DESCRIPTION,"
 + "\r\n" + "\t" + EncryptDecryptUtil.getDecryptSQL("app.hkid") + " as hkid ,"
 + "\r\n" + "\t" + EncryptDecryptUtil.getDecryptSQL("app.passport_no") + " as passport_no ,"
  + "\r\n" + "\t" + " (" + EncryptDecryptUtil.getDecryptSQL("app.hkid") + "  || " + EncryptDecryptUtil.getDecryptSQL("app.passport_no") + " ) as HKIDPASSPORT"
    + "\r\n" + "\t" + " , app.CHINESE_NAME as CHINESE_NAME ,indCert.APPLICATION_DATE , indCert.GAZETTE_DATE ,indCert.REGISTRATION_DATE ,indCert.EXPIRY_DATE ,indCert.REMOVAL_DATE , indCert.RETENTION_DATE, indCert.RESTORE_DATE"
 + "\r\n" + "\t" + "FROM  C_ind_application indApp                                   "
 + "\r\n" + "\t" + "LEFT JOIN  C_Ind_Certificate indCert  ON indApp.uuid =  indCert.master_id  "
 + "\r\n" + "\t" + "LEFT JOIN C_applicant app ON app.uuid = indApp.APPLICANT_ID            "
 + "\r\n" + "\t" + " LEFT JOIN c_ind_Qualification iq on iq.MASTER_ID =indApp.uuid"
 + "\r\n" + "\t" + " left join c_ind_Qualification_Detail iqd on iqd.IND_QUALIFICATION_ID = iq.uuid"
 + "\r\n" + "\t" + "where 1=1                                         "
 + "\r\n" + "\t" + "and indApp.REGISTRATION_TYPE = '" + RegistrationConstant.REGISTRATION_TYPE_MWIA+"' ";
        //String SearchMWIA_q = ""
        //    + "\r\n" + "\t" + "select indApp.UUID, "
        //    + "\r\n" + "\t" + "indApp.FILE_REFERENCE_NO "
        //    + "\r\n" + "\t" + ", app.SURNAME || ' ' || app.GIVEN_NAME_ON_ID as name"
        //    + "\r\n" + "\t" + ", app.SURNAME "
        //    + "\r\n" + "\t" + ", app.GIVEN_NAME_ON_ID "
        //    + "\r\n" + "\t" + ", sCat.code "
        //    + "\r\n" + "\t" + ", indCert.CERTIFICATION_NO "
        //    + "\r\n" + "\t" + ", svAppStatus.ENGLISH_DESCRIPTION "
        //    + "\r\n" + "\t" + "from c_ind_application indApp "
        //    + "\r\n" + "\t" + "left join c_applicant app on app.uuid = indApp.APPLICANT_ID "
        //    + "\r\n" + "\t" + "left join c_Ind_Certificate indCert on indCert.master_id = indApp.uuid "
        //    + "\r\n" + "\t" + "left join c_s_system_value svAppStatus on svAppStatus.uuid = indCert.APPLICATION_STATUS_ID "
        //    + "\r\n" + "\t" + "left join c_s_category_code sCat on sCat.uuid = indCert.CATEGORY_ID "
        //    + "\r\n" + "\t" + "where 1=1 "
        //    + "\r\n" + "\t" + "and indApp.REGISTRATION_TYPE = 'IMW' ";
        //+ "\r\n" + "\t" + "ORDER BY indCert.certification_No asc ";

        private const string SearchRIA_q = ""
            + "\r\n" + "\t" + "SELECT DISTINCT                                                                "
            + "\r\n" + "\t" + "T1.UUID                                                                        "
            + "\r\n" + "\t" + ", T2.FILE_REFERENCE_NO                                                         "
            + "\r\n" + "\t" + ", TRIM(T3.SURNAME||' '||T3.GIVEN_NAME_ON_ID) AS NAME                           "
            + "\r\n" + "\t" + ", T6.CODE                                                                      "
            + "\r\n" + "\t" + ", T1.CERTIFICATION_NO, sv.ENGLISH_DESCRIPTION                                  "
            + "\r\n" + "\t" + "FROM C_IND_CERTIFICATE T1                                                      "
            + "\r\n" + "\t" + "INNER JOIN C_IND_APPLICATION T2 ON T2.UUID = T1.MASTER_ID                      "
            + "\r\n" + "\t" + "INNER JOIN C_APPLICANT T3 ON T3.UUID = T2.APPLICANT_ID                         "
            + "\r\n" + "\t" + "LEFT JOIN C_IND_QUALIFICATION T4 ON T4.MASTER_ID = T2.UUID                     "
            + "\r\n" + "\t" + "inner join C_S_SYSTEM_VALUE sv on sv.UUID = T4.PRB_ID                          "
            + "\r\n" + "\t" + "LEFT JOIN C_IND_QUALIFICATION_DETAIL T5 ON T5.IND_QUALIFICATION_ID = T4.UUID   "
            + "\r\n" + "\t" + "INNER JOIN C_S_CATEGORY_CODE T6 ON T6.UUID = T1.CATEGORY_ID                    "
            + "\r\n" + "\t" + "INNER JOIN C_S_SYSTEM_VALUE T7 ON T6.CATEGORY_GROUP_ID = T7.UUID               "
            + "\r\n" + "\t" + "INNER JOIN C_S_SYSTEM_TYPE T8 ON T8.UUID = T7.SYSTEM_TYPE_ID                   "
            + "\r\n" + "\t" + "WHERE T2.REGISTRATION_TYPE = 'IP'                                              "
            + "\r\n" + "\t" + "AND T8.TYPE = 'CATEGORY_GROUP'                                                 "
            + "\r\n" + "\t" + "AND T7.REGISTRATION_TYPE = 'IP'                                                "
            + "\r\n" + "\t" + "AND T7.CODE = 'RI'                                                             "
            + "\r\n" + "\t" + "AND T1.CERTIFICATION_NO IS NOT NULL                                            "
            + "\r\n" + "\t" + "AND T1.EXPIRY_DATE IS NOT NULL                                                 "
            + "\r\n" + "\t" + "AND T1.EXPIRY_DATE > CURRENT_DATE                                              "
            + "\r\n" + "\t" + "AND (T1.REMOVAL_DATE IS NULL OR T1.REMOVAL_DATE > CURRENT_DATE)                ";

        // Deferral Record for indApplication
        String SearchDFR_indApp_q = ""
            + "\r\n" + "\t" + "select indDeferralRecord.* from "
            + "\r\n" + "\t" + "(select  "
            + "\r\n" + "\t" + "indProMon.uuid as ind_Pro_Mon_Uuid "
            + "\r\n" + "\t" + ", '' as comp_Pro_Mon_Uuid "
            + "\r\n" + "\t" + ", indApp.REGISTRATION_TYPE as Referral_type "
            + "\r\n" + "\t" + ", indApp.FILE_REFERENCE_NO as file_Ref_No "
            + "\r\n" + "\t" + ", app.SURNAME || ' ' || app.GIVEN_NAME_ON_ID as eng_Name "
            + "\r\n" + "\t" + ", sCat.code as cat_Code "
            + "\r\n" + "\t" + ", '' as comp_name "
            + "\r\n" + "\t" + ", '' as candidate_name "
            + "\r\n" + "\t" + ", indProMon.RECEIVED_DATE as rec_Date "
            + "\r\n" + "\t" + ", indProMon.defer_date as deferral_Date "
            + "\r\n" + "\t" + ", '' as role "
            + "\r\n" + "\t" + "from C_IND_PROCESS_MONITOR indProMon  "
            + "\r\n" + "\t" + "inner join C_IND_APPLICATION indApp on indProMon.MASTER_ID=indApp.UUID  "
            + "\r\n" + "\t" + "inner join C_APPLICANT app on indApp.APPLICANT_ID=app.UUID  "
            + "\r\n" + "\t" + "inner join C_s_category_code sCat on sCat.uuid = indProMon.CATEGORY_ID "
            + "\r\n" + "\t" + "where 1=1 "
            + "\r\n" + "\t" + "<CONDITION> "
            //+ "\r\n" + "\t" + "indApp.REGISTRATION_TYPE= 'IP' "
            //+ "\r\n" + "\t" + "and (SYSDATE - indProMon.DEFER_DATE)/30 >= 1 "
            + "\r\n" + "\t" + ") indDeferralRecord ";

        //<CONDITION>

        // Deferral Record for indApplication
        String SearchDFR_compApp_q = ""
            + "\r\n" + "\t" + "select compDeferralRecord.* from "
            + "\r\n" + "\t" + "(select  "
            + "\r\n" + "\t" + "'' as ind_Pro_Mon_Uuid "
            + "\r\n" + "\t" + ", compProMon.uuid as comp_Pro_Mon_Uuid "
            + "\r\n" + "\t" + ", compApp.REGISTRATION_TYPE as Referral_type "
            + "\r\n" + "\t" + ", compApp.FILE_REFERENCE_NO as file_Ref_No "
            + "\r\n" + "\t" + ", '' as eng_Name "
            + "\r\n" + "\t" + ", '' as cat_Code "
            + "\r\n" + "\t" + ", compApp.ENGLISH_COMPANY_NAME as comp_name "
            + "\r\n" + "\t" + ", app.SURNAME || ' ' || app.GIVEN_NAME_ON_ID as candidate_name "
            + "\r\n" + "\t" + ", compProMon.received_date as rec_Date "
            + "\r\n" + "\t" + ", compProMon.DEFER_DATE as deferral_Date "
            + "\r\n" + "\t" + ", svRole.code as role "
            + "\r\n" + "\t" + "from C_COMP_PROCESS_MONITOR compProMon  "
            + "\r\n" + "\t" + "inner join C_COMP_APPLICANT_INFO compAppInfo on compProMon.COMPANY_APPLICANTS_ID=compAppInfo.UUID  "
            + "\r\n" + "\t" + "inner join C_COMP_APPLICATION compApp on compAppInfo.MASTER_ID=compApp.UUID  "
            + "\r\n" + "\t" + "inner join C_APPLICANT app on compAppInfo.APPLICANT_ID=app.UUID  "
            + "\r\n" + "\t" + "inner join C_S_SYSTEM_VALUE svRole on compAppInfo.APPLICANT_ROLE_ID=svRole.UUID  "
            + "\r\n" + "\t" + "<CONDITION> "
            //+ "\r\n" + "\t" + "where compApp.REGISTRATION_TYPE= 'CMW' "
            //+ "\r\n" + "\t" + "and (SYSDATE - compProMon.DEFER_DATE)/30 >= 1 "
            + "\r\n" + "\t" + ") compDeferralRecord ";


        private const string exportCertificate_q = ""
         + "\r\n" + "\t" + "SELECT "
         + "\r\n" + "\t" + "app.UUID AS UUID "
         + "\r\n" + "\t" + "from C_APPLICANT app where rownum <=100";

        private string SearchQP_whereQ(Fn01Search_QPSearchModel model)
        {
            string whereQ = "";
            if (!string.IsNullOrWhiteSpace(model.FileRef))
            {
                whereQ += "\r\n\t" + "AND T1.FILE_REFERENCE_NO LIKE :FileRef";
                model.QueryParameters.Add("FileRef", "%" + model.FileRef.Trim().ToUpper() + "%");
            }
            /*                                                                                                            "
            //+ "\r\n" + "\t" + " FILE_REFERENCE_NO                                                                                                    "
                                                                                                              "
                                                                                                           "
            //+ "\r\n" + "\t" + " , ENGLISH_DESCRIPTION                                                                                                "
            //+ "\r\n" + "\t" + " , UUID, REGISTRATION_TYPE                                                                                            "
                                                                                                                     "
                                                                                                                 "
            //+ "\r\n" + "\t" + " , NAME                                                                                                               "
            //+ "\r\n" + "\t" + " , CATGRP                                                                                                             "
                                                                                                              "
                                                                                                                 "
                                                                                                                        "
                                                                                                                  "
            //+ "\r\n" + "\t" + " , CHINESE_COMPANY_NAME                                                                                               "
            //+ "\r\n" + "\t" + " , EXPIRY_DATE                                                                                                        "
                                                                                                          "
                                                                                                         "
                                                                                                           "

            */

            if (!string.IsNullOrWhiteSpace(model.RegNo))
            {//+ "\r\n" + "\t" + " , CERTIFICATION_NO 
            }
            if (!string.IsNullOrWhiteSpace(model.HKID))
            {//+ "\r\n" + "\t" + " , HKID      
            }
            if (!string.IsNullOrWhiteSpace(model.PassportNo))
            {//+ "\r\n" + "\t" + " , PASSPORT_NO   
            }
            if (!string.IsNullOrWhiteSpace(model.SurnName))
            {//+ "\r\n" + "\t" + " , SURNAME, CATEGORY   
            }

            if (!string.IsNullOrWhiteSpace(model.ChiName))
            {//+ "\r\n" + "\t" + " , CHINESE_NAME 
            }
            if (!string.IsNullOrWhiteSpace(model.ComName))
            {//+ "\r\n" + "\t" + " , COMPANY_NAME  
            }
            if (!string.IsNullOrWhiteSpace(model.BRNo))
            {//+ "\r\n" + "\t" + " , BR_NO  
            }


            if (!string.IsNullOrWhiteSpace(model.SerialNo))
            {//+ "\r\n" + "\t" + " , QP_CARD_SERIAL_NO   
            }

            if (model.IssueDate != null)
            {//+ "\r\n" + "\t" + " , QP_CARD_ISSUE_DATE   
            }
            if (model.ExpiryDate != null)
            {//+ "\r\n" + "\t" + " , QP_CARD_EXPIRY_DATE   
            }
            if (model.ReturnDate != null)
            {//+ "\r\n" + "\t" + " , QP_CARD_RETURN_DATE
            }

            if (!string.IsNullOrWhiteSpace(model.QPService))
            {//+ "\r\n" + "\t" + " , WILLINGNESS_QP   
            }











            /*
            if (!string.IsNullOrWhiteSpace(model.ComName))
            {
                whereQ += "\r\n\t" + "AND EXISTS(SELECT 1 FROM C_COMP_APPLICANT_INFO T2 WHERE T2.MASTER_ID = T1.UUID AND EXISTS(SELECT 1 FROM C_APPLICANT T3 WHERE T3.UUID = T2.APPLICANT_ID AND UPPER(T3.SURNAME) LIKE :Surname))";
                model.QueryParameters.Add("Surname", model.Surname.Trim().ToUpper() + "%");
            }

            if (!string.IsNullOrWhiteSpace(model.BRNo))
            {
                whereQ += "\r\n\t" + "AND EXISTS(SELECT 1 FROM C_COMP_APPLICANT_INFO T2 WHERE T2.MASTER_ID = T1.UUID AND EXISTS(SELECT 1 FROM C_APPLICANT T3 WHERE T3.UUID = T2.APPLICANT_ID AND UPPER(T3.GIVEN_NAME_ON_ID) LIKE :GivenName))";
                model.QueryParameters.Add("GivenName", (model.KeywordSearch ? "%" : "") + model.GivenName.Trim().ToUpper() + "%");
            }

            if (!string.IsNullOrWhiteSpace(model.QPTypeList))
            {
                whereQ += "\r\n\t" + "AND EXISTS(SELECT 1 FROM C_COMP_APPLICANT_INFO T2 WHERE T2.MASTER_ID = T1.UUID AND EXISTS(SELECT 1 FROM C_APPLICANT T3 WHERE T3.UUID = T2.APPLICANT_ID AND UPPER(T3.CHINESE_NAME) LIKE :ChineseName))";
                model.QueryParameters.Add("ChineseName", "%" + model.ChineseName.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.SerialNo))
            {
                whereQ += "\r\n\t" + "AND T1.PRACTICE_NOTES_ID = :Pnrc";
                model.QueryParameters.Add("Pnrc", model.Pnrc);
            }
            if (!string.IsNullOrWhiteSpace(model.IssueDate))
            {
                whereQ += "\r\n\t" + "AND EXISTS(SELECT 1 FROM C_BUILDING_SAFETY_INFO T2 WHERE T2.MASTER_ID = T1.UUID AND T2.BUILDING_SAFETY_ID = :ServiceInBS)";
                model.QueryParameters.Add("ServiceInBS", model.ServiceInBS);
            }

            if (string.IsNullOrWhiteSpace(model.ReturnDate))
            {
                whereQ += "\r\n\t" + "AND T1." + model.DateType + " <= :DateTo";
                model.QueryParameters.Add("DateTo", model.DateTo);
            }


            if (!string.IsNullOrWhiteSpace(model.ExpiryDate) && (!string.IsNullOrWhiteSpace(model.DateFrom) || !string.IsNullOrWhiteSpace(model.DateTo)))
            {
                if (string.IsNullOrWhiteSpace(model.DateFrom))
                {
                    whereQ += "\r\n\t" + "AND T1." + model.DateType + " >= :DateFrom";
                    model.QueryParameters.Add("DateFrom", model.DateFrom);
                }
                if (string.IsNullOrWhiteSpace(model.ReturnDate))
                {
                    whereQ += "\r\n\t" + "AND T1." + model.DateType + " <= :DateTo";
                    model.QueryParameters.Add("DateTo", model.DateTo);
                }
            }*/
            return whereQ;
        }



        private string SearchRIA_whereQ(Fn01Search_RIASearchModel model)
        {
            string whereQ = "";
            if (!string.IsNullOrWhiteSpace(model.FileRef))
            {
                whereQ += "\r\n\t" + "AND UPPER(T2.FILE_REFERENCE_NO) LIKE :FileRef";
                model.QueryParameters.Add("FileRef", "%" + model.FileRef.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.RegNo))
            {
                whereQ += "\r\n\t" + "AND UPPER(T1.CERTIFICATION_NO) LIKE :RegNo";
                model.QueryParameters.Add("RegNo", "%" + model.RegNo.Trim().ToUpper() + "%");
            }

            if (!string.IsNullOrWhiteSpace(model.HKID))
            {
                whereQ += "\r\n\t" + "AND " + EncryptDecryptUtil.getDecryptSQL("T3.HKID") + " LIKE :HKID";
                model.QueryParameters.Add("HKID", "%" + model.HKID.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.PassportNo))
            {
                whereQ += "\r\n\t" + "AND " + EncryptDecryptUtil.getDecryptSQL("T3.PASSPORT_NO") + " LIKE :PassportNo";
                model.QueryParameters.Add("PassportNo", "%" + model.PassportNo.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.SurnName))
            {
                whereQ += "\r\n\t" + "AND UPPER(T3.SURNAME) LIKE :SurnName";
                model.QueryParameters.Add("SurnName", model.SurnName.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.GivenName))
            {
                whereQ += "\r\n\t" + "AND UPPER(T3.GIVEN_NAME_ON_ID) LIKE :GivenName";
                model.QueryParameters.Add("GivenName", model.GivenName.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.ChiName))
            {
                whereQ += "\r\n\t" + "AND UPPER(T3.CHINESE_NAME) LIKE :ChiName";
                model.QueryParameters.Add("ChiName", "%" + model.ChiName.Trim() + "%");
            }
            //
            if (!string.IsNullOrWhiteSpace(model.Address))
            {
                string s = "XXXXXXXXXXXXselect ";
            }

            if (!string.IsNullOrWhiteSpace(model.ServiceInBS))
            {
                whereQ += "\r\n\t" + "AND EXISTS(SELECT 1 FROM C_BUILDING_SAFETY_INFO WHERE MASTER_ID = T2.UUID AND BUILDING_SAFETY_ID = :ServiceInBS)";
                model.QueryParameters.Add("ServiceInBS", model.ServiceInBS);
            }
            if (!string.IsNullOrWhiteSpace(model.Pnrc))
            {
                whereQ += "\r\n\t" + "AND T2.PRACTICE_NOTES_ID = :Pnrc";
                model.QueryParameters.Add("Pnrc", model.Pnrc);
            }
            if (!string.IsNullOrWhiteSpace(model.Sex))
            {
                whereQ += "\r\n\t" + "AND T3.GENDER = :Sex";
                model.QueryParameters.Add("Sex", model.Sex);
            }
            if (model.Prb != null && model.Prb.Count > 0)
            {
                whereQ += "\r\n\t" + "AND T4.PRB_ID IN(:Prb)";
                model.QueryParameters.Add("Prb", model.Prb);
            }
            if (model.Qcode != null && model.Qcode.Count > 0)
            {
                whereQ += "\r\n\t" + "AND T6.UUID IN(:Qcode)";
                model.QueryParameters.Add("Qcode", model.Qcode);
            }
            if (model.DisDiv != null && model.DisDiv.Count > 0)
            {
                whereQ += "\r\n\t" + "AND T5.S_CATEGORY_CODE_DETAIL_ID IN(:DisDiv)";
                model.QueryParameters.Add("DisDiv", model.DisDiv);
            }
            return whereQ;
        }

        public Fn01Search_QPSearchModel SearchQP(Fn01Search_QPSearchModel model)
        {
            model.Query = SearchQP_q(model);
            //model.QueryWhere = SearchQP_whereQ(model);
            model.Search();
            return model;
        }
        public string ExportQP(Fn01Search_QPSearchModel model)
        {
            model.Query = SearchQP_q(model);
            //model.QueryWhere = SearchQP_whereQ(model);
            return model.Export("ExportData");
        }
        public Fn06AS_DBSearchModel SearchAS(Fn06AS_DBSearchModel model)
        {
            model.Query = SearchAS_q(model);
            model.Search();
            return model;
        }
        public Fn01Search_CNVSearchModel SearchCNV(Fn01Search_CNVSearchModel model)
        {
            model.Query = SearchCNV_q;

            model.QueryWhere = SearchCNV_whereQ(model);

            model.Search();
            return model;
        }
        private string SearchCNV_whereQ(Fn01Search_CNVSearchModel model)
        {
            string whereQ = "";
            if (!string.IsNullOrWhiteSpace(model.HKID))
            {
                whereQ += "\r\n\t" + "AND " + "HKID" + " LIKE :HKID";
                model.QueryParameters.Add("HKID", "%" + model.HKID.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.PassportNo))
            {
                whereQ += "\r\n\t" + "AND " + "PASSPORT" + " LIKE :PassportNo";
                model.QueryParameters.Add("PassportNo", "%" + model.PassportNo.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.Surname))
            {
                whereQ += "\r\n\t" + "AND UPPER(SURNAME) LIKE :Surname";
                model.QueryParameters.Add("Surname", "%" + model.Surname.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.GivenName))
            {
                whereQ += "\r\n\t" + "AND UPPER(GIVENAME) LIKE :GivenName";
                model.QueryParameters.Add("GivenName", "%" + model.GivenName.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.ChiName))
            {
                whereQ += "\r\n\t" + "AND CHINESE_NAME LIKE :ChiName";
                model.QueryParameters.Add("ChiName", "%" + model.ChiName + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.ProprietorName))
            {
                whereQ += "\r\n\t" + "AND UPPER(PROPRI_NAME) LIKE :ProprietorName";
                model.QueryParameters.Add("ProprietorName", "%" + model.ProprietorName.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.BRNo))
            {
                whereQ += "\r\n\t" + "AND UPPER(BR_NO) LIKE :BRNo";
                model.QueryParameters.Add("BRNo", "%" + model.BRNo.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.ComName))
            {
                whereQ += "\r\n\t" + "AND UPPER(ENGLISH_NAME) LIKE :ComName";
                model.QueryParameters.Add("ComName", "%" + model.ComName.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.OrdReg))
            {
                whereQ += "\r\n\t" + "AND UPPER(CR_SECTION) LIKE :OrdReg";
                model.QueryParameters.Add("OrdReg", "%" + model.OrdReg.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.SuspReason))
            {
                whereQ += "\r\n\t" + "AND UPPER(SRR_SUSPENSION_DETAILS) LIKE :SuspReason";
                model.QueryParameters.Add("SuspReason", "%" + model.SuspReason.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.Ref))
            {
                whereQ += "\r\n\t" + "AND UPPER(REFERENCE) LIKE :Ref";
                model.QueryParameters.Add("Ref", "%" + model.Ref.Trim().ToUpper() + "%");
            }

            if (!string.IsNullOrWhiteSpace(model.CnvSource))
            {
                whereQ += "\r\n\t" + "AND (select sv.uuid from C_S_SYSTEM_VALUE sv where sv.uuid= CONVICTION_SOURCE_ID ) = :CnvSource";
                model.QueryParameters.Add("CnvSource", model.CnvSource);
            }
            bool combineDateSearch = false;
            if (model.FromDateOfOffence != null && model.ToDateOfOffence != null && model.FromDateOfJudgement != null && model.ToDateOfJudgement != null)
            {
                combineDateSearch = true;

            }
            if (model.FromDateOfOffence != null && !combineDateSearch)
            {
                whereQ += "\r\n\t" + "AND CR_OFFENCE_DATE >= :DateFrom";
                model.QueryParameters.Add("DateFrom", model.FromDateOfOffence);
            }
            if (model.ToDateOfOffence != null && !combineDateSearch)
            {
                whereQ += "\r\n\t" + "AND CR_OFFENCE_DATE <= :DateTo";
                model.QueryParameters.Add("DateTo", model.ToDateOfOffence);
            }
      


            if (model.FromDateOfJudgement != null && !combineDateSearch)
            {// OR ( RECORD_TYPE='S'  AND srr_Approval_Date >= :DateFrom) )OR ( RECORD_TYPE='S'  AND (srr_Effective_Date >= :DateFrom) )OR ( RECORD_TYPE='D'  AND ( da_Decision_Date >= :DateFrom   ) )OR ( RECORD_TYPE='M'  AND ( misc_Receiving_Date >= :DateFrom  ) )
                whereQ += "\r\n\t" + "AND ((RECORD_TYPE='C'  AND ( CR_JUDGE_DATE >= :DateFromJ1))  OR ( RECORD_TYPE='S'  AND ( srr_Approval_Date >= :DateFromJ2)) OR ( RECORD_TYPE='S'  AND (srr_Effective_Date >= :DateFromJ3)) OR ( RECORD_TYPE='D'  AND ( da_Decision_Date >= :DateFromJ4   )) OR ( RECORD_TYPE='M'  AND ( misc_Receiving_Date >= :DateFromJ5  )  ))";
                model.QueryParameters.Add("DateFromJ1", model.FromDateOfJudgement);
                model.QueryParameters.Add("DateFromJ2", model.FromDateOfJudgement);
                model.QueryParameters.Add("DateFromJ3", model.FromDateOfJudgement);
                model.QueryParameters.Add("DateFromJ4", model.FromDateOfJudgement);
                model.QueryParameters.Add("DateFromJ5", model.FromDateOfJudgement);
            }


            if (model.ToDateOfJudgement != null && !combineDateSearch)
            {
                whereQ += "\r\n\t" + "AND ((RECORD_TYPE='C'  AND ( CR_JUDGE_DATE <= :DateToJ1))  OR ( RECORD_TYPE='S'  AND ( srr_Approval_Date <= :DateToJ2)) OR ( RECORD_TYPE='S'  AND (srr_Effective_Date <= :DateToJ3)) OR ( RECORD_TYPE='D'  AND ( da_Decision_Date <= :DateToJ4   )) OR ( RECORD_TYPE='M'  AND ( misc_Receiving_Date <= :DateToJ5  )  ))";

                model.QueryParameters.Add("DateToJ1", model.ToDateOfJudgement);
                model.QueryParameters.Add("DateToJ2", model.ToDateOfJudgement);
                model.QueryParameters.Add("DateToJ3", model.ToDateOfJudgement);
                model.QueryParameters.Add("DateToJ4", model.ToDateOfJudgement);
                model.QueryParameters.Add("DateToJ5", model.ToDateOfJudgement);
            }

            if (combineDateSearch)
            {
                whereQ += "\r\n\t" + "AND ((";
                whereQ += "\r\n\t" + " CR_OFFENCE_DATE >= :DateFrom";
                model.QueryParameters.Add("DateFrom", model.FromDateOfOffence);
                whereQ += "\r\n\t" + "AND CR_OFFENCE_DATE <= :DateTo";
                model.QueryParameters.Add("DateTo", model.ToDateOfOffence);
                whereQ += "\r\n\t" + ") or (";

                whereQ += "\r\n\t" + " ((RECORD_TYPE='C'  AND ( CR_JUDGE_DATE >= :DateFromJ1))  OR ( RECORD_TYPE='S'  AND ( srr_Approval_Date >= :DateFromJ2)) OR ( RECORD_TYPE='S'  AND (srr_Effective_Date >= :DateFromJ3)) OR ( RECORD_TYPE='D'  AND ( da_Decision_Date >= :DateFromJ4   )) OR ( RECORD_TYPE='M'  AND ( misc_Receiving_Date >= :DateFromJ5  )  ))";
                model.QueryParameters.Add("DateFromJ1", model.FromDateOfJudgement);
                model.QueryParameters.Add("DateFromJ2", model.FromDateOfJudgement);
                model.QueryParameters.Add("DateFromJ3", model.FromDateOfJudgement);
                model.QueryParameters.Add("DateFromJ4", model.FromDateOfJudgement);
                model.QueryParameters.Add("DateFromJ5", model.FromDateOfJudgement);
                whereQ += "\r\n\t" + "AND ((RECORD_TYPE='C'  AND ( CR_JUDGE_DATE <= :DateToJ1))  OR ( RECORD_TYPE='S'  AND ( srr_Approval_Date <= :DateToJ2)) OR ( RECORD_TYPE='S'  AND (srr_Effective_Date <= :DateToJ3)) OR ( RECORD_TYPE='D'  AND ( da_Decision_Date <= :DateToJ4   )) OR ( RECORD_TYPE='M'  AND ( misc_Receiving_Date <= :DateToJ5  )  ))";

                model.QueryParameters.Add("DateToJ1", model.ToDateOfJudgement);
                model.QueryParameters.Add("DateToJ2", model.ToDateOfJudgement);
                model.QueryParameters.Add("DateToJ3", model.ToDateOfJudgement);
                model.QueryParameters.Add("DateToJ4", model.ToDateOfJudgement);
                model.QueryParameters.Add("DateToJ5", model.ToDateOfJudgement);
                whereQ += "\r\n\t" + "))";
            }


            if (model.CnvType != null)
            {
                whereQ += "\r\n\t" + "AND ConvictionType = :CnvType";
                model.QueryParameters.Add("CnvType", model.CnvType);
          
            }




            return whereQ;
        }


        public Fn01Search_RIASearchModel SearchRIA(Fn01Search_RIASearchModel model)
        {
            model.Query = SearchRIA_q;
            model.QueryWhere = SearchRIA_whereQ(model);
            model.Search();
            return model;
        }
        public string ExportRIA(Fn01Search_RIASearchModel model)
        {
            model.Query = SearchRIA_q;
            model.QueryWhere = SearchRIA_whereQ(model);
            return model.Export("ExportData");
        }

        public Fn01Search_MWIADisplayModel ViewMWIA(string id)
        {
            string ServiceInMWIS = "";
            if (string.IsNullOrEmpty(id))
            {
                BuildingSafetyCheckListModel newbsModel = new BuildingSafetyCheckListModel()
                { MasterUuid = id, RegType = RegistrationConstant.REGISTRATION_TYPE_IP };
                newbsModel.init();
                return new Fn01Search_MWIADisplayModel()
                {
                    BsModel = newbsModel
                };
            }

            using (EntitiesRegistration db = new EntitiesRegistration())
            {

                var q = db.C_IND_APPLICATION.Where(x => x.UUID == id)
                    .Include(x => x.C_APPLICANT)
                    .Include(x => x.C_ADDRESS1)
                    .Include(x => x.C_ADDRESS2)
                    .Include(x => x.C_ADDRESS3)
                    .Include(x => x.C_ADDRESS4)
                    .Include(x => x.C_ADDRESS5)
                    .Include(x => x.C_IND_CERTIFICATE)
                       .Include(x => x.C_IND_CERTIFICATE.Select(y => y.C_S_CATEGORY_CODE))
                       .Include(x => x.C_IND_CERTIFICATE.Select(y => y.C_S_CATEGORY_CODE).Select(z => z.C_S_SYSTEM_VALUE))
                        .Include(x => x.C_IND_CERTIFICATE.Select(y => y.C_S_SYSTEM_VALUE)).FirstOrDefault();
                    ;
                List<C_BUILDING_SAFETY_INFO> BuildSafetyInfo = new List<C_BUILDING_SAFETY_INFO>();

                if (!string.IsNullOrEmpty(id))
                {
                    BuildSafetyInfo = db.C_BUILDING_SAFETY_INFO.Include("C_S_SYSTEM_VALUE").Where(o => o.MASTER_ID == q.UUID).ToList();

                }
             

                //var queryApp = from IndApp in db.C_IND_APPLICATION
                //               join ChinAdd in db.C_ADDRESS on IndApp.CHINESE_HOME_ADDRESS_ID equals ChinAdd.UUID
                //               join EngAdd in db.C_ADDRESS on IndApp.ENGLISH_HOME_ADDRESS_ID equals EngAdd.UUID
                //               join BSChiAdd in db.C_ADDRESS on IndApp.CHINESE_BS_ADDRESS_ID equals BSChiAdd.UUID
                //               join BSEngAdd in db.C_ADDRESS on IndApp.ENGLISH_BS_ADDRESS_ID equals BSEngAdd.UUID
                //               join OfficeChiAdd in db.C_ADDRESS on IndApp.CHINESE_OFFICE_ADDRESS_ID equals OfficeChiAdd.UUID
                //               join OfficeEngAdd in db.C_ADDRESS on IndApp.ENGLISH_OFFICE_ADDRESS_ID equals OfficeEngAdd.UUID
                //               join Applicant in db.C_APPLICANT on IndApp.APPLICANT_ID equals Applicant.UUID
                //               join Certi in db.C_IND_CERTIFICATE on IndApp.UUID equals Certi.MASTER_ID
                //               join cat in db.C_S_CATEGORY_CODE on Certi.CATEGORY_ID equals cat.UUID
                //               join sv in db.C_S_SYSTEM_VALUE on cat.CATEGORY_GROUP_ID equals sv.UUID
                //               join sv_app_status in db.C_S_SYSTEM_VALUE on Certi.APPLICATION_STATUS_ID equals sv_app_status.UUID
                //               where IndApp.UUID == id
                //               select new { IndApp, ChinAdd, EngAdd, BSChiAdd, BSEngAdd, OfficeChiAdd, OfficeEngAdd, Applicant, Certi, sv, sv_app_status };

                //var BuildSafetyInfo = db.C_BUILDING_SAFETY_INFO.Include("C_S_SYSTEM_VALUE").Where(o => o.MASTER_ID == queryApp.FirstOrDefault().IndApp.UUID).ToList();

                var MWItemListQuery = from IndApp in db.C_IND_APPLICATION
                                      join indAppMWItem in db.C_IND_APPLICATION_MW_ITEM on IndApp.UUID equals indAppMWItem.MASTER_ID
                                      join sv_mw_item in db.C_S_SYSTEM_VALUE on indAppMWItem.ITEM_DETAILS_ID equals sv_mw_item.UUID
                                      where IndApp.UUID == id
                                      orderby sv_mw_item.CODE.Substring(7)
                                      
                                      select sv_mw_item;

                var OrderMWItemList = MWItemListQuery.ToList();
                OrderMWItemList = OrderMWItemList.OrderBy(x => Int32.Parse(x.CODE.Substring(7))).ToList();

                BuildingSafetyCheckListModel bsModel = new BuildingSafetyCheckListModel()
                { MasterUuid = id, RegType = RegistrationConstant.REGISTRATION_TYPE_MWIA };
                bsModel.init();
                //new add
                if (q.SERVICE_IN_MWIS == null)
                {
                    ServiceInMWIS = "-";
                }
                else
                {
                    ServiceInMWIS = q.SERVICE_IN_MWIS;
                }

                return new Fn01Search_MWIADisplayModel
                {

                    C_IND_APPLICATION = q,
                    C_APPLICANT  = q?.C_APPLICANT,
                    HOME_ADDRESS_ENG = q?.C_ADDRESS,
                    HOME_ADDRESS_CHI =q?.C_ADDRESS1,
                    OFFICE_ADDRESS_CHI = q?.C_ADDRESS2,
                    OFFICE_ADDRESS_ENG= q?.C_ADDRESS3,
                    BS_ADDRESS_ENG = q?.C_ADDRESS4,
                    BS_ADDRESS_CHI = q?.C_ADDRESS5,
                    C_IND_CERTIFICATE =q?.C_IND_CERTIFICATE.FirstOrDefault(),
                    SV_CATEGORY_C_S_SYSTEM_VALUE = q?.C_IND_CERTIFICATE.FirstOrDefault().C_S_CATEGORY_CODE.C_S_SYSTEM_VALUE,
                    APP_STATUS_C_S_SYSTEM_VALUE = q?.C_IND_CERTIFICATE.FirstOrDefault().C_S_SYSTEM_VALUE2,
               
                    //C_IND_APPLICATION = queryApp.FirstOrDefault().IndApp,
                    //C_APPLICANT = queryApp.FirstOrDefault().Applicant,
                    //HOME_ADDRESS_ENG = queryApp.FirstOrDefault().EngAdd,
                    //HOME_ADDRESS_CHI = queryApp.FirstOrDefault().ChinAdd,
                    //C_IND_CERTIFICATE = queryApp.FirstOrDefault().Certi,

                    //SV_CATEGORY_C_S_SYSTEM_VALUE = queryApp.FirstOrDefault().sv,
                    //APP_STATUS_C_S_SYSTEM_VALUE = queryApp.FirstOrDefault().sv_app_status,
                    //
                    //OFFICE_ADDRESS_CHI =queryApp.FirstOrDefault().OfficeChiAdd,
                    //OFFICE_ADDRESS_ENG = queryApp.FirstOrDefault().OfficeEngAdd,
                    //BS_ADDRESS_ENG = queryApp.FirstOrDefault().BSEngAdd,
                    //BS_ADDRESS_CHI = queryApp.FirstOrDefault().BSChiAdd,
                    MWITEMLIST = OrderMWItemList, // MWItemListQuery.ToList(),
                    C_BUILDING_SAFETY_INFOs = BuildSafetyInfo,
                    BsModel = bsModel,
                    ServiceInMWIS = ServiceInMWIS
                };

            }




        }


        public string ExportMWIA(Dictionary<string, string>[] Columns, FormCollection post)
        {
            DisplayGrid dlr = new DisplayGrid() { Query = SearchMWIA_q, Columns = Columns, Parameters = post };
            //dlr.Sort = "indCert.certification_No";
            //dlr.SortType = 2;
            return dlr.Export("ExportData");
        }


        public Fn01Search_PASearchModel SearchPA(Fn01Search_PASearchModel model)
        {
            model.Query = SearchPA_q;

            model.QueryWhere = SearchPA_whereQ(model);

            model.Search();
            return model;
        }
        private string SearchPA_whereQ(Fn01Search_PASearchModel model)
        {
            string whereQ = "";
            if (!string.IsNullOrWhiteSpace(model.FileRef))
            {
                whereQ += "\r\n\t" + "AND indApp.FILE_REFERENCE_NO LIKE :FileRef";
                model.QueryParameters.Add("FileRef", "%" + model.FileRef.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.TelNo))
            {
                whereQ += "\r\n\t" + "AND ( (replace(indApp.TELEPHONE_NO1,' ','') LIKE :TelNo) or (replace(indApp.TELEPHONE_NO2,' ','') LIKE :TelNo) or (replace(indApp.TELEPHONE_NO3,' ','' ) LIKE :TelNo) ) ";
                model.QueryParameters.Add("TelNo", "%" + model.TelNo.Trim().Replace(" ", "") + "%");


            }
            if (!string.IsNullOrWhiteSpace(model.Email))
            {
                whereQ += "\r\n\t" + "AND UPPER(indApp.EMAIL) like :Email";
                model.QueryParameters.Add("Email", "%" + model.Email.Trim().ToUpper() + "%");


            }


            if (!string.IsNullOrWhiteSpace(model.RegNo))
            {
                whereQ += "\r\n\t" + "AND indCert.CERTIFICATION_NO LIKE :RegNo";
                model.QueryParameters.Add("RegNo", "%" + model.RegNo.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.HKID))
            {
                whereQ += "\r\n\t" + "AND " + EncryptDecryptUtil.getDecryptSQL("app.hkid") + " LIKE :HKID";
                model.QueryParameters.Add("HKID", "%" + model.HKID.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.PassportNo))
            {
                whereQ += "\r\n\t" + "AND " + EncryptDecryptUtil.getDecryptSQL("app.passport_no") + " LIKE :PassportNo";
                model.QueryParameters.Add("PassportNo", "%" + model.PassportNo.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.SurnName))
            {
                whereQ += "\r\n\t" + "AND UPPER(app.SURNAME) LIKE :SurnName";
                model.QueryParameters.Add("SurnName", "%" + model.SurnName.Trim().ToUpper() + "%");

            }
            if (!string.IsNullOrWhiteSpace(model.GivenName))
            {
                whereQ += "\r\n\t" + "AND UPPER(app.GIVEN_NAME_ON_ID) LIKE :GivenName";
                model.QueryParameters.Add("GivenName", (model.KeywordSearch ? "%" : "") + model.GivenName.Trim().ToUpper() + "%");

            }

            if (!string.IsNullOrWhiteSpace(model.ChiName))
            {
                whereQ += "\r\n\t" + "AND app.CHINESE_NAME LIKE :ChiName";
                model.QueryParameters.Add("ChiName", "%" + model.ChiName.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.Address))
            {
                if (model.Address.Any(c => (uint)c >= 0x4E00 && (uint)c <= 0x2FA1F))
                {
                    whereQ += "\r\n\t" + "AND ((EXISTS (SELECT 1 FROM C_ADDRESS T2 WHERE T2.UUID = indApp.CHINESE_HOME_ADDRESS_ID AND UPPER(T2.ADDRESS_LINE1||T2.ADDRESS_LINE2||T2.ADDRESS_LINE3||T2.ADDRESS_LINE4||T2.ADDRESS_LINE5) LIKE :AddressChi ) or  (EXISTS (SELECT 1 FROM C_ADDRESS T2 WHERE T2.UUID = indApp.CHINESE_OFFICE_ADDRESS_ID AND UPPER(T2.ADDRESS_LINE1||T2.ADDRESS_LINE2||T2.ADDRESS_LINE3||T2.ADDRESS_LINE4||T2.ADDRESS_LINE5) LIKE :AddressChi)) ))";
                    model.QueryParameters.Add("AddressChi", "%" + model.Address.Trim().ToUpper() + "%");
                }
                else
                {
                    whereQ += "\r\n\t" + "AND ( (EXISTS (SELECT 1 FROM C_ADDRESS T2 WHERE T2.UUID = indApp.ENGLISH_HOME_ADDRESS_ID AND UPPER(T2.ADDRESS_LINE1||T2.ADDRESS_LINE2||T2.ADDRESS_LINE3||T2.ADDRESS_LINE4||T2.ADDRESS_LINE5) LIKE :AddressEng) or (EXISTS (SELECT 1 FROM C_ADDRESS T2 WHERE T2.UUID = indApp.ENGLISH_OFFICE_ADDRESS_ID AND UPPER(T2.ADDRESS_LINE1||T2.ADDRESS_LINE2||T2.ADDRESS_LINE3||T2.ADDRESS_LINE4||T2.ADDRESS_LINE5) LIKE :AddressEng)) ))";
                    model.QueryParameters.Add("AddressEng", "%" + model.Address.Trim().ToUpper() + "%");


                }
            }

            if (!string.IsNullOrWhiteSpace(model.CatCode))
            {
                whereQ += "\r\n\t" + "AND indCert.CATEGORY_ID = :CatCode";
                model.QueryParameters.Add("CatCode", model.CatCode);

            }

            if (!string.IsNullOrWhiteSpace(model.ServicesInBuidingSafety))
            {
                whereQ += "\r\n\t" + "AND (  indApp.uuid IN( SELECT bsi.master_Id  FROM c_Building_Safety_Info bsi  inner join C_S_system_value sv on bsi.BUILDING_SAFETY_ID =sv.uuid   WHERE  sv.uuid = :ServicesInBuidingSafety))";
                model.QueryParameters.Add("ServicesInBuidingSafety", model.ServicesInBuidingSafety);

            }

            if (model.SelectedPRBList != null)
            {


                whereQ += "\r\n\t" + "AND (select  sv.uuid from c_s_system_value sv where  sv.uuid= iq.PRB_ID) IN (:PRB)";
                model.QueryParameters.Add("PRB", model.SelectedPRBList);


            }
            if (!string.IsNullOrWhiteSpace(model.PNAP))
            {
                whereQ += "\r\n\t" + "AND (select sv.uuid from c_s_system_value sv where sv.uuid =indApp.PRACTICE_NOTES_ID ) = :PNAP";
                model.QueryParameters.Add("PNAP", model.PNAP);

            }

            if (model.SelectedQC != null)
            {

                whereQ += "\r\n\t" + "AND (select  catCode.uuid from c_s_category_code catCode where  catCode.uuid= indCert.CATEGORY_ID) IN (:QC)";
                model.QueryParameters.Add("QC", model.SelectedQC);
            }
            if (!string.IsNullOrWhiteSpace(model.Sex))
            {
                whereQ += "\r\n\t" + "AND app.Gender = :Sex";
                model.QueryParameters.Add("Sex", model.Sex);

            }
            if (model.SelectedQualificationDetails != null)
            {
                whereQ += "\r\n\t" + "AND (select  catCodeDetail.uuid from c_s_category_code_detail catCodeDetail where  catCodeDetail.uuid= iqd.S_CATEGORY_CODE_DETAIL_ID) IN (:DisciplinesDiv)";
                model.QueryParameters.Add("DisciplinesDiv", model.SelectedQualificationDetails);

            }
            if (!string.IsNullOrWhiteSpace(model.TypeOfDate)
                          && RegistrationConstant.DATE_TYPE_MAP.ContainsKey(model.TypeOfDate)
                          && (model.FromDate != null || model.ToDate != null)
                          )
            {
                if (model.FromDate != null)
                {
                    whereQ += "\r\n\t" + "AND indCert." + RegistrationConstant.DATE_TYPE_MAP[model.TypeOfDate] + " >= :DateFrom";
                    model.QueryParameters.Add("DateFrom", model.FromDate);
                }
                if (model.ToDate != null)
                {
                    whereQ += "\r\n\t" + "AND indCert." + RegistrationConstant.DATE_TYPE_MAP[model.TypeOfDate] + " <= :DateTo";
                    model.QueryParameters.Add("DateTo", model.ToDate);
                }
            }
            return whereQ;
        }

        private string SearchMWIA_whereQ(Fn01Search_MWIASearchModel model)
        {
            string whereQ = "";
            if (!string.IsNullOrWhiteSpace(model.FileRef))
            {
                whereQ += "\r\n\t" + "AND upper(indApp.FILE_REFERENCE_NO) LIKE :FileRef";
                model.QueryParameters.Add("FileRef", "%" + model.FileRef.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.TelNo))
            {
                whereQ += "\r\n\t" + "AND ( (replace(indApp.TELEPHONE_NO1,' ','') LIKE :TelNo) or (replace(indApp.TELEPHONE_NO2,' ','') LIKE :TelNo) or (replace(indApp.TELEPHONE_NO3,' ','' ) LIKE :TelNo) ) ";
                model.QueryParameters.Add("TelNo", "%" + model.TelNo.Trim().Replace(" ", "") + "%");


            }
            if (!string.IsNullOrWhiteSpace(model.Email))
            {
                whereQ += "\r\n\t" + "AND UPPER(indApp.EMAIL) like :Email";
                model.QueryParameters.Add("Email", "%" + model.Email.Trim().ToUpper() + "%");


            }
            if (!string.IsNullOrWhiteSpace(model.RegNo))
            {
                whereQ += "\r\n\t" + "AND upper(indCert.CERTIFICATION_NO) LIKE :RegNo";
                model.QueryParameters.Add("RegNo", "%" + model.RegNo.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.HKID))
            {
                whereQ += "\r\n\t" + "AND " + EncryptDecryptUtil.getDecryptSQL("app.hkid") + " LIKE :HKID";
                model.QueryParameters.Add("HKID", "%" + model.HKID.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.PassportNo))
            {
                whereQ += "\r\n\t" + "AND " + EncryptDecryptUtil.getDecryptSQL("app.passport_no") + " LIKE :PassportNo";
                model.QueryParameters.Add("PassportNo", "%" + model.PassportNo.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.SurnName))
            {
                whereQ += "\r\n\t" + "AND app.SURNAME LIKE :SurnName";
                model.QueryParameters.Add("SurnName", "%" + model.SurnName.Trim().ToUpper() + "%");

            }
            if (!string.IsNullOrWhiteSpace(model.GivenName))
            {
                whereQ += "\r\n\t" + "AND upper(app.GIVEN_NAME_ON_ID) LIKE :GivenName";
                model.QueryParameters.Add("GivenName", (model.KeywordSearch ? "%" : "") + model.GivenName.Trim().ToUpper() + "%");

            }

            if (!string.IsNullOrWhiteSpace(model.ChiName))
            {
                whereQ += "\r\n\t" + "AND app.CHINESE_NAME LIKE :ChiName";
                model.QueryParameters.Add("ChiName", "%" + model.ChiName.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.ServicesInBuidingSafety))
            {
                whereQ += "\r\n\t" + "AND (  indApp.uuid IN( SELECT bsi.master_Id  FROM c_Building_Safety_Info bsi  inner join C_S_system_value sv on bsi.BUILDING_SAFETY_ID =sv.uuid   WHERE  sv.uuid = :ServicesInBuidingSafety))";
                model.QueryParameters.Add("ServicesInBuidingSafety", model.ServicesInBuidingSafety);

            }
            if (!string.IsNullOrWhiteSpace(model.PNAP))
            {
                whereQ += "\r\n\t" + "AND (select sv.uuid from c_s_system_value sv where sv.uuid =indApp.PRACTICE_NOTES_ID ) = :PNAP";
                model.QueryParameters.Add("PNAP", model.PNAP);

            }
            if (!string.IsNullOrWhiteSpace(model.TypeOfDate)
                         && RegistrationConstant.DATE_TYPE_MAP.ContainsKey(model.TypeOfDate)
                         && (model.FromDate != null || model.ToDate != null)
                         )
            {
                if (model.FromDate != null)
                {
                    whereQ += "\r\n\t" + "AND indCert." + RegistrationConstant.DATE_TYPE_MAP[model.TypeOfDate] + " >= :DateFrom";
                    model.QueryParameters.Add("DateFrom", model.FromDate);
                }
                if (model.ToDate != null)
                {
                    whereQ += "\r\n\t" + "AND indCert." + RegistrationConstant.DATE_TYPE_MAP[model.TypeOfDate] + " <= :DateTo";
                    model.QueryParameters.Add("DateTo", model.ToDate);
                }
            }
            if (!string.IsNullOrWhiteSpace(model.Sex))
            {
                whereQ += "\r\n\t" + "AND app.Gender = :Sex";
                model.QueryParameters.Add("Sex", model.Sex);

            }
            if (!string.IsNullOrWhiteSpace(model.MWCap))
            {
                whereQ += "\r\n\t" + "AND ( indApp.uuid IN(SELECT (select indApp.uuid from C_IND_APPLICATION indApp where mwi.MASTER_ID = indApp.uuid)   FROM  C_Ind_Application_Mw_Item mwi inner join C_S_System_Value sysVal on mwi.ITEM_DETAILS_ID = sysVal.uuid WHERE    sysVal.code = :MWCap)  )";
                model.QueryParameters.Add("MWCap", "Item " + model.MWCap);

            }
            return whereQ;
        }

        public void SearchMWIA(Fn01Search_MWIASearchModel model)
        {
            model.Query = SearchMWIA_q;
            model.QueryWhere = SearchMWIA_whereQ(model);
            model.Search();
        }

        public string ExportPA(Fn01Search_PASearchModel model)
        {
            model.Query = SearchPA_q;
            model.QueryWhere = SearchPA_whereQ(model);
            //DisplayGrid dlr = new DisplayGrid() { Query = SearchGCA_q, Columns = Columns, Parameters = post };
            return model.Export("ExportData");
        }
        public string ExportDFR(Fn01Search_DFRSearchModel model)
        {
            String conditionTag = "<CONDITION>";
            String conditionValueIndApp = "and 2=2 ";
            String conditionValueCompApp = "and 2=2 ";
            //conditionValueIndApp += "and indApp.REGISTRATION_TYPE= 'IP' ";
            //conditionValueIndApp += "and (SYSDATE - indProMon.DEFER_DATE)/30 >= 1 ";

            //conditionValueCompApp += "and compApp.REGISTRATION_TYPE= 'CMW' ";
            //conditionValueCompApp += "and (SYSDATE - compProMon.DEFER_DATE)/30 >= 1 ";

            String SearchDFR_indApp_q = this.SearchDFR_indApp_q;
            String SearchDFR_compApp_q = this.SearchDFR_compApp_q;

            // validation
            /*
            if (String.IsNullOrEmpty(model.DeferralPeriodSymbol) || String.IsNullOrEmpty(model.DeferralValue))
            {
                return;
            }
            */

            // add parameter
            if (model != null)
            {
                if (!String.IsNullOrEmpty(model.FileRef))
                {
                    String fileRef = model.FileRef.ToUpper().Trim();
                    conditionValueIndApp += "and indApp.FILE_REFERENCE_NO like '%" + fileRef + "%' ";
                    conditionValueCompApp += "and compApp.FILE_REFERENCE_NO like '%" + fileRef + "%' ";
                }
                if (!String.IsNullOrEmpty(model.CompName))
                {
                    String compName = model.CompName.ToUpper().Trim();
                    conditionValueIndApp += "and 1=2 ";
                    conditionValueCompApp += "and compApp.ENGLISH_COMPANY_NAME like '%" + compName + "%' ";
                }
                if (!String.IsNullOrEmpty(model.SurnName))
                {
                    String surnName = model.SurnName.ToUpper().Trim();
                    conditionValueIndApp += "and app.SURNAME like '%" + surnName + "%' ";
                    conditionValueCompApp += "and app.SURNAME like '%" + surnName + "%' ";
                }
                if (!String.IsNullOrEmpty(model.GivenName))
                {
                    String givenName = model.GivenName.ToUpper().Trim();
                    conditionValueIndApp += "and upper(app.GIVEN_NAME_ON_ID) like '%" + givenName + "%' ";
                    conditionValueCompApp += "and upper(app.GIVEN_NAME_ON_ID) like '%" + givenName + "%' ";
                }
                if (!String.IsNullOrEmpty(model.ChineseName))
                {
                    String chineseName = model.ChineseName.ToUpper().Trim();
                    conditionValueIndApp += "and app.CHINESE_NAME like '%" + chineseName + "%' ";
                    conditionValueCompApp += "and app.CHINESE_NAME like '%" + chineseName + "%' ";
                }
                if (!String.IsNullOrEmpty(model.DeferralPeriodSymbol) && !String.IsNullOrEmpty(model.DeferralValue))
                {
                    String deferralPeriodSymbol = model.DeferralPeriodSymbol.Trim();
                    String deferralValue = model.DeferralValue.Trim();

                    //conditionValueIndApp += "and (SYSDATE - indProMon.DEFER_DATE)/30 >= 1 ";
                    //conditionValueCompApp += "and (SYSDATE - compProMon.DEFER_DATE)/30 >= 1 ";
                    conditionValueIndApp += "and (SYSDATE - indProMon.DEFER_DATE)/30 " + deferralPeriodSymbol + deferralValue;
                    conditionValueCompApp += "and (SYSDATE - compProMon.DEFER_DATE)/30" + deferralPeriodSymbol + deferralValue;
                }

                String regTypeIP = "'" + RegistrationConstant.REGISTRATION_TYPE_IP + "'";
                String regTypeMWCI = "'" + RegistrationConstant.REGISTRATION_TYPE_MWIA + "'";
                String regTypeGBC = "'" + RegistrationConstant.REGISTRATION_TYPE_CGA + "'";
                String regTypeMWC = "'" + RegistrationConstant.REGISTRATION_TYPE_MWCA + "'";

                String indAppTypeStr = "";
                String compAppTypeStr = "";
                if (!String.IsNullOrEmpty(model.ChkProf) && "ON".Equals(model.ChkProf.ToUpper()))
                {
                    indAppTypeStr += String.IsNullOrEmpty(indAppTypeStr) ? regTypeIP : ", " + regTypeIP;
                }
                if (!String.IsNullOrEmpty(model.ChkMWCI) && "ON".Equals(model.ChkMWCI.ToUpper()))
                {
                    indAppTypeStr += String.IsNullOrEmpty(indAppTypeStr) ? regTypeMWCI : ", " + regTypeMWCI;
                }
                if (!String.IsNullOrEmpty(model.ChkGBC) && "ON".Equals(model.ChkGBC.ToUpper()))
                {
                    compAppTypeStr += String.IsNullOrEmpty(compAppTypeStr) ? regTypeGBC : ", " + regTypeGBC;
                }
                if (!String.IsNullOrEmpty(model.ChkMWC) && "ON".Equals(model.ChkMWC.ToUpper()))
                {
                    compAppTypeStr += String.IsNullOrEmpty(compAppTypeStr) ? regTypeMWC : ", " + regTypeMWC;
                }

                // append criteric
                if (!String.IsNullOrEmpty(indAppTypeStr))
                {
                    conditionValueIndApp += "and indApp.REGISTRATION_TYPE in (" + indAppTypeStr + ") ";
                }
                else
                {
                    conditionValueIndApp += "and 2=3 ";
                }

                if (!String.IsNullOrEmpty(compAppTypeStr))
                {
                    conditionValueCompApp += "and compApp.REGISTRATION_TYPE in (" + compAppTypeStr + ") ";
                }
                else
                {
                    conditionValueCompApp += "and 2=3 ";
                }
            }

            // replace condition to sql
            SearchDFR_indApp_q = SearchDFR_indApp_q.Replace(conditionTag, conditionValueIndApp);
            SearchDFR_compApp_q = SearchDFR_compApp_q.Replace(conditionTag, conditionValueCompApp);

            // sql by default
            String combinedSql = ""
                + "\r\n" + "\t" + "select combinedDeferralRecord.* from ( "
                + "\r\n" + "\t" + SearchDFR_indApp_q
                + "\r\n" + "\t" + "union"
                + "\r\n" + "\t" + SearchDFR_compApp_q
                + "\r\n" + "\t" + ") combinedDeferralRecord "
                + "\r\n" + "\t" + "where 1=1 ";


            model.Query = combinedSql;

            return model.Export("ExportData");
        }
        public string ExportCNV(Fn01Search_CNVSearchModel model)
        {
            model.Query = SearchCNV_q;
            model.QueryWhere = SearchCNV_whereQ(model);
            //DisplayGrid dlr = new DisplayGrid() { Query = SearchGCA_q, Columns = Columns, Parameters = post };
            return model.Export("ExportData");
        }
        public Fn01Search_QPDisplayModel ViewQP(string id)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                C_COMP_APPLICATION application = db.C_COMP_APPLICATION.Where(o => o.UUID == id).FirstOrDefault();

                List<C_S_SYSTEM_VALUE> bs = (from t1 in db.C_S_SYSTEM_VALUE
                                             join t2 in db.C_BUILDING_SAFETY_INFO on t1 equals t2.C_S_SYSTEM_VALUE
                                             where t2.MASTER_ID == id
                                             select t1).ToList();

                List<ApplicantDisplayListModel> aplts =
                    ApplicantDisplayListModel.Load((new RegistrationCompAppService()).GetApplicantsByCompUuid(id, null, true));


                //select t1).ToList();
                C_ADDRESS chinessAddess = (from t1 in db.C_ADDRESS
                                           join t2 in db.C_COMP_APPLICATION on t1.UUID equals t2.CHINESE_ADDRESS_ID
                                           where t2.UUID == id
                                           select t1).FirstOrDefault();
                C_ADDRESS englishAddress = (from t1 in db.C_ADDRESS
                                            join t2 in db.C_COMP_APPLICATION on t1.UUID equals t2.ENGLISH_ADDRESS_ID
                                            where t2.UUID == id
                                            select t1).FirstOrDefault();


                return null;/* new Fn01Search_QPDisplayModel()
                {
                    C_COMP_APPLICATION = application
                    ,
                    C_S_SYSTEM_VALUEs_BS = bs
                    ,
                    C_ADDRESS_Chinese = chinessAddess
                    ,
                    C_ADDRESS_English = englishAddress
                    ,
                    C_APPLICANTs = aplts
                };*/
            }
        }











        public Fn01Search_RIADisplayModel ViewRIA(string id)
        {
            //return null;

            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                C_IND_CERTIFICATE cert = db.C_IND_CERTIFICATE.Where(o => o.UUID == id).Include(x=>x.C_S_CATEGORY_CODE).FirstOrDefault();
                C_IND_APPLICATION application = (from t1 in db.C_IND_CERTIFICATE
                                                 join t2 in db.C_IND_APPLICATION on t1.C_IND_APPLICATION equals t2
                                                 where t1.UUID == id
                                                 select t2).FirstOrDefault();

                List<C_S_SYSTEM_VALUE> bs = (from t1 in db.C_S_SYSTEM_VALUE
                                             join t2 in db.C_BUILDING_SAFETY_INFO on t1 equals t2.C_S_SYSTEM_VALUE
                                             where t2.MASTER_ID == application.UUID
                                             select t1).ToList();


                ApplicantDisplayListModel aplt = null;
                if (application.C_APPLICANT != null) aplt = new ApplicantDisplayListModel()
                {
                    HKID = EncryptDecryptUtil.getDecryptHKID(application.C_APPLICANT.HKID)           ,
                    PASSPORT = application.C_APPLICANT.PASSPORT_NO
                                                                                                     ,
                    TITLE = application.C_APPLICANT.C_S_SYSTEM_VALUE.ENGLISH_DESCRIPTION
                                                                                                      ,
                    SURNAME = application.C_APPLICANT.SURNAME
                                                                                                     ,
                    GIVEN_NAME_ON_ID = application.C_APPLICANT.GIVEN_NAME_ON_ID
                                                                                                      ,
                    GENDER = application.C_APPLICANT.GENDER

                };
                C_ADDRESS chinessAddess = null;
                C_ADDRESS englishAddress = null;

                //select t1).ToList();
                if (application.CHINESE_OFFICE_ADDRESS_ID != null)
                {
                    chinessAddess = db.C_ADDRESS.Where(o => o.UUID == application.CHINESE_OFFICE_ADDRESS_ID).FirstOrDefault();
                }
                else if (application.CHINESE_HOME_ADDRESS_ID != null)
                {
                    chinessAddess = db.C_ADDRESS.Where(o => o.UUID == application.CHINESE_HOME_ADDRESS_ID).FirstOrDefault();
                }


                if (application.ENGLISH_OFFICE_ADDRESS_ID != null)
                {
                    englishAddress = db.C_ADDRESS.Where(o => o.UUID == application.ENGLISH_OFFICE_ADDRESS_ID).FirstOrDefault();
                }
                else if (application.ENGLISH_HOME_ADDRESS_ID != null)
                {
                    englishAddress = db.C_ADDRESS.Where(o => o.UUID == application.ENGLISH_HOME_ADDRESS_ID).FirstOrDefault();
                }

                C_S_SYSTEM_VALUE appStatus = cert.C_S_SYSTEM_VALUE2;
                C_S_CATEGORY_CODE catCode = cert.C_S_CATEGORY_CODE;
                List<C_IND_QUALIFICATION> qualLists = db.C_IND_QUALIFICATION.Where(x => x.C_IND_APPLICATION.UUID == application.UUID)
                                                        .Include(x => x.C_S_SYSTEM_VALUE)
                                                        .Include(x => x.C_S_SYSTEM_VALUE1)
                                                        .ToList();
//List <C_IND_QUALIFICATION> qualLists =
//                    (from t1 in db.C_IND_QUALIFICATION
//                     join t2 in db.C_S_SYSTEM_VALUE on t1.C_S_SYSTEM_VALUE1 equals t2
//                     join sv in db.C_S_SYSTEM_VALUE on t1.PRB_ID equals sv.UUID
//                     where t1.C_IND_APPLICATION.UUID == application.UUID
//                     select t1).ToList();
                //application.C_IND_QUALIFICATION.ToList();

                return new Fn01Search_RIADisplayModel()
                {
                    C_IND_APPLICATION = application,
                    C_S_SYSTEM_VALUEs_BS = bs,
                    C_ADDRESS_Chinese = chinessAddess,
                    C_ADDRESS_English = englishAddress,
                    C_IND_CERTIFICATE = cert,
                    C_APPLICANT = aplt,
                    C_S_SYSTEM_VALUE_AppStatus = appStatus,
                    C_S_CATEGORY_CODE = catCode,
                    C_IND_QUALIFICATIONs = qualLists
                };
            }

        }
        /*
        public Fn01Search_MWCADisplayModel ViewMWCA(string id)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                C_COMP_APPLICATION application = db.C_COMP_APPLICATION.Where(o => o.UUID == id).FirstOrDefault();

                List<C_S_SYSTEM_VALUE> bs = (from t1 in db.C_S_SYSTEM_VALUE
                                             join t2 in db.C_BUILDING_SAFETY_INFO on t1 equals t2.C_S_SYSTEM_VALUE
                                             where t2.MASTER_ID == id
                                             select t1).ToList();

                List<ApplicantDisplayListModel> aplts = (new RegistrationCommonService()).GetApplicantsByCompUuid(id,null, true);
               
                C_ADDRESS chinessAddess = (from t1 in db.C_ADDRESS
                                           join t2 in db.C_COMP_APPLICATION on t1.UUID equals t2.CHINESE_ADDRESS_ID
                                           where t2.UUID == id
                                           select t1).FirstOrDefault();
                C_ADDRESS englishAddress = (from t1 in db.C_ADDRESS
                                            join t2 in db.C_COMP_APPLICATION on t1.UUID equals t2.ENGLISH_ADDRESS_ID
                                            where t2.UUID == id
                                            select t1).FirstOrDefault();

                List<object> class1 = null;
                List<object> class2 = null;
                List<object> class3 = null;
                using (DbConnection conn = db.Database.Connection)
                {//Class 1
                    string q = ""
                        + "\r\n" + "\t" + "SELECT T4.CODE FROM C_COMP_APPLICANT_MW_ITEM T1                                                     "
                        + "\r\n" + "\t" + "INNER JOIN C_COMP_APPLICANT_INFO T2 ON T2.UUID = T1.COMPANY_APPLICANTS_ID                           "
                        + "\r\n" + "\t" + "INNER JOIN C_COMP_APPLICATION T3 ON T3.UUID = T2.MASTER_ID                                          "
                        + "\r\n" + "\t" + "INNER JOIN C_S_SYSTEM_VALUE T4 ON T4.UUID = T1.ITEM_TYPE_ID                                         "
                        + "\r\n" + "\t" + "WHERE EXISTS (SELECT 1 FROM C_S_SYSTEM_VALUE WHERE CODE LIKE 'A%' AND UUID = T2.APPLICANT_ROLE_ID)  "
                        + "\r\n" + "\t" + "AND EXISTS (SELECT 1 FROM C_S_SYSTEM_VALUE WHERE CODE ='1' AND UUID = T2.APPLICANT_STATUS_ID)       "
                        + "\r\n" + "\t" + "AND EXISTS (SELECT 1 FROM C_S_SYSTEM_VALUE WHERE CODE = :classNo AND UUID = T1.ITEM_CLASS_ID)      "
                        + "\r\n" + "\t" + "AND T3.UUID = :uuid ORDER BY 1                                                    ";
                    Dictionary<string, object> class1Paras = new Dictionary<string, object>(); class1Paras.Add("classNo", "Class 1"); class1Paras.Add("uuid", id);
                    Dictionary<string, object> class2Paras = new Dictionary<string, object>(); class2Paras.Add("classNo", "Class 2"); class2Paras.Add("uuid", id);
                    Dictionary<string, object> class3Paras = new Dictionary<string, object>(); class3Paras.Add("classNo", "Class 3"); class3Paras.Add("uuid", id);
                    List<Dictionary<string, object>> class1List = CommonUtil.loadDbData(CommonUtil.GetDataReader(conn, q, class1Paras));
                    List<Dictionary<string, object>> class2List = CommonUtil.loadDbData(CommonUtil.GetDataReader(conn, q, class2Paras));
                    List<Dictionary<string, object>> class3List = CommonUtil.loadDbData(CommonUtil.GetDataReader(conn, q, class3Paras));
                    class1 = class1List.Select(o => o["CODE"]).ToList();
                    class2 = class2List.Select(o => o["CODE"]).ToList().Except(class1).ToList();
                    class3 = class3List.Select(o => o["CODE"]).ToList().Except(class2).ToList().Except(class1).ToList();
                    Console.WriteLine("fdsfsf");
                }

                return new Fn01Search_MWCADisplayModel()
                {
                    C_COMP_APPLICATION = application,
                    C_S_SYSTEM_VALUEs_BS = bs,
                    C_ADDRESS_Chinese = chinessAddess,
                    C_ADDRESS_English = englishAddress,
                    C_APPLICANTs = aplts,
                    Class1 = class1,
                    Class2 = class2,
                    Class3 = class3
                };
            }
        }
        */
        public Fn01Search_CNVDisplayModel ViewCompCNV(string id)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                var query = from CompCNV in db.C_COMP_CONVICTION
                            where CompCNV.UUID == id
                            select CompCNV;
                return new Fn01Search_CNVDisplayModel()
                { C_COMP_CONVICTION = query.FirstOrDefault() };
            }

        }
        public Fn01Search_CNVDisplayModel ViewIndCNV(string id)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                var IndQuery = from IndCNV in db.C_IND_CONVICTION
                               where IndCNV.UUID == id
                               select IndCNV;
                return new Fn01Search_CNVDisplayModel()
                { C_IND_CONVICTION = IndQuery.FirstOrDefault() };
            }

        }

        // Andy: view details of PA Application
        public Fn01Search_PADisplayModel ViewPA(string id)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                string ServiceInMWIS = "";
                if (string.IsNullOrWhiteSpace(id))
                {


                    BuildingSafetyCheckListModel newbsModel = new BuildingSafetyCheckListModel()
                    { MasterUuid = id, RegType = RegistrationConstant.REGISTRATION_TYPE_IP };
                    newbsModel.init();

                    return new Fn01Search_PADisplayModel()
                    {
                        BsModel = newbsModel
                    }; 
                }
              

                //var queryApp = from IndApp in db.C_IND_APPLICATION
                //               join ChinAdd in db.C_ADDRESS on IndApp.CHINESE_HOME_ADDRESS_ID equals ChinAdd.UUID
                //               join EngAdd in db.C_ADDRESS on IndApp.ENGLISH_HOME_ADDRESS_ID equals EngAdd.UUID
                //               join OfficeChinAdd in db.C_ADDRESS on IndApp.CHINESE_OFFICE_ADDRESS_ID equals OfficeChinAdd.UUID
                //               join OfficeEngAdd in db.C_ADDRESS on IndApp.ENGLISH_OFFICE_ADDRESS_ID equals OfficeEngAdd.UUID
                //               join BSChinAdd in db.C_ADDRESS on IndApp.CHINESE_OFFICE_ADDRESS_ID equals BSChinAdd.UUID
                //               join BSEngAdd in db.C_ADDRESS on IndApp.ENGLISH_BS_ADDRESS_ID equals BSEngAdd.UUID
                //               join Quali in db.C_IND_QUALIFICATION on IndApp.UUID equals Quali.MASTER_ID 
                //               join Applicant in db.C_APPLICANT on IndApp.APPLICANT_ID equals Applicant.UUID
                //               join Certi in db.C_IND_CERTIFICATE on IndApp.UUID equals Certi.MASTER_ID
                //               join cat in db.C_S_CATEGORY_CODE on Certi.CATEGORY_ID equals cat.UUID
                //               join sv in db.C_S_SYSTEM_VALUE on cat.CATEGORY_GROUP_ID equals sv.UUID
                //               join sv_app_status in db.C_S_SYSTEM_VALUE on Certi.APPLICATION_STATUS_ID equals sv_app_status.UUID
                //               where IndApp.UUID == id
                //               select new { IndApp, ChinAdd, EngAdd, Applicant, Certi, sv, sv_app_status, OfficeChinAdd, OfficeEngAdd, BSChinAdd, BSEngAdd, Quali };

                var queryApp = from IndApp in db.C_IND_APPLICATION
                               join ChinAdd in db.C_ADDRESS on IndApp.CHINESE_HOME_ADDRESS_ID equals ChinAdd.UUID
                               join EngAdd in db.C_ADDRESS on IndApp.ENGLISH_HOME_ADDRESS_ID equals EngAdd.UUID
                               join OfficeChinAdd in db.C_ADDRESS on IndApp.CHINESE_OFFICE_ADDRESS_ID equals OfficeChinAdd.UUID
                               join OfficeEngAdd in db.C_ADDRESS on IndApp.ENGLISH_OFFICE_ADDRESS_ID equals OfficeEngAdd.UUID
                               join BSChinAdd in db.C_ADDRESS on IndApp.CHINESE_BS_ADDRESS_ID equals BSChinAdd.UUID
                               join BSEngAdd in db.C_ADDRESS on IndApp.ENGLISH_BS_ADDRESS_ID equals BSEngAdd.UUID
                               join Quali in db.C_IND_QUALIFICATION on IndApp.UUID equals Quali.MASTER_ID into gj

                               from x in gj.DefaultIfEmpty()
                               join Applicant in db.C_APPLICANT on IndApp.APPLICANT_ID equals Applicant.UUID

                               join Certi in db.C_IND_CERTIFICATE on IndApp.UUID equals Certi.MASTER_ID into gj2
                               from x2 in gj2.DefaultIfEmpty()


                               join cat in db.C_S_CATEGORY_CODE on  x2.CATEGORY_ID equals cat.UUID into gj3
                               from x3 in gj3.DefaultIfEmpty()

                               join sv in db.C_S_SYSTEM_VALUE on x3.CATEGORY_GROUP_ID equals sv.UUID into gj4
                               from x4 in gj4.DefaultIfEmpty()

                               join sv_app_status in db.C_S_SYSTEM_VALUE on x2.APPLICATION_STATUS_ID equals sv_app_status.UUID into gj5
                               from x5 in gj5.DefaultIfEmpty()
                                where IndApp.UUID == id
                               select new { IndApp, ChinAdd, EngAdd, Applicant,  OfficeChinAdd, OfficeEngAdd, BSChinAdd, BSEngAdd,

                                   Quali = (x == null ? null : x),
                                   Certi = (x2 == null ? null : x2),
                                   sv    = (x4 == null ? null : x4),
                                   sv_app_status = (x5 == null ? null : x5),
                               };





                
                var BuildSafetyInfo = db.C_BUILDING_SAFETY_INFO.Include("C_S_SYSTEM_VALUE").Where(o => o.MASTER_ID == queryApp.FirstOrDefault().IndApp.UUID).ToList();


                var PRBList = (from Quali in db.C_IND_QUALIFICATION
                               join cat in db.C_S_CATEGORY_CODE on Quali.CATEGORY_ID equals cat.UUID
                               join sv in db.C_S_SYSTEM_VALUE on cat.CATEGORY_GROUP_ID equals sv.UUID
                               join svprbDescrption in db.C_S_SYSTEM_VALUE on Quali.PRB_ID equals svprbDescrption.UUID
                               where Quali.MASTER_ID == queryApp.FirstOrDefault().IndApp.UUID
                               select new Fn01Search_PADisplayModel.PRBLIST { PRB_c_IND_QUALIFICATION = Quali, PRB_c_S_CATEGORY_CODE = cat, PRB_c_S_SYSTEM_VALUE = sv , PRB_Des_c_S_SYSTEM_VALUE = svprbDescrption }).ToList();

                // var CertiList = db.C_IND_CERTIFICATE.Where(x => x.MASTER_ID == id).OrderBy(x => x.CERTIFICATION_NO).ToList();
                var CertiList = (from indcert in db.C_IND_CERTIFICATE
                                 join svperiod in db.C_S_SYSTEM_VALUE on indcert.PERIOD_OF_VALIDITY_ID equals svperiod.UUID
                                 join svappform in db.C_S_SYSTEM_VALUE on indcert.APPLICATION_FORM_ID equals svappform.UUID
                                 join catcode in db.C_S_CATEGORY_CODE on indcert.CATEGORY_ID equals catcode.UUID
                                 where indcert.MASTER_ID == id
                                 select new Fn01Search_PADisplayModel.CERTLIST { CERT_c_IND_CERTIFICATE = indcert, CERT_APPFORM_c_S_SYSTEM_VALUE = svappform, CERT_PERIODVAD_c_S_SYSTEM_VALUE = svperiod , CERT_c_S_CATEGORY_CODE =catcode }).ToList();
                BuildingSafetyCheckListModel bsModel = new BuildingSafetyCheckListModel()
                { MasterUuid = id, RegType = RegistrationConstant.REGISTRATION_TYPE_IP };
                bsModel.init();
                if (queryApp.Count() == 0 ||queryApp.FirstOrDefault().IndApp.SERVICE_IN_MWIS == null)
                {
                    ServiceInMWIS = "-";
                }
                else
                {
                    ServiceInMWIS = queryApp.FirstOrDefault().IndApp.SERVICE_IN_MWIS;
                }


                return new Fn01Search_PADisplayModel()
                {
                    C_IND_APPLICATION = queryApp.FirstOrDefault().IndApp,
                    C_APPLICANT = queryApp.FirstOrDefault().Applicant,
                    HOME_ADDRESS_ENG = queryApp.FirstOrDefault().EngAdd,
                    HOME_ADDRESS_CHI = queryApp.FirstOrDefault().ChinAdd,
                    OFFICE_ADDRESS_ENG = queryApp.FirstOrDefault().OfficeEngAdd,
                    OFFICE_ADDRESS_CHI = queryApp.FirstOrDefault().OfficeChinAdd,
                    BS_ADDRESS_ENG = queryApp.FirstOrDefault().BSEngAdd,
                    BS_ADDRESS_CHI = queryApp.FirstOrDefault().BSChinAdd,

                    C_IND_CERTIFICATE = queryApp.FirstOrDefault().Certi,
                    C_BUILDING_SAFETY_INFOs = BuildSafetyInfo,
                    SV_CATEGORY_C_S_SYSTEM_VALUE = queryApp.FirstOrDefault().sv,
                    APP_STATUS_C_S_SYSTEM_VALUE = queryApp.FirstOrDefault().sv_app_status,
                    C_IND_QUALIFICATION = queryApp.FirstOrDefault().Quali,
                    PRBLISTs = PRBList,
                    CERTLISTs = CertiList,
                    EditFormKey = queryApp == null ? -1 : queryApp.FirstOrDefault().Applicant.MODIFIED_DATE.Ticks
                    ,
                    BsModel = bsModel
                    ,
                    ServiceInMWIS = ServiceInMWIS
            };

            }
        }

        public Fn01Search_PADisplayModel GetApplicantById(Fn01Search_PADisplayModel model,string hkid,string passportno)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                string tempHKID = EncryptDecryptUtil.getEncrypt(hkid);
                string tempPASSPORTNO = EncryptDecryptUtil.getEncrypt(passportno);
                var queryApp = from IndApp in db.C_IND_APPLICATION
                               join ChinAdd in db.C_ADDRESS on IndApp.CHINESE_HOME_ADDRESS_ID equals ChinAdd.UUID
                               join EngAdd in db.C_ADDRESS on IndApp.ENGLISH_HOME_ADDRESS_ID equals EngAdd.UUID
                               join OfficeChinAdd in db.C_ADDRESS on IndApp.ENGLISH_OFFICE_ADDRESS_ID equals OfficeChinAdd.UUID
                               join OfficeEngAdd in db.C_ADDRESS on IndApp.ENGLISH_OFFICE_ADDRESS_ID equals OfficeEngAdd.UUID
                               join Applicant in db.C_APPLICANT on IndApp.APPLICANT_ID equals Applicant.UUID
                               where Applicant.HKID == tempHKID || Applicant.PASSPORT_NO== tempPASSPORTNO
                               select new { IndApp, ChinAdd, EngAdd, Applicant,OfficeChinAdd, OfficeEngAdd, };
                if (queryApp.Any())
                {
                    model.C_APPLICANT = queryApp.FirstOrDefault().Applicant;

                    model.HOME_ADDRESS_CHI = queryApp.FirstOrDefault().ChinAdd;
                    model.HOME_ADDRESS_ENG = queryApp.FirstOrDefault().EngAdd;
                    model.OFFICE_ADDRESS_CHI = queryApp.FirstOrDefault().OfficeChinAdd;
                    model.OFFICE_ADDRESS_ENG = queryApp.FirstOrDefault().OfficeEngAdd;


                    model.C_IND_APPLICATION.FAX_NO1 = queryApp.FirstOrDefault().IndApp.FAX_NO1;
                    model.C_IND_APPLICATION.FAX_NO2 = queryApp.FirstOrDefault().IndApp.FAX_NO2;
                    model.C_IND_APPLICATION.TELEPHONE_NO1 = queryApp.FirstOrDefault().IndApp.TELEPHONE_NO1;
                    model.C_IND_APPLICATION.TELEPHONE_NO2 = queryApp.FirstOrDefault().IndApp.TELEPHONE_NO2;
                    model.C_IND_APPLICATION.TELEPHONE_NO3 = queryApp.FirstOrDefault().IndApp.TELEPHONE_NO3;
                    model.C_IND_APPLICATION.EMERGENCY_NO1 = queryApp.FirstOrDefault().IndApp.EMERGENCY_NO1;
                    model.C_IND_APPLICATION.EMERGENCY_NO2 = queryApp.FirstOrDefault().IndApp.EMERGENCY_NO2;
                    model.C_IND_APPLICATION.EMERGENCY_NO3 = queryApp.FirstOrDefault().IndApp.EMERGENCY_NO3;
                    model.C_IND_APPLICATION.ENGLISH_CARE_OF = queryApp.FirstOrDefault().IndApp.ENGLISH_CARE_OF;
                    model.C_IND_APPLICATION.CHINESE_CARE_OF = queryApp.FirstOrDefault().IndApp.CHINESE_CARE_OF;
                    model.C_IND_APPLICATION.CORRESPONDENCE_LANG_ID = queryApp.FirstOrDefault().IndApp.CORRESPONDENCE_LANG_ID;
                    model.C_IND_APPLICATION.POST_TO = queryApp.FirstOrDefault().IndApp.POST_TO;
                    model.C_IND_APPLICATION.EMAIL = queryApp.FirstOrDefault().IndApp.EMAIL;
                    model.result = true;
                }
                else
                {
                    model.result = false;
                    model.ErrMsg = "HKID or Passport No. does not exists.";

                }

            }

            return model;
        }


        public ApplicantDisplayModel GetApplicant( ApplicantDisplayModel model, string hkid, string passportno)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                string tempHKID = EncryptDecryptUtil.getEncrypt(hkid);
                bool tempResult = string.IsNullOrWhiteSpace(tempHKID);
                string tempPASSPORTNO = string.IsNullOrWhiteSpace( passportno) ? EncryptDecryptUtil.getEncrypt("nopasssport"):EncryptDecryptUtil.getEncrypt(passportno);
                var queryApp = (from IndApp in db.C_IND_APPLICATION
                               join ChinAdd in db.C_ADDRESS on IndApp.CHINESE_HOME_ADDRESS_ID equals ChinAdd.UUID
                               join EngAdd in db.C_ADDRESS on IndApp.ENGLISH_HOME_ADDRESS_ID equals EngAdd.UUID
                               join OfficeChinAdd in db.C_ADDRESS on IndApp.CHINESE_OFFICE_ADDRESS_ID equals OfficeChinAdd.UUID
                               join OfficeEngAdd in db.C_ADDRESS on IndApp.ENGLISH_OFFICE_ADDRESS_ID equals OfficeEngAdd.UUID
                               join Applicant in db.C_APPLICANT on IndApp.APPLICANT_ID equals Applicant.UUID
                               join certi in db.C_IND_CERTIFICATE on IndApp.UUID equals certi.MASTER_ID
                               where !tempResult ? Applicant.HKID == tempHKID : Applicant.PASSPORT_NO == tempPASSPORTNO
                               select new { IndApp, certi, ChinAdd, EngAdd, Applicant, OfficeChinAdd, OfficeEngAdd, }).FirstOrDefault();

                if (queryApp != null)
                {
                    model.HKID = queryApp.Applicant.HKID;
                    model.PassportNo = queryApp.Applicant.PASSPORT_NO;
                    model.Title = queryApp.Applicant.TITLE_ID;
                    model.Sex = queryApp.Applicant.GENDER;
                    model.Surname = queryApp.Applicant.SURNAME;
                    model.GivenName = queryApp.Applicant.GIVEN_NAME_ON_ID;
                    model.ChineseName = queryApp.Applicant.CHINESE_NAME;
                    model.Status = queryApp.certi.APPLICATION_STATUS_ID;
                    model.HOME_ADDRESS_CHI1 = queryApp.ChinAdd.ADDRESS_LINE1;
                    model.HOME_ADDRESS_CHI2 = queryApp.ChinAdd.ADDRESS_LINE2;
                    model.HOME_ADDRESS_CHI3 = queryApp.ChinAdd.ADDRESS_LINE3;
                    model.HOME_ADDRESS_CHI4 = queryApp.ChinAdd.ADDRESS_LINE4;
                    model.HOME_ADDRESS_CHI5 = queryApp.ChinAdd.ADDRESS_LINE5;
                    model.HOME_ADDRESS_ENG1 = queryApp.EngAdd.ADDRESS_LINE1;
                    model.HOME_ADDRESS_ENG2 = queryApp.EngAdd.ADDRESS_LINE2;
                    model.HOME_ADDRESS_ENG3 = queryApp.EngAdd.ADDRESS_LINE3;
                    model.HOME_ADDRESS_ENG4 = queryApp.EngAdd.ADDRESS_LINE4;
                    model.HOME_ADDRESS_ENG5 = queryApp.EngAdd.ADDRESS_LINE5;
                    model.OFFICE_ADDRESS_CHI1 = queryApp.OfficeChinAdd.ADDRESS_LINE1;
                    model.OFFICE_ADDRESS_CHI2 = queryApp.OfficeChinAdd.ADDRESS_LINE2;
                    model.OFFICE_ADDRESS_CHI3 = queryApp.OfficeChinAdd.ADDRESS_LINE3;
                    model.OFFICE_ADDRESS_CHI4 = queryApp.OfficeChinAdd.ADDRESS_LINE4;
                    model.OFFICE_ADDRESS_CHI5 = queryApp.OfficeChinAdd.ADDRESS_LINE5;
                    model.OFFICE_ADDRESS_ENG1 = queryApp.OfficeEngAdd.ADDRESS_LINE1;
                    model.OFFICE_ADDRESS_ENG2 = queryApp.OfficeEngAdd.ADDRESS_LINE2;
                    model.OFFICE_ADDRESS_ENG3 = queryApp.OfficeEngAdd.ADDRESS_LINE3;
                    model.OFFICE_ADDRESS_ENG4 = queryApp.OfficeEngAdd.ADDRESS_LINE4;
                    model.OFFICE_ADDRESS_ENG5 = queryApp.OfficeEngAdd.ADDRESS_LINE5;
                    model.FAX_NO1 = queryApp.IndApp.FAX_NO1;
                    model.FAX_NO2 = queryApp.IndApp.FAX_NO2;
                    model.TELEPHONE_NO1 = queryApp.IndApp.TELEPHONE_NO1;
                    model.TELEPHONE_NO2 = queryApp.IndApp.TELEPHONE_NO2;
                    model.TELEPHONE_NO3 = queryApp.IndApp.TELEPHONE_NO3;
                    model.EMERGENCY_NO1 = queryApp.IndApp.EMERGENCY_NO1;
                    model.EMERGENCY_NO2 = queryApp.IndApp.EMERGENCY_NO2;
                    model.EMERGENCY_NO3 = queryApp.IndApp.EMERGENCY_NO3;
                    model.ENGLISH_CARE_OF = queryApp.IndApp.ENGLISH_CARE_OF;
                    model.CHINESE_CARE_OF = queryApp.IndApp.CHINESE_CARE_OF;
                    model.CORRESPONDENCE_LANG_ID = queryApp.IndApp.CORRESPONDENCE_LANG_ID;
                    model.POST_TO = queryApp.IndApp.POST_TO;
                    model.EMAIL = queryApp.IndApp.EMAIL;

                }
                else
                {
                  ///  model.result = false;
                   model.ErrorMessage = "HKID or Passport No. does not exists.";

                }

            }

            return model;
        }

        public C_IND_CERTIFICATE GetSelectedCertiDetails(string id)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                return db.C_IND_CERTIFICATE.Where(x => x.UUID == id).FirstOrDefault();
            }
        }



        // Andy: search Defferal
        public void SearchDFR(Fn01Search_DFRSearchModel model)
        {
            String conditionTag = "<CONDITION>";
            String conditionValueIndApp = "and 2=2 " ;
            String conditionValueCompApp = "and 2=2 ";
            //conditionValueIndApp += "and indApp.REGISTRATION_TYPE= 'IP' ";
            //conditionValueIndApp += "and (SYSDATE - indProMon.DEFER_DATE)/30 >= 1 ";

            //conditionValueCompApp += "and compApp.REGISTRATION_TYPE= 'CMW' ";
            //conditionValueCompApp += "and (SYSDATE - compProMon.DEFER_DATE)/30 >= 1 ";

            String SearchDFR_indApp_q = this.SearchDFR_indApp_q;
            String SearchDFR_compApp_q = this.SearchDFR_compApp_q;

            // validation
            /*
            if (String.IsNullOrEmpty(model.DeferralPeriodSymbol) || String.IsNullOrEmpty(model.DeferralValue))
            {
                return;
            }
            */

            // add parameter
            if (model != null)
            {
                if (!String.IsNullOrEmpty(model.FileRef))
                {
                    String fileRef = model.FileRef.ToUpper().Trim();
                    conditionValueIndApp += "and indApp.FILE_REFERENCE_NO like '%" + fileRef + "%' ";
                    conditionValueCompApp += "and compApp.FILE_REFERENCE_NO like '%" + fileRef + "%' ";
                }
                if (!String.IsNullOrEmpty(model.CompName))
                {
                    String compName = model.CompName.ToUpper().Trim();
                    conditionValueIndApp += "and 1=2 ";
                    conditionValueCompApp += "and compApp.ENGLISH_COMPANY_NAME like '%" + compName + "%' ";
                }
                if (!String.IsNullOrEmpty(model.SurnName))
                {
                    String surnName = model.SurnName.ToUpper().Trim();
                    conditionValueIndApp += "and app.SURNAME like '%" + surnName + "%' ";
                    conditionValueCompApp += "and app.SURNAME like '%" + surnName + "%' ";
                }
                if (!String.IsNullOrEmpty(model.GivenName))
                {
                    String givenName = model.GivenName.ToUpper().Trim();
                    conditionValueIndApp += "and upper(app.GIVEN_NAME_ON_ID) like '%" + givenName + "%' ";
                    conditionValueCompApp += "and upper(app.GIVEN_NAME_ON_ID) like '%" + givenName + "%' ";
                }
                if (!String.IsNullOrEmpty(model.ChineseName))
                {
                    String chineseName = model.ChineseName.ToUpper().Trim();
                    conditionValueIndApp += "and app.CHINESE_NAME like '%" + chineseName + "%' ";
                    conditionValueCompApp += "and app.CHINESE_NAME like '%" + chineseName + "%' ";
                }
                if (!String.IsNullOrEmpty(model.DeferralPeriodSymbol) && !String.IsNullOrEmpty(model.DeferralValue))
                {
                    String deferralPeriodSymbol = model.DeferralPeriodSymbol.Trim();
                    String deferralValue = model.DeferralValue.Trim();

                    //conditionValueIndApp += "and (SYSDATE - indProMon.DEFER_DATE)/30 >= 1 ";
                    //conditionValueCompApp += "and (SYSDATE - compProMon.DEFER_DATE)/30 >= 1 ";
                    conditionValueIndApp += "and (SYSDATE - indProMon.DEFER_DATE)/30 " + deferralPeriodSymbol + deferralValue;
                    conditionValueCompApp += "and (SYSDATE - compProMon.DEFER_DATE)/30" + deferralPeriodSymbol + deferralValue;
                }

                String regTypeIP = "'" + RegistrationConstant.REGISTRATION_TYPE_IP + "'";
                String regTypeMWCI = "'" + RegistrationConstant.REGISTRATION_TYPE_MWIA + "'";
                String regTypeGBC = "'" + RegistrationConstant.REGISTRATION_TYPE_CGA + "'";
                String regTypeMWC = "'" + RegistrationConstant.REGISTRATION_TYPE_MWCA + "'";

                String indAppTypeStr = "";
                String compAppTypeStr = "";
                if (!String.IsNullOrEmpty(model.ChkProf) && "ON".Equals(model.ChkProf.ToUpper()))
                {
                    indAppTypeStr += String.IsNullOrEmpty(indAppTypeStr) ? regTypeIP : ", " + regTypeIP;
                }
                if (!String.IsNullOrEmpty(model.ChkMWCI) && "ON".Equals(model.ChkMWCI.ToUpper()))
                {
                    indAppTypeStr += String.IsNullOrEmpty(indAppTypeStr) ? regTypeMWCI : ", " + regTypeMWCI;
                }
                if (!String.IsNullOrEmpty(model.ChkGBC) && "ON".Equals(model.ChkGBC.ToUpper()))
                {
                    compAppTypeStr += String.IsNullOrEmpty(compAppTypeStr) ? regTypeGBC : ", " + regTypeGBC;
                }
                if (!String.IsNullOrEmpty(model.ChkMWC) && "ON".Equals(model.ChkMWC.ToUpper()))
                {
                    compAppTypeStr += String.IsNullOrEmpty(compAppTypeStr) ? regTypeMWC : ", " + regTypeMWC;
                }

                // append criteric
                if (!String.IsNullOrEmpty(indAppTypeStr))
                {
                    conditionValueIndApp += "and indApp.REGISTRATION_TYPE in ("+ indAppTypeStr + ") ";
                }
                else
                {
                    conditionValueIndApp += "and 2=3 ";
                }

                if (!String.IsNullOrEmpty(compAppTypeStr))
                {
                    conditionValueCompApp += "and compApp.REGISTRATION_TYPE in (" + compAppTypeStr + ") ";
                }
                else
                {
                    conditionValueCompApp += "and 2=3 ";
                }
            }

            // replace condition to sql
            SearchDFR_indApp_q = SearchDFR_indApp_q.Replace(conditionTag, conditionValueIndApp);
            SearchDFR_compApp_q = SearchDFR_compApp_q.Replace(conditionTag, conditionValueCompApp);

            // sql by default
            String combinedSql = ""
                + "\r\n" + "\t" + "select combinedDeferralRecord.* from ( "
                + "\r\n" + "\t" + SearchDFR_indApp_q
                + "\r\n" + "\t" + "union"
                + "\r\n" + "\t" + SearchDFR_compApp_q
                + "\r\n" + "\t" + ") combinedDeferralRecord "
                + "\r\n" + "\t" + "where 1=1 ";
                //+ "\r\n" + "\t" + "order by combinedDeferralRecord.file_Ref_No ";

            model.Query = combinedSql;
            model.Search();
        }

        // Andy: view details of Defferal - fro Prof and MWC(W)
        public Fn01Search_DFRDisplayModel ViewDFRInfoForProfAndMWCI(string indProMonUuid)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                // get entites
                var queryApp = from IndProMon in db.C_IND_PROCESS_MONITOR
                               join sCatCode in db.C_S_CATEGORY_CODE on IndProMon.CATEGORY_ID equals sCatCode.UUID
                               join cIndApp in db.C_IND_APPLICATION on IndProMon.MASTER_ID equals cIndApp.UUID
                               join app in db.C_APPLICANT on cIndApp.APPLICANT_ID equals app.UUID
                               where IndProMon.UUID == indProMonUuid
                               select new { IndProMon, sCatCode, cIndApp, app };

                return new Fn01Search_DFRDisplayModel()
                {
                    C_IND_PROCESS_MONITOR = queryApp.FirstOrDefault().IndProMon
                    , C_S_CATEGORY_CODE = queryApp.FirstOrDefault().sCatCode
                    , C_IND_APPLICATION = queryApp.FirstOrDefault().cIndApp
                    , C_APPLICANT = queryApp.FirstOrDefault().app
                };
            }
        }

        // Andy: view details of Defferal - fro GBC and MWC
        public Fn01Search_DFRDisplayModel ViewDFRInfoForGBCAndMWC(string compProMonUuid)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                // get entites
                var queryApp = from compProMon in db.C_COMP_PROCESS_MONITOR
                               join sInterviewResult in db.C_S_SYSTEM_VALUE on compProMon.INTERVIEW_RESULT_ID equals sInterviewResult.UUID
                               join sApplicationForm in db.C_S_SYSTEM_VALUE on compProMon.APPLICATION_FORM_ID equals sApplicationForm.UUID
                               join cCompApp in db.C_COMP_APPLICATION on compProMon.MASTER_ID equals cCompApp.UUID
                               join cCompAppInfo in db.C_COMP_APPLICANT_INFO on compProMon.COMPANY_APPLICANTS_ID equals cCompAppInfo.UUID
                               join svRole in db.C_S_SYSTEM_VALUE on cCompAppInfo.APPLICANT_ROLE_ID equals svRole.UUID
                               join cApplicant in db.C_APPLICANT on cCompAppInfo.APPLICANT_ID equals cApplicant.UUID
                               where compProMon.UUID == compProMonUuid
                               select new { compProMon, cCompApp, sInterviewResult, sApplicationForm, cCompAppInfo, svRole, cApplicant };
                if (queryApp.Any())
                {
                    return new Fn01Search_DFRDisplayModel()
                    {
                        C_COMP_PROCESS_MONITOR = queryApp.FirstOrDefault().compProMon
                     ,
                        C_COMP_APPLICATION = queryApp.FirstOrDefault().cCompApp
                     ,
                        S_INTERVIEW_RESULT = queryApp.FirstOrDefault().sInterviewResult
                     ,
                        S_APPLICATION_FORM = queryApp.FirstOrDefault().sApplicationForm
                     ,
                        C_COMP_APPLICANT_INFO = queryApp.FirstOrDefault().cCompAppInfo
                     ,
                        SV_ROLE = queryApp.FirstOrDefault().svRole
                     ,
                        C_APPLICANT = queryApp.FirstOrDefault().cApplicant
                    };
                }
                else
                {
                    return new Fn01Search_DFRDisplayModel()
                    { };
                }
               
            }
        }

        // Andy: view details of Defferal
        public Fn01Search_DFRDisplayModel ViewDFR(string id)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                // get entites
                var queryApp = from IndApp in db.C_IND_APPLICATION
                               join ChinAdd in db.C_ADDRESS on IndApp.CHINESE_HOME_ADDRESS_ID equals ChinAdd.UUID
                               join EngAdd in db.C_ADDRESS on IndApp.ENGLISH_HOME_ADDRESS_ID equals EngAdd.UUID
                               join Applicant in db.C_APPLICANT on IndApp.APPLICANT_ID equals Applicant.UUID
                               join Certi in db.C_IND_CERTIFICATE on IndApp.UUID equals Certi.MASTER_ID
                               where IndApp.UUID == id
                               select new { IndApp, ChinAdd, EngAdd, Applicant, Certi };

                //var BuildSafetyInfo = db.C_BUILDING_SAFETY_INFO.Where(o => o.MASTER_ID == queryApp.FirstOrDefault().IndApp.UUID).ToList();

                return new Fn01Search_DFRDisplayModel()
                {
                };
            }
        }

        // Andy: export Defferal
        public string ExportDFR(Dictionary<string, string>[] Columns, FormCollection post)
        {
            //DisplayGrid dlr = new DisplayGrid() { Query = SearchDFR_q, Columns = Columns, Parameters = post };
            //return dlr.Export("ExportData");

            return "";
        }

        // Andy: Export certificate
        public string ExportCertificate(UpdateAppStatusSearchModel model)
        {
            model.Query = exportCertificate_q;

            Dictionary<string, string>[] Columns = new Dictionary<string, string>[3];

            Dictionary<string, string> c1 = new Dictionary<string, string>();
            c1.Add("displayName", "aNdY fIELD");
            c1.Add("columnName", "UUID");

            Dictionary<string, string> c2 = new Dictionary<string, string>();
            c2.Add("displayName", "kkk");
            c2.Add("columnName", "UUID");

            Dictionary<string, string> c3 = new Dictionary<string, string>();
            c3.Add("displayName", "wilson");
            c3.Add("columnName", "UUID");

            //o.Add(displayName: "File Reference", columnName: "FILE_REFERENCE_NO");

            Columns[0] = c1;
            Columns[1] = c2;
            Columns[2] = c3;

            model.Columns = Columns;

            //model.QueryWhere = SearchGCA_whereQ(model);
            return model.Export("ExportData");
        }


    }
}
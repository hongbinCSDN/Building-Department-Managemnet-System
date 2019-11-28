using MWMS2.Areas.Signboard.Models;
using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Services.Signborad.SignboardDAO
{
    public class SignboardRPTServices : BaseCommonService
    {
        public FileStreamResult ExportTBESReport(Fn04PRT_TBESSearchModel model)
        {
            List<string> headerList = new List<string>() {
                    "Item",
                    "SCU Ref. No.",
                    "Particulars of the signboard owner(CO / LTD / PERSON)",
                    "Works Location",
                    "SPO","PO",
                    "TO/SO",
                    "Form Type SC01 SC02 SC03",
                    "SC01 / SC02 /SC03 Received Date",
                    "No. of signboard submitted",
                    "Works Item No. for Certification",
                    "Class of MW",
                    "MW Item No.",
                    "Alteration  /  Strengthening  Involved (For SC01, SC02, SC03) ( Y / N )",
                    "A- Acknowledged R- Rejected W- Withdrawn",
                    "Form Type SC01C SC02C N/A",
                    "SC01C / SC02C Received Date",
                    "REG. NO. of AP",
                    "REG. NO. of PRC",
                    "Previously Related SCU Ref. No.",
                    "Order Related Cases",
                    "Status"
            };
            List<List<object>> data = new List<List<object>>();
            data = retrieveTBESInformation(model);

            return this.exportExcelFile("ToBeExpiry", headerList, data);
        }

        public List<List<object>> retrieveTBESInformation(Fn04PRT_TBESSearchModel model)
        {
            Dictionary<string, object> queryParameters = new Dictionary<string, object>();
            List<List<object>> data = new List<List<object>>();
            string whereQ = "";
            if (model.PeriodDate != null)
            {
                whereQ = "\r\n" + "\t" + "and sv.RECEIVED_DATE =:PeriodDate";
                queryParameters.Add("PeriodDate", model.PeriodDate);
            }
            if (model.To_Be_Expiry_Month != null)
            {
                whereQ = "\r\n" + "\t" + "and val.SIGNBOARD_EXPIRY_DATE <= add_months(sysdate ,:Month)";
                queryParameters.Add("Month", model.To_Be_Expiry_Month);
            }
            string queryStr = ""
                + "\r\n" + "\t" + "select                                                                                                                                    "
                + "\r\n" + "\t" + "ROW_NUMBER() OVER (ORDER BY SCURefNo) AS ROWNUMBER                                                                                        "
                + "\r\n" + "\t" + ",SCURefNo,PartOfTheSO,WorkLocation,SPO1,PO1,TO1,FromType,SC01SC02SC03ReceivedDate,                                                                    "
                + "\r\n" + "\t" + "NoOfSignboardSubmitted,WorkItemNoCert,MWITEM,MWCODE,Strengthening,S01S02S03AR,                                                            "
                + "\r\n" + "\t" + "Form_TypeSC02CSC01C,S01CS02VR,S01CS02CDORL,ApNo,PrcRegNo,PR_SCU_Ref_No,                                                                   "
                + "\r\n" + "\t" + "ORDER_Related_Cases,status                                                                                                                "
                + "\r\n" + "\t" + "from                                                                                                                                      "
                + "\r\n" + "\t" + "(                                                                                                                                         "
                + "\r\n" + "\t" + "select distinct                                                                                                                           "
                + "\r\n" + "\t" + "sv.reference_no as SCURefNo,                                                                                                              "
                + "\r\n" + "\t" + "svpc.NAME_ENGLISH as PartOfTheSO,                                                                                                         "
                + "\r\n" + "\t" + "sb.LOCATION_OF_SIGNBOARD as WorkLocation,                                                                                                 "
                + "\r\n" + "\t" + "SpoSp.BD_PORTAL_LOGIN as SPO1,                                                                                                            "
                + "\r\n" + "\t" + "PoSp.BD_PORTAL_LOGIN as PO1,                                                                                                              "
                + "\r\n" + "\t" + "ToSp.BD_PORTAL_LOGIN  as TO1,                                                                                                             "
                + "\r\n" + "\t" + "case when sv.FORM_CODE in ('SC02', 'SC01', 'SC03') then sv.FORM_CODE else null End as FromType,                                           "
                + "\r\n" + "\t" + "case when sv.FORM_CODE in ('SC02', 'SC01', 'SC03') then sv.Received_date else null End as SC01SC02SC03ReceivedDate,                       "
                + "\r\n" + "\t" + "'1' as NoOfSignboardSubmitted,                                                                                                            "
                + "\r\n" + "\t" + "SVITEM.VALIDATION_ITEM as WorkItemNoCert,                                                                                                 "
                + "\r\n" + "\t" + "                                                                                                                                          "
                + "\r\n" + "\t" + "decode(                                                                                                                                   "
                + "\r\n" + "\t" + "MWCODE.CLASS_CODE,                                                                                                                        "
                + "\r\n" + "\t" + "'CLASS I','I',                                                                                                                            "
                + "\r\n" + "\t" + "'CLASS II','II',                                                                                                                          "
                + "\r\n" + "\t" + "'CLASS III','III', MWCODE.CLASS_CODE) as MWITEM ,                                                                                         "
                + "\r\n" + "\t" + "MWCODE.MW_ITEM_CODE as MWCODE,                                                                                                            "
                + "\r\n" + "\t" + "                                                                                                                                          "
                + "\r\n" + "\t" + "case when FORM_CODE in ('SC02', 'SC01', 'SC03') then val.RECORD_ITEM_RESULT else null End as Strengthening,                               "
                + "\r\n" + "\t" + "case when FORM_CODE in ('SC02', 'SC01', 'SC03') then val.VALIDATION_RESULT else null end as S01S02S03AR,                                  "
                + "\r\n" + "\t" + "Case when FORM_CODE='SC01C' then 'SC01C' when sv.FORM_CODE='SC02C' then 'SC02C' else 'N/A' End as Form_TypeSC02CSC01C,                    "
                + "\r\n" + "\t" + "Case when FORM_CODE in ('SC02C', 'SC01C') then val.VALIDATION_RESULT else null End as S01CS02VR,                                          "
                + "\r\n" + "\t" + "Case when FORM_CODE in ('SC02C', 'SC01C') then sv.DATE_OF_RESULT_LETTER else null End as S01CS02CDORL,                                    "
                + "\r\n" + "\t" + "Case when IDENTIFY_FLAG in('AP') then svap.CERTIFICATION_NO else null End as ApNo,                                                        "
                + "\r\n" + "\t" + "Case when IDENTIFY_FLAG in('PRC') then svap.CERTIFICATION_NO else null End as PrcRegNo,                                                   "
                + "\r\n" + "\t" + "sv.PREVIOUS_SUBMISSION_NUMBER as PR_SCU_Ref_No,                                                                                           "
                + "\r\n" + "\t" + "sv.RELATED_ORDER_NO as ORDER_Related_Cases,                                                                                               "
                + "\r\n" + "\t" + "Case                                                                                                                                      "
                + "\r\n" + "\t" + "when sv.RECEIVED_DATE < sv.RECEIVED_DATE+1825 then '" + SignboardConstant.TDL_SEARCH_STATUS_VALIDATE + "'                                 "
                + "\r\n" + "\t" + "when sv.RECEIVED_DATE > sv.RECEIVED_DATE+1825 and sv.PREVIOUS_SUBMISSION_NUMBER is null then '" + SignboardConstant.TDL_SEARCH_STATUS_EXPIRED_BUT_NOT_REVALIDATED + "'               "
                + "\r\n" + "\t" + "when sv.RECEIVED_DATE > sv.RECEIVED_DATE+1825 and sv.PREVIOUS_SUBMISSION_NUMBER is not null then '" + SignboardConstant.TDL_SEARCH_STATUS_EXPIRED_BUT_REVALIDATED + "'    "
                + "\r\n" + "\t" + "when sv.PREVIOUS_SUBMISSION_NUMBER is not null then '" + SignboardConstant.TDL_SEARCH_STATUS_REVALIDATED + "'                             "
                + "\r\n" + "\t" + "Else null END as status                                                                                                                   "
                + "\r\n" + "\t" + "from b_sv_record sv                                                                                                                       "
                + "\r\n" + "\t" + "inner join b_sv_validation val on val.SV_RECORD_ID = sv.uuid                                                                              "
                + "\r\n" + "\t" + "inner join b_sv_signboard sb on sb.uuid = sv.SV_SIGNBOARD_ID                                                                              "
                + "\r\n" + "\t" + "inner join b_sv_record_validation_item SVITEM on SVITEM.SV_RECORD_ID = sv.uuid                                                            "
                + "\r\n" + "\t" + "inner join b_sv_record_item MWCODE on MWCODE.SV_RECORD_ID = sv.uuid                                                                       "
                + "\r\n" + "\t" + "inner join b_sv_Appointed_Professional svap on sv.uuid = svap.SV_RECORD_ID                                                                "
                + "\r\n" + "\t" + "inner join B_SV_PERSON_CONTACT svpc on svpc.uuid = sb.OWNER_ID                                                                            "
                + "\r\n" + "\t" + "inner join sys_post ToSp on ToSp.uuid = sv.TO_USER_ID                                                                                     "
                + "\r\n" + "\t" + "inner join sys_post PoSp on PoSp.uuid = sv.PO_USER_ID                                                                                     "
                + "\r\n" + "\t" + "inner join sys_post SpoSp on SpoSp.uuid = sv.SPO_USER_ID                                                                                   "
                + "\r\n" + "\t" + "where 1=1                                                                                                                                 "
                + "\r\n" + "\t" + "and val.SIGNBOARD_EXPIRY_DATE is not null                                                                                                 "
                + "\r\n" + "\t" + "and sv.UUID is not null                                                                                                                   "
                + "\r\n" + "\t" + whereQ
                + "\r\n" + "\t" + ")                                                                                                                                         "
                + "\r\n" + "\t" + "order by ROWNUMBER                                                                                                                        "
                ;

            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                using (DbConnection conn = db.Database.Connection)
                {
                    DbDataReader dr = CommonUtil.GetDataReader(conn, queryStr, queryParameters);
                    data = CommonUtil.convertToList(dr);
                    conn.Close();
                }
            }
            return data;
        }


    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Entity;
using MWMS2.Constant;
using MWMS2.Models;
using NPOI.SS.Formula.Functions;
using System.Data.Entity;
using Oracle.ManagedDataAccess.Client;
using System.Data.Entity.Validation;
using MWMS2.Utility;
using System.Web.Mvc;
using System.Reflection;

namespace MWMS2.Services
{
    public class ProcessingTdlDAOService : BaseDAOService
    {

        private const string search_q = @" SELECT T1.UUID                   UUID,
                                                   T1.REFERENCE_NO           REFERENCE_NO,
                                                   T1.DSN                    DSN,
                                                   T1.FORM_NO                FORM_NO,
                                                   T1.RRM_SYN_STATUS         RRM_SYN_STATUS,
                                                   T2.ISSUE_DATE_OF_BD106    ISSUE_DATE_OF_BD106,
                                                   T2.COMPLETION_DATE        COMPLETION_DATE,
                                                   T2.ANNUAL_INSPECTION_DATE ANNUAL_INSPECTION_DATE,
                                                   T2.SITE_INSP_DATE         SITE_INSP_DATE
                                            FROM   P_MW_Modification T1
                                                   LEFT JOIN P_MOD_BD106 T2
                                                          ON T1.UUID = T2.MW_MODIFICATION_ID
                                            WHERE  1 = 1  and T1.HANDING_STAFF is not null
                                                                                          ";
        private const string searchDR_q = @"SELECT *
                                            FROM   P_MW_DIRECT_RETURN
                                            WHERE  1 = 1
                                                   AND HANDING_STAFF_2 = :HANDING_STAFF_2 ";

        private const string getDRCheckboxList_q = @" SELECT sv.UUID             UUID,
                                                               sv.SYSTEM_Type_id SystemTypeID,
                                                               sv.code           Code,
                                                               sv.parent_id      ParentID,
                                                               sv.description    Description_C,
                                                               sv.description_e  Description_E,
                                                               is_active         IsActive
                                                        FROM   p_s_system_value sv
                                                               LEFT JOIN p_s_system_type sType
                                                                      ON sType.uuid = sv.SYSTEM_TYPE_ID
                                                        WHERE  1 = 1
                                                               AND sType.type = 'Irregularities'
                                                        ORDER  BY code ASC 
                                                         ";
        private const string getDWRemark2_q = @" SELECT sv.*
                                                FROM   p_s_system_value sv
                                                       LEFT JOIN p_s_system_type sType
                                                              ON sType.uuid = sv.SYSTEM_TYPE_ID
                                                WHERE  1 = 1
                                                       AND sType.type = 'DW_REMARK2'
                                                ORDER  BY code ASC ";
        private const string getDWRemark3_q = @" SELECT sv.*
                                                    FROM   p_s_system_value sv
                                                           LEFT JOIN p_s_system_type sType
                                                                  ON sType.uuid = sv.SYSTEM_TYPE_ID
                                                    WHERE  1 = 1
                                                           AND sType.type = 'DW_REMARK3'
                                                    ORDER  BY code ASC ";

        private const string getDRDetailLanguage_q = @" SELECT T1.*
                                                        FROM   P_S_System_value T1
                                                               LEFT JOIN P_S_System_type T2
                                                                      ON T1.SYSTEM_TYPE_ID = T2.UUID
                                                        WHERE  T2.type = 'DR_Language'
                                                        ORDER  BY ORDERING ";
        public Fn03TSK_TdlSearchModel SearchMOD(Fn03TSK_TdlSearchModel model)
        {
            model.Query = search_q;
            model.Search();
            return model;
        }

        public Fn03TSK_TdlSearchModel SearchDR(Fn03TSK_TdlSearchModel model)
        {
            model.Query = searchDR_q;
            model.QueryParameters.Add("HANDING_STAFF_2", Utility.SessionUtil.LoginPost.CODE);
            model.Search();
            return model;
        }

        public Fn03TSK_TdlSearchModel GetDRDetailLanguage(Fn03TSK_TdlSearchModel model)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                model.Languages = db.Database.SqlQuery<P_S_SYSTEM_VALUE>(getDRDetailLanguage_q).ToList();
                //model.Languages = new List<P_S_SYSTEM_VALUE>();
                //for (int i = 0; i < 11; i++)
                //{
                //    model.Languages.Add(new P_S_SYSTEM_VALUE() { DESCRIPTION = "Chinese", DESCRIPTION_E = "English" });
                //}
                return model;
            }
        }

        public Fn03TSK_TdlSearchModel GetDRByUUID(string uuid)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                Fn03TSK_TdlSearchModel model = new Fn03TSK_TdlSearchModel();
                model.DR = db.P_MW_DIRECT_RETURN.Where(m => m.UUID == uuid).FirstOrDefault();
                return model;
            }
        }
        public List<Fn03TSK_DRCheckboxList> GetDRCheckboxList()
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return db.Database.SqlQuery<Fn03TSK_DRCheckboxList>(getDRCheckboxList_q).ToList();
            }
        }

        public V_CRM_INFO GetVCRMInfo(string ContractorRegNo)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return (from dr in db.P_MW_DIRECT_RETURN
                        join ci in db.V_CRM_INFO on dr.CONTRACTOR_REG_NO equals ci.CERTIFICATION_NO
                        where ci.CERTIFICATION_NO == ContractorRegNo
                        select ci).FirstOrDefault();
            }
        }

        public P_S_TO_DETAILS GetToDetails(string HandingStaff)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return (from dr in db.P_MW_DIRECT_RETURN
                        join td in db.P_S_TO_DETAILS on dr.HANDING_STAFF_2 equals td.TO_POST
                        where td.TO_POST == HandingStaff
                        select td).FirstOrDefault();
            }
        }


        public List<P_MW_DIRECT_RETURN_IRREGULARITIES> GetDRCheckboxSelected(Fn03TSK_TdlSearchModel model)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return db.P_MW_DIRECT_RETURN_IRREGULARITIES.Where(m => m.MASTER_ID == model.DR.UUID).ToList();
            }
        }
        public P_S_SYSTEM_VALUE GetDWRemark2()
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return db.Database.SqlQuery<P_S_SYSTEM_VALUE>(getDWRemark2_q).First();
            }
        }
        public P_S_SYSTEM_VALUE GetDWRemark3()
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return db.Database.SqlQuery<P_S_SYSTEM_VALUE>(getDWRemark3_q).First();
            }
        }
        public bool IsCheckedDWRemark3(P_S_SYSTEM_VALUE dwRemark3, Fn03TSK_TdlSearchModel model)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return db.P_MW_DIRECT_RETURN_IRREGULARITIES.Where(m => m.MASTER_ID == model.DR.UUID && m.SV_IRREGULARITIES_ID == dwRemark3.UUID).FirstOrDefault() == null ? false : true;
            }
        }
        public ServiceResult RemoveFromTDL(Fn03TSK_TdlSearchModel model)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                P_MW_DIRECT_RETURN dr = db.P_MW_DIRECT_RETURN.Where(m => m.UUID == model.DR.UUID).FirstOrDefault();
                dr.HANDING_STAFF_2 = null;
                //dr.HANDING_STAFF_3 = null;
                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    return new ServiceResult()
                    {
                        Result = ServiceResult.RESULT_FAILURE
                        ,
                        Message = { ex.Message }
                    };
                }
                return new ServiceResult()
                {
                    Result = ServiceResult.RESULT_SUCCESS
                };
            }
        }

        public ServiceResult UpdateDRDetail(Fn03TSK_TdlSearchModel model)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    db.Entry(model.DR).State = EntityState.Modified;
                    List<P_MW_DIRECT_RETURN_IRREGULARITIES> originDRI = db.P_MW_DIRECT_RETURN_IRREGULARITIES.Where(m => m.MASTER_ID == model.DR.UUID).ToList();
                    var directReturn = db.P_MW_DIRECT_RETURN.Where(m => m.UUID == model.DR.UUID).FirstOrDefault();
                    if (originDRI.Count() > 0)
                    {
                        db.P_MW_DIRECT_RETURN_IRREGULARITIES.RemoveRange(originDRI);
                    }
                    foreach (var item in model.ListCheckbox)
                    {
                        if (item.IsChecked)
                        {
                            db.P_MW_DIRECT_RETURN_IRREGULARITIES.Add(new P_MW_DIRECT_RETURN_IRREGULARITIES()
                            {
                                MASTER_ID = model.DR.UUID
                                ,
                                SV_IRREGULARITIES_ID = item.UUID
                                ,
                                IS_CHECKED = "True"
                            });
                        }
                    }
                    if (model.DWRemark3.IsChecked)
                    {
                        db.P_MW_DIRECT_RETURN_IRREGULARITIES.Add(new P_MW_DIRECT_RETURN_IRREGULARITIES()
                        {
                            MASTER_ID = model.DR.UUID
                                    ,
                            SV_IRREGULARITIES_ID = model.DWRemark3.UUID
                                    ,
                            IS_CHECKED = "True"
                        });
                    }

                    if (!string.IsNullOrEmpty(model.DR.LANGUAGE))
                    {
                        directReturn.LANGUAGE = model.DR.LANGUAGE;
                    }
                    if (model.DR.RECEIVED_DATE != null)
                    {
                        directReturn.RECEIVED_DATE = model.DR.RECEIVED_DATE;
                    }

                    try
                    {
                        db.SaveChanges();
                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        return new ServiceResult()
                        {
                            Result = ServiceResult.RESULT_FAILURE
                            ,
                            Message =
                            {
                                ex.Message
                            }
                        };
                    }

                    return new ServiceResult()
                    {
                        Result = ServiceResult.RESULT_SUCCESS
                    };
                }
            }
        }
















        public void SearchGeneralRecords(string recordType, Fn03TSK_TdlSearchModel model)
        {
            GeneralRecordsQ(recordType, model);
            //model.Rpp = -1;
            model.Search();
        }
        public void ExcelGeneralRecords(string recordType, Fn03TSK_TdlSearchModel model)
        {
            GeneralRecordsQ(recordType, model);
            model.Export("Todo List - " + recordType + " Records");
        }
        public void GeneralRecordsQ(string recordType, Fn03TSK_TdlSearchModel model)
        {
            if (model == null) return;
            model.Query =
                      //       "\r\n\t" + " select gr.uuid                                                                                  "
                      //     
                      //       
                      //       + "\r\n\t" + " ,  gr.submit_type                                                                               "
                      //     + "\r\n\t" + " ,  gr.icc_number                                                                                "
                      //     + "\r\n\t" + " ,  refNo.REFERENCE_NO                                                                           "
                      //     + "\r\n\t" + " ,  gr.RECEIVE_DATE                                                                              "
                      //     + "\r\n\t" + " ,  gr.FINAL_REPLY_DATE                                                                          "
                      //     + "\r\n\t" + " ,  to_char(gr.INTERIM_REPLY_DUE_DATE, 'dd/mm/yyyy') interimReplyDueDate                         "
                      //     + "\r\n\t" + " ,  to_char(gr.INTERIM_REPLY_DATE, 'dd/mm/yyyy') interimReplyDate                                "
                      //     + "\r\n\t" + " ,  to_char(gr.MODIFIED_DATE, 'dd/mm/yyyy') modifiedDate                                         "
                      //     + "\r\n\t" + " ,  gr.subject_matter                                                                            "
                      //     + "\r\n\t" + " ,  gr.CHANNEL                                                                                   "
                      //     + "\r\n\t" + " ,  gr.form_status                                                                               "
                      //     + "\r\n\t" + " ,   gr.CASE_TITLE                                                                               "
                      //
                      //
                      //     + "\r\n\t" + " , (SELECT MAX(J1.dsn)                                                                            "
                      //     + "\r\n\t" + " FROM P_MW_DSN J1                                                                                 "
                      //     + "\r\n\t" + " , P_S_SYSTEM_VALUE J2                                                                            
                      //
                      //
                      //
                      //
                      //     + "\r\n\t" + " WHERE J1.SCANNED_STATUS_ID = J2.UUID                                                             "
                      //     + "\r\n\t" + " AND J2.CODE IN( 'GENERAL_COMPLETED' , 'DOCUMENT_SORTING')                                        "
                      //     + "\r\n\t" + " AND J1.RECORD_ID = refNo.REFERENCE_NO ) AS DSN                                                   "


                      //  "\r\n\t" + " select gr.uuid                                                                                  "
                      "\r\n\t" + " select gr.uuid AS UUID                                                       "
                    + "\r\n\t" + " , CASE WHEN gr.submit_type = 'ICC' THEN refNo.REFERENCE_NO                   "
                    + "\r\n\t" + " ELSE NULL END AS ICCNO                                                       "
                    + "\r\n\t" + "                                                                              "
                    + "\r\n\t" + " , CASE WHEN gr.submit_type = 'ICC' THEN gr.uuid                              "
                    + "\r\n\t" + " ELSE                                                                         "
                    + "\r\n\t" + " (SELECT MAX(J1.dsn) FROM P_MW_DSN J1                                         "
                    + "\r\n\t" + " , P_S_SYSTEM_VALUE J2                                                        "
                    + "\r\n\t" + " WHERE J1.SCANNED_STATUS_ID = J2.UUID                                         "
                    + "\r\n\t" + " AND J2.CODE IN('GENERAL_COMPLETED', 'DOCUMENT_SORTING')                      "
                    + "\r\n\t" + " AND J1.RECORD_ID = refNo.REFERENCE_NO)                                       "
                    + "\r\n\t" + " END                                                                          "
                    + "\r\n\t" + " AS KEYID                                                                     "
                    + "\r\n\t" + "                                                                              "
                    + "\r\n\t" + " , (SELECT MAX(J1.dsn) FROM P_MW_DSN J1                                       "
                    + "\r\n\t" + " , P_S_SYSTEM_VALUE J2                                                        "
                    + "\r\n\t" + " WHERE J1.SCANNED_STATUS_ID = J2.UUID                                         "
                    + "\r\n\t" + " AND J2.CODE IN( 'GENERAL_COMPLETED' , 'DOCUMENT_SORTING')                    "
                    + "\r\n\t" + " AND J1.RECORD_ID = refNo.REFERENCE_NO ) AS DSN                               "
                    + "\r\n\t" + "                                                                              "
                    + "\r\n\t" + " , refNo.REFERENCE_NO AS CASENO                                               "
                    + "\r\n\t" + "                                                                              "
                    + "\r\n\t" + " , gr.RECEIVE_DATE AS receiveDate                                             "
                    + "\r\n\t" + " , gr.RECEIVE_DATE + 30 AS FinalReplyDueDate                                  "
                    + "\r\n\t" + " , FLOOR(gr.RECEIVE_DATE - SYSDATE) AS FinalReplyRemainingDays                       "
                    + "\r\n\t" + "                                                                              "
                    + "\r\n\t" + " , gr.INTERIM_REPLY_DUE_DATE AS  InterimReplyDueDate                          "
                    + "\r\n\t" + " , FLOOR(gr.RECEIVE_DATE - (SYSDATE + 10)) AS InterimReplyRemainingDays              "
                    + "\r\n\t" + "                                                                              "
                    + "\r\n\t" + "                                                                              "
                    + "\r\n\t" + " , gr.INTERIM_REPLY_DATE AS InterimReplyDate                                  "
                    + "\r\n\t" + "                                                                              "
                    + "\r\n\t" + " , gr.subject_matter AS SubjectMatter                                         "
                    + "\r\n\t" + " , gr.CHANNEL AS Channel                                                      "
                    + "\r\n\t" + "                                                                              "
                    + "\r\n\t" + " , CASE WHEN gr.form_status = 'General Record New' THEN 'Open'                "
                    + "\r\n\t" + " WHEN gr.form_status = 'General Record Draft' THEN 'In progress'              "
                    + "\r\n\t" + " WHEN gr.form_status = 'General Record Completed' THEN 'In progress'          "
                    + "\r\n\t" + " END AS status                                                                "
                    + "\r\n\t" + " , gr.CASE_TITLE AS CaseTitle                                                 "

                    //+ "\r\n\t" + " , (SELECT T.CREATETIME                                                                     "
                    //+ "\r\n\t" + " FROM P_DSN_REQUESTID DS, P_PROCESSTRANSACTION A, P_WFTASK T                                "
                    //+ "\r\n\t" + " WHERE DS.REQUESTID = A.REQUESTID AND                                                       "
                    //+ "\r\n\t" + " A.INSTANCEID = T.INSTANCEID AND                                                            "
                    //+ "\r\n\t" + " (T.TASKNAME = 'Enquiry' OR T.TASKNAME = 'Complaint Handling CheckList') AND DS.DSN =       "
                    //+ "\r\n\t" + " (SELECT MAX(J1.dsn) FROM P_MW_DSN J1                                                       "
                    //+ "\r\n\t" + " , P_S_SYSTEM_VALUE J2                                                                      "
                    //+ "\r\n\t" + " WHERE J1.SCANNED_STATUS_ID = J2.UUID                                                       "
                    //+ "\r\n\t" + " AND J2.CODE IN( 'GENERAL_COMPLETED' , 'DOCUMENT_SORTING')                                  "
                    //+ "\r\n\t" + " AND J1.RECORD_ID = refNo.REFERENCE_NO )                                                    "
                    //+ "\r\n\t" + " AND ROWNUM = 1                                                                             "
                    //+ "\r\n\t" + " ) AS ASSDATE                                                                               "



                    + "\r\n\t" + " from P_MW_GENERAL_RECORD gr                                                                     "
                    + "\r\n\t" + " left join P_MW_COMPLAINT_CHECKLIST checklist on checklist.record_id = gr.uuid                   "
                    + "\r\n\t" + " join P_MW_REFERENCE_NO refNo on refNo.uuid = gr.REFERENCE_NUMBER                                ";
            string whereQ = ""
                    + "\r\n\t" + " where((gr.submit_type = :recordType1) or(gr.submit_type = 'ICC' and gr.icc_type = :recordType2)) "
                    + "\r\n\t" + " and(gr.form_status in ('General Record New', 'General Record Draft')                             "
                    + "\r\n\t" + " or(gr.form_status = 'General Record Completed'  and (checklist.flow_status IS NULL OR checklist.flow_status != 'DONE')   ))            ";
            whereQ += ""
+ "\r\n\t" + " AND EXISTS(SELECT * FROM P_WF_INFO J1                                  "
+ "\r\n\t" + " INNER JOIN P_WF_TASK J2 ON J2.P_WF_INFO_ID = J1.UUID        "
+ "\r\n\t" + " INNER JOIN P_WF_TASK_USER J3 ON J3.P_WF_TASK_ID = J2.UUID   "
+ "\r\n\t" + " WHERE J3.STATUS = 'WF_STATUS_OPEN' AND RECORD_ID = gr.UUID AND J3.USER_ID Like :USER_ID        )";

            var role = GetObjectData<SYS_ROLE>(@"SELECT R.*
                                                    FROM   SYS_POST_ROLE PR
                                                           JOIN SYS_POST P
                                                             ON PR.SYS_POST_ID = P.UUID
                                                                AND P.CODE = :UserCode
                                                           JOIN SYS_ROLE R
                                                             ON PR.SYS_ROLE_ID = R.UUID
                                                                AND R.CODE = :RoleCode ",
                                                                new OracleParameter[] {
                                                                    new OracleParameter(":UserCode",SessionUtil.LoginPost.CODE),
                                                                    new OracleParameter(":RoleCode",ProcessingConstant.ADMIN)
                                                                }).FirstOrDefault();

            if (role != null)
            {
                model.QueryParameters.Add("USER_ID", "%");
            }
            else
            {
                model.QueryParameters.Add("USER_ID", SessionUtil.LoginPost.UUID);
            }

            model.QueryParameters.Add("recordType1", recordType);
            model.QueryParameters.Add("recordType2", recordType);
            if (!string.IsNullOrWhiteSpace(model.RefNo))
            {
                whereQ = whereQ + "\r\n\t" + " and refNo.REFERENCE_NO = :regNo                                                    ";
                model.QueryParameters.Add("regNo", model.RefNo);
            }
            model.QueryWhere = whereQ;
            model.Sort = "gr.RECEIVE_DATE";
        }















        public void SearchAudits(Fn03TSK_TdlSearchModel model)
        {
            AuditsQ(model);
            //model.Rpp = -1;
            model.Search();
        }
        public void ExcelAudits(Fn03TSK_TdlSearchModel model)
        {
            AuditsQ(model);
            model.Export("Todo List - Audit Records");
        }
        public void AuditsQ(Fn03TSK_TdlSearchModel model)
        {
            if (model == null) return;
            model.Query = ""
                    + "\r\n\t" + " select                                                                         "
                    + "\r\n\t" + " au.dsn                                                                         "
                    + "\r\n\t" + " , refNo.reference_no                                                           "
                    + "\r\n\t" + " , rcd.s_form_type_code                                                         "
                    + "\r\n\t" + " , to_char(au.MODIFIED_DATE, 'dd/mm/yyyy') assignDate                           "
                    + "\r\n\t" + " , au.audit_status                                                              "
                    + "\r\n\t" + " , au.uuid                                                                      "
                    + "\r\n\t" + " , rcd.uuid as recrod_id                                                        "
                    + "\r\n\t" + " , CASE                                                                         "
                    + "\r\n\t" + " WHEN audit_status = 'Open' THEN 1                                              "
                    + "\r\n\t" + " WHEN audit_status = 'In progress' THEN 2                                       "
                    + "\r\n\t" + " WHEN audit_status = 'PO Completed' THEN 3 END AS ORD                           "
                    + "\r\n\t" + " from p_mw_record_audit au                                                      "
                    + "\r\n\t" + " INNER JOIN p_mw_record rcd on rcd.uuid = au.mw_record_id                       "
                    + "\r\n\t" + " INNER JOIN p_mw_reference_no refNo on refNo.uuid = rcd.REFERENCE_NUMBER        ";


            if (!string.IsNullOrWhiteSpace(model.RefNo))
            {
                string whereQ = "\r\n\t" + " WHERE refNo.reference_no = :regNo                                    ";
                model.QueryParameters.Add("regNo", model.RefNo);
                model.QueryWhere = whereQ;
            }

            model.Sort = "ORD, refNo.reference_no";
        }











        public void SearchSubmissions(Fn03TSK_TdlSearchModel model)
        {
            SubmissionsQ(model);
            //model.Rpp = -1;
            model.Search();


        }
        public void ExcelSubmissions(Fn03TSK_TdlSearchModel model)
        {
            SubmissionsQ(model);
            model.Export("Todo List - Submission Records");
        }
        public void SubmissionsQ(Fn03TSK_TdlSearchModel model)
        {
            if (model == null) return;
            model.Query = @"SELECT ( CASE
                                       WHEN V.STATUS_CODE = 'MW_VERT_STATUS_ROLLBACK' THEN 0
                                       WHEN V.STATUS_CODE = 'MW_VERT_STATUS_OPEN' THEN 1
                                       WHEN V.STATUS_CODE = 'MW_ACKN_STATUS_OPEN' THEN
                                         CASE
                                           WHEN V.SPO_ROLLBACK IS NULL THEN 2
                                           WHEN V.SPO_ROLLBACK = 'Y' THEN 3
                                           ELSE 4
                                         END
                                       ELSE 5
                                     END ) AS ORD,
                                   ( CASE
                                       WHEN V.STATUS_CODE = 'MW_VERT_STATUS_OPEN' THEN 'Verification'
                                       WHEN V.STATUS_CODE = 'MW_VERT_STATUS_ROLLBACK' THEN 'Rollbacked Verification'
                                       WHEN V.STATUS_CODE = 'MW_ACKN_STATUS_OPEN' THEN 'Acknowledgement'
                                       ELSE NULL
                                     END ) AS TASK,
                                   RN.REFERENCE_NO,
                                   R.S_FORM_TYPE_CODE,
                                   F.RECEIVED_DATE,
                                   R.MODIFIED_DATE,
                                   ( CASE
                                       WHEN V.STATUS_CODE = 'MW_VERT_STATUS_OPEN' THEN
                                         CASE
                                           WHEN FCHECK.UUID IS NULL THEN 'Open'
                                           ELSE 'In progress'
                                         END
                                       WHEN V.STATUS_CODE = 'MW_ACKN_STATUS_OPEN' THEN
                                         CASE
                                           WHEN 'Acknowledgement-SPO' = '123'--VWF.ACTIVITYID                                                                                                           
                                         THEN
                                             CASE
                                               WHEN SCHECK.MODIFIED_BY = SCHECK.CREATED_BY THEN 'Open'
                                               ELSE 'In progress'
                                             END
                                           ELSE
                                             CASE
                                               WHEN POCHECK.UUID IS NULL THEN 'Open'
                                               ELSE 'In progress'
                                             END
                                         END
                                       ELSE 'Open'
                                     END ) AS PROGRESS,
                                   R.UUID  AS R_UUID,
                                   V.UUID  AS V_UUID,
                                   R.MW_DSN,
                                   WT.TASK_CODE,
                                   WTU.USER_ID
                            FROM   P_WF_INFO WI
                                   JOIN P_WF_TASK WT
                                     ON WI.UUID = WT.P_WF_INFO_ID
                                        AND WT.STATUS = 'WF_STATUS_OPEN'
                                   JOIN P_WF_TASK_User WTU
                                     ON WT.UUID = WTU.P_WF_TASK_ID
                                        AND WTU.USER_ID Like :USER_ID
                                   JOIN P_MW_RECORD R
                                     ON WI.RECORD_ID = R.MW_DSN
                                        AND IS_DATA_ENTRY = 'Y'
                                   JOIN P_MW_FORM F
                                     ON R.UUID = F.MW_RECORD_ID
                                   JOIN P_MW_REFERENCE_NO RN
                                     ON R.REFERENCE_NUMBER = RN.UUID
                                   JOIN P_MW_VERIFICATION V
                                     ON R.UUID = V.MW_RECORD_ID
                                        AND V.STATUS_CODE IN ( 'MW_VERT_STATUS_OPEN', 'MW_ACKN_STATUS_OPEN', 'MW_VERT_STATUS_ROLLBACK' )
                                        AND Nvl(V.HANDLING_UNIT, 'PEM') = :HANDLING_UNIT
                                        AND ( CASE
                                                WHEN WT.TASK_CODE LIKE '%SMM' THEN 'SMM'
                                                ELSE 'PEM'
                                              END ) = Nvl(V.HANDLING_UNIT, 'PEM')
                                   LEFT JOIN P_MW_RECORD_FORM_CHECKLIST FCHECK
                                          ON R.UUID = FCHECK.MW_RECORD_ID
                                             AND Nvl(V.HANDLING_UNIT, 'PEM') = FCHECK.HANDLING_UNIT
                                   LEFT JOIN P_MW_RECORD_FORM_CHECKLIST_PO POCHECK
                                          ON FCHECK.UUID = POCHECK.MW_RECORD_FORM_CHECKLIST_ID
                                             AND Nvl(V.HANDLING_UNIT, 'PEM') = POCHECK.HANDLING_UNIT
                                   LEFT JOIN P_MW_SUMMARY_MW_ITEM_CHECKLIST SCHECK
                                          ON R.UUID = SCHECK.MW_RECORD_ID
                                             AND Nvl(V.HANDLING_UNIT, 'PEM') = SCHECK.HANDLING_UNIT
                            WHERE  WI.RECORD_TYPE = 'WF_TYPE_SUBMISSION'
                                   AND WI.CURRENT_STATUS = 'WF_STATUS_OPEN' ";


            var role = GetObjectData<SYS_ROLE>(@"SELECT R.*
                                                    FROM   SYS_POST_ROLE PR
                                                           JOIN SYS_POST P
                                                             ON PR.SYS_POST_ID = P.UUID
                                                                AND P.CODE = :UserCode
                                                           JOIN SYS_ROLE R
                                                             ON PR.SYS_ROLE_ID = R.UUID
                                                                AND R.CODE = :RoleCode ", 
                                                                new OracleParameter[] {
                                                                    new OracleParameter(":UserCode",SessionUtil.LoginPost.CODE),
                                                                    new OracleParameter(":RoleCode",ProcessingConstant.ADMIN)
                                                                }).FirstOrDefault();

            if (role != null)
            {
                model.QueryParameters.Add("USER_ID", "%");
            }
            else
            {
                model.QueryParameters.Add("USER_ID", SessionUtil.LoginPost.UUID);
            }

            model.QueryParameters.Add("HANDLING_UNIT", model.HandlingUnit);


            if (!string.IsNullOrWhiteSpace(model.RefNo))
            {
                model.QueryWhere = "\r\n\t" + " and RN.REFERENCE_NO = :regNo ";
                model.QueryParameters.Add("regNo", model.RefNo);
            }

            model.Sort = "ORD, F.RECEIVED_DATE , RN.REFERENCE_NO";
        }


        #region Submision

        public P_MW_DSN GetP_MW_DSN(string DSN)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return db.P_MW_DSN.Where(d => d.DSN == DSN).FirstOrDefault();
            }
        }

        public P_MW_REFERENCE_NO GetP_MW_REFERENCE_NO(string UUID)
        {
            return GetObjectRecordByUuid<P_MW_REFERENCE_NO>(UUID);
        }

        public P_MW_RECORD GetP_MW_RECORD(string UUID)
        {
            return GetObjectRecordByUuid<P_MW_RECORD>(UUID);
        }

        public List<P_MW_SCANNED_DOCUMENT> GetP_MW_SCANNED_DOCUMENTs(string RECORD_ID)
        {
            string sSql = @"SELECT SD.*
                            FROM   P_MW_DSN D
                                   JOIN P_MW_SCANNED_DOCUMENT SD
                                     ON D.UUID = SD.DSN_ID
                            WHERE  D.RECORD_ID = :RECORD_ID ";
            OracleParameter[] oracleParameters = new OracleParameter[]
            {
                new OracleParameter(":RECORD_ID",RECORD_ID)
            };
            return GetObjectData<P_MW_SCANNED_DOCUMENT>(sSql, oracleParameters).ToList();
        }

        public List<P_MW_SCANNED_DOCUMENT> GetP_MW_SCANNED_DOCUMENTsIC(string RECORD_ID, string MW_RECORD_ID)
        {
            string sSql = @"SELECT SD.UUID,
                                   SD.DSN_ID,
                                   SD.DSN_SUB,
                                   SD.FILE_PATH,
                                   SD.DOCUMENT_TYPE,
                                   SD.SCAN_DATE,
                                   SD.FOLDER_TYPE,
                                   RLI.TEMPLATE_NAME,
                                   NVL(Max(RLI.MODIFIED_DATE),SYSDATE) AS MODIFIED_DATE
                            FROM   P_MW_DSN D
                                   JOIN P_MW_SCANNED_DOCUMENT SD
                                     ON D.UUID = SD.DSN_ID
                                   LEFT JOIN P_MW_RECORD_LETTER_INFO RLI
                                          ON D.UUID = RLI.MW_DSN_ID
                                             AND RLI.MW_RECORD_ID = :MW_RECORD_ID
                            WHERE  D.RECORD_ID = :RECORD_ID
                                   AND D.SUBMIT_TYPE = 'Issued Correspondence'
                            GROUP  BY SD.UUID,
                                      SD.DSN_ID,
                                      SD.DSN_SUB,
                                      SD.FILE_PATH,
                                      SD.DOCUMENT_TYPE,
                                      SD.SCAN_DATE,
                                      SD.FOLDER_TYPE,
                                      RLI.TEMPLATE_NAME ";
            OracleParameter[] oracleParameters = new OracleParameter[]
            {
                new OracleParameter(":MW_RECORD_ID",MW_RECORD_ID),
                new OracleParameter(":RECORD_ID",RECORD_ID)

            };
            return GetObjectData<P_MW_SCANNED_DOCUMENT>(sSql, oracleParameters).ToList();
        }

        public List<P_MW_SCANNED_DOCUMENT> GetP_MW_SCANNED_DOCUMENTsNIC(string RECORD_ID)
        {
            string sSql = @"SELECT SD.*
                            FROM   P_MW_DSN D
                                   JOIN P_MW_SCANNED_DOCUMENT SD
                                     ON D.UUID = SD.DSN_ID
                            WHERE  D.RECORD_ID = :RECORD_ID
                                   AND D.SUBMIT_TYPE != 'Issued Correspondence' ";
            OracleParameter[] oracleParameters = new OracleParameter[]
            {
                new OracleParameter(":RECORD_ID",RECORD_ID)
            };
            return GetObjectData<P_MW_SCANNED_DOCUMENT>(sSql, oracleParameters).ToList();
        }

        public P_MW_RECORD_PSAC GetP_MW_RECORD_PSAC(string RECORD_ID, string handlingUnit)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return db.P_MW_RECORD_PSAC.Where(d => d.RECORD_ID == RECORD_ID && (handlingUnit == ProcessingConstant.HANDLING_UNIT_SMM ? (d.HANDLING_UNIT == handlingUnit) : ((d.HANDLING_UNIT == handlingUnit) || (d.HANDLING_UNIT == null)))).FirstOrDefault();
            }
        }

        public P_MW_RECORD_SAC GetP_MW_RECORD_SAC(string RECORD_ID, string pageCode, string handlingUnit)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return db.P_MW_RECORD_SAC.Where(d => d.RECORD_ID == RECORD_ID && d.SYS_PAGE_CODE == pageCode && (handlingUnit == ProcessingConstant.HANDLING_UNIT_SMM ? (d.HANDLING_UNIT == handlingUnit) : ((d.HANDLING_UNIT == handlingUnit) || (d.HANDLING_UNIT == null)))).FirstOrDefault();
            }
        }

        public P_MW_SUMMARY_MW_ITEM_CHECKLIST GetP_MW_SUMMARY_MW_ITEM_CHECKLIST(string MW_RECORD_ID, string handlingUnit)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return db.P_MW_SUMMARY_MW_ITEM_CHECKLIST.Where(d => d.MW_RECORD_ID == MW_RECORD_ID && (handlingUnit == ProcessingConstant.HANDLING_UNIT_SMM ? (d.HANDLING_UNIT == handlingUnit) : ((d.HANDLING_UNIT == handlingUnit) || (d.HANDLING_UNIT == null)))).OrderByDescending(o => o.MODIFIED_DATE).FirstOrDefault();
            }
        }

        public List<P_MW_RECORD_PSAC_IMAGE> GetP_MW_RECORD_PSAC_IMAGEs(string MW_RECORD_ID)
        {
            string sSql = @"SELECT RPI.*,
                                   D.DSN
                            FROM   P_MW_RECORD R
                                   JOIN P_MW_RECORD_PSAC RP
                                     ON R.UUID = RP.RECORD_ID
                                   JOIN P_MW_RECORD_PSAC_IMAGE RPI
                                     ON RP.UUID = RPI.P_MW_RECORD_PSAC_ID
                                   JOIN P_MW_DSN D
                                     ON RPI.P_DSN_ID = D.UUID
                            WHERE  R.UUID = :MW_RECORD_ID  ";

            OracleParameter[] oracleParameters = new OracleParameter[]
               {
                    new OracleParameter(":MW_RECORD_ID",MW_RECORD_ID)
               };
            return GetObjectData<P_MW_RECORD_PSAC_IMAGE>(sSql, oracleParameters).ToList();
        }

        public List<P_MW_RECORD_ITEM> GetP_MW_RECORD_ITEMs(string MW_RECORD_ID)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return db.P_MW_RECORD_ITEM.Where(d => d.MW_RECORD_ID == MW_RECORD_ID).ToList();
            }
        }

        public P_MW_RECORD_FORM_CHECKLIST GetP_MW_RECORD_FORM_CHECKLIST(string MW_RECORD_ID, string MW_VERIFICATION_ID, string handlingUnit)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return db.P_MW_RECORD_FORM_CHECKLIST.Where(d => d.MW_RECORD_ID == MW_RECORD_ID && d.MW_VERIFICATION_ID == MW_VERIFICATION_ID && (handlingUnit == ProcessingConstant.HANDLING_UNIT_SMM ? (d.HANDLING_UNIT == handlingUnit) : ((d.HANDLING_UNIT == handlingUnit) || (d.HANDLING_UNIT == null)))).FirstOrDefault();
            }
        }

        public P_MW_RECORD_FORM_CHECKLIST_PO GetP_MW_RECORD_FORM_CHECKLIST_PO(string MW_RECORD_FORM_CHECKLIST_ID, string handlingUnit)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return db.P_MW_RECORD_FORM_CHECKLIST_PO.Where(d => d.MW_RECORD_FORM_CHECKLIST_ID == MW_RECORD_FORM_CHECKLIST_ID && (handlingUnit == ProcessingConstant.HANDLING_UNIT_SMM ? (d.HANDLING_UNIT == handlingUnit) : ((d.HANDLING_UNIT == handlingUnit) || (d.HANDLING_UNIT == null)))).FirstOrDefault();
            }
        }

        public List<RecordItemCheckListItem> GetRecordItemCheckListItems(string MW_RECORD_ID, int MW_ITEM_VERSION, string MW_VERIFICATION_ID)
        {
            string sSql = @"SELECT ICI.UUID AS S_MW_ITEM_CHECKLIST_ITEM_ID,
                                       RIC.UUID AS MW_RECORD_ITEM_CHECKLIST_ID,
                                       ICI.MW_ITEM_NO,
                                       ICI.CHECKLIST_NO,
                                       ICI.DESCRIPTION,
                                       ICI.CHECKLIST_TYPE,
                                       ICI.ORDERING,
                                       RICI.ANSWER,
                                       RICI.TEXT_ANSWER,
                                       RICI.REMARKS,
                                       RICI.PO_AGREEMENT,
                                       RICI.PO_REMARK,
                                       RI.CLASS_CODE,
                                       RI.ORDERING AS CLASS_CODE_ORDERING,
                                       ICI.CODE AS ID,
                                       RICI.UUID
                                FROM   P_MW_RECORD_ITEM RI
                                       JOIN P_S_MW_ITEM_CHECKLIST_ITEM ICI
                                         ON RI.MW_ITEM_CODE = ICI.MW_ITEM_NO
                                            AND MW_ITEM_VERSION = :MW_ITEM_VERSION
                                       LEFT JOIN P_MW_RECORD_ITEM_CHECKLIST RIC
                                         ON RI.UUID = RIC.MW_RECORD_ITEM_ID AND RIC.MW_VERIFICATION_ID = :MW_VERIFICATION_ID
                                       LEFT JOIN P_MW_RECORD_ITEM_CHECKLIST_ITEM RICI
                                              ON RIC.UUID = RICI.MW_RECORD_ITEM_CHECKLIST_ID
                                                 AND RICI.S_MW_ITEM_CHECKLIST_ITEM_ID = ICI.UUID
                                WHERE  RI.MW_RECORD_ID = :MW_RECORD_ID
                                ORDER  BY ICI.MW_ITEM_NO,
                                          RI.CLASS_CODE,
                                          RI.ORDERING,
                                          ICI.ORDERING,
                                          ICI.UUID ";
            OracleParameter[] oracleParameters = new OracleParameter[]
            {
                new OracleParameter(":MW_ITEM_VERSION",MW_ITEM_VERSION),
                new OracleParameter(":MW_VERIFICATION_ID",MW_VERIFICATION_ID),
                new OracleParameter(":MW_RECORD_ID",MW_RECORD_ID)
            };
            return GetObjectData<RecordItemCheckListItem>(sSql, oracleParameters).ToList();

        }

        public List<RecordItemCheckListItem> GetFinalRecordItemCheckListItems(string MW_RECORD_ID, int MW_ITEM_VERSION)
        {
            string sSql = @"SELECT T1.*,
                                   RIC.UUID AS MW_RECORD_ITEM_CHECKLIST_ID,
                                   RICI.ANSWER,
                                   RICI.TEXT_ANSWER,
                                   RICI.REMARKS,
                                   RICI.PO_AGREEMENT,
                                   RICI.PO_REMARK
                            FROM   (SELECT ICI.UUID        AS S_MW_ITEM_CHECKLIST_ITEM_ID,
                                           RI.MW_ITEM_CODE AS MW_ITEM_NO,
                                           ICI.CHECKLIST_NO,
                                           ICI.DESCRIPTION,
                                           ICI.CHECKLIST_TYPE,
                                           ICI.ORDERING,
                                           ICI.CODE        AS ID,
                                           RI.UUID,
                                           RI.CLASS_CODE,
                                           RI.ORDERING     AS CLASS_CODE_ORDERING,
                                           RI.ORDERING     AS ITEM_ORDERING,
                                           ICI.SUBMISSION_TYPE
                                    FROM   P_MW_RECORD_ITEM RI,
                                           (SELECT *
                                            FROM   P_S_MW_ITEM_CHECKLIST_ITEM
                                            WHERE  Trim(MW_ITEM_NO) IS NULL
                                                   AND MW_ITEM_VERSION = :MW_ITEM_VERSION ) ICI
                                    WHERE  RI.MW_RECORD_ID = :MW_RECORD_ID
                                           AND RI.STATUS_CODE = 'FINAL') T1
                                   LEFT JOIN P_MW_RECORD_ITEM_CHECKLIST RIC
                                          ON T1.UUID = RIC.MW_RECORD_ITEM_ID
                                   LEFT JOIN P_MW_RECORD_ITEM_CHECKLIST_ITEM RICI
                                          ON RIC.UUID = RICI.MW_RECORD_ITEM_CHECKLIST_ID
                                             AND RICI.S_MW_ITEM_CHECKLIST_ITEM_ID = T1.S_MW_ITEM_CHECKLIST_ITEM_ID
                            ORDER  BY T1.MW_ITEM_NO,
                                      T1.CLASS_CODE,
                                      T1.ORDERING,
                                      T1.ORDERING,
                                      T1.S_MW_ITEM_CHECKLIST_ITEM_ID ";
            OracleParameter[] oracleParameters = new OracleParameter[]
            {
                new OracleParameter(":MW_ITEM_VERSION",MW_ITEM_VERSION),
                new OracleParameter(":MW_RECORD_ID",MW_RECORD_ID)
            };
            return GetObjectData<RecordItemCheckListItem>(sSql, oracleParameters).ToList();

        }

        public List<RecordItemCheckListItem> GetFinalRecordItemCheckListItems(string MW_RECORD_ID, int MW_ITEM_VERSION, EntitiesMWProcessing db)
        {
            string sSql = @"SELECT T1.*,
                                   RIC.UUID AS MW_RECORD_ITEM_CHECKLIST_ID,
                                   RICI.ANSWER,
                                   RICI.TEXT_ANSWER,
                                   RICI.REMARKS,
                                   RICI.PO_AGREEMENT,
                                   RICI.PO_REMARK
                            FROM   (SELECT ICI.UUID        AS S_MW_ITEM_CHECKLIST_ITEM_ID,
                                           RI.MW_ITEM_CODE AS MW_ITEM_NO,
                                           ICI.CHECKLIST_NO,
                                           ICI.DESCRIPTION,
                                           ICI.CHECKLIST_TYPE,
                                           ICI.ORDERING,
                                           ICI.CODE        AS ID,
                                           RI.UUID,
                                           RI.CLASS_CODE,
                                           RI.ORDERING     AS CLASS_CODE_ORDERING,
                                           RI.ORDERING     AS ITEM_ORDERING,
                                           ICI.SUBMISSION_TYPE
                                    FROM   P_MW_RECORD_ITEM RI,
                                           (SELECT *
                                            FROM   P_S_MW_ITEM_CHECKLIST_ITEM
                                            WHERE  Trim(MW_ITEM_NO) IS NULL
                                                   AND MW_ITEM_VERSION = :MW_ITEM_VERSION ) ICI
                                    WHERE  RI.MW_RECORD_ID = :MW_RECORD_ID
                                           AND RI.STATUS_CODE = 'FINAL') T1
                                   LEFT JOIN P_MW_RECORD_ITEM_CHECKLIST RIC
                                          ON T1.UUID = RIC.MW_RECORD_ITEM_ID
                                   LEFT JOIN P_MW_RECORD_ITEM_CHECKLIST_ITEM RICI
                                          ON RIC.UUID = RICI.MW_RECORD_ITEM_CHECKLIST_ID
                                             AND RICI.S_MW_ITEM_CHECKLIST_ITEM_ID = T1.S_MW_ITEM_CHECKLIST_ITEM_ID
                            ORDER  BY T1.MW_ITEM_NO,
                                      T1.CLASS_CODE,
                                      T1.ORDERING,
                                      T1.ORDERING,
                                      T1.S_MW_ITEM_CHECKLIST_ITEM_ID ";
            OracleParameter[] oracleParameters = new OracleParameter[]
            {
                new OracleParameter(":MW_ITEM_VERSION",MW_ITEM_VERSION),
                new OracleParameter(":MW_RECORD_ID",MW_RECORD_ID)
            };
            return GetObjectData<RecordItemCheckListItem>(sSql, oracleParameters, db).ToList();

        }

        public List<RecordItem> GetRecordItemsByVerificationIDAndRecordID(string MW_VERIFICATION_ID, string MW_RECORD_ID, EntitiesMWProcessing db)
        {
            string sSql = @"SELECT RI.UUID,
                                   RI.MW_RECORD_ID,
                                   RI.MW_ITEM_CODE,
                                   RIC.MW_VERIFICATION_ID AS MW_VERIFICATION_ID,
                                   RIC.UUID               AS MW_RECORD_ITEM_CHECKLIST_ID
                            FROM   P_MW_RECORD_ITEM RI
                                   LEFT JOIN P_MW_RECORD_ITEM_CHECKLIST RIC
                                          ON RI.UUID = RIC.MW_RECORD_ITEM_ID
                                             AND RIC.MW_VERIFICATION_ID = :MW_VERIFICATION_ID
                            WHERE  RI.MW_RECORD_ID = :MW_RECORD_ID ";
            OracleParameter[] oracleParameters = new OracleParameter[]
            {
                new OracleParameter(":MW_VERIFICATION_ID",MW_VERIFICATION_ID),
                new OracleParameter(":MW_RECORD_ID",MW_RECORD_ID)
            };

            return GetObjectData<RecordItem>(sSql, oracleParameters, db).ToList();

        }

        public List<RecordItem> GetFinalRecordItemsByVerificationIDAndRecordID(string MW_VERIFICATION_ID, string MW_RECORD_ID, EntitiesMWProcessing db)
        {
            string sSql = @"SELECT RI.UUID,
                                   RI.MW_RECORD_ID,
                                   RI.MW_ITEM_CODE,
                                   RIC.MW_VERIFICATION_ID AS MW_VERIFICATION_ID,
                                   RIC.UUID               AS MW_RECORD_ITEM_CHECKLIST_ID
                            FROM   P_MW_RECORD_ITEM RI
                                   LEFT JOIN P_MW_RECORD_ITEM_CHECKLIST RIC
                                          ON RI.UUID = RIC.MW_RECORD_ITEM_ID
                                             AND RIC.MW_VERIFICATION_ID = :MW_VERIFICATION_ID
                            WHERE  RI.MW_RECORD_ID = :MW_RECORD_ID AND RI.STATUS_CODE = 'FINAL'  ";
            OracleParameter[] oracleParameters = new OracleParameter[]
            {
                new OracleParameter(":MW_VERIFICATION_ID",MW_VERIFICATION_ID),
                new OracleParameter(":MW_RECORD_ID",MW_RECORD_ID)
            };

            return GetObjectData<RecordItem>(sSql, oracleParameters, db).ToList();

        }

        public List<P_S_MW_ITEM_CHECKLIST_ITEM> GetP_S_MW_ITEM_CHECKLIST_ITEMs(string MW_ITEM_NO, string DataVersion)
        {
            string sSql = @"SELECT *
                                FROM   P_S_MW_ITEM_CHECKLIST_ITEM
                                WHERE  MW_ITEM_NO = :MW_ITEM_NO
                                ORDER  BY ORDERING ";

            OracleParameter[] oracleParameters = new OracleParameter[]
            {
                    new OracleParameter(":MW_ITEM_NO",MW_ITEM_NO)
            };
            return GetObjectData<P_S_MW_ITEM_CHECKLIST_ITEM>(sSql, oracleParameters).ToList();

        }

        public List<P_S_MW_ITEM_CHECKLIST_ITEM> GetP_S_MW_ITEM_CHECKLIST_ITEMsByType(string MW_ITEM_NO, string Type, string DataVersion)
        {

            string sSql = @"SELECT *
                                FROM   P_S_MW_ITEM_CHECKLIST_ITEM
                                WHERE  MW_ITEM_NO = :MW_ITEM_NO
                                AND    CHECKLIST_TYPE = :CHECKLIST_TYPE
                                ORDER  BY ORDERING ";

            OracleParameter[] oracleParameters = new OracleParameter[]
            {
                    new OracleParameter(":MW_ITEM_NO",MW_ITEM_NO),
                    new OracleParameter(":CHECKLIST_TYPE",Type)
            };
            return GetObjectData<P_S_MW_ITEM_CHECKLIST_ITEM>(sSql, oracleParameters).ToList();

        }
        public List<P_S_MW_ITEM_CHECKBOX> GetP_S_MW_ITEM_CHECKBOXs(string ITEM_NOs, int MW_ITEM_VERSION)
        {
            string sSql = string.Format(@"SELECT *
                                          FROM   P_S_MW_ITEM_CHECKBOX
                                          WHERE  ITEM_NO IN ( {0} )
                                          AND    MW_ITEM_VERSION = {1} ", ITEM_NOs, MW_ITEM_VERSION);

            return GetObjectData<P_S_MW_ITEM_CHECKBOX>(sSql).ToList();
        }
        public List<P_S_MW_ITEM_NATURE> GetP_S_MW_ITEM_NATUREs(string ITEM_NOs, int MW_ITEM_VERSION)
        {
            //string sSql = string.Format(@"SELECT *
            //                              FROM   P_S_MW_ITEM_NATURE
            //                              WHERE  ITEM_NO IN ( {0} ) 
            //                              AND    MW_ITEM_VERSION = {1} ", ITEM_NOs, MW_ITEM_VERSION);
            string sSql = string.Format(@"SELECT *
                                          FROM   P_S_MW_ITEM_NATURE
                                          WHERE  ITEM_NO IN ( '1.1' ) 
                                          AND    MW_ITEM_VERSION = {1} ", ITEM_NOs, MW_ITEM_VERSION);

            return GetObjectData<P_S_MW_ITEM_NATURE>(sSql).ToList();
        }

        public List<P_MW_ADDRESS> GetP_MW_ADDRESSes(string MW_RECORD_ID)
        {
            string sSql = @"SELECT A.*
                            FROM   P_MW_RECORD R
                                   JOIN P_MW_RECORD_ADDRESS_INFO RAI
                                     ON R.UUID = RAI.MW_RECORD_ID
                                   JOIN P_MW_ADDRESS A
                                     ON RAI.MW_ADDRESS_ID = A.UUID
                            WHERE  R.UUID = :MW_RECORD_ID ";
            OracleParameter[] oracleParameters = new OracleParameter[]
                {
                    new OracleParameter(":MW_RECORD_ID",MW_RECORD_ID)
                };
            return GetObjectData<P_MW_ADDRESS>(sSql, oracleParameters).ToList();
        }

        public List<SelectListItem> GetSelectListItem()
        {
            string sSql = @"SELECT SV.UUID        AS Value,
                                   SV.DESCRIPTION AS Text
                            FROM   P_S_SYSTEM_VALUE SV
                                   JOIN P_S_SYSTEM_TYPE ST
                                     ON SV.SYSTEM_TYPE_ID = ST.UUID
                            WHERE  ST.Type = 'OFFENSE_TYPE' ";

            return GetObjectData<SelectListItem>(sSql).ToList();
        }

        public P_MW_RECORD_AL_FOLLOW_UP GetP_MW_RECORD_AL_FOLLOW_UPByRecordID(string MW_RECORD_ID, string handlingUnit)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return db.P_MW_RECORD_AL_FOLLOW_UP.Where(w => w.MW_RECORD_ID == MW_RECORD_ID && (handlingUnit == ProcessingConstant.HANDLING_UNIT_SMM ? (w.HANDLING_UNIT == handlingUnit) : ((w.HANDLING_UNIT == handlingUnit) || (w.HANDLING_UNIT == null)))).FirstOrDefault();
            }
        }

        public List<P_MW_RECORD_AL_FOLLOW_UP_OFFENCES> GetP_MW_RECORD_AL_FOLLOW_UP_OFFENCESByMasterID(string MASTER_ID)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return db.P_MW_RECORD_AL_FOLLOW_UP_OFFENCES.Where(w => w.MASTER_ID == MASTER_ID).ToList();
            }
        }

        public P_MW_ADDRESS GetP_MW_ADDRESSByUuid(string UUID)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return db.P_MW_ADDRESS.Where(w => w.UUID == UUID).FirstOrDefault();
            }
        }

        public List<P_MW_APPOINTED_PROFESSIONAL> GetP_MW_APPOINTED_PROFESSIONALs(string MW_RECORD_ID)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return db.P_MW_APPOINTED_PROFESSIONAL.Where(w => w.MW_RECORD_ID == MW_RECORD_ID).ToList();
            }
        }

        public P_MW_FORM GetP_MW_FORM(string MW_RECORD_ID)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return db.P_MW_FORM.Where(w => w.MW_RECORD_ID == MW_RECORD_ID).FirstOrDefault();
            }
        }

        public List<P_MW_FORM_09> GetP_MW_FORM_09s(string MW_RECORD_ID)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return db.P_MW_FORM_09.Where(w => w.MW_RECORD_ID == MW_RECORD_ID).ToList();
            }
        }

        public List<SYS_POST> GetPOListByTOID(string TOID)
        {
            string sSql = @"SELECT P.*
                            FROM   SYS_POST P
                                   JOIN P_S_SCU_TEAM ST
                                     ON P.UUID = ST.SYS_POST_ID
                                        AND ST.CHILD_SYS_POST_ID = :TOID ";
            OracleParameter[] oracleParameters = new OracleParameter[]
            {
                new OracleParameter(":TOID",TOID)
            };
            return GetObjectData<SYS_POST>(sSql, oracleParameters).ToList();
        }

        public int AddP_MW_RECORD_FORM_CHECKLIST(P_MW_RECORD_FORM_CHECKLIST model, EntitiesMWProcessing db)
        {
            db.P_MW_RECORD_FORM_CHECKLIST.Add(model);
            return db.SaveChanges();
        }

        public void SaveP_MW_RECORD_FORM_CHECKLIST(P_MW_RECORD_FORM_CHECKLIST model, string MW_RECORD_ID, string MW_VERIFICATION_ID, string FORM_CODE, string StatusCode, EntitiesMWProcessing db)
        {
            //Start modify by dive 20191023
            if (string.IsNullOrEmpty(model.UUID))
            {

                model.MW_RECORD_ID = MW_RECORD_ID;
                model.MW_VERIFICATION_ID = MW_VERIFICATION_ID;
                model.FORM_CODE = FORM_CODE;

                //Get Draft System Value

                P_S_SYSTEM_VALUE systemValue = (from sv in db.P_S_SYSTEM_VALUE
                                                join st in db.P_S_SYSTEM_TYPE
                                                on sv.SYSTEM_TYPE_ID equals st.UUID
                                                where st.TYPE == "RECORD_STATUS" && sv.CODE == StatusCode
                                                select sv).FirstOrDefault();

                model.STATUS_ID = systemValue.UUID;
                model.SSP_STATUS_ID = systemValue.UUID;
                db.P_MW_RECORD_FORM_CHECKLIST.Add(model);
                db.SaveChanges();
            }


            P_MW_RECORD_FORM_CHECKLIST record = db.P_MW_RECORD_FORM_CHECKLIST.Where(d => d.UUID == model.UUID).FirstOrDefault();

            //if (record == null)
            //{
            //    record = db.P_MW_RECORD_FORM_CHECKLIST.Where(d => d.MW_RECORD_ID == MW_RECORD_ID && d.MW_VERIFICATION_ID == MW_VERIFICATION_ID).FirstOrDefault();
            //}

            //// Handle Obj null reference
            //if (record == null)
            //{
            //    record = new P_MW_RECORD_FORM_CHECKLIST();
            //    record.MW_RECORD_ID = MW_RECORD_ID;
            //    record.MW_VERIFICATION_ID = MW_VERIFICATION_ID;
            //    record.FORM_CODE = FORM_CODE;

            //    //Get Draft System Value

            //    P_S_SYSTEM_VALUE systemValue = (from sv in db.P_S_SYSTEM_VALUE
            //                                    join st in db.P_S_SYSTEM_TYPE
            //                                    on sv.SYSTEM_TYPE_ID equals st.UUID
            //                                    where st.TYPE == "RECORD_STATUS" && sv.CODE == StatusCode
            //                                    select sv).FirstOrDefault();

            //    record.STATUS_ID = systemValue.UUID;
            //    record.SSP_STATUS_ID = systemValue.UUID;
            //    db.P_MW_RECORD_FORM_CHECKLIST.Add(record);
            //    db.SaveChanges();
            //}

            //End modify by dive 20191023

            record.INFO_NOT = model.INFO_NOT;
            record.INFO_NOT_RMK = model.INFO_NOT_RMK;

            record.INFO_DATE = model.INFO_DATE;
            record.INFO_DATE_RMK = model.INFO_DATE_RMK;

            record.INFO_OTHER = model.INFO_OTHER;
            record.INFO_OTHER_RMK = model.INFO_OTHER_RMK;

            record.FORM05_WORK_RELATED = model.FORM05_WORK_RELATED;
            record.FORM05_WORK_RELATED_RMK = model.FORM05_WORK_RELATED_RMK;

            record.FORM05_LOCATION_VALID = model.FORM05_LOCATION_VALID;
            record.FORM05_LOCATION_VALID_RMK = model.FORM05_LOCATION_VALID_RMK;

            record.APPLICANT_DETAIL_VALID = model.APPLICANT_DETAIL_VALID;
            record.APPLICANT_DETAIL_VALID_RMK = model.APPLICANT_DETAIL_VALID_RMK;

            record.SIGNBOARD_DETAIL_VALID = model.SIGNBOARD_DETAIL_VALID;
            record.SIGNBOARD_DETAIL_VALID_RMK = model.SIGNBOARD_DETAIL_VALID_RMK;

            record.INFO_AP_RI = model.INFO_AP_RI;
            record.INFO_AP_RI_RMK = model.INFO_AP_RI_RMK;

            record.AP_DETAIL_VALID = model.AP_DETAIL_VALID;
            record.AP_DETAIL_VALID_RMK = model.AP_DETAIL_VALID_RMK;

            if (!string.IsNullOrEmpty(model.RSE_DETAIL_VALID))
            {
                record.RSE_DETAIL_VALID = model.RSE_DETAIL_VALID;
            }
            record.RSE_DETAIL_VALID_RMK = model.RSE_DETAIL_VALID_RMK;

            if (!string.IsNullOrEmpty(model.RGE_DETAIL_VALID))
            {
                record.RGE_DETAIL_VALID = model.RGE_DETAIL_VALID;
            }
            record.RGE_DETAIL_VALID_RMK = model.RGE_DETAIL_VALID_RMK;

            record.PRC_DETAIL_VALID = model.PRC_DETAIL_VALID;
            record.PRC_DETAIL_VALID_RMK = model.PRC_DETAIL_VALID_RMK;

            record.PBP_AP_INFO_NAME = model.PBP_AP_INFO_NAME;
            record.PBP_RSE_INFO_NAME = model.PBP_RSE_INFO_NAME;
            record.PBP_RGE_INFO_NAME = model.PBP_RGE_INFO_NAME;

            record.PBP_AP_VALID = model.PBP_AP_VALID;
            record.PBP_AP_VALID_RMK = model.PBP_AP_VALID_RMK;


            record.PBP_AP_SIGN = model.PBP_AP_SIGN;
            record.PBP_AP_SIGN_RMK = model.PBP_AP_SIGN_RMK;

            record.PBP_AP_SSP = model.PBP_AP_SSP;
            record.PBP_AP_SSP_RMK = model.PBP_AP_SSP_RMK;

            if (!string.IsNullOrEmpty(model.PBP_AP_SIGNATURE_DATE))
            {
                record.PBP_AP_SIGNATURE_DATE = model.PBP_AP_SIGNATURE_DATE;
            }
            record.PBP_AP_SIGNATURE_DATE_RMK = model.PBP_AP_SIGNATURE_DATE_RMK;

            record.PBP_RSE_VALID = model.PBP_RSE_VALID;
            record.PBP_RSE_VALID_RMK = model.PBP_RSE_VALID_RMK;

            record.PBP_RSE_SIGN = model.PBP_RSE_SIGN;
            record.PBP_RSE_SIGN_RMK = model.PBP_RSE_SIGN_RMK;

            if (!string.IsNullOrEmpty(model.PBP_RSE_SIGNATURE_DATE))
            {
                record.PBP_RSE_SIGNATURE_DATE = model.PBP_RSE_SIGNATURE_DATE;
            }
            record.PBP_RSE_SIGNATURE_DATE_RMK = model.PBP_RSE_SIGNATURE_DATE_RMK;

            if (!string.IsNullOrEmpty(model.PBP_RGE_VALID))
            {
                record.PBP_RGE_VALID = model.PBP_RGE_VALID;
            }
            record.PBP_RGE_VALID_RMK = model.PBP_RGE_VALID_RMK;

            if (!string.IsNullOrEmpty(model.PBP_RGE_SIGN))
            {
                record.PBP_RGE_SIGN = model.PBP_RGE_SIGN;
            }
            record.PBP_RGE_SIGN_RMK = model.PBP_RGE_SIGN_RMK;

            if (!string.IsNullOrEmpty(model.PBP_RGE_SIGNATURE_DATE))
            {
                record.PBP_RGE_SIGNATURE_DATE = model.PBP_RGE_SIGNATURE_DATE;
            }
            record.PBP_RGE_SIGNATURE_DATE_RMK = model.PBP_RGE_SIGNATURE_DATE_RMK;

            if (!string.IsNullOrEmpty(model.PBP_OTHER))
            {
                record.PBP_OTHER = model.PBP_OTHER;
            }
            record.PBP_OTHER_RMK = model.PBP_OTHER_RMK;

            record.PRC_INFO_NAME = model.PRC_INFO_NAME;
            record.PRC_INFO_AS_NAME = model.PRC_INFO_AS_NAME;

            record.PRC_VALID = model.PRC_VALID;
            record.PRC_VALID_RMK = model.PRC_VALID_RMK;

            record.PRC_CAP = model.PRC_CAP;
            record.PRC_CAP_RMK = model.PRC_CAP_RMK;

            record.PRC_AS_VALID = model.PRC_AS_VALID;
            record.PRC_AS_VALID_RMK = model.PRC_AS_VALID_RMK;

            record.PRC_AS_SIGN = model.PRC_AS_SIGN;
            record.PRC_AS_SIGN_RMK = model.PRC_AS_SIGN_RMK;

            record.PRC_AS_OTHER = model.PRC_AS_OTHER;
            record.PRC_AS_OTHER_RMK = model.PRC_AS_OTHER_RMK;

            if (!string.IsNullOrEmpty(model.PRC_SIGNATURE_DATE))
            {
                record.PRC_SIGNATURE_DATE = model.PRC_SIGNATURE_DATE;
            }
            record.PRC_SIGNATURE_DATE_RMK = model.PRC_SIGNATURE_DATE_RMK;

            record.PRC_OTHER = model.PRC_OTHER;
            record.PRC_OTHER_RMK = model.PRC_OTHER_RMK;

            record.RGBC_MWC_INFO_NAME = model.RGBC_MWC_INFO_NAME;
            record.RGBC_MWC_INFO_AS_NAME = model.RGBC_MWC_INFO_AS_NAME;

            record.FORM6_AP_CAP = model.FORM6_AP_CAP;
            record.FORM6_AP_CAP_RMK = model.FORM6_AP_CAP_RMK;

            if (!string.IsNullOrEmpty(model.FORM6_AP_AS_VALID))
            {
                record.FORM6_AP_AS_VALID = model.FORM6_AP_AS_VALID;
            }
            record.FORM6_AP_AS_VALID_RMK = model.FORM6_AP_AS_VALID_RMK;

            record.FORM6_AP_AS_SIGN = model.FORM6_AP_AS_SIGN;
            record.FORM6_AP_AS_SIGN_RMK = model.FORM6_AP_AS_SIGN_RMK;

            if (!string.IsNullOrEmpty(model.FORM6_AP_AS_OTHER))
            {
                record.FORM6_AP_AS_OTHER = model.FORM6_AP_AS_OTHER;
            }
            record.FORM6_AP_AS_OTHER_RMK = model.FORM6_AP_AS_OTHER_RMK;

            record.FORM07_DEC_S27 = model.FORM07_DEC_S27;
            record.FORM07_DEC_S27_RMK = model.FORM07_DEC_S27_RMK;

            record.DATE_VALID = model.DATE_VALID;
            record.DATE_VALID_RMK = model.DATE_VALID_RMK;

            record.FORM09_REASON = model.FORM09_REASON;
            record.FORM09_REASON_RMK = model.FORM09_REASON_RMK;

            record.FORM09_ACTING_PERIOD = model.FORM09_ACTING_PERIOD;
            record.FORM09_ACTING_PERIOD_RMK = model.FORM09_ACTING_PERIOD_RMK;

            record.PBP_NEW_AP_VALID = model.PBP_NEW_AP_VALID;
            record.PBP_NEW_AP_VALID_RMK = model.PBP_NEW_AP_VALID_RMK;

            record.PBP_NEW_AP_SIGN = model.PBP_NEW_AP_SIGN;
            record.PBP_NEW_AP_SIGN_RMK = model.PBP_NEW_AP_SIGN_RMK;

            record.PBP_NEW_SIGNATURE_DATE = model.PBP_NEW_SIGNATURE_DATE;
            record.PBP_NEW_SIGNATURE_DATE_RMK = model.PBP_NEW_SIGNATURE_DATE_RMK;

            record.INFO_NOT_PRC_CLASS1 = model.INFO_NOT_PRC_CLASS1;
            record.INFO_NOT_PRC_CLASS1_RMK = model.INFO_NOT_PRC_CLASS1_RMK;

            record.APPLICANT_SIGN_VALID = model.APPLICANT_SIGN_VALID;
            record.APPLICANT_SIGN_VALID_RMK = model.APPLICANT_SIGN_VALID_RMK;

            record.PRC_DEC2 = model.PRC_DEC2;
            record.PRC_DEC2_RMK = model.PRC_DEC2_RMK;

            record.FORM32_WORK_ITEMS = model.FORM32_WORK_ITEMS;
            record.FORM32_WORK_ITEMS_RMK = model.FORM32_WORK_ITEMS_RMK;

            record.FORM07_DEC_S48_2 = model.FORM07_DEC_S48_2;
            record.FORM07_DEC_S48_2_RMK = model.FORM07_DEC_S48_2_RMK;

            record.FORM07_DEC_S48_4 = model.FORM07_DEC_S48_4;
            record.FORM07_DEC_S48_4_RMK = model.FORM07_DEC_S48_4_RMK;

            record.PBP_AP_DEC1 = model.PBP_AP_DEC1;
            record.PBP_AP_DEC1_RMK = model.PBP_AP_DEC1_RMK;

            db.SaveChanges();

        }

        public void AddP_MW_RECORD_PSAC(P_MW_RECORD_PSAC model)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                db.P_MW_RECORD_PSAC.Add(model);
                db.SaveChanges();
            }
        }

        public void SaveP_MW_RECORD_PSAC(P_MW_RECORD_PSAC model, EntitiesMWProcessing db)
        {
            if (string.IsNullOrEmpty(model.UUID))
            {
                db.P_MW_RECORD_PSAC.Add(model);
            }
            else
            {
                P_MW_RECORD_PSAC record = db.P_MW_RECORD_PSAC.Where(d => d.UUID == model.UUID).FirstOrDefault();

                record.RECEIPT_DATE = model.RECEIPT_DATE;
                record.INSPECTION_DATE = model.INSPECTION_DATE;

                record.CHK_INACCESSIBLE = model.CHK_INACCESSIBLE;
                record.CHK_MW_COMM_AND_COMP = model.CHK_MW_COMM_AND_COMP;
                record.CHK_MW_IN_PROGRESS = model.CHK_MW_IN_PROGRESS;
                record.CHK_MW_NOT_YET_COMM = model.CHK_MW_NOT_YET_COMM;

                record.BS_NAME = model.BS_NAME;
                record.BS_POST = model.BS_POST;
                record.BS_SIGNATURE_DATE = model.BS_SIGNATURE_DATE;

                record.SO_NAME = model.SO_NAME;
                record.SO_POST = model.SO_POST;
                record.SO_SIGNATURE_DATE = model.SO_SIGNATURE_DATE;
            }

            db.SaveChanges();

        }

        public void SaveP_MW_RECORD_SAC(P_MW_RECORD_SAC model, EntitiesMWProcessing db)
        {
            if (string.IsNullOrEmpty(model.UUID))
            {
                db.P_MW_RECORD_SAC.Add(model);
            }
            else
            {
                P_MW_RECORD_SAC record = db.P_MW_RECORD_SAC.Where(d => d.UUID == model.UUID).FirstOrDefault();

                record.INSPECTION_DATE = model.INSPECTION_DATE;
                record.CHK_COMMENCEMENT = model.CHK_COMMENCEMENT;
                record.CHK_COMPLETION = model.CHK_COMPLETION;
                record.DESK_STUDY_A_CORR_MW_ITEM = model.DESK_STUDY_A_CORR_MW_ITEM;
                record.DESK_STUDY_B_LAST_INSP_RECORD = model.DESK_STUDY_B_LAST_INSP_RECORD;
                record.SITE_INSAP_A_COMPATIBLE_SITE_INSP = model.SITE_INSAP_A_COMPATIBLE_SITE_INSP;
                record.SITE_INSAP_B_ADEQUATE_PRECAUTION = model.SITE_INSAP_B_ADEQUATE_PRECAUTION;
                record.SITE_INSAP_C_NO_OBVIOUS_CONTRAVENTION = model.SITE_INSAP_C_NO_OBVIOUS_CONTRAVENTION;
                record.SITE_INSAP_D_COMPATIBLE_SITE_INSP = model.SITE_INSAP_D_COMPATIBLE_SITE_INSP;
                record.SITE_INSAP_E_NO_MATERIAL_DEVIATION = model.SITE_INSAP_E_NO_MATERIAL_DEVIATION;
                record.SITE_INSAP_F_NO_CONTRAVENTION = model.SITE_INSAP_F_NO_CONTRAVENTION;
                record.ACTION_TAKEN_CHK_NA = model.ACTION_TAKEN_CHK_NA;
                record.ACTION_TAKEN_CHK_INFORM = model.ACTION_TAKEN_CHK_INFORM;
                record.ACTION_TAKEN_CHK_INFORM_PBP = model.ACTION_TAKEN_CHK_INFORM_PBP;
                record.ACTION_TAKEN_CHK_INFORM_PRC = model.ACTION_TAKEN_CHK_INFORM_PRC;
                record.ACTION_TAKEN_INFORM_DATE = model.ACTION_TAKEN_INFORM_DATE;
                record.ACTION_TAKEN_CHK_INFORM_PHONE = model.ACTION_TAKEN_CHK_INFORM_PHONE;
                record.ACTION_TAKEN_CHK_INFORM_LETTER = model.ACTION_TAKEN_CHK_INFORM_LETTER;
                record.ACTION_TAKEN_CHK_IRR = model.ACTION_TAKEN_CHK_IRR;
                record.ACTION_TAKEN_CHK_IRR_PBP = model.ACTION_TAKEN_CHK_IRR_PBP;
                record.ACTION_TAKEN_CHK_IRR_PRC = model.ACTION_TAKEN_CHK_IRR_PRC;
                record.ACTION_TAKEN_CHK_IRR_COMPLETE = model.ACTION_TAKEN_CHK_IRR_COMPLETE;
                record.ACTION_TAKEN_CHK_IRR_PARTLY = model.ACTION_TAKEN_CHK_IRR_PARTLY;
                record.ACTION_TAKEN_IRR_DATE = model.ACTION_TAKEN_IRR_DATE;
                record.IRR_NOT_RECTIFIED_CHK_NA = model.IRR_NOT_RECTIFIED_CHK_NA;
                record.IRR_NOT_RECTIFIED_CHK_ID = model.IRR_NOT_RECTIFIED_CHK_ID;
                record.IRR_NOT_RECTIFIED_CHK_NON_ID = model.IRR_NOT_RECTIFIED_CHK_NON_ID;
                record.IRR_NOT_RECTIFIED_CHK_OTHER = model.IRR_NOT_RECTIFIED_CHK_OTHER;
                record.IRR_NOT_RECTIFIED_OTHER = model.IRR_NOT_RECTIFIED_OTHER;
                record.IRR_NOT_RECTIFIED_CHK_MW = model.IRR_NOT_RECTIFIED_CHK_MW;
                record.IRR_NOT_RECTIFIED_CHK_MW_DISCIPLINARY = model.IRR_NOT_RECTIFIED_CHK_MW_DISCIPLINARY;
                record.IRR_NOT_RECTIFIED_CHK_MW_PROSECUTION = model.IRR_NOT_RECTIFIED_CHK_MW_PROSECUTION;
                record.IRR_NOT_RECTIFIED_CHK_MW_PBP = model.IRR_NOT_RECTIFIED_CHK_MW_PBP;
                record.IRR_NOT_RECTIFIED_CHK_MW_PRC = model.IRR_NOT_RECTIFIED_CHK_MW_PRC;
                record.IRR_NOT_RECTIFIED_CHK_OS_IRR = model.IRR_NOT_RECTIFIED_CHK_OS_IRR;
                record.IRR_NOT_RECTIFIED_CHK_OS_DISCIPLINARY = model.IRR_NOT_RECTIFIED_CHK_OS_DISCIPLINARY;
                record.IRR_NOT_RECTIFIED_CHK_OS_PROSECUTION = model.IRR_NOT_RECTIFIED_CHK_OS_PROSECUTION;
                record.IRR_NOT_RECTIFIED_CHK_OS_PBP = model.IRR_NOT_RECTIFIED_CHK_OS_PBP;
                record.IRR_NOT_RECTIFIED_CHK_OS_PRC = model.IRR_NOT_RECTIFIED_CHK_OS_PRC;
                record.UNIT_HEAD_ENDORSEMENT_DATE = model.UNIT_HEAD_ENDORSEMENT_DATE;
                record.INSP_OFFICER_1_DATE = model.INSP_OFFICER_1_DATE;
                record.INSP_OFFICER_2_DATE = model.INSP_OFFICER_2_DATE;
                record.INSP_OFFICER_1_NAME = model.INSP_OFFICER_1_NAME;
                record.INSP_OFFICER_2_NAME = model.INSP_OFFICER_2_NAME;


            }
            db.SaveChanges();
        }

        public void SaveP_MW_RECORD_PSAC_IMAGEs(List<P_MW_RECORD_ITEM_CHECKLIST_ITEM> models, EntitiesMWProcessing db)
        {

        }

        public int RemoveP_MW_RECORD_ITEM_CHECKLIST_ITEMs(string[] MW_RECORD_ITEM_CHECKLIST_IDs, EntitiesMWProcessing db)
        {
            foreach (string sID in MW_RECORD_ITEM_CHECKLIST_IDs)
            {
                var records = db.P_MW_RECORD_ITEM_CHECKLIST_ITEM.Where(w => w.MW_RECORD_ITEM_CHECKLIST_ID == sID).ToList();
                if (records != null && records.Count > 0)
                {
                    db.P_MW_RECORD_ITEM_CHECKLIST_ITEM.RemoveRange(db.P_MW_RECORD_ITEM_CHECKLIST_ITEM.Where(w => w.MW_RECORD_ITEM_CHECKLIST_ID == sID));
                }

            }
            return db.SaveChanges();
        }

        public void SaveP_MW_RECORD_ITEM_CHECKLIST_ITEMs(List<P_MW_RECORD_ITEM_CHECKLIST_ITEM> P_MW_RECORD_ITEM_CHECKLIST_ITEMs, EntitiesMWProcessing db)
        {
            //Delete 
            string[] MW_RECORD_ITEM_CHECKLIST_IDs = (from l in P_MW_RECORD_ITEM_CHECKLIST_ITEMs select l.MW_RECORD_ITEM_CHECKLIST_ID).Distinct().ToArray();
            RemoveP_MW_RECORD_ITEM_CHECKLIST_ITEMs(MW_RECORD_ITEM_CHECKLIST_IDs, db);

            foreach (var item in P_MW_RECORD_ITEM_CHECKLIST_ITEMs)
            {
                db.P_MW_RECORD_ITEM_CHECKLIST_ITEM.Add(item);
            }

            db.SaveChanges();
        }

        public void SaveP_MW_SUMMARY_MW_ITEM_CHECKLIST(P_MW_SUMMARY_MW_ITEM_CHECKLIST model, EntitiesMWProcessing db)
        {
            if (string.IsNullOrEmpty(model.UUID))
            {
                db.P_MW_SUMMARY_MW_ITEM_CHECKLIST.Add(model);
            }
            else
            {
                P_MW_SUMMARY_MW_ITEM_CHECKLIST record = db.P_MW_SUMMARY_MW_ITEM_CHECKLIST.Where(w => w.UUID == model.UUID).FirstOrDefault();
                record.RECOMMEDATION_APPLICATION = model.RECOMMEDATION_APPLICATION;
                //record.GROUNDS_OF_REFUSAL = model.GROUNDS_OF_REFUSAL;
                //record.GROUNDS_OF_CONDITIONAL = model.GROUNDS_OF_CONDITIONAL;
                record.SL_MWG02_REQUIRED = model.SL_MWG02_REQUIRED;
                record.CHANGE_PREVIOUS_FORM_STATUS = model.CHANGE_PREVIOUS_FORM_STATUS;
                record.REMARK = model.REMARK;

            }
            db.SaveChanges();

        }

        public void AddP_MW_ADDRESS(P_MW_ADDRESS model)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                db.P_MW_ADDRESS.Add(model);
                db.SaveChanges();
            }
        }

        public void AddP_MW_RECORD_ADDRESS_INFO(P_MW_RECORD_ADDRESS_INFO model)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                db.P_MW_RECORD_ADDRESS_INFO.Add(model);
                db.SaveChanges();
            }
        }

        public void DeleteP_MW_RECORD_ADDRESS_INFO(P_MW_RECORD_ADDRESS_INFO model)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                var record = db.P_MW_RECORD_ADDRESS_INFO.Where(w => w.MW_RECORD_ID == model.MW_RECORD_ID && w.MW_ADDRESS_ID == model.MW_ADDRESS_ID).FirstOrDefault();

                if (record != null)
                {
                    db.P_MW_RECORD_ADDRESS_INFO.Remove(record);
                }
                db.SaveChanges();
            }
        }

        public void AddP_MW_RECORD_PSAC_IMAGE(P_MW_RECORD_PSAC_IMAGE model)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                db.P_MW_RECORD_PSAC_IMAGE.Add(model);
                db.SaveChanges();
            }
        }

        public void DeleteP_MW_RECORD_PSAC_IMAGE(P_MW_RECORD_PSAC_IMAGE model)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                db.P_MW_RECORD_PSAC_IMAGE.Remove(db.P_MW_RECORD_PSAC_IMAGE.Where(w => w.UUID == model.UUID).FirstOrDefault());
                db.SaveChanges();
            }
        }

        public void SaveP_MW_SCANNED_DOCUMENTs(List<P_MW_SCANNED_DOCUMENT> models)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                foreach (var model in models)
                {
                    P_MW_SCANNED_DOCUMENT record = db.P_MW_SCANNED_DOCUMENT.Where(w => w.UUID == model.UUID).FirstOrDefault();
                    record.FOLDER_TYPE = model.FOLDER_TYPE;
                }
                db.SaveChanges();
            }
        }

        public int UpdateSPO(P_MW_RECORD p_MW_RECORD, P_MW_RECORD_FORM_CHECKLIST p_MW_RECORD_FORM_CHECKLIST)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                P_MW_RECORD mwRecord = db.P_MW_RECORD.Where(w => w.UUID == p_MW_RECORD.UUID).FirstOrDefault();

                mwRecord.SPO_COMPLETE = "Y";

                if (string.IsNullOrEmpty(p_MW_RECORD_FORM_CHECKLIST.UUID))
                {
                    P_S_SYSTEM_VALUE systemValue = (from sv in db.P_S_SYSTEM_VALUE
                                                    join st in db.P_S_SYSTEM_TYPE
                                                    on sv.SYSTEM_TYPE_ID equals st.UUID
                                                    where st.TYPE == "RECORD_STATUS" && sv.CODE == ProcessingConstant.S_VAL_DRAFT
                                                    select sv).FirstOrDefault();

                    p_MW_RECORD_FORM_CHECKLIST.STATUS_ID = systemValue.UUID;
                    p_MW_RECORD_FORM_CHECKLIST.SSP_STATUS_ID = systemValue.UUID;

                    p_MW_RECORD_FORM_CHECKLIST.MW_RECORD_ID = mwRecord.UUID;
                    p_MW_RECORD_FORM_CHECKLIST.FORM_CODE = mwRecord.S_FORM_TYPE_CODE;

                    db.P_MW_RECORD_FORM_CHECKLIST.Add(p_MW_RECORD_FORM_CHECKLIST);
                }
                else
                {
                    P_MW_RECORD_FORM_CHECKLIST formChecklist = db.P_MW_RECORD_FORM_CHECKLIST.Where(w => w.UUID == p_MW_RECORD_FORM_CHECKLIST.UUID).FirstOrDefault();
                    formChecklist.PBP_AP_SSP_SPO = p_MW_RECORD_FORM_CHECKLIST.PBP_AP_SSP_SPO;
                    formChecklist.PBP_AP_SSP_SPO_RMK = p_MW_RECORD_FORM_CHECKLIST.PBP_AP_SSP_SPO_RMK;
                }

                return db.SaveChanges();
            }
        }

        public int AddP_MW_RECORD_ITEM_CHECKLIST(P_MW_RECORD_ITEM_CHECKLIST model, EntitiesMWProcessing db)
        {
            db.P_MW_RECORD_ITEM_CHECKLIST.Add(model);
            return db.SaveChanges();
        }

        public int RemoveP_MW_RECORD_ITEM_CHECKLISTByItemIDAndVerificationID(string MW_RECORD_ITEM_ID, string MW_VERIFICATION_ID, EntitiesMWProcessing db)
        {
            P_MW_RECORD_ITEM_CHECKLIST record = db.P_MW_RECORD_ITEM_CHECKLIST.Where(w => w.MW_RECORD_ITEM_ID == MW_RECORD_ITEM_ID && w.MW_VERIFICATION_ID == MW_VERIFICATION_ID).FirstOrDefault();

            if (record != null)
            {
                db.P_MW_RECORD_ITEM_CHECKLIST.Remove(record);
            }

            return db.SaveChanges();
        }

        public P_S_SYSTEM_VALUE GetSystemValueByTypeAndCode(string type, string code, EntitiesMWProcessing db)
        {
            P_S_SYSTEM_VALUE record = (from sSystemValue in db.P_S_SYSTEM_VALUE
                                       join sSystemType in db.P_S_SYSTEM_TYPE on sSystemValue.SYSTEM_TYPE_ID equals sSystemType.UUID
                                       where 1 == 1
                                       && sSystemType.TYPE == type
                                       && sSystemValue.CODE == code
                                       && sSystemValue.IS_ACTIVE == ProcessingConstant.FLAG_Y
                                       select sSystemValue).FirstOrDefault();
            return record;
        }

        public int UpdateP_MW_RECORD_ITEM_CHECKLIST(string UUID, string VARIATION_DECLARED, EntitiesMWProcessing db)
        {
            P_MW_RECORD_ITEM_CHECKLIST record = db.P_MW_RECORD_ITEM_CHECKLIST.Where(w => w.UUID == UUID).FirstOrDefault();
            record.VARIATION_DECLARED = VARIATION_DECLARED;
            return db.SaveChanges();
        }

        public void SaveP_MW_RECORD_ITEM_INFO(List<PreMwItem> PreMwItems, EntitiesMWProcessing db)
        {
            foreach (PreMwItem item in PreMwItems)
            {
                P_MW_RECORD_ITEM_INFO record = db.P_MW_RECORD_ITEM_INFO.Where(w => w.ORIENTAL_ITEM_ID == item.FianlItemUUID).FirstOrDefault();
                if (record != null)
                {
                    db.P_MW_RECORD_ITEM_INFO.Remove(record);
                }


                if (item.IsMatchItem)
                {
                    db.P_MW_RECORD_ITEM_INFO.Add(new P_MW_RECORD_ITEM_INFO()
                    {
                        ORIENTAL_ITEM_ID = item.FianlItemUUID,
                        REVISED_ITEM_ID = item.REVISED_ITEM_ID
                    });
                }
                db.SaveChanges();
            }
        }

        public ServiceResult SaveAndNext(P_MW_RECORD P_MW_RECORD, P_MW_RECORD_FORM_CHECKLIST P_MW_RECORD_FORM_CHECKLIST, P_MW_RECORD_PSAC P_MW_RECORD_PSAC, List<P_MW_RECORD_ITEM_CHECKLIST_ITEM> P_MW_RECORD_ITEM_CHECKLIST_ITEMs, P_MW_RECORD_SAC P_MW_RECORD_SAC, List<RecordItemCheckListItem> RecordItemCheckListItems, string MW_VERIFICATION_ID, List<PreRecordItemChecklist> PreRecordItemChecklists, List<PreMwItem> PreMwItems, List<RecordItemCheckListItem> FinalRecordItemCheckListItems, bool IsSubmit, string handlingUnit)
        {
            ServiceResult serviceResult = new ServiceResult();
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        P_MW_RECORD mwRecord = GetObjectRecordByUuid<P_MW_RECORD>(P_MW_RECORD.UUID, db);

                        string statysCode = "";

                        if (IsSubmit)
                        {
                            statysCode = "SUBMIT";
                        }
                        else
                        {
                            statysCode = "DRAFT";
                        }

                        P_MW_RECORD_FORM_CHECKLIST.HANDLING_UNIT = handlingUnit;
                        SaveP_MW_RECORD_FORM_CHECKLIST(P_MW_RECORD_FORM_CHECKLIST, P_MW_RECORD.UUID, MW_VERIFICATION_ID, P_MW_RECORD.S_FORM_TYPE_CODE, statysCode, db);

                        if (P_MW_RECORD_PSAC != null)
                        {
                            P_MW_RECORD_PSAC.RECORD_ID = P_MW_RECORD.UUID;
                            P_MW_RECORD_PSAC.HANDLING_UNIT = handlingUnit;

                            P_MW_RECORD_PSAC psacRecord = GetP_MW_RECORD_PSAC(P_MW_RECORD.UUID, handlingUnit);
                            if (psacRecord != null)
                            {
                                P_MW_RECORD_PSAC.UUID = psacRecord.UUID;
                            }
                            SaveP_MW_RECORD_PSAC(P_MW_RECORD_PSAC, db);
                        }

                        //Get Item No
                        List<RecordItem> recordItems = GetRecordItemsByVerificationIDAndRecordID(MW_VERIFICATION_ID, P_MW_RECORD.UUID, db);

                        //Start modify by dive 20191105
                        //Distinguish PEM SMM
                        //Get signboard item
                        P_S_SYSTEM_VALUE svSignBoardItem = GetSystemValueByTypeAndCode(ProcessingConstant.TYPE_S_MW_ITEM,
                                            ProcessingConstant.CODE_SIGNBOARD_ITEMS, db);
                        string[] SignboardItemList = svSignBoardItem.DESCRIPTION.Split(',');

                        bool isSignboard = ProcessingConstant.HANDLING_UNIT_SMM.Equals(handlingUnit);

                        for (int i = 0; i < recordItems.Count(); i++)
                        {
                            if ((isSignboard && !SignboardItemList.Contains(recordItems[i].MW_ITEM_CODE)) || (!isSignboard && SignboardItemList.Contains(recordItems[i].MW_ITEM_CODE)))
                            {
                                recordItems.Remove(recordItems[i]);
                            }
                        }

                        //End modify by dive

                        P_S_SYSTEM_VALUE systemValue = GetSystemValueByTypeAndCode("RECORD_STATUS", "DRAFT", db);

                        //Default SubmittionType
                        string submissionType = mwRecord.MW_PROGRESS_STATUS_CODE;

                        //Check FianRecordItem

                        //Check Form Code
                        if (mwRecord.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_10) || mwRecord.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_31))
                        {
                            submissionType = ProcessingConstant.FORM_10;
                        }

                        foreach (RecordItem item in recordItems)
                        {

                            if (string.IsNullOrEmpty(item.MW_RECORD_ITEM_CHECKLIST_ID))
                            {
                                P_MW_RECORD_ITEM_CHECKLIST p_MW_RECORD_ITEM_CHECKLIST = new P_MW_RECORD_ITEM_CHECKLIST();
                                p_MW_RECORD_ITEM_CHECKLIST.MW_RECORD_ITEM_ID = item.UUID;
                                p_MW_RECORD_ITEM_CHECKLIST.MW_VERIFICATION_ID = MW_VERIFICATION_ID;
                                p_MW_RECORD_ITEM_CHECKLIST.STATUS_ID = systemValue.UUID;
                                p_MW_RECORD_ITEM_CHECKLIST.SUBMISSION = submissionType;
                                p_MW_RECORD_ITEM_CHECKLIST.HANDLING_UNIT = handlingUnit;
                                //Final Item
                                if (mwRecord.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_02) || mwRecord.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_04) || mwRecord.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_10))
                                {
                                    p_MW_RECORD_ITEM_CHECKLIST.VARIATION_DECLARED = PreRecordItemChecklists.Where(w => w.MW_ITEM_CODE == item.MW_ITEM_CODE).FirstOrDefault().VARIATION_DECLARED;
                                }

                                AddP_MW_RECORD_ITEM_CHECKLIST(p_MW_RECORD_ITEM_CHECKLIST, db);

                                item.MW_RECORD_ITEM_CHECKLIST_ID = p_MW_RECORD_ITEM_CHECKLIST.UUID;


                            }
                            else
                            {
                                //Update P_MW_RECORD_ITEM_CHECKLIST VARIATION_DECLARED
                                if (mwRecord.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_02) || mwRecord.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_04) || mwRecord.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_10))
                                {
                                    PreRecordItemChecklist preRecordItemChecklist = PreRecordItemChecklists.Where(w => w.MW_ITEM_CODE == item.MW_ITEM_CODE).FirstOrDefault();

                                    UpdateP_MW_RECORD_ITEM_CHECKLIST(item.MW_RECORD_ITEM_CHECKLIST_ID, preRecordItemChecklist.VARIATION_DECLARED, db);
                                }
                            }

                            RecordItemCheckListItems.Where(w => w.MW_ITEM_NO == item.MW_ITEM_CODE).ToList().ForEach(i => i.MW_RECORD_ITEM_CHECKLIST_ID = item.MW_RECORD_ITEM_CHECKLIST_ID);
                        }

                        foreach (var item in RecordItemCheckListItems)
                        {
                            P_MW_RECORD_ITEM_CHECKLIST_ITEMs.Add(new P_MW_RECORD_ITEM_CHECKLIST_ITEM()
                            {
                                S_MW_ITEM_CHECKLIST_ITEM_ID = item.S_MW_ITEM_CHECKLIST_ITEM_ID,
                                MW_RECORD_ITEM_CHECKLIST_ID = item.MW_RECORD_ITEM_CHECKLIST_ID,
                                ANSWER = item.ANSWER,
                                TEXT_ANSWER = item.ANSWER,
                                REMARKS = item.REMARKS,
                                PO_REMARK = item.PO_REMARK,
                                HANDLING_UNIT = handlingUnit
                            });
                        }


                        SaveP_MW_RECORD_ITEM_CHECKLIST_ITEMs(P_MW_RECORD_ITEM_CHECKLIST_ITEMs, db);

                        if (P_MW_RECORD_SAC != null)
                        {
                            P_MW_RECORD_SAC.RECORD_ID = P_MW_RECORD.UUID;
                            P_MW_RECORD_SAC.HANDLING_UNIT = handlingUnit;

                            P_MW_RECORD_SAC sacRecord = GetP_MW_RECORD_SAC(P_MW_RECORD.UUID, ProcessingConstant.PAGE_CODE_SAC, handlingUnit);
                            if (sacRecord != null)
                            {
                                P_MW_RECORD_SAC.UUID = sacRecord.UUID;
                            }
                            SaveP_MW_RECORD_SAC(P_MW_RECORD_SAC, db);
                        }

                        //P_MW_RECORD_ITEM_INFO
                        if (mwRecord.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_02) || mwRecord.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_04) || mwRecord.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_10))
                        {

                            //Get Final Record
                            P_MW_RECORD finalRecord = GetFinalRecordByReferenceNo(mwRecord.REFERENCE_NUMBER, db);
                            int itemVersion = 1;



                            List<RecordItemCheckListItem> referenceItemCheckListItems = GetFinalRecordItemCheckListItems(finalRecord.UUID, itemVersion, db);


                            foreach (RecordItemCheckListItem refItem in referenceItemCheckListItems)
                            {
                                foreach (RecordItemCheckListItem finalItem in FinalRecordItemCheckListItems)
                                {
                                    if (refItem.S_MW_ITEM_CHECKLIST_ITEM_ID == finalItem.S_MW_ITEM_CHECKLIST_ITEM_ID && refItem.MW_ITEM_NO == finalItem.MW_ITEM_NO)
                                    {
                                        finalItem.MW_RECORD_ITEM_CHECKLIST_ID = refItem.MW_RECORD_ITEM_CHECKLIST_ID;
                                        break;
                                    }
                                }
                            }

                            //Delete Final Item Checklist Item
                            string[] MW_RECORD_ITEM_CHECKLIST_IDs = (from l in FinalRecordItemCheckListItems select l.MW_RECORD_ITEM_CHECKLIST_ID).Distinct().ToArray();

                            RemoveP_MW_RECORD_ITEM_CHECKLIST_ITEMs(MW_RECORD_ITEM_CHECKLIST_IDs, db);


                            //Final Item CheckList
                            List<RecordItem> finalRecordItems = GetFinalRecordItemsByVerificationIDAndRecordID(MW_VERIFICATION_ID, finalRecord.UUID, db);

                            foreach (RecordItem item in finalRecordItems)
                            {
                                //Remove 
                                RemoveP_MW_RECORD_ITEM_CHECKLISTByItemIDAndVerificationID(item.UUID, MW_VERIFICATION_ID, db);

                                var preItem = (from p in PreMwItems
                                               where p.ItemCode == item.MW_ITEM_CODE
                                               select p).FirstOrDefault();

                                if (!preItem.IsMatchItem)
                                {
                                    P_MW_RECORD_ITEM_CHECKLIST p_MW_RECORD_ITEM_CHECKLIST = new P_MW_RECORD_ITEM_CHECKLIST();
                                    p_MW_RECORD_ITEM_CHECKLIST.MW_RECORD_ITEM_ID = item.UUID;
                                    p_MW_RECORD_ITEM_CHECKLIST.MW_VERIFICATION_ID = MW_VERIFICATION_ID;
                                    p_MW_RECORD_ITEM_CHECKLIST.STATUS_ID = systemValue.UUID;
                                    p_MW_RECORD_ITEM_CHECKLIST.SUBMISSION = submissionType;

                                    AddP_MW_RECORD_ITEM_CHECKLIST(p_MW_RECORD_ITEM_CHECKLIST, db);
                                    item.MW_RECORD_ITEM_CHECKLIST_ID = p_MW_RECORD_ITEM_CHECKLIST.UUID;

                                }

                                FinalRecordItemCheckListItems.Where(w => w.MW_ITEM_NO == item.MW_ITEM_CODE).ToList().ForEach(i => i.MW_RECORD_ITEM_CHECKLIST_ID = item.MW_RECORD_ITEM_CHECKLIST_ID);
                            }

                            List<P_MW_RECORD_ITEM_CHECKLIST_ITEM> FinalP_MW_RECORD_ITEM_CHECKLIST_ITEMs = new List<P_MW_RECORD_ITEM_CHECKLIST_ITEM>();

                            List<RecordItemCheckListItem> finalItemChecklists = new List<RecordItemCheckListItem>();

                            //Add Final Item checklist Item
                            finalItemChecklists = (from f in FinalRecordItemCheckListItems
                                                   join p in PreMwItems
                                                   on f.MW_ITEM_NO equals p.ItemCode
                                                   where !p.IsMatchItem
                                                   select f).ToList();

                            foreach (RecordItemCheckListItem finalItemChecklist in finalItemChecklists)
                            {
                                FinalP_MW_RECORD_ITEM_CHECKLIST_ITEMs.Add(new P_MW_RECORD_ITEM_CHECKLIST_ITEM()
                                {
                                    S_MW_ITEM_CHECKLIST_ITEM_ID = finalItemChecklist.S_MW_ITEM_CHECKLIST_ITEM_ID,
                                    MW_RECORD_ITEM_CHECKLIST_ID = finalItemChecklist.MW_RECORD_ITEM_CHECKLIST_ID,
                                    ANSWER = finalItemChecklist.ANSWER,
                                    TEXT_ANSWER = finalItemChecklist.ANSWER,
                                    REMARKS = finalItemChecklist.REMARKS,
                                    PO_REMARK = finalItemChecklist.PO_REMARK
                                });
                            }


                            SaveP_MW_RECORD_ITEM_INFO(PreMwItems, db);
                            SaveP_MW_RECORD_ITEM_CHECKLIST_ITEMs(FinalP_MW_RECORD_ITEM_CHECKLIST_ITEMs, db);
                        }


                        db.SaveChanges();
                        tran.Commit();
                    }
                    catch (DbEntityValidationException validationError)
                    {
                        var errorMessages = validationError.EntityValidationErrors
                        .SelectMany(validationResult => validationResult.ValidationErrors)
                        .Select(m => m.ErrorMessage);
                        Console.WriteLine("Error :" + validationError.Message);
                        return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { validationError.Message } };
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        Console.WriteLine("Error :" + ex.Message);
                        return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { ex.Message } };
                    }
                    return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
                }

            }
        }

        public ServiceResult SaveSummary(P_MW_SUMMARY_MW_ITEM_CHECKLIST P_MW_SUMMARY_MW_ITEM_CHECKLIST)
        {
            ServiceResult serviceResult = new ServiceResult();
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    try
                    {

                        SaveP_MW_SUMMARY_MW_ITEM_CHECKLIST(P_MW_SUMMARY_MW_ITEM_CHECKLIST, db);

                        //MW33 Updaet all wm form RECOMMEDATION_APPLICATION
                        if (P_MW_SUMMARY_MW_ITEM_CHECKLIST.CHANGE_PREVIOUS_FORM_STATUS == "Y")
                        {
                            //Get Ref No 
                            P_MW_SUMMARY_MW_ITEM_CHECKLIST record = GetP_MW_SUMMARY_MW_ITEM_CHECKLIST(P_MW_SUMMARY_MW_ITEM_CHECKLIST.UUID, P_MW_SUMMARY_MW_ITEM_CHECKLIST.HANDLING_UNIT);

                            List<P_MW_SUMMARY_MW_ITEM_CHECKLIST> models = (from rn in db.P_MW_REFERENCE_NO
                                                                           join r in db.P_MW_RECORD
                                                                           on rn.UUID equals r.REFERENCE_NUMBER
                                                                           join smic in db.P_MW_SUMMARY_MW_ITEM_CHECKLIST
                                                                           on r.UUID equals smic.MW_RECORD_ID
                                                                           where rn.REFERENCE_NO == record.P_MW_RECORD.P_MW_REFERENCE_NO.REFERENCE_NO
                                                                           && smic.HANDLING_UNIT == record.HANDLING_UNIT
                                                                           select smic).ToList();

                            foreach (var model in models)
                            {
                                model.RECOMMEDATION_APPLICATION = P_MW_SUMMARY_MW_ITEM_CHECKLIST.RECOMMEDATION_APPLICATION;
                            }
                        }

                        db.SaveChanges();
                        tran.Commit();
                    }
                    catch (DbEntityValidationException validationError)
                    {
                        var errorMessages = validationError.EntityValidationErrors
                        .SelectMany(validationResult => validationResult.ValidationErrors)
                        .Select(m => m.ErrorMessage);
                        Console.WriteLine("Error :" + validationError.Message);
                        return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { validationError.Message } };
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        Console.WriteLine("Error :" + ex.Message);
                        return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { ex.Message } };
                    }
                    return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
                }

            }
        }


        public ServiceResult SummarySubmit(string REFERENCE_NUMBER, string REFERENCE_NO, string MW_DSN, P_MW_SUMMARY_MW_ITEM_CHECKLIST P_MW_SUMMARY_MW_ITEM_CHECKLIST, string V_UUID)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {

                    try
                    {
                        //Save P_MW_SUMMARY_MW_ITEM_CHECKLIST
                        SaveP_MW_SUMMARY_MW_ITEM_CHECKLIST(P_MW_SUMMARY_MW_ITEM_CHECKLIST, db);

                        P_MW_RECORD finalMwRecord = db.P_MW_RECORD.Where(w => w.REFERENCE_NUMBER == REFERENCE_NUMBER && w.IS_DATA_ENTRY == "N").FirstOrDefault();

                        finalMwRecord.STATUS_CODE = ProcessingConstant.MW_VERIFCAITON_COMPLETED;

                        P_MW_VERIFICATION mwVerification = db.P_MW_VERIFICATION.Where(w => w.UUID == V_UUID).FirstOrDefault();

                        if (mwVerification == null)
                        {
                            throw new Exception("Verification record not valid.");

                        }
                        mwVerification.STATUS_CODE = ProcessingConstant.MW_ACKN_STATUS_OPEN;
                        string taskCode = (ProcessingConstant.HANDLING_UNIT_SMM.Equals(mwVerification.HANDLING_UNIT) ? ProcessingWorkFlowManagementService.WF_SUBMISSION_TASK_VERIF_TO_SMM : ProcessingWorkFlowManagementService.WF_SUBMISSION_TASK_VERIF_TO);

                        ProcessingWorkFlowManagementService.Instance.ToNext(db
                            , ProcessingWorkFlowManagementService.WF_TYPE_SUBMISSION
                            // , finalMwRecord.UUID
                            , MW_DSN
                            , taskCode
                            , SessionUtil.LoginPost.UUID);

                        db.SaveChanges();
                        tran.Commit();
                    }
                    catch (DbEntityValidationException validationError)
                    {
                        var errorMessages = validationError.EntityValidationErrors
                        .SelectMany(validationResult => validationResult.ValidationErrors)
                        .Select(m => m.ErrorMessage);
                        Console.WriteLine("Error :" + validationError.Message);
                        return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { validationError.Message } };
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        Console.WriteLine("Error :" + ex.Message);
                        return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { ex.Message } };
                    }
                    return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };

                }
            }
        }

        #endregion

        public int SubmitAcknowledgement_PO(string REFERENCE_NUMBER, string REFERENCE_NO, string MW_DSN, EntitiesMWProcessing db, string V_UUID)
        {

            P_MW_RECORD finalMwRecord = db.P_MW_RECORD.Where(w => w.REFERENCE_NUMBER == REFERENCE_NUMBER && w.IS_DATA_ENTRY == "N").FirstOrDefault();

            finalMwRecord.STATUS_CODE = ProcessingConstant.MW_VERIFCAITON_COMPLETED;

            //P_MW_VERIFICATION mwVerification = (from mwr in db.P_MW_RECORD
            //                                    join mwno in db.P_MW_REFERENCE_NO on mwr.REFERENCE_NUMBER equals mwno.UUID
            //                                    join mwv in db.P_MW_VERIFICATION on mwr.UUID equals mwv.MW_RECORD_ID
            //                                    where mwno.REFERENCE_NO == REFERENCE_NO
            //                                    && mwr.MW_DSN == MW_DSN
            //                                    && mwr.STATUS_CODE == ProcessingConstant.MW_SECOND_COMPLETE
            //                                    select mwv).FirstOrDefault();

            P_MW_VERIFICATION mwVerification = db.P_MW_VERIFICATION.Where(w => w.UUID == V_UUID).FirstOrDefault();

            if (mwVerification == null)
            {
                throw new Exception("Verification record not valid.");

            }
            mwVerification.STATUS_CODE = ProcessingConstant.MW_ACKN_STATUS_OPEN;
            string taskCode = (ProcessingConstant.HANDLING_UNIT_SMM.Equals(mwVerification.HANDLING_UNIT) ? ProcessingWorkFlowManagementService.WF_SUBMISSION_TASK_ACKN_PO_SMM : ProcessingWorkFlowManagementService.WF_SUBMISSION_TASK_ACKN_PO);

            //P_WF_TASK v = db.P_WF_TASK.Where(o => o.STATUS == "WF_STATUS_OPEN").Where(o => o.P_WF_INFO.RECORD_ID == MW_DSN).FirstOrDefault();
            //if (v == null) throw new Exception("Flow not found.");
            //MW191000076
            ProcessingWorkFlowManagementService.Instance.ToNext(db
                , ProcessingWorkFlowManagementService.WF_TYPE_SUBMISSION
                // , finalMwRecord.UUID
                , MW_DSN
                //, v.TASK_CODE
                , taskCode
                , SessionUtil.LoginPost.UUID);

            return db.SaveChanges();

        }

        public P_S_RULE_OF_CON_LETTER_AND_REF GetP_S_RULE_OF_CON_LETTER_AND_REFByFormCode(string FormCode)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return db.P_S_RULE_OF_CON_LETTER_AND_REF.Where(w => w.S_FORM_TYPE_CODE == FormCode).FirstOrDefault();
            }
        }

        public P_MW_RECORD GetFinalRecordByReferenceNo(string ReferenceNo, EntitiesMWProcessing db)
        {
            return db.P_MW_RECORD.Where(w => w.REFERENCE_NUMBER == ReferenceNo && w.IS_DATA_ENTRY == ProcessingConstant.FLAG_N).FirstOrDefault();
        }


        public int SaveP_MW_SUMMARY_MW_ITEM_CHECKLIST_ACK(P_MW_SUMMARY_MW_ITEM_CHECKLIST model, EntitiesMWProcessing db)
        {
            P_MW_SUMMARY_MW_ITEM_CHECKLIST record = db.P_MW_SUMMARY_MW_ITEM_CHECKLIST.Where(w => w.UUID == model.UUID).FirstOrDefault();

            if (record == null) { return 0; }

            record.RECOMMEDATION_APPLICATION = model.RECOMMEDATION_APPLICATION;
            record.REMARK = model.REMARK;
            record.TRANSFER_RRM = model.TRANSFER_RRM;

            return db.SaveChanges();

        }

        public int SaveP_MW_RECORD_AL_FOLLOW_UP(P_MW_RECORD_AL_FOLLOW_UP model, EntitiesMWProcessing db)
        {
            if (string.IsNullOrEmpty(model.UUID))
            {
                db.P_MW_RECORD_AL_FOLLOW_UP.Add(model);
            }
            else
            {
                P_MW_RECORD_AL_FOLLOW_UP record = db.P_MW_RECORD_AL_FOLLOW_UP.Where(w => w.UUID == model.UUID).FirstOrDefault();

                record.IS_MINOR = model.IS_MINOR;
                record.IS_INVOLVE_MAJOR_OFFENCE = model.IS_INVOLVE_MAJOR_OFFENCE;

            }

            return db.SaveChanges();
        }

        public int SaveP_MW_RECORD_AL_FOLLOW_UP_OFFENCESs(List<P_MW_RECORD_AL_FOLLOW_UP_OFFENCES> models, string masterID, EntitiesMWProcessing db)
        {
            //delete
            List<P_MW_RECORD_AL_FOLLOW_UP_OFFENCES> records = db.P_MW_RECORD_AL_FOLLOW_UP_OFFENCES.Where(w => w.MASTER_ID == masterID).ToList();

            if (records != null && records.Count() > 0)
            {
                db.P_MW_RECORD_AL_FOLLOW_UP_OFFENCES.RemoveRange(records);
                db.SaveChanges();
            }

            db.P_MW_RECORD_AL_FOLLOW_UP_OFFENCES.AddRange(models);

            return db.SaveChanges();
        }

        public int SaveP_MW_RECORD_WL_FOLLOW_UP(P_MW_RECORD_WL_FOLLOW_UP model, EntitiesMWProcessing db)
        {
            if (string.IsNullOrEmpty(model.UUID))
            {
                db.P_MW_RECORD_WL_FOLLOW_UP.Add(model);
            }
            else
            {
                P_MW_RECORD_WL_FOLLOW_UP record = db.P_MW_RECORD_WL_FOLLOW_UP.Where(w => w.UUID == model.UUID).FirstOrDefault();

                record.REFERRAL_LSS_DATE = model.REFERRAL_LSS_DATE;
                record.REFERRAL_EBD_DATE = model.REFERRAL_EBD_DATE;
                record.DISCOVERY_DATE = model.DISCOVERY_DATE;
            }

            return db.SaveChanges();
        }

        public int SaveP_MW_RECORD_REFERRED_TO_LSS_EBD(P_MW_RECORD_REFERRED_TO_LSS_EBD model, EntitiesMWProcessing db)
        {
            if (string.IsNullOrEmpty(model.UUID))
            {
                db.P_MW_RECORD_REFERRED_TO_LSS_EBD.Add(model);
            }
            else
            {
                P_MW_RECORD_REFERRED_TO_LSS_EBD record = db.P_MW_RECORD_REFERRED_TO_LSS_EBD.Where(w => w.UUID == model.UUID).FirstOrDefault();

                record.DROPPED_WITHDRAWN_DATE = model.DROPPED_WITHDRAWN_DATE;
                record.SUMMON_DATE = model.SUMMON_DATE;
                record.HEARING_DATE = model.HEARING_DATE;

                record.TOTAL_FINE = model.TOTAL_FINE;
                record.REMARKS = model.REMARKS;
                record.STATUS = model.STATUS;
            }

            return db.SaveChanges();
        }

        public int SaveP_MW_RECORD_FORM_CHECKLIST_ACK(P_MW_RECORD_FORM_CHECKLIST model, string recordID, string verificationID, EntitiesMWProcessing db)
        {
            //P_MW_RECORD_FORM_CHECKLIST record = db.P_MW_RECORD_FORM_CHECKLIST.Where(d => d.MW_RECORD_ID == recordID && d.MW_VERIFICATION_ID == verificationID).FirstOrDefault();

            P_MW_RECORD_FORM_CHECKLIST record = db.P_MW_RECORD_FORM_CHECKLIST.Where(d => d.UUID == model.UUID).FirstOrDefault();

            if (record != null)
            {
                record.INFO_NOT = model.INFO_NOT;
                record.INFO_NOT_RMK = model.INFO_NOT_RMK;

            }

            return db.SaveChanges();
        }

        public int SaveP_MW_RECORD_FORM_CHECKLIST_PO(P_MW_RECORD_FORM_CHECKLIST_PO model, EntitiesMWProcessing db)
        {
            P_MW_RECORD_FORM_CHECKLIST_PO record = db.P_MW_RECORD_FORM_CHECKLIST_PO.Where(d => d.MW_RECORD_FORM_CHECKLIST_ID == model.MW_RECORD_FORM_CHECKLIST_ID).FirstOrDefault();

            if (record != null)
            {
                db.P_MW_RECORD_FORM_CHECKLIST_PO.Remove(record);
            }

            db.P_MW_RECORD_FORM_CHECKLIST_PO.Add(model);

            return db.SaveChanges();
        }

        public int SaveP_MW_RECORD_ITEM_CHECKLIST_ITEM_PO(List<P_MW_RECORD_ITEM_CHECKLIST_ITEM> models, EntitiesMWProcessing db)
        {
            foreach (var model in models)
            {
                P_MW_RECORD_ITEM_CHECKLIST_ITEM record = db.P_MW_RECORD_ITEM_CHECKLIST_ITEM.Where(w => w.UUID == model.UUID).FirstOrDefault();
                if (record != null)
                {
                    record.PO_AGREEMENT = model.PO_AGREEMENT;
                    record.PO_REMARK = model.PO_REMARK;
                }
            }
            return db.SaveChanges();
        }

        public int SaveP_MW_FORM_09s(List<P_MW_FORM_09> models, EntitiesMWProcessing db)
        {
            foreach (P_MW_FORM_09 model in models)
            {
                P_MW_FORM_09 record = db.P_MW_FORM_09.Where(w => w.UUID == model.UUID).FirstOrDefault();

                record.PO_AGREEMENT = model.PO_AGREEMENT;
                record.PO_RMK = model.PO_RMK;
            }

            return db.SaveChanges();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Constant;
using MWMS2.Entity;
using Oracle.ManagedDataAccess.Client;

namespace MWMS2.Services.ProcessingDAO.DAO
{
    public class ProcessingTSKSSDAService : BaseDAOService
    {

        public Fn03TSK_SSSearchModel Search(Fn03TSK_SSSearchModel model)
        {
            List<OracleParameter> oracleParameters = new List<OracleParameter>();

            string search_where = CreateSqlForSearchMwItem(model, oracleParameters);

            // string SearchDistinctReferenceNo_q = @"SELECT DISTINCT refNo.reference_no
            //                                                 FROM   P_MW_RECORD record
            //                                                        INNER JOIN P_MW_REFERENCE_NO refNo
            //                                                                ON record.reference_number = refNo.uuid " + search_where;
            // string SearchCountDistinctMWDSN_q = @"SELECT Count(DISTINCT en.mw_Dsn)
            //                                                 FROM   p_Mw_Record en
            //                                                        INNER JOIN p_Mw_Verification v
            //                                                                ON en.uuid = v.mw_record_id
            //                                                        INNER JOIN p_mw_reference_no refNo
            //                                                                ON en.reference_number = refNo.uuid
            //                                                 WHERE 1=1 and refNo.reference_no IN ( " + SearchDistinctReferenceNo_q + " ) ";
            //model.CountByDSN = GetObjectData<decimal>(SearchCountDistinctMWDSN_q, oracleParameters.ToArray()).FirstOrDefault().ToString();


            string version2Q = ""
                + "\r\n\t" + " SELECT Count(DISTINCT en.mw_Dsn)                                     "
                + "\r\n\t" + " FROM   p_Mw_Record en                                                "
                //+ "\r\n\t" + " WHERE EXISTS (                                                       "
                //+ "\r\n\t" + " 	SELECT 1 FROM p_Mw_Verification v WHERE en.uuid = v.mw_record_id    "
                //+ "\r\n\t" + " )                                                                    "
                + "\r\n\t" + " WHERE EXISTS (                                                         "
                + "\r\n\t" + " 	SELECT 1 FROM p_mw_reference_no refNo                               "
                + "\r\n\t" + " 	WHERE en.reference_number = refNo.uuid                              "
                + "\r\n\t" + " 	AND EXISTS (                                                        "
                + "\r\n\t" + " 		SELECT 1 FROM P_MW_RECORD record                                "
                + "\r\n\t" + " INNER JOIN P_MW_REFERENCE_NO refNo"
                + "\r\n\t" + "      ON record.reference_number = refNo.uuid"
                + "\r\n\t" + " LEFT JOIN P_MW_FILEREF fileRef"
                + "\r\n\t" + "      ON fileRef.mw_record_id = refNo.reference_no"
                + "\r\n\t" + search_where
                //+ "\r\n\t" + " 		AND record.is_Data_Entry = 'Y'                                "
                + "\r\n\t" + " 		AND record.reference_number = refNo.uuid                        "
                + "\r\n\t" + " 	)                                                                   "
                + "\r\n\t" + " )                                                                    ";
            model.CountByDSN = GetObjectData<decimal>(version2Q, oracleParameters.ToArray()).FirstOrDefault().ToString();





            string searchCountOfSearchMwItem_q = @"SELECT Count(DISTINCT refNo.reference_no)
                                                        FROM   P_MW_RECORD record
                                                               INNER JOIN P_MW_REFERENCE_NO refNo
                                                                       ON record.reference_number = refNo.uuid 
                                                                LEFT JOIN P_MW_FILEREF fileRef
                                                                               ON fileRef.mw_record_id=refNo.reference_no" + search_where;
            model.CountByMWNo = GetObjectData<decimal>(searchCountOfSearchMwItem_q, oracleParameters.ToArray()).FirstOrDefault().ToString();

            model.Query = string.Format(@"SELECT res.reference_no,
                                                   P_get_item_code_by_record_id2(r.uuid) ITEMCODE,
                                                   r.commencement_date,
                                                   r.completion_date,
                                                   r.LOCATION_OF_MINOR_WORK
                                            FROM   P_MW_RECORD r
                                                   INNER JOIN (SELECT DISTINCT refNo.reference_no,
                                                                               record.uuid uuid
                                                               FROM   P_MW_REFERENCE_NO refNo
                                                                      INNER JOIN P_MW_RECORD record
                                                                              ON record.reference_number = refNo.uuid
                                                                      LEFT JOIN P_MW_FILEREF fileRef
                                                                              ON fileRef.mw_record_id=refNo.reference_no {0}) res
                                                           ON res.uuid = r.uuid ", search_where);
            model.QueryParameters = oracleParameters.ToDictionary(m => m.ParameterName.Trim(':'), m => m.Value);
            model.Search();
            return model;
        }



        public string CreateSqlForSearchMwItem(Fn03TSK_SSSearchModel model, List<OracleParameter> oracleParameters)
        {
            StringBuilder whereq = new StringBuilder();

            List<string> itemNo = new List<string>();
            foreach (var item in model.Checkbox_ItemNo_TypeofMWs_Class1.MWItemNos.Keys)
            {
                if (model.Checkbox_ItemNo_TypeofMWs_Class1.MWItemNos[item].IsChecked)
                {
                    itemNo.Add(model.Checkbox_ItemNo_TypeofMWs_Class1.MWItemNos[item].Code);
                }
            }
            foreach (var item in model.Checkbox_ItemNo_TypeofMWs_Class2.MWItemNos.Keys)
            {
                if (model.Checkbox_ItemNo_TypeofMWs_Class2.MWItemNos[item].IsChecked)
                {
                    itemNo.Add(model.Checkbox_ItemNo_TypeofMWs_Class2.MWItemNos[item].Code);
                }
            }
            foreach (var item in model.Checkbox_ItemNo_TypeofMWs_Class3.MWItemNos.Keys)
            {
                if (model.Checkbox_ItemNo_TypeofMWs_Class3.MWItemNos[item].IsChecked)
                {
                    itemNo.Add(model.Checkbox_ItemNo_TypeofMWs_Class3.MWItemNos[item].Code);
                }
            }

            if (itemNo.Count > 0 || !string.IsNullOrWhiteSpace(model.ItemInfo))
            {
                whereq.Append(@" INNER JOIN P_Mw_Record_Item item
                                    ON item.mw_record_id = record.uuid ");
            }

            //if (StringUtil.isNotBlank(itemNo) || StringUtil.isNotBlank(itemInfo))
            //{
            //    queryString += "  ,MwRecordItem item ";
            //}

            if (!string.IsNullOrWhiteSpace(model.AP.ENGLISH_NAME)
                    || !string.IsNullOrWhiteSpace(model.AP.CHINESE_NAME)
                    || !string.IsNullOrWhiteSpace(model.AP.CERTIFICATION_NO))
            {
                whereq.Append(@" INNER JOIN P_Mw_Appointed_Professional ap1
                                   ON ap1.mw_record_ID = record.uuid
                                      AND ap1.identify_Flag IN ( 'RI', 'AP', 'RSE', 'RGE' ) ");
                //queryString += " ,MwAppointedProfessional ap1 ";
            }
            if (!string.IsNullOrWhiteSpace(model.PRC.ENGLISH_NAME)
                    || !string.IsNullOrWhiteSpace(model.PRC.CHINESE_NAME)
                    || !string.IsNullOrWhiteSpace(model.PRC.CERTIFICATION_NO))
            {
                whereq.Append(@" INNER JOIN P_Mw_Appointed_Professional ap2
                                    ON ap2.mw_record_ID = record.uuid 
                                       AND ap2.identify_Flag = 'PRC' ");
                //queryString += " ,MwAppointedProfessional ap2 ";
            }
            if (!string.IsNullOrWhiteSpace(model.MWAddress.DISPLAY_STREET)
                    || !string.IsNullOrWhiteSpace(model.MWAddress.DISPLAY_STREET_NO)
                    || !string.IsNullOrWhiteSpace(model.MWAddress.DISPLAY_BUILDINGNAME)
                    || !string.IsNullOrWhiteSpace(model.MWAddress.DISPLAY_FLOOR)
                    || !string.IsNullOrWhiteSpace(model.MWAddress.DISPLAY_FLAT)
                    || !string.IsNullOrWhiteSpace(model.MWAddress.DISPLAY_DISTRICT)
                    || !string.IsNullOrWhiteSpace(model.MWAddress.DISPLAY_REGION))
            {
                whereq.Append(@" INNER JOIN P_MW_RECORD_ADDRESS_INFO address
                                    ON address.mw_record_id = record.uuid
                                 INNER JOIN P_MW_ADDRESS addr
                                    ON addr.uuid = address.mw_address_id");
                //queryString += " ,MwRecordAddressInfo addressInfo ";
            }

            whereq.Append(@" WHERE  record.is_Data_Entry = :isDataEntry ");
            oracleParameters.Add(new OracleParameter(":isDataEntry", ProcessingConstant.FLAG_N));

            if (!string.IsNullOrWhiteSpace(model.RefNo))
            {
                whereq.Append(@" AND refNo.reference_No LIKE :refNo ");
                oracleParameters.Add(new OracleParameter(":refNo", model.PrefixRefNo + model.RefNo + "%"));
            }

            if (!string.IsNullOrWhiteSpace(model.DSN))
            {
                whereq.Append(@"  AND refNo.reference_No IN (SELECT d.record_Id
                                                              FROM   P_Mw_Dsn d
                                                              WHERE  d.dsn = :searchDsn)  ");
                oracleParameters.Add(new OracleParameter(":searchDsn", model.DSN));
            }

            if (!string.IsNullOrWhiteSpace(model.SubmissionNature))
            {
                whereq.Append(@" AND refNo.reference_No IN (SELECT d.record_Id
                                  FROM   P_Mw_Dsn d
                                  WHERE  d.submission_Nature = :nature) ");
                oracleParameters.Add(new OracleParameter(":nature", model.SubmissionNature));
            }

            //whereq.Append(@" AND P_security_user_can_view(record.uuid, :userGroupType, :isSpo, :userId) > 0 ");

            // item
            if (itemNo.Count > 0 || !string.IsNullOrWhiteSpace(model.ItemInfo))
            {
                if (itemNo.Count > 0)
                {
                    whereq.Append(" And item.mw_Item_Code IN ('" + string.Join("','", itemNo) + "') ");
                }

                if (!string.IsNullOrWhiteSpace(model.ItemInfo))
                {
                    whereq.Append(@" AND item.relevant_Reference LIKE :itemInfo
                                        AND record.Import_Form_Type IS NOT NULL ");
                    oracleParameters.Add(new OracleParameter(":itemInfo", "%" + model.ItemInfo + "%"));
                }
                whereq.Append(@" AND item.status_Code = :itemStatusCode  ");
                oracleParameters.Add(new OracleParameter(":itemStatusCode", ProcessingConstant.MW_RECORD_ITEM_STATUS_FINAL));
            }

            // ap
            if (!string.IsNullOrWhiteSpace(model.AP.ENGLISH_NAME))
            {
                whereq.Append(@" And lower(ap1.english_Name) LIKE lower(:apEnglishName)  ");
                oracleParameters.Add(new OracleParameter(":apEnglishName", "%" + model.AP.ENGLISH_NAME + "%"));
            }
            if (!string.IsNullOrWhiteSpace(model.AP.CHINESE_NAME))
            {
                whereq.Append(@" And lower(ap1.chinese_Name) LIKE lower(:apChineseName) ");
                oracleParameters.Add(new OracleParameter(":apChineseName", "%" + model.AP.CHINESE_NAME + "%"));
            }
            if (!string.IsNullOrWhiteSpace(model.AP.CERTIFICATION_NO))
            {
                whereq.Append(@" AND ap1.certification_No LIKE :apCertificationNo ");
                oracleParameters.Add(new OracleParameter(":apCertificationNo", "%" + model.AP.CERTIFICATION_NO + "%"));
            }

            // prc
            if (!string.IsNullOrWhiteSpace(model.PRC.ENGLISH_NAME))
            {
                whereq.Append(@" AND Lower(ap2.english_Name) LIKE Lower(:prcEnglishName) ");
                oracleParameters.Add(new OracleParameter(":prcEnglishName", "%" + model.PRC.ENGLISH_NAME + "%"));
            }
            if (!string.IsNullOrWhiteSpace(model.PRC.CHINESE_NAME))
            {
                whereq.Append(@" AND Lower(ap2.chinese_Name) LIKE Lower(:prcChineseName) ");
                oracleParameters.Add(new OracleParameter(":prcChineseName", "%" + model.PRC.CHINESE_NAME + "%"));
            }
            if (!string.IsNullOrWhiteSpace(model.PRC.CERTIFICATION_NO))
            {
                whereq.Append(@" AND ap2.certification_No LIKE :prcCertificationNo ");
                oracleParameters.Add(new OracleParameter(":prcCertificationNo", "%" + model.PRC.CERTIFICATION_NO + "%"));
            }

            if (!string.IsNullOrWhiteSpace(model.StatusId))
            {
                if (model.StatusId == ProcessingConstant.MW_STATUS_IN_PROGRESS)
                {
                    whereq.Append(@"    AND ( To_date(record.commencement_Date, 'dd/MM/yyyy') <= To_date(:today, 'dd/MM/yyyy')
                                        AND record.completion_Date IS NULL ) ");
                    oracleParameters.Add(new OracleParameter(":today", DateTime.Now.ToString("dd/MM/yyyy")));
                }
                else if (model.StatusId == ProcessingConstant.MW_STATUS_COMPLETED)
                {
                    whereq.Append(@" AND ( To_date(record.completion_Date, 'dd/MM/yyyy') <= To_date(:today, 'dd/MM/yyyy') ) ");
                    oracleParameters.Add(new OracleParameter(":today", DateTime.Now.ToString("dd/MM/yyyy")));
                }
            }

            if (!string.IsNullOrWhiteSpace(model.AuditRelated))
            {
                if (model.AuditRelated == ProcessingConstant.FLAG_Y)
                {
                    whereq.Append(@" AND ( record.audit_Related IN ( :auditRelated )
                                     OR record.audit_Related IS NULL )");
                }
                else
                {
                    whereq.Append(@" AND record.audit_Related IN ( :auditRelated ) ");
                }
                oracleParameters.Add(new OracleParameter(":auditRelated", model.AuditRelated));
            }

            if (!string.IsNullOrWhiteSpace(model.EfssRefNo))
            {
                whereq.Append(@" AND Upper(record.efss_Ref_No) LIKE Upper(:efssRefNo) ");

                oracleParameters.Add(new OracleParameter(":efssRefNo", model.EfssRefNo + "%"));
            }

            if (!string.IsNullOrWhiteSpace(model.FileRefFour))
            {
                model.FileRefNo += model.FileRefFour;
            }
            if (!string.IsNullOrWhiteSpace(model.FileRefTwo))
            {
                model.FileRefNo += "/" + model.FileRefTwo;
            }
            if (!string.IsNullOrWhiteSpace(model.FileRefNo))
            {
                whereq.Append(@" AND ( ( Upper(record.FILEREF_FOUR
                                        || '/'
                                        || record.FILEREF_TWO) LIKE Upper(:fileRefNo) )
                                    OR ( Upper(fileRef.FILEREF_FOUR
                                            || '/'
                                            || fileRef.FILEREF_TWO) LIKE :fileRefNo ) ) ");

                oracleParameters.Add(new OracleParameter(":fileRefNo", model.FileRefNo + "%"));
            }
            if (!string.IsNullOrWhiteSpace(model.BlockId))
            {
                whereq.Append(@" AND fileRef.BLK_ID=:blkId ");
                oracleParameters.Add(new OracleParameter("blkId", model.BlockId));
            }
            if (!string.IsNullOrWhiteSpace(model.UnitId))
            {
                whereq.Append(@" AND fileRef.UNIT_ID=:unitId ");
                oracleParameters.Add(new OracleParameter("unitId", model.UnitId));
            }

            if (!string.IsNullOrWhiteSpace(model.FormNo))
            {
                whereq.Append(@" AND record.S_Form_Type_Code = :formNo ");

                oracleParameters.Add(new OracleParameter(":formNo", model.FormNo));
            }

            if (!string.IsNullOrWhiteSpace(model.SubmissionType))
            {
                whereq.Append(@" AND record.submit_Type = :submissionType ");

                oracleParameters.Add(new OracleParameter(":submissionType", model.SubmissionType));
            }

            if (!string.IsNullOrWhiteSpace(model.ClassCode))
            {
                whereq.Append(@" AND record.class_Code = :classCode ");

                oracleParameters.Add(new OracleParameter(":classCode", model.ClassCode));
            }

            if (!string.IsNullOrWhiteSpace(model.SubmissionDateForm) || !string.IsNullOrWhiteSpace(model.SubmissionDateTo))
            {
                whereq.Append(@" and 0 < " + GetReceivedDateSubSQL(model, oracleParameters));
            }

            if (!string.IsNullOrWhiteSpace(model.CommenceDateFrom))
            {
                whereq.Append(@" AND To_date(to_char(record.commencement_Date,'dd/MM/yyyy'), 'dd/MM/yyyy') >= To_date(:commenceFromDate, 'dd/MM/yyyy') ");

                oracleParameters.Add(new OracleParameter(":commenceFromDate", model.CommenceDateFrom));
            }

            if (!string.IsNullOrWhiteSpace(model.CommenceDateTo))
            {
                whereq.Append(@" AND To_date(to_char(record.commencement_Date,'dd/MM/yyyy'), 'dd/MM/yyyy') <= To_date(:commenceToDate, 'dd/MM/yyyy') ");

                oracleParameters.Add(new OracleParameter(":commenceToDate", model.CommenceDateTo));
            }

            if (!string.IsNullOrWhiteSpace(model.CompleteDateFrom))
            {
                whereq.Append(@" AND To_date(to_char(record.completion_Date,'dd/MM/yyyy'), 'dd/MM/yyyy') >= To_date(:completeFromDate, 'dd/MM/yyyy') ");

                oracleParameters.Add(new OracleParameter(":completeFromDate", model.CompleteDateFrom));
            }

            if (!string.IsNullOrWhiteSpace(model.CompleteDateTo))
            {
                whereq.Append(@" AND To_date(to_char(record.completion_Date,'dd/MM/yyyy'), 'dd/MM/yyyy') <= To_date(:completeToDate, 'dd/MM/yyyy') ");

                oracleParameters.Add(new OracleParameter(":completeToDate", model.CompleteDateTo));
            }

            if (!string.IsNullOrWhiteSpace(model.LocationOfMW))
            {
                whereq.Append(@" AND record.location_Of_Minor_Work LIKE :locationOfMw ");

                oracleParameters.Add(new OracleParameter(":locationOfMw", "%" + model.LocationOfMW + "%"));
            }

            //mwAddress
            if (!string.IsNullOrWhiteSpace(model.MWAddress.DISPLAY_STREET))
            {
                whereq.Append(@" AND addr.display_Street LIKE :street ");
                oracleParameters.Add(new OracleParameter(":street", "%" + model.MWAddress.DISPLAY_STREET + "%"));
            }
            if (!string.IsNullOrWhiteSpace(model.MWAddress.DISPLAY_STREET_NO))
            {
                whereq.Append(@" AND addr.display_Street_No LIKE :streetNo ");
                oracleParameters.Add(new OracleParameter(":streetNo", "%" + model.MWAddress.DISPLAY_STREET_NO + "%"));

            }
            if (!string.IsNullOrWhiteSpace(model.MWAddress.DISPLAY_BUILDINGNAME))
            {
                whereq.Append(@" AND addr.display_Buildingname LIKE :building");
                oracleParameters.Add(new OracleParameter(":building", "%" + model.MWAddress.DISPLAY_BUILDINGNAME + "%"));

            }
            if (!string.IsNullOrWhiteSpace(model.MWAddress.DISPLAY_FLOOR))
            {
                whereq.Append(@" AND addr.display_Floor LIKE :floor ");
                oracleParameters.Add(new OracleParameter(":floor", "%" + model.MWAddress.DISPLAY_FLOOR + "%"));

            }
            if (!string.IsNullOrWhiteSpace(model.MWAddress.DISPLAY_FLAT))
            {
                whereq.Append(@" AND addr.display_Flat LIKE :flat ");
                oracleParameters.Add(new OracleParameter(":flat", "%" + model.MWAddress.DISPLAY_FLAT + "%"));

            }
            if (!string.IsNullOrWhiteSpace(model.MWAddress.DISPLAY_DISTRICT))
            {
                whereq.Append(@" AND addr.display_District LIKE :district ");
                oracleParameters.Add(new OracleParameter(":district", "%" + model.MWAddress.DISPLAY_DISTRICT + "%"));

            }
            if (!string.IsNullOrWhiteSpace(model.MWAddress.DISPLAY_REGION))
            {
                whereq.Append(@" AND addr.display_Region LIKE :region ");
                oracleParameters.Add(new OracleParameter(":region", model.MWAddress.DISPLAY_REGION));

            }

            return whereq.ToString();
        }

        public string GetReceivedDateSubSQL(Fn03TSK_SSSearchModel model, List<OracleParameter> oracleParameters)
        {
            StringBuilder whereq = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(model.SubmissionDateForm) && !string.IsNullOrWhiteSpace(model.SubmissionDateTo))
            {
                whereq.Append(@" ( 1 ) ");
            }
            else if (string.IsNullOrWhiteSpace(model.SubmissionDateTo))
            {
                whereq.Append(@"(SELECT Count(recordCount.uuid)
                                 FROM   P_Mw_Record recordCount
                                        INNER JOIN P_Mw_Form mwFormCount
                                                ON recordCount.uuid = mwFormCount.mw_record_id
                                        INNER JOIN P_MW_REFERENCE_NO refNo
                                                ON refNo.uuid = recordCount.reference_number
                                 WHERE  recordCount.status_Code = 'MW_SECOND_COMPLETE'
                                        AND To_date(to_char(mwFormCount.received_Date,'dd/MM/yyyy'), 'dd/MM/yyyy') >= To_date(:submissionFromDate, 'dd/MM/yyyy')) ");

                oracleParameters.Add(new OracleParameter(":submissionFromDate", model.SubmissionDateForm));
            }
            else if (string.IsNullOrWhiteSpace(model.SubmissionDateForm))
            {
                whereq.Append(@"(SELECT Count(recordCount.uuid)
                                 FROM   P_Mw_Record recordCount
                                        INNER JOIN P_Mw_Form mwFormCount
                                                ON recordCount.uuid = mwFormCount.mw_record_id
                                        INNER JOIN P_MW_REFERENCE_NO refNo
                                                ON refNo.uuid = recordCount.reference_number
                                 WHERE  recordCount.status_Code = 'MW_SECOND_COMPLETE'
                                        AND To_date(to_char(mwFormCount.received_Date,'dd/MM/yyyy'), 'dd/MM/yyyy') <= To_date(:submissionToDate, 'dd/MM/yyyy')) ");

                oracleParameters.Add(new OracleParameter(":submissionToDate", model.SubmissionDateTo));
            }
            else
            {
                whereq.Append(@"(SELECT Count(recordCount.uuid)
                                 FROM   P_Mw_Record recordCount
                                        INNER JOIN P_Mw_Form mwFormCount
                                                ON recordCount.uuid = mwFormCount.mw_record_id
                                        INNER JOIN P_MW_REFERENCE_NO refNo
                                                ON refNo.uuid = recordCount.reference_number
                                 WHERE  recordCount.status_Code = 'MW_SECOND_COMPLETE'
                                        AND To_date(to_char(mwFormCount.received_Date,'dd/MM/yyyy'), 'dd/MM/yyyy') >= To_date(:submissionToDate, 'dd/MM/yyyy'))
                                        AND To_date(to_char(mwFormCount.received_Date,'dd/MM/yyyy'), 'dd/MM/yyyy') <= To_date(:submissionFromDate, 'dd/MM/yyyy')) ");

                oracleParameters.Add(new OracleParameter(":submissionToDate", model.SubmissionDateTo));
                oracleParameters.Add(new OracleParameter(":submissionFromDate", model.SubmissionDateForm));
            }
            return whereq.ToString();
        }

        public string Excel(Fn03TSK_SSSearchModel model)
        {
            List<OracleParameter> oracleParameters = new List<OracleParameter>();

            CreateExcelColumns(model);

            string search_where = CreateSqlForSearchMwItem(model, oracleParameters);
            model.Query = string.Format(@"  SELECT res.reference_no,
                                                   P_get_item_code_by_record_id2(r.uuid) ITEM_CODE,
                                                   r.FILEREF_FOUR||'/'||r.FILEREF_TWO FILE_REF_NO,
                                                   ack.NATURE,
                                                   ack.FORM_NO,
                                                   ack.RECEIVED_DATE,
                                                   substr(ack.PRC_NO,1,instr(ack.PRC_NO,' ')-1) PRC,
                                                   substr(ack.PBP_NO,1,instr(ack.PBP_NO,' ')-1) PBP,
                                                   ack.SSP,
                                                   ack.STREET,
                                                   ack.STREET_NO,
                                                   ack.BUILDING,
                                                   ack.FLOOR,
                                                   ack.UNIT,
                                                   ack.PAW,
                                                   ack.PAW_CONTACT,
                                                   ack.IO_MGT,
                                                   ack.IO_MGT_CONTACT,
                                                   ack.REMARK,
                                                   ack.LETTER_DATE,
                                                   taskStatus.VERIFICATIONTO,
                                                   taskStatus.VERIFICATIONSPO,
                                                   taskStatus.ACKNOWLEDGEMENTPO,
                                                   taskStatus.ACKNOWLEDGEMENTSPO,
                                                   'TBC' OrderBDRef,
                                                   'TBC' PreviousRelate,
                                                   'TBC' ACKResult,
                                                   r.*
                                            FROM   P_MW_RECORD r
                                                   INNER JOIN (SELECT DISTINCT refNo.reference_no,
                                                                               record.uuid uuid
                                                               FROM   P_MW_REFERENCE_NO refNo
                                                                      INNER JOIN P_MW_RECORD record
                                                                              ON record.reference_number = refNo.uuid
                                                               WHERE  record.is_Data_Entry = :isDataEntry
                                                                      AND refNo.reference_No LIKE :refNo) res
                                                           ON res.uuid = r.uuid
                                                    inner join P_MW_ACK_LETTER ack
                                                            on ack.DSN=r.MW_DSN
                                                    left join(
                                                        SELECT *
                                                        FROM   (SELECT info.record_id,
                                                                       task.TASK_CODE,
                                                                       u.BD_PORTAL_LOGIN
                                                                FROM   P_WF_INFO info
                                                                       INNER JOIN P_WF_TASK task
                                                                               ON task.P_WF_INFO_ID = info.uuid
                                                                       INNER JOIN P_WF_TASK_USER taskU
                                                                               ON taskU.P_WF_TASK_ID = task.uuid
                                                                       INNER JOIN SYS_POST u
                                                                               ON taskU.user_id = u.uuid)
                                                         pivot(sum(TASK_CODE) 
                                                                for TASK_CODE in 
                                                                (  'Verification-TO' VerificationTO
                                                                    ,'Verification-SPO' VerificationSPO
                                                                    ,'Acknowledgement-PO' AcknowledgementPO
                                                                    ,'Acknowledgement-SPO' AcknowledgementSPO))
                                                    ) taskStatus
                                                    on taskStatus.RECORD_ID=r.MW_DSN
                                             ", search_where);
            model.QueryParameters = oracleParameters.ToDictionary(m => m.ParameterName.Trim(':'), m => m.Value);
            model.Search();

            return model.ExportCurrentData("Submission Search_" + DateTime.Now.ToString("yyyy-MM-dd"));
        }

        private static void CreateExcelColumns(Fn03TSK_SSSearchModel model)
        {
            Dictionary<string, string> col1 = GetExcelColumn("Screened Nature", "NATURE");
            Dictionary<string, string> col2 = GetExcelColumn("dsn", "MW_DSN");
            Dictionary<string, string> col3 = GetExcelColumn("Received Date", "RECEIVED_DATE");
            Dictionary<string, string> col4 = GetExcelColumn("Form type", "FORM_NO");
            Dictionary<string, string> col5 = GetExcelColumn("Ref No.", "REFERENCE_NO");
            Dictionary<string, string> col6 = GetExcelColumn("AP/RI reg. no. ", "PBP");
            Dictionary<string, string> col7 = GetExcelColumn("PRC reg. no. ", "PRC");
            Dictionary<string, string> col8 = GetExcelColumn("Location of Minor Work", "LOCATION_OF_MINOR_WORK");
            Dictionary<string, string> col9 = GetExcelColumn("Street", "STREET");
            Dictionary<string, string> col10 = GetExcelColumn("Street No.", "STREET_NO");
            Dictionary<string, string> col11 = GetExcelColumn("Building / Block", "BUILDING");
            Dictionary<string, string> col12 = GetExcelColumn("Floor", "FLOOR");
            Dictionary<string, string> col13 = GetExcelColumn("Unit", "UNIT");
            Dictionary<string, string> col14 = GetExcelColumn("Item No.", "ITEM_CODE");
            Dictionary<string, string> col15 = GetExcelColumn("SSP", "SSP");
            Dictionary<string, string> col16 = GetExcelColumn("Commencement Date", "COMMENCEMENT_DATE");
            Dictionary<string, string> col17 = GetExcelColumn("Completion Date", "COMPLETION_DATE");
            Dictionary<string, string> col18 = GetExcelColumn("Order/BD Ref.", "ORDERBDREF");
            Dictionary<string, string> col19 = GetExcelColumn("PAW", "PAW");
            Dictionary<string, string> col20 = GetExcelColumn("Contact of PAW", "PAW_CONTACT");
            Dictionary<string, string> col21 = GetExcelColumn("I.O. or Property Management", "IO_MGT");
            Dictionary<string, string> col22 = GetExcelColumn("Contact of I.O. or Property Management", "IO_MGT_CONTACT");
            Dictionary<string, string> col23 = GetExcelColumn("Remark", "REMARK");
            Dictionary<string, string> col24 = GetExcelColumn("Letter Date", "LETTER_DATE");
            Dictionary<string, string> col25 = GetExcelColumn("Previous Related MW No.", "PREVIOUSRELATE");
            Dictionary<string, string> col26 = GetExcelColumn("File Ref No", "FILE_REF_NO");
            Dictionary<string, string> col27 = GetExcelColumn("Acknowledgement Result", "ACKRESULT");
            Dictionary<string, string> col28 = GetExcelColumn("Verification TO", "VERIFICATIONTO");
            Dictionary<string, string> col29 = GetExcelColumn("Verification SPO", "VERIFICATIONSPO");
            Dictionary<string, string> col30 = GetExcelColumn("Acknowledgement PO", "ACKNOWLEDGEMENTPO");
            Dictionary<string, string> col31 = GetExcelColumn("Acknowledgement SPO", "ACKNOWLEDGEMENTSPO");

            model.Columns = new Dictionary<string, string>[]
            {
                col1,col2,col3,col4,col5
                ,col6,col7,col8,col9,col10
                ,col11,col12,col13,col14,col15
                ,col16,col17,col18,col19,col20
                ,col21,col22,col23,col24,col25
                ,col26,col27,col28,col29,col30
                ,col31
            };
        }

        private static Dictionary<string, string> GetExcelColumn(string displayName, string columnName)
        {
            Dictionary<string, string> col = new Dictionary<string, string>();
            col.Add("displayName", displayName);
            col.Add("columnName", columnName);
            return col;
        }
    }
}
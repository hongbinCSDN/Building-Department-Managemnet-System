using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Utility;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;

namespace MWMS2.Services.Signborad.SignboardDAO
{
    public class SignboardAuditRecordDAOService : BaseDAOService
    {

        public List<B_SV_AUDIT_RECORD> getAuditRecord(string uuid)
        {
            List<B_SV_AUDIT_RECORD> resultList = new List<B_SV_AUDIT_RECORD>();
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                //resultList = 
            }
            return resultList;
        }

        public B_SV_AUDIT_RECORD FindById(string id)
        {
            B_SV_AUDIT_RECORD instance = new B_SV_AUDIT_RECORD();
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                instance = db.B_SV_AUDIT_RECORD.Find(id);
            }
            return instance;
        }

        public List<B_SV_AUDIT_RECORD> FindByProperty(string propertyName, Object value)
        {
            List<B_SV_AUDIT_RECORD> resultList = new List<B_SV_AUDIT_RECORD>();
            string queryString = @"SELECT * FROM B_SV_AUDIT_RECORD WHERE :PROPERTY_NAME = :VALUE";
            queryString = queryString.Replace(":PROPERTY_NAME", propertyName);
            OracleParameter[] oracleParameters = new OracleParameter[]
            {
            new OracleParameter("VALUE", value)
            };

            return GetObjectData<B_SV_AUDIT_RECORD>(queryString, oracleParameters).ToList();
        }

        public List<B_SV_AUDIT_RECORD> FindByAll()
        {
            List<B_SV_AUDIT_RECORD> resultList = new List<B_SV_AUDIT_RECORD>();
            using (EntitiesSignboard db = new EntitiesSignboard()) 
            {
                resultList = db.B_SV_AUDIT_RECORD.ToList();
            }
            return resultList;
        }

        public List<B_SV_SCANNED_DOCUMENT> GetScannedDocumentList(string uuid) // uuid is reference no.
        {
            List<B_SV_SCANNED_DOCUMENT> resultList = new List<B_SV_SCANNED_DOCUMENT>();
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                resultList = db.B_SV_SCANNED_DOCUMENT.Where(x => x.RECORD_ID == uuid).ToList();
            }
            return resultList;
        }

        public List<B_SV_PHOTO_LIBRARY> GetPhotoLibraryList(string sv_signboard_id)
        {
            List<B_SV_PHOTO_LIBRARY> resultList = new List<B_SV_PHOTO_LIBRARY>();
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                resultList = db.B_SV_PHOTO_LIBRARY.Where(x => x.SV_SIGNBOARD_ID == sv_signboard_id).ToList();
            }
            return resultList;
        }

        public List<List<object>> GetAuditRecord(string id)
        {
            Dictionary<string, object> queryParameters = new Dictionary<string, object>();
            List<List<object>> data = new List<List<object>>();

            string queryString = "select DISTINCT svr.reference_no AS SUBMISSION_NO, svr.form_code AS FORM_CODE, svr.received_date AS RECEIVED_DATE,"
                                + " svv.AUDIT_RESULT AS AUDIT_RESULT, svv.audit_remark AS REAMRK, svv.referral_date AS REFERRAL_DATE, svv.reply_date AS REPLY_DATE, sba.full_address AS ADDRESS,"
                                + " owner.name_chinese as OWNER_CHINESE_NAME, owner.name_english as OWNER_ENGLISH_NAME, ownera.full_address as OWNER_ADDRESS, owner.email as OWNER_EMAIL, owner.contact_no as CONTACT_NO, owner.fax_no as OWNER_FAX_NO,"
                                + " paw.name_chinese as PAW_CHINESE_NAME, paw.name_english as PAW_ENGLISH_NAME, pawa.full_address as PAW_ADDRESS, paw.email as PAW_EMAIL,paw.contact_no as PAW_CONTACT_NO, paw.fax_no as PAW_FAX_NO,"
                                + " io.name_chinese as IO_CHINESE_NAME, io.name_english as IO_ENGLISH_NAME, ioa.full_address as IO_ADDRESS, io.email as IO_EMAIL, io.contact_no as IO_CONTACT_NO, io.fax_no as IO_FAX_NO, "
                                + " io.pbp_name AS IO_PBP_NAME, io.pbp_contact_no AS IO_PBP_CONTACT_NO, io.prc_name AS IO_PRC_NAME, io.prc_contact_no AS IO_PRC_CONTACT_NO,"
                                + " ap.certification_no as AP_CERT_NO, ap.chinese_name as AP_CHINESE_NAME, ap.english_name as AP_ENGLISH_NAME, "
                                + " add_ap.EN_ADDRESS_LINE1 AS AP_EN_ADD_1, add_ap.EN_ADDRESS_LINE2 AS AP_EN_ADD_2, add_ap.EN_ADDRESS_LINE3 AS AP_EN_ADD_3, add_ap.EN_ADDRESS_LINE4 as AP_EN_ADD_4, add_ap.EN_ADDRESS_LINE5 AS AP_EN_ADD_5,"
                                + " add_ap.CN_ADDRESS_LINE1 AS AP_CN_ADD_1, add_ap.CN_ADDRESS_LINE2 AS AP_CN_ADD_2, add_ap.CN_ADDRESS_LINE3 AS AP_CN_ADD_3, add_ap.CN_ADDRESS_LINE4 AS AP_CN_ADD_4, add_ap.CN_ADDRESS_LINE5 AS AP_CN_ADD_5,"
                                + " ap.contact_no as AP_CONTACT_NO, ap.fax_no as AP_FAX_NO,"
                                + " rse.certification_no as RSE_CERT_NO , rse.chinese_name as RSE_CHINESE_NAME, rse.english_name as RSE_ENGLISH_NAME,"
                                + " add_rse.EN_ADDRESS_LINE1 AS RSE_EN_ADD_1, add_rse.EN_ADDRESS_LINE2 AS RSE_EN_ADD_2, add_rse.EN_ADDRESS_LINE3 AS RSE_EN_ADD_3, add_rse.EN_ADDRESS_LINE4 as RSE_EN_ADD_4, add_rse.EN_ADDRESS_LINE5 AS RSE_EN_ADD_5,"
                                + " add_rse.CN_ADDRESS_LINE1 AS RSE_CN_ADD_1, add_rse.CN_ADDRESS_LINE2 AS RSE_CN_ADD_2, add_rse.CN_ADDRESS_LINE3 AS RSE_CN_ADD_3, add_rse.CN_ADDRESS_LINE4 AS RSE_CN_ADD_4, add_rse.CN_ADDRESS_LINE5 AS RSE_CN_ADD_5,"
                                + " rse.contact_no as RSE_CONTACT_NO, rse.fax_no as RSE_FAX_NO,"
                                + " rge.certification_no as RGE_CERT_NO, rge.chinese_name as RGE_CHINESE_NAME, rge.english_name as RGE_ENGLISH_NAME, "
                                + " add_rge.EN_ADDRESS_LINE1 AS RGE_EN_ADD_1, add_rge.EN_ADDRESS_LINE2 AS RGE_EN_ADD_2, add_rge.EN_ADDRESS_LINE3 AS RGE_EN_ADD_3, add_rge.EN_ADDRESS_LINE4 as RGE_EN_ADD_4, add_rge.EN_ADDRESS_LINE5 AS RGE_EN_ADD_5,"
                                + " add_rge.CN_ADDRESS_LINE1 AS RGE_CN_ADD_1, add_rge.CN_ADDRESS_LINE2 AS RGE_CN_ADD_2, add_rge.CN_ADDRESS_LINE3 AS RGE_CN_ADD_3, add_rge.CN_ADDRESS_LINE4 AS RGE_CN_ADD_4, add_rge.CN_ADDRESS_LINE5 AS RGE_CN_ADD_5,"
                                + " rge.contact_no as RGE_CONTACT_NO, rge.fax_no as RGE_FAX_NO,"
                                + " prc.certification_no as PRC_CERT_NO, prc.chinese_name as PRC_CHINESE_NAME, prc.english_name as PRC_ENGLISH_NAME, "
                                + " add_prc.EN_ADDRESS_LINE1 AS PRC_EN_ADD_1, add_prc.EN_ADDRESS_LINE2 AS PRC_EN_ADD_2, add_prc.EN_ADDRESS_LINE3 AS PRC_EN_ADD_3, add_prc.EN_ADDRESS_LINE4 as PRC_EN_ADD_4, add_prc.EN_ADDRESS_LINE5 AS PRC_EN_ADD_5,"
                                + " add_prc.CN_ADDRESS_LINE1 AS PRC_CN_ADD_1, add_prc.CN_ADDRESS_LINE2 AS PRC_CN_ADD_2, add_prc.CN_ADDRESS_LINE3 AS PRC_CN_ADD_3, add_prc.CN_ADDRESS_LINE4 AS PRC_CN_ADD_4, add_prc.CN_ADDRESS_LINE5 AS PRC_CN_ADD_5,"
                                + " prc.contact_no as PRC_CONTACT_NO, prc.fax_no as PRC_FAX_NO, prc.as_chinese_name AS PRC_AS_CHINESE_NAME, prc.as_english_name AS PRC_AS_ENGLISH_NAME"
                                + " from b_sv_audit_record svv"
                                + " LEFT JOIN b_sv_record svr ON svr.uuid = svv.sv_record_id"
                                + " LEFT JOIN b_sv_signboard svs ON svs.uuid = svr.sv_signboard_id"
                                + " LEFT JOIN b_sv_address sba ON sba.uuid = svs.location_address_id"
                                + " LEFT JOIN b_sv_person_contact owner ON owner.uuid = svs.owner_id"
                                + " LEFT JOIN b_sv_address ownera ON ownera.uuid = owner.sv_address_id"
                                + " LEFT JOIN b_sv_person_contact paw ON paw.uuid = svr.paw_id"
                                + " LEFT JOIN b_sv_address pawa ON pawa.uuid = paw.sv_address_id"
                                + " LEFT JOIN b_sv_person_contact io ON io.uuid = svr.oi_id"
                                + " LEFT JOIN b_sv_address ioa ON ioa.uuid = io.sv_address_id"
                                + " LEFT JOIN b_sv_appointed_professional ap ON ap.sv_record_id = svr.uuid"
                                + " LEFT JOIN b_sv_appointed_professional rse ON rse.sv_record_id = svr.uuid"
                                + " LEFT JOIN b_sv_appointed_professional rge ON rge.sv_record_id = svr.uuid"
                                + " LEFT JOIN b_sv_appointed_professional prc ON prc.sv_record_id = svr.uuid"
                                + " LEFT JOIN B_CRM_PBP_PRC add_ap ON add_ap.CERTIFICATION_NO = ap.CERTIFICATION_NO"
                                + " LEFT JOIN B_CRM_PBP_PRC add_rse ON add_rse.CERTIFICATION_NO = rse.CERTIFICATION_NO"
                                + " LEFT JOIN B_CRM_PBP_PRC add_rge ON add_rge.CERTIFICATION_NO = rge.CERTIFICATION_NO"
                                + " LEFT JOIN B_CRM_PBP_PRC add_prc ON add_prc.CERTIFICATION_NO = prc.CERTIFICATION_NO"
                                + " where svv.uuid= :uuid"
                                + " and ap.identify_flag='AP'"
                                + " and rse.identify_flag='RSE'"
                                + " and rge.identify_flag='RGE'"
                                + " and prc.identify_flag='PRC'";

            queryParameters.Add("uuid", id);


            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                using (DbConnection conn = db.Database.Connection)
                {
                    DbDataReader dr = CommonUtil.GetDataReader(conn, queryString, queryParameters);
                    data = CommonUtil.convertToList(dr);
                    conn.Close();
                }
            }
            return data;
        }    
    }
}
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
using MWMS2.Areas.Signboard.Models;
using System.Globalization;
using MWMS2.Services.Signborad.SignboardDAO;
using System.IO;

namespace MWMS2.Services.Signborad.SignboardServices
{
    public class SignboardTDLService
    {
        // -------------- Validation Search
        string SearchTDL_VS_q = ""
                     + "SELECT *"
                     + "\r\n" + "FROM"
                     + "\r\n\t" + "("
                     + "\r\n\t\t" + " SELECT SVV_UUID, REF_NO, EXPIRY_DATE, LOCATION, SV_RECORD_UUID, FORM_CODE, RECEIVED_DATE, VALID_RESULT, SVR_CREATED_DATE, REMOVAL_DISCOV_DATE, STATUS, ALTERNATION, RECOMMENDATION, MW_ITEM_CODE, REVISION "
                     + "\r\n\t\t\t" + ", CASE WHEN MIN_ITEM_NO LIKE '1.%' THEN 'Class I' WHEN MIN_ITEM_NO LIKE '2.%' THEN 'Class II' WHEN MIN_ITEM_NO LIKE '3.%' THEN 'Class III' END AS CLASS "
                     + "\r\n\t\t" + "FROM"
                     + "\r\n\t\t\t" + "("
                     + "\r\n\t\t\t\t" + " select distinct svv.uuid as SVV_UUID, svr.reference_no AS REF_NO, svv.signboard_expiry_date AS EXPIRY_DATE "
                     + "\r\n\t\t\t\t\t" + " , svs.location_of_signboard AS LOCATION, svr.uuid as SV_RECORD_UUID , svr.form_code AS FORM_CODE "
                     + "\r\n\t\t\t\t\t" + " , svr.received_date AS RECEIVED_DATE, svv.validation_result AS VALID_RESULT"
                     + "\r\t\t\t\t\t\t" + " , svr.created_date AS SVR_CREATED_DATE, svr.signboard_removal_discov_date AS REMOVAL_DISCOV_DATE"
                     + "\r\n\t\t\t\t\t" + " , CASE WHEN (svv.validation_result is null AND svv.spo_endorsement_date is null) THEN 'Open' WHEN (svv.validation_result is not null and svv.spo_endorsement_date is not null) THEN 'Close' END AS STATUS "
                     + "\r\n\t\t\t\t\t" + " , svr.with_Alteration AS ALTERNATION, svr.RECOMMENDATION AS RECOMMENDATION, svr.PAW_ID as PAW_ID "
                     + "\r\n\t\t\t\t" + " from b_sv_record svr, b_sv_validation svv, b_sv_address sva, b_sv_signboard svs "
                     ;


        public Fn02TDL_VSSearchModel SearchTDL_VS(Fn02TDL_VSSearchModel model)
        {
            model.Query = SearchTDL_VS_q;
            model.QueryWhere = SearchTDL_VS_whereQ  (model);
            model.Search();

            if (model.Data != null)
            {
                SignboardTDLService tdl = new SignboardTDLService();
                for (int i = 0; i < model.Data.Count; i++)
                {
                    model.Data[i]["FILE_PATH"] = tdl.checkThumbnailFilePath(model.Data[i]["FILE_PATH"].ToString());
                }
            }

            return model;
        }
        //private string SearchTDL_VS_whereQ(Fn02TDL_VSSearchModel model)
        //{
        //    string whereQ = "";
        //    // string CurrUserId = "";
        //    string classType = "";

        //    // CurrUserId = "8a85934933b6427e0133b648cd94002a";    // Current logged in user's uuid

        //    // Order No, Class Type, Item No: add to FROM clause
        //    if (!string.IsNullOrWhiteSpace(model.OrderNo) || !string.IsNullOrWhiteSpace(model.Class) || !string.IsNullOrWhiteSpace(model.ItemNo))
        //    {
        //        whereQ += ", b_svr_item svri";
        //    }

        //    // Handling Officer
        //    //if (string.IsNullOrWhiteSpace(model.HandlingOfficer))
        //    //{
        //    //    whereQ += ", b_sv_appointed_professional svap";
        //    //    whereQ += "\r\n\t\t\t\t" + " WHERE b_is_final_validation(svr.uuid) = 'Y'";
        //    //    whereQ += "\r\n\t\t\t\t\t" + " AND svv.svr_ID=svr.uuid AND svs.uuid=svr.svs_id and sva.uuid = svs.LOCATION_ADDRESS_ID AND svap.svr_id = svr.uuid";
        //    //}
        //    //else
        //    //{
        //    whereQ += " , b_sv_appointed_professional svap, b_wf_info wfi, b_wf_task wft, b_wf_task_user wftu";
        //    whereQ += "\r\n\t\t\t\t" + " where b_is_final_validation(svr.uuid) = 'Y' and";
        //    whereQ += "\r\n\t\t\t\t\t" + " svv.svr_ID = svr.uuid and";
        //    whereQ += "\r\n\t\t\t\t\t" + " svs.uuid = svr.svs_id and sva.uuid = svs.LOCATION_ADDRESS_ID and";
        //    whereQ += "\r\n\t\t\t\t\t" + " svap.svr_id = svr.uuid and wfi.record_id = svv.svr_id and wfi.uuid = wft.wf_info_id and wft.uuid = wftu.wf_task_id"; ;
        //    whereQ += "\r\n\t\t\t\t\t" + " and svv.spo_endorsement_date is not null"; // display the validation result of signboard submission after endorsement by SPO 
        //     //}

        //    // Order No, Class Type, Item No: add to WHERE clause (join)
        //    if (!string.IsNullOrWhiteSpace(model.OrderNo) || !string.IsNullOrWhiteSpace(model.Class) || !string.IsNullOrWhiteSpace(model.ItemNo))
        //    {
        //        whereQ += " and svr.uuid=svri.svr_ID";
        //    }

        //    if (!string.IsNullOrWhiteSpace(model.RefNo))
        //    {
        //        whereQ += "\r\n\t\t\t\t\t" + " and lower(svr.reference_no) like :fileRefNo ";
        //        model.QueryParameters.Add("fileRefNo", "%" + model.RefNo.ToLower() + "%");
        //    }
        //    if (!string.IsNullOrWhiteSpace(model.OrderNo))
        //    {
        //        whereQ += "\r\n\t\t\t\t\t" + " and lower(svri.revision) like :fileBdRef ";
        //        model.QueryParameters.Add("fileBdRef", "%" + model.OrderNo.ToLower() + "%");
        //    }
        //    if (model.ReceivedDateFrom.HasValue)
        //    {
        //        whereQ += "\r\n\t\t\t\t\t" + " and svr.RECEIVED_DATE >= :receivedDateFrom ";
        //        model.QueryParameters.Add("receivedDateFrom", model.ReceivedDateFrom.Value);
        //    }
        //    if (model.ReceivedDateTo.HasValue)
        //    {
        //        whereQ += "\r\n\t\t\t\t\t" + " and svr.RECEIVED_DATE <= :receivedDateTo ";
        //        model.QueryParameters.Add("receivedDateTo", model.ReceivedDateTo.Value);
        //    }
        //    if (!string.IsNullOrWhiteSpace(model.Status))
        //    {
        //        if (model.Status.Equals("Open"))
        //        {
        //            whereQ += "\r\n\t\t\t\t\t" + " and svv.validation_result is null and svv.spo_endorsement_date is null";
        //        }
        //        else if (model.Status.Equals("Close"))
        //        {
        //            whereQ += "\r\n\t\t\t\t\t" + " and svv.validation_result is not null and svv.spo_endorsement_date is not null";
        //        }
        //    }
        //    if (!string.IsNullOrWhiteSpace(model.Street))
        //    {
        //        whereQ += "\r\n\t\t\t\t\t" + " and lower(sva.street) like :street ";
        //        model.QueryParameters.Add("street", "%" + model.Street.ToLower() + "%");
        //    }
        //    if (!string.IsNullOrWhiteSpace(model.StreetNo))
        //    {
        //        whereQ += "\r\n\t\t\t\t\t" + " and lower(sva.street_no) like :streetNo ";
        //        model.QueryParameters.Add("streetNo", "%" + model.StreetNo.ToLower() + "%");
        //    }
        //    if (!string.IsNullOrWhiteSpace(model.Building))
        //    {
        //        whereQ += "\r\n\t\t\t\t\t" + " and lower(sva.buildingname) like :building ";
        //        model.QueryParameters.Add("building", "%" + model.Building.ToLower() + "%");
        //    }
        //    if (!string.IsNullOrWhiteSpace(model.RelatedParty))
        //    {
        //        whereQ += "\r\n\t\t\t\t\t" + " and svap.identify_flag = :relatedParty ";
        //        model.QueryParameters.Add("relatedParty", model.RelatedParty);
        //    }
        //    if (!string.IsNullOrWhiteSpace(model.ChineseName))
        //    {
        //        whereQ += "\r\n\t\t\t\t\t" + " and lower(svap.chinese_name) like :chinName ";
        //        model.QueryParameters.Add("chinName", "%" + model.ChineseName.ToLower() + "%");
        //    }
        //    if (!string.IsNullOrWhiteSpace(model.EnglishName))
        //    {
        //        whereQ += "\r\n\t\t\t\t\t" + " and lower(svap.english_name) like :engName ";
        //        model.QueryParameters.Add("engName", "%" + model.EnglishName.ToLower() + "%");
        //    }
        //    if (!string.IsNullOrWhiteSpace(model.CertOfRegNo))
        //    {
        //        whereQ += "\r\n\t\t\t\t\t" + " and lower(svap.certification_no) like :certRegNo ";
        //        model.QueryParameters.Add("certRegNo", "%" + model.CertOfRegNo.ToLower() + "%");
        //    }
        //    if (!string.IsNullOrWhiteSpace(model.Class))
        //    {
        //        if (model.Class.Equals("CLASS I"))
        //        {
        //            classType = "1.";
        //        }
        //        else if (model.Class.Equals("CLASS II"))
        //        {
        //            classType = "2.";
        //        }
        //        else if (model.Class.Equals("CLASS III"))
        //        {
        //            classType = "3.";
        //        }
        //        whereQ += "\r\n\t\t\t\t\t" + " and svri.MW_ITEM_CODE like :classType ";
        //        model.QueryParameters.Add("classType", "%" + classType + "%");
        //    }
        //    if (!string.IsNullOrWhiteSpace(model.ItemNo))
        //    {
        //        whereQ += "\r\n\t\t\t\t\t" + " and svri.MW_ITEM_CODE like :itemNo ";
        //        model.QueryParameters.Add("itemNo", "%" + model.ItemNo + "%");
        //    }
        //    if (!string.IsNullOrWhiteSpace(model.HandlingOfficer))
        //    {
        //        whereQ += "\r\n\t\t\t\t\t" + " and wftu.user_id = :OfficerUuid and wfi.record_Type =:recordType ";
        //        model.QueryParameters.Add("OfficerUuid", model.HandlingOfficer);
        //        model.QueryParameters.Add("recordType", SignboardConstant.WF_TYPE_VALIDATION);
        //    }
        //    if (!string.IsNullOrWhiteSpace(model.District))
        //    {
        //        whereQ += "\r\n\t\t\t\t\t" + " and lower(sva.district) like :district ";
        //        model.QueryParameters.Add("district", "%" + model.District.ToLower() + "%");
        //    }
        //    if (!string.IsNullOrWhiteSpace(model.Region))
        //    {
        //        whereQ += "\r\n\t\t\t\t\t" + " and sva.region = :region ";
        //        model.QueryParameters.Add("region", model.Region);
        //    }
        //    if (!string.IsNullOrWhiteSpace(model.Alternation))
        //    {
        //        if ("1".Equals(model.Alternation))
        //        {
        //            whereQ += "\r\n\t\t\t\t\t" + " and svr.with_Alteration = :alteration ";
        //            model.QueryParameters.Add("alteration", "Y");
        //        }
        //        else
        //        {
        //            whereQ += "\r\n\t\t\t\t\t" + " and svr.with_Alteration = :alteration or svr.with_Alteration is null";
        //            model.QueryParameters.Add("alteration", "N");
        //        }
        //    }
        //    if (model.InspectDateFrom.HasValue)
        //    {
        //        whereQ += "\r\n\t\t\t\t\t" + " and svr.INSPECTION_DATE >= :inspectDateFrom ";
        //        model.QueryParameters.Add("inspectDateFrom", model.InspectDateFrom.Value);
        //    }
        //    if (model.InspectDateTo.HasValue)
        //    {
        //        whereQ += "\r\n\t\t\t\t\t" + " and svr.INSPECTION_DATE <= :inspectDateTo ";
        //        model.QueryParameters.Add("inspectDateTo", model.InspectDateTo.Value);
        //    }
        //    if (model.CompletionDateFrom.HasValue)
        //    {
        //        whereQ += "\r\n\t\t\t\t\t" + " and svr.ACTUAL_ALTERATION_COMP_DATE >= :completeDateFrom ";
        //        model.QueryParameters.Add("completeDateFrom", model.CompletionDateFrom.Value);
        //    }
        //    if (model.InspectDateTo.HasValue)
        //    {
        //        whereQ += "\r\n\t\t\t\t\t" + " and svr.ACTUAL_ALTERATION_COMP_DATE <= :completeDateTo ";
        //        model.QueryParameters.Add("completeDateTo", model.InspectDateTo.Value);
        //    }
        //    if (model.ValidationExpiryDateFrom.HasValue)
        //    {
        //        whereQ += "\r\n\t\t\t\t\t" + " and svr.VALIDATION_EXPIRY_DATE >= :validationExpiryDateFrom ";
        //        model.QueryParameters.Add("validationExpiryDateFrom", model.ValidationExpiryDateFrom.Value);
        //    }
        //    if (model.ValidationExpiryDateTo.HasValue)
        //    {
        //        whereQ += "\r\n\t\t\t\t\t" + " and svr.VALIDATION_EXPIRY_DATE <= :validationExpiryDateTo ";
        //        model.QueryParameters.Add("validationExpiryDateTo", model.ValidationExpiryDateTo.Value);
        //    }
        //    if (model.ExpiryDateFrom.HasValue)
        //    {
        //        whereQ += "\r\n\t\t\t\t\t" + " and svv.signboard_expiry_date >= :expiryDateFrom ";
        //        model.QueryParameters.Add("expiryDateFrom", model.ExpiryDateFrom.Value);
        //    }
        //    if (model.ExpiryDateTo.HasValue)
        //    {
        //        whereQ += "\r\n\t\t\t\t\t" + " and svv.signboard_expiry_date <= :expiryDateTo ";
        //        model.QueryParameters.Add("expiryDateTo", model.ExpiryDateTo.Value);
        //    }
        //    if (!string.IsNullOrWhiteSpace(model.Recommend))
        //    {
        //        whereQ += "\r\n\t\t\t\t\t" + " and svr.RECOMMENDATION = :recommendation ";
        //        model.QueryParameters.Add("recommendation", model.Recommend);
        //    }
        //    //if (model.SignboardRemoved != null)
        //    //{
        //        if (model.SignboardRemoved == false)
        //        {
        //            whereQ += "\r\n\t\t\t\t\t" + " and svr.SIGNBOARD_REMOVAL is null ";
        //        }
        //        else if (model.SignboardRemoved == true)
        //        {
        //            whereQ += "\r\n\t\t\t\t\t" + " and svr.SIGNBOARD_REMOVAL is not null ";
        //        }
        //    //}
        //    if (!string.IsNullOrWhiteSpace(model.RelatedOrderNo))
        //    {
        //        whereQ += "\r\n\t\t\t\t\t" + " and lower(svs.S24_ORDER_NO) LIKE :relatedOrderNo ";
        //        model.QueryParameters.Add("relatedOrderNo", "%" + model.RelatedOrderNo.ToLower() + "%");
        //    }

        //    //whereQ += "\r\n\t" + " and wfu.user_id = :userId ";
        //    //model.QueryParameters.Add("userId", CurrUserId);

        //    whereQ += "\r\n\t\t\t" + ") S1";
        //    whereQ += "\r\n\t\t" + "LEFT JOIN b_SV_PERSON_CONTACT PAW ON PAW_ID = PAW.UUID";
        //    whereQ += "\r\n\t\t" + "LEFT JOIN";
        //    whereQ += "\r\n\t\t\t" + "(";
        //    whereQ += "\r\n\t\t\t\t" + "SELECT svr_id, LISTAGG(mw_item_code, ' , ') WITHIN GROUP(ORDER BY mw_item_code) AS MW_ITEM_CODE, min(MW_ITEM_CODE) AS MIN_ITEM_NO, min(REVISION) AS REVISION";
        //    whereQ += "\r\n\t\t\t\t" + "FROM B_svr_ITEM";
        //    whereQ += "\r\n\t\t\t\t" + "GROUP BY svr_id";
        //    whereQ += "\r\n\t\t\t" + ") S2";
        //    whereQ += "\r\n\t\t" + "ON S1.svr_UUID = S2.svr_id";

        //    // PAW
        //    if (!string.IsNullOrWhiteSpace(model.Paw))
        //    {
        //        whereQ += "\r\n\t" + " where (lower(PAW.NAME_CHINESE) like :searchPaw OR lower(PAW.NAME_ENGLISH) like :searchPaw) ";
        //        model.QueryParameters.Add("searchPaw", "%" + model.Paw.ToLower() + "%");
        //    }

        //    whereQ += "\r\n\t" + ") T1";
        //    whereQ += "\r\n" + "LEFT JOIN";
        //    whereQ += "\r\n\t" + "(";
        //    whereQ += "\r\n\t\t" + "SELECT svr_id, LISTAGG(validation_item, ' , ') WITHIN GROUP(ORDER BY validation_item) AS VALIDATION_ITEM";
        //    whereQ += "\r\n\t\t" + "FROM B_svr_VALIDATION_ITEM";
        //    whereQ += "\r\n\t\t" + "GROUP BY svr_id";
        //    whereQ += "\r\n\t" + ") T2";
        //    whereQ += "\r\n" + "ON T1.svr_UUID = T2.svr_id";

        //    return whereQ;
        //}

        private string SearchTDL_VS_whereQ(Fn02TDL_VSSearchModel model)
        {
            string whereQ = "";
            // string CurrUserId = "";
            string classType = "";

            string CurrUserId = SessionUtil.LoginPost.UUID;    // Current logged in user's uuid

            // Order No, Class Type, Item No: add to FROM clause
            if (!string.IsNullOrWhiteSpace(model.OrderNo) || !string.IsNullOrWhiteSpace(model.Class) || !string.IsNullOrWhiteSpace(model.ItemNo))
            {
                whereQ += ", b_sv_record_item svri";
            }

            // Handling Officer
            //if (string.IsNullOrWhiteSpace(model.HandlingOfficer))
            //{
            //    whereQ += ", b_sv_appointed_professional svap";
            //    whereQ += "\r\n\t\t\t\t" + " WHERE b_is_final_validation(svr.uuid) = 'Y'";
            //    whereQ += "\r\n\t\t\t\t\t" + " AND svv.SV_RECORD_ID=svr.uuid AND svs.uuid=svr.sv_signboard_id and sva.uuid = svs.LOCATION_ADDRESS_ID AND svap.sv_record_id = svr.uuid";
            //}
            //else
            //{
            whereQ += " , b_sv_appointed_professional svap, b_wf_info wfi, b_wf_task wft, b_wf_task_user wftu";
            whereQ += "\r\n\t\t\t\t" + " where b_is_final_validation(svr.uuid) = 'Y' and";
            whereQ += "\r\n\t\t\t\t\t" + " svv.SV_RECORD_ID = svr.uuid and";
            whereQ += "\r\n\t\t\t\t\t" + " svs.uuid = svr.sv_signboard_id and sva.uuid = svs.LOCATION_ADDRESS_ID and";
            whereQ += "\r\n\t\t\t\t\t" + " svap.sv_record_id = svr.uuid and wfi.record_id = svv.sv_record_id and wfi.uuid = wft.wf_info_id and wft.uuid = wftu.wf_task_id"; ;
            whereQ += "\r\n\t\t\t\t\t" + " and svv.spo_endorsement_date is not null"; // display the validation result of signboard submission after endorsement by SPO 
                                                                                      //}

            // Order No, Class Type, Item No: add to WHERE clause (join)
            if (!string.IsNullOrWhiteSpace(model.OrderNo) || !string.IsNullOrWhiteSpace(model.Class) || !string.IsNullOrWhiteSpace(model.ItemNo))
            {
                whereQ += " and svr.uuid=svri.SV_RECORD_ID";
            }

            if (!string.IsNullOrWhiteSpace(model.RefNo))
            {
                whereQ += "\r\n\t\t\t\t\t" + " and lower(svr.reference_no) like :fileRefNo ";
                model.QueryParameters.Add("fileRefNo", "%" + model.RefNo.Trim().ToLower() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.OrderNo))
            {
                whereQ += "\r\n\t\t\t\t\t" + " and lower(svri.revision) like :fileBdRef ";
                model.QueryParameters.Add("fileBdRef", "%" + model.OrderNo.Trim().ToLower() + "%");
            }
            if (model.ReceivedDateFrom.HasValue)
            {
                whereQ += "\r\n\t\t\t\t\t" + " and svr.RECEIVED_DATE >= :receivedDateFrom ";
                model.QueryParameters.Add("receivedDateFrom", model.ReceivedDateFrom.Value);
            }
            if (model.ReceivedDateTo.HasValue)
            {
                whereQ += "\r\n\t\t\t\t\t" + " and svr.RECEIVED_DATE <= :receivedDateTo ";
                model.QueryParameters.Add("receivedDateTo", model.ReceivedDateTo.Value);
            }
            if (!string.IsNullOrWhiteSpace(model.Status))
            {
                if (model.Status.Equals("Open"))
                {
                    whereQ += "\r\n\t\t\t\t\t" + " and svv.validation_result is null and svv.spo_endorsement_date is null";
                }
                else if (model.Status.Equals("Close"))
                {
                    whereQ += "\r\n\t\t\t\t\t" + " and svv.validation_result is not null and svv.spo_endorsement_date is not null";
                }
            }
            if (!string.IsNullOrWhiteSpace(model.Street))
            {
                whereQ += "\r\n\t\t\t\t\t" + " and lower(sva.street) like :street ";
                model.QueryParameters.Add("street", "%" + model.Street.Trim().ToLower() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.StreetNo))
            {
                whereQ += "\r\n\t\t\t\t\t" + " and lower(sva.street_no) like :streetNo ";
                model.QueryParameters.Add("streetNo", "%" + model.StreetNo.Trim().ToLower() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.Building))
            {
                whereQ += "\r\n\t\t\t\t\t" + " and lower(sva.buildingname) like :building ";
                model.QueryParameters.Add("building", "%" + model.Building.Trim().ToLower() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.RelatedParty))
            {
                whereQ += "\r\n\t\t\t\t\t" + " and svap.identify_flag = :relatedParty ";
                model.QueryParameters.Add("relatedParty", model.RelatedParty);
            }
            if (!string.IsNullOrWhiteSpace(model.ChineseName))
            {
                whereQ += "\r\n\t\t\t\t\t" + " and lower(svap.chinese_name) like :chinName ";
                model.QueryParameters.Add("chinName", "%" + model.ChineseName.Trim().ToLower() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.EnglishName))
            {
                whereQ += "\r\n\t\t\t\t\t" + " and lower(svap.english_name) like :engName ";
                model.QueryParameters.Add("engName", "%" + model.EnglishName.Trim().ToLower() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.CertOfRegNo))
            {
                whereQ += "\r\n\t\t\t\t\t" + " and lower(svap.certification_no) like :certRegNo ";
                model.QueryParameters.Add("certRegNo", "%" + model.CertOfRegNo.Trim().ToLower() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.Class))
            {
                if (model.Class.Equals("CLASS I"))
                {
                    classType = "1.";
                }
                else if (model.Class.Equals("CLASS II"))
                {
                    classType = "2.";
                }
                else if (model.Class.Equals("CLASS III"))
                {
                    classType = "3.";
                }
                whereQ += "\r\n\t\t\t\t\t" + " and svri.MW_ITEM_CODE like :classType ";
                model.QueryParameters.Add("classType", "%" + classType + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.ItemNo))
            {
                whereQ += "\r\n\t\t\t\t\t" + " and svri.MW_ITEM_CODE like :itemNo ";
                model.QueryParameters.Add("itemNo", "%" + model.ItemNo.Trim() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.HandlingOfficer))
            {
                whereQ += "\r\n\t\t\t\t\t" + " and wftu.sys_post_id = :OfficerUuid and wfi.record_Type =:recordType ";
                model.QueryParameters.Add("OfficerUuid", model.HandlingOfficer);
                model.QueryParameters.Add("recordType", SignboardConstant.WF_TYPE_VALIDATION);
            }
            if (!string.IsNullOrWhiteSpace(model.District))
            {
                whereQ += "\r\n\t\t\t\t\t" + " and lower(sva.district) like :district ";
                model.QueryParameters.Add("district", "%" + model.District.Trim().ToLower() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.Region))
            {
                whereQ += "\r\n\t\t\t\t\t" + " and sva.region = :region ";
                model.QueryParameters.Add("region", model.Region);
            }
            if (!string.IsNullOrWhiteSpace(model.Alternation))
            {
                if ("1".Equals(model.Alternation))
                {
                    whereQ += "\r\n\t\t\t\t\t" + " and svr.with_Alteration = :alteration ";
                    model.QueryParameters.Add("alteration", "Y");
                }
                else
                {
                    whereQ += "\r\n\t\t\t\t\t" + " and svr.with_Alteration = :alteration or svr.with_Alteration is null";
                    model.QueryParameters.Add("alteration", "N");
                }
            }
            if (model.InspectDateFrom.HasValue)
            {
                whereQ += "\r\n\t\t\t\t\t" + " and svr.INSPECTION_DATE >= :inspectDateFrom ";
                model.QueryParameters.Add("inspectDateFrom", model.InspectDateFrom.Value);
            }
            if (model.InspectDateTo.HasValue)
            {
                whereQ += "\r\n\t\t\t\t\t" + " and svr.INSPECTION_DATE <= :inspectDateTo ";
                model.QueryParameters.Add("inspectDateTo", model.InspectDateTo.Value);
            }
            if (model.CompletionDateFrom.HasValue)
            {
                whereQ += "\r\n\t\t\t\t\t" + " and svr.ACTUAL_ALTERATION_COMP_DATE >= :completeDateFrom ";
                model.QueryParameters.Add("completeDateFrom", model.CompletionDateFrom.Value);
            }
            if (model.InspectDateTo.HasValue)
            {
                whereQ += "\r\n\t\t\t\t\t" + " and svr.ACTUAL_ALTERATION_COMP_DATE <= :completeDateTo ";
                model.QueryParameters.Add("completeDateTo", model.InspectDateTo.Value);
            }
            if (model.ValidationExpiryDateFrom.HasValue)
            {
                whereQ += "\r\n\t\t\t\t\t" + " and svr.VALIDATION_EXPIRY_DATE >= :validationExpiryDateFrom ";
                model.QueryParameters.Add("validationExpiryDateFrom", model.ValidationExpiryDateFrom.Value);
            }
            if (model.ValidationExpiryDateTo.HasValue)
            {
                whereQ += "\r\n\t\t\t\t\t" + " and svr.VALIDATION_EXPIRY_DATE <= :validationExpiryDateTo ";
                model.QueryParameters.Add("validationExpiryDateTo", model.ValidationExpiryDateTo.Value);
            }
            if (model.ExpiryDateFrom.HasValue)
            {
                whereQ += "\r\n\t\t\t\t\t" + " and svv.signboard_expiry_date >= :expiryDateFrom ";
                model.QueryParameters.Add("expiryDateFrom", model.ExpiryDateFrom.Value);
            }
            if (model.ExpiryDateTo.HasValue)
            {
                whereQ += "\r\n\t\t\t\t\t" + " and svv.signboard_expiry_date <= :expiryDateTo ";
                model.QueryParameters.Add("expiryDateTo", model.ExpiryDateTo.Value);
            }
            if (!string.IsNullOrWhiteSpace(model.Recommend))
            {
                whereQ += "\r\n\t\t\t\t\t" + " and svr.RECOMMENDATION = :recommendation ";
                model.QueryParameters.Add("recommendation", model.Recommend);
            }
            //if (model.SignboardRemoved != null)
           // {
                if (model.SignboardRemoved == false)
                {
                    whereQ += "\r\n\t\t\t\t\t" + " and svr.SIGNBOARD_REMOVAL is null ";
                }
                else if (model.SignboardRemoved == true)
                {
                    whereQ += "\r\n\t\t\t\t\t" + " and svr.SIGNBOARD_REMOVAL is not null ";
                }
           // }
            if (!string.IsNullOrWhiteSpace(model.RelatedOrderNo))
            {
                whereQ += "\r\n\t\t\t\t\t" + " and lower(svs.S24_ORDER_NO) LIKE :relatedOrderNo ";
                model.QueryParameters.Add("relatedOrderNo", "%" + model.RelatedOrderNo.ToLower() + "%");
            }

            //whereQ += "\r\n\t" + " and wftu.sys_post_id = :userId ";
            //model.QueryParameters.Add("userId", CurrUserId);
           



            whereQ += "\r\n\t\t\t" + ") S1";
            whereQ += "\r\n\t\t" + "LEFT JOIN b_SV_PERSON_CONTACT PAW ON PAW_ID = PAW.UUID";
            whereQ += "\r\n\t\t" + "LEFT JOIN";
            whereQ += "\r\n\t\t\t" + "(";
            whereQ += "\r\n\t\t\t\t" + "SELECT sv_record_id, LISTAGG(mw_item_code, ' , ') WITHIN GROUP(ORDER BY mw_item_code) AS MW_ITEM_CODE, min(MW_ITEM_CODE) AS MIN_ITEM_NO, min(REVISION) AS REVISION";
            whereQ += "\r\n\t\t\t\t" + "FROM B_SV_RECORD_ITEM";
            whereQ += "\r\n\t\t\t\t" + "GROUP BY sv_record_id";
            whereQ += "\r\n\t\t\t" + ") S2";
            whereQ += "\r\n\t\t" + "ON S1.SV_RECORD_UUID = S2.sv_record_id";

            // PAW
            if (!string.IsNullOrWhiteSpace(model.Paw))
            {
                whereQ += "\r\n\t" + " where (lower(PAW.NAME_CHINESE) like :searchPaw OR lower(PAW.NAME_ENGLISH) like :searchPaw) ";
                model.QueryParameters.Add("searchPaw", "%" + model.Paw.ToLower() + "%");
            }

            whereQ += "\r\n\t" + ") T1";
            whereQ += "\r\n" + "LEFT JOIN";
            whereQ += "\r\n\t" + "(";
            whereQ += "\r\n\t\t" + "SELECT sv_record_id, LISTAGG(validation_item, ' , ') WITHIN GROUP(ORDER BY validation_item) AS VALIDATION_ITEM";
            whereQ += "\r\n\t\t" + "FROM B_SV_RECORD_VALIDATION_ITEM";
            whereQ += "\r\n\t\t" + "GROUP BY sv_record_id";
            whereQ += "\r\n\t" + ") T2";
            whereQ += "\r\n" + "ON T1.SV_RECORD_UUID = T2.sv_record_id";
            whereQ += "\r\n\t\t LEFT JOIN B_SV_SCANNED_DOCUMENT sdd ON REF_NO = sdd.RECORD_ID ";
            whereQ += " \r\n WHERE sdd.AS_THUMBNAIL = 'Y'";

            return whereQ;
        }

        public string ExportTDL_VS(Fn02TDL_VSSearchModel model)
        {
            model.Query = SearchTDL_VS_q;
            model.QueryWhere = SearchTDL_VS_whereQ(model);

            // customized excel columns
            model.Columns = new List<Dictionary<string, string>>()
                .Append(new Dictionary<string, string> { ["columnName"] = "REF_NO", ["displayName"] = "Submission No." })
                .Append(new Dictionary<string, string> { ["columnName"] = "STATUS", ["displayName"] = "Status" })
                .Append(new Dictionary<string, string> { ["columnName"] = "CLASS", ["displayName"] = "Class" })
                .Append(new Dictionary<string, string> { ["columnName"] = "RECEIVED_DATE", ["displayName"] = "Received Date" })
                .Append(new Dictionary<string, string> { ["columnName"] = "MW_ITEM_CODE", ["displayName"] = "Item No." })
                .Append(new Dictionary<string, string> { ["columnName"] = "ALTERNATION", ["displayName"] = "With Alteration/Strengthening" })
                .Append(new Dictionary<string, string> { ["columnName"] = "REVISION", ["displayName"] = "Order No./ Direction/ Notice No./ BD file reference number" })
                .Append(new Dictionary<string, string> { ["columnName"] = "RECOMMENDATION", ["displayName"] = "Recommendation" })
                .Append(new Dictionary<string, string> { ["columnName"] = "LOCATION", ["displayName"] = "Signboard Location" })
                .Append(new Dictionary<string, string> { ["columnName"] = "", ["displayName"] = " Case Officer" }).ToArray();

            return model.Export("Validation Search Report");
        }


        // -------------- Signboard Search
        string SearchTDL_SS_q = ""
            + "\r\n" + "select svr.reference_no AS REF_NO, sva.buildingname AS BUILDING_NAME, sva.street AS STREET,"
            + "\r\n\t" + "sva.street_no AS STREET_NO, svs.facade AS FACADE, svs.type AS TYPE, svs.btm_floor AS BOTTOM,"
            + "\r\n\t" + "svs.top_floor AS TOP, svs.a_m2 AS DISPLAY_AREA, svs.p_m AS PROJECTION, svs.h_m AS HEIGHT, svs.h2_m AS CLEARANCE, svs.led AS LED, svs.building_portion AS BUILDING_PORTION, svr.form_code AS FORM_CODE, svs.uuid, svs.new_item_number,"
            + "\r\n\t" + "svpc.NAME_CHINESE AS CHIN_NAME, svpc.NAME_ENGLISH AS ENG_NAME, sva2.FULL_ADDRESS AS ADDRESS, svr.SIGNBOARD_REMOVAL_DISCOV_DATE AS REMOVAL_DATE, svv.SIGNBOARD_EXPIRY_DATE AS EXPIRY_DATE"
            + "\r\n\t" + ", DECODE(svv.VALIDATION_RESULT, 'A', 'Accept', 'R', 'Refuse', 'CA', 'Conditional Accept', '', '') VALIDATION_RESULT"
            + "\r\n\t" + ", svr.uuid AS svr_uuid, svr.received_date AS RECEIVED_DATE, svr.with_Alteration AS ALTERNATION, REVISION"
            + "\r\n\t" + ", svr.RECOMMENDATION AS RECOMMENDATION, svs.location_of_signboard AS LOCATION, MW_ITEM_CODE"
             + "\r\n\t" + ", CASE WHEN (svv.validation_result is null AND svv.spo_endorsement_date is null) THEN 'Open' WHEN (svv.validation_result is not null and svv.spo_endorsement_date is not null) THEN 'Close' END AS STATUS "
            + "\r\n\t" + ", CASE WHEN MIN_ITEM_NO LIKE '1.%' THEN 'Class I' WHEN MIN_ITEM_NO LIKE '2.%' THEN 'Class II' WHEN MIN_ITEM_NO LIKE '3.%' THEN 'Class III' END AS CLASS "
            + "\r\n\t" + ", ssd.file_path AS THUMBNAIL"
            + "\r\n" + "from b_sv_signboard svs"
            + "\r\n\t" + "left join b_sv_record svr on svr.sv_signboard_id = svs.uuid"
            + "\r\n\t" + "left join b_sv_address sva  on svs.location_address_id = sva.uuid"
            + "\r\n\t" + "LEFT JOIN b_SV_PERSON_CONTACT svpc  ON svs.OWNER_ID = svpc.UUID"
            + "\r\n\t" + "LEFT JOIN b_SV_PERSON_CONTACT PAW ON svr.PAW_ID = PAW.UUID"
            + "\r\n\t" + "LEFT JOIN b_SV_ADDRESS sva2 ON sva2.UUID = svpc.sv_address_id"
            + "\r\n\t" + "LEFT JOIN B_SV_VALIDATION svv ON svv.SV_RECORD_ID = svr.UUID"
            + "\r\n\t" + "LEFT JOIN B_SV_SCANNED_DOCUMENT ssd ON svr.reference_no = ssd.RECORD_ID"
            + "\r\n\t" + "LEFT JOIN ("
            + "\r\n\t\t" + "SELECT sv_record_id AS inner_sv_record_id, LISTAGG(mw_item_code, ' , ') WITHIN GROUP(ORDER BY mw_item_code) AS MW_ITEM_CODE, min(MW_ITEM_CODE) AS MIN_ITEM_NO, min(REVISION) AS REVISION"
            + "\r\n\t\t" + "FROM b_sv_record_ITEM"
            + "\r\n\t\t" + "GROUP BY sv_record_id"
            + "\r\n\t\t" + ") ON svr.uuid = inner_sv_record_id"
            ;

        public Fn02TDL_SSSearchModel SearchTDL_SS(Fn02TDL_SSSearchModel model)
        {
            model.Query = SearchTDL_SS_q;
            model.QueryWhere = SearchTDL_SS_whereQ(model);
            model.Search();

            if (model.Data != null)
            {
                SignboardTDLService tdl = new SignboardTDLService();
                for (int i = 0; i < model.Data.Count; i++)
                {
                    model.Data[i]["THUMBNAIL"] = tdl.checkThumbnailFilePath(model.Data[i]["THUMBNAIL"].ToString());
                }
            }

            return model;
        }
        private string SearchTDL_SS_whereQ(Fn02TDL_SSSearchModel model)
        {
            string whereQ = "\r\n" + "where 1 = 1 AND ssd.AS_THUMBNAIL = 'Y'";

            // Submission no.
            if (!string.IsNullOrWhiteSpace(model.RefNo))
            {
                whereQ += "\r\n\t" + " and lower(svr.reference_no) like :SearchSbNo ";
                model.QueryParameters.Add("SearchSbNo", "%" + model.RefNo.ToLower() + "%");
            }
            // MW submission no. (x)
            // 4+2
            if (!string.IsNullOrWhiteSpace(model.FileRefNo)) {
                whereQ += "\r\n\t" + " and lower(sva.FILE_REFERENCE_NO) like :searchFileRefNo ";
                model.QueryParameters.Add("searchFileRefNo", "%" + model.FileRefNo.ToLower() + "%");
            }
            // Validation submission no. (x)
            // Record type
            if (!string.IsNullOrWhiteSpace(model.RecordType)) {
                if(model.RecordType.Equals(SignboardConstant.VALIDATED)) {
                    whereQ += "\r\n\t" + " and b_is_final_record(svr.uuid) = 'Y' ";
                }
                if(model.RecordType.Equals(SignboardConstant.NOT_VALIDATED)) {
                    whereQ += "\r\n\t" + " and svs.validated = 'N' ";
                }
            }
            else {
                whereQ += "\r\n\t" + " and (b_is_final_record(svr.uuid) = 'Y' or svs.validated = 'N') ";
            }
            // Related Order No
            if (!string.IsNullOrWhiteSpace(model.RelatedOrderNo))
            {
                whereQ += "\r\n\t\t\t\t\t" + " and lower(svs.S24_ORDER_NO) LIKE :relatedOrderNo ";
                model.QueryParameters.Add("relatedOrderNo", "%" + model.RelatedOrderNo.ToLower() + "%");
            }
            // Expiry date
            if (model.ExpiryDateFrom.HasValue)
            {
                whereQ += "\r\n\t\t\t\t\t" + " and svv.signboard_expiry_date >= :expiryDateFrom ";
                model.QueryParameters.Add("expiryDateFrom", model.ExpiryDateFrom.Value);
            }
            if (model.ExpiryDateTo.HasValue)
            {
                whereQ += "\r\n\t\t\t\t\t" + " and svv.signboard_expiry_date <= :expiryDateTo ";
                model.QueryParameters.Add("expiryDateTo", model.ExpiryDateTo.Value);
            }
            // Facade
            if (!string.IsNullOrWhiteSpace(model.Facade))
            {
                whereQ += "\r\n\t" + " and lower(svs.facade) = :searchFacade ";
                model.QueryParameters.Add("searchFacade", model.Facade.ToLower());
            }
            // Type
            if (!string.IsNullOrWhiteSpace(model.Type))
            {
                whereQ += "\r\n\t" + " and lower(svs.type) = :searchType ";
                model.QueryParameters.Add("searchType", model.Type.ToLower());
            }
            // Bottom fixing at floor
            if (!string.IsNullOrWhiteSpace(model.BotFixingAtFloor))
            {
                whereQ += "\r\n\t" + " and svs.btm_floor = :searchBottom ";
                model.QueryParameters.Add("searchBottom", model.BotFixingAtFloor);
            }
            // Top fixing at floor
            if (!string.IsNullOrWhiteSpace(model.TopFixingAtFloor))
            {
                whereQ += "\r\n\t" + " and svs.top_floor = :searchTop ";
                model.QueryParameters.Add("searchTop", model.TopFixingAtFloor);
            }
            // Display area
            if (!string.IsNullOrWhiteSpace(model.DisplayArea))
            {
                whereQ += "\r\n\t" + " and svs.a_m2 = :searchDisplayArea ";
                model.QueryParameters.Add("searchDisplayArea", model.DisplayArea);
            }
            // Projection
            if (!string.IsNullOrWhiteSpace(model.Projection))
            {
                whereQ += "\r\n\t" + " and svs.p_m = :searchProjection ";
                model.QueryParameters.Add("searchProjection", model.Projection);
            }
            // Height of sb
            if (!string.IsNullOrWhiteSpace(model.HeightOfSb))
            {
                whereQ += "\r\n\t" + " and svs.h_m = :searchHeight ";
                model.QueryParameters.Add("searchHeight", model.HeightOfSb);
            }
            // Clearance abouve ground
            if (!string.IsNullOrWhiteSpace(model.ClearanceAbvGround))
            {
                whereQ += "\r\n\t" + " and svs.h2_m = :searchClearance ";
                model.QueryParameters.Add("searchClearance", model.ClearanceAbvGround);
            }
            // LED/TV
            if (!string.IsNullOrWhiteSpace(model.LedOrTv))
            {
                whereQ += "\r\n\t" + " and svs.led = :searchLed ";
                model.QueryParameters.Add("searchLed", model.LedOrTv);
            }
            // Building portion
            if (!string.IsNullOrWhiteSpace(model.BlgPortion))
            {
                whereQ += "\r\n\t" + " and lower(svs.building_portion) = :searchBuildingPortion ";
                model.QueryParameters.Add("searchBuildingPortion", model.BlgPortion.ToLower());
            }
            // Street
            if (!string.IsNullOrWhiteSpace(model.Street))
            {
                whereQ += "\r\n\t" + " and lower(sva.street) like :street ";
                model.QueryParameters.Add("street", "%" + model.Street.ToLower() + "%");
            }
            // Street no.
            if (!string.IsNullOrWhiteSpace(model.StreetNo))
            {
                whereQ += "\r\n\t" + " and lower(sva.street_no) like :streetNo ";
                model.QueryParameters.Add("streetNo", "%" + model.StreetNo.ToLower() + "%");
            }
            // Building/Block
            if (!string.IsNullOrWhiteSpace(model.Building))
            {
                whereQ += "\r\n\t" + " and lower(sva.buildingname) like :building ";
                model.QueryParameters.Add("building", "%" + model.Building.ToLower() + "%");
            }
            // District
            if (!string.IsNullOrWhiteSpace(model.District))
            {
                whereQ += "\r\n\t" + " and lower(sva.district) like :district ";
                model.QueryParameters.Add("district", "%" + model.District.ToLower() + "%");
            }
            // Region
            if (!string.IsNullOrWhiteSpace(model.Region))
            {
                whereQ += "\r\n\t\t\t\t\t" + " and sva.region = :region ";
                model.QueryParameters.Add("region", model.Region);
            }
            // BCIS area code
            if (!string.IsNullOrWhiteSpace(model.BcisAreaCode))
            {
                whereQ += "\r\n\t" + " and sva.bcis_district = :bcisAreaCode ";
                model.QueryParameters.Add("bcisAreaCode", model.BcisAreaCode);
            }
            // Sb owner
            if (!string.IsNullOrWhiteSpace(model.SbOwner))
            {
                whereQ += "\r\n\t" + " and (lower(svpc.NAME_CHINESE) like :searchOwner OR lower(svpc.NAME_ENGLISH) like :searchOwner) ";
                model.QueryParameters.Add("searchOwner", "%" + model.SbOwner.ToLower() + "%");
            }
            // PAW
            if (!string.IsNullOrWhiteSpace(model.Paw))
            {
                whereQ += "\r\n\t" + " and (lower(PAW.NAME_CHINESE) like :searchPaw OR lower(PAW.NAME_ENGLISH) like :searchPaw) ";
                model.QueryParameters.Add("searchPaw", "%" + model.Paw.ToLower() + "%");
            }
            // Source of info


            return whereQ;
        }

        public string ExportTDL_SS(Fn02TDL_SSSearchModel model)
        {
            model.Query = SearchTDL_SS_q;
            model.QueryWhere = SearchTDL_SS_whereQ(model);

            // customized excel columns
            model.Columns = new List<Dictionary<string, string>>()
                .Append(new Dictionary<string, string> { ["columnName"] = "REF_NO", ["displayName"] = "Submission No." })
                .Append(new Dictionary<string, string> { ["columnName"] = "STATUS", ["displayName"] = "Status" })
                .Append(new Dictionary<string, string> { ["columnName"] = "CLASS", ["displayName"] = "Class" })
                .Append(new Dictionary<string, string> { ["columnName"] = "RECEIVED_DATE", ["displayName"] = "Received Date" })
                .Append(new Dictionary<string, string> { ["columnName"] = "MW_ITEM_CODE", ["displayName"] = "Item No." })
                .Append(new Dictionary<string, string> { ["columnName"] = "ALTERNATION", ["displayName"] = "With Alteration/Strengthening" })
                .Append(new Dictionary<string, string> { ["columnName"] = "REVISION", ["displayName"] = "Order No./ Direction/ Notice No./ BD file reference number" })
                .Append(new Dictionary<string, string> { ["columnName"] = "RECOMMENDATION", ["displayName"] = "Recommendation" })
                .Append(new Dictionary<string, string> { ["columnName"] = "LOCATION", ["displayName"] = "Signboard Location" })
                .Append(new Dictionary<string, string> { ["columnName"] = "", ["displayName"] = " Case Officer" }).ToArray();

            return model.Export("Signboard Search Report");
        }

        public Fn02TDL_SSDisplayModel ViewSignboard(string id)  // signboard uuid
        {
            using (EntitiesSignboard db = new EntitiesSignboard()) {

                // check user role --> privacy
                bool show_privacy = false;
                show_privacy = true;

                B_SV_SIGNBOARD svs = db.B_SV_SIGNBOARD.Where(x => x.UUID == id).FirstOrDefault();
                B_SV_RECORD svr = db.B_SV_RECORD.Where(x => x.SV_SIGNBOARD_ID == id).FirstOrDefault();
                B_SV_ADDRESS sva = db.B_SV_ADDRESS.Where(x => x.UUID == svs.LOCATION_ADDRESS_ID).FirstOrDefault();

                var svv = (from svr0 in db.B_SV_RECORD
                              join svv0 in db.B_SV_VALIDATION on svr0.UUID equals svv0.SV_RECORD_ID into svvalidation
                              from svvalidation0 in svvalidation.DefaultIfEmpty()
                              //where svvalidation0.SPO_ENDORSEMENT_DATE != null 
                              where svr0.SV_SIGNBOARD_ID == id
                              select new SvValidationDisplayModel
                              {
                                  SignboardRefNo = svr0.REFERENCE_NO,
                                  FormCode = svr0.FORM_CODE,
                                  ReceivedDate = svr0.RECEIVED_DATE,
                                  ExpiryDate = svvalidation0.SIGNBOARD_EXPIRY_DATE,
                                  ValidationResult = svvalidation0.VALIDATION_RESULT  // A, R, CA
                              }).ToList();


                B_SV_SCANNED_DOCUMENT svsd_thumbnail = db.B_SV_SCANNED_DOCUMENT.Where(x => x.RECORD_ID == svr.REFERENCE_NO).Where(y => y.AS_THUMBNAIL == "Y").FirstOrDefault();

                B_SV_PERSON_CONTACT paw = db.B_SV_PERSON_CONTACT.Where(x => x.UUID == svr.PAW_ID).FirstOrDefault();
                B_SV_ADDRESS sva_paw = db.B_SV_ADDRESS.Where(x => x.UUID == paw.SV_ADDRESS_ID).FirstOrDefault();

                B_SV_PERSON_CONTACT svpc = db.B_SV_PERSON_CONTACT.Where(x => x.UUID == svs.OWNER_ID).FirstOrDefault();
                B_SV_ADDRESS sva_svpc = db.B_SV_ADDRESS.Where(x => x.UUID == svpc.SV_ADDRESS_ID).FirstOrDefault();

                B_SV_PERSON_CONTACT oi = db.B_SV_PERSON_CONTACT.Where(x => x.UUID == svr.OI_ID).FirstOrDefault();
                B_SV_ADDRESS sva_oi = db.B_SV_ADDRESS.Where(x => x.UUID == oi.SV_ADDRESS_ID).FirstOrDefault();

                List<B_SV_SCANNED_DOCUMENT> svsd = db.B_SV_SCANNED_DOCUMENT.Where(x => x.RECORD_ID == svr.REFERENCE_NO).OrderBy(x => x.CREATED_DATE).ToList();

                List<B_SV_PHOTO_LIBRARY> svpl = db.B_SV_PHOTO_LIBRARY.Where(x => x.SV_SIGNBOARD_ID == svs.UUID).OrderBy(x => x.CREATED_DATE).ToList();

                List<B_SV_SIGNBOARD_RELATION> svsr1 = db.B_SV_SIGNBOARD_RELATION.Where(x => x.SIGNBOARD_NO_1 == svr.REFERENCE_NO).ToList();
                List<B_SV_SIGNBOARD_RELATION> svsr2 = db.B_SV_SIGNBOARD_RELATION.Where(x => x.SIGNBOARD_NO_2 == svr.REFERENCE_NO).ToList();
                // Combine 2 lists
                List<string> svsr = new List<string>();
                if(svsr1 != null && svsr1.Count() > 0)
                {
                    foreach (var item in svsr1)
                    {
                        svsr.Add(item.SIGNBOARD_NO_2);
                    }
                }
                if(svsr2 != null && svsr2.Count() > 0)
                {
                    foreach (var item in svsr2)
                    {
                        if (!svsr.Contains(item.SIGNBOARD_NO_1))
                        {
                            svsr.Add(item.SIGNBOARD_NO_1);
                        }
                    }
                }
                if(svsr != null && svsr.Count() > 0)
                {
                    svsr.Sort();
                }

                List<B_SV_24_ORDER> svo = db.B_SV_24_ORDER.Where(x => x.REFERENCE_NUMBER == svr.REFERENCE_NO).ToList();

                List<B_SV_GC> svgc = db.B_SV_GC.Where(x => x.REFERENCE_NUMBER == svr.REFERENCE_NO).ToList();

                List<B_SV_COMPLAIN> svc = db.B_SV_COMPLAIN.Where(x => x.REFERENCE_NUMBER == svr.REFERENCE_NO).ToList();

                string status = svr != null ? getStatusByReceivedDateAndSCNo(svr.RECEIVED_DATE, svr.REFERENCE_NO) : null;

                return new Fn02TDL_SSDisplayModel()
                {
                    ShowPrivacy = show_privacy,
                    ValidationList = (svv != null && svv.Count > 0 ? svv : null),
                    SubmissionNo = (svr != null ? svr.REFERENCE_NO : null),
                    Description = svs.DESCRIPTION,
                    Facade = svs.FACADE,
                    Type = svs.TYPE,
                    BtmFloor = svs.BTM_FLOOR,
                    TopFloor = svs.TOP_FLOOR,
                    Area = svs.A_M2,
                    Projection = svs.P_M,
                    Height = svs.H_M,
                    Clearance = svs.H2_M,
                    Led = svs.LED,
                    BuildingPortion = svs.BUILDING_PORTION,
                    Thumbnail = (svsd_thumbnail != null ? checkThumbnailFilePath(svsd_thumbnail.FILE_PATH) : "No Thumbnail"),
                    SignboardFlat = (sva != null ? sva.FLAT : null),
                    SignboardFloor = (sva != null ? sva.FLOOR : null),
                    SignboardBlock = (sva != null ? sva.BLOCK : null),
                    SignboardBuilding = (sva != null ? sva.BUILDINGNAME : null),
                    SignboardStreetNo = (sva != null ? sva.STREET_NO : null),
                    SignboardStreet = (sva != null ? sva.STREET : null),
                    SignboardDistrict = (sva != null ? sva.DISTRICT : null),
                    SignboardRegion = (sva != null ? sva.REGION : null), // HK, KLW, NT
                    SignboardRvdNo = svs.RVD_NO,
                    PawChiName = (paw != null ? paw.NAME_CHINESE : null),
                    PawEngName = (paw != null ? paw.NAME_ENGLISH : null),
                    PawIdNo = (paw != null ? paw.ID_NUMBER : null),
                    PawIdType = (paw != null ? paw.ID_TYPE : null),
                    PawIdIssueCountry = (paw != null ? paw.ID_ISSUE_COUNTRY : null),
                    PawFlat = (sva_paw != null ? sva_paw.FLAT : null),
                    PawFloor = (sva_paw != null ? sva_paw.FLOOR : null),
                    PawBlock = (sva_paw != null ? sva_paw.BLOCK : null),
                    PawBuilding = (sva_paw != null ? sva_paw.BUILDINGNAME : null),
                    PawStreetNo = (sva_paw != null ? sva_paw.STREET_NO : null),
                    PawStreet = (sva_paw != null ? sva_paw.STREET : null),
                    PawDistrict = (sva_paw != null ? sva_paw.DISTRICT : null),
                    PawRegion = (sva_paw != null ? sva_paw.REGION : null),
                    PawEmail = (sva_paw != null ? paw.EMAIL : null),
                    PawContact = (sva_paw != null ? paw.CONTACT_NO : null),
                    PawFax = (sva_paw != null ? paw.FAX_NO : null),
                    OwnerChiName = (svpc != null ? svpc.NAME_CHINESE : null),
                    OwnerEngName = (svpc != null ? svpc.NAME_ENGLISH : null),
                    OwnerIdNo = (svpc != null ? svpc.ID_NUMBER : null),
                    OwnerIdType = (svpc != null ? svpc.ID_TYPE : null),
                    OwnerIdIssueCountry = (svpc != null ? svpc.ID_ISSUE_COUNTRY : null),
                    OwnerFlat = (sva_svpc != null ? sva_svpc.FLAT : null),
                    OwnerFloor = (sva_svpc != null ? sva_svpc.FLOOR : null),
                    OwnerBlock = (sva_svpc != null ? sva_svpc.BLOCK : null),
                    OwnerBuilding = (sva_svpc != null ? sva_svpc.BUILDINGNAME : null),
                    OwnerStreetNo = (sva_svpc != null ? sva_svpc.STREET_NO : null),
                    OwnerStreet = (sva_svpc != null ? sva_svpc.STREET_NO : null),
                    OwnerDistrict = (sva_svpc != null ? sva_svpc.DISTRICT : null),
                    OwnerRegion = (sva_svpc != null ? sva_svpc.REGION : null),
                    OwnerEmail = (svpc != null ? svpc.EMAIL : null),
                    OwnerContact = (svpc != null ? svpc.CONTACT_NO : null),
                    OwnerFax = (svpc != null ? svpc.FAX_NO : null),
                    OiChiName = (oi != null ? oi.NAME_CHINESE : null),
                    OiEngName = (oi != null ? oi.NAME_ENGLISH : null),
                    OiIdNo = (oi != null ? oi.ID_NUMBER : null),
                    OiIdType = (oi != null ? oi.ID_TYPE : null),
                    OiIdIssueCountry = (oi != null ? oi.ID_ISSUE_COUNTRY : null),
                    OiFlat = (sva_oi != null ? sva_oi.FLAT : null),
                    OiFloor = (sva_oi != null ? sva_oi.FLOOR : null),
                    OiBlock = (sva_oi != null ? sva_oi.BLOCK : null),
                    OiBuilding = (sva_oi != null ? sva_oi.BUILDINGNAME : null),
                    OiStreetNo = (sva_oi != null ? sva_oi.STREET_NO : null),
                    OiStreet = (sva_oi != null ? sva_oi.STREET_NO : null),
                    OiDistrict = (sva_oi != null ? sva_oi.DISTRICT : null),
                    OiRegion = (sva_oi != null ? sva_oi.REGION : null),
                    OiEmail = (oi != null ? oi.EMAIL : null),
                    OiContact = (oi != null ? oi.CONTACT_NO : null),
                    OiFax = (oi != null ? oi.FAX_NO : null),
                    OiPrcName = (oi != null ? oi.PRC_NAME : null),
                    OiPrcContact = (oi != null ? oi.PRC_CONTACT_NO : null),
                    OiPbpName = (oi != null ? oi.PBP_NAME : null),
                    OiPbpContact = (oi != null ? oi.PBP_CONTACT_NO : null),
                    DocList = (svsd != null && svsd.Count() > 0 ? svsd : null),
                    PhotoList = (svpl != null && svpl.Count() > 0 ? svpl : null),
                    SignboardRelationList = (svsr != null && svsr.Count() > 0 ? svsr : null),
                    SvOrderList = (svo != null && svo.Count() > 0 ? svo : null),
                    SvGcList = (svgc != null && svgc.Count() > 0 ? svgc : null),
                    SvComplainList = (svc != null && svc.Count() > 0 ? svc : null),
                    Status = status,
                };
            }
            
        }

        public Fn02TDL_VSDisplayModel ViewVS(string refNo) // B_SV_VALIDATION
        {
            Fn02TDL_VSDisplayModel model = new Fn02TDL_VSDisplayModel();

            model.ShowPrivacy = true; // security right

            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                //model.B_SV_VALIDATION = db.B_SV_VALIDATION.Where(x => x.UUID == uuid).FirstOrDefault();
                //string svRecordId = model.B_SV_VALIDATION.SV_RECORD_ID;
                model.B_SV_RECORD = db.B_SV_RECORD.Where(o => o.REFERENCE_NO == refNo)
                                                 .Include(o => o.B_SV_VALIDATION)
                                                 .Include(o => o.B_SV_SIGNBOARD)
                                                 .Include(o => o.B_SV_RECORD_ITEM) //SIGNBOARD INFO
                                                 .Include(o => o.B_SV_SIGNBOARD.B_SV_ADDRESS)
                                                 .Include(o => o.B_SV_PERSON_CONTACT)  //OI
                                                 .Include(o => o.B_SV_PERSON_CONTACT.B_SV_ADDRESS)
                                                 .Include(o => o.B_SV_PERSON_CONTACT1)  //PAW
                                                 .Include(o => o.B_SV_PERSON_CONTACT1.B_SV_ADDRESS)
                                                 .Include(o => o.B_SV_SIGNBOARD.B_SV_PERSON_CONTACT)  //OWNER
                                                 .Include(o => o.B_SV_SIGNBOARD.B_SV_PERSON_CONTACT.B_SV_ADDRESS)
                                                 .FirstOrDefault();

                SvAppointedProfessionalDAOService SvAppointedProfessionalDAOService = new SvAppointedProfessionalDAOService();
                model.AP = SvAppointedProfessionalDAOService.getSvAppointedProfessional(model.B_SV_RECORD.UUID, SignboardConstant.PBP_CODE_AP);
                model.RSE = SvAppointedProfessionalDAOService.getSvAppointedProfessional(model.B_SV_RECORD.UUID, SignboardConstant.PBP_CODE_RSE);
                model.RGE = SvAppointedProfessionalDAOService.getSvAppointedProfessional(model.B_SV_RECORD.UUID, SignboardConstant.PBP_CODE_RGE);
                model.PRC = SvAppointedProfessionalDAOService.getSvAppointedProfessional(model.B_SV_RECORD.UUID, SignboardConstant.PRC_CODE);

                List<B_SV_SCANNED_DOCUMENT> svsd = db.B_SV_SCANNED_DOCUMENT.Where(x => x.RECORD_ID == model.B_SV_RECORD.REFERENCE_NO).OrderBy(x => x.CREATED_DATE).ToList();
                if(svsd != null && svsd.Count() > 0)
                {
                    model.DocList = svsd;
                }
                else
                {
                    model.DocList = null;
                }
                List<B_SV_PHOTO_LIBRARY> svpl = db.B_SV_PHOTO_LIBRARY.Where(x => x.SV_SIGNBOARD_ID == model.B_SV_RECORD.SV_SIGNBOARD_ID).ToList();
                if(svpl != null && svpl.Count() > 0)
                {
                    model.PhotoList = svpl;
                }
                else
                {
                    model.PhotoList = null;
                }

                List<B_SV_SIGNBOARD_RELATION> svsr1 = db.B_SV_SIGNBOARD_RELATION.Where(x => x.SIGNBOARD_NO_1 == model.B_SV_RECORD.REFERENCE_NO).ToList();
                List<B_SV_SIGNBOARD_RELATION> svsr2 = db.B_SV_SIGNBOARD_RELATION.Where(x => x.SIGNBOARD_NO_2 == model.B_SV_RECORD.REFERENCE_NO).ToList();
                // Combine 2 lists
                List<string> svsr = new List<string>();
                if (svsr1 != null && svsr1.Count() > 0)
                {
                    foreach (var item in svsr1)
                    {
                        svsr.Add(item.SIGNBOARD_NO_2);
                    }
                }
                if (svsr2 != null && svsr2.Count() > 0)
                {
                    foreach (var item in svsr2)
                    {
                        if (!svsr.Contains(item.SIGNBOARD_NO_1))
                        {
                            svsr.Add(item.SIGNBOARD_NO_1);
                        }
                    }
                }
                if (svsr != null && svsr.Count() > 0)
                {
                    svsr.Sort();
                    model.SignboardRelationList = svsr;
                }
                else
                {
                    model.SignboardRelationList = null;
                }
                if(!string.IsNullOrWhiteSpace(model.B_SV_RECORD.REFERENCE_NO))
                {
                    List<List<object>> processHandlings = SearchValidationTask(model.B_SV_RECORD.REFERENCE_NO);
                    if(processHandlings != null && processHandlings.Count() > 0)
                    {
                        model.ProcessHandlings = new List<ProcessHandlingDisplayModel>();
                        for(int i = 0; i < processHandlings.Count; i++)
                        {
                            ProcessHandlingDisplayModel result = new ProcessHandlingDisplayModel();
                            result.Uuid = (string) processHandlings[i][0];
                            if(processHandlings[i][5] != DBNull.Value)
                            {
                                result.FormCode = (string) processHandlings[i][5];
                            }
                            else
                            {
                                result.FormCode = "";
                            }
                            if(processHandlings[i][6] != DBNull.Value)
                            {
                                result.ReceivedDate = (DateTime) processHandlings[i][6];
                            }
                            else
                            {
                                result.ReceivedDate = null;
                            }
                            if(processHandlings[i][7] != DBNull.Value)
                            {
                                result.ValidationResult = (string) processHandlings[i][7];
                            }
                            else
                            {
                                result.ValidationResult = "";
                            }
                            if (processHandlings[i][2] != DBNull.Value)
                            {
                                result.ExpiryDate = (DateTime) processHandlings[i][2];
                            }
                            else
                            {
                                result.ExpiryDate = null;
                            }
                            model.ProcessHandlings.Add(result);
                            model.svValidationUuid = model.ProcessHandlings[0].Uuid;
                        }
                    }
                    else
                    {
                        model.ProcessHandlings = null;
                    }
                }
                else
                {
                    model.ProcessHandlings = null;
                }
                WorkFlowDAOService workFlowDAOService = new WorkFlowDAOService();
                List<List<object>> taskUserList = new List<List<object>>();
                taskUserList = workFlowDAOService.getWFTaskUser(SignboardConstant.WF_TYPE_VALIDATION, model.B_SV_RECORD.UUID, SignboardConstant.WF_MAP_VALIDATION_TO);
                if(taskUserList != null && taskUserList.Count() > 0)
                {
                    model.To = getUserListDisplay(taskUserList);
                }
                else
                {
                    model.To = "";
                }
                taskUserList = workFlowDAOService.getWFTaskUser(SignboardConstant.WF_TYPE_VALIDATION, model.B_SV_RECORD.UUID, SignboardConstant.WF_MAP_VALIDATION_PO);
                if (taskUserList != null && taskUserList.Count() > 0)
                {
                    model.Po = getUserListDisplay(taskUserList);
                }
                else
                {
                    model.Po = "";
                }                
                taskUserList = workFlowDAOService.getWFTaskUser(SignboardConstant.WF_TYPE_VALIDATION, model.B_SV_RECORD.UUID, SignboardConstant.WF_MAP_VALIDATION_SPO);
                if (taskUserList != null && taskUserList.Count() > 0)
                {
                    model.Spo = getUserListDisplay(taskUserList);
                }
                else
                {
                    model.Spo = "";
                }
                
                string svValidationId = db.B_SV_VALIDATION.Where(x => x.SV_RECORD_ID == model.B_SV_RECORD.UUID).FirstOrDefault().UUID;

                List<B_SV_RECORD_ITEM> svri = db.B_SV_RECORD_ITEM.Where(x => x.SV_RECORD_ID == model.B_SV_RECORD.UUID).ToList();
                if(svri != null && svri.Count() > 0)
                {
                    model.B_SV_RECORD_ITEM = svri;
                }
                else
                {
                    model.B_SV_RECORD_ITEM = null;
                }
            }

            model.Status = getStatusByReceivedDateAndSCNo(model.B_SV_RECORD.RECEIVED_DATE, model.B_SV_RECORD.REFERENCE_NO);

            return model;
        }

        String SearchSBInfo_q = ""
             + "\r\n" + "\t" + " Select "
             + "\r\n" + "\t" + " svr.REFERENCE_NO as REFERENCE_NO, svri.MW_ITEM_CODE as ITEM_CODE, svri.LOCATION_DESCRIPTION as DESCRIPTION, svri.REVISION AS REVISION"
             + "\r\n" + "\t" + " from B_SV_RECORD_ITEM svri "
             + "\r\n" + "\t" + " inner join B_SV_RECORD svr on svr.UUID = svri.SV_RECORD_ID "
             + "\r\n" + "\t" + " where 1=1 ";


        public Fn02TDL_VSDisplayModel SearchSB_Info(Fn02TDL_VSDisplayModel model)
        {
            model.Query = SearchSBInfo_q;
            model.QueryWhere = SearchSBInfo_whereQ(model);
            model.Search();
            return model;
        }

        private string SearchSBInfo_whereQ(Fn02TDL_VSDisplayModel model)
        {
            string whereQ = "";

            whereQ += "\r\n\t" + "AND UPPER(svr.UUID) LIKE :UUID";
            model.QueryParameters.Add("UUID", "%" + model.B_SV_RECORD.UUID.Trim().ToUpper() + "%");

            return whereQ;
        }

        public List<List<object>> SearchValidationTask(string refNo)
        {
            Dictionary<string, object> queryParameters = new Dictionary<string, object>();
            List<List<object>> data = new List<List<object>>();

            string queryString = ""
                + " \r\n select distinct svv.uuid, svr.reference_no, svv.signboard_expiry_date, svs.location_of_signboard,  svr.uuid as sv_record_uuid,"
                + " \r\n svr.form_code, svr.received_date, svv.validation_result, svr.created_date, svr.signboard_removal_discov_date"
                + " \r\n from b_sv_record svr, b_sv_validation svv,  b_sv_address sva, b_sv_signboard svs, b_sv_appointed_professional svap"
                + " \r\n where svv.SV_RECORD_ID = svr.uuid and svs.uuid = svr.sv_signboard_id and sva.uuid = svs.LOCATION_ADDRESS_ID"
                + " \r\n and svap.sv_record_id = svr.uuid"
                + " \r\n and lower(svr.reference_no) like lower(:searchFileRefNo)"
                + " \r\n ORDER BY svr.reference_no";

            queryParameters.Add("searchFileRefNo", refNo);


            using (EntitiesSignboard db = new EntitiesSignboard())
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

        public string getUserListDisplay(List<List<object>> taskUserList)
        {
            string comma = ", ";
            string result = "";
            for(int i = 0; i < taskUserList.Count; i++)
            {
                result += (string)taskUserList[i][1];
                if(i < taskUserList.Count- 1 )
                {
                    result += comma;
                }
            }
            return result;
        }

        public List<List<SYS_POST>> loadValidationHandlingOfficers(string svValidationId)
        {
            Console.Write("loadValidationHandlingOfficers controller...");
            string svRecordId = "";
            List<List<SYS_POST>> resultList = new List<List<SYS_POST>>();
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                svRecordId = db.B_SV_VALIDATION.Find(svValidationId).SV_RECORD_ID;
            }
            WorkFlowDAOService workFlowDAOService = new WorkFlowDAOService();
            List<List<object>> userList = new List<List<object>>();

            userList = workFlowDAOService.getWFTaskUser(SignboardConstant.WF_TYPE_VALIDATION, svRecordId, SignboardConstant.WF_MAP_VALIDATION_TO);
            List<SYS_POST> tos = new List<SYS_POST>();
            for (int i = 0; i < userList.Count; i++)
            {
                SYS_POST suser = new SYS_POST();
                using (EntitiesAuth auth = new EntitiesAuth())
                {
                    suser = auth.SYS_POST.Find(userList[i][0]);
                    
                    tos.Add(CopyToNewSysPostObject(suser));
                }
            }
            resultList.Add(tos);

            userList = workFlowDAOService.getWFTaskUser(SignboardConstant.WF_TYPE_VALIDATION, svRecordId, SignboardConstant.WF_MAP_VALIDATION_PO);
            List<SYS_POST> pos = new List<SYS_POST>();
            for (int i = 0; i < userList.Count; i++)
            {
                SYS_POST suser = new SYS_POST();
                using (EntitiesAuth auth = new EntitiesAuth())
                {
                    suser = auth.SYS_POST.Find(userList[i][0]);
                }
                pos.Add(CopyToNewSysPostObject(suser));
            }
            resultList.Add(pos);

            userList = workFlowDAOService.getWFTaskUser(SignboardConstant.WF_TYPE_VALIDATION, svRecordId, SignboardConstant.WF_MAP_VALIDATION_SPO);
            List<SYS_POST> spos = new List<SYS_POST>();
            for (int i = 0; i < userList.Count; i++)
            {
                SYS_POST suser = new SYS_POST();
                using (EntitiesAuth auth = new EntitiesAuth())
                {
                    suser = auth.SYS_POST.Find(userList[i][0]);
                }
                spos.Add(CopyToNewSysPostObject(suser));
            }
            resultList.Add(spos);

            return resultList;
        }

        public SYS_POST CopyToNewSysPostObject(SYS_POST suser)
        {
            SYS_POST suser_new = new SYS_POST();
            suser_new.UUID = suser.UUID;
            suser_new.SYS_RANK_ID = suser.SYS_RANK_ID;
            suser_new.SYS_UNIT_ID = suser.SYS_UNIT_ID;
            suser_new.SUPERVISOR_ID = suser.SUPERVISOR_ID;
            suser_new.CODE = suser.CODE;
            suser_new.PHONE = suser.PHONE;
            suser_new.FAX_NO = suser.FAX_NO;
            suser_new.IS_ACTIVE = suser.IS_ACTIVE;
            suser_new.CREATED_DT = suser.CREATED_DT;
            suser_new.CREATED_POST = suser.CREATED_POST;
            suser_new.CREATED_NAME = suser.CREATED_NAME;
            suser_new.CREATED_SECTION = suser.CREATED_SECTION;
            suser_new.LAST_MODIFIED_DT = suser.LAST_MODIFIED_DT;
            suser_new.LAST_MODIFIED_POST = suser.LAST_MODIFIED_POST;
            suser_new.LAST_MODIFIED_NAME = suser.LAST_MODIFIED_NAME;
            suser_new.LAST_MODIFIED_SECTION = suser.LAST_MODIFIED_SECTION;
            suser_new.BD_PORTAL_LOGIN = suser.BD_PORTAL_LOGIN;
            suser_new.PW = suser.PW;
            suser_new.EMAIL = suser.EMAIL;
            suser_new.DSMS_USERNAME = suser.DSMS_USERNAME;
            suser_new.DSMS_PW = suser.DSMS_PW;
            suser_new.RECEIVE_CASE = suser.RECEIVE_CASE;

            return suser_new;
        }

        public string checkThumbnailFilePath(string path)
        {
            if (File.Exists(path))
            {
                return path.Replace("pdf", "jpg");
            }
            else
            {
                return "No Thumbnail";
            }
        }

        public string getStatusByReceivedDateAndSCNo(DateTime? receivedDate, string scNo)
        {
            if (receivedDate == null) return null;
            string result = "";
            TimeSpan ts = DateTime.Now - receivedDate.Value;
            double dateDiff = ts.TotalDays / 365.25;
            bool isExist = false;
            using(EntitiesSignboard db = new EntitiesSignboard())
            {
                List<string> previousScNo = db.B_SV_RECORD.Select(x => x.PREVIOUS_SUBMISSION_NUMBER).Distinct().ToList();
                if(previousScNo.Contains(scNo))
                {
                    isExist = true;
                }
                if(dateDiff < 5)
                {
                    result = SignboardConstant.TDL_SEARCH_STATUS_VALIDATE;
                }
                else if(dateDiff > 5 && !isExist)
                {
                    result = SignboardConstant.TDL_SEARCH_STATUS_EXPIRED_BUT_NOT_REVALIDATED;
                }
                else if(dateDiff > 5 && isExist)
                {
                    result = SignboardConstant.TDL_SEARCH_STATUS_EXPIRED_BUT_REVALIDATED;
                }
                else
                {
                    result = SignboardConstant.TDL_SEARCH_STATUS_REVALIDATED;
                }

                return result;
            }
        }
    }
}
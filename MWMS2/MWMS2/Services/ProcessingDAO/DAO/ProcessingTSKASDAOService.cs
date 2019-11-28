using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Constant;
using Oracle.ManagedDataAccess.Client;

namespace MWMS2.Services.ProcessingDAO.DAO
{
    public class ProcessingTSKASDAOService
    {


        public Fn03TSK_ASModel GetMwRecordByAddress(Fn03TSK_ASModel model)
        {
            List<OracleParameter> oracleParameters = new List<OracleParameter>(); ;
            string search_q = @"SELECT res.reference_no,
                                       P_get_item_code_by_record_id2(r.uuid) ITEMCODE,
                                       r.COMMENCEMENT_DATE,
                                       r.COMPLETION_DATE,
                                       r.STATUS_CODE,
                                       mwForm.received_date
                                FROM   P_MW_RECORD r
                                       INNER JOIN (SELECT DISTINCT refNo.reference_No,
                                                                   record.uuid uuid
                                                   FROM   P_Mw_Record record
                                                          INNER JOIN P_mw_Reference_No refNo
                                                                  ON record.reference_number = refNo.uuid
                                                          INNER JOIN P_Mw_Record_Address_Info addressInfo
                                                                  ON record.uuid = addressInfo.mw_record_id
                                                          INNER JOIN P_Mw_Address address
                                                                  ON addressInfo.mw_address_id = address.uuid "
                                                                  + CreateSqlForGetMwRecordByAddress(model, oracleParameters)
                                                                  + @" ) res
                                                ON res.uuid = r.uuid 
                                       INNER JOIN P_MW_FORM mwForm
                                               ON r.uuid = mwForm.mw_record_id";
            model.Query = search_q;
            model.QueryParameters = oracleParameters.ToDictionary(m => m.ParameterName, m => m.Value);
            model.Sort = " res.reference_No ";
            model.Search();
            return model;
        }

        public string ExcelRecord(Fn03TSK_ASModel model)
        {
            List<OracleParameter> oracleParameters = new List<OracleParameter>(); ;
            string search_q = @"SELECT res.reference_no,
                                       P_get_item_code_by_record_id2(r.uuid) ITEMCODE,
                                       r.COMMENCEMENT_DATE,
                                       r.COMPLETION_DATE,
                                       r.STATUS_CODE AS STATUS,
                                       mwForm.received_date
                                FROM   P_MW_RECORD r
                                       INNER JOIN (SELECT DISTINCT refNo.reference_No,
                                                                   record.uuid uuid
                                                   FROM   P_Mw_Record record
                                                          INNER JOIN P_mw_Reference_No refNo
                                                                  ON record.reference_number = refNo.uuid
                                                          INNER JOIN P_Mw_Record_Address_Info addressInfo
                                                                  ON record.uuid = addressInfo.mw_record_id
                                                          INNER JOIN P_Mw_Address address
                                                                  ON addressInfo.mw_address_id = address.uuid "
                                                                  + CreateSqlForGetMwRecordByAddress(model, oracleParameters)
                                                                  + @" ) res
                                                ON res.uuid = r.uuid 
                                       INNER JOIN P_MW_FORM mwForm
                                               ON r.uuid = mwForm.mw_record_id";
            model.Query = search_q;
            model.QueryParameters = oracleParameters.ToDictionary(m => m.ParameterName, m => m.Value);
            model.Sort = " res.reference_No ";
            return model.Export("Address Search " + DateTime.Now.ToShortDateString() + DateTime.Now.ToShortTimeString());
        }

        public string CreateSqlForGetMwRecordByAddress(Fn03TSK_ASModel model, List<OracleParameter> oracleParameters)
        {
            StringBuilder where_q = new StringBuilder();
            ProcessingConstant constant = new ProcessingConstant();
            //--AND P_security_user_can_view(record.uuid, :userGroupType, :isSpo, :userId) > 0
            where_q.Append(string.Format(@" WHERE  record.is_Data_Entry = :isDataEntry
                                   AND record.status_Code IN ( '{0}' ) ", string.Join("','", constant.getFutureMwRecordStatusListByStatus(ProcessingConstant.MW_VERIFCAITON_COMPLETED, null).ToArray())));
            oracleParameters.Add(new OracleParameter(":isDataEntry", ProcessingConstant.FLAG_N));


            if (!string.IsNullOrWhiteSpace(model.StreetRoadVillage))
            {
                where_q.Append(@" AND ( ( address.display_Street LIKE :street )
                                          OR ( ( address.english_Street_Name
                                                 || ' '
                                                 || address.english_Street_Direction
                                                 || ' '
                                                 || address.english_Street_Type
                                                 || ' '
                                                 || address.english_St_Location_Name_1 ) LIKE :street )
                                          OR ( ( address.english_Street_Name
                                                 || ' '
                                                 || address.english_Street_Type
                                                 || ' '
                                                 || address.english_Street_Direction
                                                 || ' '
                                                 || address.english_St_Location_Name_1 ) LIKE :street )
                                          OR ( ( address.chinese_Street_Name
                                                 || address.chinese_Street_Type
                                                 || address.chinese_Street_Direction
                                                 || ' '
                                                 || address.CHINESE_ST_LOCATION_NAME_1 ) LIKE :street )
                                          OR ( ( address.chinese_Street_Name
                                                 || address.chinese_Street_Direction
                                                 || address.chinese_Street_Type
                                                 || ' '
                                                 || address.CHINESE_ST_LOCATION_NAME_1 ) LIKE :street ) ) ");
                oracleParameters.Add(new OracleParameter(":street", model.StreetRoadVillage + "%"));
            }

            if (!string.IsNullOrWhiteSpace(model.StreetLotNumber))
            {
                where_q.Append(@" AND address.display_Street_No LIKE :streetNo ");
                oracleParameters.Add(new OracleParameter(":streetNo", model.StreetLotNumber + "%"));
            }

            if (!string.IsNullOrWhiteSpace(model.BuildingEstate))
            {
                where_q.Append(@" AND ( ( address.display_Buildingname LIKE :buildingName )
                                          OR ( address.english_Building_Name_Line_1 LIKE :buildingName )
                                          OR ( address.chinese_Building_Name_Line_1 LIKE :buildingName ) ) ");
                oracleParameters.Add(new OracleParameter(":buildingName", model.BuildingEstate + "%"));
            }

            if (!string.IsNullOrWhiteSpace(model.Floor))
            {
                where_q.Append(@" AND ( ( address.display_Floor LIKE :floor )
                                          OR ( address.english_Floor_Description LIKE :floor )
                                          OR ( address.chinese_Floor_Description LIKE :floor ) ) ");
                oracleParameters.Add(new OracleParameter(":floor", model.Floor + "%"));
            }

            if (!string.IsNullOrWhiteSpace(model.FlatRoom))
            {
                where_q.Append(@" AND ( ( address.display_Flat LIKE :flat )
                                          OR ( address.english_Unit_No LIKE :flat )
                                          OR ( address.english_Unit_No LIKE :flat ) ) ");
                oracleParameters.Add(new OracleParameter(":flat", model.FlatRoom + "%"));
            }

            if (!string.IsNullOrWhiteSpace(model.District))
            {
                where_q.Append(@" AND address.display_District LIKE :district ");
                oracleParameters.Add(new OracleParameter(":district", model.District + "%"));
            }

            if (!string.IsNullOrWhiteSpace(model.Region))
            {
                where_q.Append(@" AND address.display_Region LIKE :region ");
                oracleParameters.Add(new OracleParameter(":region", model.Region + "%"));
            }
            return where_q.ToString();
        }

        public Fn03TSK_ASModel GetMWGeneralRecordByAddress(Fn03TSK_ASModel model)
        {
            List<OracleParameter> oracleParameters = new List<OracleParameter>(); ;
            string search_q = @"SELECT record.*,refNo.reference_no
                                FROM   P_Mw_General_Record record
                                       INNER JOIN P_mw_Reference_No refNo
                                               ON record.reference_number = refNo.uuid
                                       INNER JOIN P_Mw_General_Record_Address_Info addressInfo
                                               ON addressInfo.mw_general_record_id = record.uuid
                                       INNER JOIN P_MW_Address address
                                               ON addressInfo.mw_address_id = address.uuid
                                WHERE  1 = 1 
                                ";

            model.Query = search_q;
            model.QueryWhere = CreateSqlForGetMwGeneralRecordByAddress(model, oracleParameters);
            model.QueryParameters = oracleParameters.ToDictionary(m => m.ParameterName, m => m.Value);
            model.Sort = " refNo.reference_No ";
            model.Search();
            return model;
        }

        public string ExcelGR(Fn03TSK_ASModel model)
        {
            List<OracleParameter> oracleParameters = new List<OracleParameter>(); ;
            string search_q = @"SELECT record.*,refNo.reference_no,record.ICC_NUMBER AS DSN_ICCNO
                                FROM   P_Mw_General_Record record
                                       INNER JOIN P_mw_Reference_No refNo
                                               ON record.reference_number = refNo.uuid
                                       INNER JOIN P_Mw_General_Record_Address_Info addressInfo
                                               ON addressInfo.mw_general_record_id = record.uuid
                                       INNER JOIN P_MW_Address address
                                               ON addressInfo.mw_address_id = address.uuid
                                WHERE  1 = 1 
                                ";

            model.Query = search_q;
            model.QueryWhere = CreateSqlForGetMwGeneralRecordByAddress(model, oracleParameters);
            model.QueryParameters = oracleParameters.ToDictionary(m => m.ParameterName, m => m.Value);
            model.Sort = " refNo.reference_No ";
            return model.Export("Address GR " + DateTime.Now.ToShortDateString() + DateTime.Now.ToShortTimeString());
        }

        public string CreateSqlForGetMwGeneralRecordByAddress(Fn03TSK_ASModel model, List<OracleParameter> oracleParameters)
        {
            StringBuilder where_q = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(model.StreetRoadVillage))
            {
                where_q.Append(@" AND ( ( address.display_Street LIKE :street )
                                          OR ( ( address.english_Street_Name
                                                 || ' '
                                                 || address.english_Street_Direction
                                                 || ' '
                                                 || address.english_Street_Type
                                                 || ' '
                                                 || address.english_St_Location_Name_1 ) LIKE :street )
                                          OR ( ( address.english_Street_Name
                                                 || ' '
                                                 || address.english_Street_Type
                                                 || ' '
                                                 || address.english_Street_Direction
                                                 || ' '
                                                 || address.english_St_Location_Name_1 ) LIKE :street )
                                          OR ( ( address.chinese_Street_Name
                                                 || address.chinese_Street_Type
                                                 || address.chinese_Street_Direction
                                                 || ' '
                                                 || address.chinese_St_Location_Name_1 ) LIKE :street )
                                          OR ( ( address.chinese_Street_Name
                                                 || address.chinese_Street_Direction
                                                 || address.chinese_Street_Type
                                                 || ' '
                                                 || address.chinese_St_Location_Name_1 ) LIKE :street ) ) ");
                oracleParameters.Add(new OracleParameter(":street", model.StreetRoadVillage + "%"));
            }

            if (!string.IsNullOrWhiteSpace(model.StreetLotNumber))
            {
                where_q.Append(@" AND address.display_Street_No LIKE :streetNo ");
                oracleParameters.Add(new OracleParameter(":streetNo", model.StreetLotNumber + "%"));
            }

            if (!string.IsNullOrWhiteSpace(model.BuildingEstate))
            {
                where_q.Append(@" AND ( ( address.display_Buildingname LIKE :buildingName )
                                          OR ( address.english_Building_Name_Line_1 LIKE :buildingName )
                                          OR ( address.chinese_Building_Name_Line_1 LIKE :buildingName ) ) ");
                oracleParameters.Add(new OracleParameter(":buildingName", model.BuildingEstate + "%"));
            }

            if (!string.IsNullOrWhiteSpace(model.Floor))
            {
                where_q.Append(@" AND ( ( address.display_Floor LIKE :floor )
                                          OR ( address.english_Floor_Description LIKE :floor )
                                          OR ( address.chinese_Floor_Description LIKE :floor ) ) ");
                oracleParameters.Add(new OracleParameter(":floor", model.Floor + "%"));
            }

            if (!string.IsNullOrWhiteSpace(model.FlatRoom))
            {
                where_q.Append(@" AND ( ( address.display_Flat LIKE :flat )
                                          OR ( address.english_Unit_No LIKE :flat )
                                          OR ( address.english_Unit_No LIKE :flat ) ) ");
                oracleParameters.Add(new OracleParameter(":flat", model.FlatRoom + "%"));
            }

            if (!string.IsNullOrWhiteSpace(model.District))
            {
                where_q.Append(@" AND address.display_District LIKE :district ");
                oracleParameters.Add(new OracleParameter(":district", model.District + "%"));
            }

            if (!string.IsNullOrWhiteSpace(model.Region))
            {
                where_q.Append(@" AND address.display_Region LIKE :region ");
                oracleParameters.Add(new OracleParameter(":region", model.Region + "%"));
            }

            return where_q.ToString();
        }
    }
}
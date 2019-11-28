using MWMS2.Areas.Registration.Models;
using MWMS2.Entity;
using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using MWMS2.Utility;
using MWMS2.Constant;
using System.Text;
using System.IO;
using OfficeOpenXml;
using System.IO.Compression;
using MWMS2.Areas.Signboard.Models;

namespace MWMS2.Services
{
    public class SignboardDataExportService : RegistrationCommonService
    {
        private static log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        private string QP_SQL =
            " select svr.reference_no, svr.form_code, svr.received_date, svv.signboard_expiry_date, "
					+ "decode(svv.validation_result, " + 
					    "'A', :validation_result_accept, " + 
					    "'R', :validation_result_refuse, " + 
					    "'C', :validation_result_conditional_accept, " + 
					    "svv.validation_result) as validation_result,'', " 
					+ "svv.spo_endorsement_date, "
					+ "svv.letter_reason, svv.letter_remarks, " 
					+ "sba.full_address, "
					+ "owner.name_chinese as ownerCnaame, owner.name_english as ownerEname, ownera.full_address as owneraddress, owner.email as owneremail, owner.contact_no as ownercontact, owner.fax_no as ownerfaxno, "
					+ "paw.name_chinese as pawCnaame, paw.name_english as pawEname, pawa.full_address as pawaddress, paw.email as pawemail,paw.contact_no as pawcontact, paw.fax_no as pawfaxno, "
					+ "io.name_chinese as ioCnaame, io.name_english as ioEname, ioa.full_address as ioaddress, io.email as ioemail, io.contact_no as iocontact, io.fax_no as iofaxno, io.pbp_name, io.pbp_contact_no, io.prc_name, io.prc_contact_no, "
					+ "ap.certification_no as apcert, ap.chinese_name as apCname, ap.english_name as apEname, ap.contact_no as apcontact, ap.fax_no as apfax, "
					+ "rse.certification_no as rsecert , rse.chinese_name as rseCname, rse.english_name as rseEname, rse.contact_no as rsecontact, rse.fax_no as rsefax, "
					+ "rge.certification_no as rgecert, rge.chinese_name as rgeCname, rge.english_name as rgeEname, rge.contact_no as rgecontact, rge.fax_no as rgefax, "
					+ "prc.certification_no as prccert, prc.chinese_name as prcCname, prc.english_name as prcEname, prc.contact_no as prccontact, prc.fax_no as prcfax, prc.as_chinese_name, prc.as_english_name, svv.remarks, "
					+ "svs.location_of_signboard, svs.description, svs.facade, svs.type, svs.btm_floor, svs.top_floor, svs.a_m2, svs.p_m, svs.h_m, svs.h2_m, svs.led, svs.building_portion, svs.rvd_no, sba.rv_block_id, sba.bcis_block_id, sba.bcis_district, sba.file_reference_no "
					+ "from b_sv_record svr, b_sv_validation svv, b_sv_address sba, b_sv_signboard svs, "
                    + "b_sv_person_contact owner, b_sv_address ownera, "
                    + "b_sv_person_contact paw, b_sv_address pawa, "
                    + "b_sv_person_contact io, b_sv_address ioa, "
                    + "b_sv_appointed_professional ap, b_sv_appointed_professional rse, "
                    + "b_sv_appointed_professional rge, b_sv_appointed_professional prc "
                    + "where svv.uuid in ( :uuid ) and "
					+ "svr.uuid = svv.sv_record_id and "
					+ "svs.uuid = svr.sv_signboard_id and "
					+ "sba.uuid = svs.location_address_id and "
					+ "owner.uuid = svs.owner_id and ownera.uuid = owner.sv_address_id and "
					+ "paw.uuid = svr.paw_id and pawa.uuid = paw.sv_address_id and "
					+ "io.uuid = svr.oi_id and ioa.uuid = io.sv_address_id and "
					+ "ap.sv_record_id = svr.uuid  and ap.identify_flag=:ap and "
					+ "rse.sv_record_id = svr.uuid  and rse.identify_flag=:rse and "
					+ "rge.sv_record_id = svr.uuid  and rge.identify_flag=:rge and "
					+ "prc.sv_record_id = svr.uuid  and prc.identify_flag=:prc "
			        + " order by svr.reference_no ";

        public FileStreamResult exportQPExcelData(string uuid)
        {
            List<string> headerList = new List<string>() {

                "Submission No.", "Form Code", "Received Date", "Expirty Date",
                "Validation Result", "Remarks", "SPO Emdorsement Date", "Letter Reason",
                "Letter Remarks", "Signboard Address", "Location of Signboard", "Description", "Facade",
                "Type", "Bottom fixing at Floor", "Top fixing at Floor", "Display Area", "Projection",
                "Heiogh of Signboard", "Clearance above ground", "LED/TV", "Building Portion", "RVD No.",
                "RV Block ID", "BCIS Block ID", "BCIS District", "BD ref.(42)", "Owner Chinese Name", "Owner English Name",
                "Owner Address", "Owner Email", "Owner Contact No.", "Owner Fax NO", "PAW Chinese Name", "PAW English Name",
                "PAW Address", "PAW Email", "PAW Contact No.", "PAW Fax NO", "IO Chinese Name", "IO English Name",
                "IO Address", "IO Email", "IO Contact No", "IO PBP Name",  "IO PBP Contact No",   "IO PRC Name", "IO PRC Contact No",
                "AP Certification No", "AP Chinese Name", "AP English Name", "AP EN_ADDRESS_LINE1", "AP EN_ADDRESS_LINE2", "AP EN_ADDRESS_LINE3",
                "AP EN_ADDRESS_LINE4", "AP EN_ADDRESS_LINE5", "AP CN_ADDRESS_LINE1", "AP CN_ADDRESS_LINE2", "AP CN_ADDRESS_LINE3", "AP CN_ADDRESS_LINE4",
                "AP CN_ADDRESS_LINE5", "AP Contact No",   "AP Fax No",   "RSE Certification No",    "RSE Chinese Name",    "RSE English Name",    "RSE EN_ADDRESS_LINE1",
                "RSE EN_ADDRESS_LINE2",    "RSE EN_ADDRESS_LINE3",    "RSE EN_ADDRESS_LINE4",    "RSE EN_ADDRESS_LINE5",    "RSE CN_ADDRESS_LINE1",
                "RSE CN_ADDRESS_LINE2",    "RSE CN_ADDRESS_LINE3",    "RSE CN_ADDRESS_LINE4",    "RSE CN_ADDRESS_LINE5",    "RSE Contact No",
                "RSE Fax No",  "RGE Certification No",    "RGE Chinese Name",    "RGE English Name",    "RGE EN_ADDRESS_LINE1",    "RGE EN_ADDRESS_LINE2",
                "RGE EN_ADDRESS_LINE3",    "RGE EN_ADDRESS_LINE4",    "RGE EN_ADDRESS_LINE5",    "RGE CN_ADDRESS_LINE1",    "RGE CN_ADDRESS_LINE2",    "RGE CN_ADDRESS_LINE3",    "RGE CN_ADDRESS_LINE4",
                "RGE CN_ADDRESS_LINE5",    "RGE Contact No",  "RGE Fax No",  "PRC Certification No",    "PRC Chinese Name",    "PRC English Name",
                "PRC EN_ADDRESS_LINE1",    "PRC EN_ADDRESS_LINE2",    "PRC EN_ADDRESS_LINE3",    "PRC EN_ADDRESS_LINE4",    "PRC EN_ADDRESS_LINE5",
                "PRC CN_ADDRESS_LINE1",    "PRC CN_ADDRESS_LINE2",    "PRC CN_ADDRESS_LINE3",    "PRC CN_ADDRESS_LINE4",    "PRC CN_ADDRESS_LINE5",
                "PRC Contact No",  "PRC Fax No",  "PRC AS Chinese Name", "PRC AS English Name" };

            List<List<object>> data = null;
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                using (DbConnection conn = db.Database.Connection)
                {
                    String sql = this.QP_SQL;
                    Fn01SCUR_MMSearchModel model = new Fn01SCUR_MMSearchModel();
                    model.QueryParameters.Add("validation_result_accept", ApplicationConstant.RECOMMEND_ACK_STR);
                    model.QueryParameters.Add("validation_result_refuse", ApplicationConstant.RECOMMEND_REF_STR);
                    model.QueryParameters.Add("validation_result_conditional_accept", ApplicationConstant.RECOMMEND_COND_STR);
                    var a = uuid.Split(',');
                    List<String> dataList = new List<String>();
                    for (int i = 0; i < a.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(a[i]))
                        {
                            dataList.Add(a[i]);
                        }
                        
                    }
                    if (dataList != null && dataList.Count > 0)
                    {
                        string parStr = "";
                        for (int i = 0; i < dataList.Count; i++)
                        {
                            parStr += ", :uuid"+i;
                        }
                        parStr = parStr.Substring(2);
                        sql = sql.Replace(":uuid", parStr);
                    }

                    for (int i = 0; i < dataList.Count; i++)
                    {
                        model.QueryParameters.Add("uuid"+i, dataList[i]);
                    }
                    //model.QueryParameters.Add("uuid", uuid);
                    model.QueryParameters.Add("ap", ApplicationConstant.PBP_CODE_AP);
                    model.QueryParameters.Add("rse", ApplicationConstant.PBP_CODE_RSE);
                    model.QueryParameters.Add("rge", ApplicationConstant.PBP_CODE_RGE);
                    model.QueryParameters.Add("prc", ApplicationConstant.PRC_CODE);
                    DbDataReader dr = CommonUtil.GetDataReader(conn, sql, model.QueryParameters);
                    data = CommonUtil.convertToList(dr);
                    conn.Close();
                }
            }
            return this.exportExcelFile("Validation", headerList, data);

        }
        public FileStreamResult exportQPCSVData(string uuid)
        {
            List<string> headerList = new List<string>() {

                "Submission No.", "Form Code", "Received Date", "Expirty Date",
                "Validation Result", "Remarks", "SPO Emdorsement Date", "Letter Reason",
                "Letter Remarks", "Signboard Address", "Location of Signboard", "Description", "Facade",
                "Type", "Bottom fixing at Floor", "Top fixing at Floor", "Display Area", "Projection",
                "Heiogh of Signboard", "Clearance above ground", "LED/TV", "Building Portion", "RVD No.",
                "RV Block ID", "BCIS Block ID", "BCIS District", "BD ref.(42)", "Owner Chinese Name", "Owner English Name",
                "Owner Address", "Owner Email", "Owner Contact No.", "Owner Fax NO", "PAW Chinese Name", "PAW English Name",
                "PAW Address", "PAW Email", "PAW Contact No.", "PAW Fax NO", "IO Chinese Name", "IO English Name",
                "IO Address", "IO Email", "IO Contact No", "IO PBP Name",  "IO PBP Contact No",   "IO PRC Name", "IO PRC Contact No",
                "AP Certification No", "AP Chinese Name", "AP English Name", "AP EN_ADDRESS_LINE1", "AP EN_ADDRESS_LINE2", "AP EN_ADDRESS_LINE3",
                "AP EN_ADDRESS_LINE4", "AP EN_ADDRESS_LINE5", "AP CN_ADDRESS_LINE1", "AP CN_ADDRESS_LINE2", "AP CN_ADDRESS_LINE3", "AP CN_ADDRESS_LINE4",
                "AP CN_ADDRESS_LINE5", "AP Contact No",   "AP Fax No",   "RSE Certification No",    "RSE Chinese Name",    "RSE English Name",    "RSE EN_ADDRESS_LINE1",
                "RSE EN_ADDRESS_LINE2",    "RSE EN_ADDRESS_LINE3",    "RSE EN_ADDRESS_LINE4",    "RSE EN_ADDRESS_LINE5",    "RSE CN_ADDRESS_LINE1",
                "RSE CN_ADDRESS_LINE2",    "RSE CN_ADDRESS_LINE3",    "RSE CN_ADDRESS_LINE4",    "RSE CN_ADDRESS_LINE5",    "RSE Contact No",
                "RSE Fax No",  "RGE Certification No",    "RGE Chinese Name",    "RGE English Name",    "RGE EN_ADDRESS_LINE1",    "RGE EN_ADDRESS_LINE2",
                "RGE EN_ADDRESS_LINE3",    "RGE EN_ADDRESS_LINE4",    "RGE EN_ADDRESS_LINE5",    "RGE CN_ADDRESS_LINE1",    "RGE CN_ADDRESS_LINE2",    "RGE CN_ADDRESS_LINE3",    "RGE CN_ADDRESS_LINE4",
                "RGE CN_ADDRESS_LINE5",    "RGE Contact No",  "RGE Fax No",  "PRC Certification No",    "PRC Chinese Name",    "PRC English Name",
                "PRC EN_ADDRESS_LINE1",    "PRC EN_ADDRESS_LINE2",    "PRC EN_ADDRESS_LINE3",    "PRC EN_ADDRESS_LINE4",    "PRC EN_ADDRESS_LINE5",
                "PRC CN_ADDRESS_LINE1",    "PRC CN_ADDRESS_LINE2",    "PRC CN_ADDRESS_LINE3",    "PRC CN_ADDRESS_LINE4",    "PRC CN_ADDRESS_LINE5",
                "PRC Contact No",  "PRC Fax No",  "PRC AS Chinese Name", "PRC AS English Name" };

            List<List<object>> data = null;
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                using (DbConnection conn = db.Database.Connection)
                {
                    String sql = this.QP_SQL;
                    Fn01SCUR_MMSearchModel model = new Fn01SCUR_MMSearchModel();
                    model.QueryParameters.Add("validation_result_accept", ApplicationConstant.RECOMMEND_ACK_STR);
                    model.QueryParameters.Add("validation_result_refuse", ApplicationConstant.RECOMMEND_REF_STR);
                    model.QueryParameters.Add("validation_result_conditional_accept", ApplicationConstant.RECOMMEND_COND_STR);
                    var a = uuid.Split(',');
                    List<String> dataList = new List<String>();
                    for (int i = 0; i < a.Length; i++)
                    {
                        dataList.Add(a[i]);
                    }
                    if (dataList != null && dataList.Count > 0)
                    {
                        string parStr = "";
                        for (int i = 0; i < dataList.Count; i++)
                        {
                            parStr += ", :uuid" + i;
                        }
                        parStr = parStr.Substring(2);
                        sql = sql.Replace(":uuid", parStr);
                    }

                    for (int i = 0; i < dataList.Count; i++)
                    {
                        model.QueryParameters.Add("uuid" + i, dataList[i]);
                    }
                    //model.QueryParameters.Add("uuid", uuid);
                    model.QueryParameters.Add("ap", ApplicationConstant.PBP_CODE_AP);
                    model.QueryParameters.Add("rse", ApplicationConstant.PBP_CODE_RSE);
                    model.QueryParameters.Add("rge", ApplicationConstant.PBP_CODE_RGE);
                    model.QueryParameters.Add("prc", ApplicationConstant.PRC_CODE);
                    DbDataReader dr = CommonUtil.GetDataReader(conn, sql, model.QueryParameters);
                    data = CommonUtil.convertToList(dr);
                    conn.Close();
                }
            }
            return this.exportCSVFile("ValidationDataExport", headerList, data);

        }
    }
}

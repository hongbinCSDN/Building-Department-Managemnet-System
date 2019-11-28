using MWMS2.Areas.Registration.Models;
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
using MWMS2.Constant;
using System.Text;
using System.IO;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Globalization;
using OfficeOpenXml;

namespace MWMS2.Services
{
    public class RegistrationASService : BaseCommonService
    {
        public Fn06AS_DBDisplayModel ViewASdb(string appUuid, string STATUS)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                string HighestClass;
                string Email1;
                string Email2;
                string ID;
                string FilePath = "";
                string ConsentStatus = "";

                C_APPLICANT applicant = (from app in db.C_APPLICANT
                                         where app.UUID == appUuid
                                         select app).FirstOrDefault();

                C_COMP_APPLICANT_INFO compAppInfo = (from CompApp in db.C_COMP_APPLICANT_INFO
                                                     where CompApp.APPLICANT_ID == appUuid
                                                     orderby CompApp.CREATED_DATE
                                                     select CompApp).FirstOrDefault();

                C_COMP_APPLICATION CompApplication = db.C_COMP_APPLICATION.Where(o => o.UUID == compAppInfo.MASTER_ID).FirstOrDefault();

                C_AS_CONSENT asConsent = null;

                if (!string.IsNullOrWhiteSpace(applicant.HKID))
                {
                    asConsent = db.C_AS_CONSENT.Where(o => o.HKID == applicant.HKID).FirstOrDefault();
                }
                else
                {
                    asConsent = db.C_AS_CONSENT.Where(o => o.PASSPORT_NO == applicant.PASSPORT_NO).FirstOrDefault();
                }
                C_ADDRESS chinessAddess = null;
                C_ADDRESS englishAddress = null;
                if (CompApplication.CHINESE_ADDRESS_ID != null)
                {
                    chinessAddess = db.C_ADDRESS.Where(o => o.UUID == CompApplication.CHINESE_ADDRESS_ID).FirstOrDefault();
                }
                else if (CompApplication.CHINESE_ADDRESS_ID != null)
                {
                    chinessAddess = db.C_ADDRESS.Where(o => o.UUID == CompApplication.CHINESE_ADDRESS_ID).FirstOrDefault();
                }

                if (CompApplication.ENGLISH_ADDRESS_ID != null)
                {
                    englishAddress = db.C_ADDRESS.Where(o => o.UUID == CompApplication.ENGLISH_ADDRESS_ID).FirstOrDefault();
                }
                else if (CompApplication.ENGLISH_ADDRESS_ID != null)
                {
                    englishAddress = db.C_ADDRESS.Where(o => o.UUID == CompApplication.ENGLISH_ADDRESS_ID).FirstOrDefault();
                }

                Email1 = compAppInfo.EMAIL1;
                Email2 = compAppInfo.EMAIL2;




                if (!string.IsNullOrWhiteSpace(applicant.HKID))
                {
                    ID = EncryptDecryptUtil.getDecryptHKID(applicant.HKID);
                }
                else
                {
                    ID = EncryptDecryptUtil.getDecryptHKID(applicant.PASSPORT_NO);
                }

                if (asConsent == null)
                {
                    asConsent = new C_AS_CONSENT();
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(asConsent.FILE_PATH_NONRESTRICTED))
                    {
                        FilePath = asConsent.FILE_PATH_NONRESTRICTED;
                    }
                    else
                    {
                        FilePath = "";
                    }
                }
                if (asConsent.CLASS_OF_MINOR_WORKS == null)
                {
                    HighestClass = findHighestClassOfMinorWorks(applicant.UUID);
                }
                else
                {
                    HighestClass = asConsent.CLASS_OF_MINOR_WORKS;
                }
                if (asConsent.CONSENT == null)
                {
                    ConsentStatus = RegistrationConstant.NOT_INDICATED;
                }
                else
                {
                    ConsentStatus = asConsent.CONSENT;
                }
                Fn06AS_DBDisplayModel model = new Fn06AS_DBDisplayModel()
                {
                    C_APPLICANT = applicant,
                    EngFullName = applicant.SURNAME + " " + applicant.GIVEN_NAME_ON_ID,
                    ChinessAddess = chinessAddess,
                    EnglishAddress = englishAddress,
                    C_AS_CONSENT = asConsent,
                    C_COMP_APPLICANT_INFO = compAppInfo,
                    Email1 = Email1 != null ? Email1 : "",
                    Email2 = Email2 != null ? Email2 : "",
                    ID = ID != null ? ID : "",
                    FilePath = FilePath,
                    STATUS = STATUS,
                    ClassOfMW = HighestClass,
                    ConsentStatus = ConsentStatus
                };
                return model;
            }
        }
        public string findHighestClassOfMinorWorks(string appUuid)
        {
            string result = "";
            string querystr = "select C_GET_MWC_HIGHEST_CLASS_BY_APP('" + appUuid + "') as HighestClass from dual";
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                using (DbConnection conn = db.Database.Connection)
                {
                    DbDataReader dr = CommonUtil.GetDataReader(conn, querystr, null);
                    List<Dictionary<string, object>> Data = CommonUtil.LoadDbData(dr);
                    if (Data.Count > 0)
                    {
                        result = Data[0]["HIGHESTCLASS"].ToString();
                    }
                    conn.Close();
                }
            }
            return result;
        }
        public void GetASdata(Fn06AS_DBDisplayModel model)
        {
            string queryStr = "" +
                        "\r\n" + "select distinct " +
                        "\r\n" + " app.uuid as uuid, " +
                        "\r\n" + " comp_app.FILE_REFERENCE_NO as FILEREF, " +
                        "\r\n" + " comp_app.ENGLISH_COMPANY_NAME as ENG_COMP_NAME, " +
                        "\r\n" + " comp_app.CHINESE_COMPANY_NAME as CHI_COMP_NAME, " +
                        "\r\n" + EncryptDecryptUtil.getDecryptSQL("app.hkid") + " as hkid, " +
                        "\r\n" + EncryptDecryptUtil.getDecryptSQL("app.passport_no") + " as passport_no, " +
                        "\r\n" + " app.SURNAME as surname, " +
                        "\r\n" + " app.GIVEN_NAME_ON_ID as given_name_on_id, " +
                        "\r\n" + " app.CHINESE_NAME as chinese_name," +
                        "\r\n" + " sv2.CODE as CODE, " +
                        "\r\n" + " sv.ENGLISH_DESCRIPTION as STATUS, " +
                        "\r\n" + " sv2.REGISTRATION_TYPE as REG_TYPE, " +
                        "\r\n" + " com_app_info.ACCEPT_DATE as ACCEP_DATE, " +
                        "\r\n" + " com_app_info.REMOVAL_DATE as RMDATE, " +
                        "\r\n" + " as1.CONSENT as asConsent, " +
                        "\r\n" + " com_app_info.FILE_PATH_NONRESTRICTED as SINGATURE, com_app_info.uuid as COM_APP_INFO_UUID " +
                        "\r\n" + " from c_applicant app " +
                        "\r\n" + " inner join c_comp_applicant_info com_app_info on com_app_info.APPLICANT_ID = app.uuid" +
                        "\r\n" + " inner join c_comp_application comp_app on comp_app.uuid = com_app_info.master_id " +
                        "\r\n" + " inner join c_s_system_value sv on sv.uuid = com_app_info.APPLICANT_STATUS_ID " +
                        "\r\n" + " inner join c_s_system_value sv2 on sv2.uuid =com_app_info.APPLICANT_ROLE_ID " +
                        "\r\n" + " inner join c_s_category_code s_cate_code on s_cate_code.uuid = comp_app.CATEGORY_ID " +
                        "\r\n" + " left outer join c_comp_Applicant_Mw_Item comp_app_Mw_Item on comp_app_Mw_Item.COMPANY_APPLICANTS_ID = com_app_info.uuid " +
                        "\r\n" + " left outer join c_s_system_value sv3 on sv3.uuid = comp_app_Mw_Item.ITEM_CLASS_ID " +
                        "\r\n" + " left outer join c_as_consent as1 on as1.hkid = app.hkid or as1.passport_no = app.passport_no " +
                        "\r\n" + " where sv2.CODE='AS' ";

            queryStr += "\r\n" + " and app.uuid=:uuid ";
            model.QueryParameters.Add("uuid", model.C_APPLICANT.UUID);

            model.Query = queryStr;
            model.Search();
        }
        public ServiceResult SaveAS_DB(Fn06AS_DBDisplayModel model, string path, IEnumerable<HttpPostedFileBase> file)
        {
            string fileExt0 = "";
            if (file.ElementAt(0) != null) { fileExt0 = "." + System.IO.Path.GetExtension(file.ElementAt(0).FileName).Substring(1); }
            //string fileExt = "." + System.IO.Path.GetExtension(file.FileName).Substring(1);
            string uuid = CommonUtil.NewUuid();
            try
            {
                using (EntitiesRegistration db = new EntitiesRegistration())
                {
                    C_APPLICANT applicant = (from app in db.C_APPLICANT where app.UUID == model.C_APPLICANT.UUID select app).FirstOrDefault();
                    if (model.C_AS_CONSENT.UUID != null)
                    {
                        C_AS_CONSENT ConsentForm = (from asc in db.C_AS_CONSENT where asc.UUID == model.C_AS_CONSENT.UUID select asc).FirstOrDefault();
                        ConsentForm.CLASS_OF_MINOR_WORKS = model.ClassOfMW;//model.C_AS_CONSENT.CLASS_OF_MINOR_WORKS;
                        ConsentForm.CONSENT = model.ConsentStatus;//model.C_AS_CONSENT.CONSENT;
                        ConsentForm.CONTACT_TEL_NO = model.C_AS_CONSENT.CONTACT_TEL_NO;
                        ConsentForm.DATE_OF_CONSENT = DateTime.Today;
                        if (file.ElementAt(0) != null)
                        {
                            ConsentForm.FILE_PATH_NONRESTRICTED = ConsentForm.UUID + "0" + fileExt0;
                        }
                        else
                        {
                            ConsentForm.FILE_PATH_NONRESTRICTED = null;
                        }
                        ConsentForm.MODIFIED_DATE = System.DateTime.Now;
                        ConsentForm.MODIFIED_BY = SystemParameterConstant.UserName;

                        // db.C_AS_CONSENT.Add(ConsentForm);
                    }
                    else
                    {
                        C_AS_CONSENT ConsentForm = new C_AS_CONSENT();
                        ConsentForm.UUID = Guid.NewGuid().ToString();
                        ConsentForm.HKID = applicant.HKID;
                        ConsentForm.PASSPORT_NO = applicant.PASSPORT_NO;
                        ConsentForm.SURNAME = applicant.SURNAME;
                        ConsentForm.GIVEN_NAME_ON_ID = applicant.GIVEN_NAME_ON_ID;
                        ConsentForm.CHINESE_NAME = applicant.CHINESE_NAME;
                        ConsentForm.CLASS_OF_MINOR_WORKS = model.ClassOfMW;//model.C_AS_CONSENT.CLASS_OF_MINOR_WORKS;
                        ConsentForm.CONSENT = model.ConsentStatus;//model.C_AS_CONSENT.CONSENT;
                        ConsentForm.CONTACT_TEL_NO = model.C_AS_CONSENT.CONTACT_TEL_NO;
                        ConsentForm.DATE_OF_CONSENT = DateTime.Today;
                        if (file.ElementAt(0) != null)
                        {
                            ConsentForm.FILE_PATH_NONRESTRICTED = ConsentForm.UUID + "0" + fileExt0;
                        }
                        else
                        {
                            ConsentForm.FILE_PATH_NONRESTRICTED = null;
                        }
                        ConsentForm.MODIFIED_DATE = System.DateTime.Now;
                        ConsentForm.MODIFIED_BY = SystemParameterConstant.UserName;
                        ConsentForm.CREATED_DATE = System.DateTime.Now;
                        ConsentForm.CREATED_BY = SystemParameterConstant.UserName;
                        db.C_AS_CONSENT.Add(ConsentForm);
                    }
                    db.SaveChanges();
                    if (file.ElementAt(0) != null)
                    {
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        file.ElementAt(0).SaveAs(Path.Combine(path, Path.GetFileName(model.C_AS_CONSENT.UUID + "0" + fileExt0)));
                    }
                }
                return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
            }
            catch (Exception ex)
            {
                return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { ex.Message } };
            }
        }
        public void DeleteASImg(string id)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                C_AS_CONSENT asConsent = db.C_AS_CONSENT.Where(o => o.UUID == id).FirstOrDefault();
                asConsent.FILE_PATH_NONRESTRICTED = null;
                db.SaveChanges();
            }
        }
        public FileStreamResult exportASReport(Fn06AS_DBSearchModel model)
        {
            //List<List<object>> list = GetASReportData(model);
            //Header
            List<string> columnHeaders = new List<string>();
            columnHeaders.Add("Name");
            columnHeaders.Add("BR Number");
            columnHeaders.Add("District Area");


            //return exportASExcelFile("RegisteredCompanyReport", columnHeaders, list);
            return null;
        }
        public FileStreamResult exportASExcelFile(string fileName, List<string> Columns, List<List<object>> Data)
        {
            MemoryStream stream = new MemoryStream();
            ExcelPackage ep = new ExcelPackage();
            ExcelWorksheet sheet = ep.Workbook.Worksheets.Add("Export");
            for (int i = 0; i < Columns.Count; i++)
            {
                sheet.Cells[1, i + 1].LoadFromText(Columns[i].ToString());
                sheet.Cells[1, i + 1].Style.Font.Bold = true;
                sheet.Cells[1, i + 1].Style.Font.Size = 14;
                sheet.Cells[1, i + 1].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
            }
            if (Data != null)
            {
                for (int i = 0; i < Data.Count; i++)
                {
                    List<object> eachRow = Data[i];
                    for (int j = 0; j < Columns.Count; j++)
                    {
                        sheet.Cells[i + 2, j + 1].LoadFromText(eachRow[j].ToString());
                    }
                }
            }
            sheet.Cells.AutoFitColumns();
            ep.SaveAs(stream);
            stream.Position = 0;
            FileStreamResult fsr = new FileStreamResult(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            fsr.FileDownloadName = fileName + ".xlsx";
            return fsr;
        }

        public FileResult ViewCRMImageByUUID(string uuid)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                var query = db.C_AS_CONSENT.Where(a => a.UUID == uuid).FirstOrDefault();
                byte[] fileBytes = new byte[] { };
                if (query != null)
                {
                    string filePath = ApplicationConstant.CRMFilePath + RegistrationConstant.ASFORM_PATH + ApplicationConstant.FileSeparator.ToString() + query.FILE_PATH_NONRESTRICTED;
                    if (File.Exists(filePath))
                    {
                        fileBytes = File.ReadAllBytes(filePath);
                        if (query.FILE_PATH_NONRESTRICTED.Contains("jpg"))
                        {
                            return new FileContentResult(fileBytes, System.Net.Mime.MediaTypeNames.Image.Jpeg);

                        }
                        else if (query.FILE_PATH_NONRESTRICTED.Contains("gif"))
                        {
                            return new FileContentResult(fileBytes, System.Net.Mime.MediaTypeNames.Image.Gif);
                        }
                        else if (query.FILE_PATH_NONRESTRICTED.Contains("png"))
                        {
                            return new FileContentResult(fileBytes, "image/png");
                        }
                    }
                }
                return null;
            }

        }
    }
}
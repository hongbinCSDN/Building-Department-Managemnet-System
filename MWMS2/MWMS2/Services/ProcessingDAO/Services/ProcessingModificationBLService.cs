using MWMS2.Areas.Admin.Models;
using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Services.AdminService.DAO;
using Newtonsoft.Json;
using NPOI.OpenXmlFormats.Wordprocessing;
using NPOI.XWPF.UserModel;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Services
{
    public class ProcessingModificationBLService
    {
        private ProcessingModificationDAOService DaoService;

        protected ProcessingModificationDAOService DA
        {
            get { return DaoService ?? (DaoService = new ProcessingModificationDAOService()); }
        }

        /// <summary>
        /// Get Form Model
        /// </summary>
        /// <returns></returns>
        public Fn01LM_ModModel GetFormModel()
        {
            Fn01LM_ModModel model = new Fn01LM_ModModel();
            model.P_MW_MODIFICATION = new Entity.P_MW_MODIFICATION();
            model.P_MW_MODIFICATION.RECEIVED_DATE = DateTime.Now;
            return model;
        }



        /// <summary>
        /// Search Mod
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult SearchMod(Fn01LM_SearchModModel model)
        {
            return new ContentResult()
            {
                Content = JsonConvert.SerializeObject(DA.SearchMod(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }),
                ContentType = "application/json"
            };
        }

        public ActionResult SearchModOfToday()
        {
            return new ContentResult()
            {
                Content = JsonConvert.SerializeObject(DA.SearchModOfToday(), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }),
                ContentType = "application/json"
            };
        }

        public ContentResult SearchIncomingDoc(Fn01LM_ModModel model)
        {
            return new ContentResult()
            {
                Content = JsonConvert.SerializeObject(DA.SearchIncomingDoc(model.P_MW_MODIFICATION.UUID), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }),
                ContentType = "application/json"
            };
        }

        public ContentResult SearchOutgoingDoc(Fn01LM_ModModel model)
        {
            return new ContentResult()
            {
                Content = JsonConvert.SerializeObject(DA.SearchOutgoingDoc(model.P_MW_MODIFICATION.UUID), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }),
                ContentType = "application/json"
            };
        }

        /// <summary>
        /// Create Modification
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult Create(FormCollection formCollection, Fn01LM_ModModel model)
        {
            ServiceResult serviceResult = new ServiceResult();
            try
            {
                model.P_MW_MODIFICATION.IS_BUILDING_WORKS = (model.WorkType == "B").ToString();
                model.P_MW_MODIFICATION.IS_STREET_WORKS = (model.WorkType == "S").ToString();

                //model.P_MW_MODIFICATION.HANDING_STAFF = model.HandingStaff;

                model.P_MW_MODIFICATION = DA.Create(model);

                if (!string.IsNullOrEmpty(model.P_MW_MODIFICATION.UUID))
                {
                    //model.listMwRefNo = new List<string>();
                    List<P_MW_MODIFICATION_RELATED_MWNO> listMwNo = new List<P_MW_MODIFICATION_RELATED_MWNO>();
                    //Get MW_NO
                    foreach (string item in formCollection.AllKeys.Where(d => d.Contains("MwRefNo")))
                    {
                        if (!string.IsNullOrWhiteSpace(formCollection[item].ToString()))
                        {
                            listMwNo.Add(new P_MW_MODIFICATION_RELATED_MWNO()
                            {
                                MODIFICATION_ID = model.P_MW_MODIFICATION.UUID,
                                MW_NO = formCollection[item].ToString().Trim()
                            });
                            //model.listMwRefNo.Add(formCollection[item].ToString());
                        }
                    }

                    //Save Mw_No
                    DA.SaveMwNo(listMwNo, model.P_MW_MODIFICATION.UUID);
                }

                serviceResult.Result = string.IsNullOrEmpty(model.P_MW_MODIFICATION.UUID) ? ServiceResult.RESULT_FAILURE : ServiceResult.RESULT_SUCCESS;
            }
            catch (Exception e)
            {
                serviceResult.Result = ServiceResult.RESULT_FAILURE;
                serviceResult.Message = new List<string>() { e.Message };
            }
            return new JsonResult() { Data = serviceResult };
        }

        /// <summary>
        /// Get Modification Item
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        public Fn01LM_ModModel GetModification(string uuid)
        {
            if (string.IsNullOrEmpty(uuid)) { return null; }
            Fn01LM_ModModel model = new Fn01LM_ModModel();
            model.P_MW_MODIFICATION = DA.GetModification(uuid);
            //model.HandingStaff = model.P_MW_MODIFICATION.HANDING_STAFF;
            model.P_MOD_BD106 = DA.GetModBd106(uuid);
            model.listMwNo = DA.GetWmNoList(uuid);
            model.listBD106Item = DA.GetModBd106Items<P_MOD_BD106_ITEM_View>(uuid);
            List<P_MOD_BD106_ITEM> listBD106ItemSelected = model.P_MOD_BD106 == null ? new List<P_MOD_BD106_ITEM>() : DA.GetModBD106ItemByBD106(model.P_MOD_BD106.UUID);
            if (listBD106ItemSelected.Count() > 0)
            {
                foreach (P_MOD_BD106_ITEM item in listBD106ItemSelected)
                {
                    model.listBD106Item.Find(m => m.S_SYSTEM_VALUE_UUID == item.S_SYSTEM_VALUE_UUID).IsChecked = true;
                }
            }
            return model;
        }

        public ActionResult Update(FormCollection formCollection, Fn01LM_ModModel model)
        {
            model.listMwRefNo = new List<string>();
            foreach (string item in formCollection.AllKeys.Where(d => d.Contains("MwRefNo")))
            {
                model.listMwRefNo.Add(formCollection[item].ToString());
            }
            ServiceResult serviceResult = new ServiceResult();
            try
            {
                Fn01LM_ModModel record = GetModification(model.P_MW_MODIFICATION.UUID);

                record.P_MW_MODIFICATION.EMAIL = model.P_MW_MODIFICATION.EMAIL;
                record.P_MW_MODIFICATION.ADDRESS = model.P_MW_MODIFICATION.ADDRESS;
                record.P_MW_MODIFICATION.LOT_NO = model.P_MW_MODIFICATION.LOT_NO;
                record.P_MW_MODIFICATION.APPLICANT_NAME = model.P_MW_MODIFICATION.APPLICANT_NAME;
                record.P_MW_MODIFICATION.RECEIVED_DATE = model.P_MW_MODIFICATION.RECEIVED_DATE;
                record.P_MW_MODIFICATION.REGULATIONS = model.P_MW_MODIFICATION.REGULATIONS;
                record.WorkType = model.WorkType;
                record.P_MW_MODIFICATION.DESC_OF_MODI = model.P_MW_MODIFICATION.DESC_OF_MODI;

                record.P_MW_MODIFICATION.LOC_OF_SUBJECT = model.P_MW_MODIFICATION.LOC_OF_SUBJECT;
                record.P_MW_MODIFICATION.UNABLE_TO_COMPLY_REASON = model.P_MW_MODIFICATION.UNABLE_TO_COMPLY_REASON;
                record.P_MW_MODIFICATION.SUPPORTING_DOCUMENT = model.P_MW_MODIFICATION.SUPPORTING_DOCUMENT;
                record.P_MW_MODIFICATION.APPLICANT_NAME = model.P_MW_MODIFICATION.APPLICANT_NAME;
                record.P_MW_MODIFICATION.RECEIVED_DATE = model.P_MW_MODIFICATION.RECEIVED_DATE;
                record.P_MW_MODIFICATION.APPLICANT_CAPACITY = model.P_MW_MODIFICATION.APPLICANT_CAPACITY;

                //record.P_MW_MODIFICATION.HANDING_STAFF = model.HandingStaff;
                record.P_MW_MODIFICATION.HANDING_STAFF = model.P_MW_MODIFICATION.HANDING_STAFF;

                serviceResult.Result = DA.Update(record) > 0 ? ServiceResult.RESULT_SUCCESS : ServiceResult.RESULT_FAILURE;

                if (serviceResult.Result == ServiceResult.RESULT_SUCCESS)
                {
                    //model.listMwRefNo = new List<string>();
                    List<P_MW_MODIFICATION_RELATED_MWNO> listMwNo = new List<P_MW_MODIFICATION_RELATED_MWNO>();
                    //Get MW_NO
                    foreach (string item in formCollection.AllKeys.Where(d => d.Contains("MwRefNo")))
                    {
                        if (!string.IsNullOrWhiteSpace(formCollection[item].ToString()))
                        {
                            listMwNo.Add(new P_MW_MODIFICATION_RELATED_MWNO()
                            {
                                MODIFICATION_ID = model.P_MW_MODIFICATION.UUID,
                                MW_NO = formCollection[item].ToString().Trim()
                            });
                            //model.listMwRefNo.Add(formCollection[item].ToString());
                        }

                    }

                    //Save Mw_No
                    DA.SaveMwNo(listMwNo, model.P_MW_MODIFICATION.UUID);
                }
            }
            catch (Exception e)
            {
                serviceResult.Result = ServiceResult.RESULT_FAILURE;
                serviceResult.Message = new List<string>() { e.Message };
            }
            return new JsonResult() { Data = serviceResult };
        }

        // Begin Add by Chester

        public ServiceResult SaveBD106(FormCollection formCollection, Fn01LM_ModModel model)
        {
            model.ListSiteInspectComp = new List<string>();
            foreach (string item in formCollection.AllKeys.Where(d => d.Contains("siteInspectComp")))
            {
                model.ListSiteInspectComp.Add(formCollection[item].ToString());
            }
            model.ListSiteInspectDate = new List<string>();
            foreach (string item in formCollection.AllKeys.Where(d => d.Contains("siteInspectDate")))
            {
                model.ListSiteInspectDate.Add(formCollection[item].ToString());
            }
            if (DA.GetModBd106(model.P_MW_MODIFICATION.UUID) != null)
            {
                return DA.UpdateBD106(model);
            }
            return DA.SaveBD106(model);
        }

        public ServiceResult GetMWNo(string dsn)
        {
            return DA.GetMWNo(dsn);
        }

        //public FileStreamResult ExcelMod(Fn01LM_SearchModModel model)
        //{
        //    List<string> headerList = new List<string>()
        //    {
        //        "Ref No.","Form Type","DSN No.","Received Date","Handing staff(PO)","Status"
        //        ,"ADDRESS_ENG","ADDRESS_CHI","RECEIVED_DATE_OF_FORM_BA16","RESULT_OF_THE_APPLICATION"
        //        ,"ISSUE_DATE_OF_BD106","NO_OF_APPROVED_FLATS_INVOLVED"
        //        ,"NO_OF_CUBICLES_AFTER_SUBDIVISION","FLOOR_AREA_OF_SUBDIVIDED_CUBICLES","COMPLETION_DATE"
        //        ,"ANNUAL_INSPECTION_DATE","CREATED_BY","CREATED_DATE","MODIFIED_BY","MODIFIED_DATE"
        //        ,"LSS_REFERRAL_DATE","EBD_REFERRAL_DATE","SITE_INSP_COMPLETED","SITE_INSP_DATE"
        //        ,"INSP_RESULT","IS_VALID","PERMIT_NO","OUR_REF_NO","STATUS","Regulation(s) exempted/modified list"
        //    };
        //    List<List<object>> data = DA.ExcelMod(model);
        //    BaseCommonService baseService = new BaseCommonService();
        //    return baseService.exportExcelFile("Modification", headerList, data);
        //}
        public Fn01LM_SearchModModel ExcelMod(Fn01LM_SearchModModel model)
        {
            List<string> headerList = new List<string>()
            {
                "Ref No.","Form Type","DSN No.","Received Date","Handling staff(PO)","Status"
                ,"ADDRESS_ENG","ADDRESS_CHI","RECEIVED_DATE_OF_FORM_BA16","RESULT_OF_THE_APPLICATION"
                ,"ISSUE_DATE_OF_BD106","NO_OF_APPROVED_FLATS_INVOLVED"
                ,"NO_OF_CUBICLES_AFTER_SUBDIVISION","FLOOR_AREA_OF_SUBDIVIDED_CUBICLES","COMPLETION_DATE"
                ,"ANNUAL_INSPECTION_DATE","CREATED_BY","CREATED_DATE","MODIFIED_BY","MODIFIED_DATE"
                ,"LSS_REFERRAL_DATE","EBD_REFERRAL_DATE","SITE_INSP_COMPLETED","SITE_INSP_DATE"
                ,"INSP_RESULT","IS_VALID","PERMIT_NO","OUR_REF_NO","STATUS","Regulation(s) exempted/modified list"
            };
            Dictionary<string, string> col1 = model.CreateExcelColumn("Ref No.", "REFERENCE_NO");
            Dictionary<string, string> col2 = model.CreateExcelColumn("Form Type", "FORM_NO");
            Dictionary<string, string> col3 = model.CreateExcelColumn("DSN No.", "DSN");
            Dictionary<string, string> col4 = model.CreateExcelColumn("Received Date", "RECEIVED_DATE");
            Dictionary<string, string> col5 = model.CreateExcelColumn("Handling staff(PO)", "HANDING_STAFF");
            Dictionary<string, string> col6 = model.CreateExcelColumn("Status", "RRM_SYN_STATUS");
            Dictionary<string, string> col7 = model.CreateExcelColumn("ADDRESS_ENG", "ADDRESS_ENG");
            Dictionary<string, string> col8 = model.CreateExcelColumn("ADDRESS_CHI", "ADDRESS_CHI");
            Dictionary<string, string> col9 = model.CreateExcelColumn("RECEIVED_DATE_OF_FORM_BA16", "RECEIVED_DATE_OF_FORM_BA16");
            Dictionary<string, string> col10 = model.CreateExcelColumn("RESULT_OF_THE_APPLICATION", "RESULT_OF_THE_APPLICATION");
            Dictionary<string, string> col11 = model.CreateExcelColumn("ISSUE_DATE_OF_BD106", "ISSUE_DATE_OF_BD106");
            Dictionary<string, string> col12 = model.CreateExcelColumn("NO_OF_APPROVED_FLATS_INVOLVED", "NO_OF_APPROVED_FLATS_INVOLVED");
            Dictionary<string, string> col13 = model.CreateExcelColumn("NO_OF_CUBICLES_AFTER_SUBDIVISION", "NO_OF_CUBICLES_AFTER_SUBDIVISION");
            Dictionary<string, string> col14 = model.CreateExcelColumn("FLOOR_AREA_OF_SUBDIVIDED_CUBICLES", "FLOOR_AREA_OF_SUBDIVIDED_CUBICLES");
            Dictionary<string, string> col15 = model.CreateExcelColumn("COMPLETION_DATE", "COMPLETION_DATE");
            Dictionary<string, string> col16 = model.CreateExcelColumn("ANNUAL_INSPECTION_DATE", "ANNUAL_INSPECTION_DATE");
            Dictionary<string, string> col17 = model.CreateExcelColumn("CREATED_BY", "CREATED_BY");
            Dictionary<string, string> col18 = model.CreateExcelColumn("CREATED_DATE", "CREATED_DATE");
            Dictionary<string, string> col19 = model.CreateExcelColumn("MODIFIED_BY", "MODIFIED_BY");
            Dictionary<string, string> col20 = model.CreateExcelColumn("MODIFIED_DATE", "MODIFIED_DATE");
            Dictionary<string, string> col21 = model.CreateExcelColumn("LSS_REFERRAL_DATE", "LSS_REFERRAL_DATE");
            Dictionary<string, string> col22 = model.CreateExcelColumn("EBD_REFERRAL_DATE", "EBD_REFERRAL_DATE");
            Dictionary<string, string> col23 = model.CreateExcelColumn("SITE_INSP_COMPLETED", "SITE_INSP_COMPLETED");
            Dictionary<string, string> col24 = model.CreateExcelColumn("SITE_INSP_DATE", "SITE_INSP_DATE");
            Dictionary<string, string> col25 = model.CreateExcelColumn("INSP_RESULT", "INSP_RESULT");
            Dictionary<string, string> col26 = model.CreateExcelColumn("IS_VALID", "IS_VALID");
            Dictionary<string, string> col27 = model.CreateExcelColumn("PERMIT_NO", "PERMIT_NO");
            Dictionary<string, string> col28 = model.CreateExcelColumn("OUR_REF_NO", "OUR_REF_NO");
            Dictionary<string, string> col29 = model.CreateExcelColumn("STATUS", "STATUS");
            Dictionary<string, string> col30 = model.CreateExcelColumn("Regulation(s) exempted/modified list", "CODE");

            model.Columns = new Dictionary<string, string>[]
            {
                col1,col2,col3
                ,col4,col5,col6
                ,col7,col8,col9
                ,col10,col11,col12
                ,col13,col14,col15
                ,col16,col17,col18
                ,col19,col20,col21
                ,col22,col23,col24
                ,col25,col26,col27
                ,col28,col29,col30
            };
            DA.ExcelMod(model);
            return model;
        }

        public DisplayGrid ExcelBA16()
        {
            return DA.SearchModOfToday();
        }

        public XWPFDocument PrintWord(string id, string tempPath)
        {
            Fn01LM_ModModel modificationModel = GetModification(id);
            PEM1103DAOService PEM1103DA = new PEM1103DAOService();
            PEM1103AckLetterTemplateSignatureModel letterTemplateSignatureModel = PEM1103DA.SearchAckLetterTemplateSignature(ProcessingConstant.S_TYPE_MOD_LETTER_TEMPLETE);
            Fn01LM_PrintModModel model = new Fn01LM_PrintModModel()
            {
                UUID = id
                ,
                REF_NO = modificationModel.P_MW_MODIFICATION.REFERENCE_NO
                ,
                RECEIVED_DATE = modificationModel.P_MW_MODIFICATION.RECEIVED_DATE?.ToString("dd.MM.yyyy")
                ,
                PERMIT_NO = modificationModel.P_MOD_BD106.PERMIT_NO
                ,
                ADDRESS = modificationModel.P_MW_MODIFICATION.ADDRESS
                ,
                ANNUAL_DATE = modificationModel.P_MOD_BD106.ANNUAL_INSPECTION_DATE?.ToString("dd.MM.yyyy")
                ,
                ISSUE_DATE = modificationModel.P_MOD_BD106.ISSUE_DATE_OF_BD106?.ToString("dd.MM.yyyy")
                ,ModEngSpoName = letterTemplateSignatureModel.SPO_Eng_Name
                ,ModChiSpoName = letterTemplateSignatureModel.SPO_Chi_Name
                ,ModEngPosition = letterTemplateSignatureModel.SPO_Eng_Position
                ,ModChiPosition = letterTemplateSignatureModel.SPO_Chi_Position
            };
            return BaseCommonService.GetWordDocument<Fn01LM_PrintModModel>(tempPath, model, null, ProcessingConstant.ACKNOWLEDGEMENT_LETTER_ENG_FONTFAMILY);
        }
        // End Add by Chester
    }
}
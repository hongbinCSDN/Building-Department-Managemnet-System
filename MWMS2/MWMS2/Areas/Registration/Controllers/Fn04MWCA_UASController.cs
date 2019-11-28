using MWMS2.Models;
using MWMS2.Services;
using MWMS2.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Areas.Registration.Models;
using MWMS2.Constant;
using System.Text;
using System.IO;

namespace MWMS2.Areas.Registration.Controllers
{
    public class Fn04MWCA_UASController : Controller
    {
        // GET: Registration/Fn04MWCA_UAS
        public ActionResult Index(UpdateAppStatusSearchModel model)
        {
            return View(model);
        }
        //public JsonResult Search()
        //{
        //    DisplayGrid dlr = demoSearch();
        //    return Json(dlr);
        //}
        public ActionResult Search(UpdateAppStatusSearchModel model)
        {
            RegistrationCommonService rs = new RegistrationCommonService();
            return Content(JsonConvert.SerializeObject(rs.SearchUASforGCAandMWCA(model,RegistrationConstant.REGISTRATION_TYPE_MWCA), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        public ActionResult Form(string id)
        {
            RegistrationCommonService rs = new RegistrationCommonService();
            UpdateAppStatusDisplayModel model = rs.ViewCompUAS(id);
            return View("FormMWCA_UAS", model);
        }
        public ActionResult CalculateDate(string id,string registrationDate, string selectedType, string NewRegValPerList, string RetValPerList, string ResValPerList)

        {
            RegistrationCommonService rs = new RegistrationCommonService();
            UpdateAppStatusDisplayModel model = rs.ViewCompUAS(id);
            model = rs.CalDate(selectedType, registrationDate, NewRegValPerList, RetValPerList, ResValPerList, model);

            if (model.C_COMP_APPLICATION.EXTEND_DATE == null)
            {
                var result = new
                {
                    NEWREGEXTEND_DATE = "",
                    RETEXTEND_DATE = "",
                    RESEXTEND_DATE = "",
                    DateResApp = model.dateRestorationApplicationRes.ToString(),
                    DateRetApp = model.dateRetentionApplcationRet.ToString()
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var result = new
                {
                    NEWREGEXTEND_DATE = model.NewRegExtDate.ToString(),
                    RETEXTEND_DATE = model.RetExtDate.ToString(),
                    RESEXTEND_DATE = model.ResExtDate.ToString(),
                    DateResApp = model.dateRestorationApplicationRes.ToString(),
                    DateRetApp = model.dateRetentionApplcationRet.ToString()

                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Save([Bind(Exclude = "")]UpdateAppStatusDisplayModel model)
        {
            RegistrationCommonService rs = new RegistrationCommonService();
            rs.SaveComp_status(model);
            UpdateAppStatusDisplayModel model1 = rs.ViewCompUAS(model.C_COMP_APPLICATION.UUID);
            model1.SaveSuccess = true;
            return View("FormMWCA_UAS", model1);
        }
        //[HttpPost]
        //public ActionResult Excel(FormCollection post, Dictionary<string, string>[] Columns)
        //{
        //    if (post.AllKeys.Contains("key")) return DisplayGrid.getMemory(post["key"]);
        //    RegistrationMWCAService registrationMCAService = new RegistrationMWCAService();
        //    return Json(new { key = registrationMCAService.ExportTemp(Columns, post) });
        //}
        public ActionResult Excel(UpdateAppStatusSearchModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            RegistrationCommonService rs = new RegistrationCommonService();
            return Json(new { key = rs.ExportUASforGCAandMWCAReport(model, RegistrationConstant.REGISTRATION_TYPE_MWCA) });
        }
        public DisplayGrid demoSearch()
        {
            DisplayGrid dlr = new DisplayGrid();


            //dlr.Columns = new string[] { "File Reference", "Company Name", "Reg. No.", "Date of Gazette", "Date of Expiry", "Date of Retention", "Date of Restoration", "Status" };
            //dlr.Datas = new List<object[]>();
            ////dlr.Datas.Add(new object[] { "v11", "v12", "v13", "v14", "v15", "v16", "v17", "v18", });
            ////dlr.Datas.Add(new object[] { "v21", "v22", "v23", "v24", "v25", "v26", "v27", "v28", });
            ////dlr.Datas.Add(new object[] { "v31", "v32", "v33", "v34", "v35", "v36", "v37", "v38", });

            dlr.Rpp = 25;
            ////dlr.TotalRecord = 3;
            ////dlr.CurrentPage = 1;
            return dlr;
        }


        // Export Comp certificate
        public FileStreamResult ExportCertificate(String process, String certNo, String compAppUuid
            , String rptIssueDt, String rptRcvdDt, String signature)
        {
            RegistrationApplicationStatusService registrationApplicationStatusService = new RegistrationApplicationStatusService();
            String certificateContent = registrationApplicationStatusService.populateCompExportCert(process, compAppUuid, rptIssueDt, rptRcvdDt, signature);
            var byteArray = Encoding.UTF8.GetBytes(certificateContent);
            var stream = new MemoryStream(byteArray);
            return File(stream, "text/plain", "tmp.txt");
        }

        // Export Comp Letter
        public FileStreamResult ExportLetter(String process, String certNo, String compAppUuid
            , String rptIssueDt, String rptRcvdDt, String signature)
        {
            RegistrationApplicationStatusService registrationApplicationStatusService = new RegistrationApplicationStatusService();
            String certificateContent = registrationApplicationStatusService.populateCompExportLetter(process, compAppUuid, rptIssueDt, rptRcvdDt, signature);
            var byteArray = Encoding.UTF8.GetBytes(certificateContent);
            var stream = new MemoryStream(byteArray);
            return File(stream, "text/plain", "tmp.txt");
        }

        // Export Comp Letter with code
        public FileStreamResult ExportLetterWithCode(String process, String certNo, String compAppUuid
            , String rptIssueDt, String rptRcvdDt, String signature
            , String missingItemRetensionNullChk, String missingItemRetensionChequeChk, String missingItemRetensionProfRegCertChk
            , String missingItemRetensionOthersChk, String missingItemRetensionIncompleteFormChk
            , String missingItemRestorationNullChk, String missingItemRestorationChequeChk, String missingItemRestorationProfRegCertChk
            , String missingItemRestorationOthersChk, String missingItemRestorationIncompleteFormChk)
        {
            RegistrationApplicationStatusService registrationApplicationStatusService = new RegistrationApplicationStatusService();
            String certificateContent = registrationApplicationStatusService.populateCompExportLetterWithCode(certNo, process, compAppUuid, rptIssueDt, rptRcvdDt, signature
                           , missingItemRetensionNullChk, missingItemRetensionChequeChk, missingItemRetensionProfRegCertChk
                           , missingItemRetensionOthersChk, missingItemRetensionIncompleteFormChk
                           , missingItemRestorationNullChk, missingItemRestorationChequeChk, missingItemRestorationProfRegCertChk
                           , missingItemRestorationOthersChk, missingItemRestorationIncompleteFormChk
                );
            var byteArray = Encoding.UTF8.GetBytes(certificateContent);
            var stream = new MemoryStream(byteArray);
            return File(stream, "text/plain", "tmp.txt");
        }

    }
}
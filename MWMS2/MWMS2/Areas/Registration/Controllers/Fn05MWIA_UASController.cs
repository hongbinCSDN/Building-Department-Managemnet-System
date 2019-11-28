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
    public class Fn05MWIA_UASController : Controller
    {
        // GET: Registration/Fn05MWIA_UAS
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
            return Content(JsonConvert.SerializeObject(rs.SearchUASforPAandMWIA(model, RegistrationConstant.REGISTRATION_TYPE_MWIA), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        public ActionResult Excel(UpdateAppStatusSearchModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            RegistrationCommonService rs = new RegistrationCommonService();
            return Json(new { key = rs.ExportUASforPAandMWIAReport(model, RegistrationConstant.REGISTRATION_TYPE_MWIA) });
        }
        public ActionResult Form(string id,string code)
        {
            RegistrationCommonService rs = new RegistrationCommonService();
            UpdateAppStatusDisplayModel model = rs.ViewIndUAS(id,code);
            return View("FormMWIA_UAS", model);
        }
        public ActionResult Save([Bind(Exclude = "")]UpdateAppStatusDisplayModel model)
        {
            RegistrationCommonService rs = new RegistrationCommonService();
            rs.SaveInd_status(model);
            UpdateAppStatusDisplayModel model1 = rs.ViewIndUAS(model.C_IND_APPLICATION.UUID,model.CatCode);
            model1.SaveSuccess = true;
            return View("FormMWIA_UAS", model1);
        }
        public ActionResult CalculateIndDate(string id, string selectedType,string registrationDate, string NewRegValPerList, string RetValPerList, string ResValPerList,string code)

        {
            RegistrationCommonService rs = new RegistrationCommonService();
            UpdateAppStatusDisplayModel model = rs.ViewIndUAS(id, code);
            model = rs.CalIndDate(selectedType, registrationDate, NewRegValPerList, RetValPerList, ResValPerList, model);

            if (model.C_IND_CERTIFICATE.EXTENDED_DATE == null)
            {
                var result = new
                {
                    NEWREGEXTEND_DATE = "",
                    RETEXTEND_DATE = "",
                    RESEXTEND_DATE = "",
                    DateResApp = model.dateIndRestorationApplicationRes.ToString(),
                    DateRetApp = model.dateIndRetentionApplcationRet.ToString()
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var result = new
                {
                    NEWREGEXTEND_DATE = model.IndNewRegExtDate.ToString(),
                    RETEXTEND_DATE = model.IndRetExtDate.ToString(),
                    RESEXTEND_DATE = model.IndResExtDate.ToString(),
                    DateResApp = model.dateIndRestorationApplicationRes.ToString(),
                    DateRetApp = model.dateIndRetentionApplcationRet.ToString()

                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        //[HttpPost]
        //public ActionResult Excel(FormCollection post, Dictionary<string, string>[] Columns)
        //{
        //    if (post.AllKeys.Contains("key")) return DisplayGrid.getMemory(post["key"]);
        //    RegistrationMWIAService registrationMWIAService = new RegistrationMWIAService();
        //    return Json(new { key = registrationMWIAService.ExportMWIA(Columns, post) });
        //}
        public DisplayGrid demoSearch()
        {
            DisplayGrid dlr = new DisplayGrid();


            //dlr.Columns = new string[] { "File Reference", "Category", "Name", "Chinese Name", "Reg. No.", "Date of Gazette", "Date of Expiry", "Date of Retention", "Date of Restoration", "Status" };
            //dlr.Datas = new List<object[]>();
            ////dlr.Datas.Add(new object[] { "v11", "v12", "v13", "v14", "v15", "v16", "v17", "v18", "v19", "V20" });
            ////dlr.Datas.Add(new object[] { "v21", "v22", "v23", "v24", "v25", "v26", "v27", "v28", "v29", "V30" });
            ////dlr.Datas.Add(new object[] { "v31", "v32", "v33", "v34", "v35", "v36", "v37", "v38", "v39", "V40" });

            dlr.Rpp = 25;
            ////dlr.TotalRecord = 3;
            ////dlr.CurrentPage = 1;
            return dlr;
        }

        //public FileStreamResult ExportCertificate(String process, String certNo, String compAppUuid
        //    , String issueDt, String lastDocRecDt, String signature)
        //{
        //    RegistrationCommonService registrationCommonService = new RegistrationCommonService();
        //    //sString certificateContent = registrationCommonService.exportCompUpdateAppStatus("" ,process, certNo, compAppUuid, issueDt, lastDocRecDt, signature);
        //    //var byteArray = Encoding.UTF8.GetBytes(certificateContent);
        //    //var stream = new MemoryStream(byteArray);
        //    //return File(stream, "text/plain", "tmp.txt");
        //    return null;
        //}

        // Export Individual certificate
        public FileStreamResult ExportCertificate(String process, String certNo, String indCertUuid
            , String rptIssueDt, String rptRcvdDt, String signature)
        {
            RegistrationApplicationStatusService registrationApplicationStatusService = new RegistrationApplicationStatusService();
            String certificateContent = registrationApplicationStatusService.populateProfExportCert(process, indCertUuid, rptIssueDt, rptRcvdDt, signature);
            var byteArray = System.Text.Encoding.UTF8.GetBytes(certificateContent);
            var stream = new MemoryStream(byteArray);
            return File(stream, "text/plain", "tmp.txt");
        }

        // Export Individual QP Card
        public FileStreamResult ExportQPCard(String process, String certNo, String indCertUuid
            , String rptIssueDt, String rptRcvdDt, String signature)
        {
            RegistrationApplicationStatusService registrationApplicationStatusService = new RegistrationApplicationStatusService();
            String certificateContent = registrationApplicationStatusService.populateProfExportQPCard(process, indCertUuid, rptIssueDt, rptRcvdDt, signature);
            var byteArray = System.Text.Encoding.UTF8.GetBytes(certificateContent);
            var stream = new MemoryStream(byteArray);
            return File(stream, "text/plain", "tmp.txt");
        }

        // Export Individual Letter Card
        public FileStreamResult ExportLetter(String process, String certNo, String indCertUuid
            , String rptIssueDt, String rptRcvdDt, String signature)
        {
            RegistrationApplicationStatusService registrationApplicationStatusService = new RegistrationApplicationStatusService();
            String certificateContent = registrationApplicationStatusService.populateProfExportLetter(process, indCertUuid, rptIssueDt, rptRcvdDt, signature);
            var byteArray = System.Text.Encoding.UTF8.GetBytes(certificateContent);
            var stream = new MemoryStream(byteArray);
            return File(stream, "text/plain", "tmp.txt");
        }

        // Export Individual Letter Card
        public FileStreamResult ExportLetterWithCode(String process, String certNo, String indCertUuid
            , String rptIssueDt, String rptRcvdDt, String signature)
        {
            RegistrationApplicationStatusService registrationApplicationStatusService = new RegistrationApplicationStatusService();
            String certificateContent = registrationApplicationStatusService.populateProfExportLetterWithCode(process, certNo, indCertUuid, rptIssueDt, rptRcvdDt, signature);
            var byteArray = System.Text.Encoding.UTF8.GetBytes(certificateContent);
            var stream = new MemoryStream(byteArray);
            return File(stream, "text/plain", "tmp.txt");
        }
    }
}
﻿using MWMS2.Models;
using MWMS2.Utility;
using MWMS2.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Areas.Registration.Models;
using System.IO;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;
using MWMS2.Constant;

namespace MWMS2.Areas.Registration.Controllers
{
    public class Fn02GCA_UASController : Controller
    {
        // GET: Registration/Fn02GCA_USA
        public ActionResult Index(UpdateAppStatusSearchModel model)
        {
            return View(model);
        }

        public ActionResult Search(UpdateAppStatusSearchModel model)
        {
            RegistrationCommonService rs = new RegistrationCommonService();
            //rs.SearchUAS(model);
            return Content(JsonConvert.SerializeObject(rs.SearchUASforGCAandMWCA(model, RegistrationConstant.REGISTRATION_TYPE_CGA), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        public ActionResult Excel(UpdateAppStatusSearchModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            RegistrationCommonService rs = new RegistrationCommonService();
            return Json(new { key = rs.ExportUASforGCAandMWCAReport(model, RegistrationConstant.REGISTRATION_TYPE_CGA) });
        }
        //public ActionResult Excel(FormCollection post, Dictionary<string, string>[] Columns)
        //{
        //    if (post.AllKeys.Contains("key")) return DisplayGrid.getMemory(post["key"]);
        //    RegistrationGCAService rs = new RegistrationGCAService();
        //    return Json(new { key = rs.ExportTemp(Columns, post) });
        //}
        public ActionResult Form(string id)
        {
            RegistrationCommonService rs = new RegistrationCommonService();
            UpdateAppStatusDisplayModel model = rs.ViewCompUAS(id);
            return View("FormGCA_UAS", model);
        }
        public ActionResult CalculateDate(string id,string selectedType,string registrationDate,string NewRegValPerList, string RetValPerList, string ResValPerList)

        {
            RegistrationCommonService rs = new RegistrationCommonService();
            UpdateAppStatusDisplayModel model = rs.ViewCompUAS(id);
            model = rs.CalDate(selectedType, registrationDate, NewRegValPerList, RetValPerList, ResValPerList, model);

            if (model.C_COMP_APPLICATION.EXTEND_DATE == null)
            {
                var result = new
                {
                    //EXTEND_DATE = "",
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
            return View("FormGCA_UAS", model1);
        }

        [HttpPost]


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


        public ActionResult ExportCertificateRPT0011aToExcel()
        {
            var products = new System.Data.DataTable("teste");
            products.Columns.Add("col1", typeof(int));
            products.Columns.Add("col2", typeof(string));

            products.Rows.Add(1, "product 1");
            products.Rows.Add(2, "product 2");
            products.Rows.Add(3, "product 3");
            products.Rows.Add(4, "product 4");
            products.Rows.Add(5, "product 5");
            products.Rows.Add(6, "product 6");
            products.Rows.Add(7, "product 7");


            var grid = new GridView();
            grid.DataSource = products;
            grid.DataBind();

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=MyExcelFile.xls");
            Response.ContentType = "application/ms-excel";

            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            grid.RenderControl(htw);

            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            return View();
        }


        //[HttpPost]
        public ActionResult ExcelCertificateRPT0011a()
        {
            UpdateAppStatusSearchModel model = new UpdateAppStatusSearchModel();

            //if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            RegistrationSearchService rs = new RegistrationSearchService();
            string key = rs.ExportCertificate(model);
            return DisplayGrid.getMemory(key);
            //return Json(new { key = rs.ExportCertificate(model) });
        }
    }    
}
using MWMS2.Areas.Registration.Models;
using MWMS2.Constant;
using MWMS2.Models;
using MWMS2.Services;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace MWMS2.Areas.Registration.Controllers
{
    public class Fn03PA_BUController : Controller
    {
        // GET: Registration/Fn03PA_BU
        public ActionResult Index(Fn03PA_BUSearchModel model)
        {
            return View(model);
        }
        public ActionResult Search(Fn03PA_BUSearchModel model)
        {
            RegistrationCommonService rs = new RegistrationCommonService();
            
            return Content(JsonConvert.SerializeObject(rs.SearchBU(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        public ActionResult ImportExcel()
        {



            if (Request.Files["FileUpload1"].ContentLength > 0)
            {
                string extension = System.IO.Path.GetExtension(Request.Files["FileUpload1"].FileName).ToLower();
                string[] validFileTypes = { ".xls", ".xlsx", ".csv" };

                string tempPath = ApplicationConstant.CRMFilePath;
                tempPath += RegistrationConstant.BATCHUPLOAD_PATH;

                string dbSavePath = Guid.NewGuid().ToString().Replace("-", "") + extension;
                string path1 = string.Format("{0}/{1}", tempPath, dbSavePath);
                // string path1 = string.Format("{0}/{1}", tempPath, Request.Files["FileUpload1"].FileName);
                if (!Directory.Exists(path1))
                {
                    try
                    {
                        Directory.CreateDirectory(tempPath);
                    }
                    catch(Exception ex)
                    {
                        return Json(new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = new List<string> { ex.Message } });
                    }
                    
                }
                if (validFileTypes.Contains(extension))
                {
                    if (System.IO.File.Exists(path1))
                    { System.IO.File.Delete(path1); }
                    Request.Files["FileUpload1"].SaveAs(path1);

                    RegistrationBatchUploadService s = new RegistrationBatchUploadService();
                    //s.ReadExcel(path1);
                    s.SaveBatchUpload(path1);
                    s.SaveBatchUploadHistory(Request.Files["FileUpload1"].FileName, dbSavePath);

                    return Json(new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS});
                    //return RedirectToAction("Index");
                }
                else
                {
                    return Json(new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = new List<string> { "Error: File Type is no valid!!!" } });
                }
 
                // s.BatchUploadExcel(path1);


            }
            return Json(new ServiceResult() { Result = ServiceResult.RESULT_FAILURE , Message = new List<string> { "Error: Please insert file" } });
    
        }

        public FileResult DownloadFile(string path, string pathName)
        {
            RegistrationCommonService rs = new RegistrationCommonService();
            var supportedTypes = new[] { ".xls", ".xlsx", ".csv" };
            var fileExt = "";
            fileExt = System.IO.Path.GetExtension(path).Substring(1);
            string tempPath = ApplicationConstant.CRMFilePath;
            tempPath += RegistrationConstant.BATCHUPLOAD_PATH;
            byte[] fileBytesrs = rs.DownloadFile(tempPath+ ApplicationConstant.FileSeparator+ path);
            //byte[] fileBytesrs = rs.DownloadFile(Server.MapPath(leaveFormPath), path);
         
            return File(fileBytesrs, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", pathName);


        }
        [HttpPost]
        public ActionResult Excel(Fn03PA_BUSearchModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            RegistrationCommonService rs = new RegistrationCommonService();
            return Json(new { key = rs.ExportBU(model) });
        }
    }
}
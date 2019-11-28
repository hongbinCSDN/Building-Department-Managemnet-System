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
using MWMS2.Entity;
using System.IO;
using OfficeOpenXml;
using MWMS2.Constant;

namespace MWMS2.Areas.Registration.Controllers
{
    public class Fn07CNV_CNVController : Controller
    {
        // GET: Registration/Fn07CNV_CNV
        public ActionResult Index(Fn07CNV_CNVSearchModel model)
        {
            return View(model);
        }

        public ActionResult Form(string id, string type)
        {
            RegistrationCNVService rs = new RegistrationCNVService();
            if (type == "Company")
            {
                return View("FormComp", rs.ViewCompCNV(id));

            }
            else
            {
                return View("FormInd", rs.ViewIndCNV(id));

            }
            // return View(rs.ViewCNV(id));

        }

        //For creating new record
        [HttpPost]
        public ActionResult CreateNewConv()
        {
            Fn07CNV_CNVSearchModel model = new Fn07CNV_CNVSearchModel();
            return View("CreateNew",model);
        }


        public ActionResult ImportView(string importType)
        {
            Fn07CNV_CNVSearchModel model = new Fn07CNV_CNVSearchModel();
            model.ImportType = importType;
            return View("Import", model);
        }

        [HttpPost]
        public ActionResult ImportFile(string importType)
        {
            Fn07CNV_CNVSearchModel model = new Fn07CNV_CNVSearchModel();
            model.ImportType = importType;  //get part of the value 
            //return View(model);
            return RedirectToAction("Import", model);
        }

        public ActionResult ImportLD(HttpPostedFileBase file, Fn07CNV_CNVSearchModel model)
        {
            string fileName = file.FileName;
            string fileContentType = file.ContentType;
            byte[] fileBytes = new byte[file.ContentLength];
            var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));

            var compConvictionList = new List<C_COMP_CONVICTION>();
            //   RegistrationBatchUploadService ss = new RegistrationBatchUploadService();

            //    List<List<string>> rows = ss.ReadExcel(fileName);

            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                using (var package = new ExcelPackage(file.InputStream))
                //using (FileStream FS = new FileStream(pRutaArchivo, FileMode.Open, FileAccess.Read))
                {
                    var currentSheet = package.Workbook.Worksheets;
                    var workSheet = currentSheet.First();
                    int startRow = 0;
   
                   if (model.ImportType == "FEHD")
                        startRow = 6;
                    else
                        startRow = 2;


                    var noOfCol = workSheet.Dimension.End.Column;
                    var noOfRow = workSheet.Dimension.End.Row;

                    try {
                        for (int rowIterator = startRow; rowIterator <= noOfRow; rowIterator++)
                        {
                            C_COMP_CONVICTION cmp = new C_COMP_CONVICTION();

                            //  cmp.REGISTRATION_TYPE = "GBC";




                            cmp.REGISTRATION_TYPE = model.registrationType;
                            if (model.ImportType == "LD")
                            {
                                cmp.CONVICTION_SOURCE_ID = db.C_S_SYSTEM_VALUE.Where(x => x.CODE == "5" && x.C_S_SYSTEM_TYPE.TYPE == RegistrationConstant.SYSTEM_TYPE_CONVICTION_SOURCE).FirstOrDefault().UUID;
                                cmp.SRR_REPORT = ApplicationConstant.DB_UNCHECKED;
                                cmp.DA_REPORT = ApplicationConstant.DB_UNCHECKED;
                                cmp.MISC_REPORT = ApplicationConstant.DB_UNCHECKED;

                            

                                cmp.ENGLISH_NAME = CommonUtil.getDisplay(workSheet.Cells[rowIterator, 1].Value);
                                cmp.PROPRI_NAME = CommonUtil.getDisplay(workSheet.Cells[rowIterator, 2].Value);
                                cmp.SITE_DESCRIPTION = CommonUtil.getDisplay(workSheet.Cells[rowIterator, 3].Value);
                                cmp.CR_SECTION = CommonUtil.getDisplay(workSheet.Cells[rowIterator, 4].Value);
                                cmp.CR_OFFENCE_DATE = Convert.ToDateTime(CommonUtil.getDisplay(workSheet.Cells[rowIterator, 5].Value));
                                cmp.CR_JUDGE_DATE = Convert.ToDateTime(CommonUtil.getDisplay(workSheet.Cells[rowIterator, 6].Value));
                                cmp.CR_FINE = Int32.Parse(CommonUtil.getDisplay(workSheet.Cells[rowIterator, 7].Value));
                                cmp.REFERENCE = CommonUtil.getDisplay(workSheet.Cells[rowIterator, 8].Value);
                                cmp.REMARKS = CommonUtil.getDisplay(workSheet.Cells[rowIterator, 9].Value);

                                int dp = Int32.Parse(CommonUtil.getDisplay(workSheet.Cells[rowIterator, 10].Value));
                                int ip = Int32.Parse(CommonUtil.getDisplay(workSheet.Cells[rowIterator, 11].Value)); //seems useless in mwms1.0

                                if (CommonUtil.getDisplay(workSheet.Cells[rowIterator, 12].Value) == "TRUE")
                                {
                                    cmp.CR_ACCIDENT = ApplicationConstant.DB_CHECKED;
                                }
                                else
                                {
                                    cmp.CR_ACCIDENT = ApplicationConstant.DB_UNCHECKED;
                                }
                                if (dp >= 1)
                                {
                                    cmp.CR_FATAL = ApplicationConstant.DB_CHECKED;
                                }
                                else
                                {
                                    cmp.CR_FATAL = ApplicationConstant.DB_UNCHECKED;
                                }
                                if (CommonUtil.getDisplay(workSheet.Cells[rowIterator, 12].Value) == "TRUE" &&
                                        dp >= 1)
                                {
                                    cmp.CR_REPORT = ApplicationConstant.DB_CHECKED;
                                }
                                else
                                {
                                    cmp.CR_REPORT = ApplicationConstant.DB_UNCHECKED;
                                }

                            }
                            else if (model.ImportType == "FEHD")
                            {
                                cmp.ENGLISH_NAME = CommonUtil.getDisplay(workSheet.Cells[rowIterator, 2].Value);
                                cmp.SITE_DESCRIPTION = CommonUtil.getDisplay(workSheet.Cells[rowIterator, 3].Value);
                                cmp.CR_OFFENCE_DATE = Convert.ToDateTime(CommonUtil.getDisplay(workSheet.Cells[rowIterator, 4].Value));
                                cmp.CR_JUDGE_DATE = Convert.ToDateTime(CommonUtil.getDisplay(workSheet.Cells[rowIterator, 5].Value));
                                cmp.CR_FINE = Int32.Parse(CommonUtil.getDisplay(workSheet.Cells[rowIterator, 6].Value));
                                cmp.CONVICTION_SOURCE_ID = db.C_S_SYSTEM_VALUE.Where(x => x.CODE == "6" && x.C_S_SYSTEM_TYPE.TYPE == RegistrationConstant.SYSTEM_TYPE_CONVICTION_SOURCE).FirstOrDefault().UUID;



                            }
                            else if (model.ImportType == "EPD")
                            {
                                cmp.CONVICTION_SOURCE_ID = db.C_S_SYSTEM_VALUE.Where(x => x.CODE == "12" && x.C_S_SYSTEM_TYPE.TYPE == RegistrationConstant.SYSTEM_TYPE_CONVICTION_SOURCE).FirstOrDefault().UUID;

                                cmp.ENGLISH_NAME = CommonUtil.getDisplay(workSheet.Cells[rowIterator, 1].Value);
                                cmp.CHINESE_NAME = CommonUtil.getDisplay(workSheet.Cells[rowIterator, 2].Value);
                                cmp.CR_OFFENCE_DATE = Convert.ToDateTime(CommonUtil.getDisplay(workSheet.Cells[rowIterator, 3].Value));
                                cmp.CR_JUDGE_DATE = Convert.ToDateTime(CommonUtil.getDisplay(workSheet.Cells[rowIterator, 4].Value));
                                cmp.REFERENCE = CommonUtil.getDisplay(workSheet.Cells[rowIterator, 5].Value);


                            }
                            else
                            {
                                cmp.CONVICTION_SOURCE_ID = db.C_S_SYSTEM_VALUE.Where(x => x.CODE == "12" && x.C_S_SYSTEM_TYPE.TYPE == RegistrationConstant.SYSTEM_TYPE_CONVICTION_SOURCE).FirstOrDefault().UUID;

                                cmp.ENGLISH_NAME = CommonUtil.getDisplay(workSheet.Cells[rowIterator, 2].Value);
                                cmp.CHINESE_NAME = CommonUtil.getDisplay(workSheet.Cells[rowIterator, 3].Value);
                                cmp.CR_OFFENCE_DATE = Convert.ToDateTime(CommonUtil.getDisplay(workSheet.Cells[rowIterator, 4].Value));
                                cmp.CR_JUDGE_DATE = Convert.ToDateTime(CommonUtil.getDisplay(workSheet.Cells[rowIterator, 5].Value));
                                cmp.REFERENCE = CommonUtil.getDisplay(workSheet.Cells[rowIterator, 6].Value);

                            }

                            if (model.ImportType != "LD")
                            {
                                cmp.CR_ACCIDENT = ApplicationConstant.DB_UNCHECKED;
                                cmp.CR_FATAL = ApplicationConstant.DB_UNCHECKED;
                                cmp.CR_REPORT = ApplicationConstant.DB_UNCHECKED;
                                cmp.SRR_REPORT = ApplicationConstant.DB_UNCHECKED;
                                cmp.DA_REPORT = ApplicationConstant.DB_UNCHECKED;
                                cmp.MISC_REPORT = ApplicationConstant.DB_UNCHECKED;
                            }


                            cmp.RECORD_TYPE = "C";




                            cmp.IMPORT_DATE = DateTime.Now;
                            compConvictionList.Add(cmp);
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                  
                }


                for (int i = 0; i < compConvictionList.Count; i++)
                {
                    db.C_COMP_CONVICTION.Add(compConvictionList[i]);
                }

                db.SaveChanges();
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Import(HttpPostedFileBase file, Fn07CNV_CNVSearchModel model)
        {
            if (file != null && file.ContentLength > 0)
                try
                {
                        string fileExtensionName = file.FileName.Split('.').ToList<string>()[1];
                        if(fileExtensionName == "xls")
                        {
                            ViewBag.Message = "The file needs to be in xlsx format.";
                            return View(model);
                        }
                        ImportLD(file, model);

                   // string path = Path.Combine(Server.MapPath("~/Images"),
                   //                           Path.GetFileName(file.FileName));
                   //file.SaveAs(path);
                    ViewBag.Message = "File uploaded successfully";
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "ERROR:" + ex.Message.ToString();
                }
            else
            {
                ViewBag.Message = "You have not specified a file.";
            }
            return View(model);
        }


        public ActionResult Search(Fn07CNV_CNVSearchModel model)
        {
            RegistrationCNVService rs = new RegistrationCNVService();
            rs.SearchCNV(model);
            return Content(JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateNew([Bind(Exclude = "")] Fn07CNV_CNVSearchModel model)
        { 
            if (ModelState.IsValid)
            {
                RegistrationCNVService rs = new RegistrationCNVService();
                rs.CreateCNV(model);
                Fn07CNV_CNVSearchModel smodel = new Fn07CNV_CNVSearchModel();
                return View("Index", smodel);
            }
            return View(model);
        }
           
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(Fn07CNV_CNVDisplayModel model)
        {
            if (ModelState.IsValid)
            {
                RegistrationCNVService rs = new RegistrationCNVService();
                rs.SaveComp(model);
                Fn07CNV_CNVSearchModel smodel = new Fn07CNV_CNVSearchModel();

                return View("Index",smodel);
            }

            return View();
        }

        public ActionResult Delete(string id, string type)
        {
            
            RegistrationCNVService s = new RegistrationCNVService();
        
            if(type == "Company" || type == "company" || type == "CGC" || type == "CMW") {

                s.DeleteRecord(id);
            }
            else {
                s.DeleteIndRecord(id);
            }
            Fn07CNV_CNVSearchModel smodel = new Fn07CNV_CNVSearchModel();
            return View("Index", smodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveInd(Fn07CNV_CNVDisplayModel model)
        {
            if (ModelState.IsValid)
            {
                RegistrationCNVService rs = new RegistrationCNVService();
                rs.SaveIndForm(model);
                Fn07CNV_CNVSearchModel smodel = new Fn07CNV_CNVSearchModel();
                return View("Index", smodel);
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Excel(Fn07CNV_CNVSearchModel model)
        {
            //if (post.AllKeys.Contains("key")) return DisplayGrid.getMemory(post["key"]);
            //RegistrationCNVService registrationCNVService = new RegistrationCNVService();
            //return Json(new { key = registrationCNVService.ExportCNV(Columns, post) });
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            RegistrationCNVService rs = new RegistrationCNVService();
            return Json(new { key = rs.ExportCNV(model) });
        }

        public ActionResult GetApplicantById(string hkid)
        {

            RegistrationCNVService rs = new RegistrationCNVService();
            Fn07CNV_CNVSearchModel model = new Fn07CNV_CNVSearchModel();
            model = rs.GetApplicant(model, hkid);
            return Content(JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");

        }

        public ActionResult getTemplate(string ImportType)
        {
            
            string filePath = "";
            filePath += ApplicationConstant.CRMBATCHUPLOAD;

            filePath +=  ApplicationConstant.FileSeparator + ImportType  + "Sample.xlsx";

            //AuditLogService.logDebug("File Path :"+ filePath);
            if (!System.IO.File.Exists(filePath))
            {
                return Content("File not found.");
            }
            else
            {
            

                return  File(System.IO.File.ReadAllBytes(@filePath), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Sample.xlsx");
            }

        }


    }


       
    
}
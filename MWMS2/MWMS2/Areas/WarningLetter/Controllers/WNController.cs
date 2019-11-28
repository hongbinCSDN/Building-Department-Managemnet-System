using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MWMS2.Models;
using System.Data.Entity;
using System.Net;
using System.IO;
using System.Globalization;
using System.Data.Entity.Validation;
using MWMS2.Controllers;
using MWMS2.Entity;
using MWMS2.Services;
using Newtonsoft.Json;
using System.Web.Configuration;
using MWMS2.Constant;
using System.Web;
using MWMS2.Areas.Registration.Controllers;
using MWMS2.Utility;

namespace MWMS2.Areas.WarningLetter.Controllers
{
    public class WNController :  ValidationController
    {

        EntitiesWarningLetter db = new EntitiesWarningLetter();
          
        CommonFunction cf = new CommonFunction();
       
        // GET: WN
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult WNClear()
        {
            //ModelState.Clear();
            return RedirectToAction("WN");
        }
        [HttpPost]
        public ActionResult WNSearch(ModelWN wN)
        {

            WarningLetterSearchService ss = new WarningLetterSearchService();
            return Content(JsonConvert.SerializeObject(ss.SearchWL(wN), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");

     
        }

        [HttpPost]
        public ActionResult Excel(ModelWN model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            WarningLetterSearchService wl = new WarningLetterSearchService();
            return Json(new { key = wl.Export_WN(model) });
        }

        public ActionResult WN()
        {
            try
            {
                var OffenseType = from st in db.W_S_SYSTEM_TYPE
                                  join sv in db.W_S_SYSTEM_VALUE on st.UUID equals sv.S_SYSTEM_TYPE_ID
                                  where st.TYPE == "Type_Of_Offense"

                                  select sv;
                //pass offense type by viewbag
                ViewBag.OffenseType = OffenseType;


                var IrrTechType = from st in db.W_S_SYSTEM_TYPE
                                  join sv in db.W_S_SYSTEM_VALUE on st.UUID equals sv.S_SYSTEM_TYPE_ID
                                  where st.TYPE == "Technical"
                                  orderby sv.DESCRIPTION_ENG
                              select sv;
                //pass offense type by viewbag
                ViewBag.IrrTechType = IrrTechType;


                var IrrProType = from st in db.W_S_SYSTEM_TYPE
                                  join sv in db.W_S_SYSTEM_VALUE on st.UUID equals sv.S_SYSTEM_TYPE_ID
                                  where st.TYPE == "Procedural"
                                 orderby sv.DESCRIPTION_ENG
                                 select sv;
                //pass offense type by viewbag
                ViewBag.IrrProType = IrrProType;


                var IrrMisType = from st in db.W_S_SYSTEM_TYPE
                                  join sv in db.W_S_SYSTEM_VALUE on st.UUID equals sv.S_SYSTEM_TYPE_ID
                                  where st.TYPE == "Miscellaneous"
                                 orderby sv.DESCRIPTION_ENG
                                 select sv;
                //pass offense type by viewbag
                ViewBag.IrrMisType = IrrMisType;





                var MWItems = from st in db.W_S_SYSTEM_TYPE
                              join sv in db.W_S_SYSTEM_VALUE on st.UUID equals sv.S_SYSTEM_TYPE_ID
                              where st.TYPE == "Type_Of_MW_Items"

                              select sv;
                ViewBag.MWItems = MWItems;



                return View();
            }
            catch (Exception ex)
            {
                AuditLogService.logError(ex.Message + " *** " + ex.InnerException.Message);
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            //     }

        }


        public ActionResult ShowPDF(string id, int fileCount)
        {
            var query = from wl_file in db.W_WL_FILE
                        where wl_file.WL_UUID == id && wl_file.STATUS_DESCRIPTION == "Active"
                        select wl_file;

            byte[] data = null;

            data = query.ToList()[fileCount].LETTER_FILE;

            return File(data, "application/pdf");

        }


        public ActionResult WNEditDetails(string uuid, string file_UUID,string errorMsg)
        {
            try
            {
                if (errorMsg != null)
                {
                    ViewBag.ErrorMessage = "Invalid file type, please confrim the file type.";
                }
                AuditLogService.logDebug("View WNEdit");
                W_WL wL = null;
                
                if (uuid == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                if (file_UUID != null)
                {
                    var wl_del_file = db.W_WL_FILE.Where(f => f.UUID == file_UUID).FirstOrDefault();
                    wl_del_file.STATUS_DESCRIPTION = "File_Removed";
                    db.SaveChanges();

                }

                var query = from wl_file in db.W_WL_FILE
                            where wl_file.WL_UUID == uuid && wl_file.STATUS_DESCRIPTION == "Active"
                            select wl_file;

                ViewBag.WL_File = query;

                var SelectedMWItemsQuery = from wl_mw_item in db.W_WL_MW_ITEM
                                           where wl_mw_item.WL_UUID == uuid
                                           select wl_mw_item;

                ViewBag.SelectedMWItemsQuery = SelectedMWItemsQuery;

                var MWItems = from st in db.W_S_SYSTEM_TYPE
                              join sv in db.W_S_SYSTEM_VALUE on st.UUID equals sv.S_SYSTEM_TYPE_ID
                              where st.TYPE == "Type_Of_MW_Items"

                              select sv;
                ViewBag.MWItems = MWItems;

                var SelectedOffenseQuery = from wl_type_of_offense in db.W_WL_TYPE_OF_OFFENSE
                                           where wl_type_of_offense.WL_UUID == uuid
                                           select wl_type_of_offense;

                ViewBag.SelectedOffense = SelectedOffenseQuery;


                var OffenseType = from st in db.W_S_SYSTEM_TYPE
                                  join sv in db.W_S_SYSTEM_VALUE on st.UUID equals sv.S_SYSTEM_TYPE_ID
                                  where st.TYPE == "Type_Of_Offense"

                                  select sv;
                ViewBag.OffenseType = OffenseType;

                var IrrTechType = from st in db.W_S_SYSTEM_TYPE
                                  join sv in db.W_S_SYSTEM_VALUE on st.UUID equals sv.S_SYSTEM_TYPE_ID
                                  where st.TYPE == "Technical"
                                  orderby sv.DESCRIPTION_ENG
                                  select sv;
                //pass offense type by viewbag
                ViewBag.IrrTechType = IrrTechType;


                var IrrProType = from st in db.W_S_SYSTEM_TYPE
                                 join sv in db.W_S_SYSTEM_VALUE on st.UUID equals sv.S_SYSTEM_TYPE_ID
                                 where st.TYPE == "Procedural"
                                 orderby sv.DESCRIPTION_ENG
                                 select sv;
                //pass offense type by viewbag
                ViewBag.IrrProType = IrrProType;


                var IrrMisType = from st in db.W_S_SYSTEM_TYPE
                                 join sv in db.W_S_SYSTEM_VALUE on st.UUID equals sv.S_SYSTEM_TYPE_ID
                                 where st.TYPE == "Miscellaneous"
                                 orderby sv.DESCRIPTION_ENG
                                 select sv;
                //pass offense type by viewbag
                ViewBag.IrrMisType = IrrMisType;


                wL = db.W_WL.Find(uuid);

                if (wL == null)
                {
                    return HttpNotFound();
                }

             
                ///if (wL.EXPIRY_DATE != null)
                ///    ViewBag.ExpiryDate = cf.DateTimeToString(wL.EXPIRY_DATE.Value);

                if(wL.LETTER_ISSUE_DATE !=null)
                ViewBag.IssuedDate = cf.DateTimeToString(wL.LETTER_ISSUE_DATE.Value);


                ViewBag.CreatedDate = cf.DateTimeToString(wL.CREATED_DATE.Value);


                ViewBag.CreatedDate = cf.DateTimeToString(wL.CREATED_DATE.Value);

      
                #region AS Name Handling
                using (EntitiesMWProcessing entitiesMWProcessing = new EntitiesMWProcessing())
                {
 
                    var ASquery = entitiesMWProcessing.V_CRM_INFO.Where(s => s.CERTIFICATION_NO == wL.REGISTRATION_NO).Distinct().Select(x => new {x.UUID, x.AS_SURNAME, x.AS_GIVEN_NAME, x.AS_CHINESE_NAME, x.STATUS ,x.COMP_STATUS }).Distinct();

                    if (ASquery.Any())
                    {
                        ViewBag.ComCurrentStatus = ASquery.FirstOrDefault().COMP_STATUS;
                    }
                    


                    List<string> AS_ENG_NAME_List = new List<string>();
                    foreach (var item in ASquery)
                    {
                        AS_ENG_NAME_List.Add(item.AS_SURNAME + " " + item.AS_GIVEN_NAME + "," + item.AS_CHINESE_NAME);

                        if (wL.AS_UUID == item.UUID)
                        {
                            ViewBag.ASCurrentStatus = item.STATUS;

                        }
                        //if (wL.AUTHORIZED_SIGNATORY_NAME_CHI == item.AS_CHINESE_NAME)
                        //{
                        //    ViewBag.ASCurrentStatus = item.STATUS;
                        //}
                    }

                    ViewBag.AS_ENG_NAME_List = AS_ENG_NAME_List;
                    wL.AUTHORIZED_SIGNATORY_NAME_ENG = wL.AUTHORIZED_SIGNATORY_NAME_ENG + "," + wL.AUTHORIZED_SIGNATORY_NAME_CHI;

                   

                    #endregion
                    //endregion 

                    AuditLogService.logDebug("View WNEdit End");
                    return View("~/Areas/WarningLetter/Views/WN/WNEditDetails.cshtml",wL);
                }
            }
            catch (Exception ex)
            {
                AuditLogService.logError(ex.Message + " *** " + ex.InnerException.Message);
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
        }



        //[HttpPost]
        //[ValidateAntiForgeryToken]

        // public ActionResult WNEdit(ModelWN wN, [Bind(Include = "UUID,SUBJECT,CATEGORY,REGISTRATION_NO,MW_SUBMISSION_NO,MW_ITEMS,COMP_CONTRACTOR_NAME_ENG,COMP_CONTRACTOR_NAME_CHI,CREATION_DATE,SECTION_UNIT,FILE_REF_FOUR,FILE_REF_TWO,WL_ISSUED_BY,POST,CASE_OFFICER,RELATED_TO,SOURCE,LETTER_ISSUE_DATE,LETTER_FILE_PATH,AUTHORIZED_SIGNATURE,STATUS,REMARK,CREATED_DATE,CREATED_BY,MODIFIED_DATE,MODIFIED_BY,OTHER_REFERENCE_NO,AUTHORIZED_SIGNATORY_NAME_CHI,AUTHORIZED_SIGNATORY_NAME_ENG")] WL wL)


        public ActionResult WNEdit(ModelWN wN, [Bind(Exclude = "")] W_WL wL)
        {

            try
            {
                AuditLogService.logDebug("Edit Warning Letter id:" + wL.UUID);
                AuditLogService.logDebug("Subject :" + wL.SUBJECT);
                //     using (db) {
                if (ModelState.IsValid)
                {
                
                    byte[] fileData = null;
       
                    for (int i = 0; i < Request.Files.Count; i++)
                    {

                        var file = Request.Files[i];

                        if (file != null && file.ContentLength > 0)
                        {
                            AuditLogService.logDebug("File Name :" + file.FileName);
                            var fileName = Path.GetFileName(file.FileName);
                            string tempPath = ApplicationConstant.WLFilePath;
                            //tempPath += RegistrationConstant.WARNINGLETTER_PATH;
                            tempPath += getUploadFolderPath(wN.REGISTRATION_NO);
                            var fileExt = Path.GetExtension(file.FileName).Substring(1);
                            string tempFilePathName = System.Guid.NewGuid().ToString().Replace("-", "") + "." + fileExt;

                            if (!Directory.Exists(tempPath))
                            {
                                AuditLogService.logDebug("Create Path: " +tempPath);
                                Directory.CreateDirectory(tempPath);
                            }
                            file.SaveAs(Path.Combine(tempPath, Path.GetFileName(tempFilePathName)));
                            if (fileExt != "pdf")
                            {
                           

                                return RedirectToAction("WNEditDetails","WN",new { uuid = wL.UUID ,errorMsg = "Invalid file type, please confrim the file type." });
                            }
                            file.SaveAs(Path.Combine(tempPath, Path.GetFileName(fileName)));
                            AuditLogService.logDebug("file Saved");
                            //using (var binaryReader = new BinaryReader(Request.Files[i].InputStream))
                            //{
                            //    fileData = binaryReader.ReadBytes(Request.Files[i].ContentLength);
                            //}
                            W_WL_FILE wl_file = new W_WL_FILE();

                            wl_file.UUID = System.Guid.NewGuid().ToString();
                          //  wl_file.LETTER_FILE = fileData;
                            wl_file.WL_UUID = wL.UUID;
                            wl_file.CREATED_BY = SessionUtil.LoginPost.CODE;
                            wl_file.CREATED_DATE = System.DateTime.Now;
                            wl_file.MODIFIED_BY = SessionUtil.LoginPost.CODE;
                            wl_file.MODIFIED_DATE = System.DateTime.Now;
                            wl_file.STATUS_DESCRIPTION = "Active";
                            wl_file.FILE_NAME = fileName;
                            wl_file.FILE_PATH = getUploadFolderPath(wN.REGISTRATION_NO)+ tempFilePathName;
                            db.W_WL_FILE.Add(wl_file);
     
                        }
                    }


                    var ToDeleteOffenseQuery = db.W_WL_TYPE_OF_OFFENSE.Where(p => p.WL_UUID == wL.UUID);

                    foreach (var item in ToDeleteOffenseQuery)
                    {
                        db.W_WL_TYPE_OF_OFFENSE.Remove(item);
                    }
                    if (wN.v_IssuedDate == "" || wN.v_IssuedDate == null)
                    { }
                    else
                    {
                        wL.LETTER_ISSUE_DATE = cf.StringToDateTime(wN.v_IssuedDate);
                    }
                    if (wN.v_Offense_Type_CheckBox != null)
                    {
                        foreach (var item in wN.v_Offense_Type_CheckBox)
                        {
                            W_WL_TYPE_OF_OFFENSE wl_Type_of_offense = new W_WL_TYPE_OF_OFFENSE();
                            wl_Type_of_offense.UUID = System.Guid.NewGuid().ToString();
                            wl_Type_of_offense.WL_UUID = wL.UUID;
                            wl_Type_of_offense.CREATED_BY = SessionUtil.LoginPost.CODE;
                            wl_Type_of_offense.CREATED_DATE = System.DateTime.Now;
                            wl_Type_of_offense.MODIFIED_BY = SessionUtil.LoginPost.CODE;
                            wl_Type_of_offense.MODIFIED_DATE = System.DateTime.Now;

                            wl_Type_of_offense.WL_TYPE_OF_OFFENSE_ENG = item.ToString();
                            var q = db.W_S_SYSTEM_VALUE.Where(x => x.DESCRIPTION_ENG == item.ToString()).FirstOrDefault();

                            W_S_OFFENSE_SCORE score = db.W_S_OFFENSE_SCORE.Where(x => x.EFFECTIVE_DT <= wL.LETTER_ISSUE_DATE && x.OFFENSE_ID == q.UUID)
                                                    .OrderByDescending(x => x.EFFECTIVE_DT).FirstOrDefault();
                            if (score != null)
                            {
                                wl_Type_of_offense.SCORE = score.SCORE;
                                wl_Type_of_offense.OFFENSE_ID = score.OFFENSE_ID;
                            }

                            //W_WL_TYPE_OF_OFFENSE TEMP = db.W_S_SYSTEM_VALUE.Where(x => x. == item.ToString()).FirstOrDefault();
                            //wl_Type_of_offense.SCORE = TEMP.;
                            //wl_Type_of_offense.SCORE = TEMP.UUID;
                            db.W_WL_TYPE_OF_OFFENSE.Add(wl_Type_of_offense);
                        }

                    }

                    var ToDeleteMWItemsQuery = db.W_WL_MW_ITEM.Where(p => p.WL_UUID == wL.UUID);
                  
                    foreach (var item in ToDeleteMWItemsQuery)
                    {
                        db.W_WL_MW_ITEM.Remove(item);
                    }
                    if (wN.v_MWItems_Type_CheckBox != null)
                    {
                        foreach (var item in wN.v_MWItems_Type_CheckBox)
                        {

                            W_WL_MW_ITEM wl_mw_item = new W_WL_MW_ITEM();

                            wl_mw_item.UUID = System.Guid.NewGuid().ToString();
                            wl_mw_item.WL_UUID = wL.UUID;
                            wl_mw_item.CREATED_BY = SessionUtil.LoginPost.CODE;
                            wl_mw_item.CREATED_DATE = System.DateTime.Now;
                            wl_mw_item.MODIFIED_BY = SessionUtil.LoginPost.CODE;
                            wl_mw_item.MODIFIED_DATE = System.DateTime.Now;

                            wl_mw_item.WL_MW_ITEM_ENG = item.ToString();


                            db.W_WL_MW_ITEM.Add(wl_mw_item);

                        }
                    }


                    #region AS Name Handling

                    //if (!string.IsNullOrWhiteSpace(wL.AUTHORIZED_SIGNATORY_NAME_ENG))
                    //{
                    //    var SplitASEng = wL.AUTHORIZED_SIGNATORY_NAME_ENG.Split(',');
                    //    wL.AUTHORIZED_SIGNATORY_NAME_ENG = SplitASEng[0];
                    //    wL.AUTHORIZED_SIGNATORY_NAME_CHI = SplitASEng[1];

                    //}
                    #endregion
                    wL.AS_UUID = wL.AUTHORIZED_SIGNATORY_NAME_ENG;
                    //wL.EXPIRY_DATE = DateTime.ParseExact(v_ExpiryDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    db.Entry(wL).State = EntityState.Modified;


                    AuditLogService.logDebug("to db save");
                    db.SaveChanges();
                    AuditLogService.logDebug("Warning letter finished");
                }
                      //   }
            }
           catch (DbEntityValidationException dbex)
           {
               AuditLogService.logError(dbex.Message + " *** " + dbex.InnerException.Message);
               return new HttpStatusCodeResult(HttpStatusCode.NotFound);
           }
           catch (Exception ex)
           {
              AuditLogService.logError(ex.Message + " *** " + ex.InnerException.Message);
              return new HttpStatusCodeResult(HttpStatusCode.NotFound);
          }
            return RedirectToAction("WN");
            //return View("WN");
            //ViewBag.Message = m_ModelWN;

        }
        public ActionResult WNCreate()
        {
            //     using (db)
            //     {

            var OffenseType = from st in db.W_S_SYSTEM_TYPE
                              join sv in db.W_S_SYSTEM_VALUE on st.UUID equals sv.S_SYSTEM_TYPE_ID
                              where st.TYPE == "Type_Of_Offense"
                              select sv;
            ViewBag.OffenseType = OffenseType.ToList();

            var IrrTechType = from st in db.W_S_SYSTEM_TYPE
                              join sv in db.W_S_SYSTEM_VALUE on st.UUID equals sv.S_SYSTEM_TYPE_ID
                              where st.TYPE == "Technical"
                              orderby sv.DESCRIPTION_ENG
                              select sv;
            //pass offense type by viewbag
            ViewBag.IrrTechType = IrrTechType;


            var IrrProType = from st in db.W_S_SYSTEM_TYPE
                             join sv in db.W_S_SYSTEM_VALUE on st.UUID equals sv.S_SYSTEM_TYPE_ID
                             where st.TYPE == "Procedural"
                             orderby sv.DESCRIPTION_ENG
                             select sv;
            //pass offense type by viewbag
            ViewBag.IrrProType = IrrProType;


            var IrrMisType = from st in db.W_S_SYSTEM_TYPE
                             join sv in db.W_S_SYSTEM_VALUE on st.UUID equals sv.S_SYSTEM_TYPE_ID
                             where st.TYPE == "Miscellaneous"
                             orderby sv.DESCRIPTION_ENG
                             select sv;
            //pass offense type by viewbag
            ViewBag.IrrMisType = IrrMisType;

            var MWItems = from st in db.W_S_SYSTEM_TYPE
                          join sv in db.W_S_SYSTEM_VALUE on st.UUID equals sv.S_SYSTEM_TYPE_ID
                          where st.TYPE == "Type_Of_MW_Items"

                          select sv;

            using (EntitiesAuth dbr = new EntitiesAuth())
            {
                ViewBag.GetSectionUnit = dbr.SYS_SECTION.ToList();
            }

            ViewBag.MWItems = MWItems;
            ViewBag.CaseOfficer = SessionUtil.LoginPost.BD_PORTAL_LOGIN;
            ViewBag.Test = "123";
            return View();
        }
        public ActionResult GET_AS_STATUS(string RegNo,int idx)
        {
            using (EntitiesMWProcessing entitiesMWProcessing = new EntitiesMWProcessing())
            {
              
                var query = entitiesMWProcessing.V_CRM_INFO.Where(s => s.CERTIFICATION_NO.Equals(RegNo) && s.AS_SURNAME !=null).Distinct().ToList();                     
                var AS = query.Select(x => new { x.CHINESE_NAME, x.SURNAME, x.GIVEN_NAME, x.AS_SURNAME, x.AS_GIVEN_NAME, x.AS_CHINESE_NAME, x.STATUS,x.COMP_STATUS }).Distinct().ToList();

                if (query.Count == 0)
                {
                    return Json("", JsonRequestBehavior.AllowGet);
                }

                var result = new
                {
                    Status = AS[idx].STATUS
                };
     
                return Json(result, JsonRequestBehavior.AllowGet);
                
     
            }
        }
        [HttpPost]

        public ActionResult GET_CON_IND_NAME(string RegNo)
        {
            using (EntitiesMWProcessing entitiesMWProcessing = new EntitiesMWProcessing())
            {
                AuditLogService.logDebug("Function Start");
                var query = entitiesMWProcessing.V_CRM_INFO.Where(s => s.CERTIFICATION_NO.Equals(RegNo)).Distinct().ToList();
                if (query.Count == 0)
                {
                    return Json(new
                    {
                        //pass Chi, eng, status and as to view
                        ChineseName = "Invalid certification no.",
                        EnglishName = "Invalid",
                        Status = "",
                        CompStatus = "",


                    }, JsonRequestBehavior.AllowGet);
                }
                AuditLogService.logDebug("1");
                try
                {
                    AuditLogService.logDebug("1.1");
                    if (query.Where(x => x.AS_SURNAME != null).Count() > 0)
                    {
                        var AS = query.Select(x => new { x.UUID, x.CHINESE_NAME, x.SURNAME, x.GIVEN_NAME, x.AS_SURNAME, x.AS_GIVEN_NAME, x.AS_CHINESE_NAME, x.STATUS, x.COMP_STATUS }).Distinct();
                        AuditLogService.logDebug("2");
                        List<string> AS_NAME_List = new List<string>();
                        List<string> AS_UUID_LIST = new List<string>();
                        // List<string> AS_STATUS_LIST = new List<string>();
                        foreach (var item in AS)
                        {
                            // User require merge the text box 
                            if (item.UUID != null)
                            {
                                AS_UUID_LIST.Add(item.UUID);
                                // AS_STATUS_LIST.Add(item.STATUS);
                            }

                        }
                        foreach (var item in AS)
                        {
                            // User require merge the text box 
                            if (item.AS_SURNAME != null)
                            {
                                AS_NAME_List.Add(item.AS_SURNAME + " " + item.AS_GIVEN_NAME + "," + item.AS_CHINESE_NAME);
                                // AS_STATUS_LIST.Add(item.STATUS);
                            }

                        }
                        AuditLogService.logDebug("3");
                        // ViewBag.AS_STATUS_LIST = AS_STATUS_LIST;
                        var result = new
                        {
                            ChineseName = "",
                            EnglishName = "",
                            Status = "",
                            CompStatus = "",
                            AS_UUID_LIST,
                            AS_NAME_List

                        };
                        //if (query.Any())
                        if (AS.Any())
                        {
                            AuditLogService.logDebug("3.1");
                            result = new
                            {
                                //pass Chi, eng, status and as to view
                                ChineseName = query.First().CHINESE_NAME,
                                EnglishName = query.First().SURNAME + " " + query.First().GIVEN_NAME,
                                Status = query.First().STATUS,
                                CompStatus = query.FirstOrDefault().COMP_STATUS,
                                AS_UUID_LIST,
                                AS_NAME_List

                            };

                        }
                        else
                        {
                            result = new
                            {
                                //pass Chi, eng, status and as to view
                                ChineseName = "Invalid certification no.",
                                EnglishName = "Invalid",
                                Status = "",
                                CompStatus = "",
                                AS_UUID_LIST,
                                AS_NAME_List

                            };
                        }
                        AuditLogService.logDebug("4");
                        // var AS = query.Select(x => new { x.AS_SURNAME, x.AS_GIVEN_NAME });

                        //foreach (var item in AS)
                        //{ }
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var Professional = query.Select(x => new { x.UUID, x.CHINESE_NAME, x.SURNAME, x.GIVEN_NAME, x.STATUS}).Distinct();

                        return Json(new
                        {
                            //pass Chi, eng, status and as to view
                            ChineseName = Professional.FirstOrDefault().CHINESE_NAME,
                            EnglishName = Professional.FirstOrDefault().SURNAME+" "+Professional.FirstOrDefault().GIVEN_NAME,
                            CompStatus = Professional.FirstOrDefault().STATUS,
                            //CompStatus = "",
                        
                        }, JsonRequestBehavior.AllowGet);
                    }



                }
                catch (Exception ex)
                {
                    AuditLogService.logError(ex.Message);
                }
                AuditLogService.logDebug("Function End");


                return Json("", JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult DownloadFile(string uuid ,string path,string pathName)
        {
            WarningLetterSearchService wns = new WarningLetterSearchService();
            var file = wns.DownloadFile(uuid, path, pathName);
            if (file == null)
            {
                return Content("File not found.");
            }
            else
            {
                return file;
            }
        }

        public ActionResult WNSubmitValidation(ModelWN wN, [Bind(Exclude = "")] W_WL wL)
        {

            if (ValidateResult.Result.Equals(ServiceResult.RESULT_FAILURE)) return Json(ValidateResult);
            else
            {
                for (int i = 0; i < Request.Files.Count; i++)
                {

                    var file = Request.Files[i];



                    if (file != null && file.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var fileExt = Path.GetExtension(file.FileName).Substring(1);
                        if (fileExt != "pdf")
                        {
                            return Json(new ServiceResult() { Result = ServiceResult.RESULT_FAILURE ,Message =new List<string>() { "The upload document type is not valid!" } });
                        }
                    }
                }

                        return Json(new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS });
                //return Json(new  {  ServiceResult.RESULT_SUCCESS });
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult WNCreate(ModelWN wN, [Bind(Exclude = "")] W_WL wL)
        //public ActionResult WNCreate(ModelWN wN, [Bind(Include = "UUID,SUBJECT,CATEGORY,REGISTRATION_NO,MW_SUBMISSION_NO,MW_ITEMS,COMP_CONTRACTOR_NAME_ENG,COMP_CONTRACTOR_NAME_CHI,CREATION_DATE,SECTION_UNIT,AUTHOR,FILE_REF_FOUR,FILE_REF_TWO,WL_ISSUED_BY,POST,CASE_OFFICER,RELATED_TO,SOURCE,LETTER_ISSUE_DATE,LETTER_FILE_PATH,AUTHORIZED_SIGNATURE,STATUS,REMARK,CREATED_DATE,CREATED_BY,MODIFIED_DATE,MODIFIED_BY,OTHER_REFERENCE_NO,AUTHORIZED_SIGNATORY_NAME_ENG,AUTHORIZED_SIGNATORY_NAME_CHI,WL_FILE")] WL wL)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    //    using (db)
                    //  {
                    AuditLogService.logDebug("Create Warning Letter");
                    wL.UUID = System.Guid.NewGuid().ToString();
                    AuditLogService.logDebug("UUID : " + wL.UUID);
                    byte[] fileData = null;

                    //if (Request.Files.Count > 0)
                    wN.v_Temp_file = new List<Tuple<byte[], string>>();
                    wN.M_WL_FILE = new List<W_WL_FILE>();
                    for (int i = 0; i < Request.Files.Count; i++)
                    {

                        var file = Request.Files[i];



                        if (file != null && file.ContentLength > 0)
                        {
                            var fileName = Path.GetFileName(file.FileName);
                            var fileExt = Path.GetExtension(file.FileName).Substring(1);
                            if (fileExt != "pdf")
                            {
                                var OffenseType = from st in db.W_S_SYSTEM_TYPE
                                                  join sv in db.W_S_SYSTEM_VALUE on st.UUID equals sv.S_SYSTEM_TYPE_ID
                                                  where st.TYPE == "Type_Of_Offense"
                                                  select sv;
                                ViewBag.OffenseType = OffenseType.ToList();


                                var MWItems = from st in db.W_S_SYSTEM_TYPE
                                              join sv in db.W_S_SYSTEM_VALUE on st.UUID equals sv.S_SYSTEM_TYPE_ID
                                              where st.TYPE == "Type_Of_MW_Items"

                                              select sv;
                                ViewBag.MWItems = MWItems;
                                ViewBag.ErrorMessage = "Invalid file type, please confrim the file type.";
                                return View(wL);
                            }
                            string tempPath = ApplicationConstant.WLFilePath;
                            tempPath += getUploadFolderPath(wN.REGISTRATION_NO);
                            DirectoryInfo di = Directory.CreateDirectory(tempPath);
                            string tempFilePathName = System.Guid.NewGuid().ToString().Replace("-","") + "." + fileExt;
                            file.SaveAs(Path.Combine(tempPath, Path.GetFileName(tempFilePathName)));


                            //using (var binaryReader = new BinaryReader(Request.Files[i].InputStream))
                            //{
                            //    fileData = binaryReader.ReadBytes(Request.Files[i].ContentLength);
                                
                            //    // wN.v_Temp_file.Add(new Tuple<byte[], string>(fileData, fileName));
                            //}
                            W_WL_FILE wl_file = new W_WL_FILE();

                            wl_file.UUID = System.Guid.NewGuid().ToString();
                            //wl_file.LETTER_FILE = fileData;
                            wl_file.FILE_PATH = getUploadFolderPath(wN.REGISTRATION_NO) + tempFilePathName;
                            wl_file.WL_UUID = wL.UUID;
                            wl_file.CREATED_BY = SessionUtil.LoginPost.CODE;
                            wl_file.CREATED_DATE = System.DateTime.Now;
                            wl_file.MODIFIED_BY = SessionUtil.LoginPost.CODE;
                            wl_file.MODIFIED_DATE = System.DateTime.Now;
                            wl_file.STATUS_DESCRIPTION = "Active";
                            wl_file.FILE_NAME = fileName;

                            wN.M_WL_FILE.Add(wl_file);
                            wL.W_WL_FILE.Add(wl_file);
                  
                            ////   db.WL_FILE.Add(wl_file);
                            //var path = Path.Combine(Server.MapPath("~/Images/"), fileName);
                            // file.SaveAs(path);
                        }
                    }
                    if (wN.v_IssuedDate == "" || wN.v_IssuedDate == null)
                    { }
                    else
                    {
                        wL.LETTER_ISSUE_DATE = cf.StringToDateTime(wN.v_IssuedDate);
                    }

                    wN.M_WL_TYPE_OF_OFFENSE = new List<W_WL_TYPE_OF_OFFENSE>();
                    if (wN.v_Offense_Type_CheckBox != null)
                    {
                        foreach (var item in wN.v_Offense_Type_CheckBox)
                        {
                            W_WL_TYPE_OF_OFFENSE wl_Type_of_offense = new W_WL_TYPE_OF_OFFENSE();
                            wl_Type_of_offense.UUID = System.Guid.NewGuid().ToString();
                            wl_Type_of_offense.WL_UUID = wL.UUID;
                            wl_Type_of_offense.CREATED_BY = SessionUtil.LoginPost.CODE;
                            wl_Type_of_offense.CREATED_DATE = System.DateTime.Now;
                            wl_Type_of_offense.MODIFIED_BY = SessionUtil.LoginPost.CODE;
                            wl_Type_of_offense.MODIFIED_DATE = System.DateTime.Now;

                            wl_Type_of_offense.WL_TYPE_OF_OFFENSE_ENG = item.ToString();
                            var q = db.W_S_SYSTEM_VALUE.Where(x => x.DESCRIPTION_ENG == item.ToString()).FirstOrDefault();

                            W_S_OFFENSE_SCORE score = db.W_S_OFFENSE_SCORE.Where(x => x.EFFECTIVE_DT <= wL.LETTER_ISSUE_DATE && x.OFFENSE_ID==q.UUID)
                                                    .OrderByDescending(x => x.EFFECTIVE_DT).FirstOrDefault();
                            if (score != null)
                            {
                                wl_Type_of_offense.SCORE = score.SCORE;
                                wl_Type_of_offense.OFFENSE_ID = score.OFFENSE_ID;
                            }


                            wN.M_WL_TYPE_OF_OFFENSE.Add(wl_Type_of_offense);
                            wL.W_WL_TYPE_OF_OFFENSE.Add(wl_Type_of_offense);
                            ////  db.WL_TYPE_OF_OFFENSE.Add(wl_Type_of_offense);
                        }
                    }

                    wN.M_WL_MW_ITEM = new List<W_WL_MW_ITEM>();
                    if (wN.v_MWItems_Type_CheckBox != null)
                    {
                        foreach (var item in wN.v_MWItems_Type_CheckBox)
                        {

                            W_WL_MW_ITEM wl_mw_item = new W_WL_MW_ITEM();

                            wl_mw_item.UUID = System.Guid.NewGuid().ToString();
                            wl_mw_item.WL_UUID = wL.UUID;
                            wl_mw_item.CREATED_BY = SessionUtil.LoginPost.CODE;
                            wl_mw_item.CREATED_DATE = System.DateTime.Now;
                            wl_mw_item.MODIFIED_BY = SessionUtil.LoginPost.CODE;
                            wl_mw_item.MODIFIED_DATE = System.DateTime.Now;

                            wl_mw_item.WL_MW_ITEM_ENG = item.ToString();

                            ////wN.M_WL_MW_ITEM.Add(wl_mw_item);
                            wL.W_WL_MW_ITEM.Add(wl_mw_item);
                            //// db.WL_MW_ITEM.Add(wl_mw_item);

                        }
                    }



                    #region AS Name Handleing
                    //split the AS text box 
                    //if (wL.AUTHORIZED_SIGNATORY_NAME_ENG != null)
                    //{
                    //    var SplitASEng = wL.AUTHORIZED_SIGNATORY_NAME_ENG.Split(',');
                    //    wL.AUTHORIZED_SIGNATORY_NAME_ENG = SplitASEng[0];
                    //    wL.AUTHORIZED_SIGNATORY_NAME_CHI = SplitASEng[1];
                    //}
                    #endregion
                  //  wL.POST =SessionUtil.LoginPost.CODE;
                    wL.CASE_OFFICER = SessionUtil.LoginPost.BD_PORTAL_LOGIN;

                    wL.CREATED_DATE = System.DateTime.Now;
                    wL.CREATED_BY = SessionUtil.LoginPost.CODE;
                    wL.MODIFIED_BY = SessionUtil.LoginPost.CODE;
                    wL.MODIFIED_DATE = System.DateTime.Now;

                    /// wN.M_WL = wL;
                    db.W_WL.Add(wL);

                    db.SaveChanges();
                    AuditLogService.logDebug("Create WL Finished");
                    //    }
                }
                catch (DbEntityValidationException dbex)
                {
                    AuditLogService.logError(dbex.Message + " *** " + dbex.InnerException.Message);
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);

                }
                catch (Exception ex)
                {
                    AuditLogService.logError(ex.Message + " *** " + ex.InnerException.Message);
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                }
                return RedirectToAction("WN");
                //return View("WN");
                //return RedirectToAction("WNCreateConfirm",wL);
            }
            else
            {
                var OffenseType = from st in db.W_S_SYSTEM_TYPE
                                  join sv in db.W_S_SYSTEM_VALUE on st.UUID equals sv.S_SYSTEM_TYPE_ID
                                  where st.TYPE == "Type_Of_Offense"
                                  select sv;
                ViewBag.OffenseType = OffenseType.ToList();


                var MWItems = from st in db.W_S_SYSTEM_TYPE
                              join sv in db.W_S_SYSTEM_VALUE on st.UUID equals sv.S_SYSTEM_TYPE_ID
                              where st.TYPE == "Type_Of_MW_Items"

                              select sv;
                ViewBag.MWItems = MWItems;
                ViewBag.CaseOfficer = SessionUtil.LoginPost.CODE;
            }
            return View(wL);
        }


        public ActionResult WNDelete(string id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                var query = db.W_WL.Find(id);

                db.W_WL_FILE.RemoveRange(db.W_WL_FILE.Where(x => x.WL_UUID == id));
                db.W_WL_MW_ITEM.RemoveRange(db.W_WL_MW_ITEM.Where(x => x.WL_UUID == id));
                db.W_WL_TYPE_OF_OFFENSE.RemoveRange(db.W_WL_TYPE_OF_OFFENSE.Where(x => x.WL_UUID == id));
                db.W_WL.Remove(query);
                //query.STATUS = "Deleted";
                db.SaveChanges();
                AuditLogService.logDebug("Warning Letter ID:" + id + " Deleted");
                return RedirectToAction("WN");

            }
            catch (Exception ex)
            {
                AuditLogService.logError(ex.Message + " *** " + ex.InnerException.Message);
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
        }
        protected override void Dispose(bool disposing)
        {
            try {
                if (disposing)
                {
                    db.Dispose();
                }
                base.Dispose(disposing);
            }
            catch (Exception ex)
            { }
         
        }

        public string getUploadFolderPath(string fileReferenceNo)
        {
            string fileSeparator = Char.ToString(ApplicationConstant.FileSeparator);
            string path = "docs" + fileSeparator;
            string subPath = fileReferenceNo.Replace("(", "_").Replace(")", "_").Replace(" ", "__").Replace("/", "_")
                + fileSeparator;
            path += subPath;
           
            return path;
            
        }

 
    }
}

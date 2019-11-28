using MWMS2.Areas.WarningLetter.Models;
using MWMS2.Controllers;
using MWMS2.Entity;
using MWMS2.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.WarningLetter.Controllers
{
    public class WLController : Controller
    {
        // GET: WarningLetter/WL
        public ActionResult Index(WLModel model)
        {
            using (EntitiesWarningLetter db = new EntitiesWarningLetter())
            {
                var IrrTechType = (from st in db.W_S_SYSTEM_TYPE
                                   join sv in db.W_S_SYSTEM_VALUE on st.UUID equals sv.S_SYSTEM_TYPE_ID
                                   where st.TYPE == "Technical"
                                   orderby sv.DESCRIPTION_ENG
                                   select sv).ToList();

                model.IrrTechType = IrrTechType;


                var IrrProType = (from st in db.W_S_SYSTEM_TYPE
                                  join sv in db.W_S_SYSTEM_VALUE on st.UUID equals sv.S_SYSTEM_TYPE_ID
                                  where st.TYPE == "Procedural"
                                  orderby sv.DESCRIPTION_ENG
                                  select sv).ToList();

                model.IrrProType = IrrProType;


                var IrrMisType = (from st in db.W_S_SYSTEM_TYPE
                                  join sv in db.W_S_SYSTEM_VALUE on st.UUID equals sv.S_SYSTEM_TYPE_ID
                                  where st.TYPE == "Miscellaneous"
                                  orderby sv.DESCRIPTION_ENG
                                  select sv).ToList();

                model.IrrMisType = IrrMisType;
            }
              
            return View(model);
        }
        [HttpPost]
        public ActionResult WLSearch(WLModel wl)
        {
            WarningLetterSearchService ss = new WarningLetterSearchService();
            return Content(JsonConvert.SerializeObject(ss.SearchWarningLetter(wl), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");

        }

        public ActionResult Form(string uuid, string file_UUID, string errorMsg)
        {
            try
            {
                using (EntitiesWarningLetter db = new EntitiesWarningLetter())
                {
                    WLDisplayModel model = new WLDisplayModel();
                    if (errorMsg != null)
                    {
                        model.ErrorMessage = "Invalid file type, please confrim the file type.";
                    }
                    AuditLogService.logDebug("View WNEdit");


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

                    model.W_WL_FILE_LIST = (from wl_file in db.W_WL_FILE
                                where wl_file.WL_UUID == uuid && wl_file.STATUS_DESCRIPTION == "Active"
                                select wl_file).ToList();

                    var SelectedOffenseQuery = (from wl_type_of_offense in db.W_WL_TYPE_OF_OFFENSE
                                               where wl_type_of_offense.WL_UUID == uuid
                                               select wl_type_of_offense).ToList();

                    model.SelectedOffense = SelectedOffenseQuery;


                    var OffenseType = (from st in db.W_S_SYSTEM_TYPE
                                      join sv in db.W_S_SYSTEM_VALUE on st.UUID equals sv.S_SYSTEM_TYPE_ID
                                      where st.TYPE == "Type_Of_Offense"

                                      select sv).ToList();
                    model.OffenseType = OffenseType;

                    var IrrTechType =( from st in db.W_S_SYSTEM_TYPE
                                      join sv in db.W_S_SYSTEM_VALUE on st.UUID equals sv.S_SYSTEM_TYPE_ID
                                      where st.TYPE == "Technical"
                                      orderby sv.DESCRIPTION_ENG
                                      select sv).ToList();
                
                    model.IrrTechType = IrrTechType;


                    var IrrProType = (from st in db.W_S_SYSTEM_TYPE
                                     join sv in db.W_S_SYSTEM_VALUE on st.UUID equals sv.S_SYSTEM_TYPE_ID
                                     where st.TYPE == "Procedural"
                                     orderby sv.DESCRIPTION_ENG
                                     select sv).ToList();
              
                    model.IrrProType = IrrProType;

            
                    var IrrMisType = (from st in db.W_S_SYSTEM_TYPE
                                     join sv in db.W_S_SYSTEM_VALUE on st.UUID equals sv.S_SYSTEM_TYPE_ID
                                     where st.TYPE == "Miscellaneous"
                                     orderby sv.DESCRIPTION_ENG
                                     select sv).ToList();
            
                    model.IrrMisType = IrrMisType;


                    model.W_WL = db.W_WL.Find(uuid);

                    if (model.W_WL == null)
                    {
                        return HttpNotFound();
                    }


                    ///if (wL.EXPIRY_DATE != null)
                    ///    ViewBag.ExpiryDate = cf.DateTimeToString(wL.EXPIRY_DATE.Value);

                    CommonFunction cf = new CommonFunction();

                    if (model.W_WL.LETTER_ISSUE_DATE != null)
                        model.IssuedDate = cf.DateTimeToString(model.W_WL.LETTER_ISSUE_DATE.Value);


                    model.CreatedDate = cf.DateTimeToString(model.W_WL.CREATED_DATE.Value);
                                     
                    
                    #region AS Name Handling
                    using (EntitiesMWProcessing entitiesMWProcessing = new EntitiesMWProcessing())
                    {
                        var ASquery = entitiesMWProcessing.V_CRM_INFO.Where(s => s.CERTIFICATION_NO == model.W_WL.REGISTRATION_NO).Distinct().Select(x => new { x.UUID, x.AS_SURNAME, x.AS_GIVEN_NAME, x.AS_CHINESE_NAME, x.STATUS, x.COMP_STATUS }).Distinct();

                        if (ASquery.Any())
                        {
                            model.ComCurrentStatus = ASquery.FirstOrDefault().COMP_STATUS;
                        }                        
                        List<string> AS_ENG_NAME_List = new List<string>();
                        foreach (var item in ASquery)
                        {
                            AS_ENG_NAME_List.Add(item.AS_SURNAME + " " + item.AS_GIVEN_NAME + "," + item.AS_CHINESE_NAME);

                            if (model.W_WL.AS_UUID == item.UUID)
                            {
                                model.ASCurrentStatus = item.STATUS;
                            }
                        }
                        model.AS_ENG_NAME_List = AS_ENG_NAME_List;
                        model.W_WL.AUTHORIZED_SIGNATORY_NAME_ENG = model.W_WL.AUTHORIZED_SIGNATORY_NAME_ENG + "," + model.W_WL.AUTHORIZED_SIGNATORY_NAME_CHI;
                        
                        #endregion        
                        AuditLogService.logDebug("View WNEdit End");
                        return View(model);
                    }
                }
                
            }
            catch (Exception ex)
            {
                AuditLogService.logError(ex.Message + " *** " + ex.InnerException.Message);
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
        }

        public ActionResult GET_AS_STATUS(string RegNo, int idx)
        {
            using (EntitiesMWProcessing entitiesMWProcessing = new EntitiesMWProcessing())
            {

                var query = entitiesMWProcessing.V_CRM_INFO.Where(s => s.CERTIFICATION_NO.Equals(RegNo) && s.AS_SURNAME != null).Distinct().ToList();
                var AS = query.Select(x => new { x.CHINESE_NAME, x.SURNAME, x.GIVEN_NAME, x.AS_SURNAME, x.AS_GIVEN_NAME, x.AS_CHINESE_NAME, x.STATUS, x.COMP_STATUS }).Distinct().ToList();

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
         
                        foreach (var item in AS)
                        {
                            // User require merge the text box 
                            if (item.UUID != null)
                            {
                                AS_UUID_LIST.Add(item.UUID);

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

                        var result = new
                        {
                            ChineseName = "",
                            EnglishName = "",
                            Status = "",
                            CompStatus = "",
                            AS_UUID_LIST,
                            AS_NAME_List

                        };
                     
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
                        
                                ChineseName = "Invalid certification no.",
                                EnglishName = "Invalid",
                                Status = "",
                                CompStatus = "",
                                AS_UUID_LIST,
                                AS_NAME_List

                            };
                        }
                        AuditLogService.logDebug("4");
                  
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var Professional = query.Select(x => new { x.UUID, x.CHINESE_NAME, x.SURNAME, x.GIVEN_NAME, x.STATUS }).Distinct();

                        return Json(new
                        {
                            //pass Chi, eng, status and as to view
                            ChineseName = Professional.FirstOrDefault().CHINESE_NAME,
                            EnglishName = Professional.FirstOrDefault().SURNAME + " " + Professional.FirstOrDefault().GIVEN_NAME,
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
        public ActionResult DownloadFile(string uuid, string path, string pathName)
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

    }
}
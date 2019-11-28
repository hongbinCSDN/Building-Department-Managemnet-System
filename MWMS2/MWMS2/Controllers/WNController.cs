using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Models;
using MWMS2.Dao;
using System.Data.Entity;
using System.Net;
using System.IO;
using System.Globalization;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;


namespace MWMS2.Controllers
{
    public class WNController : Controller
    {

        Entities db = new Entities();
        CommonFunction cf = new CommonFunction();
        private static log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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

        public ActionResult WNSearch(ModelWN wN)
        {
            return View();

            //try
            //{

            //    Log.Debug("--------------Start Search----------------");
            //    List<ModelWN> modelWNList = new List<ModelWN>();
            //    // using (db)
            //    // {
            //    var watch = System.Diagnostics.Stopwatch.StartNew();
            //    // the code that you want to measure comes here

            //    //get offense type
            //    var OffenseType = from st in db.W_S_SYSTEM_TYPE
            //                      join sv in db.W_S_SYSTEM_VALUE on st.UUID equals sv.S_SYSTEM_TYPE_ID
            //                      where st.TYPE == "Type_Of_Offense"

            //                      select sv;

            //    //pass offense type by viewbag
            //    ViewBag.OffenseType = OffenseType;

            //    var MWItems = from st in db.W_S_SYSTEM_TYPE
            //                  join sv in db.W_S_SYSTEM_VALUE on st.UUID equals sv.S_SYSTEM_TYPE_ID
            //                  where st.TYPE == "Type_Of_MW_Items"

            //                  select sv;
            //    ViewBag.MWItems = MWItems;


            //    var query = from wl in db.W_WL

            //                join mw in db.W_WL_MW_ITEM on wl.UUID equals mw.WL_UUID into mwGroup
            //                from mwitem in mwGroup.DefaultIfEmpty()
            //                join off in db.W_WL_TYPE_OF_OFFENSE on wl.UUID equals off.WL_UUID into offenseGroup
            //                from offense in offenseGroup.DefaultIfEmpty()
            //                orderby wl.CREATED_DATE descending
            //                select new { wl, offense, mwitem };

            //    if (wN.v_Offense_Type_CheckBox != null && wN.v_Offense_Type_CheckBox.Count() != 0)
            //    {

            //        query = (from x in query
            //                 where (x.offense.WL_TYPE_OF_OFFENSE_ENG != null && wN.v_Offense_Type_CheckBox.Contains(x.offense.WL_TYPE_OF_OFFENSE_ENG)) || x.offense.WL_TYPE_OF_OFFENSE_ENG == null
            //                 select x);
            //    }

            //    if (wN.v_MWItems_Type_CheckBox != null && wN.v_MWItems_Type_CheckBox.Count() != 0)
            //    {
            //        query = (from x in query
            //                 where (x.mwitem.WL_MW_ITEM_ENG != null && wN.v_MWItems_Type_CheckBox.Contains(x.mwitem.WL_MW_ITEM_ENG)) || x.mwitem.WL_MW_ITEM_ENG == null
            //                 select x);
            //    }
            //    if (wN.v_Cat_Checkbox != null && wN.v_Cat_Checkbox.Count() != 0)
            //    {

            //        query = (from x in query
            //                 where (x.wl.CATEGORY != null && wN.v_Cat_Checkbox.Contains(x.wl.CATEGORY)) || x.wl.CATEGORY == null
            //                 select x);
            //    }
            //    if (wN.v_Section_checkbox != null && wN.v_Section_checkbox.Count() != 0)
            //    {
            //        query = (from x in query
            //                 where (x.wl.SECTION_UNIT != null && wN.v_Section_checkbox.Contains(x.wl.SECTION_UNIT)) || x.wl.SECTION_UNIT == null
            //                 select x);


            //    }
            //    if (wN.v_Related_checkbox != null && wN.v_Related_checkbox.Count() != 0)
            //    {
            //        query = (from x in query
            //                 where (x.wl.RELATED_TO != null && wN.v_Related_checkbox.Any(val => x.wl.RELATED_TO.Equals(val))) || x.wl.RELATED_TO == null
            //                 select x);

            //    }

            //    if (wN.v_Source_checkbox != null && wN.v_Source_checkbox.Count() != 0)
            //    {
            //        query = (from x in query
            //                 where (x.wl.SOURCE != null && wN.v_Source_checkbox.Contains(x.wl.SOURCE)) || x.wl.SOURCE == null
            //                 select x);


            //    }

            //    if (!String.IsNullOrEmpty(wN.SUBJECT))
            //    {
            //        query = query.Where(s => s.wl.SUBJECT.ToLower().Contains(wN.SUBJECT.ToLower()));

            //    }

            //    if (!String.IsNullOrEmpty(wN.REGISTRATION_NO))
            //    {
            //        query = query.Where(s => s.wl.REGISTRATION_NO.Contains(wN.REGISTRATION_NO));
            //    }
            //    if (!String.IsNullOrEmpty(wN.FILE_REF_FOUR))
            //    {
            //        query = query.Where(s => s.wl.FILE_REF_FOUR.Contains(wN.FILE_REF_FOUR));
            //    }
            //    if (!String.IsNullOrEmpty(wN.FILE_REF_TWO))
            //    {
            //        query = query.Where(s => s.wl.FILE_REF_TWO.Contains(wN.FILE_REF_TWO));
            //    }
            //    if (!String.IsNullOrEmpty(wN.MW_SUBMISSION_NO))
            //    {
            //        query = query.Where(s => s.wl.MW_SUBMISSION_NO != null && s.wl.MW_SUBMISSION_NO.ToLower().Contains(wN.MW_SUBMISSION_NO.ToLower()));
            //    }
            //    if (!String.IsNullOrEmpty(wN.COMP_CONTRACTOR_NAME_ENG))
            //    {
            //        if (wN.COMP_CONTRACTOR_NAME_ENG.Any(c => (uint)c >= 0x4E00 && (uint)c <= 0x2FA1F))
            //        {
            //            query = query.Where(s => (s.wl.COMP_CONTRACTOR_NAME_CHI.Contains(wN.COMP_CONTRACTOR_NAME_ENG)) || (s.wl.AUTHORIZED_SIGNATORY_NAME_CHI.Contains(wN.COMP_CONTRACTOR_NAME_ENG)));


            //        }
            //        else
            //        {
            //            query = query.Where(s => s.wl.COMP_CONTRACTOR_NAME_ENG.ToLower().Contains(wN.COMP_CONTRACTOR_NAME_ENG.ToLower()) || s.wl.AUTHORIZED_SIGNATORY_NAME_ENG.ToLower().Contains(wN.COMP_CONTRACTOR_NAME_ENG.ToLower()));

            //        }

            //    }
            //    if (!String.IsNullOrEmpty(wN.CASE_OFFICER))
            //    {
            //        query = query.Where(s => s.wl.CASE_OFFICER.Contains(wN.CASE_OFFICER));
            //    }
            //    if (!String.IsNullOrEmpty(wN.SearchString_CreateStartDate))
            //    {
            //        DateTime tempStart = DateTime.ParseExact(wN.SearchString_CreateStartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            //        query = query.Where(s => s.wl.CREATED_DATE != null &&
            //        System.Data.Entity.DbFunctions.TruncateTime(s.wl.CREATED_DATE.Value) >= tempStart);
            //    }
            //    if (!String.IsNullOrEmpty(wN.SearchString_CreateEndDate))
            //    {
            //        DateTime tempEnd = DateTime.ParseExact(wN.SearchString_CreateEndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            //        query = query.Where(s => s.wl.CREATED_DATE != null &&
            //        System.Data.Entity.DbFunctions.TruncateTime(s.wl.CREATED_DATE.Value) <= tempEnd
            //        );
            //    }



            //    if (!String.IsNullOrEmpty(wN.SearchString_IssuedStartDate))
            //    {
            //        DateTime tempStart = DateTime.ParseExact(wN.SearchString_IssuedStartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            //        query = query.
            //           Where(s => s.wl.LETTER_ISSUE_DATE != null &&
            //          System.Data.Entity.DbFunctions.TruncateTime(s.wl.LETTER_ISSUE_DATE.Value) >= tempStart
            //        );
            //    }
            //    if (!String.IsNullOrEmpty(wN.SearchString_IssuedEndDate))
            //    {
            //        DateTime tempEnd = DateTime.ParseExact(wN.SearchString_IssuedEndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            //        query = query.
            //            Where(s => s.wl.LETTER_ISSUE_DATE != null &&
            //          System.Data.Entity.DbFunctions.TruncateTime(s.wl.LETTER_ISSUE_DATE.Value) <= tempEnd);
            //    }
            //    if (!String.IsNullOrEmpty(wN.WL_ISSUED_BY))
            //    {
            //        query = query.Where(s => s.wl.WL_ISSUED_BY.ToLower().Contains(wN.WL_ISSUED_BY.ToLower()));

            //    }


            //    ///if (!String.IsNullOrEmpty(wN.SearchString_ExpiryStartDate) && !String.IsNullOrEmpty(wN.SearchString_ExpiryEndDate))
            //    ///{
            //    ///    DateTime tempStart = DateTime.ParseExact(wN.SearchString_ExpiryStartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            //    ///    query = query.
            //    ///                  Where(s => s.wl.EXPIRY_DATE != null &&
            //    ///                    System.Data.Entity.DbFunctions.TruncateTime(s.wl.EXPIRY_DATE.Value) >= tempStart
            //    ///                       );
            //    ///}
            //    ///if (!String.IsNullOrEmpty(wN.SearchString_ExpiryEndDate))
            //    ///{
            //    ///    DateTime tempEnd = DateTime.ParseExact(wN.SearchString_ExpiryEndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            //    ///    query = query.
            //    ///                  Where(s => s.wl.EXPIRY_DATE != null &&
            //    ///   System.Data.Entity.DbFunctions.TruncateTime(s.wl.EXPIRY_DATE.Value) <= tempEnd);
            //    ///}
            //    if (!String.IsNullOrEmpty(wN.REMARK))
            //    {
            //        query = query.Where(s => s.wl.REMARK != null && s.wl.REMARK.ToLower().Contains(wN.REMARK.ToLower()));
            //    }
            //    if (!String.IsNullOrEmpty(wN.NOTICE_NO))
            //    {
            //        query = query.Where(s => s.wl.NOTICE_NO != null && s.wl.NOTICE_NO.ToLower().Contains(wN.NOTICE_NO.ToLower()));
            //    }





            //    foreach (var item in query)
            //    {

            //        ModelWN modelWN = new ModelWN();
            //        modelWN.UUID = item.wl.UUID;
            //        modelWN.REGISTRATION_NO = item.wl.REGISTRATION_NO;
            //        modelWN.CATEGORY = item.wl.CATEGORY;
            //        modelWN.COMP_CONTRACTOR_NAME_ENG = item.wl.COMP_CONTRACTOR_NAME_ENG;
            //        modelWN.COMP_CONTRACTOR_NAME_CHI = item.wl.COMP_CONTRACTOR_NAME_CHI;
            //        modelWN.AUTHORIZED_SIGNATORY_NAME_ENG = item.wl.AUTHORIZED_SIGNATORY_NAME_ENG;
            //        modelWN.AUTHORIZED_SIGNATORY_NAME_CHI = item.wl.AUTHORIZED_SIGNATORY_NAME_CHI;
            //        foreach (var offenseItem in item.wl.W_WL_TYPE_OF_OFFENSE.OrderBy(x => x.WL_TYPE_OF_OFFENSE_ENG))
            //        {
            //            modelWN.OFFENSE_DESCRIPTION += offenseItem.WL_TYPE_OF_OFFENSE_ENG + " ,";
            //        }
            //        foreach (var mwitem in item.wl.W_WL_MW_ITEM.OrderBy(x => x.WL_MW_ITEM_ENG))
            //        {
            //            modelWN.MW_ITEMS += mwitem.WL_MW_ITEM_ENG + " ,";
            //        }
            //        modelWN.SUBJECT = item.wl.SUBJECT;
            //        modelWN.LETTER_ISSUE_DATE = item.wl.LETTER_ISSUE_DATE;
            //        modelWN.WL_ISSUED_BY = item.wl.WL_ISSUED_BY;
            //        modelWN.STATUS = item.wl.STATUS;
            //        modelWN.MW_SUBMISSION_NO = item.wl.MW_SUBMISSION_NO;
            //        modelWN.REMARK = item.wl.REMARK;
            //        modelWN.SECTION_UNIT = item.wl.SECTION_UNIT;
            //        modelWN.FILE_REF_FOUR = item.wl.FILE_REF_FOUR;
            //        modelWN.FILE_REF_TWO = item.wl.FILE_REF_TWO;
            //        modelWN.CREATED_BY = item.wl.CREATED_BY;
            //        modelWN.CREATED_DATE = item.wl.CREATED_DATE;
            //        modelWN.MODIFIED_DATE = item.wl.MODIFIED_DATE;
            //        modelWN.RELATED_TO = item.wl.RELATED_TO;
            //        modelWN.SOURCE = item.wl.SOURCE;
            //        ///modelWN.EXPIRY_DATE = item.wl.EXPIRY_DATE;
            //        modelWN.NOTICE_NO = item.wl.NOTICE_NO;
            //        modelWNList.Add(modelWN);
            //    }

            //    if (wN.v_MWItems_Type_CheckBox != null && wN.v_MWItems_Type_CheckBox.Count() != 0)
            //    {

            //        modelWNList = (from x in modelWNList
            //                       where (x.MW_ITEMS != null && wN.v_MWItems_Type_CheckBox.Any(val => x.MW_ITEMS.Contains(val))) || x.MW_ITEMS == null
            //                       select x).ToList();

            //    }

            //    modelWNList = modelWNList.GroupBy(x => x.UUID)
            //      .Select(grp => grp.First())
            //      .ToList();

            //    watch.Stop();
            //    var elapsedMs = watch.ElapsedMilliseconds;
            //    Log.Debug("---------Search Finished------");
            //    return View("WN", modelWNList);
            //}
            //catch (Exception ex)
            //{
            //    Log.Error(ex.Message + " *** " + ex.InnerException.Message);
            //    return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            //}
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

                var MWItems = from st in db.W_S_SYSTEM_TYPE
                              join sv in db.W_S_SYSTEM_VALUE on st.UUID equals sv.S_SYSTEM_TYPE_ID
                              where st.TYPE == "Type_Of_MW_Items"

                              select sv;
                ViewBag.MWItems = MWItems;



                return View();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message + " *** " + ex.InnerException.Message);
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


        public ActionResult WNEdit(string id, string file_UUID)
        {
            try
            {
                Log.Debug("View WNEdit");
                W_WL wL = null;

                if (id == null)
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
                            where wl_file.WL_UUID == id && wl_file.STATUS_DESCRIPTION == "Active"
                            select wl_file;

                ViewBag.WL_File = query;

                var SelectedMWItemsQuery = from wl_mw_item in db.W_WL_MW_ITEM
                                           where wl_mw_item.WL_UUID == id
                                           select wl_mw_item;

                ViewBag.SelectedMWItemsQuery = SelectedMWItemsQuery;

                var MWItems = from st in db.W_S_SYSTEM_TYPE
                              join sv in db.W_S_SYSTEM_VALUE on st.UUID equals sv.S_SYSTEM_TYPE_ID
                              where st.TYPE == "Type_Of_MW_Items"

                              select sv;
                ViewBag.MWItems = MWItems;

                var SelectedOffenseQuery = from wl_type_of_offense in db.W_WL_TYPE_OF_OFFENSE
                                           where wl_type_of_offense.WL_UUID == id
                                           select wl_type_of_offense;

                ViewBag.SelectedOffense = SelectedOffenseQuery;


                var OffenseType = from st in db.W_S_SYSTEM_TYPE
                                  join sv in db.W_S_SYSTEM_VALUE on st.UUID equals sv.S_SYSTEM_TYPE_ID
                                  where st.TYPE == "Type_Of_Offense"

                                  select sv;
                ViewBag.OffenseType = OffenseType;




                wL = db.W_WL.Find(id);

                if (wL == null)
                {
                    return HttpNotFound();
                }

                ///if (wL.EXPIRY_DATE != null)
                ///    ViewBag.ExpiryDate = cf.DateTimeToString(wL.EXPIRY_DATE.Value);

               
               ViewBag.IssuedDate = cf.DateTimeToString(wL.LETTER_ISSUE_DATE.Value);

                
               ViewBag.CreatedDate = cf.DateTimeToString(wL.CREATED_DATE.Value);


                ViewBag.CreatedDate = cf.DateTimeToString(wL.CREATED_DATE.Value);

                #region AS Name Handling
                var ASquery = db.P_MW_CRM_INFO.Where(s => s.CERTIFICATION_NO.Equals(wL.REGISTRATION_NO)).Distinct().Select(x => new { x.AS_SURNAME, x.AS_GIVEN_NAME, x.AS_CHINESE_NAME }).Distinct();

                List<string> AS_ENG_NAME_List = new List<string>();
                foreach (var item in ASquery)
                {
                    AS_ENG_NAME_List.Add(item.AS_SURNAME + " " + item.AS_GIVEN_NAME + "," + item.AS_CHINESE_NAME);

                }

                ViewBag.AS_ENG_NAME_List = AS_ENG_NAME_List;
                wL.AUTHORIZED_SIGNATORY_NAME_ENG = wL.AUTHORIZED_SIGNATORY_NAME_ENG + "," + wL.AUTHORIZED_SIGNATORY_NAME_CHI;

                #endregion
                //endregion 




                Log.Debug("View WNEdit End");
                return View(wL);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message + " *** " + ex.InnerException.Message);
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
        }



        [HttpPost]
        [ValidateAntiForgeryToken]

        // public ActionResult WNEdit(ModelWN wN, [Bind(Include = "UUID,SUBJECT,CATEGORY,REGISTRATION_NO,MW_SUBMISSION_NO,MW_ITEMS,COMP_CONTRACTOR_NAME_ENG,COMP_CONTRACTOR_NAME_CHI,CREATION_DATE,SECTION_UNIT,FILE_REF_FOUR,FILE_REF_TWO,WL_ISSUED_BY,POST,CASE_OFFICER,RELATED_TO,SOURCE,LETTER_ISSUE_DATE,LETTER_FILE_PATH,AUTHORIZED_SIGNATURE,STATUS,REMARK,CREATED_DATE,CREATED_BY,MODIFIED_DATE,MODIFIED_BY,OTHER_REFERENCE_NO,AUTHORIZED_SIGNATORY_NAME_CHI,AUTHORIZED_SIGNATORY_NAME_ENG")] WL wL)


        public ActionResult WNEdit(ModelWN wN, [Bind(Exclude = "")] W_WL wL)
        {

            try
            {
                Log.Debug("Edit Warning Letter id:" + wL.UUID);
                Log.Debug("Subject :" + wL.SUBJECT);
                //     using (db) {
                if (ModelState.IsValid)
                {

                    byte[] fileData = null;
       
                    for (int i = 0; i < Request.Files.Count; i++)
                    {

                        var file = Request.Files[i];

                        if (file != null && file.ContentLength > 0)
                        {
                            var fileName = Path.GetFileName(file.FileName);

                            using (var binaryReader = new BinaryReader(Request.Files[i].InputStream))
                            {
                                fileData = binaryReader.ReadBytes(Request.Files[i].ContentLength);
                            }
                            W_WL_FILE wl_file = new W_WL_FILE();

                            wl_file.UUID = System.Guid.NewGuid().ToString();
                            wl_file.LETTER_FILE = fileData;
                            wl_file.WL_UUID = wL.UUID;
                            wl_file.CREATED_BY = SystemParameterConstant.UserName;
                            wl_file.CREATED_DATE = System.DateTime.Now;
                            wl_file.MODIFIED_BY = SystemParameterConstant.UserName;
                            wl_file.MODIFIED_DATE = System.DateTime.Now;
                            wl_file.STATUS_DESCRIPTION = "Active";
                            wl_file.FILE_NAME = fileName;
                            db.W_WL_FILE.Add(wl_file);
     
                        }
                    }


                    var ToDeleteOffenseQuery = db.W_WL_TYPE_OF_OFFENSE.Where(p => p.WL_UUID == wL.UUID);

                    foreach (var item in ToDeleteOffenseQuery)
                    {
                        db.W_WL_TYPE_OF_OFFENSE.Remove(item);
                    }

                    if (wN.v_Offense_Type_CheckBox != null)
                    {
                        foreach (var item in wN.v_Offense_Type_CheckBox)
                        {
                            W_WL_TYPE_OF_OFFENSE wl_Type_of_offense = new W_WL_TYPE_OF_OFFENSE();
                            wl_Type_of_offense.UUID = System.Guid.NewGuid().ToString();
                            wl_Type_of_offense.WL_UUID = wL.UUID;
                            wl_Type_of_offense.CREATED_BY = SystemParameterConstant.UserName;
                            wl_Type_of_offense.CREATED_DATE = System.DateTime.Now;
                            wl_Type_of_offense.MODIFIED_BY = SystemParameterConstant.UserName;
                            wl_Type_of_offense.MODIFIED_DATE = System.DateTime.Now;

                            wl_Type_of_offense.WL_TYPE_OF_OFFENSE_ENG = item.ToString();
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
                            wl_mw_item.CREATED_BY = SystemParameterConstant.UserName;
                            wl_mw_item.CREATED_DATE = System.DateTime.Now;
                            wl_mw_item.MODIFIED_BY = SystemParameterConstant.UserName;
                            wl_mw_item.MODIFIED_DATE = System.DateTime.Now;

                            wl_mw_item.WL_MW_ITEM_ENG = item.ToString();


                            db.W_WL_MW_ITEM.Add(wl_mw_item);

                        }
                    }
                    if (wN.v_Related_checkbox != null)
                    {
                        wL.RELATED_TO = "";
                        foreach (var item in wN.v_Related_checkbox)
                        {

                            wL.RELATED_TO += item.ToString() + ",";
                        }
                    }
                    #region AS Name Handling
                    var SplitASEng = wL.AUTHORIZED_SIGNATORY_NAME_ENG.Split(',');
                    wL.AUTHORIZED_SIGNATORY_NAME_ENG = SplitASEng[0];
                    wL.AUTHORIZED_SIGNATORY_NAME_CHI = SplitASEng[1];
                    #endregion
                    //wL.EXPIRY_DATE = DateTime.ParseExact(v_ExpiryDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    db.Entry(wL).State = EntityState.Modified;



                    db.SaveChanges();
                }
                //          }
            }
            catch (DbEntityValidationException dbex)
            {
                Log.Error(dbex.Message + " *** " + dbex.InnerException.Message);
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message + " *** " + ex.InnerException.Message);
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            return RedirectToAction("WN");
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


            var MWItems = from st in db.W_S_SYSTEM_TYPE
                          join sv in db.W_S_SYSTEM_VALUE on st.UUID equals sv.S_SYSTEM_TYPE_ID
                          where st.TYPE == "Type_Of_MW_Items"

                          select sv;
            ViewBag.MWItems = MWItems;

            ViewBag.Test = "123";
            return View();
        }

        [HttpPost]

        public ActionResult GET_CON_IND_NAME(string RegNo)
        {
            Log.Debug("Function Start");
            var query = db.P_MW_CRM_INFO.Where(s => s.CERTIFICATION_NO.Equals(RegNo)).Distinct().ToList();

            Log.Debug("1");
            try
            {
                Log.Debug("1.1");
               
                var AS = query.Select(x => new { x.CHINESE_NAME, x.SURNAME, x.GIVEN_NAME,  x.AS_SURNAME, x.AS_GIVEN_NAME, x.AS_CHINESE_NAME }).Distinct();
                Log.Debug("2");
                List<string> AS_NAME_List = new List<string>();
                foreach (var item in AS)
                {   
                    // User require merge the text box 
                    if (item.AS_SURNAME != null)
                        AS_NAME_List.Add(item.AS_SURNAME + " " + item.AS_GIVEN_NAME + "," + item.AS_CHINESE_NAME);

                }
                Log.Debug("3");
                var result = new
                {
                    ChineseName = "",
                    EnglishName = "",
                    Status = "",
                    AS_NAME_List

                };
                //if (query.Any())
                if (AS.Any())
                {
                    Log.Debug("3.1");
                    result = new
                    {
                        //pass Chi, eng, status and as to view
                        ChineseName = query.First().CHINESE_NAME,
                        EnglishName = query.First().SURNAME +" "+ query.First().GIVEN_NAME,
                        Status = query.First().STATUS,
                        AS_NAME_List

                    };

                }
                Log.Debug("4");
                // var AS = query.Select(x => new { x.AS_SURNAME, x.AS_GIVEN_NAME });

                //foreach (var item in AS)
                //{ }
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
            Log.Debug("Function End");


            return Json("", JsonRequestBehavior.AllowGet);
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
                    Log.Debug("Create Warning Letter");
                    wL.UUID = System.Guid.NewGuid().ToString();
                    Log.Debug("UUID : " + wL.UUID);
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

                            using (var binaryReader = new BinaryReader(Request.Files[i].InputStream))
                            {
                                fileData = binaryReader.ReadBytes(Request.Files[i].ContentLength);
                                // wN.v_Temp_file.Add(new Tuple<byte[], string>(fileData, fileName));
                            }
                            W_WL_FILE wl_file = new W_WL_FILE();

                            wl_file.UUID = System.Guid.NewGuid().ToString();
                            wl_file.LETTER_FILE = fileData;
                            wl_file.WL_UUID = wL.UUID;
                            wl_file.CREATED_BY = SystemParameterConstant.UserName;
                            wl_file.CREATED_DATE = System.DateTime.Now;
                            wl_file.MODIFIED_BY = SystemParameterConstant.UserName;
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

                    wN.M_WL_TYPE_OF_OFFENSE = new List<W_WL_TYPE_OF_OFFENSE>();
                    if (wN.v_Offense_Type_CheckBox != null)
                    {
                        foreach (var item in wN.v_Offense_Type_CheckBox)
                        {
                            W_WL_TYPE_OF_OFFENSE wl_Type_of_offense = new W_WL_TYPE_OF_OFFENSE();
                            wl_Type_of_offense.UUID = System.Guid.NewGuid().ToString();
                            wl_Type_of_offense.WL_UUID = wL.UUID;
                            wl_Type_of_offense.CREATED_BY = SystemParameterConstant.UserName;
                            wl_Type_of_offense.CREATED_DATE = System.DateTime.Now;
                            wl_Type_of_offense.MODIFIED_BY = SystemParameterConstant.UserName;
                            wl_Type_of_offense.MODIFIED_DATE = System.DateTime.Now;

                            wl_Type_of_offense.WL_TYPE_OF_OFFENSE_ENG = item.ToString();


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
                            wl_mw_item.CREATED_BY = SystemParameterConstant.UserName;
                            wl_mw_item.CREATED_DATE = System.DateTime.Now;
                            wl_mw_item.MODIFIED_BY = SystemParameterConstant.UserName;
                            wl_mw_item.MODIFIED_DATE = System.DateTime.Now;

                            wl_mw_item.WL_MW_ITEM_ENG = item.ToString();

                            ////wN.M_WL_MW_ITEM.Add(wl_mw_item);
                            wL.W_WL_MW_ITEM.Add(wl_mw_item);
                            //// db.WL_MW_ITEM.Add(wl_mw_item);

                        }
                    }


                    if (wN.v_IssuedDate == "" || wN.v_IssuedDate == null)
                    { }
                    else
                    {
                        wL.LETTER_ISSUE_DATE = cf.StringToDateTime(wN.v_IssuedDate);
                    }

                    #region AS Name Handleing
                    //split the AS text box 
                    if (wL.AUTHORIZED_SIGNATORY_NAME_ENG != null)
                    {
                        var SplitASEng = wL.AUTHORIZED_SIGNATORY_NAME_ENG.Split(',');
                        wL.AUTHORIZED_SIGNATORY_NAME_ENG = SplitASEng[0];
                        wL.AUTHORIZED_SIGNATORY_NAME_CHI = SplitASEng[1];
                    }
                    #endregion
                    wL.POST = SystemParameterConstant.UserName;
                    wL.CASE_OFFICER = "CaseAdmin";

                    wL.CREATED_DATE = System.DateTime.Now;
                    wL.CREATED_BY = SystemParameterConstant.UserName;
                    wL.MODIFIED_BY = SystemParameterConstant.UserName;
                    wL.MODIFIED_DATE = System.DateTime.Now;

                    /// wN.M_WL = wL;
                    db.W_WL.Add(wL);

                    db.SaveChanges();
                    Log.Debug("Create WL Finished");
                    //    }
                }
                catch (DbEntityValidationException dbex)
                {
                    Log.Error(dbex.Message + " *** " + dbex.InnerException.Message);
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);

                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message + " *** " + ex.InnerException.Message);
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
                query.STATUS = "Deleted";
                db.SaveChanges();
                Log.Debug("Warning Letter ID:" + id + " Deleted");
                return RedirectToAction("WN");

            }
            catch (Exception ex)
            {
                Log.Error(ex.Message + " *** " + ex.InnerException.Message);
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

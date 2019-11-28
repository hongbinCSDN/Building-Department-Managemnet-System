using MWMS2.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Models;
using MWMS2.DaoController;
using System.Globalization;

namespace MWMS2.Controllers
{
    public class SMM0104Controller : Controller
    {
        private EntitiesSignboard db = new EntitiesSignboard();
        // GET: SMM0104
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult SMM0104()
        {
            DaoSMMDataEntry daoSMMDataEntry = new DaoSMMDataEntry();

            var modelSVSubmissionList = daoSMMDataEntry.GetDataEntryDocList("","","","");
            return View("", modelSVSubmissionList);

            //var query = from submission in db.B_SV_SUBMISSION
            //            join scanned_doc in db.B_SV_SCANNED_DOCUMENT on submission.SV_SCANNED_DOCUMENT_ID equals scanned_doc.UUID
            //            where submission.STATUS == "SCU_DATA_ENTRY"
            //            orderby submission.REFERENCE_NO
            //            select new { submission, scanned_doc };

            //List<ModelSVSubmission> modelSVSubmissionList = new List<ModelSVSubmission>();

            //foreach (var item in query)
            //{
            //    ModelSVSubmission modelSVSubmission = new ModelSVSubmission();
            //    modelSVSubmission.UUID = item.submission.UUID;
            //    modelSVSubmission.SubmissionNo = item.submission.REFERENCE_NO;
            //    modelSVSubmission.DSN_NO = item.scanned_doc.DSN_NUMBER;
            //    modelSVSubmission.Form_Code = item.submission.FORM_CODE;
            //    modelSVSubmission.Received_Date = item.submission.SCU_RECEIVED_DATE;
            //    modelSVSubmission.Time = item.submission.SCU_RECEIVED_DATE.Value.ToShortTimeString();
            //    modelSVSubmission.Status = "Draft"; //query where clause , status must be "SCU_DATA_ENTRY" 
            //    modelSVSubmission.Batch_Number = item.submission.BATCH_NO;
            //    modelSVSubmissionList.Add(modelSVSubmission);

            //}

        }
        public ActionResult SMM0104DataEntryCreate(ModelSVRecord modelSVRecord)
        {
            DaoSMMDataEntry daoSMMDataEntry = new DaoSMMDataEntry();
            daoSMMDataEntry.DataEntryCreateSVRecord(modelSVRecord);


            return RedirectToAction("SMM0104DataEntryEdit", new { SubmissionId = modelSVRecord.SubmissionUUID, SumbissionNo = modelSVRecord.SubmissionNo });
            //return SMM0104DataEntryEdit(modelSVRecord.SubmissionUUID, modelSVRecord.SubmissionNo);
        }

        public ActionResult SMM0104Search(string SubmissionNo, string ReceivedStartDate, string ReceivedEndDate)
        {
            DaoSMMDataEntry daoSMMDataEntry = new DaoSMMDataEntry();
            var modelSVSubmissionList = daoSMMDataEntry.GetDataEntryDocList("",SubmissionNo, ReceivedStartDate, ReceivedEndDate);

            return View("SMM0104", modelSVSubmissionList);

            //var query = from submission in db.B_SV_SUBMISSION
            //            join scanned_doc in db.B_SV_SCANNED_DOCUMENT on submission.SV_SCANNED_DOCUMENT_ID equals scanned_doc.UUID
            //            where submission.STATUS == "SCU_DATA_ENTRY"
            //            orderby submission.REFERENCE_NO
            //            select new { submission, scanned_doc };

            //if (!String.IsNullOrEmpty(SubmissionNo))
            //{
            //    query = query.Where(x => x.submission.REFERENCE_NO == SubmissionNo);
            //}
            //if (!String.IsNullOrEmpty(ReceivedStartDate))
            //{
            //    DateTime tempStart = DateTime.ParseExact(ReceivedStartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            //    query = query.
            //              Where(s => s.submission.SCU_RECEIVED_DATE != null &&
            //             System.Data.Entity.DbFunctions.TruncateTime(s.submission.SCU_RECEIVED_DATE.Value) >= tempStart
            //           );

            //}
            //if (!String.IsNullOrEmpty(ReceivedEndDate))
            //{
            //    DateTime tempEnd = DateTime.ParseExact(ReceivedEndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            //    query = query.
            //              Where(s => s.submission.SCU_RECEIVED_DATE != null &&
            //             System.Data.Entity.DbFunctions.TruncateTime(s.submission.SCU_RECEIVED_DATE.Value) >= tempEnd
            //           );

            //}

            //List<ModelSVSubmission> modelSVSubmissionList = new List<ModelSVSubmission>();

            //foreach (var item in query)
            //{
            //    ModelSVSubmission modelSVSubmission = new ModelSVSubmission();
            //    modelSVSubmission.UUID = item.submission.UUID;
            //    modelSVSubmission.SubmissionNo = item.submission.REFERENCE_NO;
            //    modelSVSubmission.DSN_NO = item.scanned_doc.DSN_NUMBER;
            //    modelSVSubmission.Form_Code = item.submission.FORM_CODE;
            //    modelSVSubmission.Received_Date = item.submission.SCU_RECEIVED_DATE;
            //    modelSVSubmission.Time = item.submission.SCU_RECEIVED_DATE.Value.ToShortTimeString();
            //    modelSVSubmission.Status = "Draft"; //query where clause , status must be "SCU_DATA_ENTRY" 
            //    modelSVSubmission.Batch_Number = item.submission.BATCH_NO;
            //    modelSVSubmissionList.Add(modelSVSubmission);

            //}



            //return View("SMM0104", modelSVSubmissionList);
        }


            public ActionResult SMM0104DataEntryEdit(string SubmissionId, string SumbissionNo )
        {
            DaoSMMDataEntry daoSMMDataEntry = new DaoSMMDataEntry();

            ModelSVRecord modelSVRecord = daoSMMDataEntry.DataEntryDocListViewDetail(SubmissionId, SumbissionNo); 


            List <SelectListItem> RelatedOrderResult = new List<SelectListItem>();
            foreach (var RelatedOrderitem in daoSMMDataEntry.GetB_System_ValueByType("OrderType"))
            {
                RelatedOrderResult.Add(new SelectListItem
                {
                    Text = RelatedOrderitem.DESCRIPTION,
                    Value = RelatedOrderitem.CODE,

                });

            }
            ViewBag.RelatedOrderResult = RelatedOrderResult;

         
            List<List<SelectListItem>> ValidationItemResultList = new List<List<SelectListItem>>();
            for (int i = 0; i < modelSVRecord.SelectedValidationItems.Count()+1; i++)
            {
              
                List<SelectListItem> ValidationItemResult = new List<SelectListItem>();
            ValidationItemResult.Add(new SelectListItem
            {
                Text = "-",
                Value = "",

            });

        
                foreach (var ValidationItemitem in daoSMMDataEntry.GetB_System_ValueByType("ValidationItem"))
                {
                    bool selected = false;
                    if (i != modelSVRecord.SelectedValidationItems.Count())
                    {
                        if (ValidationItemitem.DESCRIPTION == modelSVRecord.SelectedValidationItems[i])
                        {
                            selected = true;
                        }
                    }
                    ValidationItemResult.Add(new SelectListItem
                    {
                        Text = ValidationItemitem.CODE,
                        Value = ValidationItemitem.DESCRIPTION,
                        Selected = selected
                    });
                 
                }
                ValidationItemResultList.Add(ValidationItemResult);
            }
            ViewBag.ValidationItemResultList = ValidationItemResultList;



            List<List<SelectListItem>> MWItemQueryResultList = new List<List<SelectListItem>>();



            for (int i = 0; i < modelSVRecord.SelectedMWItems.Count() + 1; i++)
            {
                List<SelectListItem> MWItemQueryResult = new List<SelectListItem>();
                MWItemQueryResult.Add(new SelectListItem
                {
                    Text = "-",
                    Value = "",

                });
                foreach (var MWItemitem in daoSMMDataEntry.GetB_System_ValueByType("Item No"))
                {

                    bool selected = false;
                    if (i != modelSVRecord.SelectedMWItems.Count())
                    {
                        if (MWItemitem.DESCRIPTION == modelSVRecord.SelectedMWItems[i])
                        {
                            selected = true;
                        }
                    }
                    MWItemQueryResult.Add(new SelectListItem
                    {
                        Text = MWItemitem.CODE,
                        Value = MWItemitem.DESCRIPTION,
                        Selected = selected
                    });

                }
                MWItemQueryResultList.Add(MWItemQueryResult);
            }
            ViewBag.MWItemQueryResultList = MWItemQueryResultList;


            List<SelectListItem> PAWSAMEASQueryResult = new List<SelectListItem>();
            foreach (var PAWSAMEASitem in daoSMMDataEntry.GetB_System_ValueByType("PawSameAs"))
            {


                PAWSAMEASQueryResult.Add(new SelectListItem
                {
                    Text = PAWSAMEASitem.CODE,
                    Value = PAWSAMEASitem.DESCRIPTION,

                });

            }
            ViewBag.PAWSAMEASQueryResult = PAWSAMEASQueryResult;



            List<SelectListItem> TOHandlingOfficerResult = new List<SelectListItem>();       
            //foreach (var TOitem in daoSMMDataEntry.GetTOUser())
            //{


            //    TOHandlingOfficerResult.Add(new SelectListItem
            //    {
            //        Text = TOitem.USERNAME,
            //        Value = TOitem.UUID,

            //    });

            //}
            ViewBag.TOHandlingOfficerResult = TOHandlingOfficerResult;



            List<SelectListItem> LetterType = new List<SelectListItem>();
            foreach (var LetterTypeitem in daoSMMDataEntry.GetB_System_ValueByType("LetterType"))
            {
                LetterType.Add(new SelectListItem
                {
                    Text = LetterTypeitem.DESCRIPTION,
                    Value = LetterTypeitem.CODE,

                });

            }
            ViewBag.LetterType = LetterType;



            List<SelectListItem> BCISDistrictResult = new List<SelectListItem>();
            foreach (var BCISitem in daoSMMDataEntry.GetBCISDistrict(modelSVRecord.SVInfoSelectedRegion))
            {
                BCISDistrictResult.Add(new SelectListItem
                {
                    Text = BCISitem.CODE + " - " + BCISitem.DESCRIPTION,
                    Value = BCISitem.CODE,

                });

            }
            ViewBag.BCISDistrictResult = BCISDistrictResult;



            return View(modelSVRecord);







            //ModelSVRecord modelSVRecord = new ModelSVRecord();
            //CommonFunction cf = new CommonFunction();
            //string TempRegion = "HK";




            //var query = from submission in db.B_SV_SUBMISSION
            //            join scanned_doc in db.B_SV_SCANNED_DOCUMENT on submission.SV_SCANNED_DOCUMENT_ID equals scanned_doc.UUID
            //            where submission.REFERENCE_NO == id
            //            orderby submission.REFERENCE_NO
            //            select new { submission, scanned_doc };

            //modelSVRecord.SubmissionNo = id;

            //if (FormCode == "SC01C" || FormCode == "SC02C")
            //{
            //    modelSVRecord.ReceivedDate = cf.DateTimeToString(query.Where(x => x.submission.STATUS == "VALIDATION").FirstOrDefault().submission.SCU_RECEIVED_DATE.Value);
            //}
            //else
            //{
            //    modelSVRecord.ReceivedDate = cf.DateTimeToString(query.FirstOrDefault().submission.SCU_RECEIVED_DATE.Value);
            //}
            ////var item = query.FirstOrDefault();

            //modelSVRecord.FormCode = FormCode;


            ////default 
            //modelSVRecord.FormLanguage = "ZH";






            //modelSVRecord.Regions = new List<Region>();

            //var DistrictQuery = from st in db.B_S_SYSTEM_TYPE
            //                    join sv in db.B_S_SYSTEM_VALUE on st.UUID equals sv.SYSTEM_TYPE_ID
            //                    where st.TYPE == "Region"
            //                    orderby sv.CODE
            //                    select sv;
            //foreach (var Ditem in DistrictQuery)
            //{
            //    Region region = new Region();
            //    region.Code = Ditem.CODE;
            //    region.Description = Ditem.DESCRIPTION;
            //    modelSVRecord.Regions.Add(region);

            //}




            //var RelatedOrderQuery = from st in db.B_S_SYSTEM_TYPE
            //                        join sv in db.B_S_SYSTEM_VALUE on st.UUID equals sv.SYSTEM_TYPE_ID
            //                        where st.TYPE == "OrderType"
            //                        orderby sv.IS_ACTIVE
            //                        select sv;
            //List<SelectListItem> RelatedOrderResult = new List<SelectListItem>();


            //foreach (var RelatedOrderitem in RelatedOrderQuery)
            //{
            //    RelatedOrderResult.Add(new SelectListItem
            //    {
            //        Text = RelatedOrderitem.DESCRIPTION,
            //        Value = RelatedOrderitem.DESCRIPTION,

            //    });

            //}
            //ViewBag.RelatedOrderResult = RelatedOrderResult;



            //var ValidationItemQuery = from st in db.B_S_SYSTEM_TYPE
            //                        join sv in db.B_S_SYSTEM_VALUE on st.UUID equals sv.SYSTEM_TYPE_ID
            //                        where st.TYPE == "ValidationItem"
            //                          orderby sv.CODE
            //                        select sv;
            //List<SelectListItem> ValidationItemResult = new List<SelectListItem>();
            //ValidationItemResult.Add(new SelectListItem
            //{
            //    Text = "-",
            //    Value = "",

            //});


            //foreach (var ValidationItemitem in ValidationItemQuery)
            //{


            //    ValidationItemResult.Add(new SelectListItem
            //    {
            //        Text = ValidationItemitem.CODE,
            //        Value = ValidationItemitem.DESCRIPTION,

            //    });

            //}
            //ViewBag.ValidationItemResult = ValidationItemResult;


            //var MWItemQuery = from st in db.B_S_SYSTEM_TYPE
            //                          join sv in db.B_S_SYSTEM_VALUE on st.UUID equals sv.SYSTEM_TYPE_ID
            //                          where st.TYPE == "Item No"
            //                          orderby sv.CODE
            //                          select sv;
            //List<SelectListItem> MWItemQueryResult = new List<SelectListItem>();
            //MWItemQueryResult.Add(new SelectListItem
            //{
            //    Text = "-",
            //    Value = "",

            //});


            //foreach (var MWItemitem in MWItemQuery)
            //{


            //    MWItemQueryResult.Add(new SelectListItem
            //    {
            //        Text = MWItemitem.CODE,
            //        Value = MWItemitem.DESCRIPTION,

            //    });

            //}
            //ViewBag.MWItemQueryResult = MWItemQueryResult;






            //var PAWSAMEASQuery = from st in db.B_S_SYSTEM_TYPE
            //                  join sv in db.B_S_SYSTEM_VALUE on st.UUID equals sv.SYSTEM_TYPE_ID
            //                  where st.TYPE == "PawSameAs"
            //                  orderby sv.CODE
            //                  select sv;
            //List<SelectListItem> PAWSAMEASQueryResult = new List<SelectListItem>();



            //foreach (var PAWSAMEASitem in PAWSAMEASQuery)
            //{


            //    PAWSAMEASQueryResult.Add(new SelectListItem
            //    {
            //        Text = PAWSAMEASitem.CODE,
            //        Value = PAWSAMEASitem.DESCRIPTION,

            //    });

            //}
            //ViewBag.PAWSAMEASQueryResult = PAWSAMEASQueryResult;



            //var TOHandlingOfficerQuery = from user in db.B_S_USER_ACCOUNT
            //                             where user.RANK == "TO"
            //                             orderby user.USERNAME
            //                             select user;
            //List<SelectListItem> TOHandlingOfficerResult = new List<SelectListItem>();



            //foreach (var TOitem in TOHandlingOfficerQuery)
            //{


            //    TOHandlingOfficerResult.Add(new SelectListItem
            //    {
            //        Text = TOitem.USERNAME,
            //        Value = TOitem.UUID,

            //    });

            //}
            //ViewBag.TOHandlingOfficerResult = TOHandlingOfficerResult;






            //var LetterTypeQuery = from st in db.B_S_SYSTEM_TYPE
            //                      join sv in db.B_S_SYSTEM_VALUE on st.UUID equals sv.SYSTEM_TYPE_ID
            //                      where st.TYPE == "LetterType"
            //                      orderby sv.CODE
            //                      select sv;


            //List<SelectListItem> LetterType = new List<SelectListItem>();
            //foreach (var LetterTypeitem in LetterTypeQuery)
            //{
            //    LetterType.Add(new SelectListItem
            //    {
            //        Text = LetterTypeitem.DESCRIPTION,
            //        Value = LetterTypeitem.CODE,

            //    });

            //}
            //ViewBag.LetterType = LetterType;


            //var SVRecordJoinnedQuery = from sv_record in db.B_SV_RECORD
            //                           join sv_signboard in db.B_SV_SIGNBOARD on sv_record.SV_SIGNBOARD_ID equals sv_signboard.UUID
            //                           join sv_rv_address in db.B_SV_RV_ADDRESS on sv_signboard.LOCATION_ADDRESS_ID equals sv_rv_address.SV_ADDRESS_ID
            //                           join sv_address in db.B_SV_ADDRESS on sv_signboard.LOCATION_ADDRESS_ID equals sv_address.UUID
            //                           where sv_record.REFERENCE_NO == id 
            //                           select new { sv_record, sv_signboard, sv_rv_address, sv_address };





            ////var SVRecordQuery = db.B_SV_RECORD.Where(x => x.REFERENCE_NO == id);
            //if (SVRecordJoinnedQuery.Count() > 0)
            //{
            //    modelSVRecord.FormLanguage = SVRecordJoinnedQuery.FirstOrDefault().sv_record.LANGUAGE_CODE;
            //    modelSVRecord.SVInfoLocationOfSignboard = SVRecordJoinnedQuery.FirstOrDefault().sv_signboard.LOCATION_OF_SIGNBOARD;
            //    modelSVRecord.SVInfoSelectedRegion = SVRecordJoinnedQuery.FirstOrDefault().sv_address.REGION;
            //    modelSVRecord.SVInfoBlock = SVRecordJoinnedQuery.FirstOrDefault().sv_address.BLOCK;
            //    modelSVRecord.SVInfoFloor = SVRecordJoinnedQuery.FirstOrDefault().sv_address.FLOOR;
            //    modelSVRecord.SVInfoFlat = SVRecordJoinnedQuery.FirstOrDefault().sv_address.FLAT;
            //    modelSVRecord.SVInfoBuildingEstate = SVRecordJoinnedQuery.FirstOrDefault().sv_address.BUILDINGNAME;
            //    modelSVRecord.SVInfoStreetNumber = SVRecordJoinnedQuery.FirstOrDefault().sv_address.STREET_NO;
            //    modelSVRecord.SVInfoStreetRoadVillageName = SVRecordJoinnedQuery.FirstOrDefault().sv_address.STREET;
            //    modelSVRecord.SVInfoBCISBlockID = SVRecordJoinnedQuery.FirstOrDefault().sv_address.BCIS_BLOCK_ID;

            //    modelSVRecord.SVInfoBCISDistrict = SVRecordJoinnedQuery.FirstOrDefault().sv_address.DISTRICT;

            //    modelSVRecord.SVInfoSelectedRegion = SVRecordJoinnedQuery.FirstOrDefault().sv_address.REGION;
            //    TempRegion = modelSVRecord.SVInfoSelectedRegion;
            //    modelSVRecord.SVInfoRVD_No = SVRecordJoinnedQuery.FirstOrDefault().sv_signboard.RVD_NO;
            //    //modelSVRecord.SVInofRVDBlockID= 

            //    modelSVRecord.SVInfoBCIS4plus2 = SVRecordJoinnedQuery.FirstOrDefault().sv_address.FILE_REFERENCE_NO;
            //    modelSVRecord.SVInfoBCISDistrict = SVRecordJoinnedQuery.FirstOrDefault().sv_address.BCIS_DISTRICT;










            //    //modelSVRecord.FormLanguage = SVRecordJoinnedQuery.FirstOrDefault().sv_record.LANGUAGE_CODE;
            //    //modelSVRecord.SVInfoLocationOfSignboard = SVRecordJoinnedQuery.FirstOrDefault().sv_signboard.LOCATION_OF_SIGNBOARD;
            //    //modelSVRecord.SVInfoSelectedRegion = SVRecordJoinnedQuery.FirstOrDefault().sv_address.DISPLAY_REGION;
            //    //modelSVRecord.SVInfoBlock = SVRecordJoinnedQuery.FirstOrDefault().sv_rv_address.BLOCK;
            //    //modelSVRecord.SVInfoFloor = SVRecordJoinnedQuery.FirstOrDefault().sv_rv_address.DISPLAY_FLOOR;
            //    //modelSVRecord.SVInfoFlat = SVRecordJoinnedQuery.FirstOrDefault().sv_rv_address.DISPLAY_FLAT;
            //    //modelSVRecord.SVInfoBuildingEstate = SVRecordJoinnedQuery.FirstOrDefault().sv_rv_address.DISPLAY_BUILDINGNAME;
            //    //modelSVRecord.SVInfoStreetNumber = SVRecordJoinnedQuery.FirstOrDefault().sv_rv_address.DISPLAY_STREET_NO;
            //    //modelSVRecord.SVInfoStreetRoadVillageName = SVRecordJoinnedQuery.FirstOrDefault().sv_rv_address.DISPLAY_STREET;
            //    //modelSVRecord.SVInfoBCISBlockID = SVRecordJoinnedQuery.FirstOrDefault().sv_rv_address.BCIS_BLOCK_ID;

            //    //modelSVRecord.SVInfoBCISDistrict = SVRecordJoinnedQuery.FirstOrDefault().sv_rv_address.DISPLAY_DISTRICT;
            //    //modelSVRecord.SVInfoSelectedRegion = SVRecordJoinnedQuery.FirstOrDefault().sv_rv_address.DISPLAY_REGION;

            //    //modelSVRecord.SVInfoRVD_No = SVRecordJoinnedQuery.FirstOrDefault().sv_signboard.RVD_NO;
            //    ////modelSVRecord.SVInofRVDBlockID=
            //    //modelSVRecord.SVInfoBCISBlockID = SVRecordJoinnedQuery.FirstOrDefault().sv_rv_address.BCIS_BLOCK_ID;
            //    //modelSVRecord.SVInfoBCIS4plus2 = SVRecordJoinnedQuery.FirstOrDefault().sv_rv_address.FILE_REFERENCE_NO;
            //    //modelSVRecord.SVInfoBCISDistrict = SVRecordJoinnedQuery.FirstOrDefault().sv_rv_address.bci


            //    //modelSVRecord.SVInfoLocationOfSignboard =db.B_SV_SIGNBOARD.Where(x=>x.UUID== SVRecordQuery.FirstOrDefault().SV_SIGNBOARD_ID).FirstOrDefault().LOCATION_OF_SIGNBOARD;

            //}

            //var BCISDistrictQuery = from st in db.B_S_SYSTEM_TYPE
            //                        join sv in db.B_S_SYSTEM_VALUE on st.UUID equals sv.SYSTEM_TYPE_ID
            //                        where st.TYPE == "BcisDistrict" && sv.PARENT_ID == (from st in db.B_S_SYSTEM_TYPE
            //                                                                            join sv in db.B_S_SYSTEM_VALUE on st.UUID equals sv.SYSTEM_TYPE_ID
            //                                                                            where st.TYPE == "Region" && sv.CODE == TempRegion
            //                                                                            orderby sv.CODE
            //                                                                            select sv).FirstOrDefault().UUID
            //                        orderby sv.CODE
            //                        select sv;
            //List<SelectListItem> BCISDistrictResult = new List<SelectListItem>();


            //foreach (var BCISitem in BCISDistrictQuery)
            //{
            //    BCISDistrictResult.Add(new SelectListItem
            //    {
            //        Text = BCISitem.CODE + " - " + BCISitem.DESCRIPTION,
            //        Value = BCISitem.CODE,

            //    });

            //}
            //ViewBag.BCISDistrictResult = BCISDistrictResult;


            //return View(modelSVRecord);
        }

 
        [HttpPost]
        public JsonResult SelectedRegionChange(string Region)
        {
            //var queryRegionUUID = (from st in db.B_S_SYSTEM_TYPE
            //                      join sv in db.B_S_SYSTEM_VALUE on st.UUID equals sv.SYSTEM_TYPE_ID
            //                      where st.TYPE == "Region" && sv.CODE == Region
            //                      orderby sv.CODE
            //                      select sv);


            //var BCISDistrictQuery = from st in db.B_S_SYSTEM_TYPE
            //                        join sv in db.B_S_SYSTEM_VALUE on st.UUID equals sv.SYSTEM_TYPE_ID
            //                        where st.TYPE == "BcisDistrict" && sv.PARENT_ID == (from st in db.B_S_SYSTEM_TYPE
            //                                                                            join sv in db.B_S_SYSTEM_VALUE on st.UUID equals sv.SYSTEM_TYPE_ID
            //                                                                            where st.TYPE == "Region" && sv.CODE == Region
            //                                                                            orderby sv.CODE
            //                                                                            select sv).FirstOrDefault().UUID
            //                        orderby sv.CODE
            //                        select sv;

            DaoSMMDataEntry daoSMMDataEntry = new DaoSMMDataEntry();

            List<SelectListItem> BCISDistrictResult = new List<SelectListItem>();


            foreach (var BCISitem in daoSMMDataEntry.GetBCISDistrict(Region))
            {
                BCISDistrictResult.Add(new SelectListItem
                {
                    Text = BCISitem.CODE + " - " + BCISitem.DESCRIPTION,
                    Value = BCISitem.CODE,

                });

            }
            //ViewBag.BCISDistrictResult = BCISDistrictResult;
            return Json(BCISDistrictResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SMM0104DataEntryEdit(ModelSVRecord sv)
        {
            var query = db.B_SV_RECORD.Where(x => x.REFERENCE_NO == sv.SubmissionNo);
            CommonFunction cf = new CommonFunction();
           //
           //if (query.Count() == 0)
           //{
           //    B_SV_RECORD b_SV_RECORD = new B_SV_RECORD();
           //    b_SV_RECORD.UUID = System.Guid.NewGuid().ToString();
           //    b_SV_RECORD.REFERENCE_NO = sv.SubmissionNo;
           //    b_SV_RECORD.RECEIVED_DATE = cf.StringToDateTime(sv.ReceivedDate);
           //    b_SV_RECORD.STATUS_CODE = "DATA_ENTRY_DRAFT";//plan to determine the status
           //    b_SV_RECORD.LANGUAGE_CODE = sv.FormLanguage;
           //
           //
           //    b_SV_RECORD.CREATED_BY = SystemParameterConstant.UserName;
           //    b_SV_RECORD.CREATED_DATE = System.DateTime.Now;
           //    b_SV_RECORD.MODIFIED_BY = SystemParameterConstant.UserName;
           //    b_SV_RECORD.MODIFIED_DATE = System.DateTime.Now;
           //
           //
           //    b_SV_RECORD.FORM_CODE = sv.FormCode;
           //
           //}
            return RedirectToAction("SMM0104");
        }
        [HttpPost]
        public JsonResult AutoCompleteStreetName(string Prefix)
       {
            //Note : you can bind same list from database  
            // var query = db.B_RV_STREET_NAME.Select(x => x.SM_NAME_CHN.Contains(Prefix)).GroupBy(x=>x.SM_NAME_CHN);

            //if (!String.IsNullOrEmpty(Prefix))
            //{
            //    if (Prefix.Any(c => (uint)c >= 0x4E00 && (uint)c <= 0x2FA1F))
            //    {
            //        var query = from sn in db.B_RV_STREET_NAME
            //                    where sn.SM_NAME_CHN.Contains(Prefix)
            //                    select new { StreetName = sn.SM_NAME_CHN };
            //        return Json(query.ToList(), JsonRequestBehavior.AllowGet);

            //    }
            //    else
            //    {
            //        var query = from sn in db.B_RV_STREET_NAME
            //                    where sn.SM_NAME_ENG.StartsWith(Prefix.ToUpper())
            //                    select new { StreetName = sn.SM_NAME_ENG };
            //        return Json(query.ToList(), JsonRequestBehavior.AllowGet);
            //    }


            //}
            DaoSMMDataEntry daoSMMDataEntry = new DaoSMMDataEntry();
            var List = daoSMMDataEntry.GetStreetNameByPrefix(Prefix);
      


            return Json(List, JsonRequestBehavior.AllowGet);
           /// return Json("", JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult AutoCompleteBuildingName(string Prefix)
        {
            DaoSMMDataEntry daoSMMDataEntry = new DaoSMMDataEntry();
            var List = daoSMMDataEntry.GetBuildingNameByPrefix(Prefix);



            return Json(List, JsonRequestBehavior.AllowGet);


            //    if (!String.IsNullOrEmpty(Prefix))
            //    {
            //        if (Prefix.Any(c => (uint)c >= 0x4E00 && (uint)c <= 0x2FA1F))
            //        {
            //            var query = from sn in db.B_RV_BLOCK
            //                        where sn.BK_BLDG_NAME_ENG_LINE_1.Contains(Prefix)
            //                        select new { BLDG_NAME = sn.BK_BLDG_NAME_ENG_LINE_1 };
            //            return Json(query.ToList(), JsonRequestBehavior.AllowGet);

            //        }
            //        else
            //        {
            //            var query = from sn in db.B_RV_BLOCK
            //                        where sn.BK_BLDG_NAME_CHN_LINE_1.StartsWith(Prefix.ToUpper())
            //                        select new { BLDG_NAME = sn.BK_BLDG_NAME_CHN_LINE_1 };
            //            return Json(query.ToList(), JsonRequestBehavior.AllowGet);
            //        }


            //    }
            //    return Json("", JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public JsonResult AutoCompleteFlat(string Prefix)
        {
            DaoSMMDataEntry daoSMMDataEntry = new DaoSMMDataEntry();
            var List = daoSMMDataEntry.GetFlatByPrefix(Prefix);



            return Json(List, JsonRequestBehavior.AllowGet);



            //if (!String.IsNullOrEmpty(Prefix))
            //{

            //       var query = (from sn in db.B_RV_UNIT
            //                    where sn.UT_NO_ENG.StartsWith(Prefix.ToUpper())
            //                    select new { FLAT = sn.UT_NO_ENG }).Distinct().OrderBy(x=>x.FLAT).Take(50);

            //        return Json(query.ToList(), JsonRequestBehavior.AllowGet);
            //}
            //return Json("", JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public JsonResult AutoCompleteFloor(string Prefix)
        {

            DaoSMMDataEntry daoSMMDataEntry = new DaoSMMDataEntry();
            var List = daoSMMDataEntry.GetFloorByPrefix(Prefix);



            return Json(List, JsonRequestBehavior.AllowGet);
            //if (!String.IsNullOrEmpty(Prefix))
            //{

            //        var query = (from sn in db.B_RV_UNIT
            //                    where sn.UT_FLR_DESC_ENG.StartsWith(Prefix.ToUpper())
            //                    select new { Floor = sn.UT_FLR_DESC_ENG }).Distinct().OrderBy(x=>x.Floor).Take(50);

            //    return Json(query.ToList(), JsonRequestBehavior.AllowGet);



            //}
            //return Json("", JsonRequestBehavior.AllowGet);

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
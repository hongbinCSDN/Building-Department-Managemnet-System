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
using System.Web.Configuration;
using MWMS2.Constant;

namespace MWMS2.Areas.Registration.Controllers
{
    public class Fn05MWIA_MWIAController : ValidationController
    {

        // GET: Registration/Fn05MWIA_MWIA
        public ActionResult Index(Fn05MWIA_MWIASearchModel model)
        {
            return View(model);
        }
        public ActionResult Search(Fn01Search_MWIASearchModel model)
        {
            RegistrationSearchService rs = new RegistrationSearchService();
            rs.SearchMWIA(model);
            return Content(JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        [HttpPost]
        public ActionResult Excel(FormCollection post, Dictionary<string, string>[] Columns)
        {
            if (post.AllKeys.Contains("key")) return DisplayGrid.getMemory(post["key"]);
            RegistrationSearchService rs = new RegistrationSearchService();
            return Json(new { key = rs.ExportMWIA(Columns, post) });
        }
        public ActionResult Form(string id)
        {
            SessionUtil.DraftObject = null;
            RegistrationSearchService rs = new RegistrationSearchService();
            return View("Form", rs.ViewMWIA(id));

            //return View();
        }
        public ActionResult Delete(string id)
        {


            RegistrationCommonService rs = new RegistrationCommonService();
            bool result = rs.CanDeleteApplication(id);
            if (result)
            {
                rs.DeleteApplication(id);
            }
            else
            {
                return Content(JsonConvert.SerializeObject("This application can not be deleted! Please check if any process monitors existing", Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");


            }
            return View("Index");


            //return View();
        }


        public ActionResult SaveValidation([Bind(Exclude = "")]Fn01Search_MWIADisplayModel model, IEnumerable<HttpPostedFileBase> UploadDoc)
        {

            if (ValidateResult.Result.Equals(ServiceResult.RESULT_FAILURE)) return Json(ValidateResult);
            else
            {
                return Json(new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS });
                //return Json(new  {  ServiceResult.RESULT_SUCCESS });
            }
        }
        //SaveBtn SaveValidation
        public ActionResult Save([Bind(Exclude = "")]Fn01Search_MWIADisplayModel model, IEnumerable<HttpPostedFileBase> UploadDoc)
        {

            if (UploadDoc == null || UploadDoc.ElementAt(0) == null)
            {
                RegistrationMWIAService rs = new RegistrationMWIAService();
                rs.SaveMWIA(model, UploadDoc);
            }
            else
            {
                var supportedTypes = new[] { "pdf", "gif", "jpg" };
                var fileExt = "";
                if (UploadDoc.ElementAt(0) != null)
                {
                    fileExt = System.IO.Path.GetExtension(UploadDoc.ElementAt(0).FileName).Substring(1);
                }



                if (!supportedTypes.Contains(fileExt))
                {
                    // RegistrationMWIAService rs = new RegistrationMWIAService();
                    // rs.SaveMWIA(model, UploadDoc);

                    model.result = false;
                    model.ErrMsg = "Invalid file type, please confrim the file type.";
                    return View("Form", model);
                }
                else
                {

                    RegistrationMWIAService rs = new RegistrationMWIAService();
                    rs.SaveMWIA(model, UploadDoc);
                }

            }

            return View("Index");

        }
        public ActionResult displayApplicantHistory(string id)
        {
            RegistrationCommonService rs = new RegistrationCommonService();
            return Content(JsonConvert.SerializeObject(rs.displayApplicantHistory(id), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");

        }

        public ActionResult GetQualificationByIndUuid(Fn01Search_PADisplayModel model)
        {
            RegistrationCommonService rs = new RegistrationCommonService();
            return Content(JsonConvert.SerializeObject(rs.GetQualificationByIndUuid(model.C_IND_APPLICATION.UUID), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        public ActionResult TabPageIndQualification(string id)
        {

            QualifcationDisplayListModel model = new QualifcationDisplayListModel();
            RegistrationCommonService rs = new RegistrationCommonService();
            if (string.IsNullOrWhiteSpace(id))
            {
            }
            else
            {
                model = rs.GetQualificationByUUID(id);
            }
            return View("TabPageIndQualification", model);
        }

        public ActionResult GenerateSerialNo(string id)
        {
            RegistrationMWIAService rs = new RegistrationMWIAService();


            string serial_no = "";
            string expiryDt = "";
            rs.GenerateSerialNo(id, ref serial_no, ref expiryDt);
            var result = new
            {
                SERIAL_NO = serial_no,
                EXPIRY_DATE = expiryDt,

            };
            return Content(JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");

            // return View("Form", model);



        }
        public ActionResult GetApplicantById(string hkid, string passportno)
        {
            RegistrationSearchService rs = new RegistrationSearchService();

            ApplicantDisplayModel model = new ApplicantDisplayModel();
            model = rs.GetApplicant(model, hkid, passportno);

            return Content(JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");

            // return View("Form", model);



        }
        public ActionResult PopupSURNAMEDetails(SurnameDetailModel model)
        {

            return View(model);
        }

        public ActionResult SurnameSearch(SurnameDetailModel model)
        {
            RegistrationCommonService s = new RegistrationCommonService();

            return Content(JsonConvert.SerializeObject(s.SearchSurname(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        public ActionResult AddDraftQualification(QualifcationDisplayListModel model)
        {
            RegistrationCommonService rs = new RegistrationCommonService();
            return Content(JsonConvert.SerializeObject(rs.AddDraftQualification(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        public ActionResult DeleteQualification(string id)
        {
            RegistrationCommonService rs = new RegistrationCommonService();
            return Content(JsonConvert.SerializeObject(rs.DeleteQualification(id), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        public ActionResult AddDraftMWItem(IndMWItemDisplayModel model)
        {
            RegistrationCommonService rs = new RegistrationCommonService();
            //if (model.MWItemSaveVersion == "New")
            //{
            //    model.SelectedMWItemDetail.m_MWItemSaveVersion = model.MWItemSaveVersion;
            //    return Content(JsonConvert.SerializeObject(rs.AddNewDraftMWItem(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");

            //}
            //else
            //{
            // model.SelectedMWItemDetail.m_MWItemSaveVersion = model.MWItemSaveVersion;
            //model.SelectedMWItemDetail.m_NewSelectedMWitem = model.NewSelectedMWitem ;
            //model.SelectedMWItemDetail.m_NewSelectedMWitemSupportedBy = new Dictionary<string, string>();
            //foreach (var item in model.NewSelectedMWitemSupportedBy)
            //{
            //    model.SelectedMWItemDetail.m_NewSelectedMWitemSupportedBy.Add(item.Key, item.Value);
            //}

            return Content(JsonConvert.SerializeObject(rs.AddDraftMWItemTest(model.SelectedMWItemDetail), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
            //}
        }
        public ActionResult DeleteMWItem(string id)
        {
            RegistrationCommonService rs = new RegistrationCommonService();
            return Content(JsonConvert.SerializeObject(rs.DeleteMWItem(id), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        public ActionResult TabPageIndMWItem(string id)
        {

            IndMWItemDisplayModel model = new IndMWItemDisplayModel();
            RegistrationCommonService rs = new RegistrationCommonService();
            if (string.IsNullOrWhiteSpace(id))
            {

            }
            else
            {

                model = rs.GetMWItemListByIndUUID(id);
            }

            //if (string.IsNullOrWhiteSpace(id))
            //{
            //}
            //else
            //{
            //    model = rs.GetMWItemListByIndUUID(id);

            //}
            return View("TabPageIndMWItem", model);
        }
        public ActionResult TabPageEditIndMWItem(string id, string ApplicationUUID)
        {

            IndMWItemDisplayModel model = new IndMWItemDisplayModel();
            RegistrationCommonService rs = new RegistrationCommonService();
            if (string.IsNullOrWhiteSpace(id))
            {

                model = rs.GetMWItemListByIndUUID(ApplicationUUID);
                model.SelectedMWItemDetail.m_Master_ID = ApplicationUUID;
            }
            else
            {

                model = rs.GetMWItemListByUUID(id, ApplicationUUID);

                model.SelectedMWItemDetail.m_UUID = id;
            }
            return View("TabPageEditIndMWItem", model);
        }
        public ActionResult PopupHKIDDetails(HKIDPASSPORTDetailModel model)
        {
            if (model.HKID == "null")
            {
                model.HKID = null;
            }
            if (model.PASSPORT == "null")
            {
                model.PASSPORT = null;
            }
            return View(model);
        }
        public ActionResult HKIDSearchComp(HKIDPASSPORTDetailModel model)
        {
            RegistrationCommonService s = new RegistrationCommonService();

            return Content(JsonConvert.SerializeObject(s.SearchHKIDComp(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        public ActionResult HKIDSearchInd(HKIDPASSPORTDetailModel model)
        {
            RegistrationCommonService s = new RegistrationCommonService();

            return Content(JsonConvert.SerializeObject(s.SearchHKIDInd(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        public ActionResult HKIDSearchQP(HKIDPASSPORTDetailModel model)
        {
            RegistrationCommonService s = new RegistrationCommonService();

            return Content(JsonConvert.SerializeObject(s.SearchHKIDQP(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        public ActionResult GetSignImg(string uuid)
        {

            CommonBLService ss = new CommonBLService();

            var file = ss.ViewCRMImageByUUID(uuid);

            if (file == null)
            {
                return Content("File not found.");
            }
            else
            {
                return file;
            }
            //RegistrationCommonService rs = new RegistrationCommonService();
            //if (filepath.Contains("jpg"))
            //{
            //    return new FileContentResult(rs.DownloadDocBySubpath(uuid), System.Net.Mime.MediaTypeNames.Image.Jpeg);

            //}
            //else
            //{
            //    return new FileContentResult(rs.DownloadDocBySubpath(uuid), System.Net.Mime.MediaTypeNames.Image.Gif);

            //}


        }
        public ActionResult DelDoc(string id)
        {
            RegistrationMWIAService rs = new RegistrationMWIAService();
            rs.DelDoc(id);
            return Content(JsonConvert.SerializeObject("", Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        public ActionResult WindowMWCWEForm(string FILE_REF)
        {
            RegistrationEformService rs = new RegistrationEformService();
            EformModel model = rs.initialiseEformModel(FILE_REF, "C_EFSS_MWCW");
            return View("EformSelection", rs.populateEformSelection(model));
        }

        [HttpPost]
        public ActionResult EformEfssMWCW(string Uuid, string RefNo, string FileType)
        {
            RegistrationEformService res = new RegistrationEformService();
            var result = res.getDatafromEFSSMWCW(Uuid, RefNo, FileType);
            return Content(JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        [HttpPost]
        public ActionResult UpdateEformStatus(string UUID, string STATUS)
        {
            RegistrationEformService res = new RegistrationEformService();
            var result = res.updateEformStatus_MWCW(UUID, STATUS);
            //return;
            return Content(JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        public ActionResult getMWItemCapByRefNoAndFormNo(string refNo, string FormNo)
        {
            RegistrationMWIAService rs = new RegistrationMWIAService();

            return Content(JsonConvert.SerializeObject(rs.getMWItemCapByRefNoAndFormNo(refNo, FormNo), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");


        }


    }
}
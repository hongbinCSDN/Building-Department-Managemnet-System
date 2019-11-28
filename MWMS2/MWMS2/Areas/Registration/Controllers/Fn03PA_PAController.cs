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


namespace MWMS2.Areas.Registration.Controllers
{
    public class Fn03PA_PAController : ValidationController
    {
        // GET: Registration/Fn03PA_PA
        public ActionResult Index(Fn03PA_PASearchModel model)
        {
            return View(model);
        }
        public ActionResult ProcessMonitor(string RefNo)
        {
            ProcessMonitorSearchModel model = new ProcessMonitorSearchModel();
            model.FileRef = RefNo;
            return  RedirectToAction("Index", "Fn03PA_PM", model);
        }
        public ActionResult UpdateApplicationStatus(string RefNo)
        {
            UpdateAppStatusSearchModel model = new UpdateAppStatusSearchModel();
            model.FileRef = RefNo;
            return RedirectToAction("Index", "Fn03PA_UAS", model);
        }
        public ActionResult Search(Fn03PA_PASearchModel model)
        {
            RegistrationPAService rs = new RegistrationPAService();
            rs.SearchPA(model);
            return Content(JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        [HttpPost]
        public ActionResult Excel(Fn03PA_PASearchModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            RegistrationPAService rs = new RegistrationPAService();
            return Json(new { key = rs.ExportPA(model) });
            //  return Json(new { key = rs.ExportPA(model) });
        }

        public ActionResult Form(string uuid)
        {
            SessionUtil.DraftObject = null;
            RegistrationSearchService rs = new RegistrationSearchService();
 
            return View("Form", rs.ViewPA(uuid));
           

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
            return View("Index");
     
        }
        public ActionResult GetSelectedCertiDetails(string id)
        {
            RegistrationSearchService rs = new RegistrationSearchService();
            var tmp = rs.GetSelectedCertiDetails(id);
            DateTime? TmpExpiryDate;
            TmpExpiryDate = tmp.CARD_EXPIRY_DATE == null ? tmp.EXPIRY_DATE : tmp.CARD_EXPIRY_DATE;


            var result = new
            {
          
                EXPIRY_DATE      = TmpExpiryDate == null ? "" : TmpExpiryDate.Value.ToShortDateString(),
                APP_DATE         = tmp.CARD_APP_DATE == null ? "" : tmp.CARD_APP_DATE.Value.ToShortDateString(),
                ISSUE_DATE       = tmp.CARD_ISSUE_DATE == null ? "" : tmp.CARD_ISSUE_DATE.Value.ToShortDateString(),
                SERIAL_NO        = tmp.CARD_SERIAL_NO,
                RETURN_DATE      =  tmp.CARD_RETURN_DATE == null ? "" : tmp.CARD_RETURN_DATE.Value.ToShortDateString()
            };
            return Content(JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");

           

            //return View();
        }
        //public ActionResult SaveTempPRB([Bind(Exclude = "")]Fn01Search_PADisplayModel model)
        //{
         

        //    RegistrationPAService rs = new RegistrationPAService();
        //    rs.SaveTempPRB(model);
        //    RegistrationSearchService ss = new RegistrationSearchService();
        //    return View("Form", ss.ViewPA(model.C_IND_APPLICATION.UUID));
        //    //return View("Form",model);
        //}


        public ActionResult AddDraftQualification(QualifcationDisplayListModel model)
        {
            if (ValidateResult.Result.Equals(ServiceResult.RESULT_FAILURE)) return Json(ValidateResult);
            else
            {
                RegistrationCommonService rs = new RegistrationCommonService();
                return Content(JsonConvert.SerializeObject(rs.AddDraftQualification(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
            }
        }
        //public ActionResult QualificationForm(Fn01Search_PADisplayModel m,string id)
        //{
        //    RegistrationSearchService rs = new RegistrationSearchService();
        //    var model = rs.ViewPA(m.C_IND_APPLICATION.UUID);
        //    RegistrationPAService ss = new RegistrationPAService();
        //    model = ss.GetQualificationForm(model);

        //    return View("Form", model);
        //}
        public ActionResult GetQualificationByIndUuid(Fn01Search_PADisplayModel model)
        {
            RegistrationCommonService rs = new RegistrationCommonService();
            return Content(JsonConvert.SerializeObject(rs.GetQualificationByIndUuid(model.C_IND_APPLICATION.UUID), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        public ActionResult GetCertificateByIndUuid(Fn01Search_PADisplayModel model)
        {
            RegistrationPAService rs = new RegistrationPAService();
            return Content(JsonConvert.SerializeObject(rs.GetCertificateByIndUuid(model.C_IND_APPLICATION.UUID), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        
        public ActionResult GetCatCodeDetail(string catCode)
        {
            RegistrationPAService rs = new RegistrationPAService();
            return Content(JsonConvert.SerializeObject(rs.GetCatCodeDetailByCode(catCode), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");


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

        public ActionResult displayApplicantHistory(string id)
        {

            RegistrationCommonService rs = new RegistrationCommonService();
           


           return Content(JsonConvert.SerializeObject(rs.displayApplicantHistory(id), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");

        }


        public ActionResult DeleteQualification(string id)
        {
            RegistrationCommonService rs = new RegistrationCommonService();
            return Content(JsonConvert.SerializeObject(rs.DeleteQualification(id), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        public ActionResult DeleteCertificate(string id)
        {
            RegistrationPAService rs = new RegistrationPAService();
            return Content(JsonConvert.SerializeObject(rs.DeleteCertificate(id), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");

        }
        public ActionResult TabPageIndCertificate(string id)
        {

            CertificateDisplayListModel model = new CertificateDisplayListModel();
            RegistrationPAService rs = new RegistrationPAService();
            if (string.IsNullOrWhiteSpace(id))
            {
            }
            else
            {
                model = rs.GetCertificateByUuid(id);
            }
            return View("TabPageIndCertificate", model);
        }
        public ActionResult TabPageIndCertificateTable(string id)
        {

            List<CertificateDisplayListModel> model = new List<CertificateDisplayListModel>();
            RegistrationPAService rs = new RegistrationPAService();
            //if (string.IsNullOrWhiteSpace(id))
            //{

            //}
            //else
            //{
            //    model = rs.GetCertificateByIndUuid(id);
            //}

            model = rs.GetCertificateByIndUuid(id);
            return View("TabPageIndCertificateTable", model);
        }
        public ActionResult AddDraftCertificate(CertificateDisplayListModel model)
        {

            if (ValidateResult.Result.Equals(ServiceResult.RESULT_FAILURE)) return Json(ValidateResult);
            var supportedTypes = new[] { "gif", "jpg" };
            var fileExt = "";
            if (model.UploadDoc.ElementAt(0) != null)
            {
                fileExt = System.IO.Path.GetExtension(model.UploadDoc.ElementAt(0).FileName).Substring(1);
            }



            if (!supportedTypes.Contains(fileExt) && model.UploadDoc.ElementAt(0) != null)
            {



                //      model.result = false;
                //   model.ErrMsg = "Invalid file type, please confrim the file type.";
                return Content(JsonConvert.SerializeObject(new ServiceResult() { Result = "Failed",Message = new List<string> { "Invalid file type, please confrim the file type." } }, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");

                // return View("TabPageIndCertificate", model);
            }
            else
            {


                RegistrationPAService rs = new RegistrationPAService();
                return Content(JsonConvert.SerializeObject(rs.AddDraftCertificate(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");

            }
        }
        [HttpPost]
        public ActionResult AddEformCertificate(string EFSS_PROFESSIONAL_UUID, string FILE_REFNO)
        {
            RegistrationPAService rs = new RegistrationPAService();
            return Content(JsonConvert.SerializeObject(rs.AddEformCertificate(EFSS_PROFESSIONAL_UUID, FILE_REFNO), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");

        }

        public ActionResult Save([Bind(Exclude = "")]Fn01Search_PADisplayModel model, IEnumerable<HttpPostedFileBase> UploadDoc)
        {
            RegistrationPAService rs = new RegistrationPAService();
            rs.SavePA(model, UploadDoc);
            Fn03PA_PASearchModel searchModel = new Fn03PA_PASearchModel();
            return View("Index", searchModel);          
        }

        public ActionResult SaveValidation([Bind(Exclude = "")]Fn01Search_PADisplayModel model, IEnumerable<HttpPostedFileBase> UploadDoc)
        {

            if (ValidateResult.Result.Equals(ServiceResult.RESULT_FAILURE)) return Json(ValidateResult);
            else
            {
                return Json(new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS });
            }
        }


        //public ActionResult GetApplicantById(string id,string hkid, string passportno)
        //{
        //    RegistrationSearchService rs = new RegistrationSearchService();
        //    var model = rs.ViewPA(id);

        //    model = rs.GetApplicantById(model, hkid, passportno);

        //    return View("Form", model);



        //}

        //public FileResult GetSignImg(string filepath)
        //{
        //    RegistrationCommonService rs = new RegistrationCommonService();
        //    if (filepath.Contains("jpg"))
        //    {
        //        return File(rs.DownloadDocBySubpath(filepath), System.Net.Mime.MediaTypeNames.Image.Jpeg, "Sign.jpg");

        //    }
        //    else
        //    {
        //        return File(rs.DownloadDocBySubpath(filepath), System.Net.Mime.MediaTypeNames.Image.Gif, "Sign.gif");

        //    }

        //}
        public ActionResult GetSignImg(string uuid)
        {

            CommonBLService ss = new CommonBLService();

            var file = ss.ViewCRMImageByUUID(uuid);
            //rs.ViewCRMImageByUUID(uuid);

            if (file == null)
            {
                return Content("File not found.");
            }
            else
            {
                return file;
            }

        }
        public ActionResult GetApplicantById(string hkid, string passportno)
        {
            RegistrationSearchService rs = new RegistrationSearchService();

            ApplicantDisplayModel model = new ApplicantDisplayModel();
            model = rs.GetApplicant(model, hkid, passportno);

            return Content(JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");

            // return View("Form", model);



        }
        public ActionResult GenerateSerialNo(string id)
        {
            RegistrationPAService rs = new RegistrationPAService();


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
        public ActionResult PopupHKIDDetails(HKIDPASSPORTDetailModel model)
        {
        //    if (model.HKID == null)
        //    {
        //        int a = 3;
        //    }
        //    if (model.HKID == "null")
        //    {
        //        model.HKID = null;
        //    }
        //    if (model.PASSPORT == "null")
        //    {
        //        model.PASSPORT = null;
        //    }
            return View(model);
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

        public ActionResult WindowProfEForm(string FILE_REF)
        {
            RegistrationEformService rs = new RegistrationEformService();
            EformModel model = rs.initialiseEformModel(FILE_REF, "C_EFSS_PROFESSIONAL");
            return View("EformSelection", rs.populateEformSelection(model));
        }

        [HttpPost]
        public ActionResult EformEfssProfessional(string Uuid, string RefNo, string FileType)
        {
            RegistrationEformService res = new RegistrationEformService();
            var result = res.getDatafromEFSSPROFESSIONAL(Uuid, RefNo, FileType);
            return Content(JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        [HttpPost]
        public ActionResult UpdateEformStatus(string UUID, string STATUS)
        {
            RegistrationEformService res = new RegistrationEformService();
            var result = res.updateEformStatus_PROFESSIONAL(UUID, STATUS);
            //return;
            return Content(JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
    }




}
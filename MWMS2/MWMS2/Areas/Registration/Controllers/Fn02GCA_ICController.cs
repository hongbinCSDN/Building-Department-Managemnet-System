using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Services;
using Newtonsoft.Json;
using MWMS2.Areas.Registration.Models;
using MWMS2.Constant;
using MWMS2.Utility;
using MWMS2.Entity;
using System.Text;
using System.IO;

namespace MWMS2.Areas.Registration.Controllers
{
    public class Fn02GCA_ICController : Controller
    {

        public virtual string registrationType()
        {
            return RegistrationConstant.REGISTRATION_TYPE_CGA;
        }

        // GET: Registration/Fn02GCA_IC
        public virtual ActionResult Index(CompICModel model)
        {
          
            model.RegType = registrationType();
            return View("SearchCompIC", model);
        }

        public virtual ActionResult Search(CompICModel model)
        {
            RegistrationCompICService rs = new RegistrationCompICService();
            rs.SearchIC_CGC(model);
            return Content(JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        [HttpPost]
        public ActionResult Excel(CompICModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            RegistrationCompICService rs = new RegistrationCompICService();
           
            return Json(new { key = rs.ExportIC_CGC(model).ExportKey });
        }
        public ActionResult Form(string meetingId)
        {
            SessionUtil.DraftObject = null;
            string regType = registrationType();
            RegistrationCompICService rs = new RegistrationCompICService();
            CompICModel model = rs.Form(meetingId, regType);
            return View("FormCompIC", model);
        }

        public ActionResult AjaxCandidates(CompICModel model)
        {
            RegistrationCompICService rs = new RegistrationCompICService();
            DisplayGrid r = rs.AjaxCandidates(Request["C_INTERVIEW_SCHEDULE.UUID"], registrationType());
            return Content(JsonConvert.SerializeObject(r, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        public ActionResult AddtoList(string candidateNo, string duration)
        {
                string regType = registrationType();
                RegistrationCompICService rs = new RegistrationCompICService();
                return Content(JsonConvert.SerializeObject(rs.AddtoList(duration, candidateNo, regType, Request["C_INTERVIEW_SCHEDULE.UUID"]), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        public ActionResult GenCandidates(CompICModel model)
        {
            RegistrationCompICService rs = new RegistrationCompICService();
            ServiceResult r = rs.GenCandidates(model, registrationType());
            return Content(JsonConvert.SerializeObject(r, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }



        public virtual ActionResult Save(CompICModel model)
        {
            RegistrationCompICService rs = new RegistrationCompICService();
            ServiceResult r = rs.Save(model, registrationType());
            return Content(JsonConvert.SerializeObject(r, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        public ActionResult DeleteSession(int RecordId)
        {
            RegistrationCompICService rs = new RegistrationCompICService();
          //  rs.DeteSession(RecordId);
            //List<string> draftList = SessionUtil.DraftList<string>(RecordId);
            //  draftList.Add(RecordId);
            string regType = registrationType();
            return Content(JsonConvert.SerializeObject(rs.DeteSession(RecordId, regType), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");

            //return RedirectToAction("/Index");
        }

        public ActionResult Delete(string meetingNo)
        {
            RegistrationCompICService rs = new RegistrationCompICService();
           // rs.DeleteForm(meetingNo);
            //return RedirectToAction("/Index");
            return Content(JsonConvert.SerializeObject(rs.DeleteForm(meetingNo), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");

        }

        //not sure use or not
        public ActionResult SearchCandidateNo(CompICModel model)
        {
            RegistrationCompICService rs = new RegistrationCompICService();
            rs.SearchCandidateNoAddRecord(model);
            return Content(JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        //[HttpPost]
        //public ActionResult Excel(FormCollection post, Dictionary<string, string>[] Columns)
        //{
        //    if (post.AllKeys.Contains("key")) return DisplayGrid.getMemory(post["key"]);
        //    RegistrationGCAService rs = new RegistrationGCAService();
        //    return Json(new { key = rs.ExportTemp(Columns, post) });
        //}

        // Export Comp certificate
        public FileStreamResult ExportCRCMinutes(CompICModel dataExport, String MeetingNumber, string regType)
        {
            RegistrationCompICService rs = new RegistrationCompICService();
            dataExport.RegType = regType;
            string content = rs.ExportCRCMinute(dataExport, MeetingNumber);
            var byteArray = Encoding.UTF8.GetBytes(content);
            var stream = new MemoryStream(byteArray);
            return File(stream, "text/plain", "intevCand.txt");

        }

        // Export Comp certificate
        public FileStreamResult ExportMeetingGroupCommittee(CompICModel dataExport, string MeetingNumber, string regType)
        {
            RegistrationCompICService rs = new RegistrationCompICService();
            dataExport.RegType = regType;
            string content = rs.ExportMeetingGroupCommittee(dataExport, MeetingNumber);
            var byteArray = Encoding.UTF8.GetBytes(content);
            var stream = new MemoryStream(byteArray);
            return File(stream, "text/plain", "ExportMeetingGroupCommittee.txt");
        }

        // Export Comp certificate
        public FileStreamResult ExportRenewalRestoration(CompICModel dataExport, string UUID)
        {
            RegistrationCompICService rs = new RegistrationCompICService();
            dataExport.RegType = registrationType();
            string content = rs.ExportRenewalRestoration(dataExport, UUID);
            var byteArray = Encoding.UTF8.GetBytes(content);
            var stream = new MemoryStream(byteArray);
            return File(stream, "text/plain", "Renewal_Restoration.txt");
        }

        // Export Comp certificate
        public FileStreamResult ExportInterview(CompICModel dataExport, string UUID)
        {
            RegistrationCompICService rs = new RegistrationCompICService();
            dataExport.RegType = registrationType();
            string content = rs.ExportInterview(dataExport, UUID);
            var byteArray = Encoding.UTF8.GetBytes(content);
            var stream = new MemoryStream(byteArray);
            return File(stream, "text/plain", "Register_for_Interview.txt");
        }

        // Export Comp certificate
        public FileStreamResult ExportAssessment(CompICModel dataExport, string UUID)
        {
            RegistrationCompICService rs = new RegistrationCompICService();
            dataExport.RegType = registrationType();
            string content = rs.ExportAssessment(dataExport, UUID);
            var byteArray = Encoding.UTF8.GetBytes(content);
            var stream = new MemoryStream(byteArray);
            return File(stream, "text/plain", "Register_for_Assessment.txt");
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Save(CompICModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        RegistrationCompICService rs = new RegistrationCompICService();
        //        //RegistrationCNVService rs = new RegistrationCNVService();
        //        rs.SaveIC(model);
        //        return RedirectToAction("/Index");
        //    }
        //    return View(model);
        //}

    }
}
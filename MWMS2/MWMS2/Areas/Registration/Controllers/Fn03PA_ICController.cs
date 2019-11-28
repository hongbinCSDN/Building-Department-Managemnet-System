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
using MWMS2.Constant;

namespace MWMS2.Areas.Registration.Controllers
{
    public class Fn03PA_ICController : Fn02GCA_ICController
    {
        public override string registrationType()
        {
            return RegistrationConstant.REGISTRATION_TYPE_IP;
        }

        public override ActionResult Index(CompICModel model)
        {
          
            model.RegType = registrationType();
            return View("SearchCompIC", model);
        }

        public override ActionResult Search(CompICModel model)
        {
            RegistrationCompICService rs = new RegistrationCompICService();
            rs.SearchIC_IP(model);
            return Content(JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        //public ActionResult Form(string id)
        //{
        //    RegistrationCompICService rs = new RegistrationCompICService();
        //    CompICModel model = rs.Form(id);
        //    return View("FormCompIC", model);
        //}


        //public ActionResult AddtoList(string candidateNo, string duration)
        //{   
        //    string regType = RegistrationConstant.REGISTRATION_TYPE_IP;
        //    RegistrationCompICService rs = new RegistrationCompICService();
        //    return Content(JsonConvert.SerializeObject(rs.AddtoList(duration, candidateNo, regType, Request["C_INTERVIEW_SCHEDULE.UUID"]), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");

        //}

        //public ActionResult AjaxCandidates(CompICModel model)
        //{
        //    RegistrationCompICService rs = new RegistrationCompICService();
        //    DisplayGrid r = rs.AjaxCandidates(Request["C_INTERVIEW_SCHEDULE.UUID"], RegistrationConstant.REGISTRATION_TYPE_IP);
        //    return Content(JsonConvert.SerializeObject(r, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        //}

        //[HttpPost]
        //public ActionResult Excel(FormCollection post, Dictionary<string, string>[] Columns)
        //{
        //    if (post.AllKeys.Contains("key")) return DisplayGrid.getMemory(post["key"]);
        //    RegistrationGCAService rs = new RegistrationGCAService();
        //    return Json(new { key = rs.ExportTemp(Columns, post) });
        //}


        //public FileStreamResult ExportCRCMinutes(CompICModel dataExport, String MeetingNumber)
        //{
        //    //RegistrationDataExportService rs = new RegistrationDataExportService();
        //    RegistrationCompICService rs = new RegistrationCompICService();
        //    dataExport.RegType = registrationType();
        //    return rs.ExportCRCMinute(MeetingNumber);
        //}

        //// Export Comp certificate
        //public FileStreamResult ExportMeetingGroupCommittee(CompICModel dataExport, String MeetingNumber)
        //{
        //    //RegistrationDataExportService rs = new RegistrationDataExportService();
        //    RegistrationCompICService rs = new RegistrationCompICService();
        //    dataExport.RegType = registrationType();
        //    return rs.ExportMeetingGroupCommittee(MeetingNumber);
        //}

    }
}
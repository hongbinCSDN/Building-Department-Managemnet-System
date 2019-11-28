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
    public class Fn05MWIA_ICController : Fn02GCA_ICController
    {
        public override string registrationType()
        {
            return RegistrationConstant.REGISTRATION_TYPE_MW;
        }

        public override ActionResult Index(CompICModel model)
        {

            model.RegType = registrationType();
            return View("SearchCompIC", model);
        }

        public override ActionResult Search(CompICModel model)
        {
            RegistrationCompICService rs = new RegistrationCompICService();
            rs.SearchIC_MW(model);
            return Content(JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        public override ActionResult Save(CompICModel model)
        {
            RegistrationCompICService rs = new RegistrationCompICService();
            rs.GenCandidates(model, registrationType());
            ServiceResult r = rs.Save(model, registrationType());
            return Content(JsonConvert.SerializeObject(r, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        //// GET: Registration/Fn05MWIA_IC
        //public ActionResult Index()
        //{
        //    CompICModel model = new CompICModel();
        //    return View("Index", model);
        //}

        //public ActionResult Search(CompICModel model)
        //{
        //    RegistrationCompICService rs = new RegistrationCompICService();
        //    rs.SearchIC_MW(model);
        //    return Content(JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        //}

        //public ActionResult Form(string id)
        //{
        //    string 
        //    RegistrationCompICService rs = new RegistrationCompICService();
        //    CompICModel model = rs.Form(id);
        //    return View("FormCompIC", model);
        //}



        //public ActionResult AjaxCandidates(CompICModel model)
        //{
        //    RegistrationCompICService rs = new RegistrationCompICService();
        //    DisplayGrid r = rs.AjaxCandidates(Request["C_INTERVIEW_SCHEDULE.UUID"], RegistrationConstant.REGISTRATION_TYPE_MW);
        //    return Content(JsonConvert.SerializeObject(r, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        //}

        //public ActionResult AddtoList(string candidateNo, string duration)
        //{
        //    string regType = RegistrationConstant.REGISTRATION_TYPE_MW;
        //    RegistrationCompICService rs = new RegistrationCompICService();
        //    ServiceResult r = rs.AddtoList(duration, candidateNo, regType, Request["C_INTERVIEW_SCHEDULE.UUID"]);
        //    return Content(JsonConvert.SerializeObject(r, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        //}

        //[HttpPost]
        //public ActionResult Excel(FormCollection post, Dictionary<string, string>[] Columns)
        //{
        //    if (post.AllKeys.Contains("key")) return DisplayGrid.getMemory(post["key"]);
        //    RegistrationMWIAService registrationMWIAService = new RegistrationMWIAService();
        //    return Json(new { key = registrationMWIAService.ExportMWIA(Columns, post) });
        //}

        //public DisplayGrid demoSearch()
        //{
        //    DisplayGrid dlr = new DisplayGrid();


        //    //dlr.Columns = new string[] { "Meeting No", "Interview Date", "Session", "Room", "Canceled" };
        //    //dlr.Datas = new List<object[]>();
        //    ////dlr.Datas.Add(new object[] { "v11", "v12", "v13", "v14", "v25" });
        //    ////dlr.Datas.Add(new object[] { "v21", "v22", "v23", "v24", "v35" });
        //    ////dlr.Datas.Add(new object[] { "v31", "v32", "v33", "v34", "v45" });

        //    dlr.Rpp = 25;
        //    ////dlr.TotalRecord = 3;
        //    ////dlr.CurrentPage = 1;
        //    return dlr;
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
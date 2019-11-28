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
using System.Globalization;

namespace MWMS2.Areas.Registration.Controllers
{
    public class Fn03PA_MRAController : ValidationController
    {
        public ActionResult Index(Fn02GCA_MRASearchModel mRASearchModel)
        {
        
            return View("SearchMRA", mRASearchModel);

        }
        public ActionResult Save(MeetingRoomDisplayModel model)
        {
            RegistrationCommonService ss = new RegistrationCommonService();
            if (ValidateResult.Result.Equals(ServiceResult.RESULT_FAILURE)) return Json(ValidateResult);
            ServiceResult serviceResult = ss.SaveMRA(model);

            return Json(serviceResult);
        }
        public ActionResult AjaxMemberList(MeetingRoomDisplayModel model)
        {
            RegistrationCommonService ss = new RegistrationCommonService();
            if (model.SearchBy == "O")
            {
                return Content(JsonConvert.SerializeObject(ss.SearchMemberBySurnameAndGivenName(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
            }
            else
            {

                return Content(JsonConvert.SerializeObject(ss.SearchMemberByCommitteeGroup(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");

            }
        }
        public ActionResult AjaxDeleteDrafttoMemberList(MeetingRoomDisplayModel model)
        {
            RegistrationCommonService ss = new RegistrationCommonService();


            return PartialView("CommitteeMemberListTable", ss.AjaxDeleteDrafttoMemberList(model));

            //  return Content(JsonConvert.SerializeObject(ss.SearchMemberByCommitteeGroup(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        public ActionResult AjaxAddDrafttoMemberList(MeetingRoomDisplayModel model)
        {
            RegistrationCommonService ss = new RegistrationCommonService();



            return PartialView("CommitteeMemberListTable", ss.AjaxAddDrafttoMemberList(model));
        }
        public ActionResult AjaxAvailableRoom(Fn02GCA_MRASearchModel model)
        {
            // RegistrationGCAService rs = new RegistrationGCAService();

            // return Content(JsonConvert.SerializeObject(rs.SearchMRAAvailable(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
            RegistrationCommonService ss = new RegistrationCommonService();

            return Content(JsonConvert.SerializeObject(ss.SearchMRAAvailable(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");

        }
        public ActionResult Available_Form(string rid, string sid, string IntDate, string id)
        {
            SessionUtil.DraftObject = null;
            RegistrationCommonService rs = new RegistrationCommonService();
            MeetingRoomDisplayModel model = new MeetingRoomDisplayModel();
            model.RegType = RegistrationConstant.REGISTRATION_TYPE_IP;
            model.C_INTERVIEW_SCHEDULE.ROOM_ID = rid;
            model.C_INTERVIEW_SCHEDULE.TIME_SESSION_ID = sid;
            model.C_INTERVIEW_SCHEDULE.INTERVIEW_DATE = DateTime.ParseExact(IntDate.Substring(0, 10), "yyyy-MM-dd", CultureInfo.InvariantCulture);
            return View("FormMRA", rs.GetMRADisplayDetail(id, model,""));
        }
        public ActionResult Form(string id, string Meeting_number)
        {
            SessionUtil.DraftObject = null;
            RegistrationCommonService rs = new RegistrationCommonService();
            MeetingRoomDisplayModel model = new MeetingRoomDisplayModel();
            model.RegType = RegistrationConstant.REGISTRATION_TYPE_IP;
            return View("FormMRA", rs.GetMRADisplayDetail(id, model, Meeting_number));
        }
        public ActionResult LoadMember(string id)
        {
            SessionUtil.DraftObject = null;
            RegistrationCommonService rs = new RegistrationCommonService();

            return PartialView("CommitteeMemberListTable", rs.LoadMember(id));
        }
        public ActionResult LoadGroup(MeetingRoomDisplayModel model)
        {
       
            IEnumerable<SelectListItem> x = model.GetCommitteeGroup;

            return Json(x);


        }


        public ActionResult Search(Fn02GCA_MRASearchModel model)
        {
            RegistrationGCAService rs = new RegistrationGCAService();

            return Content(JsonConvert.SerializeObject(rs.SearchMRA(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        [HttpPost]
        public ActionResult Excel(Fn02GCA_MRASearchModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            RegistrationGCAService rs = new RegistrationGCAService();

            return Json(new { key = rs.ExportMRA(model).ExportKey });
        }
        //// GET: Registration/Fn03PA_MRA
        //public ActionResult Index()
        //{
        //    return View();
        //}
        ////public JsonResult Search()
        ////{
        ////    DisplayGrid dlr = demoSearch();
        ////    return Json(dlr);
        ////}

        //public ActionResult Search(Fn03PA_MRASearchModel model)
        //{
        //    RegistrationPAService rs = new RegistrationPAService();
        //    rs.SearchMRA(model);
        //    return Content(JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        //}
        //[HttpPost]
        //public ActionResult Excel(Fn01Search_PASearchModel model)
        //{
        //    if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
        //    RegistrationSearchService rs = new RegistrationSearchService();
        //    return View();

        //}


        //public DisplayGrid demoSearch()
        //{
        //    DisplayGrid dlr = new DisplayGrid();


        //    //dlr.Columns = new string[] { "Room", "Interview Date", "Session" };
        //    //dlr.Datas = new List<object[]>();
        //    ////dlr.Datas.Add(new object[] { "v11", "v12", "v13" });
        //    ////dlr.Datas.Add(new object[] { "v21", "v22", "v23" });
        //    ////dlr.Datas.Add(new object[] { "v31", "v32", "v33" });
        //    dlr.Rpp = 25;
        //    ////dlr.TotalRecord = 3;
        //    ////dlr.CurrentPage = 1;
        //    return dlr;
        //}
    }
}
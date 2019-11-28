using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Utility;
using Newtonsoft.Json;
using MWMS2.Services;
using MWMS2.Areas.Registration.Models;
using System.Globalization;
using MWMS2.Constant;
using System.Text;
using System.IO;

namespace MWMS2.Areas.Registration.Controllers
{
    public class Fn02GCA_MRAController : ValidationController
    {
        // GET: Registration/Fn02GCA_MRA
        public ActionResult Index(Fn02GCA_MRASearchModel mRASearchModel)
        {
  
            return View("SearchMRA", mRASearchModel);

        }

        public virtual string registrationType()
        {
            return RegistrationConstant.REGISTRATION_TYPE_CGA;
        }

        public ActionResult Save(MeetingRoomDisplayModel model)
        {
            if (ValidateResult.Result.Equals(ServiceResult.RESULT_FAILURE)) return Json(ValidateResult);
            RegistrationCommonService ss = new RegistrationCommonService();
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

            //ss.AjaxAddDrafttoMemberList(model)

            return PartialView("CommitteeMemberListTable", ss.AjaxAddDrafttoMemberList(model));
        }
        public ActionResult AjaxAvailableRoom(Fn02GCA_MRASearchModel model)
        {
            // RegistrationGCAService rs = new RegistrationGCAService();

            // return Content(JsonConvert.SerializeObject(rs.SearchMRAAvailable(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
            RegistrationCommonService ss = new RegistrationCommonService();

            return Content(JsonConvert.SerializeObject(ss.SearchMRAAvailable(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");

        }
        public ActionResult Available_Form(string rid,string sid,string IntDate,string id)
        {
            SessionUtil.DraftObject = null;
            RegistrationCommonService rs = new RegistrationCommonService();
            MeetingRoomDisplayModel model = new MeetingRoomDisplayModel();
            model.RegType = RegistrationConstant.REGISTRATION_TYPE_CGA;
            model.C_INTERVIEW_SCHEDULE.ROOM_ID = rid;
            model.C_INTERVIEW_SCHEDULE.TIME_SESSION_ID = sid;
            model.C_INTERVIEW_SCHEDULE.INTERVIEW_DATE = DateTime.ParseExact(IntDate.Substring(0,10), "yyyy-MM-dd", CultureInfo.InvariantCulture);
            return View("FormMRA", rs.GetMRADisplayDetail(id, model,""));
        }
        public ActionResult Form(string id,string MEETING_NUMBER)
        {
            SessionUtil.DraftObject = null;
            RegistrationCommonService rs = new RegistrationCommonService();
            MeetingRoomDisplayModel model = new MeetingRoomDisplayModel();
            model.RegType = RegistrationConstant.REGISTRATION_TYPE_CGA;
            return View("FormMRA",rs.GetMRADisplayDetail(id, model, MEETING_NUMBER));
        }
        public ActionResult LoadMember(string id)
        {
            SessionUtil.DraftObject = null;
            RegistrationCommonService rs = new RegistrationCommonService();

            return PartialView("CommitteeMemberListTable", rs.LoadMember(id));
        }
        public ActionResult LoadGroup(MeetingRoomDisplayModel model)
        {
        
            IEnumerable<SelectListItem> x =  model.GetCommitteeGroup;

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

        // Export Comp certificate
        public FileStreamResult ExportMRACancelMeeting(MeetingRoomDisplayModel dataExport,string uuid,string MeetingMember)
        {
            RegistrationDataExportService rs = new RegistrationDataExportService();
            //RegistrationGCAService rs = new RegistrationGCAService();
            dataExport.RegType = registrationType();
            return rs.ExportMRACancelMeeting(uuid, MeetingMember);
        }

        //ExportCRCMeeting
        public FileStreamResult ExportCRCMeeting(MeetingRoomDisplayModel dataExport, string uuid, string MeetingMember)
        {
            RegistrationDataExportService rs = new RegistrationDataExportService();
            //RegistrationGCAService rs = new RegistrationGCAService();
            dataExport.RegType = registrationType();
            return rs.ExportCRCMeeting(uuid, MeetingMember);
        }

        //ExportCRCMeeting
        public FileStreamResult CRCMeetingMBR(MeetingRoomDisplayModel dataExport, string uuid, string MeetingMember)
        {
            RegistrationDataExportService rs = new RegistrationDataExportService();
            //RegistrationGCAService rs = new RegistrationGCAService();
            dataExport.RegType = registrationType();
            return rs.ExportCRCMeetingMBR(uuid, MeetingMember);
        }


    }
}
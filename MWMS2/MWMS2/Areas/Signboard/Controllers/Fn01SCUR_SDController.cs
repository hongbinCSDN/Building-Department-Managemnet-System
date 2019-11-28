using MWMS2.Areas.Signboard.Models;
using MWMS2.Models;
using MWMS2.Services;
using MWMS2.Services.Signborad.SignboardDAO;
using MWMS2.Services.Signborad.SignboardServices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.Signboard.Controllers
{
    public class Fn01SCUR_SDController : Controller
    {
        // GET: Signboard/Fn01SCUR_SD
        public ActionResult Index(Fn01SCUR_SDSearchModel model)
        {
           // RegistrationDataExportService s = new RegistrationDataExportService();
           // return s.ss();

            return View(model);
        }
        public ActionResult Search(Fn01SCUR_SDSearchModel model)
        {

            SignboardSDDaoService ss = new SignboardSDDaoService();
          
            ss.SearchScannedDocument(model);
            return Content(JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        [HttpPost]
        public ActionResult Excel(Fn01SCUR_SDSearchModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            SignboardSDDaoService ss = new SignboardSDDaoService();
            
            return Json(new { key = ss.Export_SD(model) });
        }
        public ActionResult Form(string refNo)
        {
            SignboardSDDaoService ss = new SignboardSDDaoService();
            return View(ss.ViewSD(refNo));

            //return View();
        }
        public ActionResult SetASThumbnail(Fn01SCUR_SDDisplayModel model)
        {
            SignboardSDService ss = new SignboardSDService();
           
            return View("Form", ss.SetASThumbnail(model));
        }
        public ActionResult DeleteDSN(Fn01SCUR_SDDisplayModel model)
        {
            SignboardSDDaoService ss = new SignboardSDDaoService();
            ss.DeleteSD(model);
            return View("Form",ss.ViewSD(model.SubmissionNo));
        }
        public ActionResult CreateRecord(Fn01SCUR_SDDisplayModel model)
        {
            SignboardSDDaoService ss = new SignboardSDDaoService();
            ss.CreateRecord(model);
            return View("Form", ss.ViewSD(model.SubmissionNo));
        }
        public FileResult GetSDImg(string uuid)
        {

            CommonBLService ss = new CommonBLService();
          


            return ss.ViewSMMSDImageByUUID(uuid);
     


        }
    }
}
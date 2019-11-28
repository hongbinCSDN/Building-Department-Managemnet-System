using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Models;
using MWMS2.Entity;
using System.IO;
using MWMS2.Services;
using Newtonsoft.Json;
using MWMS2.Areas.Registration.Models;
using MWMS2.Filter;
using MWMS2.Constant;
using MWMS2.Utility;

namespace MWMS2.Areas.Registration.Controllers
{
    public class Fn01Search_QPController : Controller
    {

        public ActionResult Index(Fn01Search_QPSearchModel model)
        {           
            return View(model);
        }

        public ActionResult Form(string id, string regType)
        {
            if ("IMW".Equals(regType))
            {
                RegistrationSearchService rs = new RegistrationSearchService();
                Fn01Search_MWIADisplayModel model = rs.ViewMWIA(id);
                return View("FormMWIA", model);
            } else if ("IP".Equals(regType))
            {
                RegistrationSearchService rs = new RegistrationSearchService();
                Fn01Search_PADisplayModel model = rs.ViewPA(id);
                return View("FormPA", model);
            }
            else if ("CGC".Equals(regType))
            {
                RegistrationCompAppService rs = new RegistrationCompAppService();
                CompDisplayModel model = rs.ViewComp(id, RegistrationConstant.REGISTRATION_TYPE_CGA);
                return View("FormGCA", model);
            }
            else if ("CMW".Equals(regType))
            {
                RegistrationCompAppService rs = new RegistrationCompAppService();
                CompDisplayModel model = rs.ViewComp(id, RegistrationConstant.REGISTRATION_TYPE_MWCA);
                return View("FormMWCA", model);
            }
            return null;
        }

        public ActionResult Search(Fn01Search_QPSearchModel model)
        {
          
            RegistrationSearchService rs = new RegistrationSearchService();
            return Content(JsonConvert.SerializeObject(rs.SearchQP(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        [HttpPost]
        public ActionResult Excel(Fn01Search_QPSearchModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            RegistrationSearchService rs = new RegistrationSearchService();
            return Json(new { key = rs.ExportQP(model) });
        }
        public ActionResult Save(Fn01Search_QPSearchModel model)
        {
            //if (ValidateResult.Result.Equals(ServiceResult.RESULT_FAILURE)) return Json(ValidateResult);
            RegistrationCompAppService s = new RegistrationCompAppService();
            return Json(s.SaveMWSISStatus(model));
        }

        public ActionResult GetSignImg(string uuid, string regType)
        {
            CommonBLService ss = new CommonBLService();

            if ("IMW".Equals(regType))
            {
                var file = ss.ViewCRMImageByUUID(uuid);
                if (file == null)
                {
                    return Content("File not found.");
                }
                else
                {
                    return file;
                }
            }
            else if ("IP".Equals(regType))
            {
                var file = ss.ViewCRMImageByUUID(uuid);
                if (file == null)
                {
                    return Content("File not found.");
                }
                else
                {
                    return file;
                }
            }
            else if ("CGC".Equals(regType))
            {
                var file = ss.ViewCompCRMImageByUUID(uuid);
                if (file == null)
                {
                    return Content("File not found.");
                }
                else
                {
                    return file;
                }
            }
            else if ("CMW".Equals(regType))
            {
                var file = ss.ViewCompCRMImageByUUID(uuid);
                if (file == null)
                {
                    return Content("File not found.");
                }
                else
                {
                    return file;
                }
            }
            return null;
        }
    }
}
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
using MWMS2.Utility;
using MWMS2.Constant;

namespace MWMS2.Areas.Registration.Controllers
{
    public class Fn01Search_DFRController : Controller
    {
        public ActionResult Index(Fn01Search_DFRSearchModel model)
        {
            return View(model);
        }

        // view details of record
        public ActionResult ViewForm(String referralType, String indProMonUuid, String compProMonUuid)
        {
            String regTypeIP = RegistrationConstant.REGISTRATION_TYPE_IP;
            String regTypeMWCI = RegistrationConstant.REGISTRATION_TYPE_MWIA;
            String regTypeGBC = RegistrationConstant.REGISTRATION_TYPE_CGA;
            String regTypeMWC = RegistrationConstant.REGISTRATION_TYPE_MWCA;

            String resultPageForm01 = "Form01";
            String resultPageForm02 = "Form02";

            // defualt result page
            String resultPage = resultPageForm01;
            Fn01Search_DFRDisplayModel displayModel = null;
            RegistrationSearchService rs = new RegistrationSearchService();
            if (regTypeIP.Equals(referralType) || regTypeMWCI.Equals(referralType))
            {
                resultPage = resultPageForm02;
                displayModel = rs.ViewDFRInfoForProfAndMWCI(indProMonUuid);
            }
            else if (regTypeGBC.Equals(referralType) || regTypeMWC.Equals(referralType))
            {
                resultPage = resultPageForm01;
                displayModel = rs.ViewDFRInfoForGBCAndMWC(compProMonUuid);
            }

            // return view , displayModel
            return View(resultPage, displayModel);
        }

        public ActionResult Form01()
        {
            return View();
        }

        public ActionResult Form02()
        {
            //RegistrationSearchService rs = new RegistrationSearchService();
            //rs.ViewDFR(id));
            return View();
        }

        public ActionResult Search(Fn01Search_DFRSearchModel model)
        {
            //if (model.doSearch == false)
            //{
            //    if (!SessionUtil.DraftObject.ContainsKey(ApplicationConstant.DRAFT_KEY_SEARCH))
            //    {

            //        SessionUtil.DraftObject.Add(ApplicationConstant.DRAFT_KEY_SEARCH, model);
            //    }
            //    else
            //    {
            //        SessionUtil.DraftObject[ApplicationConstant.DRAFT_KEY_SEARCH] = model;
            //    }


            //}


            RegistrationSearchService rs = new RegistrationSearchService();
            rs.SearchDFR(model);
            return Content(JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

   
        [HttpPost]
        public ActionResult Excel(Fn01Search_DFRSearchModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            RegistrationSearchService rs = new RegistrationSearchService();
            return Json(new { key = rs.ExportDFR(model) });
        }
      
    }
}
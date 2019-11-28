using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Services;
using Newtonsoft.Json;
using MWMS2.Areas.Registration.Models;
using MWMS2.Utility;
using MWMS2.Constant;

namespace MWMS2.Areas.Registration.Controllers
{
    public class Fn01Search_CNVController : Controller
    {
        public ActionResult Index(Fn01Search_CNVSearchModel model/*, string doSearch*/)
        {
            //if (doSearch == "Y")
            //{
            //    if (SessionUtil.DraftObject.ContainsKey(ApplicationConstant.DRAFT_KEY_SEARCH))
            //    {
            //        model = (Fn01Search_CNVSearchModel)SessionUtil.DraftObject[ApplicationConstant.DRAFT_KEY_SEARCH];

            //        model.doSearch = true;
            //    }
            //}

            //SessionUtil.DraftObject.Remove(ApplicationConstant.DRAFT_KEY_SEARCH);
            //SessionUtil.DraftObject = null;

            return View(model);
        }

        public ActionResult Form(string id,string type)
        {
            RegistrationSearchService rs = new RegistrationSearchService();
            if (type == "Company")
            {
                return View("FormComp", rs.ViewCompCNV(id));

            }
            else
            {
                return View("FormInd", rs.ViewIndCNV(id));

            }
                // return View(rs.ViewCNV(id));

           
        }

        public ActionResult Search(Fn01Search_CNVSearchModel model)
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
            rs.SearchCNV(model);
            return Content(JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }


        [HttpPost]
        public ActionResult Excel(Fn01Search_CNVSearchModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            RegistrationSearchService rs = new RegistrationSearchService();
            return Json(new { key = rs.ExportCNV(model) });
        }
    }
}
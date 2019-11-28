using MWMS2.Areas.Signboard.Models;
using MWMS2.Models;
using MWMS2.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.Signboard.Controllers
{
    public class Fn04RPT_PPJLController : Controller
    {
        // GET: Signboard/Fn04RPT_PPJL
        public ActionResult Index()
        {
            return View("Index");
        }
        //public JsonResult Search()
        //{
        //    DisplayGrid dlr = demoSearch();
        //    return Json(dlr);
        //}
        public ActionResult Search(Fn04RPT_PPJLSearchModel model)
        {

            SignboardSearchService rs = new SignboardSearchService();
            return Content(JsonConvert.SerializeObject(rs.SearchPPJL(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        [HttpPost]
        public ActionResult Excel(Fn04RPT_PPJLSearchModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            SignboardSearchService rs = new SignboardSearchService();
            return Json(new { key = rs.ExportPPJL(model) });
        }

        //public DisplayGrid demoSearch()
        //{
        //    DisplayGrid dlr = new DisplayGrid();


        //    //dlr.Columns = new string[] { "Chinese Name of PBP/ PRC", "English Name of PBP/ PRC", "Registration No.", "Submission No.", "Form Code", "Status", "Signboard Location", "Result" , "Endorsement Date" };
        //    //dlr.Datas = new List<object[]>();

        //    //dlr.Datas.Add(new object[] { "v11", "v12", "v13", "v14", "v11", "v12", "v13", "v14", "v14" });
        //    //dlr.Datas.Add(new object[] { "v21", "v22", "v23", "v24", "v21", "v22", "v23", "v24", "v24" });
        //    //dlr.Datas.Add(new object[] { "v31", "v32", "v33", "v34", "v31", "v32", "v33", "v34", "v34" });


        //    dlr.Rpp = 25;
        //    //dlr.TotalRecord = 3;
        //    //dlr.CurrentPage = 1;
        //    return dlr;
        //}
    }
}
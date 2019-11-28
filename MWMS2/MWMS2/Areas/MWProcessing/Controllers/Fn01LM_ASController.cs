using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.MWProcessing.Controllers
{
    public class Fn01LM_ASController : Controller
    {
        // GET: MWProcessing/Fn01LM_AS
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult Search()
        {
            DisplayGrid dlr = demoSearch();
            return Json(dlr);
        }
        [HttpPost]
        public ActionResult Excel(FormCollection post)
        {
            if (post.AllKeys.Contains("key")) return DisplayGrid.getMemory(post["key"]);
            DisplayGrid dlr = demoSearch();
            return Json(new { });
        }

        public DisplayGrid demoSearch()
        {
            DisplayGrid dlr = new DisplayGrid();


            //dlr.Columns = new string[] { "", "DSN", "MW No.", "Form","Received Date", "Letter Date", "PBP", "PRC", "Address", "A","O","S","P", "<input type="+"checkbox"+">" };//plan to add checkbox to last column header!!!!
            //dlr.Datas = new List<object[]>();
            ////dlr.Datas.Add(new object[] { "v11", "v12", "v13", "v14", "v15", "v16", "v17", "v18","v19","v20","v21","v22","v23","v24" });
            ////dlr.Datas.Add(new object[] { "v21", "v22", "v23", "v24", "v25", "v26", "v27", "v28","v29","v30","v31","v32","v33","v34" });
            ////dlr.Datas.Add(new object[] { "v31", "v32", "v33", "v34", "v35", "v36", "v37", "v38","v39","v40","v41","v42","v43","v44" });

            dlr.Rpp = 25;
            ////dlr.TotalRecord = 3;
            ////dlr.CurrentPage = 1;
            return dlr;
        }
    }
}
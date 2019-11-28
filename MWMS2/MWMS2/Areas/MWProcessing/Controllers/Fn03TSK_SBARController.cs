using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Constant;
using MWMS2.Models;
using MWMS2.Services.ProcessingDAO.Services;
using Newtonsoft.Json;

namespace MWMS2.Areas.MWProcessing.Controllers
{
    public class Fn03TSK_SBARController : Controller
    {
        private ProcessingTSKSBARBLService _BL;
        protected ProcessingTSKSBARBLService BL
        {
            get
            {
                return _BL ?? (_BL = new ProcessingTSKSBARBLService());
            }
        }

        // GET: MWProcessing/Fn03TSK_SBAR
        public ActionResult Index()
        {
            return View(new Fn03TSK_SBARModel()
            {
                ReceiveDateFrom = new DateTime(2019, 1, 1).ToString("dd/MM/yyyy")
                ,
                Checkbox_ItemNo_TypeofMWs_Class1 = BL.GetItemNosAndType(ProcessingConstant.CLASS_1)
                ,
                Checkbox_ItemNo_TypeofMWs_Class2 = BL.GetItemNosAndType(ProcessingConstant.CLASS_2)
                ,
                Checkbox_ItemNo_TypeofMWs_Class3 = BL.GetItemNosAndType(ProcessingConstant.CLASS_3)
                ,
                Checkbox_TypeofMwforms = BL.GetCheckboxTypeofMwforms()
                ,
                Checkbox_Irregularities = BL.GetCheckboxIrregularities()
            });
        }

        [HttpPost]
        public ActionResult Search(Fn03TSK_SBARModel model)
        {
            return Content(JsonConvert.SerializeObject(BL.Search(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        [HttpPost]
        public ActionResult Excel(Fn03TSK_SBARModel model)
        {
            if (!string.IsNullOrEmpty(model.Key)) return DisplayGrid.getMemory(model.Key);
            BL.Search(model);
            return Json(new { key = BL.Excel(model) });
        }
    }
}
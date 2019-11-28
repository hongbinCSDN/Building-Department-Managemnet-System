using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Services.ProcessingDAO.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.MWProcessing.Controllers
{
    public class Fn01LM_StatisticsController : Controller
    {
        private ProcessingLetterModuleBLService _BL;
        protected ProcessingLetterModuleBLService BL
        {
            get
            {
                return _BL ?? (_BL = new ProcessingLetterModuleBLService());
            }
        }

        // GET: MWProcessing/Statistics
        public ActionResult Index(Fn01LM_StatisticsModel model)
        {
            //BL.SearchStatistics(model)
            return View();
        }

        [HttpPost]
        public ActionResult StatisticsIncoming(Fn01LM_StatisticsModel model)
        {
            return PartialView("StatisticsIncoming",BL.SearchStatistics(model)); 
        }
        [HttpPost]
        public ActionResult StatisticsOutgoing(Fn01LM_StatisticsModel model)
        {
            return PartialView("StatisticsOutgoing", BL.SearchStatistics(model));
        }

        public ActionResult StatisticsSDM(Fn01LM_StatisticsModel model)
        {
            return PartialView("StatisticsSDM", BL.SearchStatistics(model));
        }

        [HttpPost]
        public ActionResult PreviousMonth(Fn01LM_StatisticsModel model)
        {
            if (model.Month != null)
            {
                if (model.Month == "1")
                {
                    model.Month = "12";
                    model.Year = (Convert.ToInt32(model.Year) - 1).ToString();
                }
                else
                {
                    model.Month = (Convert.ToInt32(model.Month) - 1).ToString();
                }

            }
            if(model.View == "Incoming")
                return PartialView("StatisticsIncoming", BL.SearchStatistics(model));
            else
                return PartialView("StatisticsOutgoing", BL.SearchStatistics(model));
        }
        [HttpPost]
        public ActionResult NextMonth(Fn01LM_StatisticsModel model)
        {
            if(model.Month != null)
            {
                if(model.Month == "12")
                {
                    model.Month = "1";
                    model.Year = (Convert.ToInt32(model.Year) + 1).ToString();
                }
                else
                {
                    model.Month = (Convert.ToInt32(model.Month) + 1).ToString();
                }
            }
            if (model.View == "Incoming")
                return PartialView("StatisticsIncoming", BL.SearchStatistics(model));
            else
                return PartialView("StatisticsOutgoing", BL.SearchStatistics(model));
        }

        [HttpPost]
        public ActionResult Export(Fn01LM_StatisticsModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            return Json(new { key = BL.Export(model) });

        }


    }
}
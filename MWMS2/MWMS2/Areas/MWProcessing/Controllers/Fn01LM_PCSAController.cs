using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Areas.Registration.Controllers;
using MWMS2.Models;
using MWMS2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.MWProcessing.Controllers
{
    public class Fn01LM_PCSAController : ValidationController
    {
        private ProcessingPcsaBLService BLService;
        protected ProcessingPcsaBLService BL
        {
            get { return BLService ?? (BLService = new ProcessingPcsaBLService()); }
        }
        // GET: MWProcessing/Fn01LM_PCSA
        public ActionResult Index()
        {
            Fn01LM_PcsaSearchModel model = new Fn01LM_PcsaSearchModel()
            {
                SelectionDateFrom = DateTime.Now
                ,
                SelectionDateTo = DateTime.Now
            };
            return View(model);
        }

        public ActionResult Details(string uuid)
        {
            PsacDetailModel psacDetailModel = new PsacDetailModel();
            BL.GetPsacDetailModel(uuid, psacDetailModel);
            return View(psacDetailModel);
        }
        public ActionResult Search(Fn01LM_PcsaSearchModel model)
        {
            if (ValidateResult.Result.Equals(ServiceResult.RESULT_FAILURE)) return Json(ValidateResult);
            return BL.SearchPcsa(model);
            //DisplayGrid dlr = demoSearch();
            //return Json(dlr);
        }

        public ActionResult Excel(Fn01LM_PcsaSearchModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            return BL.ExportPcsa(model);
        }

        public ActionResult UpdatePsac(PsacDetailModel model)
        {
            return Json(BL.UpdatePsac(model));
        }

        public ActionResult RemovePsac(PsacDetailModel model)
        {
            return Json(BL.RemovePsac(model));
        }

        public DisplayGrid demoSearch()
        {
            DisplayGrid dlr = new DisplayGrid();


            //dlr.Columns = new string[] { "Document S/N", "MW No.", "Received Date", "Address", "Selection Date", "Inspection Date", "Officer", "Result" };
            //dlr.Datas = new List<object[]>();
            ////dlr.Datas.Add(new object[] { "v11", "v12", "v13", "v14", "v15", "v16", "v17", "v18", });
            ////dlr.Datas.Add(new object[] { "v21", "v22", "v23", "v24", "v25", "v26", "v27", "v28", });
            ////dlr.Datas.Add(new object[] { "v31", "v32", "v33", "v34", "v35", "v36", "v37", "v38", });

            dlr.Rpp = 25;
            ////dlr.TotalRecord = 3;
            ////dlr.CurrentPage = 1;
            return dlr;
        }
    }
}
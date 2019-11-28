using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Entity;
using MWMS2.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Services
{
    public class ProcessingDirectReturnBLService
    {
        private ProcessingDirectReturnDAOService DAService;
        protected ProcessingDirectReturnDAOService DA
        {
            get { return DAService ?? (DAService = new ProcessingDirectReturnDAOService()); }
        }

        private ProcessingSystemValueService processingSystemValueService;
        protected ProcessingSystemValueService PsvService
        {
            get { return processingSystemValueService ?? (processingSystemValueService = new ProcessingSystemValueService()); }
        }

        private DirectReturnRosterDAOService _drRosterService;
        protected DirectReturnRosterDAOService drRosterService
        {
            get { return _drRosterService ?? (_drRosterService = new DirectReturnRosterDAOService()); }
        }

        #region Search

        public Fn01LM_DRSearchModel GetSearchModel()
        {
            Fn01LM_DRSearchModel model = new Fn01LM_DRSearchModel();
            model.IrregularitiesList = new List<Irregularities>();
            List<P_S_SYSTEM_VALUE> IrregularitiesList = PsvService.GetSystemListByType("Irregularities");
            foreach (P_S_SYSTEM_VALUE item in IrregularitiesList)
            {
                model.IrregularitiesList.Add(new Irregularities() { UUID = item.UUID, Code = item.CODE, Description = item.DESCRIPTION });
            }
            return model;
        }

        public Fn01LM_DRStatModel GetStatModel()
        {
            Fn01LM_DRStatModel model = new Fn01LM_DRStatModel();
            model.IrregularitiesList = new List<Irregularities>();
            List<P_S_SYSTEM_VALUE> IrregularitiesList = PsvService.GetSystemListByType("Irregularities");
            foreach (P_S_SYSTEM_VALUE item in IrregularitiesList)
            {
                model.IrregularitiesList.Add(new Irregularities() { UUID = item.UUID, Code = item.CODE, Description = item.DESCRIPTION });
            }
            return model;
        }

        public Fn01LM_DRSaveModel GetSaveModel()
        {
            Fn01LM_DRSaveModel model = new Fn01LM_DRSaveModel();
            model.IrregularitiesList = new List<Irregularities>();
            List<P_S_SYSTEM_VALUE> IrregularitiesList = PsvService.GetSystemListByType("Irregularities");
            foreach (P_S_SYSTEM_VALUE item in IrregularitiesList)
            {
                model.IrregularitiesList.Add(new Irregularities() { UUID = item.UUID, Code = item.CODE, Description = item.DESCRIPTION });
            }

            model.P_MW_DIRECT_RETURN = new P_MW_DIRECT_RETURN() { RECEIVED_DATE = DateTime.Now };

            //Get Default TO , PO
            P_S_DIRECT_RETURN_ROSTER directReturnRoster = drRosterService.GetRosterInfoByDate(DateTime.Now.Date);

            if (directReturnRoster != null)
            {
                model.P_MW_DIRECT_RETURN.HANDING_STAFF_2 = directReturnRoster.OFFICER_TO;
                model.P_MW_DIRECT_RETURN.HANDING_STAFF_3 = directReturnRoster.OFFICER_PO;
            }
            
            return model;
        }

        public ContentResult SearchDr(Fn01LM_DRSearchModel model)
        {
            return new ContentResult()
            {
                Content = JsonConvert.SerializeObject(DA.SearchDr(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }),
                ContentType = "application/json"
            };
        }

        public Fn01LM_DRSaveModel SearchDrDetail(string sUUID)
        {
            return DA.SearchDrDetail(sUUID);
        }

        public ContentResult SearchStatistics(Fn01LM_DRStatModel model)
        {
            return new ContentResult()
            {
                Content = JsonConvert.SerializeObject(DA.SearchStatistics(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }),
                ContentType = "application/json"
            };
        }
        public dynamic ExcelStatistics(Fn01LM_DRStatModel model)
        {
            return new { key = DA.ExcelStatistics(model) };
        }
        


        public ContentResult SearchStatisticsV2(Fn01LM_DRStatModel model)
        {
            return new ContentResult()
            {
                Content = JsonConvert.SerializeObject(DA.SearchStatisticsV2(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }),
                ContentType = "application/json"
            };
        }

        #endregion

        #region Create

        public JsonResult CreateDr(Fn01LM_DRSaveModel model)
        {
            if (DA.GetDSN(model.P_MW_DIRECT_RETURN.DSN) == null)
            {
                Dictionary<string, List<string>> errorMessage = new Dictionary<string, List<string>>();
                // Error Info
                List<string> listString = new List<string>();
                listString.Add("DSN not exists , please check it");
                errorMessage.Add("P_MW_DIRECT_RETURN.DSN", listString);
                ServiceResult serviceResult = new ServiceResult() { ErrorMessages = errorMessage };
                return new JsonResult() { Data = serviceResult };
            }
            return new JsonResult() { Data = DA.CreateDr(model) };
        }
        public dynamic ExcelStatisticsV2(Fn01LM_DRStatModel model)
        {
            return new { key = DA.ExcelStatisticsV2(model) };
        }

        #endregion

        #region Export

        public ActionResult ExportDr(Fn01LM_DRSearchModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key))
            {
                return DisplayGrid.getMemory(model.Key);
            }
            DA.ExportDr(model);
            model.Columns = model.Columns.Where(d => d.ContainsKey("columnName")).ToArray();
            string fileName = "DirectReturn" + (model.IsMaintenance ? "Maintenance" : DateTime.Now.ToString("yyyy-MM-dd"));
            return new JsonResult() { Data = new { key = model.Export(fileName) } };
        }

        #endregion

        #region Update

        public JsonResult UpdateDr(Fn01LM_DRSaveModel model)
        {
            return new JsonResult() { Data = DA.UpdateDr(model) };
        }

        #endregion
        
    }
}
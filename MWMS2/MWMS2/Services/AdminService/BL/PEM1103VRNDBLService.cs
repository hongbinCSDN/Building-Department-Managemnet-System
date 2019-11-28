using MWMS2.Areas.Admin.Models;
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
    public class PEM1103VRNDBLService
    {
        //PEM1103VRNDDAOService
        private PEM1103VRNDDAOService DAOService;
        protected PEM1103VRNDDAOService DA
        {
            get { return DAOService ?? (DAOService = new PEM1103VRNDDAOService()); }
        }

        //public List<P_S_RULE_OF_CON_LETTER_AND_REF> Search()
        //{
        //    return DA.Search();
        //}

        public ActionResult Search(PEM1103VRNDSearchModel model)
        {
            DA.Search(model);
            return new ContentResult()
            {
                Content = JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }),
                ContentType = "application/json"
            };
        }

        public ActionResult Update(PEM1103VRNDSearchModel model)
        {

            ServiceResult serviceResult = new ServiceResult();
            try
            {
                //Get Data
                List<P_S_RULE_OF_CON_LETTER_AND_REF> updateList = new List<P_S_RULE_OF_CON_LETTER_AND_REF>();
                for (int i = 0; i < model.DNcompare.Count; i++)
                {
                    updateList.Add(new P_S_RULE_OF_CON_LETTER_AND_REF()
                    {
                        UUID = model.DNcompare.ToList()[i].Key,

                        DAYS_OF_NOTIFICATION = model.DN.ToList()[i].Value,
                        DAYS_OF_NOTIFICATION_COMPARE = model.DNcompare.ToList()[i].Value,

                        CONDITIONAL_LETTER_VALUE1 = model.CL1Value.ToList()[i].Value,
                        CONDITIONAL_LETTER_COMPARE1 = model.CL1Compare.ToList()[i].Value,

                        CONDITIONAL_LETTER_VALUE2 = model.CL2Value.ToList()[i].Value,
                        CONDITIONAL_LETTER_COMPARE2 = model.CL2Compare.ToList()[i].Value,

                        REFUSAL_VALUE = model.RefusalValue.ToList()[i].Value,
                        REFUSAL_COMPARE = model.RefusalCompare.ToList()[i].Value

                    });
                }

                serviceResult.Result = DA.Update(updateList) == updateList.Count() ? ServiceResult.RESULT_SUCCESS : ServiceResult.RESULT_FAILURE;

            }
            catch (Exception e)
            {
                serviceResult.Result = ServiceResult.RESULT_FAILURE;
                serviceResult.Message = new List<string>() { e.Message };
            }
            return new JsonResult() { Data = serviceResult };
        }
    }
}
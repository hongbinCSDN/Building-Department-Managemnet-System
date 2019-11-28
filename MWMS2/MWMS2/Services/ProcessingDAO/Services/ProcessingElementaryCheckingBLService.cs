using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Services.ProcessingDAO.DAO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Services
{
    public class ProcessingElementaryCheckingBLService
    {
        private ProcessingElementaryCheckingDAOService DAService;
        protected ProcessingElementaryCheckingDAOService DA
        {
            get { return DAService ?? (DAService = new ProcessingElementaryCheckingDAOService()); }
        }

        private MwCrmInfoDaoImpl crmDaoService;
        protected MwCrmInfoDaoImpl CrmDaoService
        {
            get { return crmDaoService ?? (crmDaoService = new MwCrmInfoDaoImpl()); }
        }

        public void GetElementrayModel(Fn11EC_Model model)
        {
            if (model.ItemCodes == null)
            {
                model.ItemCodes = new List<string>() { "", "", "", "", "" };
            }
            if (string.IsNullOrEmpty(model.RegNo)) { return; }

            model.V_CRM_INFOs = CrmDaoService.GetV_CRM_INFOs(model.RegNo);

            model.V_CRM_INFO = model.V_CRM_INFOs.FirstOrDefault();

            if(model.V_CRM_INFO == null) { model.V_CRM_INFO = new V_CRM_INFO(); }

                string text = "";

            if (model.IsCompany)
            {
                foreach (V_CRM_INFO item in model.V_CRM_INFOs)
                {
                    text = "";

                    if (!string.IsNullOrEmpty(item.AS_CHINESE_NAME))
                    {
                        text += item.AS_CHINESE_NAME + ",";
                    }

                    if (!string.IsNullOrEmpty(item.AS_GIVEN_NAME))
                    {
                        text += item.AS_GIVEN_NAME + ",";
                    }

                    if (!string.IsNullOrEmpty(item.AS_SURNAME))
                    {
                        text += item.AS_SURNAME;
                    }

                    model.PbpPrcAsList.Add(new SelectListItem()
                    {
                        Text = text,
                        Value = item.UUID
                    });
                }
            }
            else if(model.V_CRM_INFO != null)
            {
                text = "";

                if (!string.IsNullOrEmpty(model.V_CRM_INFO.CHINESE_NAME))
                {
                    text += model.V_CRM_INFO.CHINESE_NAME + ",";
                }

                if (!string.IsNullOrEmpty(model.V_CRM_INFO.GIVEN_NAME))
                {
                    text += model.V_CRM_INFO.GIVEN_NAME + ",";
                }

                if (!string.IsNullOrEmpty(model.V_CRM_INFO.SURNAME))
                {
                    text += model.V_CRM_INFO.SURNAME;
                }

                model.PbpPrcAsList.Add(new SelectListItem()
                {
                    Text = text,
                    Value = model.V_CRM_INFO.UUID
                });

            }

            //Checking Item Code
            if (model.IsMwcw)
            {
                model.ValidItem = "";
                model.InvalidItem = "";
                foreach (string item in model.ItemCodes)
                {
                    if (!string.IsNullOrEmpty(item) && (model.V_CRM_INFOs.Where(w => w.ITEM_CODE == ("Item " + item)).Count() > 0))
                    {
                        model.ValidItem += " " + item + ",";
                    }
                    else if (!string.IsNullOrEmpty(item))
                    {
                        model.InvalidItem += " " + item + ",";
                    }
                }

                if (model.ValidItem.Length > 0) { model.ValidItem = model.ValidItem.Substring(0, model.ValidItem.Length - 1); }
                if (model.InvalidItem.Length > 0) { model.InvalidItem = model.InvalidItem.Substring(0, model.InvalidItem.Length - 1); }
            }

        }

        public ContentResult AjaxRegInfo(Fn11EC_Model model)
        {
            return new ContentResult()
            {
                Content = JsonConvert.SerializeObject(new { data = DA.AjaxRegInfo(model).Data }, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }),
                ContentType = "application/json"
            };
        }


    }
}
using MWMS2.Entity;
using MWMS2.DaoController;
using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Areas.Admin.Models;
using MWMS2.Services;
using Newtonsoft.Json;
using MWMS2.Areas.Registration.Controllers;

namespace MWMS2.Areas.Admin.Controllers
{
    public class ParaController : ValidationController
    {
        /*
        private ParaBLService BLService;
        protected ParaBLService BL
        {
            get { return BLService ?? (BLService = new SysBLService()); }
        }*/

        public ActionResult Index()
        {
          
            ParaModel model = new ParaModel()
            {
                CommFunc = Utility.SessionUtil.IconFunctions.Where(o => o.CODE == "ADMIN").FirstOrDefault()?.Childs.Where(o => o.USE_TYPE == "MENU_ITEM").ToList()
                ,
                CRMFunc = Utility.SessionUtil.IconFunctions.Where(o => o.CODE == "ADMIN").FirstOrDefault()?.Childs.Where(o => o.CODE == "10050").FirstOrDefault()?.Childs
                ,
                 PEMFunc = Utility.SessionUtil.IconFunctions.Where(o => o.CODE == "ADMIN").FirstOrDefault()?.Childs.Where(o => o.CODE == "10060").FirstOrDefault()?.Childs
                  ,
                   SMMFunc = Utility.SessionUtil.IconFunctions.Where(o => o.CODE == "ADMIN").FirstOrDefault()?.Childs.Where(o => o.CODE == "10070").FirstOrDefault()?.Childs
                   ,WLMFunc = Utility.SessionUtil.IconFunctions.Where(o => o.CODE == "ADMIN").FirstOrDefault()?.Childs.Where(o => o.CODE == "10080").FirstOrDefault()?.Childs

            };

            return View(model);
        }
    }
}

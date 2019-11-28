using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Constant;
using MWMS2.Services.ProcessingDAO.Services;

namespace MWMS2.Areas.MWProcessing.Controllers
{
    public class ComItemNoTypeController : Controller
    {
        private ComItemNoTypeBLService _BL;
        protected ComItemNoTypeBLService BL
        {
            get
            {
                return _BL ?? (_BL = new ComItemNoTypeBLService());
            }
        }
        // GET: MWProcessing/ComItemNoType
        public ActionResult GetCheckboxlistItemNoType()
        {
            return PartialView(new ComItemNoTypeModel()
            {
                Checkbox_ItemNo_TypeofMWs_Class1 = BL.GetItemNosAndType(ProcessingConstant.CLASS_1)
                ,
                Checkbox_ItemNo_TypeofMWs_Class2 = BL.GetItemNosAndType(ProcessingConstant.CLASS_2)
                ,
                Checkbox_ItemNo_TypeofMWs_Class3 = BL.GetItemNosAndType(ProcessingConstant.CLASS_3)
            });
        }
    }
}
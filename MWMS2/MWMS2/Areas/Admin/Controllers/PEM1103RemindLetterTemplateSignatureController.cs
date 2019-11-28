using MWMS2.Areas.Admin.Models;
using MWMS2.Constant;
using MWMS2.Models;
using MWMS2.Services.AdminService.BL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.Admin.Controllers
{
   
    public class PEM1103RemindLetterTemplateSignatureController : Controller
    {
        private PEM1103BLService _BL;
        protected PEM1103BLService BL
        {
            get
            {
                return _BL ?? (_BL = new PEM1103BLService());
            }
        }
        // GET: Admin/PEM1103RemindLetterTemplateSignature
        public ActionResult Index(string Message)
        {
            if (!string.IsNullOrEmpty(Message))
                ViewBag.Message = Message;
            return View(BL.SearchAckLetterTemplateSignature(ProcessingConstant.S_TYPE_MOD_LETTER_TEMPLETE));
        }

        public ActionResult Save(PEM1103AckLetterTemplateSignatureModel model)
        {
            string Message = null;
            if (BL.UpdateSearchAckLetterTemplateSignature(model, ProcessingConstant.S_TYPE_MOD_LETTER_TEMPLETE).Result == ServiceResult.RESULT_SUCCESS)
            {
                Message = "Save Successfully";
            }
            else
            {
                Message = "Save Failed.";
            }
            return RedirectToAction("Index", new { Message = Message });
        }
    }
}
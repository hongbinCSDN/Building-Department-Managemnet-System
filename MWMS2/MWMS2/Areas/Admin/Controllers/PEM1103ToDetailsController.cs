using MWMS2.Areas.Admin.Models;
using MWMS2.Models;
using MWMS2.Services.AdminService.BL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.Admin.Controllers
{
    public class PEM1103ToDetailsController : Controller
    {
        private PEM1103BLService _BL;
        protected PEM1103BLService BL
        {
            get
            {
                return _BL ?? (_BL = new PEM1103BLService());
            }
        }

        // GET: Admin/PEM1103ToDetails
        public ActionResult Index(string count,string message,string type)
        {
            if (!string.IsNullOrEmpty(message))
            {
                ViewBag.Message = message;
            }
            var result = BL.GetToDetails();
            if (!string.IsNullOrEmpty(count) && type == "Add")
            {
                int num = Convert.ToInt32(count) + 1 - result.pEM1103ToDetailsListModels.Count;
                for (int i=0; i < num; i++)
                {
                    result.pEM1103ToDetailsListModels.Add(new PEM1103ToDetailListModel());
                }
            }
            return View(result);
        }

        public ActionResult Delete(string UUID,int index,int count)
        {
            string message = "";
            if (string.IsNullOrEmpty(UUID))
            {
                var result = BL.GetToDetails();
                for(int i=0; i< count - result.pEM1103ToDetailsListModels.Count - 1; i++)
                {
                    result.pEM1103ToDetailsListModels.Add(new PEM1103ToDetailListModel());
                }
                return View("Index", result);
            }
            else
            {
                if(BL.DeleteToDetails(UUID).Result == ServiceResult.RESULT_SUCCESS)
                {
                    message = "Delete Successfully";
                    count--;
                }
                else
                {
                    message = "Delete Failed.";
                }
                return RedirectToAction("Index",new { count = count,message = message });
            }
        }

        public ActionResult SaveToDetails(PEM1103ToDetailsModel model)
        {
            if (BL.SaveToDetails(model).Result == ServiceResult.RESULT_SUCCESS)
            {
                ViewBag.Message = "Save Successfully";
            }
            else
            {
                ViewBag.Message = "Save Failed.";
            }
            return View("Index", BL.GetToDetails());
        }






    }
}
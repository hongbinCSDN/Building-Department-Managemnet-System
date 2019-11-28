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
    public class PEM1103PublicHolidayController : Controller
    {
        private PEM1103BLService _BL;
        protected PEM1103BLService BL
        {
            get
            {
                return _BL ?? (_BL = new PEM1103BLService());
            }
        }

        // GET: Admin/PEM1103PublicHoliday
        public ActionResult Index(string Message,string year)
        {
            if (!string.IsNullOrEmpty(Message))
                ViewBag.Message = Message;
            return View(BL.InitPublicHolidayModel(year));
        }

        public ActionResult Save(PEM1103PublicHolidayModel model)
        {
            string Message = "";
            if(BL.SavePublicHolidays(model).Result == ServiceResult.RESULT_SUCCESS)
            {
                Message = "Save Successfully";
            }
            else
            {
                Message = "Save Failed.";
            }
            return RedirectToAction("Index", new { Message = Message,year = model.Year });
        }

        public ActionResult AddNewInput(int count,string year)
        {
            var result = BL.InitPublicHolidayModel(year);
            int num = count + 1 - result.holidays.Count;
            for (int i = 0; i < num; i++)
            {
                result.holidays.Add(new Holiday());
            }
            return View("Index", result);
        }

        public ActionResult Delete(string UUID, int count,string year)
        {
            string Message = "";
            if (string.IsNullOrEmpty(UUID))
            {
                var result = BL.InitPublicHolidayModel(year);
                int num = count - result.holidays.Count - 1;
                for (int i = 0; i < num; i++)
                {
                    result.holidays.Add(new Holiday());
                }
                return View("Index", result);
            }
            else
            {
                if(BL.DeleteHoliday(UUID).Result == ServiceResult.RESULT_SUCCESS)
                {
                    Message = "Delete Successfully";
                }
                else
                {
                    Message = "Delete Failed.";
                }
                return RedirectToAction("Index", new { Message = Message, year= year });
            }

        }



    }
}
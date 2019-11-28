using MWMS2.Models;
using MWMS2.Services;
using Newtonsoft.Json;
using System.Web.Mvc;
using MWMS2.Areas.Admin.Models;
using MWMS2.Filter;
using System.Collections.Generic;
using System.Linq;
using System;
using MWMS2.Entity;
using MWMS2.Utility;
using System.Web;
using MWMS2.Constant;
using MWMS2.Areas.Registration.Controllers;

namespace MWMS2.Areas.Admin.Controllers
{
    public class CRMController : ValidationController
    {
        EntitiesRegistration db = new EntitiesRegistration();

        //Index
        public ActionResult Index(string systemType)
        {
            CRM_Model model = new CRM_Model();
            model.SystemType = systemType;
            return View(model);
        }

        public ActionResult IndexAN(String systemType)
        {
            CRM_Model model = new CRM_Model();
            model.SystemType = systemType;
            return View(model);
        }

        public ActionResult IndexBSCI(String systemType)
        {
            CRM_Model model = new CRM_Model();
            model.SystemType = systemType;
            return View(model);
        }

        public ActionResult Index2(String systemType)
        {
            CRM_Model model = new CRM_Model();
            model.SystemType = systemType;
            return View(model);
        }

        public ActionResult IndexCC(String systemType)
        {
            CRM_Model model = new CRM_Model();
            model.SystemType = systemType;
            return View(model);
        }

        public ActionResult IndexHN(String systemType)
        {
            CRM_Model model = new CRM_Model();
            model.SystemType = systemType;
            return View(model);
        }

        public ActionResult IndexRD(String systemType)
        {
            CRM_Model model = new CRM_Model();
            model.SystemType = systemType;
            return View(model);
        }

        public ActionResult IndexPHM(String systemType)
        {
            CRM_Model model = new CRM_Model();
            model.SystemType = systemType;
            model.Year = CommonUtil.getCurrentYear() + "";
            return View(model);
        }

        public ActionResult IndexCCD(String systemType)
        {
            CRM_Model model = new CRM_Model();
            model.SystemType = systemType;
            return View(model);
        }

        public ActionResult IndexLT(String systemType)
        {
            CRM_Model model = new CRM_Model();
            model.SystemType = systemType;
            return View(model);
        }

        public ActionResult IndexMWT(String systemType)
        {
            CRM_Model model = new CRM_Model();
            model.SystemType = "MINOR_WORKS_TYPE";
            return View(model);
        }

        public ActionResult IndexMWI(String systemType)
        {
            CRM_Model model = new CRM_Model();
            model.SystemType = "MINOR_WORKS_ITEM";
            return View(model);
        }




        //Search
        public ActionResult Search(CRM_Model model)
        {
            AdminCRMService rs = new AdminCRMService();
            return Content(JsonConvert.SerializeObject(rs.SearchSystemValue(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        public ActionResult SearchAN(CRM_Model model)
        {
            AdminCRMService rs = new AdminCRMService();
            return Content(JsonConvert.SerializeObject(rs.SearchAN(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        public ActionResult SearchBSCI(CRM_Model model)
        {
            AdminCRMService rs = new AdminCRMService();
            return Content(JsonConvert.SerializeObject(rs.SearchBSCI(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        //Refer to Index2
        public ActionResult SearchWithParentType(CRM_Model model)
        {
            AdminCRMService rs = new AdminCRMService();
            return Content(JsonConvert.SerializeObject(rs.SearchSystemValueWithParentType(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        public ActionResult Excel_MWT(CRM_Model model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            AdminCRMService rs = new AdminCRMService();
            return Json(new { key = rs.ExportMWT(model) });
        }

        public ActionResult SearchCC(CRM_Model model)
        {
            AdminCRMService rs = new AdminCRMService();
            return Content(JsonConvert.SerializeObject(rs.SearchCC(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        public ActionResult SearchHN(CRM_Model model)
        {
            AdminCRMService rs = new AdminCRMService();
            return Content(JsonConvert.SerializeObject(rs.SearchHN(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        public ActionResult SearchRD(CRM_Model model)
        {
            AdminCRMService rs = new AdminCRMService();
            return Content(JsonConvert.SerializeObject(rs.SearchRD(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        public ActionResult Excel_RD(CRM_Model model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            AdminCRMService rs = new AdminCRMService();
            return Json(new { key = rs.ExportRD(model) });
        }

        public ActionResult SearchPHM(CRM_Model model)
        {
            AdminCRMService rs = new AdminCRMService();
            return Content(JsonConvert.SerializeObject(rs.SearchPHM(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }


        public ActionResult SearchCCD(CRM_Model model)
        {
            AdminCRMService rs = new AdminCRMService();
            return Content(JsonConvert.SerializeObject(rs.SearchCCD(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        public ActionResult Excel_CCD(CRM_Model model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            AdminCRMService rs = new AdminCRMService();
            return Json(new { key = rs.ExportCCD(model) });
        }

        public ActionResult SearchLT(CRM_Model model)
        {
            AdminCRMService rs = new AdminCRMService();
            return Content(JsonConvert.SerializeObject(rs.SearchLT(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        public ActionResult Excel_LT(CRM_Model model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            AdminCRMService rs = new AdminCRMService();
            return Json(new { key = rs.ExportLT(model) });
        }

        public ActionResult SearchMWI(CRM_Model model)
        {
            AdminCRMService rs = new AdminCRMService();
            return Content(JsonConvert.SerializeObject(rs.SearchMWI(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }

        public ActionResult Excel_MWI(CRM_Model model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            AdminCRMService rs = new AdminCRMService();
            return Json(new { key = rs.ExportMWI(model) });
        }






        //Form
        public ActionResult Form(string id, String systemType)
        {
            AdminCRMService rs = new AdminCRMService();
            CRM_DisplayModel model = rs.ViewSystemValue(id, systemType);
            return View(model);
        }

        public ActionResult Form_AN(string id)
        {
            AdminCRMService rs = new AdminCRMService();
            CRM_DisplayModel model = rs.ViewAN(id);
            return View(model);
        }

        public ActionResult Form_BSCI(string id)
        {
            AdminCRMService rs = new AdminCRMService();
            CRM_DisplayModel model = rs.ViewBSCI(id);
            return View(model);
        }

        public ActionResult FormWithParentType(string id, String systemType)
        {
            AdminCRMService rs = new AdminCRMService();
            CRM_DisplayModel model = rs.ViewSystemValue(id, systemType);
            return View(model);
        }

        public ActionResult Form_CC(string id, String systemType)
        {
            AdminCRMService rs = new AdminCRMService();
            CRM_DisplayModel model = rs.ViewCC(id);
            return View(model);
        }

        public ActionResult Form_HN(string id)
        {
            AdminCRMService rs = new AdminCRMService();
            CRM_DisplayModel model = rs.ViewHN(id);
            return View(model);
        }

        public ActionResult Form_RD(string id)
        {
            AdminCRMService rs = new AdminCRMService();
            CRM_DisplayModel model = rs.ViewRD(id);
            return View(model);
        }

        public ActionResult Form_CCD(string id)
        {
            AdminCRMService rs = new AdminCRMService();
            CRM_DisplayModel model = rs.ViewCCD(id);
            return View(model);
        }

        public ActionResult Form_LT(string id)
        {
            AdminCRMService rs = new AdminCRMService();
            CRM_DisplayModel model = rs.ViewLT(id);
            return View(model);
        }

        public ActionResult Form_MWT(string id, string systemType)
        {
            AdminCRMService rs = new AdminCRMService();
            CRM_DisplayModel model = rs.ViewSystemValueWithParentType(id, systemType);
            return View(model);
        }

        public ActionResult Form_MWI(string id)
        {
            AdminCRMService rs = new AdminCRMService();
            CRM_DisplayModel model = rs.ViewMWI(id);
            return View(model);
        }
        /**
        //For creating new record
        public ActionResult CreateNew(String systemType)
        {
            CRM_Model model = new CRM_Model();
            model.SystemType = systemType;
            return View(model);
        }
        **/


        //Save
        [HttpPost]
        public ActionResult Save(CRM_DisplayModel model)
        {
            ServiceResult validateResult = ValidationUtil.ValidateForm(ModelState, ViewData);
            if (validateResult.Result.Equals(ServiceResult.RESULT_FAILURE))
            {
                return Json(validateResult);
            }
            AdminCRMService rs = new AdminCRMService();
            validateResult = rs.SaveSystemValue(model);
            return Json(validateResult);


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveAN(CRM_DisplayModel model)
        {

            AdminCRMService rs = new AdminCRMService();

            return Json(rs.Save_AN(model));
            //return RedirectToAction("/IndexAN");
            //return View("IndexAN", new CRM_Model() { SystemType = RegistrationConstant.AUTHORITY_AUTHORITY_NAME });

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveBSCI(CRM_DisplayModel model)
        {

            AdminCRMService rs = new AdminCRMService();
            //rs.Save_BSCI(model);
            return Json(rs.Save_BSCI(model));
            //return RedirectToAction("/IndexBSCI");

                //return View("IndexBSCI", new CRM_Model() { SystemType = RegistrationConstant.BUILDING_SAFETY_CODE_ITEM });
                //return View("IndexBSCI", model);
            //return RedirectToAction("IndexBSCI", new { systemType = RegistrationConstant.BUILDING_SAFETY_CODE_ITEM });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveCC(CRM_DisplayModel model)
        {

            AdminCRMService rs = new AdminCRMService();
            return Json(rs.Save_CC(model));
            //return RedirectToAction("/IndexCC");
            //return View("IndexCC", new CRM_Model() { SystemType = RegistrationConstant.CATEGORY_CODE });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveHN(CRM_DisplayModel model)
        {

            AdminCRMService rs = new AdminCRMService();
            return Json(rs.Save_HN(model));
            //return RedirectToAction("/IndexHN");  //HTML_NOTES
            //return View("IndexHN", new CRM_Model() { SystemType = RegistrationConstant.HTML_NOTES });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveRD(CRM_DisplayModel model)
        {

            AdminCRMService rs = new AdminCRMService();
            return Json(rs.Save_RD(model));
            //return RedirectToAction("/IndexRD");  //ROOM_DETAILS
            //return View("IndexRD", new CRM_Model() { SystemType = RegistrationConstant.ROOM_DETAILS });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveCCD(CRM_DisplayModel model)
        {

            AdminCRMService rs = new AdminCRMService();
            return Json(rs.Save_CCD(model));
            //return RedirectToAction("/IndexCCD"); //CATEGORY_CODE_DETAILS
            //return View("IndexCCD", new CRM_Model() { SystemType = RegistrationConstant.CATEGORY_CODE_DETAILS });
        }


        public ActionResult SavePublicHoilday(CRM_Model model)
        {

            AdminCRMService rs = new AdminCRMService();
            return Json(rs.SavePHM(model));
            //return View("IndexPHM", new CRM_Model() { SystemType = RegistrationConstant.PUBLIC_HOLIDAY_MANAGEMENT });


            //rs.Save_CCD(model);
            //return RedirectToAction("/IndexCCD");
            //return View("IndexPHM", new CRM_Model() { SystemType = RegistrationConstant.PUBLIC_HOLIDAY_MANAGEMENT });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveMWT(CRM_DisplayModel model)
        {
            ServiceResult validateResult = ValidationUtil.ValidateForm(ModelState, ViewData);
            if (validateResult.Result.Equals(ServiceResult.RESULT_FAILURE))
            {
                return Json(validateResult);
            }
            AdminCRMService rs = new AdminCRMService();
            rs.Save_MWT(model);
            //return RedirectToAction("/IndexMWT");  //MINOR_WORKS_TYPE
            //return View("IndexMWT", new CRM_Model() { SystemType = RegistrationConstant.MINOR_WORKS_TYPE });
            return Json(new ServiceResult()
            {
                Result = ServiceResult.RESULT_SUCCESS
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveMWI(CRM_DisplayModel model)
        {
            if (ValidateResult.Result.Equals(ServiceResult.RESULT_FAILURE)) return Json(ValidateResult);

            ServiceResult validateResult = ValidationUtil.ValidateForm(ModelState, ViewData);
            if (validateResult.Result.Equals(ServiceResult.RESULT_FAILURE))
            {
                return Json(validateResult);
            }
            AdminCRMService rs = new AdminCRMService();
            rs.Save_MWI(model);
            //return RedirectToAction("/IndexMWI"); //MINOR_WORKS_ITEM
            //return View("IndexMWI", new CRM_Model() { SystemType = RegistrationConstant.MINOR_WORKS_ITEM });
            return Json(new ServiceResult()
            {
                Result = ServiceResult.RESULT_SUCCESS
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveLT(CRM_DisplayModel model)
        {
            //ServiceResult validateResult = ValidationUtil.ValidateForm(ModelState, ViewData);
            //if (validateResult.Result.Equals(ServiceResult.RESULT_FAILURE))
            //{
            //    return Json(validateResult);
            //}
            AdminCRMService rs = new AdminCRMService();
            return Json(rs.Save_LT(model));
            //return RedirectToAction("/IndexLT");
            //return View("IndexLT", new CRM_Model() { SystemType = RegistrationConstant.LETTER_TEMPLATE });

        }

        [HttpPost]
        public ActionResult Import(HttpPostedFileBase file, CRM_DisplayModel model)
        {
            if (file != null && file.ContentLength > 0)
                try
                {
                    Import(file, model);

                    // string path = Path.Combine(Server.MapPath("~/Images"),
                    //                           Path.GetFileName(file.FileName));
                    //file.SaveAs(path);
                    ViewBag.Message = "File uploaded successfully";
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "ERROR:" + ex.Message.ToString();
                }
            else
            {
                ViewBag.Message = "You have not specified a file.";
            }
            return View("Form_LT", model);
        }




        /**
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateNew([Bind(Exclude = "")] CRM_Model model)
        {
            EntitiesRegistration db = new EntitiesRegistration();
            C_S_SYSTEM_VALUE sv = new C_S_SYSTEM_VALUE();

            var query = db.C_S_SYSTEM_TYPE.Where(x => x.TYPE == model.SystemType);

            sv.UUID = Guid.NewGuid().ToString();   //gen new UUID
            sv.REGISTRATION_TYPE = model.RegType;
            sv.SYSTEM_TYPE_ID = query.First().UUID;
            sv.CODE = model.CODE;
            sv.PARENT_ID = "";
            sv.ENGLISH_DESCRIPTION = model.EngDesc;
            sv.CHINESE_DESCRIPTION = model.ChiDesc;
            sv.IS_ACTIVE = model.IsActive == true ? "Y" : "N";
            sv.ORDERING = model.Ord;
            sv.MODIFIED_DATE = System.DateTime.Now;
            sv.MODIFIED_BY = SystemParameterConstant.UserName;
            sv.CREATED_DATE = System.DateTime.Now;
            sv.CREATED_BY = SystemParameterConstant.UserName;

            db.C_S_SYSTEM_VALUE.Add(sv);
            db.SaveChanges();

            return RedirectToAction("/Index");
        }
    **/

        public ActionResult Excel(CRM_Model model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            AdminCRMService rs = new AdminCRMService();
            //rs.SearchBSCI(model);
            //return Json(new { key = model.ExportCurrentData("ExportData") });

            return Json(new { key = rs.ExportSystemValue(model) });
        }
        [HttpPost]
        public ActionResult ExcelSystemValueWithParentType(CRM_Model model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            AdminCRMService rs = new AdminCRMService();

            //rs.SearchSystemValueWithParentType(model);
            //return Json(new { key = model.ExportCurrentData("ExportData") });

            return Json(new { key = rs.ExcelSystemValueWithParentType(model) });
        }
        public ActionResult Excel_AN(CRM_Model model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            AdminCRMService rs = new AdminCRMService();
            return Json(new { key = rs.ExportAN(model) });
        }
        [HttpPost]
        public ActionResult Excel_BSCI(CRM_Model model)
        {
            if (!string.IsNullOrEmpty(model.Key)) return DisplayGrid.getMemory(model.Key);
            AdminCRMService rs = new AdminCRMService();
            return Json(new { key = rs.ExcelBSCI(model) });
        }
        [HttpPost]
        public ActionResult Excel_CC(CRM_Model model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            AdminCRMService rs = new AdminCRMService();
            return Json(new { key = rs.ExcelCC(model) });
        }
        [HttpPost]
        public ActionResult Excel_HN(CRM_Model model)
        {
            if (!string.IsNullOrWhiteSpace(model.Key)) return DisplayGrid.getMemory(model.Key);
            AdminCRMService rs = new AdminCRMService();
            return Json(new { key = rs.ExcelHN(model) });
        }
        /**
        public DisplayGrid demoSearch()
        {
            DisplayGrid dlr = new DisplayGrid();
            dlr.Rpp = 25;
            return dlr;
        }**/
    }
}
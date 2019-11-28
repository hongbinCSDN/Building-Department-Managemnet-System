using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Controllers;
using System.Data.Entity;
using System.IO;
using MWMS2.Utility;
using MWMS2.Constant;
using MWMS2.Services.Signborad.SignboardServices;
using MWMS2.Services;

namespace MWMS2.Areas.Admin.Controllers
{
    public class SPMLetterTemplateController : Controller
    {   
        //private static String LetterTemplatePath = 
        //    ApplicationConstant.SMMFilePath + SignboardConstant.LETTER_TEMPLATE_FILE_PATH + ApplicationConstant.FileSeparator;

        private EntitiesSignboard db = new EntitiesSignboard();
        private LetterTemplateService lts = new LetterTemplateService();

        // GET: SPMLetterTemplate
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult SPMLetterTemplate()
        {
            var query = db.B_S_LETTER_TEMPLATE.OrderBy(x => x.LETTER_NAME);


            var TagRef = from st in db.B_S_SYSTEM_TYPE
                         join sv in db.B_S_SYSTEM_VALUE on st.UUID equals sv.SYSTEM_TYPE_ID
                         where st.TYPE == "LetterTemplateConstant"
                         orderby sv.CODE
                         select sv;
            ViewBag.TagRef = TagRef;
            return View(query);
        }
        public ActionResult SPMLetterTemplateCreate()
        {
            var LetterTypeQuery = from st in db.B_S_SYSTEM_TYPE
                                  join sv in db.B_S_SYSTEM_VALUE on st.UUID equals sv.SYSTEM_TYPE_ID
                                  where st.TYPE == "LetterType"
                                  orderby sv.CODE
                                  select sv;


            List<SelectListItem> LetterType = new List<SelectListItem>();
            foreach (var item in LetterTypeQuery)
            {
                LetterType.Add(new SelectListItem
                {
                    Text = item.DESCRIPTION,
                    Value = item.CODE,

                });

            }
            ViewBag.LetterType = LetterType;


            var FormCodeQuery = from st in db.B_S_SYSTEM_TYPE
                                join sv in db.B_S_SYSTEM_VALUE on st.UUID equals sv.SYSTEM_TYPE_ID
                                where st.TYPE == "FormCode"
                                orderby sv.CODE
                                select sv;


            List<SelectListItem> FormCode = new List<SelectListItem>();

            FormCode.Add(new SelectListItem
            {
                Text = "- Please Select -",
                Value = ""

            });
            foreach (var item in FormCodeQuery)
            {
                FormCode.Add(new SelectListItem
                {
                    Text = item.DESCRIPTION,
                    Value = item.CODE,

                });

            }
            ViewBag.FormCode = FormCode;



            var LetterResultQuery = from st in db.B_S_SYSTEM_TYPE
                                    join sv in db.B_S_SYSTEM_VALUE on st.UUID equals sv.SYSTEM_TYPE_ID
                                    where st.TYPE == "LetterResult"
                                    orderby sv.CODE
                                    select sv;
            List<SelectListItem> LetterResult = new List<SelectListItem>();

            LetterResult.Add(new SelectListItem
            {
                Text = "- Please Select -",
                Value = ""

            });
            foreach (var item in LetterResultQuery)
            {
                LetterResult.Add(new SelectListItem
                {
                    Text = item.DESCRIPTION,
                    Value = item.DESCRIPTION,

                });

            }
            ViewBag.LetterResult = LetterResult;


            var TagRef = from st in db.B_S_SYSTEM_TYPE
                         join sv in db.B_S_SYSTEM_VALUE on st.UUID equals sv.SYSTEM_TYPE_ID
                         where st.TYPE == "LetterTemplateConstant"
                         orderby sv.CODE
                         select sv;
            ViewBag.TagRef = TagRef;











            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SPMLetterTemplateCreate([Bind(Exclude = "")] B_S_LETTER_TEMPLATE lt, HttpPostedFileBase formFile)
        {
            if (formFile != null)
            {
                // string path = Server.MapPath("~/Uploads/");
                string path = lts.getFilePathByLetterType(lt.LETTER_TYPE);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                formFile.SaveAs(path + Path.GetFileName(formFile.FileName));
                lt.FILE_NAME = Path.GetFileName(formFile.FileName);
                lt.EXTENSION = Path.GetExtension(formFile.FileName).Substring(1);
            }
            //lt.UUID = Guid.NewGuid().ToString();


            lt.FILE_PATH = lts.getFilePathByLetterType(lt.LETTER_TYPE); // @"C:/SMM\LetterTemplatePath";
            //lt.MODIFIED_DATE = System.DateTime.Now;
            //lt.MODIFIED_BY = SystemParameterConstant.UserName;
            //lt.CREATED_DATE = System.DateTime.Now;
            //lt.CREATED_BY = SystemParameterConstant.UserName;
            db.B_S_LETTER_TEMPLATE.Add(lt);
            db.SaveChanges();

            return RedirectToAction("SPMLetterTemplate");
        }


        public ActionResult SPMLetterTemplateEdit(string id)
        {
            var query = db.B_S_LETTER_TEMPLATE.Find(id);


            var LetterTypeQuery = from st in db.B_S_SYSTEM_TYPE
                                  join sv in db.B_S_SYSTEM_VALUE on st.UUID equals sv.SYSTEM_TYPE_ID
                                  where st.TYPE == "LetterType"
                                  orderby sv.CODE
                                  select sv;


            List<SelectListItem> LetterType = new List<SelectListItem>();
            bool selected = false;
            foreach (var item in LetterTypeQuery)
            {
                if (item.CODE == query.LETTER_TYPE)
                {
                    selected = true;
                }
                LetterType.Add(new SelectListItem
                {
                    Text = item.DESCRIPTION,
                    Value = item.CODE,
                    Selected = selected
                });
                selected = false;
            }

            ViewBag.LetterType = LetterType;


            var FormCodeQuery = from st in db.B_S_SYSTEM_TYPE
                                join sv in db.B_S_SYSTEM_VALUE on st.UUID equals sv.SYSTEM_TYPE_ID
                                where st.TYPE == "FormCode"
                                orderby sv.CODE
                                select sv;


            List<SelectListItem> FormCode = new List<SelectListItem>();

            FormCode.Add(new SelectListItem
            {
                Text = "- Please Select -",

                Value = ""
            });
            foreach (var item in FormCodeQuery)
            {
                if (item.CODE == query.FORM_CODE)
                {
                    selected = true;
                }
                FormCode.Add(new SelectListItem
                {
                    Text = item.DESCRIPTION,
                    Value = item.CODE,
                    Selected = selected
                });
                selected = false;
            }
            ViewBag.FormCode = FormCode;



            var LetterResultQuery = from st in db.B_S_SYSTEM_TYPE
                                    join sv in db.B_S_SYSTEM_VALUE on st.UUID equals sv.SYSTEM_TYPE_ID
                                    where st.TYPE == "LetterResult"
                                    orderby sv.CODE
                                    select sv;
            List<SelectListItem> LetterResult = new List<SelectListItem>();

            LetterResult.Add(new SelectListItem
            {
                Text = "- Please Select -",
                Value = ""

            });
            foreach (var item in LetterResultQuery)
            {
                if (item.CODE == query.RESULT)
                {
                    selected = true;
                }
                LetterResult.Add(new SelectListItem
                {
                    Text = item.DESCRIPTION,
                    Value = item.DESCRIPTION,
                    Selected = selected
                });

            }
            ViewBag.LetterResult = LetterResult;


            var TagRef = from st in db.B_S_SYSTEM_TYPE
                         join sv in db.B_S_SYSTEM_VALUE on st.UUID equals sv.SYSTEM_TYPE_ID
                         where st.TYPE == "LetterTemplateConstant"
                         orderby sv.CODE
                         select sv;
            ViewBag.TagRef = TagRef;





            ViewBag.MODIFIED_DATE = DateUtil.ConvertToDateTimeDisplay(query.MODIFIED_DATE);


            return View(query);
        }

        public ActionResult checkFileExist(string docid)
        {
            var query = db.B_S_LETTER_TEMPLATE.Find(docid);
            string path = lts.getFilePathByLetterType(query.LETTER_TYPE);

            var isExist = lts.checkFileExist(path + query.FILE_NAME);
            if(isExist)
            {
                return Json(new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS });
            }
            else
            {
                return Json(new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = new List<string>() { "File not found." } });
            }
        }

        public ActionResult SPMLetterTemplateDownload(string docid)
        {
            try
            {
                var query = db.B_S_LETTER_TEMPLATE.Find(docid);
                string path = lts.getFilePathByLetterType(query.LETTER_TYPE);

                byte[] fileBytes = System.IO.File.ReadAllBytes(path + query.FILE_NAME);
                string fileName = query.FILE_NAME;
                var letter =  File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
                if (letter == null)
                {
                    return Content("File not found.");
                }
                else
                {
                    return letter;
                }
            }
            catch(Exception ex)
            {
                return Content("File not found.");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SPMLetterTemplateEdit([Bind(Exclude = "")] B_S_LETTER_TEMPLATE lt, HttpPostedFileBase formFile)
        {
            try
            {
                if (formFile != null)
                {
                    // string path = Server.MapPath("~/Uploads/");
                    string path = lts.getFilePathByLetterType(lt.LETTER_TYPE);

                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    formFile.SaveAs(path + Path.GetFileName(formFile.FileName));
                    lt.FILE_NAME = Path.GetFileName(formFile.FileName);
                    lt.EXTENSION = Path.GetExtension(formFile.FileName).Substring(1);
                }
                //lt.MODIFIED_DATE = System.DateTime.Now;
                lt.MODIFIED_BY = SystemParameterConstant.UserName;
                
                db.Entry(lt).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception ex)
            {

            }

            return RedirectToAction("SPMLetterTemplate");
        }

        public ActionResult SPMLetterTemplateDelete(string id)
        {
            var query = db.B_S_LETTER_TEMPLATE.Where(x => x.UUID == id).SingleOrDefault();
            db.B_S_LETTER_TEMPLATE.Remove(query);
            db.SaveChanges();

            return RedirectToAction("SPMLetterTemplate");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Entity;
using MWMS2.Models;

namespace MWMS2.Controllers
{
    public class SMM0102Controller : Controller
    {
        private EntitiesSignboard db = new EntitiesSignboard();
        // GET: SMM01
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult SMM0102SC01CreateSubNo()
        {
            try
            {
                string Prefix = DateTime.Now.ToString("yy/MM");

                var query = from RefNo in db.B_SV_REFERENCE_NO
                            where RefNo.PREFIX == Prefix && RefNo.TYPE == "SC"
                            select RefNo;
                decimal? NextNumber = 0;
                if (!query.Any())
                {
                    B_SV_REFERENCE_NO b_SV_REFERENCE_NO = new B_SV_REFERENCE_NO();
                 

                    b_SV_REFERENCE_NO.CURRENT_NUMBER = 1;

                    b_SV_REFERENCE_NO.CREATED_BY = SystemParameterConstant.UserName;
                    b_SV_REFERENCE_NO.CREATED_DATE = DateTime.Now;
                    b_SV_REFERENCE_NO.MODIFIED_BY = SystemParameterConstant.UserName;
                    b_SV_REFERENCE_NO.MODIFIED_DATE = DateTime.Now;

                    b_SV_REFERENCE_NO.PREFIX = Prefix;
                    b_SV_REFERENCE_NO.TYPE = "SC";
                    b_SV_REFERENCE_NO.UUID = Guid.NewGuid().ToString();


                    NextNumber = 1;

                    db.B_SV_REFERENCE_NO.Add(b_SV_REFERENCE_NO);


                }
                else
                {
                    query.First().MODIFIED_DATE = DateTime.Now;
                    query.First().CURRENT_NUMBER += 1;
                    NextNumber = query.First().CURRENT_NUMBER;
                }
                #region sV_SUBMISSION
               B_SV_SUBMISSION b_SV_SUBMISSION = new B_SV_SUBMISSION();
               b_SV_SUBMISSION.UUID = Guid.NewGuid().ToString();
               b_SV_SUBMISSION.RECORD_TYPE = "SCU";
               b_SV_SUBMISSION.SCU_RECEIVED_DATE = DateTime.Now;
               b_SV_SUBMISSION.FORM_CODE = "SC01";
               b_SV_SUBMISSION.STATUS = "SCU_DATA_ENTRY";
               b_SV_SUBMISSION.REFERENCE_NO = "SC" + DateTime.Now.ToString("yyMM") + NextNumber.ToString().PadLeft(4, '0');

               b_SV_SUBMISSION.CREATED_BY = SystemParameterConstant.UserName;
               b_SV_SUBMISSION.CREATED_DATE = DateTime.Now;
               b_SV_SUBMISSION.MODIFIED_BY = SystemParameterConstant.UserName;
               b_SV_SUBMISSION.MODIFIED_DATE = DateTime.Now;
               b_SV_SUBMISSION.SV_SCANNED_DOCUMENT_ID = Guid.NewGuid().ToString();
                db.B_SV_SUBMISSION.Add(b_SV_SUBMISSION);
                #endregion


                #region DSN SV_SCANNED_DOCUMENT
                var dsnQuery = db.B_SV_REFERENCE_NO.Where(x => x.PREFIX == "d");
                dsnQuery.First().CURRENT_NUMBER += 1;
                dsnQuery.First().MODIFIED_DATE = DateTime.Now;



                var DSNNextNumber = dsnQuery.First().CURRENT_NUMBER;

               B_SV_SCANNED_DOCUMENT b_SV_SCANNED_DOCUMENT = new B_SV_SCANNED_DOCUMENT();
               b_SV_SCANNED_DOCUMENT.UUID = b_SV_SUBMISSION.SV_SCANNED_DOCUMENT_ID;
               b_SV_SCANNED_DOCUMENT.RECORD_ID = b_SV_SUBMISSION.REFERENCE_NO;
               b_SV_SCANNED_DOCUMENT.RECORD_TYPE = "VALIDATION";
               b_SV_SCANNED_DOCUMENT.DSN_NUMBER = "D" + DSNNextNumber.ToString().PadLeft(10, '0');
               b_SV_SCANNED_DOCUMENT.DOCUMENT_TYPE = "Form";
               b_SV_SCANNED_DOCUMENT.FOLDER_TYPE = "Private(SCU)";

               b_SV_SCANNED_DOCUMENT.CREATED_BY = SystemParameterConstant.UserName;
               b_SV_SCANNED_DOCUMENT.CREATED_DATE = DateTime.Now;
               b_SV_SCANNED_DOCUMENT.MODIFIED_BY = SystemParameterConstant.UserName;
               b_SV_SCANNED_DOCUMENT.MODIFIED_DATE = DateTime.Now;
                db.B_SV_SCANNED_DOCUMENT.Add(b_SV_SCANNED_DOCUMENT);

                #endregion









                #region B_SV_RECORD
                //B_SV_RECORD b_SV_RECORD = new B_SV_RECORD();
                //b_SV_RECORD.UUID = System.Guid.NewGuid().ToString();

                //b_SV_RECORD.REFERENCE_NO = "SC" + DateTime.Now.ToString("yyMM") + NextNumber.ToString().PadLeft(4, '0');

                //b_SV_RECORD.RECEIVED_DATE = DateTime.Now;
                //b_SV_RECORD.STATUS_CODE = "DATA_ENTRY_DRAFT";
                //b_SV_RECORD.LANGUAGE_CODE = "ZH";

                //b_SV_RECORD.PAW_ID = "";     //person contact  B_SV_PERSON_CONTACT

                //b_SV_RECORD.SV_SIGNBOARD_ID = ""; //B_SV_SIGNBOARD

                //b_SV_RECORD.SV_SUBMISSION_ID = ""; //B_SV_SUBMISSION

                //b_SV_RECORD.TO_USER_ID = ""; //B_S_USER_ACCOUNT

                //b_SV_RECORD.TO_OFFICER = ""; //B_S_USER_ACCOUNT

                //b_SV_RECORD.PO_USER_ID = ""; //B_S_USER_ACCOUNT

                //b_SV_RECORD.SPO_USER_ID = "";//B_S_USER_ACCOUNT

                //b_SV_RECORD.OI_ID = "";      //B_SV_PERSON_CONTACT

                //b_SV_RECORD.FORM_CODE = "SC02";
                //b_SV_RECORD.INSPECTION_DATE = null;
                //b_SV_RECORD.VALIDATION_EXPIRY_DATE = null;

                //b_SV_RECORD.CREATED_BY = SystemParameterConstant.UserName;
                //b_SV_RECORD.CREATED_DATE = .DateTime.Now;
                //b_SV_RECORD.MODIFIED_BY = SystemParameterConstant.UserName;
                //b_SV_RECORD.MODIFIED_DATE = DateTime.Now;
                #endregion



                var result = new { SubNo = b_SV_SUBMISSION.REFERENCE_NO };



                db.SaveChanges();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
              var a = ex.InnerException.Message;
            }
            return null;
            // return RedirectToAction("SMM0102");
        }
        public ActionResult SMM0102()
        {


            return View();
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
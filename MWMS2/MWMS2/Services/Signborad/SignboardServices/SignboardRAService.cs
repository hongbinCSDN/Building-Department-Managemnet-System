using MWMS2.Areas.Signboard.Models;
using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Services.Signborad.SignboardDAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Services.Signborad.SignboardServices
{
    public class SignboardRAService
    {
        public SCUR_Models ReceiveCompleteForm(SCUR_Models model)
        {
            using (EntitiesSignboard db = new EntitiesSignboard())
            {

                string code = SignboardConstant.getValidationAlterationFormCode(model.FormCode);


                var query = db.B_SV_SUBMISSION.Where(x => x.FORM_CODE == code && x.REFERENCE_NO == model.SubmissionNo);
                if (!query.Any())
                {
                    return new SCUR_Models()
                    {
                        result = false,
                        ErrMsg = "No related SC01 or SC02 form is found for " + model.SubmissionNo,
                    };
                }
                else
                {

                    SignboardCommonDAOService ss = new SignboardCommonDAOService();
                  

                    B_SV_SCANNED_DOCUMENT b_SV_SCANNED_DOCUMENT = new B_SV_SCANNED_DOCUMENT();
                    b_SV_SCANNED_DOCUMENT.UUID = Guid.NewGuid().ToString().Replace("-", "");
                    b_SV_SCANNED_DOCUMENT.RECORD_ID = model.SubmissionNo.ToUpper().Trim();
                    b_SV_SCANNED_DOCUMENT.RECORD_TYPE = SignboardConstant.SCAN_DOC_RECORD_TYPE_VALIDATION;
                    b_SV_SCANNED_DOCUMENT.DSN_NUMBER = ss.GetNumber();
                    b_SV_SCANNED_DOCUMENT.DOCUMENT_TYPE = SignboardConstant.SCAN_DOC_DOCUMENT_TYPE_FORM;
                    b_SV_SCANNED_DOCUMENT.FOLDER_TYPE = SignboardConstant.SCANNED_DOC_FOLDER_TYPE_PRIVATE_SCU;
                    b_SV_SCANNED_DOCUMENT.CREATED_BY = SystemParameterConstant.UserName;
                    b_SV_SCANNED_DOCUMENT.CREATED_DATE = DateTime.Now;
                    db.B_SV_SCANNED_DOCUMENT.Add(b_SV_SCANNED_DOCUMENT);

                    B_SV_SUBMISSION svSub = new B_SV_SUBMISSION();
                    svSub.RECORD_TYPE = SignboardConstant.SV_SUBMISSION_RECORD_TYPE_SCU;
                    svSub.REFERENCE_NO = model.SubmissionNo.ToUpper().Trim();
                    svSub.FORM_CODE = model.FormCode;
                    svSub.SCU_RECEIVED_DATE = DateTime.Now;
                    svSub.STATUS = SignboardConstant.SV_SUBMISSION_STATUS_SCU_DATA_ENTRY;
                    svSub.SV_SCANNED_DOCUMENT_ID = b_SV_SCANNED_DOCUMENT.UUID;

                    db.B_SV_SUBMISSION.Add(svSub);

                    db.SaveChanges();
                }

                return new SCUR_Models() {ErrMsg="Received" }; 
            }
        }

        public SCUR_Models GenerateSubmissionNumber(SCUR_Models model)
        {
            try
            {
                using (EntitiesSignboard db = new EntitiesSignboard())
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
                    b_SV_REFERENCE_NO.UUID = Guid.NewGuid().ToString().Replace("-", "");


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
                b_SV_SUBMISSION.UUID = Guid.NewGuid().ToString().Replace("-","");
                b_SV_SUBMISSION.RECORD_TYPE = SignboardConstant.SV_SUBMISSION_RECORD_TYPE_SCU;
                b_SV_SUBMISSION.SCU_RECEIVED_DATE = DateTime.Now;
                b_SV_SUBMISSION.FORM_CODE = model.FormCode;
                b_SV_SUBMISSION.STATUS = SignboardConstant.SV_SUBMISSION_STATUS_SCU_DATA_ENTRY;
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
                b_SV_SCANNED_DOCUMENT.RECORD_TYPE = SignboardConstant.SCAN_DOC_RECORD_TYPE_VALIDATION;
                b_SV_SCANNED_DOCUMENT.DSN_NUMBER = "D" + DSNNextNumber.ToString().PadLeft(10, '0');
                b_SV_SCANNED_DOCUMENT.DOCUMENT_TYPE = SignboardConstant.SCAN_DOC_DOCUMENT_TYPE_FORM;
                b_SV_SCANNED_DOCUMENT.FOLDER_TYPE = SignboardConstant.SCANNED_DOC_FOLDER_TYPE_PRIVATE_SCU;

                b_SV_SCANNED_DOCUMENT.CREATED_BY = SystemParameterConstant.UserName;
                b_SV_SCANNED_DOCUMENT.CREATED_DATE = DateTime.Now;
                b_SV_SCANNED_DOCUMENT.MODIFIED_BY = SystemParameterConstant.UserName;
                b_SV_SCANNED_DOCUMENT.MODIFIED_DATE = DateTime.Now;
                db.B_SV_SCANNED_DOCUMENT.Add(b_SV_SCANNED_DOCUMENT);

                    #endregion







                    db.SaveChanges();

                    #region B_SV_RECORD

                    #endregion

                model.SubmissionNo = b_SV_SUBMISSION.REFERENCE_NO;

                return model;
                }

            }
            catch (Exception ex)
            {
                var a = ex.InnerException.Message;
            }
            return model;
        }

    }
}
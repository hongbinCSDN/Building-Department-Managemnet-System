using MWMS2.Constant;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Entity;
using MWMS2.Models;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace MWMS2.Areas.Registration.Models
{
    public class CertificateDisplayListModel : EditFormModel, IValidatableObject
    {
        public C_IND_APPLICATION C_IND_APPLICATION { get; set; }
        public bool result {get;set;}
        public string ErrMsg { get; set; }
        public string UUID { get; set; }
        public string IndApplicationUUID { get; set; }
        public string CATEGORY_CODE { get; set; }
        public string PERIOD_VADLIDITY_SV_CODE { get; set; }
       /// public string FILE_PATH_NONRESTRICTED { get; set; }


        public DateTime? APPLICATION_DATE { get; set; }
        public DateTime? REGISTRATION_DATE { get; set; }
        public DateTime? EXPIRY_DATE { get; set; }
        public DateTime? RETENTION_DATE { get; set; }
        public DateTime? RESTORE_DATE { get; set; }
        public DateTime? REMOVAL_DATE { get; set; }
        public string APPFORM_SV_CODE { get; set; }

        public string REMARKS { get; set; }
        public string FILE_PATH_NON_RESTRICTED { get; set; }
        public string CARD_SERIAL_NO { get; set; }

        public List<HttpPostedFileBase> _UploadDoc;
        public List<HttpPostedFileBase> UploadDoc
        {
            get { return _UploadDoc; }
            set
            {
                List<byte[]> abc = new List<byte[]>();

                for (int i = 0; i < value.Count; i++)
                {
                    MemoryStream m = new MemoryStream();
                    if (value[i] != null)
                    {
                        value[i].InputStream.CopyTo(m); abc.Add(m.ToArray());
                    }
        

                }
                UploadDocStream = abc;
                _UploadDoc = value;
            }
        }
        public List<byte[]> UploadDocStream { get; private set; }
        public string REGISTRATION_NO { get; set; }
        public string STATUS { get; set; }
        public DateTime? GAZETTE_DATE { get; set; }
        public DateTime? APPROVAL_DATE { get; set; }

        public DateTime? RETENTION_APPLICATION_SUMBITTED_DATE { get; set; }
        public DateTime? RESTORATION_APPLICATION_SUMBITTED_DATE { get; set; }

        public DateTime? REMOVED_FROM_REGISTER { get; set; }
        public DateTime? EXTENDED_DATE_EXPIRY { get; set; }


        public DateTime? EXPIRY_EXTEND_DATE { get; set; }
        public string GetCatCodeDescriptionByUUID(string uuid)
        {
            return SystemListUtil.GetCatCodeByUUID(uuid) ==null? uuid: SystemListUtil.GetCatCodeByUUID(uuid).CODE;
        }
        public string GetSVDescriptionByUUID(string uuid)
        {
            return SystemListUtil.GetSVByUUID(uuid) == null ? uuid : SystemListUtil.GetSVByUUID(uuid).CODE;
        }
        public IEnumerable<SelectListItem> RetrieveCatCodeByType()
        {
            return
                    (new List<SelectListItem>() { new SelectListItem() { Text = "- Select -", Value = "" } })
                        .Concat(SystemListUtil.RetrieveCatCodeByType(
                            RegistrationConstant.REGISTRATION_TYPE_IP)
                            .Select(o => new SelectListItem() { Text = o.CODE, Value = o.UUID }));


        }



        public IEnumerable<SelectListItem> GetPeriodValidity()
        {
            return SystemListUtil.GetValidityPeriodListByType();
        }


        public IEnumerable<SelectListItem> APPLICATION_FORM_List
        {
            get
            {
                return
                    (new List<SelectListItem>() { new SelectListItem() { Text = "- Select -", Value = "" } })
                        .Concat(SystemListUtil.GetSVListByRegTypeNType(
                            RegistrationConstant.REGISTRATION_TYPE_IP
                            , RegistrationConstant.SYSTEM_TYPE_APPLICATION_FORM)
                            .Select(o => new SelectListItem() { Text = o.CODE, Value = o.UUID }));
            }
        }
        public IEnumerable<SelectListItem> APPLICANT_STATUS_ID_List
        {
            get
            {
                return
                    (new List<SelectListItem>() { new SelectListItem() { Text = "- Select -", Value = "" } })
                        .Concat(SystemListUtil.GetSVListByType(
                             RegistrationConstant.SYSTEM_TYPE_APPLICANT_STATUS)
                            .Select(o => new SelectListItem() { Text = o.ENGLISH_DESCRIPTION, Value = o.UUID }));
            }
        }

        public List<QualifcationDisplayListModel> PRBTableList { get; set; }

        public string isYellow = "";

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield return ValidationUtil.Validate_Mandatory(this, "CATEGORY_CODE             ");
            yield return ValidationUtil.Validate_Mandatory(this, "PERIOD_VADLIDITY_SV_CODE             ");
            yield return ValidationUtil.Validate_Mandatory(this, "STATUS             ");

        }



    }
}
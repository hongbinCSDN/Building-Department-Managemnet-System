using MWMS2.Constant;
using MWMS2.Models;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.Registration.Models
{
    public class QualifcationDisplayListModel : EditFormModel, IValidatableObject
    {
        public string UUID { get; set; }
        public string PRB { get; set; }
        public string QUALIFICATIONCODE { get; set; }
        public string QUALIFICATIONCODETYPE { get; set; }
        public string REGISTRATIONNO { get; set; }
        public DateTime? EXPIRYDATE { get; set; }
        public string EXPIRYDATESTRING { get; set; }
        public List<string> SelectedCatCodeDetail { get; set; }
        public List<SelectListItem> getPRPByType()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem
            {
                Text = "- Select PRB -",
                Value = "",

            });
            selectListItems.AddRange(from sv in SystemListUtil.GetSVListByType(RegistrationConstant.SYSTEM_TYPE_PROF_REGISTRATION_BOARD)
                                     select new SelectListItem()
                                     {
                                         Text = sv.CODE,
                                         Value = sv.UUID
                                     }
                                     );

            return selectListItems;
        }
        public IEnumerable<SelectListItem> RetrieveQCByType()
        {
            return
                    (new List<SelectListItem>() { new SelectListItem() { Text = "- Select -", Value = "" } })
                        .Concat(SystemListUtil.RetrieveCatCodeByType(
                            RegistrationConstant.REGISTRATION_TYPE_IP)
                            .Select(o => new SelectListItem() { Text = o.CODE, Value = o.UUID }));


        }
        public IEnumerable<SelectListItem> RetrieveQCType()
        {
            return (new List<SelectListItem>() { new SelectListItem() { Text = "- Select -", Value = "" } })
                         .Concat(SystemListUtil.GetSVListByType(
                             RegistrationConstant.SYSTEM_TYPE_QUALIFICATION_TYPE)
                             .Select(o => new SelectListItem() { Text = o.CODE, Value = o.UUID }));
        }

       
        public IEnumerable<SelectListItem> getRIECatCode()
        {
            return (from catcode in SystemListUtil.GetCatCodeDetailByCode("RI(E)")
                    select new SelectListItem()
                    {
                        Text = catcode.ENGLISH_DESCRIPTION,
                        Value = catcode.UUID
                    }) ;
         }
        public IEnumerable<SelectListItem> getRISCatCode()
        {
            return (from catcode in SystemListUtil.GetCatCodeDetailByCode("RI(S)")
                    select new SelectListItem()
                    {
                        Text = catcode.ENGLISH_DESCRIPTION,
                        Value = catcode.UUID
                    });
        }

         public bool   ChkbxRPE_BS    { get; set; }
         public bool   ChkbxRPE_M     { get; set; }
         public bool   ChkbxRPE_S     { get; set; }
         public bool   ChkbxRPS_B     { get; set; }
         public bool   ChkbxRPS_Q     { get; set; }
         public bool   ChkbxRPE_C     { get; set; }
         public bool   ChkbxRPE_B     { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield return ValidationUtil.Validate_Mandatory(this, "PRB");
            yield return ValidationUtil.Validate_Mandatory(this, "QUALIFICATIONCODE");
            yield return ValidationUtil.Validate_Mandatory(this, "QUALIFICATIONCODETYPE");
            yield return ValidationUtil.Validate_Mandatory(this, "REGISTRATIONNO");
        }




    }
}
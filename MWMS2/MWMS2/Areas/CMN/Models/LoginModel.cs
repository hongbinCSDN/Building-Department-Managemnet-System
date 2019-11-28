using MWMS2.Entity;
using MWMS2.Filter;
using MWMS2.Models;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.CMN.Models
{
    
    public class LoginModel: IValidatableObject
    {
        [Display(Name = "Portal ID")]
        public string BD_PORTAL_LOGIN { get; set; }
        [Display(Name = "Password")]
        public string password { get; set; }

        public SYS_POST post { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {  
            
            yield return ValidationUtil.Validate_Mandatory(this, "BD_PORTAL_LOGIN");
            yield return ValidationUtil.Validate_Mandatory(this, "password");

            if(!String.IsNullOrEmpty(BD_PORTAL_LOGIN) && !String.IsNullOrEmpty(password)) { 
                SYS_POST cpost = AuthManager.getPOST(BD_PORTAL_LOGIN, EncryptDecryptUtil.getEncrypt( password));
                if (cpost == null){
                    yield return new ValidationResult("Portal Name / Password is not correct.", new List<string> { "errorMsg" });
                }
                else{
                   post = cpost;
                }
            }
            /* yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICATION.UUID                        ");
             yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICATION.FILE_REFERENCE_NO           ");
             yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICATION.BR_NO                       ");
             yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICATION.BRANCH_NO                   ");
             yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICATION.ENGLISH_COMPANY_NAME        ");
             yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICATION.CHINESE_COMPANY_NAME        ");*/
        }
    }
}
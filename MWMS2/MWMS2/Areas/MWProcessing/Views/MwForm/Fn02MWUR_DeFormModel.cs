using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web;

namespace MWMS2.Areas.MWProcessing.Models
{
    public class Fn02MWUR_DeFormModel : ICloneable, IValidatableObject
    {
        public P_MW_DSN P_MW_DSN { get; set; }
        public P_MW_RECORD P_MW_RECORD { get; set; }
        public P_MW_REFERENCE_NO P_MW_REFERENCE_NO { get; set; }

        //Common Page de_formMWAddress_new
        public P_MW_ADDRESS MWAddress { get; set; }

        //Common Page de_formOwnerAddress_new
        public P_MW_ADDRESS OwnerAddress { get; set; }
        public P_MW_PERSON_CONTACT OwnerPersonContact { get; set; }

        //Common Page de_formSignBoardAddress_new
        public P_MW_ADDRESS SignBoardAddress { get; set; }
        public P_MW_PERSON_CONTACT SignBoardPersonContact { get; set; }

        public P_MW_ADDRESS OIAddress { get; set; }
        public P_MW_PERSON_CONTACT OIPersonContact { get; set; }


        public List<P_MW_RECORD_ITEM> P_MW_RECORD_ITEMs { get; set; }

        public List<P_MW_RECORD_ITEM> P_MW_RECORD_ITEMs_CLASS_I { get; set; }
        public List<P_MW_RECORD_ITEM> P_MW_RECORD_ITEMs_CLASS_II { get; set; }
        public List<P_MW_RECORD_ITEM> P_MW_RECORD_ITEMs_CLASS_III { get; set; }

        public List<P_MW_APPOINTED_PROFESSIONAL> P_MW_APPOINTED_PROFESSIONALs { get; set; }

        public List<P_MW_FORM_09> P_MW_FORM_09s { get; set; }

        public P_MW_FORM P_MW_FORM { get; set; }

        public Dictionary<string , object> DictID { get; set; }

        public string LANG_ENGLISH = ProcessingConstant.LANG_ENGLISH;
        public string LANG_CHINESE = ProcessingConstant.LANG_CHINESE;

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        //Modify by dive 20191016
        public P_EFSS_FORM_MASTER P_EFSS_FORM_MASTER { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            foreach (ValidationResult r in ValidationUtil.Validate_Properties_Length(this, P_MW_DSN)) yield return r;
            foreach (ValidationResult r in ValidationUtil.Validate_Properties_Length(this, P_MW_RECORD)) yield return r;
            foreach (ValidationResult r in ValidationUtil.Validate_Properties_Length(this, P_MW_REFERENCE_NO)) yield return r;
            foreach (ValidationResult r in ValidationUtil.Validate_Properties_Length(this, MWAddress)) yield return r;
            foreach (ValidationResult r in ValidationUtil.Validate_Properties_Length(this, OwnerAddress)) yield return r;
            foreach (ValidationResult r in ValidationUtil.Validate_Properties_Length(this, OwnerPersonContact)) yield return r;
            foreach (ValidationResult r in ValidationUtil.Validate_Properties_Length(this, SignBoardAddress)) yield return r;
            foreach (ValidationResult r in ValidationUtil.Validate_Properties_Length(this, SignBoardPersonContact)) yield return r;
            foreach (ValidationResult r in ValidationUtil.Validate_Properties_Length(this, OIAddress)) yield return r;
            foreach (ValidationResult r in ValidationUtil.Validate_Properties_Length(this, OIPersonContact)) yield return r;

            List<string> checkclass1 = new List<string>() { "MW01", "MW10", "MW31" };
            if (checkclass1.Contains(P_MW_DSN.FORM_CODE))
            {
                bool blankItem = true;
                if (P_MW_RECORD_ITEMs_CLASS_I != null) for (int i = 0; i < P_MW_RECORD_ITEMs_CLASS_I.Count; i++) if (!string.IsNullOrWhiteSpace(P_MW_RECORD_ITEMs_CLASS_I[i].MW_ITEM_CODE) || !string.IsNullOrWhiteSpace(P_MW_RECORD_ITEMs_CLASS_I[i].LOCATION_DESCRIPTION) || !string.IsNullOrWhiteSpace(P_MW_RECORD_ITEMs_CLASS_I[i].RELEVANT_REFERENCE)) blankItem = false;
                if (blankItem) yield return new ValidationResult("Please enter at least one Minor Work Item.", new List<string> { "ALERT" });
            }
            List<string> checkclass2 = new List<string>() { "MW02", "MW03", "MW04" };
            if (checkclass2.Contains(P_MW_DSN.FORM_CODE))
            {
                bool blankItem = true;
                if (P_MW_RECORD_ITEMs_CLASS_II != null) for (int i = 0; i < P_MW_RECORD_ITEMs_CLASS_II.Count; i++) if (!string.IsNullOrWhiteSpace(P_MW_RECORD_ITEMs_CLASS_II[i].MW_ITEM_CODE) || !string.IsNullOrWhiteSpace(P_MW_RECORD_ITEMs_CLASS_II[i].LOCATION_DESCRIPTION) || !string.IsNullOrWhiteSpace(P_MW_RECORD_ITEMs_CLASS_II[i].RELEVANT_REFERENCE)) blankItem = false;
                if (blankItem) yield return new ValidationResult("Please enter at least one Minor Work Item.", new List<string> { "ALERT" });
            }
            List<string> checkclass3 = new List<string>() { "MW04", "MW05", "MW06", "MW32" };
            if (checkclass3.Contains(P_MW_DSN.FORM_CODE))
            {
                bool blankItem = true;
                if (P_MW_RECORD_ITEMs_CLASS_III != null) for (int i = 0; i < P_MW_RECORD_ITEMs_CLASS_III.Count; i++) if (!string.IsNullOrWhiteSpace(P_MW_RECORD_ITEMs_CLASS_III[i].MW_ITEM_CODE) || !string.IsNullOrWhiteSpace(P_MW_RECORD_ITEMs_CLASS_III[i].LOCATION_DESCRIPTION) || !string.IsNullOrWhiteSpace(P_MW_RECORD_ITEMs_CLASS_III[i].RELEVANT_REFERENCE)) blankItem = false;
                if (blankItem) yield return new ValidationResult("Please enter at least one Minor Work Item.", new List<string> { "ALERT" });
            }


            //ValidationResult("Please enter " + v[VAR_PROP.LABEL] + ".", new List<string> { propName })


            //P_MW_RECORD_ITEMs_CLASS_I_0__LOCATION_DESCRIPTION


            //model.P_MW_RECORD.S_FORM_TYPE_CODE
            //yield return null;
            /*
            yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICATION.UUID                        ");
            yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICATION.FILE_REFERENCE_NO           ");
            yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICATION.BR_NO                       ");
            yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICATION.BRANCH_NO                   ");*/
            //P_MW_DSN.GetType().GetProperties()[0].CanRead
            //P_MW_DSN.GetType().GetProperties()[0].PropertyType.FullName "System.String"
            //P_MW_DSN.GetType().GetProperties()[0].Name
        }
    }
}
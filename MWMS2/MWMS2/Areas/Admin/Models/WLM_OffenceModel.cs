using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.Admin.Models
{
    public class WLM_OffenceModel : DisplayGrid
    {
        public List<string> DESCRIPTION_ENG { get; set; }
        public List<string> TYPE { get; set; }
        public List<SelectListItem> OffenseType {
            get {
                using (EntitiesWarningLetter db = new EntitiesWarningLetter())
                {
                  var  q =  from st in db.W_S_SYSTEM_TYPE
                           join sv in db.W_S_SYSTEM_VALUE on st.UUID equals sv.S_SYSTEM_TYPE_ID
                    where st.TYPE == "Group_Of_Offense"

                            select sv;

                    List<SelectListItem> result = new List<SelectListItem>();
                    foreach (var item in q) {
                        result.Add(new SelectListItem()
                        {
                            Text=item.DESCRIPTION_ENG,
                            Value= item.DESCRIPTION_ENG

                        });
                    }

                    return result;
                }
              
                 
            }

        }
    }

    //public class Offence
    //{
    //    public string DESCRIPTION_ENG { get; set; }
    //}

    public class ScoreListModel : DisplayGrid, IValidatableObject
    {
        public List<SelectListItem> OffenseType
        {
            get
            {
                using (EntitiesWarningLetter db = new EntitiesWarningLetter())
                {
                    var q = from st in db.W_S_SYSTEM_TYPE
                            join sv in db.W_S_SYSTEM_VALUE on st.UUID equals sv.S_SYSTEM_TYPE_ID
                            where st.TYPE == "Group_Of_Offense"

                            select sv;

                    List<SelectListItem> result = new List<SelectListItem>();
                    foreach (var item in q)
                    {
                        result.Add(new SelectListItem()
                        {
                            Text = item.DESCRIPTION_ENG,
                            Value = item.DESCRIPTION_ENG

                        });
                    }

                    return result;
                }


            }

        }
        public string Offense_Id { get; set; }
        public string Type { get; set; }
        public string Offense_Name { get; set; }
        public List<string> Effect_Date { get; set; }
        public List<string> Score { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield return ValidationUtil.Validate_Mandatory(this, "Type");
            yield return ValidationUtil.Validate_Mandatory(this, "Offense_Name");
        }
    }
}
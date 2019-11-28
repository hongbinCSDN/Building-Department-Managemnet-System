using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Constant;
using MWMS2.Models;
using MWMS2.Utility;

namespace MWMS2.Areas.Registration.Models
{
    public class Fn01Search_RIASearchModel : DisplayGrid
    {
        public string FileRef { get; set; }
        public string RegNo { get; set; }
        public string HKID { get; set; }
        public string PassportNo { get; set; }
        public string SurnName { get; set; }
        public string GivenName { get; set; }
        public string ChiName { get; set; }
        public string Address { get; set; }



        public List<string> Prb { get; set; }
        public List<string> Qcode { get; set; }
        public List<string> DisDiv { get; set; }



        public string Pnrc { get; set; }
        public IEnumerable<SelectListItem> PnrcList { set; get; } = SystemListUtil.RetrievePNAPByType();
        //ServiceInBS
        /*
         <option>-- Select --</option>
         <option>BY SMS</option>
         <option>BY EMAIL</option>
         <option>NOT WISH</option>
         <option>N/A</option>
             */
        public string ServiceInBS { get; set; }
        public IEnumerable<SelectListItem> ServiceInBSList { set; get; } = SystemListUtil.RetrieveServiceInBSByRegType(RegistrationConstant.REGISTRATION_TYPE_IP);
        /*
                <select>
                    <option>-- Select --</option>
                    <option>1</option>
                    <option>2</option>
                    <option>3</option>
                    <option>4</option>
                    <option>-</option>
                </select>
         */

        public string Sex { get; set; }

        public IEnumerable<SelectListItem> SexList
        {
            get
            {
                return new List<SelectListItem>{
                    new SelectListItem { Text = "- Select -", Value = ""}
                    , new SelectListItem { Text = "M", Value = "M"}
                    , new SelectListItem { Text = "F", Value = "F"}
                };
            }
        }
        //public List<string> SelectedPRBList { get; set; }
        public List<SelectListItem> RetrievePRBByType()
        {
            
            var list = (from sv in SystemListUtil.GetSVListByType(RegistrationConstant.SYSTEM_TYPE_PROF_REGISTRATION_BOARD)
                        select new SelectListItem()
                        {
                            Text = sv.ENGLISH_DESCRIPTION,
                            Value = sv.UUID,
                        }
                    ).ToList(); 
            list.RemoveAll(x => x.Text == "AP AND RSE" || x.Text == "ERB (Geotechnical)" || x.Text == "ERB (Civil / Structural )");
      
       
            return list;
        }
        //public List<string> SelectedQCList { get; set; }
        public List<SelectListItem> RetrieveQCByType()
        {
            var list = SystemListUtil.RetrieveQCByType(RegistrationConstant.REGISTRATION_TYPE_IP);
            list.RemoveAll(x => x.Text != "RI(A)" && x.Text != "RI(E)" && x.Text != "RI(S)");


            return list;
     
        }

        //public List<string> SelectedDisciplinesDiv { get; set; }
        public List<SelectListItem> RetrieveDisDivByType()
        {
            return SystemListUtil.RetrieveDisDivByType();
        }


    }
}
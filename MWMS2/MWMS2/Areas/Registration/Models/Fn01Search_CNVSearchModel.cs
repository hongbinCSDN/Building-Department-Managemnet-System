using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.Registration.Models
{

    public class Fn01Search_CNVSearchModel : DisplayGrid
    {
        public String HKID { get; set; }
        public String PassportNo { get; set; }
        public String Surname { get; set; }
        public String GivenName { get; set; }
        public String ChiName { get; set; }
        public String BRNo { get; set; }
        public String ProprietorName { get; set; }
        public String ComName { get; set; }
        public String OrdReg { get; set; }
        public String SuspReason { get; set; }
        public String CnvSource { get; set; }
        public DateTime? FromDateOfOffence{ get; set; }
        public DateTime? ToDateOfOffence { get; set; }
        public DateTime? FromDateOfJudgement { get; set; }
        public DateTime? ToDateOfJudgement { get; set; }
        public String Ref { get; set; }
        public String CnvType { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }   
        public String Date { get; set; }

        public List<SelectListItem> RetrieveCNVSORUCE()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                var roleList = SessionUtil.LoginPost.SYS_POST_ROLE.Select(x => x.SYS_ROLE_ID).ToList();
                var sys = db.C_S_SYSTEM_VALUE.Where(x => roleList.Contains(x.C_S_USER_GROUP_CONV_INFO.SelectMany(y => y.SYS_ROLE_ID)));
                var q = db.C_S_USER_GROUP_CONV_INFO.Where(x => roleList.Contains(x.SYS_ROLE_ID)).Include(x => x.C_S_SYSTEM_VALUE).ToList();
                
                List<C_S_SYSTEM_VALUE> result = new List<C_S_SYSTEM_VALUE>();

                result.AddRange(q.Select(x => x.C_S_SYSTEM_VALUE));
                

                selectListItems.Add(new SelectListItem
                {
                    Text = "- All -",
                    Value = "",
                    Selected = true
                });
                using (EntitiesRegistration db2 = new EntitiesRegistration())
                {
                    selectListItems.AddRange((from x in result
                                              orderby x.CODE
                                              select new SelectListItem()
                                              {
                                                  Text = x.ENGLISH_DESCRIPTION,
                                                  Value = x.UUID,
                                              }
                                 ).ToList());

                }


              

            };
            //List<SelectListItem> selectListItems = new List<SelectListItem>();
            //selectListItems.Add(new SelectListItem
            //{
            //    Text = "- All hi -",
            //    Value = "",
            //    Selected = true
            //});

            //selectListItems.AddRange(from gci in SystemListUtil.GetCNVListbySysPostId(SessionUtil.LoginPost.SYS_POST_ROLE.Select(o => o.SYS_ROLE_ID).ToString()) //RegistrationConstant.SYSTEM_TYPE_CONVICTION_SOURCE
            //                         select new SelectListItem()
            // {
            //     Text = gci.C_S_SYSTEM_VALUE.ENGLISH_DESCRIPTION,
            //     Value = gci.UUID,
            // }

            // );

            return selectListItems;
        }





    }

   
}
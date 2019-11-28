using MWMS2.Constant;
using MWMS2.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.MWProcessing.Models
{
    public class Fn02MWUR_OutgoingModel
    {
        public P_MW_DSN P_MW_DSN { get; set; }

        public string LetterTemplateNo { get; set; }
        public string LetterType { get; set; }

        public string ScanType { get; set; }

        public List<SelectListItem> LetterTypeList
        {
            get
            {
                return new List<SelectListItem>()
                {
                    new SelectListItem(){Text = "- Select -" , Value = ""},
                    new SelectListItem(){Text = ProcessingConstant.LETTER_TEMPLATE_TYPE_ACKNOWLEDGEMENT , Value = ProcessingConstant.LETTER_TEMPLATE_TYPE_ACKNOWLEDGEMENT},
                    new SelectListItem(){Text = ProcessingConstant.LETTER_TEMPLATE_TYPE_CONDITIONAL , Value = ProcessingConstant.LETTER_TEMPLATE_TYPE_CONDITIONAL},
                    new SelectListItem(){Text = ProcessingConstant.LETTER_TEMPLATE_TYPE_REFUSE , Value = ProcessingConstant.LETTER_TEMPLATE_TYPE_REFUSE},
                    new SelectListItem(){Text = ProcessingConstant.LETTER_TEMPLATE_TYPE_REPLY_LETTER , Value = ProcessingConstant.LETTER_TEMPLATE_TYPE_REPLY_LETTER},
                    new SelectListItem(){Text = ProcessingConstant.LETTER_TEMPLATE_TYPE_INTERIM_REPLY , Value = ProcessingConstant.LETTER_TEMPLATE_TYPE_INTERIM_REPLY},
                    new SelectListItem(){Text = ProcessingConstant.LETTER_TEMPLATE_TYPE_IO_NOTIFICATION , Value = ProcessingConstant.LETTER_TEMPLATE_TYPE_IO_NOTIFICATION},
                    new SelectListItem(){Text = ProcessingConstant.LETTER_TEMPLATE_TYPE_REMINDER , Value = ProcessingConstant.LETTER_TEMPLATE_TYPE_REMINDER},
                    new SelectListItem(){Text = ProcessingConstant.LETTER_TEMPLATE_TYPE_MEMO , Value = ProcessingConstant.LETTER_TEMPLATE_TYPE_MEMO},
                    new SelectListItem(){Text = ProcessingConstant.LETTER_TEMPLATE_TYPE_EMAIL , Value = ProcessingConstant.LETTER_TEMPLATE_TYPE_EMAIL},
                    new SelectListItem(){Text = ProcessingConstant.LETTER_TEMPLATE_TYPE_OTHER , Value = ProcessingConstant.LETTER_TEMPLATE_TYPE_OTHER},
                    new SelectListItem(){Text = ProcessingConstant.LETTER_TEMPLATE_TYPE_D_LETTER , Value = ProcessingConstant.LETTER_TEMPLATE_TYPE_D_LETTER},
                    new SelectListItem(){Text = ProcessingConstant.LETTER_TEMPLATE_TYPE_REFERRAL , Value = ProcessingConstant.LETTER_TEMPLATE_TYPE_REFERRAL}
                };
            }
        }

        public List<P_LETTER_INFO> LetterTemplateList { get; set; }
    }
}
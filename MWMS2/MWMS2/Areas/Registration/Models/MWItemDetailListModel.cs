using MWMS2.Constant;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Areas.Registration.Models
{
    public class MWItemDetailListModel
    {
        public string m_UUID { get; set; }
        public string m_Master_ID { get; set; }
        public DateTime? m_APPLICATION_DATE { get; set; }
        public string m_APPLICATION_FORM_ID { get; set; }
        public string m_APPROVED_BY { get; set; }
        public DateTime? m_APPROVED_DATE { get; set; }
        public string m_STATUS_CODE { get; set; }
        public DateTime? m_Created_Date { get; set; }

        public string m_Apply_Item { get; set; }
        public string m_Approved_Item { get; set; }

       

        public string ApprovedMWItemStatus { get; set; }
        public string GetApplyMWItemApply()
        {
             string MWItem = "";
         
            var q = SystemListUtil.GetMWItemApplyApprovedDetailList(m_UUID, RegistrationConstant.MWITEM_APPLY);
            if (q.Any())
            {
                int i =1;
                foreach (var item in q)
                {
                    
                    MWItem += item.CODE.Substring(4) + ",";
              
                }
                MWItem = MWItem.Substring(0, MWItem.Length - 1);
            }
            return MWItem;

        }

        public string GetApplyMWItemApproved()
        {
            string MWItem = "";
            var q = SystemListUtil.GetMWItemApplyApprovedDetailList(m_UUID, RegistrationConstant.MWITEM_APPROVED);
            if (q.Any())
            {
                foreach (var item in q)
                {
                    MWItem += item.CODE.Substring(4) + ",";
                }
                MWItem = MWItem.Substring(0, MWItem.Length - 1);
            }

            return MWItem;



        }
        public List<string> SelectedApprovedMWItem { get; set; }
        public List<string> SelectedApplyMWItem { get; set; }

        public Dictionary<string, string> ApprovedItems { get; set; }
        public Dictionary<string, string> ApplyItems { get; set; }

        public List<string> NewVerApprovedItems { get; set; }

        public string m_MWItemDeleteByApplicant { get; set; }
        public string m_MWItemSaveVersion { get; set; }

        public string[] m_NewSelectedMWitem { get; set; }
        public bool isTray { get; set; }
        public Dictionary<string, string> m_NewSelectedMWitemSupportedBy { get; set; }
    }
}
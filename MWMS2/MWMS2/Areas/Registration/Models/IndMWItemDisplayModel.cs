using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MWMS2.Entity;
using MWMS2.Utility;
using MWMS2.Constant;
using System.Web.Mvc;
using MWMS2.Services;
namespace MWMS2.Areas.Registration.Models
{
    public class IndMWItemDisplayModel
    {
        public string IndApplication_UUID { get; set; }
        public List<MWItemDetailListModel> MWItemDetailList { get; set; }
        public MWItemDetailListModel SelectedMWItemDetail { get; set; } = new MWItemDetailListModel();



        public List<string> LatestApprovedMWItem { get; set; }

        public IEnumerable<SelectListItem> GetAppFormCodeList
        {
            get
            {
                return
                        (SystemListUtil.GetSVListByRegTypeNType(RegistrationConstant.REGISTRATION_TYPE_MWIA,
                            RegistrationConstant.SYSTEM_TYPE_APPLICATION_FORM)
                            .Select(o => new SelectListItem() { Text = o.CODE, Value = o.UUID }));
            }
        }
        public IEnumerable<SelectListItem> GetApprovedByList
        {
            get
            {
                return (new List<SelectListItem>() { new SelectListItem() { Text = "- Select -", Value = "" } })
                        .Concat(SystemListUtil.GetSVListByRegTypeNType(RegistrationConstant.REGISTRATION_TYPE_MWIA,
                            RegistrationConstant.SYSTEM_TYPE_APPROVED_BY_LIST)
                            .Select(o => new SelectListItem() { Text = o.CODE, Value = o.CODE }));
            }
        }

        public string GetAppFormCode(string uuid, bool isTray)
        {
            if (isTray)
                return SystemListUtil.GetSVByUUID(uuid).CODE + "*";
            else
                return SystemListUtil.GetSVByUUID(uuid).CODE;
        }




        public List<C_S_SYSTEM_VALUE> GetMWItemFullList()
        {
            List<C_S_SYSTEM_VALUE> svlist = new List<C_S_SYSTEM_VALUE>();
            svlist = SystemListUtil.GetMWItemFullListByClass(RegistrationConstant.MW_CLASS_III);
            //List<SimpleDataModel> sdm = new List<SimpleDataModel>();
            return svlist;

        }
        public List<C_S_MW_IND_CAPA_MAIN> GetNewMWItemFullList()
        {
            return SystemListUtil.GetNewMWItemFullList(); ;
        }
        public List<string> GetCurrentMWItemApproved()
        {
            this.LatestApprovedMWItem = new List<string>();


            var query = SystemListUtil.GetLatesetMWItemApprovedDetailList(IndApplication_UUID);
            foreach (var item in query)
            {
                LatestApprovedMWItem.Add(item.ITEM_DETAILS_ID + item.SUPPORTED_BY_ID);

            }

            return LatestApprovedMWItem;



        }
        public List<string> GetCurrentMWItemApply()
        {

            List<string> latestApprovedMWItem = new List<string>();
            var queryM = MWItemDetailList.OrderByDescending(x => x.m_Created_Date);
            string LatestRecordUUID = "";
            if (queryM != null)
                LatestRecordUUID = queryM.FirstOrDefault().m_UUID;

            var query = SystemListUtil.GetSelectedMWItemApplyDetailList(LatestRecordUUID);
            foreach (var item in query)
            {
                latestApprovedMWItem.Add(item.ITEM_DETAILS_ID + item.SUPPORTED_BY_ID);

            }

            return latestApprovedMWItem;



        }
        public void GetCurrentMWItemApprovedbyUUID()
        {
            this.SelectedMWItemDetail.SelectedApprovedMWItem = new List<string>();
            var query = SystemListUtil.GetSelectedMWItemApprovedDetailList(SelectedMWItemDetail.m_UUID);
            foreach (var item in query)
            {
                SelectedMWItemDetail.SelectedApprovedMWItem.Add(item.ITEM_DETAILS_ID + item.SUPPORTED_BY_ID);
            }
        }
        public void GetCurrentMWItemApplybyUUID()
        {
            this.SelectedMWItemDetail.SelectedApplyMWItem = new List<string>();
            var query = SystemListUtil.GetSelectedMWItemApplyDetailList(SelectedMWItemDetail.m_UUID);

            foreach (var item in query)
            {
                SelectedMWItemDetail.SelectedApplyMWItem.Add(item.ITEM_DETAILS_ID + item.SUPPORTED_BY_ID);
            }


        }
        public string MWItemDeleteByApplicant { get; set; }
        public string MWItemSaveVersion { get; set; }
        public IEnumerable<SelectListItem> VersionSelector
        {
            get
            {
                List<SelectListItem> selectListItems = new List<SelectListItem>();
                selectListItems.Add(new SelectListItem() { Text = "Old", Value = "Old" });
                selectListItems.Add(new SelectListItem() { Text = "New", Value = "New", Selected = true });
                return selectListItems;


            }
        }

        public string[] NewSelectedMWitem { get; set; }

        public Dictionary<string, string> NewSelectedMWitemSupportedBy { get; set; }

        public void GetFinalTrayMWItem()
        {
            RegistrationMWIAService ss = new RegistrationMWIAService();
            FinalTrayItemList = ss.GetFinalTrayMWItemByMasterUUID(IndApplication_UUID);
        }

        public List<C_MW_IND_CAPA_FINAL> FinalTrayItemList;

        public void GetTrayMWItem()
        {
            RegistrationMWIAService ss = new RegistrationMWIAService();
            TrayItemList = ss.GetTrayMWItemByMasterUUID(SelectedMWItemDetail.m_UUID);
        }

        public List<C_MW_IND_CAPA_DETAIL> TrayItemList;

        public bool TrayDisplay { get; set; }
    }
}
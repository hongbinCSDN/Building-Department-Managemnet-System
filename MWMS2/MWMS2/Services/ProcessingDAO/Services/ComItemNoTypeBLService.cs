using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Services.ProcessingDAO.DAO;

namespace MWMS2.Services.ProcessingDAO.Services
{
    public class ComItemNoTypeBLService
    {
        private ProcessingMwItemDAOService _MWItemDA;
        protected ProcessingMwItemDAOService MWItemDA
        {
            get
            {
                return _MWItemDA ?? (_MWItemDA = new ProcessingMwItemDAOService());
            }
        }
        public MwClassListModel GetItemNosAndType(string classType)
        {
            Dictionary<string, CheckboxModel> mwItemNos = new Dictionary<string, CheckboxModel>();
            Dictionary<string, CheckboxModel> mwItemTypes = new Dictionary<string, CheckboxModel>();
            foreach (var item in MWItemDA.GetMWItemNoAndType(classType))
            {
                if (!mwItemTypes.ContainsKey(item.MWItemType))
                {
                    mwItemTypes.Add(item.MWItemType, new CheckboxModel()
                    {
                        Code = item.MWItemType
                        ,
                        Description = item.MWItemType
                        ,
                        Group = classType + "_" + item.MWItemType
                        ,
                        UUID = item.MWItemTypeUUID
                    });
                }

                if (!mwItemNos.ContainsKey(item.MWItemNo))
                {
                    mwItemNos.Add(item.MWItemNo, new CheckboxModel()
                    {
                        Code = item.MWItemNo
                    ,
                        Description = item.MWItemNo
                    ,
                        Group = classType + "_" + item.MWItemType
                    ,
                        UUID = item.MWItemUUID
                    });
                }
                else
                {
                    mwItemNos[item.MWItemNo].Group += "|" + classType + "_" + item.MWItemType;
                }
            }

            return new MwClassListModel()
            {
                MWItemTypes = mwItemTypes.OrderBy(m => m.Key).ToDictionary(m => m.Key, o => o.Value)
                ,
                MWItemNos = mwItemNos
            };
        }
    }
}
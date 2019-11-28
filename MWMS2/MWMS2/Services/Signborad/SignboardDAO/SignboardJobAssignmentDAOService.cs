using MWMS2.Areas.Signboard.Models;
using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Services.Signborad.SignboardDAO
{
    public class SignboardJobAssignmentDAOService
    {
        public List<List<object>> GetSPOAssignmentList()
        {
            Dictionary<string, object> queryParameters = new Dictionary<string, object>();
            List<List<object>> data = new List<List<object>>();

            string queryString = ""
                + " \r\n SELECT R.UUID, R.REFERENCE_NO, R.RECEIVED_DATE, S.LOCATION_OF_SIGNBOARD, P.NAME_CHINESE, P.NAME_ENGLISH, "
                + " \r\n\t R.TO_USER_ID, R.PO_USER_ID, R.SPO_USER_ID, R.IS_FOR_SPO_ASSIGNMENT, R.FORM_CODE"
                + " \r\n FROM B_SV_RECORD R, B_SV_SIGNBOARD S, B_SV_PERSON_CONTACT P"
                + " \r\n WHERE R.SV_SIGNBOARD_ID = S.UUID AND R.PAW_ID = P.UUID"
                + " \r\n AND R.WF_STATUS = :WF_STATUS"
                + " \r\n AND R.SPO_ASSIGNMENT_DATE IS NULL";

            queryParameters.Add("WF_STATUS", SignboardConstant.WF_MAP_ASSIGING);


            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                using (DbConnection conn = db.Database.Connection)
                {
                    DbDataReader dr = CommonUtil.GetDataReader(conn, queryString, queryParameters);
                    data = CommonUtil.convertToList(dr);
                    conn.Close();
                }
            }
            return data;

        }

        static public List<SelectListItem> GetScuTeamByPosition(string position)
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                SScuTeamManagerService ss = new SScuTeamManagerService();
                var list = ss.getWFUserList(position);
                //var list = db.B_S_SCU_TEAM.Where(x => x.POSITION == position).Where(x => x.SYS_POST_ID != null).ToList();
                if(list != null && list.Count() > 0)
                {
                    using(EntitiesAuth auth = new EntitiesAuth())
                    {
                        foreach (var item in list)
                        {
                            string id = SignboardConstant.S_USER_ACCOUNT_RANK_TO.Equals(position) ? item.CHILD_SYS_POST_ID : item.SYS_POST_ID;
                            string name = auth.SYS_POST.Find(id).BD_PORTAL_LOGIN;
                            //SelectListItem list_item = new SelectListItem()
                            //{
                            //    Text = name, Value = id
                            //};
                            //selectListItems.Add(list_item);

                            //SYS_POST user = auth.SYS_POST.Find(item.SYS_POST_ID);
                            SelectListItem list_item = new SelectListItem();
                            list_item.Text = name;
                            list_item.Value = id;
                            if (selectListItems != null && selectListItems.Count() > 0)
                            {
                                bool isExist = false;
                                for (int i = 0; i < selectListItems.Count(); i++)
                                {
                                    if (selectListItems[i].Value.Equals(list_item.Value))
                                    {
                                        isExist = true;
                                    }
                                }
                                if (!isExist)
                                {
                                    selectListItems.Add(list_item);
                                }
                            }
                            else
                            {
                                selectListItems.Add(list_item);
                            }

                        }
                    }
                }
            }
            return selectListItems;
        }


    }
}
using MWMS2.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Services.AdminService.DAO
{
    public class AuditLogDAOService
    {
        String SearchAL_q = ""
     + "\r\n" + "\t" + " Select  LOG_DATE, LOGIN, "
+ "ACTION "
     + ", DESCRIPTION,IPADDRESS from sys_log where 1=1 "


     ;

        //        String SearchAL_q = ""
        //            + "\r\n" + "\t" + " Select  LOG_DATE, LOGIN, " 
        //+ " case when ACTION='Login-dp'             THEN 'Portal Login' "
        //+ "  when ACTION='Logout'               THEN 'Logout' "
        //+ "  when ACTION='DeleteToDetails'      THEN 'Delete Record' "
        //+ "  when ACTION='Login-login'          THEN 'Password Login' else ACTION END AS ACTION "
        //            +", DESCRIPTION,IPADDRESS from sys_log where 1=1 "


        //            ;
        private string SearchAL_whereQ(AuditLogSearchModel model)
        {
            string whereQ = "";
            if (model.FromDate != null)
            {
                whereQ += "\r\n\t" + "AND LOG_DATE >= :DateFrom";
                model.QueryParameters.Add("DateFrom", model.FromDate);
            }
            if (model.ToDate != null)
            {
                whereQ += "\r\n\t" + "AND LOG_DATE <= :DateTo";
                model.QueryParameters.Add("DateTo", model.ToDate);
            }
            if (!string.IsNullOrWhiteSpace(model.Action))
            {
                whereQ += "\r\n\t" + "AND UPPER(ACTION) like :act";
                model.QueryParameters.Add("act","%"+model.Action.Trim().ToUpper()+"%");

            }
            if (!string.IsNullOrWhiteSpace(model.Post))
            {
                whereQ += "\r\n\t" + "AND UPPER(LOGIN) like :post";
                model.QueryParameters.Add("post", "%" + model.Post.Trim().ToUpper() + "%");

            }
            if (!string.IsNullOrWhiteSpace(model.Detail))
            {
                whereQ += "\r\n\t" + "AND UPPER(DESCRIPTION) like :detail";
                model.QueryParameters.Add("detail", "%" + model.Detail.Trim().ToUpper() + "%");

            }
            return whereQ;
        }

            public string ExportAL(AuditLogSearchModel model)
        {
            model.Query = SearchAL_q;
            model.QueryWhere = SearchAL_whereQ(model);
            //DisplayGrid dlr = new DisplayGrid() { Query = SearchGCA_q, Columns = Columns, Parameters = post };
            return model.Export("ExportData");
        }

        public AuditLogSearchModel SearchAL(AuditLogSearchModel model)
        {
            model.Query = SearchAL_q;

            model.QueryWhere = SearchAL_whereQ(model);

            model.Search();
            return model;
        }
    }
}
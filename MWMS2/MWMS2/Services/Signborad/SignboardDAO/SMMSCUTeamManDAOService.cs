using MWMS2.Areas.Admin.Models;
using MWMS2.Entity;
using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Utility;
using System.Data.Entity;
using MWMS2.Controllers;

namespace MWMS2.Services
{
    public class SMMSCUTeamManDAOService
    {
        public Dictionary<string, object> AjaxTeamData()
        {
            DisplayGrid dg = new DisplayGrid() { Rpp = -1 };
            dg.Query = ""
             + "\r\n\t" + " SELECT T1.UUID, T1.PARENT_ID, T2.AREA_CODE, T1.USER_ACCOUNT_ID FROM"
             + "\r\n\t" + " B_S_SCU_TEAM T1"
             + "\r\n\t" + " LEFT JOIN B_S_SCU_TEAM_AREA T2"
             + "\r\n\t" + " ON T1.UUID = T2.AREA_CODE";
            dg.Search();
            List<Dictionary<string, object>> parents = dg.Data.Where(o => string.IsNullOrWhiteSpace(o["PARENT_ID"].ToString())).ToList();
            //id: "node__A", nodeId: "A", desc: "Report Received", children: 
            for(int i =0; i < dg.Data.Count; i++)
            {
                dg.Data[i]["id"] = string.IsNullOrWhiteSpace(dg.Data[i]["UUID"] as string) ? "" : "node__" + dg.Data[i]["UUID"];
                dg.Data[i]["nodeId"] = string.IsNullOrWhiteSpace(dg.Data[i]["UUID"] as string) ? "" : dg.Data[i]["UUID"];
                dg.Data[i]["desc"] = string.IsNullOrWhiteSpace(dg.Data[i]["USER_ACCOUNT_ID"] as string) ? "" : dg.Data[i]["USER_ACCOUNT_ID"].ToString().Substring(0, 6);
            }
            if (parents.Count > 0)
            {
                parents[0]["desc"] = "SYSTEM";
                loadNodeChilds(dg.Data, parents[0]);
                return parents[0];
            } else
            {
                return null;
            }

        }
        private void loadNodeChilds(List<Dictionary<string, object>> data, Dictionary<string, object> parent)
        {
            List<Dictionary<string, object>> childs = data.Where(o => o["PARENT_ID"].ToString() == parent["UUID"].ToString()).ToList();
            parent["children"] = childs;
            if (childs != null) for (int i = 0; i < childs.Count; i++)
                {
                    loadNodeChilds(data, childs[i]);
                }
        }



        public List<Dictionary<string, object>> StructuralAddress(string[] selectColumns, string[] orderByColumns
           , bool eqUNIT, string UNIT
           , bool eqFLOOR , string FLOOR
           , bool eqBUILDING , string BUILDING
           , bool eqSTREETNO , string STREETNO
           , bool eqST_NAME , string ST_NAME
           , bool eqSYS_DISTRICT , string SYS_DISTRICT
           , string ADR_UNIT_ID
           , string ADR_BLK_ID)
        {

            DisplayGrid model = new DisplayGrid
            {
                Query = " SELECT DISTINCT " + string.Join(",", selectColumns) + " FROM BCIS_ADDRESS",
                Rpp = 50,
                Sort = string.Join(",", orderByColumns)
            };

            string whereQ = "WHERE 1=1";
            if (!string.IsNullOrWhiteSpace(UNIT)) {

                whereQ += " AND UNIT " + (eqUNIT ? "=" : "LIKE") + " :UNIT"; model.QueryParameters.Add("UNIT", eqUNIT ? UNIT.Trim().ToUpper() : ("%" + UNIT.Trim().ToUpper() + "%"));
            }
            if (!string.IsNullOrWhiteSpace(FLOOR)) {
                whereQ += " AND FLOOR " + (eqFLOOR ? "=" : "LIKE") + " :FLOOR"; model.QueryParameters.Add("FLOOR", eqFLOOR ? FLOOR.Trim().ToUpper() : ("%" + FLOOR.Trim().ToUpper() + "%"));
            }
            if (!string.IsNullOrWhiteSpace(BUILDING)) {
                whereQ += " AND BUILDING " + (eqBUILDING ? "=" : "LIKE") + " :BUILDING"; model.QueryParameters.Add("BUILDING", eqBUILDING ? BUILDING.Trim().ToUpper() : ("%" + BUILDING.Trim().ToUpper() + "%"));
            }
            if (!string.IsNullOrWhiteSpace(STREETNO)) {
                whereQ += " AND STREETNO " + (eqSTREETNO ? "=" : "LIKE") + " :STREETNO"; model.QueryParameters.Add("STREETNO", eqSTREETNO ? STREETNO.Trim().ToUpper() : ("%" + STREETNO.Trim().ToUpper() + "%"));
            }
            if (!string.IsNullOrWhiteSpace(ST_NAME)) {
                whereQ += " AND ST_NAME " + (eqST_NAME ? "=" : "LIKE") + " :ST_NAME"; model.QueryParameters.Add("ST_NAME", eqST_NAME ? ST_NAME.Trim().ToUpper() : ("%" + ST_NAME.Trim().ToUpper() + "%"));
            }
            if (!string.IsNullOrWhiteSpace(SYS_DISTRICT)) {
                whereQ += " AND SYS_DISTRICT " + (eqSYS_DISTRICT ? "=" : "LIKE") + " :SYS_DISTRICT"; model.QueryParameters.Add("SYS_DISTRICT", eqSYS_DISTRICT ? SYS_DISTRICT.Trim().ToUpper() : ("%" + SYS_DISTRICT.Trim().ToUpper() + "%"));
            }
            if (!string.IsNullOrWhiteSpace(ADR_UNIT_ID)) {
                whereQ += " AND ADR_UNIT_ID = :ADR_UNIT_ID"; model.QueryParameters.Add("ADR_UNIT_ID",  ADR_UNIT_ID.Trim().ToUpper());
            }
            if (!string.IsNullOrWhiteSpace(ADR_BLK_ID)) {
                whereQ += " AND ADR_BLK_ID = :ADR_BLK_ID"; model.QueryParameters.Add("ADR_BLK_ID",  ADR_BLK_ID.Trim().ToUpper() );
            }
            model.QueryWhere = whereQ;
            model.Search();
            return model.Data;
        }


        public List<Dictionary<string, object>> Rank(AtcpModel model)
        {
            model.Query = "SELECT UUID, CODE, DESCRIPTION FROM SYS_RANK";
            if (!string.IsNullOrWhiteSpace(model.id))
            {
                model.QueryWhere = "WHERE UUID = :id";
                model.QueryParameters.Add("id", model.id);
            }
            else if (!string.IsNullOrWhiteSpace(model.term))
            {
                model.QueryWhere = "WHERE CODE LIKE :k1 OR DESCRIPTION LIKE :k12";
                model.QueryParameters.Add("k1", model.term.ToUpper() + "%");
                model.QueryParameters.Add("k2", model.term.ToUpper() + "%");
            }
            model.Rpp = 50;
            model.Sort = "CODE";
            model.Search();
            return model.Data;
        }
        public List<Dictionary<string, object>> Unit(AtcpModel model)
        {
            
            model.Query = "SELECT T1.UUID, T2.CODE AS SECTION, T1.CODE AS UNIT, T1.DESCRIPTION FROM SYS_UNIT T1 LEFT JOIN SYS_SECTION T2 ON T2.UUID = T1.SYS_SECTION_ID";

            if (!string.IsNullOrWhiteSpace(model.id))
            {
                model.QueryWhere = "WHERE T1.UUID = :id";
                model.QueryParameters.Add("id", model.id);
            }
            else if (!string.IsNullOrWhiteSpace(model.term))
            {
                model.QueryWhere = "WHERE T2.CODE LIKE :k1 OR T1.CODE LIKE :k2 OR UPPER(T1.DESCRIPTION) LIKE :k3";
                model.QueryParameters.Add("k1", model.term.ToUpper() + "%");
                model.QueryParameters.Add("k2", model.term.ToUpper() + "%");
                model.QueryParameters.Add("k3", model.term.ToUpper() + "%");
            }
            model.Rpp = 50;
            model.Sort = "T2.CODE, T1.CODE, T1.DESCRIPTION";
            model.Search();
            return model.Data;
        }
        

    }
}
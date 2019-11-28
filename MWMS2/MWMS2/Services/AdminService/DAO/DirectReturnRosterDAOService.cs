using MWMS2.Areas.Admin.Models;
using MWMS2.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Services
{
    public class DirectReturnRosterDAOService
    {

        public void GetSearchSql(DirectReturnRosterModel model)
        {
            string sql = @"SELECT *
                            FROM   P_S_DIRECT_RETURN_ROSTER T1
                            WHERE  1 = 1 ";

            string sqlWhere = "";

            if (model.P_S_DIRECT_RETURN_ROSTER.ON_DUTY_DATE != null)
            {
                sqlWhere += "\r\n\t" + "AND T1.ON_DUTY_DATE" + " = :ON_DUTY_DATE";
                model.QueryParameters.Add("ON_DUTY_DATE", model.P_S_DIRECT_RETURN_ROSTER.ON_DUTY_DATE);
            }

            if (!string.IsNullOrWhiteSpace(model.P_S_DIRECT_RETURN_ROSTER.OFFICER_TO))
            {
                sqlWhere += "\r\n\t" + "AND T1.OFFICER_TO = :OFFICER_TO";
                model.QueryParameters.Add("OFFICER_TO", model.P_S_DIRECT_RETURN_ROSTER.OFFICER_TO.Trim().ToUpper());
            }

            if (!string.IsNullOrWhiteSpace(model.P_S_DIRECT_RETURN_ROSTER.OFFICER_PO))
            {
                sqlWhere += "\r\n\t" + "AND T1.OFFICER_PO = :OFFICER_PO";
                model.QueryParameters.Add("OFFICER_PO", model.P_S_DIRECT_RETURN_ROSTER.OFFICER_PO.Trim().ToUpper());
            }

            model.Query = sql;
            model.QueryWhere = sqlWhere;

        }

        public void Search(DirectReturnRosterModel model)
        {
            GetSearchSql(model);
            model.Search();
        }

        public string  ExportExcel(DirectReturnRosterModel model)
        {
            GetSearchSql(model);
            return model.Export("Direct Return Roster");
        }

        public int Add(P_S_DIRECT_RETURN_ROSTER model, EntitiesMWProcessing db)
        {
            db.P_S_DIRECT_RETURN_ROSTER.Add(model);
            return db.SaveChanges();
        }

        public int Update(P_S_DIRECT_RETURN_ROSTER model, EntitiesMWProcessing db)
        {
            P_S_DIRECT_RETURN_ROSTER record = db.P_S_DIRECT_RETURN_ROSTER.Where(w => w.UUID == model.UUID).FirstOrDefault();

            if (record != null)
            {
                record.OFFICER_TO = model.OFFICER_TO;
                record.OFFICER_PO = model.OFFICER_PO;
            }

            return db.SaveChanges();
        }

        public int Delete(P_S_DIRECT_RETURN_ROSTER model, EntitiesMWProcessing db)
        {
            P_S_DIRECT_RETURN_ROSTER record = db.P_S_DIRECT_RETURN_ROSTER.Where(w => w.UUID == model.UUID).FirstOrDefault();

            if (record != null)
            {
                db.P_S_DIRECT_RETURN_ROSTER.Remove(record);
            }

            return db.SaveChanges();
        }

        public P_S_DIRECT_RETURN_ROSTER GetRosterInfoByDate(DateTime onDutyDate)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                db.Configuration.LazyLoadingEnabled = false;
                return db.P_S_DIRECT_RETURN_ROSTER.Where(w => w.ON_DUTY_DATE == onDutyDate).FirstOrDefault();
            }
        }
    }
}
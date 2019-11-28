using MWMS2.Controllers;
using MWMS2.Entity;
using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MWMS2.DaoController
{
    public class DaoPEMAuditPercentage
    {
        private EntitiesMWProcessing db = new EntitiesMWProcessing();
        CommonFunction cf = new CommonFunction();
        public IQueryable<P_S_AUDIT_CHECK_PERCENTAGE> GetAuditCheckPercentageByYear(int year)
        {
            var query = db.P_S_AUDIT_CHECK_PERCENTAGE.Where(x => x.YEAR == year);
            return query;
        }

        public bool SetAuditCheckPercentage(P_S_AUDIT_CHECK_PERCENTAGE ACP)
        {
            try
            {
                if (!(ACP.PERCENTAGE >= 0 && ACP.PERCENTAGE <= 100))
                {
                    return false;
                }

                var query = GetAuditCheckPercentageByYear(Convert.ToInt32(ACP.YEAR));
                if (query.Count() == 0)
                {

                    ACP.UUID = System.Guid.NewGuid().ToString();

                    ACP.MODIFIED_DATE = System.DateTime.Now;
                    ACP.MODIFIED_BY = SystemParameterConstant.UserName;
                    ACP.CREATED_DATE = System.DateTime.Now;
                    ACP.CREATED_BY = SystemParameterConstant.UserName;
                    db.P_S_AUDIT_CHECK_PERCENTAGE.Add(ACP);
                }
                else
                {

                    query.FirstOrDefault().PERCENTAGE = ACP.PERCENTAGE;
                    query.FirstOrDefault().MODIFIED_DATE = System.DateTime.Now;
                    query.FirstOrDefault().MODIFIED_BY = SystemParameterConstant.UserName;
                    db.Entry(query.FirstOrDefault()).State = EntityState.Modified;

                }
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

    }
}
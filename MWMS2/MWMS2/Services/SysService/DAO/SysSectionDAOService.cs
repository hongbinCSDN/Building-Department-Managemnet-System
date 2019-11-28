using MWMS2.Areas.Admin.Models;
using MWMS2.Entity;
using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Services
{
    public class SysSectionDAOService
    {
        private const string SearchSql = @"SELECT *
                                            FROM   Sys_Section T1
                                            WHERE  1 = 1 ";

        private string SearchCriteria(Sys_SectionModel model)
        {
            string whereQ = "";

            if (!string.IsNullOrWhiteSpace(model.CODE))
            {
                whereQ += "\r\n\t" + "And regexp_like(T1.CODE,:CODE,'i')";
                model.QueryParameters.Add("CODE", model.CODE);
                //whereQ += "\r\n\t" + "AND T1.CODE LIKE :CODE";
                //model.QueryParameters.Add("CODE", "%" + model.CODE.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.DESCRIPTION))
            {
                whereQ += "\r\n\t" + "AND T1.DESCRIPTION LIKE :DESCRIPTION";
                model.QueryParameters.Add("DESCRIPTION", "%" + model.DESCRIPTION.Trim() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.FAX))
            {
                whereQ += "\r\n\t" + "AND T1.FAX LIKE :FAX";
                model.QueryParameters.Add("FAX", "%" + model.FAX.Trim().ToUpper() + "%");
            }

            return whereQ;
        }

        public SYS_SECTION GetRecord(string sUUID)
        {
            using (EntitiesAuth db = new EntitiesAuth())
            {
                SYS_SECTION record = db.SYS_SECTION.Where(o => o.UUID == sUUID).FirstOrDefault();
                return record;
            }
        }

        public void Search(Sys_SectionModel model)
        {
            model.Query = SearchSql;
            model.QueryWhere = SearchCriteria(model);
            model.Search();
        }

        public string Excel(Sys_SectionModel model)
        {
            model.Query = SearchSql;
            model.QueryWhere = SearchCriteria(model);
            return model.Export("Section Management");
        }

        public int Create(SYS_SECTION model)
        {
            using (EntitiesAuth db = new EntitiesAuth())
            {
                db.SYS_SECTION.Add(model);
                return db.SaveChanges();
            }
        }

        public int Update(SYS_SECTION model)
        {
            using (EntitiesAuth db = new EntitiesAuth())
            {
                SYS_SECTION record = db.SYS_SECTION.Where(o => o.UUID == model.UUID).FirstOrDefault();

                record.CODE = model.CODE;
                record.DESCRIPTION = model.DESCRIPTION;
                record.FAX = model.FAX;
                
                return db.SaveChanges();
            }
        }
    }
}
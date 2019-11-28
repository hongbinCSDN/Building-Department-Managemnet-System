using MWMS2.Areas.Admin.Models;
using MWMS2.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Services
{
    public class PEM_UnitDAOService
    {
        private const string SearchSql = @"SELECT T1.UUID,
                                                   T1.CODE AS UNITCODE,
                                                   T2.CODE AS SECTIONCODE,
                                                   T1.DESCRIPTION,
                                                   T1.FAX
                                            FROM   SYS_UNIT T1
                                                   JOIN SYS_SECTION T2
                                                     ON T1.SYS_SECTION_ID = T2.UUID
                                            WHERE  1 = 1 ";

        private string SearchCriteria(PEM_UnitModel model)
        {
            string whereQ = "";

            if (!string.IsNullOrWhiteSpace(model.UnitCode))
            {
                whereQ += "\r\n\t" + "AND T1.CODE LIKE :CODE";
                model.QueryParameters.Add("CODE", "%" + model.UnitCode + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.Description))
            {
                whereQ += "\r\n\t" + "AND T1.DESCRIPTION LIKE :DESCRIPTION";
                model.QueryParameters.Add("DESCRIPTION", "%" + model.Description + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.Fax))
            {
                whereQ += "\r\n\t" + "AND T1.FAX LIKE :FAX";
                model.QueryParameters.Add("FAX", "%" + model.Fax + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.SectionCode))
            {
                whereQ += "\r\n\t" + "AND T2.CODE LIKE :SectionCode";
                model.QueryParameters.Add("SectionCode", "%" + model.SectionCode + "%");
            }

            return whereQ;
        }

        public SYS_UNIT GetRecord(string sUUID)
        {
            using (EntitiesAuth db = new EntitiesAuth())
            {
                SYS_UNIT record = db.SYS_UNIT.Where(o => o.UUID == sUUID).FirstOrDefault();
                return record;
            }
        }



        public void Search(PEM_UnitModel model)
        {
            model.Query = SearchSql;
            model.QueryWhere = SearchCriteria(model);
            model.Search();
        }

        public int Create(SYS_UNIT model)
        {
            using (EntitiesAuth db = new EntitiesAuth())
            {
                db.SYS_UNIT.Add(model);
                return db.SaveChanges();
            }
        }

        public int Update(SYS_UNIT model)
        {
            using (EntitiesAuth db = new EntitiesAuth())
            {
                SYS_UNIT record = db.SYS_UNIT.Where(o => o.UUID == model.UUID).FirstOrDefault();

                record.SYS_SECTION_ID = model.SYS_SECTION_ID;
                record.CODE = model.CODE;
                record.DESCRIPTION = model.DESCRIPTION;
                record.FAX = model.FAX;

                return db.SaveChanges();
            }
        }

        public string Export(PEM_UnitModel model)
        {
            model.Query = SearchSql;
            model.QueryWhere = SearchCriteria(model);
            model.Columns[model.Columns.Length - 1]["displayName"] = "";
            return model.Export("Unit Management");
        }

        public T GetObjectData<T>(string sSql, object[] sqlParams)
        {
            using (EntitiesAuth db = new EntitiesAuth())
            {
                return db.Database.SqlQuery<T>(sSql, sqlParams).FirstOrDefault();
            }
        }

        public List<T> GetObjectDataList<T>(string sSql, object[] sqlParams)
        {
            using (EntitiesAuth db = new EntitiesAuth())
            {
                return db.Database.SqlQuery<T>(sSql, sqlParams).ToList();
            }
        }
    }
}
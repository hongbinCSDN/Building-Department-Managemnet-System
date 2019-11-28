using MWMS2.Entity;
using MWMS2.Utility;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;

namespace MWMS2.Services
{
    public class BaseDAOService
    {
        public IEnumerable<T> GetObjectData<T>(string sSql)
        {
            using (EntitiesAuth db = new EntitiesAuth())
            {
                return db.Database.SqlQuery<T>(sSql).ToList();
            }
        }

        public IEnumerable<T> GetObjectData<T>(string sSql, DbContext db)
        {
            return db.Database.SqlQuery<T>(sSql).ToList();
        }

        public IEnumerable<T> GetObjectData<T>(string sSql, OracleParameter[] oracleParameters)
        {
            AuditLogService.logDebug("Start @ "+ DateUtil.ConvertToDateTimeDisplay(DateUtil.getNow()));
            AuditLogService.logDebug(sSql);

            for (int i=0;i< oracleParameters.Length; i++)
            {
                AuditLogService.logDebug(oracleParameters[i].ParameterName + ":"+ oracleParameters[i].Value);
            }
            using (EntitiesAuth db = new EntitiesAuth())
            {
                IEnumerable<T> result = db.Database.SqlQuery<T>(sSql, oracleParameters).ToList();
                AuditLogService.logDebug("End @ " + DateUtil.ConvertToDateTimeDisplay(DateUtil.getNow()));
                return result;

            }
        }

        public IEnumerable<T> GetObjectData<T>(string sSql, OracleParameter[] oracleParameters, DbContext db)
        {
            return db.Database.SqlQuery<T>(sSql, oracleParameters).ToList();
        }

        public T GetObjectRecordByUuid<T>(string sUUID)
        {
            string sSql = string.Format(@"SELECT   T1.*
                                            FROM   {0} T1
                                            WHERE  T1.UUID = :UUID ", typeof(T).Name.ToUpper());

            OracleParameter[] oracleParameters = new OracleParameter[]
            {
                new OracleParameter(":UUID",sUUID)
            };
            using (EntitiesAuth db = new EntitiesAuth())
            {
                return db.Database.SqlQuery<T>(sSql, oracleParameters).FirstOrDefault();
            }
        }

        public T GetObjectRecordByUuid<T>(string sUUID, DbContext db)
        {
            string sSql = string.Format(@"SELECT   T1.*
                                            FROM   {0} T1
                                            WHERE  T1.UUID = :UUID ", typeof(T).Name.ToUpper());

            OracleParameter[] oracleParameters = new OracleParameter[]
            {
                new OracleParameter(":UUID",sUUID)
            };

            return db.Database.SqlQuery<T>(sSql, oracleParameters).FirstOrDefault();

        }


    }
}
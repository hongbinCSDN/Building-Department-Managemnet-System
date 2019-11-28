using MWMS2.Entity;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Web;

namespace MWMS2.App_Start
{
    public static class GlobalConfig
    {
        private static Dictionary<string, decimal?> _systemColumnMetaList = new Dictionary<string, decimal?>();
        public static Dictionary<string, decimal?> SYSTEM_COLUMN_META_LIST
        {
            get {
                lock (_systemColumnMetaList)
                {
                    if (_systemColumnMetaList.Count <= 0)
                    {
                        using (EntitiesRegistration db = new EntitiesRegistration())
                        {
                            using (DbConnection conn = db.Database.Connection)
                            {
                                string q = ""
                                    + "\r\n\t" + "SELECT                                           "
                                    + "\r\n\t" + "TABLE_NAME, COLUMN_NAME, CASE WHEN DATA_TYPE ='NVARCHAR2'  THEN DATA_LENGTH/2 ELSE DATA_LENGTH END AS DATA_LENGTH              "
                                    + "\r\n\t" + "FROM USER_TAB_COLS                              "
                                  //  + "\r\n\t" + "WHERE OWNER = 'MWMS2'                             "
                                    + "\r\n\t" + "WHERE DATA_TYPE IN('CHAR', 'NVARCHAR2', 'VARCHAR2') ";
                                DbDataReader dr = CommonUtil.GetDataReader(conn, q, null);
                                _systemColumnMetaList = CommonUtil.LoadDbData(dr).ToDictionary(o => (o["TABLE_NAME"] as string) + "." + (o["COLUMN_NAME"] as string), o => o["DATA_LENGTH"] as decimal?);

                                conn.Close();
                            }
                        }
                    }
                }
                return _systemColumnMetaList;
            }
        }
    }
}
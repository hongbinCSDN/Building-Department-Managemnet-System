using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Entity;
using Oracle.ManagedDataAccess.Client;

namespace MWMS2.Services.ProcessingDAO.DAO
{
    public class ProcessingMwItemDAOService : BaseDAOService
    {
        public List<string> getItemNosByClass(string classType)
        {
            string sSql = @"SELECT DISTINCT ITEM_NO
                                FROM   (SELECT Trim(ITEM_NO) AS ITEM_NO
                                        FROM   P_S_MW_ITEM mitem,
                                               P_S_MW_CLASS mclass,
                                               P_S_MW_TYPE mtype
                                        WHERE  mtype.MW_CLASS_ID = mclass.UUID
                                               AND mitem.MW_TYPE_ID = mtype.UUID
                                               AND mclass.code = :class)
                                ORDER  BY To_number(Substr(item_no, 1, 1)),
                                          To_number(Substr(item_no, 3)) ";

            OracleParameter[] sqlParams = new OracleParameter[] {
                new OracleParameter("class",classType)
            };

            return GetObjectData<string>(sSql, sqlParams).ToList();
        }

        public List<ProcessingMwItemModel> GetMWItemNoAndType(string classType)
        {
            string sSql = @"SELECT mtype.UUID    AS MWItemTypeUUID,
                                   mtype.CODE    AS MWItemType,
                                   mitem.UUID    AS MWItemUUID,
                                   Trim(item_no) AS MWItemNo
                            FROM   P_S_MW_TYPE mtype,
                                    P_S_MW_ITEM mitem,
                                    P_S_MW_CLASS mclass
                            WHERE  mtype.UUID = mitem.MW_TYPE_ID
                                    AND mclass.UUID = mtype.MW_CLASS_ID
                                    AND mclass.CODE = :class
                            ORDER  BY   To_number(Substr(MWItemNo, 1, 1)),
                                        To_number(Substr(MWItemNo, 3)) ";
            OracleParameter[] sqlParams = new OracleParameter[]
            {
                new OracleParameter("class",classType)
            };

            return GetObjectData<ProcessingMwItemModel>(sSql, sqlParams).ToList();
        }
    }
}
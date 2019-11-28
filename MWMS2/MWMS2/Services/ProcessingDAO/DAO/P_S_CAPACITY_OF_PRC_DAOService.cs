using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Services
{
    public class P_S_CAPACITY_OF_PRC_DAOService:BaseDAOService
    {

        public List<string> getItemListByRole(String contractorRole)
        {
            string sSql = @"SELECT CAP.ITEM_NO
                            FROM P_S_CAPACITY_OF_PRC CAP
                            WHERE CAP.CONTRACTOR_ROLE = :contractorRole
                            ORDER BY CAP.ITEM_NO ASC ";
            OracleParameter[] oracleParameters = new OracleParameter[]
            {
                new OracleParameter(":contractorRole",contractorRole)
            };
            return GetObjectData<string>(sSql, oracleParameters).ToList();
        }
    }
}
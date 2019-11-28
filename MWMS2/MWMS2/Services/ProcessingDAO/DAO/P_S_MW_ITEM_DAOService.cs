using MWMS2.Constant;
using MWMS2.Entity;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Services
{
    public class P_S_MW_ITEM_DAOService:BaseDAOService
    {
        public List<string> getItemNosByClassAndType(string classCode , string type)
        {
            String queryString = "SELECT sMwItem.ITEM_NO FROM S_MW_ITEM sMwItem  ";

            if (!string.IsNullOrEmpty(type))
            {
                queryString += " , P_S_MW_TYPE sMwType ";
            }
            if (!string.IsNullOrEmpty(classCode))
            {
                queryString += " , P_S_MW_CLASS sMwClass ";
            }
            if (!string.IsNullOrEmpty(type) || !string.IsNullOrEmpty(classCode))
            {
                queryString += " where ";
            }
            int paramCount = 0;
            if (!string.IsNullOrEmpty(type))
            {
                queryString += " sMwItem.MW_TYPE_ID = sMwType.uuid and sMwType.code = :type";
                paramCount++;
            }
            if (!string.IsNullOrEmpty(classCode))
            {
                if (paramCount > 0)
                {
                    queryString += " and ";
                }
                queryString += " sMwType.MW_CLASS_ID  = sMwClass.uuid and sMwClass.code in ";
                if (classCode.Equals(ProcessingConstant.CLASS_1))
                {
                    queryString += "('Class 1','Class 2','Class 3') ";
                }
                if (classCode.Equals(ProcessingConstant.CLASS_2))
                {
                    queryString += "('Class 2','Class 3') ";
                }
                if (classCode.Equals(ProcessingConstant.CLASS_3))
                {
                    queryString += "('Class 3') ";
                }

            }
            queryString += " order by sMwItem.ITEM_NO ";

            OracleParameter[] oracleParameters = new OracleParameter[]
            {
                new OracleParameter(":type",type)
            };

            return GetObjectData<string>(queryString, oracleParameters).ToList();
        }

        public P_S_MW_ITEM GetP_S_MW_ITEMByItemNo(string itemNo)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return db.P_S_MW_ITEM.Where(w => w.ITEM_NO.Trim() == itemNo).FirstOrDefault();
            }
        }
    }
}
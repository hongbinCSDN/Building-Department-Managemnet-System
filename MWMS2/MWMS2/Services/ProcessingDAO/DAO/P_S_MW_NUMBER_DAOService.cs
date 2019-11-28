using MWMS2.Constant;
using MWMS2.Entity;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;

namespace MWMS2.Services
{
    public class P_S_MW_NUMBER_DAOService : BaseDAOService
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        public string GetNewNumberOfDsn()
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                //Get Max DSN Number 
                string maxNumberSql = @"SELECT MAX(Replace(MW_NUMBER,'D','')) AS MAX_NO
                                        FROM P_S_MW_NUMBER
                                        WHERE  1 = 1
                                               AND MW_NUMBER LIKE :MW_NUMBER And Length(MW_NUMBER)=11";
                OracleParameter[] oracleParameters = new OracleParameter[]
                {
                    new OracleParameter(":MW_NUMBER",ProcessingConstant.PREFIX_DSN + "%")
                };

                string sCurrentNumber = GetObjectData<string>(maxNumberSql, oracleParameters).FirstOrDefault();

                if (string.IsNullOrEmpty(sCurrentNumber)) { sCurrentNumber = "0"; }

                int iMaxNumber = GetNumber(sCurrentNumber, ProcessingConstant.PREFIX_DSN);

                iMaxNumber += 1;

                string sNextNumber = GetMaxNumber(iMaxNumber, ProcessingConstant.PREFIX_DSN);

                //Insert New P_S_MW_NUMBER
                P_S_MW_NUMBER NumberRecord = new P_S_MW_NUMBER();
                NumberRecord.MW_NUMBER = sNextNumber;

                AddP_S_MW_NUMBER(NumberRecord, db);

                return sNextNumber;

            }
        }

        public int AddP_S_MW_NUMBER(P_S_MW_NUMBER model)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                db.P_S_MW_NUMBER.Add(model);
                return db.SaveChanges();
            }
        }

        public int AddP_S_MW_NUMBER(P_S_MW_NUMBER model, EntitiesMWProcessing db)
        {
            db.P_S_MW_NUMBER.Add(model);
            return db.SaveChanges();
        }

        private int GetNumber(string sMaxNumber, string sPrefix)
        {
            return int.Parse(sMaxNumber.Replace(sPrefix, ""));
        }

        private string GetMaxNumber(int iMaxNumber, string sPrefix)
        {
            string sMaxNumber = "0000000000";

            sMaxNumber = sMaxNumber + iMaxNumber.ToString();

            sMaxNumber = sPrefix + sMaxNumber.Substring(sMaxNumber.Length - 10);

            return sMaxNumber;

        }


    }
}
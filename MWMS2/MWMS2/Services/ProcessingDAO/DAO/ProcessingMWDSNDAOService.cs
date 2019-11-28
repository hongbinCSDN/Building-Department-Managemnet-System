using MWMS2.Constant;
using MWMS2.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace MWMS2.Services
{
    public class ProcessingMWDSNDAOService
    {


        public void createOrUpdateMWDSNList(List<P_MW_DSN> dsnList)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                using (DbContextTransaction transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        for (int i =0; i< dsnList.Count(); i++) { 
                            if (String.IsNullOrEmpty(dsnList[i].UUID))
                            {
                                db.P_MW_DSN.Add(dsnList[i]);
                                db.SaveChanges();
                            }
                            else
                            {
                                db.Entry(dsnList[i]).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        } 

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine("Error :" + ex.Message);
                        throw ex;
                    }


                }
            }
        }

        public void createOrUpdateMWDSN(P_MW_DSN dsn)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                using (DbContextTransaction transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        if (String.IsNullOrEmpty(dsn.UUID))
                        {
                            db.P_MW_DSN.Add(dsn);
                            db.SaveChanges();
                        }
                        else
                        {
                            db.Entry(dsn).State = EntityState.Modified;
                            db.SaveChanges();
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine("Error :" + ex.Message);
                        throw ex;
                    }


                }
            }
        }
        public List<P_MW_DSN> GetDSNList(String statusCode)
        {
            List<P_MW_DSN> resultList = null;
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                resultList = (from dsn in db.P_MW_DSN
                              join sSystemValue in db.P_S_SYSTEM_VALUE on dsn.SCANNED_STATUS_ID equals sSystemValue.UUID
                              where 1 == 1 && sSystemValue.CODE == statusCode
                              orderby dsn.DSN descending
                              select dsn).ToList();
            }
            return resultList;
        }

        public P_MW_DSN GetP_MW_DSN(string DSN)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return db.P_MW_DSN.Include(d => d.P_S_SYSTEM_VALUE).Where(d => d.DSN == DSN).FirstOrDefault();
            }
        }

        public List<P_MW_DSN> GetDSNList(List<String> statusCodeList)
        {
            List<P_MW_DSN> resultList = null;
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                resultList = (from dsn in db.P_MW_DSN
                              join sSystemValue in db.P_S_SYSTEM_VALUE on dsn.SCANNED_STATUS_ID equals sSystemValue.UUID
                              where 1 == 1 && statusCodeList.Contains(sSystemValue.CODE)
                              orderby dsn.DSN descending
                              select dsn).ToList();
            }
            return resultList;
        }

    }
}
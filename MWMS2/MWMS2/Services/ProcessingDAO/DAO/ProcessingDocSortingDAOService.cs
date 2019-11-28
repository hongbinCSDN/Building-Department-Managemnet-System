using MWMS2.Entity;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace MWMS2.Services
{
    public class ProcessingDocSortingDAOService : BaseDAOService
    {
        public P_MW_DSN GetP_MW_DSN(string DSN)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return db.P_MW_DSN.Where(d => d.DSN == DSN).FirstOrDefault();
            }
        }

        public List<P_MW_SCANNED_DOCUMENT> GetP_MW_SCANNED_DOCUMENT(string DSN_ID)
        {

            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return db.P_MW_SCANNED_DOCUMENT.Include(a => a.P_MW_DSN).Where(d => d.DSN_ID == DSN_ID).ToList();
            }

        }

        public P_MW_GENERAL_RECORD GetMWGeneralRecord(string P_MW_REFERENCE_NO_UUID)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return db.P_MW_GENERAL_RECORD.Where(d => d.REFERENCE_NUMBER == P_MW_REFERENCE_NO_UUID).FirstOrDefault();
            }
        }

        public P_S_SYSTEM_VALUE GetPSystemValue(string code)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return db.P_S_SYSTEM_VALUE.Where(d => d.CODE == code).FirstOrDefault();
            }

        }

        public void saveDocSorting(P_MW_REFERENCE_NO newReferNo, P_MW_DSN dsn, P_MW_GENERAL_RECORD oldGeneralRecord,
            P_MW_GENERAL_RECORD newGeneralRecord)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                using (DbContextTransaction transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        db.Entry(dsn).State = EntityState.Modified;
                        db.SaveChanges();

                        if (String.IsNullOrEmpty(newReferNo.UUID))
                        {
                            db.P_MW_REFERENCE_NO.Add(newReferNo);
                            db.SaveChanges();
                        }


                        if (oldGeneralRecord != null)
                        {
                            db.Entry(oldGeneralRecord).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        newGeneralRecord.REFERENCE_NUMBER = newReferNo.UUID;
                        db.P_MW_GENERAL_RECORD.Add(newGeneralRecord);
                        db.SaveChanges();
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
    }
}
using MWMS2.Entity;
using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MWMS2.Services.Signborad.SignboardServices
{
    public class SignboardSequenceNumberGenerator
    {
        public long getSequenceNumber(string seqType, string prefix)
        {
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                using (DbContextTransaction transaction = db.Database.BeginTransaction())
                {
                    long sequenceNubmer = new long();
                    //try
                    //{

                    //List<B_SV_REFERENCE_NO> result = (from srefer in db.B_SV_REFERENCE_NO
                    //                            where srefer.PREFIX == prefix
                    //                            && srefer.TYPE == seqType
                    //                            select srefer).ToList();
                    B_SV_REFERENCE_NO result = (from srefer in db.B_SV_REFERENCE_NO
                                                where srefer.PREFIX == prefix
                                                && srefer.TYPE == seqType
                                                select srefer).FirstOrDefault();
                if (result != null)
                {
                        sequenceNubmer = Convert.ToInt32(result.CURRENT_NUMBER) + 1;
                        result.CURRENT_NUMBER = sequenceNubmer;
                        //B_SV_REFERENCE_NO srefno = new B_SV_REFERENCE_NO();
                        //sequenceNubmer = Convert.ToInt64(result[0].CURRENT_NUMBER);
                        //sequenceNubmer = sequenceNubmer + 1;
                        //srefno.CURRENT_NUMBER = sequenceNubmer;
                        //srefno.PREFIX = result[0].PREFIX;
                        //srefno.TYPE = result[0].TYPE;
                        //db.SaveChanges();
                }
                else
                {
                        B_SV_REFERENCE_NO srefno = new B_SV_REFERENCE_NO();
                            //if (string.IsNullOrEmpty(srefno.UUID))
                            //{
                            //    db.B_SV_REFERENCE_NO.Add(srefno);
                            //    db.SaveChanges();
                            //}
                        srefno.UUID = Guid.NewGuid().ToString("N");
                        sequenceNubmer = sequenceNubmer + 1;
                        srefno.CURRENT_NUMBER = sequenceNubmer;
                        srefno.TYPE = seqType;
                        srefno.PREFIX = prefix;
                        db.B_SV_REFERENCE_NO.Add(srefno);
                        //db.SaveChanges();
                }
                    db.SaveChanges();
                    transaction.Commit();
                        //}
                        //catch
                        //{

                        //}
                        return sequenceNubmer;
                
                }
            }
        }
    }
}
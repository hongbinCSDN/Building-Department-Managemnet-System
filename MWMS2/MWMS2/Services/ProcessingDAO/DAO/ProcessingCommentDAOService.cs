using MWMS2.Entity;
using MWMS2.Utility;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Services
{
    public class ProcessingCommentDAOService:BaseDAOService
    {
        public List<P_MW_COMMENT> GetP_MW_COMMENTs(string RECORD_ID)
        {
            string sSql = @"SELECT C.*,
                                   ( CASE U.CODE
                                       WHEN 'SU' THEN 'SMM'
                                       WHEN 'MW' THEN 'PEM'
                                       ELSE NULL
                                     END ) AS HANDLINGUNIT
                            FROM   P_MW_COMMENT C
                                   LEFT JOIN SYS_POST P
                                          ON C.FROM_USER = P.CODE
                                   JOIN SYS_UNIT U
                                     ON P.SYS_UNIT_ID = U.UUID
                            WHERE  C.RECORD_ID = :RECORD_ID ";
            OracleParameter[] oracleParameters = new OracleParameter[]
            {
                new OracleParameter(":RECORD_ID",RECORD_ID)
            };
            return GetObjectData<P_MW_COMMENT>(sSql, oracleParameters).ToList();
        }

        public int AddP_MW_COMMENT(P_MW_COMMENT model)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                model.FROM_USER = SessionUtil.LoginPost.CODE;
                db.P_MW_COMMENT.Add(model);
                return db.SaveChanges();
            }
        }

        public int UpdateP_MW_COMMENT(P_MW_COMMENT model)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                P_MW_COMMENT record = db.P_MW_COMMENT.Where(w => w.UUID == model.UUID).FirstOrDefault();

                record.COMMENT_AREA = model.COMMENT_AREA;
                record.MODIFIED_BY = SessionUtil.LoginPost.CODE;

                return db.SaveChanges();
            }
        }

        public int AddP_MW_COMMENT(P_MW_COMMENT model, EntitiesMWProcessing db)
        {
            model.FROM_USER = SessionUtil.LoginPost.CODE;
            db.P_MW_COMMENT.Add(model);
            return db.SaveChanges();
        }
    }
}
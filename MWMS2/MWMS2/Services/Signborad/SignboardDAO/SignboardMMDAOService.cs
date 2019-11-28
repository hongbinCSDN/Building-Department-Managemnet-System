using MWMS2.Areas.Signboard.Models;
using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Services.Signborad.SignboardDAO
{
    public class SignboardMMDAOService
    {

        string Serach_SCUR_all_q = "SELECT ROWNUM,v.uuid as UUID, s.BATCH_NO AS BATCHNO, r.reference_No AS REFERENCENO, r.FORM_CODE AS FORMCODE, " +
            "r.RECEIVED_DATE AS RECEIVEDDATE, CASE v.VALIDATION_RESULT WHEN 'A' THEN 'Accept' ELSE 'Refuse' END AS VALIDATIONRESULT, v.SPO_ENDORSEMENT_DATE AS SPOENDORSEMENTDATE, " +
            "v.ENDORSED_BY AS ENDORSEDBY " +
            "from B_SV_Validation v " +
            "INNER JOIN B_SV_RECORD r ON v.SV_RECORD_ID = r.UUID " +
            "INNER JOIN B_SV_SUBMISSION s ON r.SV_SUBMISSION_ID = s.UUID " +
            "WHERE v.SPO_ENDORSEMENT_DATE IS NOT NULL AND v.LETTER_STATUS IS NULL ";


        public Fn01SCUR_MMSearchModel mailMergeSearch(Fn01SCUR_MMSearchModel model)
        {
            model.Query = Serach_SCUR_all_q;
            model.Search();
            return model;

        }
        public int Update_B_SV_VALIDATION_Status(string sUUID, string sStatusID,String sMergeBy)
        {
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                B_SV_VALIDATION record = db.B_SV_VALIDATION.Where(d => d.UUID == sUUID).FirstOrDefault();
                if (record != null)
                {
                    record.LETTER_MERGE_BY = sMergeBy;
                    record.LETTER_STATUS = sStatusID;
                    return db.SaveChanges();
                }

                return 0;
            
            }

        }
    }
}
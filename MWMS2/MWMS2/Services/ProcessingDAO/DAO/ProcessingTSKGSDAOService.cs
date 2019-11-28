using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Entity;

namespace MWMS2.Services.ProcessingDAO.DAO
{
    public class ProcessingTSKGSDAOService
    {
        private string search_q = @"SELECT RECORD.*,
                                           REFERENCENUMBER.REFERENCE_NO,
                                           ( CASE
                                               WHEN RECORD.RECEIVE_DATE IS NOT NULL THEN RECORD.RECEIVE_DATE + 30
                                               ELSE NULL
                                             END ) AS FinalReplyDueDate,
                                           ( CASE
                                               WHEN RECEIVE_DATE IS NOT NULL THEN Floor(SYSDATE - ( RECORD.RECEIVE_DATE + 30 ))
                                               ELSE NULL
                                             END ) AS FinalReplyRemainingDays,
                                           ( CASE
                                               WHEN RECEIVE_DATE IS NOT NULL THEN Floor(( SYSDATE + 10 ) - RECORD.RECEIVE_DATE)
                                               ELSE NULL
                                             END ) AS InterimReplyRemainingDays,
                                           ( CASE
                                               WHEN RECORD.SUBMIT_TYPE = 'ICC' THEN RECORD.ICC_NUMBER
                                               ELSE D.DSN
                                             END ) AS DSN_ICCNO
                                    FROM   P_MW_GENERAL_RECORD RECORD
                                           JOIN P_MW_REFERENCE_NO REFERENCENUMBER
                                             ON REFERENCENUMBER.UUID = RECORD.REFERENCE_NUMBER
                                           JOIN P_WF_INFO WI
                                             ON RECORD.UUID = WI.RECORD_ID
                                           JOIN P_WF_TASK WT
                                             ON WI.UUID = WT.P_WF_INFO_ID
                                           LEFT JOIN P_MW_DSN D
                                                  ON REFERENCENUMBER.REFERENCE_NO = D.REcord_ID
                                                     AND D.SUBMIT_TYPE IN ( 'Enquiry', 'Complaint' )
                                    WHERE  1 = 1 ";

        public Fn03TSK_GSModel Search(Fn03TSK_GSModel model)
        {
            model.Query = search_q;
            model.Search();
            return model;
        }


    }
}
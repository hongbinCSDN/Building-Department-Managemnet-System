using MWMS2.Constant;
using MWMS2.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Services
{
    public class P_MW_APPOINTED_PROFESSIONAL_DAOService
    {


        public P_MW_APPOINTED_PROFESSIONAL FindPBPByFinalMWRecord(P_MW_RECORD p_MW_RECORD, string IDENTIFY_FLAG, string IDENTIFY_FLAG2)
        {
            if (p_MW_RECORD.IS_DATA_ENTRY != ProcessingConstant.FLAG_N) { return null; }

            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return db.P_MW_APPOINTED_PROFESSIONAL.Where(w => w.MW_RECORD_ID == p_MW_RECORD.UUID && (w.IDENTIFY_FLAG == IDENTIFY_FLAG || w.IDENTIFY_FLAG == IDENTIFY_FLAG2)).OrderBy(o => o.ORDERING).FirstOrDefault();
            }
        }

        public P_MW_APPOINTED_PROFESSIONAL FindPBPByFinalMWRecord(P_MW_RECORD p_MW_RECORD, string IDENTIFY_FLAG)
        {
            if (p_MW_RECORD.IS_DATA_ENTRY != ProcessingConstant.FLAG_N) { return null; }

            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return db.P_MW_APPOINTED_PROFESSIONAL.Where(w => w.MW_RECORD_ID == p_MW_RECORD.UUID && w.IDENTIFY_FLAG == IDENTIFY_FLAG).OrderBy(o => o.ORDERING).FirstOrDefault();
            }
        }

        public P_MW_RECORD GetLatestSecondEntryMwRecordByRefNoAndFormCode(string REFERENCE_NUMBER, string S_FORM_TYPE_CODE)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return db.P_MW_RECORD.Where(w => w.REFERENCE_NUMBER == REFERENCE_NUMBER && w.S_FORM_TYPE_CODE == S_FORM_TYPE_CODE && w.STATUS_CODE == ProcessingConstant.MW_SECOND_COMPLETE).OrderByDescending(o => o.CREATED_DATE).FirstOrDefault();
            }
        }

        public P_MW_APPOINTED_PROFESSIONAL findFormPBPByMWRecordOrdering(P_MW_RECORD p_MW_RECORD, int ordering)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return db.P_MW_APPOINTED_PROFESSIONAL.Where(w => w.MW_RECORD_ID == p_MW_RECORD.UUID && w.ORDERING == ordering).FirstOrDefault();
            }
        }

        public P_MW_APPOINTED_PROFESSIONAL findByMWRecordCertNo(P_MW_RECORD mwRecord, String certificationNo)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return db.P_MW_APPOINTED_PROFESSIONAL.Where(w => w.MW_RECORD_ID == mwRecord.UUID && w.CERTIFICATION_NO == certificationNo).OrderBy(o => o.ORDERING).FirstOrDefault();
            }
        }

        public void FillAppointedProfessionalByAckLetter(List<P_MW_APPOINTED_PROFESSIONAL> aps, P_MW_ACK_LETTER ackLetter)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                V_CRM_INFO apMwCrmInfo = null;

                if (!string.IsNullOrWhiteSpace(ackLetter.PBP_NO))
                {
                    apMwCrmInfo = db.V_CRM_INFO.Where(w => w.CERTIFICATION_NO == ackLetter.PBP_NO).FirstOrDefault();
                }

                V_CRM_INFO prcMwCrmInfo = null;

                if (!string.IsNullOrWhiteSpace(ackLetter.PRC_NO))
                {
                    prcMwCrmInfo = db.V_CRM_INFO.Where(w => w.CERTIFICATION_NO == ackLetter.PRC_NO).FirstOrDefault();
                }

                if (aps != null)
                {
                    for (int i = 0; i < aps.Count(); i++)
                    {
                        aps[i].COMMENCED_DATE = ackLetter.COMM_DATE;
                        aps[i].COMPLETION_DATE = ackLetter.COMP_DATE;

                        if (ProcessingConstant.AP.Equals(aps[i].IDENTIFY_FLAG))
                        {
                            //AP
                            aps[i].CERTIFICATION_NO = ackLetter.PBP_NO;

                            if (apMwCrmInfo != null)
                            {
                                aps[i].EXPIRY_DATE = apMwCrmInfo.EXPIRY_DATE;
                                aps[i].CHINESE_NAME = apMwCrmInfo.CHINESE_NAME;
                                aps[i].ENGLISH_NAME = apMwCrmInfo.SURNAME + " " + apMwCrmInfo.GIVEN_NAME;
                                aps[i].FAX_NO = apMwCrmInfo.FAX_NO;
                            }

                        }
                        else if (ProcessingConstant.PRC.Equals(aps[i].IDENTIFY_FLAG))
                        {
                            //PRC
                            aps[i].CERTIFICATION_NO = ackLetter.PRC_NO;

                            if (apMwCrmInfo != null)
                            {
                                aps[i].EXPIRY_DATE = prcMwCrmInfo.EXPIRY_DATE;
                                aps[i].CHINESE_NAME = prcMwCrmInfo.CHINESE_NAME;
                                aps[i].ENGLISH_NAME = prcMwCrmInfo.SURNAME + " " + prcMwCrmInfo.GIVEN_NAME;
                                aps[i].FAX_NO = prcMwCrmInfo.FAX_NO;
                            }
                        }

                    }
                }
            }

        }

    }
}
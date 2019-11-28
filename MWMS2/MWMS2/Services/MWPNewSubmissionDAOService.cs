using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MWMS2.Constant;
using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Entity;
using MWMS2.Services;
using System.Data.Common;
using MWMS2.Utility;
using System.Data.Entity;

namespace MWMS2.Services
{
    public class MWPNewSubmissionDAOService
    {
        String SearchMod_q = "select '0' as UUID, '1' as REC_DATE, '2' as TIME, '3' as DSN, '4' as REF_NO, '5' as FORM_NO"
                 + "\r\n\t" + "from dual ";

        public Fn02MWUR_MWURC_Model ReceiveNewSubmission(Fn02MWUR_MWURC_Model model)
        {
            //model.Query = SearchMod_q;
            //model.QueryWhere = SearchMOD_whereQ(model);
            //model.Search(); 

            //List<P_MW_DSN> MwDsnList = new List<P_MW_DSN>();
            P_MW_DSN P_MW_DSN = new P_MW_DSN();
            P_MW_DSN.DSN = "D000";
            P_MW_DSN.FORM_CODE = "MW01";

            model.MwDsnList.Add(P_MW_DSN);
            model.MwDsnList.Add(P_MW_DSN);

            return model;
        }

        public P_MW_DSN GetNewDsnByAutoGen()
        {
            P_MW_DSN MwDsn = new P_MW_DSN();
            MwDsn.DSN = "D000";
            MwDsn.FORM_CODE = "MW01";

            return MwDsn;
        }

        public string getIccMaxNumber(string prefix, string prefixCode)
        {
            string result = "";

            // parameter list
            Dictionary<string, object> QueryParameters = new Dictionary<string, object>();

            string q = @"SELECT Max(MW_NUMBER) AS MAX_NO
                         FROM P_S_MW_NUMBER
                         WHERE  1 = 1
                         AND MW_NUMBER LIKE :prefixNumber 
                         And Length(MW_NUMBER)= :length ";

            

            if (ProcessingConstant.PREFIX_ENQ.Equals(prefix) || ProcessingConstant.PREFIX_COMP.Equals(prefix))
            {
                QueryParameters.Add("prefixNumber", prefixCode + "%");
                QueryParameters.Add("length", "9");
            }
            else if (ProcessingConstant.PREFIX_MW.Equals(prefix))
            {
                QueryParameters.Add("prefixNumber", prefix + prefixCode + "%");
                QueryParameters.Add("length", "11");
            }
            else if (ProcessingConstant.PREFIX_D.Equals(prefix))
            {
                QueryParameters.Add("prefixNumber", prefix + "%");
                QueryParameters.Add("length", "11");
            }

            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                using (DbConnection conn = db.Database.Connection)
                {
                    DbDataReader dr = CommonUtil.GetDataReader(conn, q, QueryParameters);
                    List<Dictionary<string, object>> Data = CommonUtil.LoadDbData(dr);
                    if (Data.Count > 0)
                    {
                        result = Data[0]["MAX_NO"].ToString();
                    }
                    conn.Close();
                }
            }
            return result;
        }

        public String getMaxMwNumber(String prefix, String prefixDate)
        {
            String result = "";

            // parameter list
            Dictionary<string, object> QueryParameters = new Dictionary<string, object>();

            string q = "select max(mw_number) as MAX_NO from p_s_mw_number where 1=1 ";

            if (ProcessingConstant.PREFIX_D.Equals(prefix) || ProcessingConstant.PREFIX_OI.Equals(prefix))
            {
                q += "\r\n\t" + "and MW_NUMBER like :prefix And Length(MW_NUMBER)=11 ";
                QueryParameters.Add("prefix", prefix + "%");
            }
            else if (ProcessingConstant.PREFIX_MW.Equals(prefix))
            {
                q += "\r\n\t" + "and (MW_NUMBER not like :prefix1) and (MW_NUMBER not like :prefix2)";
                QueryParameters.Add("prefix1", ProcessingConstant.PREFIX_D + "%");
                QueryParameters.Add("prefix2", ProcessingConstant.PREFIX_OI + "%");
            }
            else
            {
                q += "\r\n\t" + "and (MW_NUMBER not like :prefix1) and (MW_NUMBER not like :prefix2)";
                QueryParameters.Add("prefix1", ProcessingConstant.PREFIX_D + "%");
                QueryParameters.Add("prefix2", ProcessingConstant.PREFIX_OI + "%");
            }

            if (!String.IsNullOrEmpty(prefixDate))
            {
                q += "\r\n\t" + "and MW_NUMBER like :prefixDate";
                QueryParameters.Add("prefixDate", prefixDate + "%");
            }

            //q = q.Replace(":sDate", sDate);
            //q = q.Replace(":mask", mask);

            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                using (DbConnection conn = db.Database.Connection)
                {
                    DbDataReader dr = CommonUtil.GetDataReader(conn, q, QueryParameters);
                    List<Dictionary<string, object>> Data = CommonUtil.LoadDbData(dr);
                    if (Data.Count > 0)
                    {
                        result = Data[0]["MAX_NO"].ToString();
                    }
                    conn.Close();
                }
            }
            return result;
        }

        public String getMaxModNumber(String year)
        {
            String result = "";

            // parameter list
            Dictionary<string, object> QueryParameters = new Dictionary<string, object>();

            string q = "select max(current_number) as MAX_NO from p_mw_modification_no where 1=1 ";

            if (!String.IsNullOrEmpty(year))
            {
                //q += "\r\n\t" + "and prefix like :year";
                // Begin Modify by chester 2019-08-09
                q += "\r\n\t" + "and prefix like :prefix";
                // End Modify by chester 2019-08-09
                QueryParameters.Add("prefix", year);
            }

            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                using (DbConnection conn = db.Database.Connection)
                {
                    DbDataReader dr = CommonUtil.GetDataReader(conn, q, QueryParameters);
                    List<Dictionary<string, object>> Data = CommonUtil.LoadDbData(dr);
                    if (Data.Count > 0)
                    {
                        result = Data[0]["MAX_NO"].ToString();
                    }
                    conn.Close();
                }
            }
            return result;
        }

        public String SaveMwReferenceNo(P_MW_REFERENCE_NO targetObj, EntitiesMWProcessing db)
        {
            String msg = "";

            db.P_MW_REFERENCE_NO.Add(targetObj);
            if (db.SaveChanges() > 0)
            {
                msg = "Success";
            }
            else
            {
                msg = "Fail";
            }

            return msg;
        }

        public String SaveMwDSN(P_MW_DSN targetObj, EntitiesMWProcessing db)
        {
            String msg = "";
            db.P_MW_DSN.Add(targetObj);
            if (db.SaveChanges() > 0)
            {
                msg = "Success";
            }
            else
            {
                msg = "Fail";
            }
            return msg;
        }

        public String SaveSMwNo(P_S_MW_NUMBER targetObj)
        {
            String msg = "";
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                using (DbContextTransaction transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        db.P_S_MW_NUMBER.Add(targetObj);
                        if (db.SaveChanges() > 0)
                        {
                            msg = "Success";
                        }
                        else
                        {
                            msg = "Fail";
                        }
                        db.SaveChanges();
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine("Error :" + ex.Message);
                    }

                }
            }
            return msg;
        }

        public String SaveSMwNo(P_S_MW_NUMBER targetObj, EntitiesMWProcessing db)
        {
            String msg = "";
            db.P_S_MW_NUMBER.Add(targetObj);
            if (db.SaveChanges() > 0)
            {
                msg = "Success";
            }
            else
            {
                msg = "Fail";
            }
            return msg;
        }

        public String SaveMwModNo(P_MW_MODIFICATION_NO targetObj, EntitiesMWProcessing db)
        {
            String msg = "";
            db.P_MW_MODIFICATION_NO.Add(targetObj);
            if (db.SaveChanges() > 0)
            {
                msg = "Success";
            }
            else
            {
                msg = "Fail";
            }
            return msg;
        }

        public static List<P_MW_DSN> FindParentDsnByRefNumberAndFormNo(String refNo, String formNo)
        {
            List<String> parentFormNoList = new List<string>();

            if (ProcessingConstant.FORM_MW02.Equals(formNo)
                || ProcessingConstant.FORM_MW11.Equals(formNo))
            {
                parentFormNoList.Add(ProcessingConstant.FORM_MW01);
            }

            if (ProcessingConstant.FORM_MW04.Equals(formNo)
                || ProcessingConstant.FORM_MW12.Equals(formNo))
            {
                parentFormNoList.Add(ProcessingConstant.FORM_MW03);
            }

            if (ProcessingConstant.FORM_MW07.Equals(formNo)
                || ProcessingConstant.FORM_MW08.Equals(formNo)
                || ProcessingConstant.FORM_MW09.Equals(formNo)
                || ProcessingConstant.FORM_MW10.Equals(formNo)
                || ProcessingConstant.FORM_MW31.Equals(formNo)
                || ProcessingConstant.FORM_MW33.Equals(formNo))
            {
                parentFormNoList.Add(ProcessingConstant.FORM_MW01);
                parentFormNoList.Add(ProcessingConstant.FORM_MW03);
                parentFormNoList.Add(ProcessingConstant.FORM_MW05);
                parentFormNoList.Add(ProcessingConstant.FORM_MW06);
            }

            if (ProcessingConstant.FORM_MW06_01.Equals(formNo))
            {
                parentFormNoList.Add(ProcessingConstant.FORM_MW01);
            }

            if (ProcessingConstant.FORM_MW06_02.Equals(formNo))
            {
                parentFormNoList.Add(ProcessingConstant.FORM_MW03);
            }

            if (ProcessingConstant.FORM_BA16.Equals(formNo))
            {
                parentFormNoList.Add(ProcessingConstant.FORM_BA16);
            }

            // distinct form no list
            parentFormNoList.Distinct();

            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                List<P_MW_DSN> MwDsnList = db.P_MW_DSN.Where(o => o.RECORD_ID == refNo && parentFormNoList.Contains(o.FORM_CODE))
                    .OrderBy(o => o.FORM_CODE)
                    .ToList();
                return MwDsnList;
            }
        }

        public P_MW_REFERENCE_NO getMwReferenceNoByRefNo(String refNo)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                P_MW_REFERENCE_NO P_MW_REFERENCE_NO = db.P_MW_REFERENCE_NO.Where(o => o.REFERENCE_NO == refNo).FirstOrDefault();
                return P_MW_REFERENCE_NO;
            }
        }

        public void saveNewICCRecord(P_MW_REFERENCE_NO referNo, P_MW_GENERAL_RECORD mwGeneralRecord, String sysPostCode)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                using (DbContextTransaction transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        if (String.IsNullOrEmpty(referNo.UUID))
                        {
                            db.P_MW_REFERENCE_NO.Add(referNo);
                            db.SaveChanges();
                        }
                        mwGeneralRecord.REFERENCE_NUMBER = referNo.UUID;
                        db.P_MW_GENERAL_RECORD.Add(mwGeneralRecord);
                        db.SaveChanges();

                        // if assignedOfficer = null
                        if (String.IsNullOrEmpty(sysPostCode))
                        {
                            ProcessingWorkFlowManagementService.Instance.StartWorkFlowEnquiry(db, mwGeneralRecord, referNo.REFERENCE_NO);
                        }
                        else
                        {
                            // assign case to specify handlingOfficer
                            ProcessingWorkFlowManagementService.Instance.StartWorkFlowEnquiryToOfficer(db, mwGeneralRecord, referNo.REFERENCE_NO, sysPostCode);
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
    }
}
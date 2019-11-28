using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MWMS2.Constant;
using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Entity;
using System.Globalization;

namespace MWMS2.Services
{
    public class ProcessingCommonService
    {
        public static P_MW_DSN createDsnRecord(string newDsn, string statusCode)
        {
            // save MwDsn
            P_MW_DSN mwDsn = new P_MW_DSN();
            mwDsn.DSN = newDsn;
            ProcessingSystemValueDAOService dao = new ProcessingSystemValueDAOService();
            P_S_SYSTEM_VALUE sysValue = dao.GetSSystemValueByTypeAndCode(
                   ProcessingConstant.SYSTEM_TYPE_DSN_STATUS, statusCode);
            mwDsn.SCANNED_STATUS_ID = sysValue.UUID;
            return mwDsn;
        }

        public static String formatFixedWidth(String s, int width)
        {
            String result = s;
            //int totalWidth = s.Length;
            for (int i = result.Length; result.Length < width; i++)
            {
                result = "0" + result;
            }
            return result;
        }

        public static String getDsnSequence(String latestRecord, String prefix, int width)
        {
            int newMwNo;
            String rawLatestRecord = latestRecord.Replace(prefix, "");
            if (!String.IsNullOrEmpty(rawLatestRecord))
            {
                newMwNo = (int.Parse(rawLatestRecord)) + 1;
            }
            else
            {
                newMwNo = 1;
            }
            return prefix + ProcessingCommonService.formatFixedWidth(newMwNo + "", width);
        }

        public static String getNewSequenceNo(String prefix, String prefixCode, String latestRecord)
        {
            String newNo = "";

            if (ProcessingConstant.PREFIX_D.Equals(prefix))
            {
                if (String.IsNullOrEmpty(latestRecord))
                {
                    latestRecord = "00";
                }

                int newMwNo = int.Parse(latestRecord.Substring(1)) + 1;
                newNo = formatFixedWidth(newMwNo + "", 10);
            }
            else
            {
                if (String.IsNullOrEmpty(latestRecord))
                {
                    latestRecord = prefixCode + "00001";
                }

                int newMwNo = int.Parse(latestRecord) + 1;
                newNo = formatFixedWidth(newMwNo + "", 9);
            }

            return newNo;
        }
        public static P_MW_REFERENCE_NO getMwReferenceNo(String referenceNo, String category)
        {
            P_MW_REFERENCE_NO mwReferenceNo = new P_MW_REFERENCE_NO();
            mwReferenceNo.CATEGORY_CODE = category;
            mwReferenceNo.REFERENCE_NO = referenceNo;

            return mwReferenceNo;
        }

        public static String SynchronizedSaveNewRecord(P_S_MW_NUMBER sMwNumber, String prefix)
        {
            MWPNewSubmissionDAOService MWPNewSubmissionDAOService = new MWPNewSubmissionDAOService();
            ProcessingSystemValueDAOService ProcessingSystemValueDAOService = new ProcessingSystemValueDAOService();

            String latestRecord = "";
            String newRecord = "";

            //Modify by dive 20191014
            List<P_S_SYSTEM_VALUE> sSystemValues =
                    ProcessingSystemValueDAOService.GetSSystemValueByType(ProcessingConstant.MW_NO_PREFIX);

            if (sSystemValues.Count == 0) { return null; }

            // get last record from db
            latestRecord = MWPNewSubmissionDAOService.getIccMaxNumber(prefix, sSystemValues[0].CODE);

            // last record +1
            newRecord = getNewSequenceNo(prefix, sSystemValues[0].CODE, latestRecord);

            if (ProcessingConstant.PREFIX_D.Equals(prefix))
            {
                sMwNumber.MW_NUMBER = prefix + newRecord;
            }
            else
            {
                sMwNumber.MW_NUMBER = newRecord;
            }
            

            MWPNewSubmissionDAOService.SaveSMwNo(sMwNumber);

            P_MW_REFERENCE_NO mwReferenceNo1 = null;
            P_MW_REFERENCE_NO mwReferenceNo2 = null;
            P_MW_REFERENCE_NO mwReferenceNo3 = null;
            P_MW_REFERENCE_NO mwReferenceNo4 = null;
            if ("MW".Equals(prefix))
            {
                mwReferenceNo1 = MWPNewSubmissionDAOService.getMwReferenceNoByRefNo("MW" + newRecord);
                mwReferenceNo2 = MWPNewSubmissionDAOService.getMwReferenceNoByRefNo("VS" + newRecord);
                mwReferenceNo3 = MWPNewSubmissionDAOService.getMwReferenceNoByRefNo("Enq" + newRecord);
                mwReferenceNo4 = MWPNewSubmissionDAOService.getMwReferenceNoByRefNo("Com" + newRecord);
            }
            if (mwReferenceNo1 != null || mwReferenceNo2 != null || mwReferenceNo3 != null || mwReferenceNo4 != null)
            {
                return SynchronizedSaveNewRecord(new P_S_MW_NUMBER(), prefix);
            }
            else
            {
                return prefix + newRecord;
            }

        }

        public string generateNumber(string numberType, string formNo, bool VsForMW01_MW03, string TypeOfRefNo, string inputRefNo)
        {
            MWPNewSubmissionDAOService MWPNewSubmissionDAOService = new MWPNewSubmissionDAOService();
            ProcessingSystemValueDAOService ProcessingSystemValueDAOService = new ProcessingSystemValueDAOService();

            String MWForm = "MWForm";
            String Enquiry = "Enquiry";
            String Complaint = "Complaint";
            String Modification = "Modification";

            // init current date
            DateTime currDate = DateTime.Now;
            String yearPrefix = currDate.Year.ToString();
            String yearYYYYStr = currDate.Year.ToString();
            yearPrefix = yearPrefix.Substring(yearPrefix.Length - 2);
            String monthPrefix = currDate.Month.ToString();
            monthPrefix = monthPrefix.Length == 1 ? "0" + monthPrefix : monthPrefix;
            String datePrefix = yearPrefix + monthPrefix;

            // get current max dsn no from db
            String currMaxDsnNo = MWPNewSubmissionDAOService.getMaxMwNumber(ProcessingConstant.PREFIX_D, "");

            // get current max MW/VS/Enq/Com reference no
            String currMaxRefNo = MWPNewSubmissionDAOService.getMaxMwNumber(ProcessingConstant.PREFIX_MW, datePrefix);

            // get current max Modification reference no
            String currMaxModiNo = MWPNewSubmissionDAOService.getMaxModNumber(yearYYYYStr);

            String newRefNo = "";
            if (Modification.Equals(numberType))
            {
                if (String.IsNullOrEmpty(currMaxModiNo))
                {
                    newRefNo = "1";
                }
                else
                { // current reference no +1
                    newRefNo = (int.Parse(currMaxModiNo) + 1) + "";
                }
            }
            else
            {
                if (String.IsNullOrEmpty(currMaxRefNo))
                {
                    // handle seq no not existing in db
                    newRefNo = getMwSubmissionNewSequenceNo(datePrefix, "");
                }
                else
                {
                    // current max reference no +1
                    newRefNo = getMwSubmissionNewSequenceNo(datePrefix, currMaxRefNo);
                }
            }

            // add prefix for refNo
            String formattedNewRefNo = ProcessingConstant.PREFIX_MW + newRefNo;
            if (ProcessingConstant.FORM_MW06.Equals(formNo) ||
                (((ProcessingConstant.FORM_MW01.Equals(formNo) || (ProcessingConstant.FORM_MW03.Equals(formNo)))
                && true == VsForMW01_MW03)))
            {
                formattedNewRefNo = ProcessingConstant.PREFIX_VS + newRefNo;
            }
            else if (Enquiry.Equals(numberType))
            {
                formattedNewRefNo = ProcessingConstant.PREFIX_ENQ + newRefNo;
            }
            else if (Complaint.Equals(numberType))
            {
                formattedNewRefNo = ProcessingConstant.PREFIX_COMP + newRefNo;
            }
            else if (Modification.Equals(numberType))
            {
                if ("newGen".Equals((TypeOfRefNo)))
                {
                    // e.g. MW 001/2019 (MOD)
                    formattedNewRefNo = "MW " + formatFixedWidth(newRefNo, 3) + "/" + yearYYYYStr + " (MOD)";
                }
                else
                {
                    // get input of existing Mod No
                    formattedNewRefNo = inputRefNo;
                }
            }

            // new dsn = current max dsn no +1
            int newDsnNo = int.Parse(currMaxDsnNo.Substring(1)) + 1;
            String formattedNewDsn = ProcessingConstant.PREFIX_D + formatFixedWidth(newDsnNo.ToString(), 10);

            return formattedNewDsn;
        }

        public String getMwSubmissionNewSequenceNo(String prefix, String latestRecord)
        {
            String newNo = "";
            if (String.IsNullOrEmpty(latestRecord))
            {
                newNo = prefix + "00001";
            }
            else
            {
                int newSeqNo = (int.Parse(latestRecord) + 1);
                //newNo = prefix + formatFixedWidth(newSeqNo.ToString(), 5);
                newNo = newSeqNo.ToString();
            }

            return newNo;
        }

        public static String getCategoryOutOfSubmitType(String submissionType)
        {
            String category = "";
            if (submissionType.Equals(ProcessingConstant.SUBMIT_TYPE_FORM))
            {
                category = ProcessingConstant.MW;
            }
            else if (submissionType.Equals(ProcessingConstant.SUBMIT_TYPE_VS))
            {
                category = ProcessingConstant.VS;
            }
            else if (submissionType.Equals(ProcessingConstant.SUBMIT_TYPE_ENQ))
            {
                category = ProcessingConstant.ENQ;
            }
            else if (submissionType.Equals(ProcessingConstant.SUBMIT_TYPE_COM))
            {
                category = ProcessingConstant.COM;
            }
            else if (submissionType.Equals(ProcessingConstant.SUBMIT_TYPE_MED))
            {
                category = ProcessingConstant.MED;
            }
            else if (submissionType.Equals(ProcessingConstant.SUBMIT_TYPE_PUB))
            {
                category = ProcessingConstant.PUB;
            }
            else if (submissionType.Equals(ProcessingConstant.SUBMIT_TYPE_LEG))
            {
                category = ProcessingConstant.LEG;
            }
            else if (submissionType.Equals(ProcessingConstant.SUBMIT_TYPE_ENF))
            {
                category = ProcessingConstant.ENF;
            }
            return category;
        }

        // EFSS
        public string getRegionFromOption(string option, string lang)
        {
            if (option.Equals("HK"))
            {
                return lang.Equals(ProcessingConstant.LANG_ENGLISH) ? "Hong Kong" : "香港";
            }
            else if (option.Equals("KLN"))
            {
                return lang.Equals(ProcessingConstant.LANG_ENGLISH) ? "Kowloon" : "九龍";
            }
            else if (option.Equals("NT"))
            {
                return lang.Equals(ProcessingConstant.LANG_ENGLISH) ? "New Territories" : "新界";
            }
            return null;
        }

        public string getAddressDisplayFormat(P_MW_ADDRESS address, string lang)
        {
            string display = "";
            if (lang.Equals(ProcessingConstant.LANG_ENGLISH))
            {
                display += stringUtil.getDisplay(address.LOCATION) + " "
                        + stringUtil.getDisplay(address.DISPLAY_FLAT) + " "
                        + stringUtil.getDisplay(address.DISPLAY_FLOOR) + " "
                        + stringUtil.getDisplay(address.DISPLAY_BUILDINGNAME) + " "
                        + stringUtil.getDisplay(address.DISPLAY_STREET_NO) + " "
                        + stringUtil.getDisplay(address.DISPLAY_STREET) + " "
                        + stringUtil.getDisplay(address.DISPLAY_DISTRICT) + " "
                        + stringUtil.getDisplay(getRegionFromOption(stringUtil.getDisplay(address.DISPLAY_REGION), lang));
            }
            else if (lang.Equals(ProcessingConstant.LANG_CHINESE))
            {
                display += address.LOCATION + " "
                        + stringUtil.getDisplay(getRegionFromOption(stringUtil.getDisplay(address.DISPLAY_REGION), lang)) + " "
                        + stringUtil.getDisplay(address.DISPLAY_DISTRICT) + " "
                        + stringUtil.getDisplay(address.DISPLAY_STREET) + " "
                        + stringUtil.getDisplay(address.DISPLAY_STREET_NO) + " "
                        + stringUtil.getDisplay(address.DISPLAY_BUILDINGNAME) + " "
                        + stringUtil.getDisplay(address.DISPLAY_FLOOR) + " "
                        + stringUtil.getDisplay(address.DISPLAY_FLAT);
            }
            return display;
        }

    }
}
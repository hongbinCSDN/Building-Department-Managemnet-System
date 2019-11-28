using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Constant
{
    public class ProcessingConstant
    {
        // Page code of SAC - Site Audit Check
        public const string PAGE_CODE_SAC = "SAC";
        public const string PAGE_CODE_WL = "WL";

        public const string SAC_ITEM_LIST = "SAC_ITEM_LIST";
        public const string SAC_ITEM_MW = "SAC_ITEM_MW";
        public const string SAC_ITEM_SB = "SAC_ITEM_SB";

        public const string SAC_MW = "MW";
        public const string SAC_SB = "SB";
        public const string SAC_MW_AND_SB = "MW_AND_SB";

        public const string HANDLING_UNIT_PEM = "PEM";
        public const string HANDLING_UNIT_SMM = "SMM";

        // TO/PO/SPO Acknowledgement Recommendation
        public const string IN_ORDER = "O";     // In Order
        public const string IN_ORDER_WITH_RECTIFICATION = "OWR";   // In Order with Rectification (For MW33 rectification)
        public const string ACKNOWLEDGEMENT_WITHDRAW = "AW";     // Acknowledgement Withdraw
        public const string SUPERSEDED = "S";    // Superseded
        public const string ISSUE_AL = "AL";    // Issue AL
        public const string TOLERATE = "T";     // Tolerate

        // Status code of MW_Record
        public const string MW_ACKNOWLEDGEMENT_COMPLETED = "MW_ACKNOWLEDGEMENT_COMPLETED";
        public const string MW_FIRST_NEW = "MW_FIRST_NEW";
        public const string MW_PO_ACKNOWLEDGEMENT = "MW_PO_ACKNOWLEDGEMENT";
        public const string MW_VERIFCAITON = "MW_VERIFCAITON";
        public const string MW_SECOND_COMPLETE_OLD = "MW_SECOND_COMPLETE_OLD";
        public const string MW_SPO_ACKNOWLEDGEMENT = "MW_SPO_ACKNOWLEDGEMENT";
        public const string MW_SECOND_COMPLETE_ERROR = "MW_SECOND_COMPLETE_ERROR";
        public const string MW_FINAL_VERSION = "MW_FINAL_VERSION";
        public const string MW_SECOND_ENTRY = "MW_SECOND_ENTRY";
        public const string MW_VERIFCAITON_COMPLETED = "MW_VERIFCAITON_COMPLETED";
        public const string MW_VERIFCAITON_SPO_COMPLETED = "MW_VERIFCAITON_SPO_COMPLETED";
        public const string MW_FIRST_ENTRY = "MW_FIRST_ENTRY";
        public const string MW_FIRST_COMPLETE = "MW_FIRST_COMPLETE";
        public const string MW_RECORD_COMPLETED = "MW_RECORD_COMPLETED";
        public const string MW_SECOND_COMPLETE = "MW_SECOND_COMPLETE";

        // List of Industrial Building Sub-Divided Flats
        public static String[] IB_SDF_List = {
            "1.42", "1.43", "1.44"
        };

        public const string TYPE_S_MW_ITEM = "S_MW_ITEM";
        public const string CODE_SIGNBOARD_ITEMS = "SIGNBOARD_ITEMS";
        public const string CODE_SDF_ITEMS = "SDF_ITEMS";

        // DSN Submit Type
        public const string MW_SUBMISSION = "MW Submission";
        public const string MW_ENQUIRY = "Enquiry";
        public const string MW_COMPLAINT = "Complaint";
        public const string MW_MODIFICATION = "Modification";

        // DSN Flow
        public const string MWU_TO_SECOND = "MWU_TO_SECOND";

        // Table: P_S_SYSTEM_TYPE Type
        public const string DSN_STATUS = "DSN_STATUS";

        // DSN_Status Code
        //public const string DSN_CHECKLIST_PHOTO_SCANNED = "DSN_CHECKLIST_PHOTO_SCANNED";
        //public const string DSN_CHECKLIST_SECTION_SCANNED = "DSN_CHECKLIST_SECTION_SCANNED";
        public const string DSN_PRINT_BARCODE_NEW = "DSN_PRINT_BARCODE_NEW";
        public const string MWU_OUTGOING_NEW_DISPATCH = "MWU_OUTGOING_NEW_DISPATCH";
        public const string MWU_OUTGOING_NEW_HOOK = "MWU_OUTGOING_NEW_HOOK";
        public const string MWU_OUTGOING_RESENT_DISPATCH = "MWU_OUTGOING_RESENT_DISPATCH";
        public const string MWU_OUTGOING_RESENT_HOOK = "MWU_OUTGOING_RESENT_HOOK";
        public const string MWU_OUTGOING_NEW_DISPATCH_SCANNED = "MWU_OUTGOING_NEW_DISPATCH_SCANNED";
        public const string MWU_OUTGOING_NEW_HOOK_SCANNED = "MWU_OUTGOING_NEW_HOOK_SCANNED";
        public const string MWU_OUTGOING_RESENT_DISPATCH_SCANNED = "MWU_OUTGOING_RESENT_DISPATCH_SCANNED";
        public const string MWU_OUTGOING_RESENT_HOOK_SCANNED = "MWU_OUTGOING_RESENT_HOOK_SCANNED";
        public const string MWU_OUTGOING_NEW_DISPATCH_COUNTED = "MWU_OUTGOING_NEW_DISPATCH_COUNTED";
        public const string MWU_OUTGOING_RESENT_DISPATCH_COUNTED = "MWU_OUTGOING_RESENT_DISPATCH_COUNTED";
        public const string MWU_OUTGOING_NEW = "MWU_OUTGOING_NEW";
        public const string MWU_OUTGOING_RESENT = "MWU_OUTGOING_RESENT";
        public const string MWU_RD_INCOMING_NEW = "MWU_RD_INCOMING_NEW";
        public const string MWU_RD_INCOMING_SCANNED = "MWU_RD_INCOMING_SCANNED";
        public const string DOCUMENT_SORTING = "DOCUMENT_SORTING";
        //public const string DSN_LETTER_NEW = "DSN_LETTER_NEW";
        //public const string DSN_CHECKLIST_SECTION_NEW = "DSN_CHECKLIST_SECTION_NEW";
        //public const string DSN_CHECKLIST_PHOTO_NEW = "DSN_CHECKLIST_PHOTO_NEW";
        public const string RD_RECEIVED = "RD_RECEIVED";
        public const string FIRST_ENTRY = "FIRST_ENTRY";
        public const string REGISTRY_RECEIVED = "REGISTRY_RECEIVED";
        public const string SCANNED = "SCANNED";
        public const string TBC = "TBC";
        public const string RD_DELIVER_COUNTED = "RD_DELIVER_COUNTED";
        public const string RD_DELIVERED = "RD_DELIVERED";
        public const string REGISTRY_RECEIPT_COUNTED = "REGISTRY_RECEIPT_COUNTED";
        //public const string DSN_REGISTRY_OUTSTANDING = "DSN_REGISTRY_OUTSTANDING";
        public const string REGISTRY_NEW = "REGISTRY_NEW";
        public const string REGISTRY_DELIVERED = "REGISTRY_DELIVERED";
        public const string RD_RECEIPT_COUNTED = "RD_RECEIPT_COUNTED";
        //public const string DSN_REGISTRY_RE_SENT = "DSN_REGISTRY_RE_SENT";
        //public const string DSN_REGISTRY_MISSING = "DSN_REGISTRY_MISSING";
        public const string CONFIRMED = "CONFIRMED";
        public const string FIRST_ENTRY_COMPLETED = "FIRST_ENTRY_COMPLETED";
        public const string SECOND_ENTRY = "SECOND_ENTRY";
        public const string DISPLAY_SECOND_ENTRY = "SECOND ENTRY";
        public const string SECOND_ENTRY_COMPLETED = "SECOND_ENTRY_COMPLETED";
        public const string CONFIRMED_WITHOUT_MW_NO = "CONFIRMED_WITHOUT_MW_NO";
        //public const string DSN_RD_OUTSTANDING = "DSN_RD_OUTSTANDING";
        //public const string DSN_RD_RE_SENT = "DSN_RD_RE_SENT";
        //public const string DSN_RD_MISSING = "DSN_RD_MISSING";
        //public const string MW__VERSION = "MW__VERSION";
        public const string RESCANNED = "RESCANNED";
        public const string GENERAL_ENTRY = "GENERAL_ENTRY";
        public const string GENERAL_COMPLETED = "GENERAL_COMPLETED";
        public const string WILL_SCAN = "WILL_SCAN";
        public const string DISPLAY_WILL_SCAN = "WILL SCAN";
        public const string GENERAL_ENTRY_WILL_SCAN = "GENERAL_ENTRY_WILL_SCAN";
        //public const string DSN_LETTER_CONFIRMED = "DSN_LETTER_CONFIRMED";
        //public const string DSN_REGISTRY_DELIVERED_COUNTED = "DSN_REGISTRY_DELIVERED_COUNTED";
        //public const string DSN_RD_RE_SENT_TBC = "DSN_RD_RE_SENT_TBC";
        //public const string DSN_RD_RE_SENT_COUNTED = "DSN_RD_RE_SENT_COUNTED";
        public const string RD_RECEIVED_ASL = "RD_RECEIVED_ASL";

        // User role
        public const string TO = "TO";
        public const string PO = "PO";
        public const string SPO = "SPO";

        public const string AdministrationGroup = "SPO";

        // Flag
        public const string FLAG_Y = "Y";
        public const string FLAG_N = "N";
        public const string FLAG_M = "M";

        // PEM Reference No prefix 
        public const string PREFIX_D = "D";
        public const string PREFIX_OI = "OI";
        public const string PREFIX_MW = "MW";
        public const string PREFIX_ENQ = "Enq";
        public const string PREFIX_COMP = "Com";
        public const string PREFIX_MOD = "MOD";
        public const string PREFIX_VS = "VS";
        public const string PREFIX_ICC = "ICC";

        // PEM form 
        public const string FORM_MW01 = "MW01";
        public const string FORM_MW02 = "MW02";
        public const string FORM_MW03 = "MW03";
        public const string FORM_MW04 = "MW04";
        public const string FORM_MW05 = "MW05";
        public const string FORM_MW06 = "MW06";
        public const string FORM_MW06_01 = "MW06_01";
        public const string FORM_MW06_02 = "MW06_02";
        public const string FORM_MW06_03 = "MW06_03";
        public const string FORM_MW07 = "MW07";
        public const string FORM_MW08 = "MW08";
        public const string FORM_MW09 = "MW09";
        public const string FORM_MW10 = "MW10";
        public const string FORM_MW11 = "MW11";
        public const string FORM_MW12 = "MW12";
        public const string FORM_MW31 = "MW31";
        public const string FORM_MW32 = "MW32";
        public const string FORM_MW33 = "MW33";
        public const string FORM_BA16 = "BA16";

        public static String[] validMWFormNos = {
            FORM_MW01,
            FORM_MW02,
            FORM_MW03,
            FORM_MW04,
            FORM_MW05,
            FORM_MW06,
            FORM_MW06_01,
            FORM_MW06_02,
            FORM_MW06_03,
            FORM_MW07,
            FORM_MW08,
            FORM_MW09,
            FORM_MW10,
            FORM_MW11,
            FORM_MW12,
            FORM_MW31,
            FORM_MW32,
            FORM_MW33,
        };







        //public const string SYSTEM_PATH_SEPARATOR = System.getProperty("file.separator");

        public const string VS_1_STRUCTURES_ITEM_UUID = "8a8593472789193101278919324807324";
        public const string VS_2_STRUCTURES_ITEM_UUID = "8a859347278919310127891932480744";
        public const string VS_3_STRUCTURES_ITEM_UUID = "8a859347278919310127891932480754";
        public const string VS_4_STRUCTURES_ITEM_UUID = "8a859347278919310127891932480764";

        public static String[] VS_STRUCTURES_ITEM_UUIDS = new String[] { VS_2_STRUCTURES_ITEM_UUID, VS_3_STRUCTURES_ITEM_UUID, VS_4_STRUCTURES_ITEM_UUID };



        public const int ADMIN_USER_ID = -1;//Admin User id

        public const int USER_ACTIVE = 1;//Admin User id


        public const int PASSWORDDURATION = 60;//update password's days

        //Form Using	
        public const string HONG_KONG = "HK";
        public const string KOWLOON = "KLW";
        public const string NEW_TERRITORIES = "NT";
        public const string DEFAULT_REGION = "HK";

        public const string LABEL_HONG_KONG = "Hong Kong";
        public const string LABEL_KOWLOON = "Kowloon";
        public const string LABEL_NEW_TERRITORIES = "New Territories";

        public const string HKID_NO = "1";
        public const string PASSPORT_NO = "2";
        public const string BUSINESS_REGISTRATION = "3";
        public const string OTHER_ID_TYPE = "4";

        public const string LABEL_HKID_NO = "HKID No.";
        public const string LABEL_PASSPORT_NO = "Passport No.";
        public const string LABEL_BUSINESS_REGISTRATION = "Business Registration No.";
        public const string LABEL_OTHER_ID_TYPE = "Other";

        public const string LANG_ENGLISH = "EN";
        public const string LANG_CHINESE = "ZH";
        public const string LANG_CHINESE_FOR_LETTER = "CN";


        //DSN status -- in DB
        public const String DSN_ALL = "";//ALL

        public const String SYSTEM_TYPE_DSN_STATUS = "DSN_STATUS";


        public const String DSN_TBC = "TBC";//R&D generate Barcode
        public const String DSN_RD_DELIVER_COUNTED = "RD_DELIVER_COUNTED";//R&D delivery counted
        public const String DSN_RD_DELIVERED = "RD_DELIVERED";//R & Dconfirm delivery ok
        public const String DSN_REGISTRY_RECEIPT_COUNTED = "REGISTRY_RECEIPT_COUNTED";//REGISTRY_RECEIPT_COUNTED
        public const String DSN_REGISTRY_RECEIVED = "REGISTRY_RECEIVED";//Registry received ok
        public const String DSN_WILL_SCAN = "WILL_SCAN";//Registry received ok
        public const String DSN_GENERAL_ENTRY_WILL_SCAN = "GENERAL_ENTRY_WILL_SCAN";//Registry received ok
        public const String DSN_REGISTRY_NEW = "REGISTRY_NEW";//REGISTRY_NEW
        public const String DSN_REGISTRY_DELIVERED_COUNTED = "DSN_REGISTRY_DELIVERED_COUNTED";//DSN_REGISTRY_DELIVERED_COUNTED
        public const String DSN_REGISTRY_DELIVERED = "REGISTRY_DELIVERED";//REGISTRY_DELIVERED
        public const String DSN_RD_RECEIPT_COUNTED = "RD_RECEIPT_COUNTED";//RD_RECEIPT_COUNTED
        public const String DSN_RD_RECEIVED = "RD_RECEIVED";//R & D received	
        public const String DSN_RD_OUTSTANDING = "DSN_RD_OUTSTANDING";//outstanding doc
        public const String DSN_RD_RE_SENT = "DSN_RD_RE_SENT";//outstanding sent
        public const String DSN_RD_MISSING = "DSN_RD_MISSING";//outstanding missing
        public const String DSN_REGISTRY_OUTSTANDING = "DSN_REGISTRY_OUTSTANDING";//outstanding doc
        public const String DSN_REGISTRY_RE_SENT = "DSN_REGISTRY_RE_SENT";//outstanding sent
        public const String DSN_REGISTRY_MISSING = "DSN_REGISTRY_MISSING";//outstanding missing
                                                                          //	public const String DSN_PRINT_BARCODE_NEW="DSN_PRINT_BARCODE_NEW";

        public const String DSN_RD_RE_SENT_TBC = "DSN_RD_RE_SENT_TBC";//DSN_RD_RE_SENT_TBC
        public const String DSN_RD_RE_SENT_COUNTED = "DSN_RD_RE_SENT_COUNTED";//DSN_RD_RE_SENT_COUNTED

        public const String DSN_SCANNED = "SCANNED";//DSN scanned
        public const String DSN_RE_SCANNED = "RESCANNED";//DSN Rescanned

        public const String DSN_CONFIRMED = "CONFIRMED";//DSN confirm
        public const String DSN_CONFIRMED_WITHOUT_MW_NO = "CONFIRMED_WITHOUT_MW_NO";//DSN confirm without mw

        public const String DSN_FIRST_ENTRY = "FIRST_ENTRY";//DSN FIRST_ENTRY
        public const String DSN_FIRST_ENTRY_COMPLETED = "FIRST_ENTRY_COMPLETED";//DSN FIRST_ENTRY_COMPLETED
        public const String DSN_SECOND_ENTRY = "SECOND_ENTRY";//DSN SECOND_ENTRY
        public const String DSN_SECOND_ENTRY_COMPLETED = "SECOND_ENTRY_COMPLETED";//DSN SECOND_ENTRY_COMPLETED
        public const String DSN_GENERAL_ENTRY = "GENERAL_ENTRY";//DSN GENERAL_ENTRY
        public const String DSN_GENERAL_ENTRY_COMPLETED = "GENERAL_COMPLETED";//DSN GENERAL_ENTRY_COMPLETED

        public const String DSN_DOCUMENT_SORTING = "DOCUMENT_SORTING";//DSN_DOCUMENT_SORTING
        public const String DSN_LETTER_NEW = "DSN_LETTER_NEW";//DSN_LETTER_NEW
        public const String DSN_LETTER_CONFIRMED = "DSN_LETTER_CONFIRMED";//DSN_LETTER_CONFIRMED
        public const String DSN_CHECKLIST_SECTION_NEW = "DSN_CHECKLIST_SECTION_NEW";//DSN_CHECKLIST_SECTION_NEW
        public const String DSN_CHECKLIST_PHOTO_NEW = "DSN_CHECKLIST_PHOTO_NEW";//DSN_CHECKLIST_PHOTO_NEW


        public const String DSN_CHECKLIST_SECTION_SCANNED = "DSN_CHECKLIST_SECTION_SCANNED";//DSN_CHECKLIST_SECTION_SCANNED
        public const String DSN_CHECKLIST_PHOTO_SCANNED = "DSN_CHECKLIST_PHOTO_SCANNED";//DSN_CHECKLIST_PHOTO_SCANNED

        //DSN status -- display
        public const String DSN_DISPLAY_ALL = "ALL";//ALL
        public const String DSN_DISPLAY_TBC = "TBC";//R&D generate Barcode
        public const String DSN_DISPLAY_COUNTED = "Counted";
        public const String DSN_DISPLAY_DELIVERED = "Delivered";
        public const String DSN_DISPLAY_REGISTRY_RECEIVED = "Received";//Registry received ok
        public const String DSN_DISPLAY_DOC_SENT = "docSent";
        public const String DSN_DISPLAY_DOC_MISSING = "docMissing";
        public const String DSN_DISPLAY_NEW = "New";
        public const String DSN_DISPLAY_INCOMING = "Incoming";
        public const String DSN_DISPLAY_SCANNED = "Scanned";//DSN scanned
        public const String DSN_DISPLAY_CONFIRMED_WITHOUT_MW_NO = "Scanned Image confirmed";//DSN confirm without mw
        public const String DSN_DISPLAY_RE_SCANNED = "Rescan";//DSN Rescanned
        public const String DSN_DISPLAY_CONFIRMED = "Confirmed";//DSN confirm
        public const String DSN_DISPLAY_DRAFT = "Draft";//DSN FIRST_ENTRY
        public const String DSN_DISPLAY_WILL_SCAN = "Waiting for scan";//DSN FIRST_ENTRY
        public const String DSN_DISPLAY_GENERAL_ENTRY_WILL_SCAN = "Waiting for scan";//Registry received ok
        public const String DSN_DISPLAY_SENT = "Sent";
        public const String DSN_DISPLAY_RE_SENT = "Resent";
        public const String DSN_DISPLAY_MISSING = "Missing";
        public const String DSN_DISPLAY_UNDELIVERED = "Undelivered";
        public const String DSN_DISPLAY_FIRST_ENTRY_COMPLETED = "First Entry Completed";//DSN FIRST_ENTRY_COMPLETED
        public const String DSN_DISPLAY_COMPLETED = "Completed";//DSN_GENERAL_ENTRY_COMPLETED

        public const String DSN_DISPLAY_RE_ASSIGN = "Re-assign";//DSN re-assign
        public const String DSN_DISPLAY_UNKNOWN = "Unknown";//DSN Unknown
        public const String DSN_DISPLAY_OTHER = "Other";//Other

        public const String DSN_DISPLAY_RE_SENT_TBC = "Re-sent TBC";//DSN re-assign
        public const String DSN_DISPLAY_RE_SENT_COUNTED = "Re-sent counted";//DSN re-assign

        //MW record STATUS -new-first entry-first entry completed -Vert -Ack - and so on 

        public const String MW__VERSION = "MW__VERSION";



        public const String MW_VERT_STATUS_OPEN = "MW_VERT_STATUS_OPEN";
        public const String MW_VERT_STATUS_ROLLBACK = "MW_VERT_STATUS_ROLLBACK";


        public const String MW_ACKN_STATUS_OPEN = "MW_ACKN_STATUS_OPEN";
        public const String MW_ACKN_STATUS_COMPLETE = "MW_ACKN_STATUS_COMPLETE";


        public const String MW_DSN_NATURE = "MW_DSN_NATURE";
        public const String MW_DSN_NATURE_TO_ICU = "MW_DSN_NATURE_TO_ICU";
        public const String MW_DSN_NATURE_SYSTEM_ERROR = "MW_DSN_NATURE_SYSTEM_ERROR";
        public const String MW_DSN_NATURE_WITHDRAWN = "MW_DSN_NATURE_WITHDRAWN";
        public const String MW_DSN_NATURE_BY_D_LETTER = "MW_DSN_NATURE_BY_D_LETTER";


        public const String RECEIPT_DOC_TYPE_FORM = "F";
        public const String RECEIPT_DOC_TYPE_ENQUIRY = "E";
        public const String RECEIPT_DOC_TYPE_COMPLAINT = "C";
        public const String RECEIPT_FORM_TYPE_AUTO = "A";
        public const String RECEIPT_FORM_TYPE_MANUAL = "M";

        public const String SYSTEM_VALUE_TYPE_RI_VALID_MW_ITEM = "RI Valid MW Item";

        // For S_TO_DETAILS table
        public const String TO_DETAILS_IS_ACTIVE_YES = "Y";
        public const String TO_DETAILS_IS_ACTIVE_NO = "N";
        public const String TO_DETAILS_IS_ACTIVE_DEFAULT = "D";
        public const String TO_DETAILS_DEFAULT_SPO_POST = "SYS_DEFAULT_SPO";


        // TODO: to add the MwRecord status after Acknowledgement function

        public const String MW_BLANK = "[BLANK]";

        /**
         * When the current status is entered, the return status list will include all the future status 
         * and the current status in the workflow.
         * 
         * @param beginStatus
         * @param endStatus TODO
         * @return The comming MwRecord Status List (return null if status does not found)
         */
        public List<string> getFutureMwRecordStatusListByStatus(String beginStatus, String endStatus)
        {
            // TODO:to add the MwRecord status after Acknowledgement function in the statusFlowList

            // status list which ordered in workflow processes
            List<string> statusFlowList =
                (new string[]{
                    MW_FIRST_NEW,                   MW_FIRST_ENTRY,                 MW_FIRST_COMPLETE,
                    MW_SECOND_ENTRY,                MW_SECOND_COMPLETE,             MW__VERSION,
                    MW_VERIFCAITON,
                    MW_VERIFCAITON_SPO_COMPLETED,   MW_VERIFCAITON_COMPLETED,       MW_PO_ACKNOWLEDGEMENT,
                    MW_SPO_ACKNOWLEDGEMENT,         MW_ACKNOWLEDGEMENT_COMPLETED,   MW_RECORD_COMPLETED
                }).ToList();




            if (!statusFlowList.Contains(beginStatus))
            {
                return null;
            }
            else
            {
                int idx = statusFlowList.IndexOf(beginStatus);
                int lastIdx;
                if (endStatus != null)
                {
                    lastIdx = statusFlowList.IndexOf(endStatus);
                }
                else
                {
                    lastIdx = statusFlowList.Count;
                }
                //return statusFlowList.GetRange(idx, lastIdx);
                // Begin add by Chester 2019-09-05
                return statusFlowList.GetRange(idx, (lastIdx - idx));
                //End add by Chester 2019 - 09 - 05
            }

        }
        public String getFormNameExist(string formName)
        {

            // status list which ordered in workflow processes
            List<String> formList = (new String[]{
                FORM_01, FORM_02, FORM_03, FORM_04, FORM_05, FORM_06, FORM_07, FORM_08, FORM_09,
                FORM_10, FORM_11, FORM_12 , FORM_31 , FORM_32 , FORM_33 ,FORM_NODATA
        }).ToList();

            if (!formList.Contains(formName.ToUpper()))
            {
                return "N";
            }
            else
            {
                return "Y";
            }

        }


        public const String CLASS_1 = "Class 1";
        public const String CLASS_2 = "Class 2";
        public const String CLASS_3 = "Class 3";

        public static List<String> getFutureClassCodeListByCurrentClassCode(String curClass)
        {

            // status list which ordered in workflow processes
            List<String> classCodeList = (new String[]{
                CLASS_1,CLASS_2,CLASS_3
        }).ToList();

            if (!classCodeList.Contains(curClass))
            {
                return null;
            }
            else
            {
                int idx = classCodeList.IndexOf(curClass);
                int lastIdx = classCodeList.Count;
                return classCodeList.GetRange(idx, lastIdx);
            }

        }

        //MW RECORD Is Data Entry
        public const String MW_IS_DATA_ENTRY = "Y";
        public const String MW_NOT_DATA_ENTRY = "N";

        //Form Type Id
        public const String FORM_01 = "MW01";
        public const String FORM_02 = "MW02";
        public const String FORM_03 = "MW03";
        public const String FORM_04 = "MW04";
        public const String FORM_05 = "MW05";
        public const String FORM_05_36 = "MW05(ITEM3.6)";
        public const String FORM_06 = "MW06";
        public const String FORM_07 = "MW07";
        public const String FORM_08 = "MW08";
        public const String FORM_09 = "MW09";
        public const String FORM_10 = "MW10";
        public const String FORM_11 = "MW11"; // Form 13
        public const String FORM_12 = "MW12"; // Form 14
        public const String FORM_31 = "MW31"; // Form 11
        public const String FORM_32 = "MW32"; // Form 12
        public const String FORM_33 = "MW33"; // From 15
        public const String FORM_NODATA = "NODATA";

        /**
        public const  String[] REPORT_COMMENCEMENT_FORM_CODES 	= new String[]{ FORM_01, FORM_03, FORM_11};
        public const  String[] REPORT_COMPLETION_FORM_CODES 		= new String[]{ FORM_02, FORM_04, FORM_05, FORM_12 };
        public const  String[] REPORT_VALIDATION_SCHEME_FORM_CODES= new String[]{ FORM_06 };
        public const  String[] REPORT_SUMMARY_FORM_CODES = 
            new String[]{ FORM_01, FORM_03, FORM_11, FORM_02, FORM_04, FORM_05, FORM_12, FORM_06};
        **/

        public static String[] REPORT_COMMENCEMENT_CLASS_I_FORM_CODES = new String[]{ FORM_01, FORM_11
    };
        public static String[] REPORT_COMPLETION_CLASS_I_FORM_CODES = new String[]{ FORM_02
};

        public static String[] REPORT_COMMENCEMENT_CLASS_II_FORM_CODES = new String[] { FORM_03, FORM_12 };
        public static String[] REPORT_COMPLETION_CLASS_II_FORM_CODES = new String[] { FORM_04 };

        public static String[] REPORT_COMPLETION_CLASS_III_FORM_CODES = new String[] { FORM_05 };
        public static String[] REPORT_VALIDATION_SCHEME_FORM_CODES = new String[] { FORM_06 };

        public static String[] REPORT_SUMMARY_FORM_CODES =
            new String[] { FORM_01, FORM_11, FORM_02, FORM_03, FORM_12, FORM_04, FORM_05, FORM_06 };





        public const String FORM_SAVE_MODE = "MODE";
        public const String FORM_SAVE_MODE_DRAFT = "DRAFT";
        public const String FORM_SAVE_MODE_SUBMIT = "SUBMIT";
        public const String FORM_SAVE_MODE_OVERRIDE = "OVERRIDE";

        //Form Part
        public const String PART_A = "PART_A";
        public const String PART_B = "PART_B";
        public const String PART_C = "PART_C";
        public const String PART_D = "PART_D";
        public const String PART_E = "PART_E";
        public const String PART_F = "PART_F";
        public const String PART_G = "PART_G";

        //record type
        //	public const String MW_RECORD="MW_RECORD";

        //identify flag
        public const String AP = "AP";
        public const String RI = "RI";
        public const String RSE = "RSE";
        public const String RGE = "RGE";
        public const String PRC = "PRC";
        public const String PBP = "PBP";

        public const String RGBC = "RGBC";

        public const String OTHER = "OTHER";

        public const String REVISED1 = "REVISED1";
        public const String REVISED2 = "REVISED2";
        public const String REVISED3 = "REVISED3";

        //form return event
        public const String FORM_01_IN = "form1";
        public const String FORM_02_IN = "form2";
        public const String FORM_03_IN = "form3";
        public const String FORM_04_IN = "form4";
        public const String FORM_05_IN = "form5";
        public const String FORM_06_IN = "form6";
        public const String FORM_07_IN = "form7";
        public const String FORM_08_IN = "form8";
        public const String FORM_09_IN = "form9";
        public const String FORM_10_IN = "form10";
        public const String FORM_31_IN = "form31";
        public const String FORM_32_IN = "form32";
        public const String FORM_11_IN = "form11";
        public const String FORM_12_IN = "form12";
        public const String FORM_33_IN = "form33";
        public const String FORM_NEW_SUFFIX = "_new";
        //doc forward
        public const String INCOMING = "Incoming";
        public const String OUTGOING = "Outgoing";

        //barcode type
        public const String PREFIX_DSN = "D";
        public const String DSN = "DSN";
        public const String MW = "MW";
        public const String VS = "VS";
        public const String ENQ = "Enq";
        public const String COM = "Com";
        public const String MED = "Med";
        public const String PUB = "Pub";

        public const String PREFIX_OUTSTANDING_ITEM = "OI";

        //
        public const String OPTION_ALL_VALUE = "";
        public const String OPTION_ALL = "-All-";
        public const String OPTION_SELECT = "- Select -";

        //enquiry status
        public const String ENQUIRY_STATUS_IN_PROGRESS_DISPALY = "In Progress";
        public const String ENQUIRY_STATUS_OUTSTANDING = "Outstanding";
        public const String ENQUIRY_STATUS_INTERIM_DISPLAY = "Interim Reply Sent";
        public const String ENQUIRY_STATUS_INTERIM = "Interim Replied";
        public const String ENQUIRY_STATUS_ENDORSED = "Endorsed";
        public const String ENQUIRY_STATUS_ = " Replied";
        public const String ENQUIRY_STATUS_CLOSED = "Closed";

        //	public const String ENQUIRY_DB_STATUS_OUTSTANDING="OUTSTANDING";
        //	public const String ENQUIRY_DB_STATUS_INTERIM="INTERIM_REPLIED";
        //	public const String ENQUIRY_DB_STATUS_="_REPLIED";
        //	public const String ENQUIRY_DB_STATUS_CLOSED="CLOSED";

        // SOURCE
        public const String SOURCE_ICC = "ICC";
        public const String SOURCE_RESOURCE_CENTER = "Resource Center";
        public const String SOURCE_GENERAL_PUBLIC = "General Public";
        public const String SOURCE_INTERNAL_REFERRAL = "Internal Referral";
        public const String SOURCE_OTHERS = "Others";

        public const String DB_SOURCE_ICC = "ICC";
        public const String DB_SOURCE_RESOURCE_CENTER = "RESOURCE_CENTER";
        public const String DB_SOURCE_GENERAL_PUBLIC = "GENERAL_PUBLIC";
        public const String DB_SOURCE_INTERNAL_REFERRAL = "INTERNAL_REFERRAL";
        public const String DB_SOURCE_OTHERS = "OTHERS";

        //channel
        public const String CHANNEL_EMAIL = "Email";
        public const String CHANNEL_FAX = "Fax";
        public const String CHANNEL_LETTER = "Letter";
        public const String CHANNEL_TELEPHONE = "Telephone";
        public const String CHANNEL_MEMO = "Memo";
        public const String CHANNEL_FORM = "Record Form";


        //enquiry PROGRESS
        public const String ENQUIRY_PROGRESS_SHOW_ALL = "Show All";
        public const String ENQUIRY_PROGRESS_SHOW_OVERDUE = "Show Overdue";
        public const String ENQUIRY_PROGRESS_SHOW_ALERT = "Show Alert";

        public const String ENQUIRY_DB_PROGRESS_SHOW_ALL = "SHOW_ALL";
        public const String ENQUIRY_DB_PROGRESS_SHOW_OVERDUE = "SHOW_OVERDUE";
        public const String ENQUIRY_DB_PROGRESS_SHOW_ALERT = "SHOW_ALERT";

        //enquiry PROGRESS
        public const String ENQUIRY_REPORT = "Enquiry";
        public const String COMPLAINT_REPORT = "Complaint";

        //enquiry nature
        public const String ENQUIRY_NATURE_MW = "MW";
        public const String ENQUIRY_NATURE_OTHERS = "Others";

        //enquiry type
        public const String ENQUIRY_TYPE_A = "A";
        public const String ENQUIRY_TYPE_B = "B";
        public const String ENQUIRY_TYPE_C = "C";
        public const String ENQUIRY_TYPE_D = "D";
        public const String ENQUIRY_TYPE_E = "E";
        public const String ENQUIRY_TYPE_F = "F";
        public const String ENQUIRY_TYPE_G = "G";

        //enquiry title
        public const String ENQUIRY_TITLE_MR = "Mr";
        public const String ENQUIRY_TITLE_MS = "Ms";
        public const String ENQUIRY_TITLE_MRS = "Mrs";
        public const String ENQUIRY_TITLE_DR = "Dr";
        public const String DOT = ".";

        //statistic report item
        public const String STATISTIC_REPORT_MW = "Minor Works";
        public const String STATISTIC_REPORT_VS = "Validation Scheme";
        public const String STATISTIC_REPORT_ENQ = "Enquiry";
        public const String STATISTIC_REPORT_COM = "Complaint";
        public const String STATISTIC_REPORT_MED = "Media";
        public const String STATISTIC_REPORT_PUB = "Publicity";

        //statistic summary report item
        public const String STATISTIC_SUMMARY_BY_CLASS = "By Class";
        public const String STATISTIC_SUMMARY_BY_FORM = "By Form";


        //class
        public const String DB_CLASS_I = "CLASS_I";
        public const String DB_CLASS_II = "CLASS_II";
        public const String DB_CLASS_III = "CLASS_III";
        public const String DB_CLASS_VS = "CLASS_VS";

        public const String DISPLAY_CLASS_I = "Class I";
        public const String DISPLAY_CLASS_II = "Class II";
        public const String DISPLAY_CLASS_III = "Class III";
        public const String DISPLAY_CLASS_VS = "Validation Scheme";

        //report format
        public const String MONTHLY = "Monthly";
        public const String YEARLY = "Yearly";

        //submission type
        public const String COMMENCEMENT = "Commencement";
        public const String COMPLETION = "Completion";
        public const String VALIDATION_SCHEME = "Validation Scheme";
        public const String AUDIT = "Audit";
        public const String NON_AUDIT = "Non Audit";
        public const String ORIGINAL = "Original";

        //DSN MW_SCANNED_DOCUMENT DOCUMENT_TYPE
        public const String DSN_DOCUMENT_TYPE_LETTER = "Letter";
        public const String DSN_DOCUMENT_TYPE_FORM = "Form";
        public const String DSN_DOCUMENT_TYPE_AMENDED_FORM = "Amended Form";

        public const String DSN_DOCUMENT_TYPE_PLAN = "Plan";
        public const String DSN_DOCUMENT_TYPE_AMENDED_PLAN = "Amended Plan";
        public const String DSN_DOCUMENT_TYPE_PHOTO = "Photo";
        public const String DSN_DOCUMENT_TYPE_AMENDED_PHOTO = "Amended Photo";



        public const String DSN_DOCUMENT_TYPE_SSP = "SSP";
        public const String DSN_DOCUMENT_TYPE_OTHER = "Other";




        //RRM MW_SCANNED_DOCUMENT DOCUMENT_TYPE
        public const string RRM_DOCUMENT_TYPE_PLAN = "PLAN";
        public const string RRM_DOCUMENT_TYPE_PHOTO = "PHOTO";
        public const string RRM_DOCUMENT_TYPE_DOCUMENT = "DOCUMENT";


        public const String DSN_FOLDER_TYPE_PUBLIC = "Public";
        public const String DSN_FOLDER_TYPE_PRIVATE = "Private";
        //	public const String DSN_FOLDER_TYPE_ISSUED = "Issued Document";

        //general record status
        public const String GENERAL_ENTRY_NEW = "General Entry New";
        public const String GENERAL_ENTRY_DRAFT = "General Entry Draft";
        public const String GENERAL_RECORD_NEW = "General Record New";
        public const String GENERAL_RECORD_DRAFT = "General Record Draft";
        public const String GENERAL_RECORD_COMPLETED = "General Record Completed";

        //page size

        public const int ENQUIRY_PAGE_SIZE = 10;
        public const int AUDIT_DATA_CONTRAOL_PAGE_SIZE = 50;

        public const int SCAN_AND_ASSIGN_PAGE_SIZE = 500000000;

        public const int ACK_SUBMIT_SEARCH_PAGE_SIZE = 300;

        //submit type
        public const String SUBMIT_TYPE_FORM = "MW Submission";
        public const String SUBMIT_TYPE_ENQ = "Enquiry";
        public const String SUBMIT_TYPE_COM = "Complaint";
        public const String SUBMIT_TYPE_MED = "Media";
        public const String SUBMIT_TYPE_PUB = "Publicity";
        public const String SUBMIT_TYPE_ISSUED_CORR = "Issued Correspondence"; //Outgoing
        public const String SUBMIT_TYPE_INCOMING_CORR = "Incomming Correspondence";
        public const String SUBMIT_TYPE_UNKNOWN = "Unknown";

        public const String SUBMIT_TYPE_DSN_STATUS = "DSN_STATUS";

        public const String SUBMIT_TYPE_VS = "Validation Scheme";
        public const String SUBMIT_TYPE_LEG = "Legal";
        public const String SUBMIT_TYPE_ENF = "Enforcement";

        public const String SUBMIT_TYPE_ENQ_INPUT = "Enquiry Input";//for enquiry input only


        public const String SUBMISSION_DISPLAY_NATURE_SUBMISSION = "Submission";
        public const String SUBMISSION_DISPLAY_NATURE_ESUBMISSION = "eSubmission";
        public const String SUBMISSION_DISPLAY_NATURE_ICU = "ICU";
        public const String SUBMISSION_DISPLAY_NATURE_DIRECT_RETURN = "Direct Return";
        public const String SUBMISSION_DISPLAY_NATURE_REVISED_CASE = "Revised Case";
        public const String SUBMISSION_DISPLAY_NATURE_WITHDRAWAL = "Withdrawal";

        //public const String IMPORT_NATURE_DSN_DELETE = "DSN-DELETE";

        public const String SUBMISSION_VALUE_NATURE_SUBMISSION = "SUBMISSION";
        public const String SUBMISSION_VALUE_NATURE_ESUBMISSION = "E-SUBMISSION";
        public const String SUBMISSION_VALUE_NATURE_ICU = "ICU";
        public const String SUBMISSION_VALUE_NATURE_DIRECT_RETURN = "DIRECT RETURN";
        public const String SUBMISSION_VALUE_NATURE_REVISED_CASE = "REVISED CASE";
        public const String SUBMISSION_VALUE_NATURE_WITHDRAWAL = "WITHDRAWAL";



        public const String LEG = "Leg";
        public const String ENF = "Enf";

        //S_MW_ITEM_NATURE
        public const String NATURE_ADDITION = "Addition";
        public const String NATURE_ALTERATION = "Alteration";
        public const String NATURE_CONSTRUCTION = "Construction";
        public const String NATURE_ERECTION = "Erection";
        public const String NATURE_FORMATION = "Formation";
        public const String NATURE_INSTALLATION = "Installation";
        public const String NATURE_LAYING = "Laying";
        public const String NATURE_REINSTATEMENT = "Reinstatement";
        public const String NATURE_REMOVAL = "Removal";
        public const String NATURE_REPAIR = "Repair";
        public const String NATURE_REPLACEMENT = "Replacement";
        public const String NATURE_STRENGTHENING = "Strengthening";

        //mw progress status code
        public const String MW_STATUS_COMMENCEMENT = "Commencement";
        public const String MW_STATUS_IN_PROGRESS = "In Progress";
        public const String MW_STATUS_COMPLETED = "Completed";

        //display Status
        public const String STATUS_OPEN = "Open";
        public const String STATUS_IN_PROGRESS = "In progress";
        public const String STATUS_PO_COMPLETED = "PO Completed";
        public const String STATUS_COMPLETED = "Completed";

        //professional type
        public const String AUTHORIZED_PERSON = "Authorized Person";
        public const String REGISTERED_INSPECTOR = "Registered Inspector";
        public const String REGISTERED_STRUCTURAL_ENGINEER = "Registered Structural Engineer";
        public const String PRESCRIBED_REGISTER_CONTRACTOR = "Prescribed Registered Contractor";
        public const String REGISTERED_GEOTECHNICAL_ENGINEER = "Registered Geotechnical Engineer";

        //Acknowledgement Submission Type
        public const String ACKNOWLEDGEMENT_SUBMISSION_TYPE_COMMENCEMENT = "Commencement";
        public const String ACKNOWLEDGEMENT_SUBMISSION_TYPE_COMPLETION = "Completion";
        public const String ACKNOWLEDGEMENT_SUBMISSION_TYPE_VALIDATION_SCHEME = "Validation Scheme";

        //days for mw general record
        public const int _REPLY_DUE_DAYS = 20;
        public const int INTERIM_REPLY_DUE_DAYS = 10;

        //complaint checklist status
        public const String COMPLAINT_CHECKLIST_DRAFT = "Draft";
        public const String COMPLAINT_CHECKLIST_PO_SUBMIT = "PO Submit";
        public const String COMPLAINT_CHECKLIST_PO_RE_ASSIGN = "PO Re-assign";
        public const String COMPLAINT_CHECKLIST_TO_SUBMIT = "TO Submit";
        public const String COMPLAINT_CHECKLIST_SPO_SUBMIT = "SPO Submit";
        public const String FLOW_FINAL_CONFIRM_SPO = "FLOW_FINAL_CON_SPO";

        //admin outstanding doc delivered by
        public const String DELIVERED_BY_MWU = "MWU";
        public const String DELIVERED_BY_RD = "R&D";

        //	SELECT STATUS_CODE FROM MW_RECORD_ITEM
        //public const String MW_RECORD_ITEM_STATUS_OLD="OLD";
        //public const String MW_RECORD_ITEM_STATUS_NEW="NEW";

        public const String MW_RECORD_ITEM_STATUS_ = "";
        public const string MW_RECORD_ITEM_STATUS_FINAL = "FINAL";
        public const string MW_RECORD_ITEM_STATUS_FINAL_VERSION = "FINAL";


        //	SELECT STATUS_ID FROM MwAppointedProfessional
        public const String MW_APPOINTED_PROFESSIONAL_STATUS_OLD = "OLD";
        public const String MW_APPOINTED_PROFESSIONAL_STATUS_NEW = "NEW";

        //interval between receipt and reply
        public const String LESS_THAN_5_DAYS = "1-5 Day";
        public const String MORE_THAN_5_DAYS = "5-10 Day";
        public const String MORE_THAN_10_DAYS = "10 Days above";

        //enquiry input record count
        public const int ENQUIRY_INPUT_RECORD_COUNT = 120;
        public const int ENQUIRY_INPUT_PAGE_COUNT = 20;

        //MW_RECORD_ITEM_CHECKLIST_ITEM answer
        public const String ANSWER_ERECTION = "Erection";
        public const String ANSWER_ALTERATION = "Alteration";
        public const String ANSWER_REMOVAL = "Removal";

        //audit action
        public const String SYSTEM_ADDED = "System Added";
        public const String NON_ADDED = "Non-Added";
        public const String MANUAL_ADD = "Manual Add";

        public const String DISPLAY_SYSTEM_ADDED = "System Added";
        public const String DISPLAY_NON_ADDED = "Not Pick";
        public const String DISPLAY_MANUAL_ADD = "Manual Pick";

        //audit result
        public const String AUDIT_OK = "OK";
        public const String AUDIT_NOT_OK = "Not OK";

        public const String PERSANTAGE = "%";

        //Rule of Conditional Letter and Refusal
        public const String RULE_OF_CON_LETTER_AND_REF_SMALLER_THAN = "SMALLER_THAN";
        public const String RULE_OF_CON_LETTER_AND_REF_SMALLER_THAN_OR_EQUAL = "SMALLER_THAN_OR_EQUAL";
        public const String RULE_OF_CON_LETTER_AND_REF_NA = "NA";
        public const String RULE_OF_CON_LETTER_AND_REF_LARGER_THAN = "LARGER_THAN";
        public const String RULE_OF_CON_LETTER_AND_REF_LARGER_THAN_OR_EQUAL = "LARGER_THAN_OR_EQUAL";

        // Notification Checking
        public const String NOTIFICATION_VALID = "O";
        public const String NOTIFICATION_CONDITIONAL = "C";
        public const String NOTIFICATION_REFUSAL = "N";

        //checking
        public const String CHECKING_OK = "O";
        public const String CHECKING_NOT_OK = "N";
        public const String CHECKING_NA = "NA";
        public const String NAME_ARE_NOT_IDENTICAL = "Names are not identical";
        public const String CERT_NOT_FOUND = "Certification of Registration No. Not Found!";


        //SUBMISSION TASK TYPE
        public const String SUBMISSION_TASK_ROLLBACK_VERIFICATION = "Rollbacked Verification";
        public const String SUBMISSION_TASK_ROLLBACK_ACKNOWLEDGEMENT = "Rollbacked Acknowledgement";
        public const String SUBMISSION_TASK_RESUBMIT_ACKNOWLEDGEMENT = "Resubmitted Acknowledgement";
        public const String SUBMISSION_TASK_VERIFICATION = "Verification";
        public const String SUBMISSION_TASK_ACKNOWLEDGEMENT = "Acknowledgement";

        //address type
        public const String RV_DEVELOPMENT_NAME = "Development Name";
        public const String RV_BUILDING = "Building";
        public const String RV_BLOCK = "Block";
        public const String RV_UNIT = "Unit";
        public const String RV_TENEMENT = "Tenement";
        public const String RV_STREET_LOCATION = "Street Location";
        public const String RV_STREET_NAME = "Street Name";
        public const String RV_LOCATION_NAME = "Location Name";
        public const String RV_NEW_DISTRICT = "New District";

        //	public const String CRM_FILE_PATH ="E:\\CRM_FILE";

        //PRC role
        public const String GBC = "GBC";
        public const String SCD = "SC(D)";
        public const String SCSF = "SC(SF)";
        public const String SCF = "SC(F)";
        public const String SCGI = "SC(GI)";
        public const String MWC = "MWC";
        public const String MWCP = "MWC(P)";
        public const String MWCW = "MWC(W)";

        // Letter Template Type
        public const String LETTER_TEMPLATE_TYPE_ACKNOWLEDGEMENT = "Acknowledgement";
        public const String LETTER_TEMPLATE_TYPE_CONDITIONAL = "Conditional";
        public const String LETTER_TEMPLATE_TYPE_REFUSE = "Refusal";
        public const String LETTER_TEMPLATE_TYPE_REPLY_LETTER = "Reply Letter";
        public const String LETTER_TEMPLATE_TYPE_INTERIM_REPLY = "Interim Reply";
        public const String LETTER_TEMPLATE_TYPE_AUDIT_REPLY = "Audit Reply";

        public const String LETTER_TEMPLATE_TYPE_D_LETTER = "D-Letter";
        public const String LETTER_TEMPLATE_TYPE_REFERRAL = "Referral";

        public const String LETTER_TEMPLATE_TYPE_IO_NOTIFICATION = "IO Notification";
        public const String LETTER_TEMPLATE_TYPE_REMINDER = "Reminder";
        public const String LETTER_TEMPLATE_TYPE_MEMO = "Memo";
        public const String LETTER_TEMPLATE_TYPE_EMAIL = "Email";
        public const String LETTER_TEMPLATE_TYPE_OTHER = "Other";

        public const String LETTER_NEW = "";
        public const String LETTER_NOT_FOUND = "Not Found";
        public const String LETTER_CREATED = "Created";
        public const String LETTER_SUBMITTED = "Submitted";
        public const String LETTER_FROZEN = "Frozen";
        public const String LETTER_CONFIRMED = "Confirmed";
        public const String LETTER_COMPLETED = "Completed";

        public const String MW_RECORD = "MwRecord";
        public const String MW_GENERAL_RECORD = "MwGeneralRecord";

        //dsn_request table
        public const String ACK_DSN = "ack_dsn";
        public const String AUDIT_DSN = "audit_dsn";

        //address type
        public const String VERIFICATION_ITEM_ADDRESS = "VERIFICATION_ITEM_ADDRESS";

        //Flow status for Enquiry and Complaint
        public const String FLOW_ENTRY = "FLOW_ENTRY";
        public const String FLOW_ENTRY_PO = "FLOW_ENTRY_PO";
        public const String FLOW_SITE_TO = "FLOW_SITE_TO";
        public const String FLOW_SITE_PO = "FLOW_SITE_PO";
        public const String FLOW__CONFIRM_SPO = "FLOW__CON_SPO";
        public const String FLOW_CASE_SPO = "FLOW_CASE_SPO";
        public const String FLOW_CASE_PO = "FLOW_CASE_PO";

        public const String FLOW_DONE = "DONE";

        //audit recommendation list
        public const String REFERRAL_FOR_PROSECUTION = "Referral for prosecution";
        public const String REFERRAL_FOR_DISCIPLINARY_ACTION = "Referral for Disciplinary Action";
        public const String REFERRAL_FOR_ENFORCEMENT = "Referral for Enforcement";
        public const String NO_FURTHER_ACTION_REQUESTED = "No Further Action Requested";

        //audit mw status list
        public const String PENDING_REPLY = "Pending Reply";
        public const String PENDING_INSPECTION = "Pending Inspection";
        public const String COMPLETED = "Completed";
        public const String UNNECESSARY_ACTION_TAKING = "Unnecessary action taking";
        public const String ISSUED_ORDER = "Issued Order";
        public const String ISSUED_NOTICE = "Issued Notice";
        public const String ISSUED_COMPLIANCE_LETTER = "Issued Compliance letter";
        public const String AUDIT_STATUS_OTHERS = "Others";


        // RRM Synchronization Status
        public const String RRM_SYN_READY = "READY";
        public const String RRM_SYN_PENDING_LOAD_DETAILS = "PENDING_LOAD_DETAILS";
        public const String RRM_SYN_PENDING_FILE_SYN = "PENDING_FILE_SYN";
        public const String RRM_SYN_COMPLETE = "COMPLETE";

        public const String NA = "N/A";

        //progress report status
        public const String PROGRESS_STATUS_OPEN = "Open";
        public const String PROGRESS_STATUS_VERTRIFICATION = "Verification in Progress";
        public const String PROGRESS_STATUS_ACKNOWLEDGEMENT = "Acknowledgement in Progress";
        public const String PROGRESS_STATUS_SSP_VERTRIFICATION = "Verification(SSP) in Progress";
        public const String PROGRESS_STATUS_ENDORSED = "Endorsed";

        public const String PROGRESS_STATUS_REFUSAL = "Refusal";
        public const String PROGRESS_STATUS_CONDITIONAL = "Conditional";

        //progress report sort by
        public const String PROGRESS_SORTBY_RECEIVED_DATE = "Received Date";
        public const String PROGRESS_SORTBY_SUBMISSION_NO = "Submission No.";

        //status for record reassigned
        public const String GENERAL_RECORD_UNUSED = "General Record Unused";

        //SSP required
        public const String SSP_NOT_REQUIRED = "Not Required";
        public const String SSP_REQUIRED = "Required";

        public const String TITLE = "Title";

        //security level
        public const String SECURITY_LEVEL = "SECURITY_LEVEL";
        public const String MWU = "MWU";
        public const String SM = "SM";
        public const String BD = "BD";

        //user type
        public const String MWU_HANDLED_USER = "MWU_HANDLED_USER";
        public const String SM_OR_UNHANDLED_USER = "SM_OR_UNHANDLED_USER";
        public const String BD_USER = "BD_USER";
        public const String ADMIN = "ADMIN";
        public const String NEED_TO_KNOW_USER = "NEED_TO_KNOW_USER";

        public const String ACTIVE_USER = "1";
        public const String INACTIVE_USER = "0";

        //from page
        public const String INFORMATION_PAGE = "informationPage";

        //comment action
        public const String COMMENT_ACTION_EDIT = "edit";
        public const String COMMENT_ACTION_VIEW = "view";

        // Generate new S/N
        public const String SCAN_DISPATCH = "Scan and Dispatch";
        public const String SCAN_HOOK = "Scan and Hook";
        public static List<String> getScanDispatchHookList()
        {
            List<String> result = new List<String>();
            result.Add(SCAN_DISPATCH);
            result.Add(SCAN_HOOK);
            return result;
        }

        public static List<String> getMWUOutgoingNewStatusList()
        {
            List<String> result = new List<String>();
            result.Add(MWU_OUTGOING_NEW_DISPATCH);
            result.Add(MWU_OUTGOING_NEW_HOOK);
            result.Add(MWU_OUTGOING_RESENT_DISPATCH);
            result.Add(MWU_OUTGOING_RESENT_HOOK);
            return result;
        }
        public static List<String> getMWUOutgoingDispatchScannedStatusList()
        { // Dispatch type only
            List<String> result = new List<String>();
            result.Add(MWU_OUTGOING_NEW_DISPATCH_SCANNED);
            result.Add(MWU_OUTGOING_RESENT_DISPATCH_SCANNED);
            return result;
        }

        public static List<String> getMWUOutgoingDispatchCountedStatusList()
        { // Dispatch type only
            List<String> result = new List<String>();
            result.Add(MWU_OUTGOING_NEW_DISPATCH_COUNTED);
            result.Add(MWU_OUTGOING_RESENT_DISPATCH_COUNTED);
            return result;
        }

        public static List<String> getMWUOutgoingDispatchDeliveredStatusList()
        { // Dispatch type only
            List<String> result = new List<String>();
            result.Add(MWU_OUTGOING_NEW);
            result.Add(MWU_OUTGOING_RESENT);
            return result;
        }

        public static List<String> getMWURdIncomingStatusList()
        {
            List<String> result = new List<String>();
            result.Add(MWU_RD_INCOMING_NEW);
            result.Add(MWU_RD_INCOMING_SCANNED);
            return result;
        }
        public static DateTime MW_ITEM_VERSTION_2_START_DATE = new DateTime(2012, 4, 1);
        public static List<String> getMwItemVersion2Array()
        {
            List<String> result = new List<String>();
            result.Add("1.17");
            result.Add("2.2");
            result.Add("2.17");
            result.Add("2.30");
            result.Add("3.23");
            return result;
        }
        public static List<String> autoGenMwFormCodeList()
        {
            List<String> result = new List<String>();
            result.Add("MW01");
            result.Add("MW03");
            result.Add("MW05");
            result.Add("MW06");
            result.Add("MW32");
            return result;
        }
        /*public const  List<String> autoGenMwFormCodeList() {
            List<String>
        }*/
        public const String MWU_RECEIVE_COUNT_GENTYPE_FORM = "form";
        public const String MWU_RECEIVE_COUNT_GENTYPE_OTHER = "other";



        public const String ACTIVITY_RECEIPT = "Receipt";
        public const String ACTIVITY_SCAN = "Scan";
        public const String ACTIVITY_FIRST_ENTRY = "First Entry";
        public const String ACTIVITY_SECOND_ENTRY = "SecondEntry";
        public const String ACTIVITY_VERIFICATION_TO = "Verification-TO";
        public const String ACTIVITY_ACKNOWLEDGEMENT_PO = "Acknowledgement-PO";
        public const String ACTIVITY_ACKNOWLEDGEMENT_SPO = "Acknowledgement-SPO";


        public const String MW_NO_PREFIX = "MW_NO_PREFIX";


        public const String MWDSN_SUBMITFLOW_RD_WITH_MW_NO = "RD_WITH_MW_NO";
        public const String SUBMITFLOW_MWU_TO_SECOND = "MWU_TO_SECOND";

        public const String MW_CRM_INFO_STATUS_ACTIVE = "Active";

        public const String S_TYPE_SYSTEM_PATH = "SYSTEM_PATH";
        public const String S_TYPE_IMPORT_STATUS = "IMPORT_STATUS";
        public const String S_TYPE_RRM_REQUEST_STATUS = "RRM_REQUEST_STATUS";
        public const String S_TYPE_IMPORT_STATUS_DESCRIPTION = "IMPORT_STATUS_DESCRIPTION";
        public const String S_TYPE_EXPORT_SEQUENCE_NUMBER = "EXPORT_SEQUENCE_NUMBER";
        public const String S_TYPE_ACL_LETTER_TO_RELATED_DATA = "ACL_LETTER_TO_RELATED_DATA";
        public const String S_TYPE_ACL_LETTER_NO_OF_VS = "ACL_LETTER_NO_OF_VS";

        public const String S_TYPE_PRECOMM_SITE_AUDIT = "S_TYPE_PRECOMM_SITE_AUDIT";

        public const String S_TYPE_DW_LETTER_CHECKBOX = "DW_LETTER_CHECKBOX";

        public const String S_TYPE_DW_LETTER_REMARK3 = "DW_REMARK3";

        public const String S_TYPE_ACK_LETTER_CHECKBOX = "ACK_LETTER_CHECKBOX";

        public const String S_TYPE_SDM_SUBMISSION = "SDM_SUBMISSION";
        public const String SDM_REPORT_TIME_LOG_CODE = "SDM_REPORT_TIME_LOG";

        public const String S_TYPE_SDM_REPORT = "SDM_REPORT";

        public const String S_TYPE_SDM_NO_OF_VALIDATE = "NO_OF_VALIDATE";

        public const String S_TYPE_ACK_LETTER_TEMPLETE = "ACK_LETTER_TEMPLETE";
        public const String S_TYPE_MOD_LETTER_TEMPLETE = "MOD_LETTER_TEMPLETE";
        public const String S_CODE_ACK_TEMPLATE_ENG_SPO_NAME = "ENG_SPO_NAME";
        public const String S_CODE_ACK_TEMPLATE_CHI_SPO_NAME = "CHT_SPO_NAME";
        public const String S_CODE_ACK_TEMPLATE_ENG_SPO_POSITION = "ENG_SPO_POSITION";
        public const String S_CODE_ACK_TEMPLATE_CHI_SPO_POSITION = "CHT_SPO_POSITION";

        public const String S_VAL_IS_ACTIVE_N = "N";
        public const String S_VAL_IS_ACTIVE_Y = "Y";

        public const String S_VAL_PRECOMM_SITE_AUDIT_DAYBACK = "DAY_BACK";

        public const String S_VAL_TEMPLATE_PATH = "TEMPLATE_PATH";
        public const String S_VAL_TEMPLATE_36_PATH = "TEMPLATE_36_PATH";
        public const String S_VAL_IMPORTED = "IMPORTED";
        public const String S_VAL_UPLOADED = "UPLOADED";
        public const String S_VAL_IMPORT_PATH = "IMPORT_PATH";
        public const String S_VAL_IMPORT_VALID = "IMPORT_VALID";
        public const String S_VAL_UPLOAD_VALID = "UPLOAD_VALID";

        public const String S_VAL_IMPORT_INVALID = "IMPORT_INVALID";






        public const String S_TYPE_RECORD_STATUS = "RECORD_STATUS";
        public const String S_VAL_SUBMIT = "SUBMIT";
        public const String S_VAL_DRAFT = "DRAFT";



        public const String IMPORT_CHI_LANGUAGE = "C";

        public const String SYSTEM_USER = "SYSTEM";



        public const String MW_UPLOAD_STATUS_UPLOADED = "UPLOADED";
        public const String MW_UPLOAD_STATUS_IMPORTED = "IMPORTED";








        public const String MW_UPLOAD_CASE_BATCH_UPLOAD_ONLY = "Batch Upload Only";
        public const String MW_UPLOAD_CASE_ADULT_CASE = "Adult Case";
        public const String MW_UPLOAD_CASE_NON_ADULT_CASE = "Non Adult Case";

        public const String MW_UPLOAD_STATUS_ERROR = "error";
        public const String MW_UPLOAD_STATUS_SUCCESS = "success";


        public const String MW_UPLOAD_ITEM_STATUS_ERROR = "error";
        public const String MW_UPLOAD_ITEM_STATUS_SUCCESS = "success";

        public const String MW_UPLOAD_ITEM_STATUS_DESC_DSN_NOT_EXIST = "DSN not exist";
        public const String MW_UPLOAD_ITEM_STATUS_DESC_SUB_DSN_EXIST = "Sub DSN alrady exist";

        public const String RELATED_CASE_ALL = "All";
        public const String RELATED_CASE_MWLIST = "MW List";
        public const String RELATED_CASE_AUDIT = "Audit";
        public const String RELATED_CASE_SIGNBOARD = "Signboard";
        public const String RELATED_CASE_ORDER = "Order";

        public const String LANGUAGE_EN = "English";
        public const String LANGUAGE_CHT = "中文";
        public const String LANGUAGE_EN_CODE = "E";
        public const String LANGUAGE_CHT_CODE = "C";

        public const String LANGUAGE_RADIO_CHINESE = "CHT";
        public const String LANGUAGE_RADIO_ENGLISH = "ENG";

        public const String DOC_TYPE_PDF = "PDF";
        public const String DOC_TYPE_DOCX = "DOCX";

        public const String GENERAL_SEARCH_COUNTER_KWUN_TONG = "Kwun Tong";
        public const String GENERAL_SEARCH_COUNTER_PIONEER_CENTRE = "Pioneer Centre";
        public const String GENERAL_SEARCH_COUNTER_E_SUBMISSION = "E-Submission";
        public const String GENERAL_SEARCH_COUNTER_WKGO = "WKGO";
        public const String GENERAL_SEARCH_COUNTER_ALL = "All";
        public const String GENERAL_SEARCH_COUNTER_KWUN_TONG_CODE = "1";
        public const String GENERAL_SEARCH_COUNTER_PIONEER_CENTRE_CODE = "2";
        public const String GENERAL_SEARCH_COUNTER_E_SUBMISSION_CODE = "3";
        public const String GENERAL_SEARCH_COUNTER_WKGO_CODE = "4";
        public const String GENERAL_SEARCH_COUNTER_ALL_CODE = "";

        public const String GENERAL_SEARCH_NATURE_SUBMISSION = "Submission";
        public const String GENERAL_SEARCH_NATURE_ESUBMISSION = "E-Submission";
        public const String GENERAL_SEARCH_NATURE_DIRECT_RETURN = "Direct Return";
        public const String GENERAL_SEARCH_NATURE_WITHDRAWAL = "Withdrawal";
        public const String GENERAL_SEARCH_NATURE_ICU = "ICU";
        public const String GENERAL_SEARCH_NATURE_ALL = "All";

        public const String GENERAL_SEARCH_NATURE_SUBMISSION_CODE = "SUBMISSION";
        public const String GENERAL_SEARCH_NATURE_ESUBMISSION_CODE = "E-SUBMISSION";
        public const String GENERAL_SEARCH_NATURE_DIRECT_RETURN_CODE = "DIRECT RETURN";
        public const String GENERAL_SEARCH_NATURE_WITHDRAWAL_CODE = "WITHDRAWAL";
        public const String GENERAL_SEARCH_NATURE_ICU_CODE = "ICU";
        public const String GENERAL_SEARCH_NATURE_ALL_CODE = "";
        public const int RELATED_CASE_PAGE_SIZE = 10;

        public const String RELATED_Y = "Y";
        public const String RELATED_N = "N";
        public const String RELATED_ALL = "All";

        public const String SUBMISSION_VALUE_COUNTER_SLABEL_KT = "KT";
        public const String SUBMISSION_VALUE_COUNTER_SLABEL_PC = "PC";
        public const String SUBMISSION_VALUE_COUNTER_SLABEL_ES = "ES";

        public const String SUBMISSION_VALUE_COUNTER_LABEL_KT = "Kwun Tong";
        public const String SUBMISSION_VALUE_COUNTER_LABEL_PC = "Pioneer Centre";
        public const String SUBMISSION_VALUE_COUNTER_LABEL_ES = "E-submission";

        public const String SUBMISSION_VALUE_COUNTER_VALUE_KT = "1";
        public const String SUBMISSION_VALUE_COUNTER_VALUE_PC = "2";
        public const String SUBMISSION_VALUE_COUNTER_VALUE_ES = "3";
        public const String SUBMISSION_VALUE_COUNTER_VALUE_WKGO = "4";

        public const String ACK_LETTER_AUDIT_PERCENTAGE = "ACK_LETTER_AUDIT_PERCENTAGE";
        public const String ACK_LETTER_AUDIT_PERCENTAGE_MW01 = "ACK_LETTER_AUDIT_PERCENTAGE_MW01";
        public const String ACK_LETTER_AUDIT_PERCENTAGE_MW03 = "ACK_LETTER_AUDIT_PERCENTAGE_MW03";
        public const String ACK_LETTER_AUDIT_PERCENTAGE_MW05 = "ACK_LETTER_AUDIT_PERCENTAGE_MW05";
        public const String ACK_LETTER_AUDIT_PERCENTAGE_MW06 = "ACK_LETTER_AUDIT_PERCENTAGE_MW06";


        public const String PRECOMM_SITE_AUDIT_RESULT_NOT_YET = "Works not yet commenced";
        public const String PRECOMM_SITE_AUDIT_RESULT_ALREADY = " Works commenced already";
        public const String PRECOMM_SITE_AUDIT_RESULT_INACCESSIBLE = "Inaccessible";
        public const String PRECOMM_SITE_AUDIT_RESULT_NOT_YET_INSPECTED = "Not yet inspected";

        public const String ACK_LETTER_TITLE_MW_LIST = "MW List";
        public const String ACK_LETTER_TITLE_ORDER = "Order Related Cases";
        public const String ACK_LETTER_TITLE_SIGNBOARD = "Signboard";


        public const String CUSTOM_DELIMITER = "CustomDSplit";
        public const String S_VAL_DATA = "DATA";
        public const String S_VAL_VS_PERIOD_2014 = "2014";
        public const String S_VAL_VS_PERIOD_2015 = "2015";
        public const String S_VAL_VS_PERIOD_2016 = "2016";
        public const String S_VAL_VS_PERIOD_2011 = "2011";

        public const String SEARCH_ACTION_COLUMN = "SearchColumn";
        public const String SEARCH_ACTION_COUNT = "Count";
        public const String SEARCH_ACTION_EXPORT = "ExportSearch";

        public const String EXPORT_END_FLAG = "[HEADER_END]";
        public const String EXPORT_MESSAGE_EMPTY = "Result not found for export.";


        public const String STATISTICS_FREEZE_TYPE_MW_SUBMISSION = "MW_S";

        public const String STATISTICS_FREEZE_TYPE_MW_RECEIVED = "MW_R";
        public const String STATISTICS_FREEZE_TYPE_MW_ACK_COUNTER = "MW_ACK_C";
        public const String STATISTICS_FREEZE_TYPE_MW_ACK_FAX = "MW_ACK_F";
        public const String STATISTICS_FREEZE_TYPE_MW_RETURN_FAX = "MW_RE_F";
        public const String STATISTICS_FREEZE_TYPE_MW_RETURN_COUNTER = "MW_RE_C";

        public const String STATISTICS_FREEZE_TYPE_MWIS_SUBMISSIONS = "MW_MWIS_S";

        public const String STATISTICS_FREEZE_TYPE_MW_TOTAL_NO_OF_SUBMISSION = "MW_MW_TNOS";

        public const String STATISTICS_FREEZE_TYPE_MW_NOTICE = "MW_N";

        public const String STATISTICS_FREEZE_TYPE_MW_CLASS_I_RATIO = "MW_CLASS_1_R";
        public const String STATISTICS_FREEZE_TYPE_MW_CLASS_II_RATIO = "MW_CLASS_2_R";
        public const String STATISTICS_FREEZE_TYPE_MW_CLASS_III_RATIO = "MW_CLASS_3_R";

        public const String STATISTICS_FREEZE_TYPE_MW_VALIAD_SCHEME = "MW_VAL_SCH";

        public const String STATISTICS_FREEZE_TYPE_MW_ITEMS_WINDOWS_2013 = "MW_ITEMS_WIN_2013";
        public const String STATISTICS_FREEZE_TYPE_MW_ITEMS_RENDERING_2013 = "MW_ITEMS_REND_2013";
        public const String STATISTICS_FREEZE_TYPE_MW_ITEMS_REPAIR_2013 = "MW_ITEMS_REP_2013";
        public const String STATISTICS_FREEZE_TYPE_MW_ITEMS_ABOVEGROUND_2013 = "MW_ITEMS_ABG_2013";
        public const String STATISTICS_FREEZE_TYPE_MW_ITEMS_ACSUPPORTING_2013 = "MW_ITEMS_ACSUPP_2013";
        public const String STATISTICS_FREEZE_TYPE_MW_ITEMS_DRY_BACK_2013 = "MW_ITEMS_DRY_B_2013";
        public const String STATISTICS_FREEZE_TYPE_MW_ITEMS_CANOPY_2013 = "MW_ITEMS_CAN_2013";
        public const String STATISTICS_FREEZE_TYPE_MW_ITEMS_SDF_2013 = "MW_ITEMS_SDF_2013";
        public const String STATISTICS_FREEZE_TYPE_MW_ITEMS_SIGN_BOARD_2013 = "MW_ITEMS_SIGN_B_2013";

        public const String STATISTICS_FREEZE_TYPE_MW_ITEMS_WINDOWS_PREVIOUS = "MW_ITEMS_WIN_P";
        public const String STATISTICS_FREEZE_TYPE_MW_ITEMS_RENDERING_PREVIOUS = "MW_ITEMS_REND_P";
        public const String STATISTICS_FREEZE_TYPE_MW_ITEMS_REPAIR_PREVIOUS = "MW_ITEMS_REP_P";
        public const String STATISTICS_FREEZE_TYPE_MW_ITEMS_ABOVEGROUND_PREVIOUS = "MW_ITEMS_ABG_P";
        public const String STATISTICS_FREEZE_TYPE_MW_ITEMS_ACSUPPORTING_PREVIOUS = "MW_ITEMS_ACSUPP_P";
        public const String STATISTICS_FREEZE_TYPE_MW_ITEMS_DRY_BACK_PREVIOUS = "MW_ITEMS_DRY_B_P";
        public const String STATISTICS_FREEZE_TYPE_MW_ITEMS_CANOPY_PREVIOUS = "MW_ITEMS_CAN_P";
        public const String STATISTICS_FREEZE_TYPE_MW_ITEMS_SDF_PREVIOUS = "MW_ITEMS_SDF_P";
        public const String STATISTICS_FREEZE_TYPE_MW_ITEMS_SIGN_BOARD_PREVIOUS = "MW_ITEMS_SIGN_B_P";

        public const String STATISTICS_FREEZE_TYPE_MW_ITEMS_WINDOWS = "MW_ITEMS_WIN";
        public const String STATISTICS_FREEZE_TYPE_MW_ITEMS_RENDERING = "MW_ITEMS_REND";
        public const String STATISTICS_FREEZE_TYPE_MW_ITEMS_REPAIR = "MW_ITEMS_REP";
        public const String STATISTICS_FREEZE_TYPE_MW_ITEMS_ABOVEGROUND = "MW_ITEMS_ABG";
        public const String STATISTICS_FREEZE_TYPE_MW_ITEMS_ACSUPPORTING = "MW_ITEMS_ACSUPP";
        public const String STATISTICS_FREEZE_TYPE_MW_ITEMS_DRY_BACK = "MW_ITEMS_DRY_B";
        public const String STATISTICS_FREEZE_TYPE_MW_ITEMS_CANOPY = "MW_ITEMS_CAN";
        public const String STATISTICS_FREEZE_TYPE_MW_ITEMS_SDF = "MW_ITEMS_SDF";
        public const String STATISTICS_FREEZE_TYPE_MW_ITEMS_SIGN_BOARD = "MW_ITEMS_SIGN_B";

        public const String STATISTICS_FREEZE_TYPE_MW_BARCODE_FROM_KT = "MW_BARCODE_KT";
        public const String STATISTICS_FREEZE_TYPE_MW_BARCODE_FROM_PC = "MW_BARCODE_PC";
        public const String STATISTICS_FREEZE_TYPE_MW_BARCODE_FROM_EC = "MW_BARCODE_EC";

        public const String STATISTICS_FREEZE_TYPE_MW_NON_BARCODE_FROM_KT = "MW_N_BARCODE_KT";
        public const String STATISTICS_FREEZE_TYPE_MW_NON_BARCODE_FROM_PC = "MW_N_BARCODE_PC";
        public const String STATISTICS_FREEZE_TYPE_MW_NON_BARCODE_FROM_EC = "MW_N_BARCODE_EC";

        public const String STATISTICS_FREEZE_TYPE_MW_E_FORM_SUBMISSION_PERCENTAGE = "MW_E_FORM_PERC";

        public static DateTime MW_REQUIREMENT_CUT_OFF_DAY = new DateTime(2015, 12, 10); //yymmdd
        public const String SELECTION_OF_REQUIREMENT_OLD = "OLD";
        public const String SELECTION_OF_REQUIREMENT_NEW = "NEW";

        public const String DW_LETTER_ITEM_LETTER_TYPE = "DW";
        public const String AL_LETTER_ITEM_LETTER_TYPE = "AL";

        public const String BUILDING_WORKS = "BUILDING_WORKS";
        public const String STREET_WORKS = "STREET_WORKS";
        public const String BUILDING_WORKS_VALUE = "BUILDING";
        public const String STREET_WORKS_VALUE = "STREET";

        // Categorisation of irregularities
        public const String REQUIRE_SITE_RECT = "REQUIRE_SITE_RECTIFICATION";
        public const String RECOMMENDATION_NOT_IN_ACKNOWLEDGEMENT_AND_NOT_REQUIRE_SITR_RECTIFICATION = "RECOMMENDATION_NOT_IN_ACKNOWLEDGEMENT_AND_NOT_REQUIRE_SITR_RECTIFICATION";
        public const String RECOMMENDATION_MARK_AS_ACKNOWLEDGEMENT = "RECOMMENDATION_MARK_AS_ACKNOWLEDGEMENT";
        public const String WITHDRAWN_WITHOUT_IRREGULARITIES = "WITHDRAWN_WITHOUT_IRREGULARITIES";

        // Begin add by Chester 

        // LetterModule Acknowledgement

        public const string ACKNOWLEDGEMENT_LETTER_TEMPLATE_PATH = "~/Template/AckLetterTemplate/";
        public const string MODIFICATION_LETTER_TEMPLATE_PATH = "~/Template/Modification/";
        public const string ADVISORY_LETTER_TEMPLATE_PATH = "~/Template/ALTemplate/";

        //Direct Return
        public const string DIRECT_RETURN_LETTER_TEMPLATE_PATH = "~/Template/DirectReturn/";

        //public const string ACKNOWLEDGEMENT_PRINT_FILENAME = "Acknowledgement";
        public const string ACKNOWLEDGEMENT_LETTER_ENG_CONTACT = "By Fax Only :  ";
        public const string ACKNOWLEDGEMENT_LETTER_CHI_CONTACT = "只發傳真 :  ";
        public const string ACKNOWLEDGEMENT_LETTER_ENG_FONTFAMILY = "Times New Roman";
        public const string ACKNOWLEDGEMENT_LETTER_CHI_FONTFAMILY = "PMingLiU";

        public const string DISPLAY_REQUIRE_SITE_RECT = "Involve irregularities that require site rectification";
        public const string DISPLAY_RECOMMENDATION_NOT_IN_ACKNOWLEDGEMENT_AND_NOT_REQUIRE_SITR_RECTIFICATION = "Only involve irregularities that does not require site rectification";
        public const string DISPLAY_RECOMMENDATION_MARK_AS_ACKNOWLEDGEMENT = "Acknowledgement";
        public const string DISPLAY_WITHDRAWN_WITHOUT_IRREGULARITIES = "Withdraw";

        // Submission Location Report SortBy
        public const string SLR_TASK_DATE_TIME_VAL = "TASK_DATE,TASK_TIME";
        public const string SLR_MW_DSN_VAL = "R.MW_DSN";
        public const string SLR_REF_NO_VAL = "RN.REFERENCE_NO";
        public const string SLR_ACTIVITY_VAL = "TASK_CODE";

        public const string SLR_TASK_DATE_TIME_TEXT = "Date/Time";
        public const string SLR_MW_DSN_TEXT = "Document S/N";
        public const string SLR_REF_NO_TEXT = "Ref No.";
        public const string SLR_ACTIVITY_TEXT = "Activity";
        // End add by Chester

        //Verfication Summary Active Date
        public const string ACTIVE_DATE = "01/01/2019";

        //Fn10RPT_MWMSWCCModel 
        public const string CHECKBOX_ACKNOWLEDGED = "Acknowledged";
        public const string CHECKBOX_NOSUBMISSION = "No Submission";
        public const string CHECKBOX_PROCESSING = "Processing";
        public const string CHECKBOX_REFUSED = "Refused";

        public const string EFSS_STATUS_ACK = "ACK";
        public const string EFSS_STATUS_DIRECT_RETURN = "R";

        public const string EFSS_SUBMISSION_MAP_STATUS_ACK = "ACK";
        public const string EFSS_SUBMISSION_MAP_STATUS_DIRECT_RETURN = "DR";

        //TDL Acknowledgement LSS_EBD
        public const string EBD = "EBD";
        public const string DISCIPLINARY = "DISCIPLINARY";

        // RPT_PPJL
        public const string RPT_STATUS_IN_PROGRESS = "In Progress";
        public const string RPT_STATUS_COMPLETED = "Completed";


        public const string WF_STATUS_OPEN = "WF_STATUS_OPEN";
        public const string WF_GO_TASK_END = "WF_GO_TASK_END";

        //Unit
        public const string UNIT_SU = "SU";
        public const string UNIT_MW = "MW";

        //Email Status
        public const string EMAIL_STATUS_READY = "Ready";
        public const string EMAIL_STATUS_TRANSMITTED = "Transmitted";

        //ACK Previous MW_No Error Message
        public const string LM_ACK_PREVIOUS_MWNO_ERROR_MSG = "The MW Form does not exist. Please refill the MW Form No.";
        public const string LM_ACK_COMP_DATE_ERROR_MSG = "CompleteDate Error";
    }
}


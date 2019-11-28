using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Constant
{
    public class RegistrationConstant
    {
        // CRM Registration form
        public const String FORM_BA26 = "BA26";
        public const String FORM_BA26A = "BA26A";
        public const String FORM_BA26B = "BA26B";
        public const String FORM_BA26C = "BA26C";
        public const String FORM_BA26D = "BA26D";

        // Generate certificate 
        public const String COLON = ":";
        public const String ARROW = "->";

        // Cert status
        public const String COMP_APPLICANT_INFO_DETAIL_STATUS_APPLY = "APPLY";
	    public const String COMP_APPLICANT_INFO_DETAIL_STATUS_APPROVED = "APPROVED";

	    public const String IND_ITEM_DETAIL_STATUS_APPLY = "APPLY";
	    public const String IND_ITEM_DETAIL_STATUS_APPROVED = "APPROVED";

        //Category Code
        public const String S_CATEGORY_CODE_AP_A = "AP(A)";
        public const String S_CATEGORY_CODE_AP_E = "AP(E)";
        public const String S_CATEGORY_CODE_AP_S = "AP(S)";
        public const String S_CATEGORY_CODE_AP_I = "API";
        public const String S_CATEGORY_CODE_AP_II = "APII";
        public const String S_CATEGORY_CODE_AP_III = "APIII";


        public const String S_CATEGORY_GROUP_RI = "RI";
        public const String S_CATEGORY_CODE_RI_A = "RI(A)";
	    public const String S_CATEGORY_CODE_RI_E = "RI(E)";
	    public const String S_CATEGORY_CODE_RI_S = "RI(S)";	
	    public const String S_CATEGORY_CODE_MWC = "MWC";
	    public const String S_CATEGORY_CODE_MWC_P = "MWC(P)";
	    public const String S_CATEGORY_CODE_MWC_W = "MWC(W)";
	    public const String S_CATEGORY_CODE_RSE = "RSE";
	    public const String S_CATEGORY_CODE_RGE = "RGE";
        public const String S_CATEGORY_CODE_GBC = "GBC";

        public static String[] MW_TYPE = {"Type A","Type B","Type C","Type D","Type E","Type F","Type G"};
        public static String[] MW_TYPE_DESCRIPTION = {
                        "A (Alteration and Addition Works)(改動及加建工程)",
                        "B (Repair Works)(修葺工程)",
                        "C (Works relating to Signboards)(關乎招牌的工程)",
                        "D (Drainage Works)(排水工程)",
                        "E (Works relating to Structures for Amenities)(關乎適意設施的工程) ",
                        "F (Finishes Works)(飾面工程)",
                        "G (Demolition Works)(拆卸工程)"
                        };

        public const String MW_CLASS_I = "Class 1";
	    public const String MW_CLASS_II = "Class 2";
	    public const String MW_CLASS_III = "Class 3";


        public const string QPCARD = "QPCARD_";
        public const string QP_PREFIX_A = "A";
	    public const string QP_PREFIX_B = "B";
	    public const string QP_PREFIX_C = "C";
	    public const string QP_PREFIX_D = "D";

        // Status Code
        public const String STATUS_ACTIVE = "1";
	    public const String STATUS_APPLICATION_IN_PROGRESS = "2";
	    public const String STATUS_DOCUMENT_MISSING = "3";
	    public const String STATUS_CERTIFICATE_PREPARED = "4";
	    public const String STATUS_SCHEDULED_FOR_INTERVIEW = "5";
	    public const String STATUS_REMOVAL_LETTER_PREPARED = "6";
	    public const String STATUS_REMOVED = "7";
	    public const String STATUS_INACTIVE = "8";
	    public const String STATUS_WITHDRAWN = "9";
	    public const String BUSINESS_CLOSED = "10";


        // for update application status
        public const string CERT = "CERT";
        public const string LETTER = "LETTER";
        public const string LETTER_WITH_CODE = "LETTER_WITH_CODE";
        public const string QP_CARD = "QP_CARD";
        
        public const int NEW_REG = 1;
        public const int RETENTION = 2;
        public const int RESTORATION = 3;
        public const int RE_REG = 4;
        public const int REMOVOAL = 5;
        public const int NEW_REG_CERT = 6;
        public const int RETENTION_CERT = 7;
        public const int RESTORATION_CERT = 8;

        // certificate
        public const string CERT_RPT0011A = "RPT0011A"; // Certificate
        public const string CERT_RPT0011B = "RPT0011B"; // Certificate
        public const string LETTER_MMD0005A = "MMD0005A"; // Letter: Application for Inclusion
        public const string LETTER_MMD0005B = "MMD0005B"; // Letter: Application for Inclusion
        public const string NOTICE_RPT0010B = "RPT0010B"; // Print Notice
        public const string LETTER_MMD0008A = "MMD0008A"; // Letter: Retention of Registration in the Register
        public const string LETTER_MMD0008B_1 = "MMD0008B_1"; // Letter: Retention of Registration in the Register
        public const string LETTER_MMD0008B_2 = "MMD0008B_2"; // Letter: Retention of Registration in the Register
        public const string FAX_TEMPLATE_RETENTION = "FAX_TEMPLATE_RETENTION"; // Fax Template Retention
        public const string FAX_TEMPLATE_RESTORATION = "FAX_TEMPLATE_RESTORATION"; // Fax Template Restoration
        public const string LETTER_MMD0009A = "MMD0009A";
        public const string LETTER_MMD0009B = "MMD0009B"; // Letter: Restoration of Registration in the Register
        public const string LETTER_MMD0007A_1 = "MMD0007A_1"; // Letter: Removal of Contractor in the Register
        public const string LETTER_MMD0007B_1 = "MMD0007B_1"; // Letter: Removal of Contractor in the Register
        public const string LETTER_MMD0007B_2 = "MMD0007B_2"; // Memo: Memo of removal of Contractor in the Register
        public const string LETTER_MMD0007C_1 = "MMD0007C_1"; // Letter: Removal of Contractor in the Register




        // applicant gender:
        public const string GENDER_M = "M";
        public const string GENDER_F = "F";
        //Item
        public const string ITEM_NULL = "0";
        public const string ITEM_CHEQUE = "1";
        public const string ITEM_PRC = "2";
        public const string ITEM_OTHERS = "3";
        public const string ITEM_FORM = "4";


        public const string MWITEM_APPROVED = "APPROVED";
        public const string MWITEM_APPLY = "APPLY";
        //Application Status
        public const string APPLICATION_STATUS_CONFIRMED = "Confirmed";
	    public const string APPLICATION_STATUS_WITHDRAW = "Withdrawn";
	    public const string APPLICATION_STATUS_REFUSE = "Refused";
	    public const string APPLICATION_STATUS_APPLICANTION_IN_PROGRESS = "Application in progress";
        // -- types of registration
        public const string REGISTRATION_TYPE_CGA = "CGC";
        public const string REGISTRATION_TYPE_IP = "IP";
        public const string REGISTRATION_TYPE_MWIA = "IMW";
        public const string REGISTRATION_TYPE_MWCA = "CMW";
        public const string REGISTRATION_TYPE_MW = "MW"; //new add
       
        public const string SYSTEM_TYPE_TITLE = "TITLE";
        public const string SYSTEM_TYPE_APPLICANT_ROLE = "APPLICANT_ROLE";
        public const string SYSTEM_TYPE_QUALIFICATION_TYPE = "QUALIFICATION_TYPE";
        public const string SYSTEM_TYPE_PERIOD_OF_VALIDITY = "PERIOD_OF_VALIDITY";
        public const string SYSTEM_TYPE_COMPANY_TYPE = "COMPANY_TYPE";
        public const string SYSTEM_TYPE_APPLICANT_STATUS = "APPLICANT_STATUS";
        public const string SYSTEM_TYPE_APPLICATION_FORM = "APPLICATION_FORM";
        public const string SYSTEM_TYPE_APPROVED_BY_LIST = "APPROVED_BY_LIST";
        public const string SYSTEM_TYPE_BUILDING_SAFETY_CODE = "BUILDING_SAFETY_CODE";
        public const string SYSTEM_TYPE_PRACTICE_NOTE = "PRACTICE_NOTES";
        public const string SYSTEM_TYPE_PROF_REGISTRATION_BOARD = "PROF_REGISTRATION_BOARD";
        public const string SYSTEM_TYPE_CONVICTION_SOURCE = "CONVICTION_SOURCE";
        public const string SYSTEM_TYPE_MWCLASS = "MINOR_WORKS_CLASS";
        public const string SYSTEM_TYPE_MWTYPE = "MINOR_WORKS_TYPE";
        public const string SYSTEM_TYPE_MWITEM = "MINOR_WORKS_ITEM";
        public const string SYSTEM_TYPE_QPSERVICES = "WILLINGNESS_DROPDOWN";
        public const string SYSTEM_TYPE_FSS_DROPDOWN = "FSS_DROPDOWN";
        public const string SYSTEM_TYPE_REGISTRATION_TYPE = "REGISTRATION_TYPE";
        public const string SYSTEM_TYPE_REGION_CODE = "REGION_CODE";
        public const string SYSTEM_TYPE_TIME_SESSION = "TIME_SESSION";
        public const string SYSTEM_TYPE_PANEL_TYPE = "PANEL_TYPE";
        public const string SYSTEM_TYPE_COMMITTEE_TYPE = "COMMITTEE_TYPE";
        public const string SYSTEM_TYPE_SOCIETY_NAME = "SOCIETY_NAME";
        public const string SYSTEM_TYPE_COMMITTEE_MEMBER_STATUS = "COMMITTEE_MEMBER_STATUS";
        public const string SYSTEM_TYPE_PANEL_ROLE = "PANEL_ROLE";
        public const string SYSTEM_TYPE_COMP_CATEGORY_CODE = "COMP_CATEGORY_CODE";
        public const string SYSTEM_TYPE_ROLE_AS_TD_OO = "ROLE_AS_TD_OO";
        public const string SYSTEM_TYPE_AS_TD_OO_STATUS = "AS_TD_OO_STATUS";
        public const string SYSTEM_TYPE_MWIS_DROPDOWN = "MWIS_DROPDOWN";
        public const string SYSTEM_TYPE_NON_BUILDING_WORKS_RELATED = "NON_BUILDING_WORKS_RELATED";
        public const string SYSTEM_TYPE_HTML_NOTES = "HTML_NOTES";
        public static Dictionary<string, string> DATE_TYPE_MAP = new Dictionary<string, string>()
        {
            {/*"applicationDate" */"1",  "APPLICATION_DATE" } ,
            {/*"registrationDate"*/"2",  "REGISTRATION_DATE"} ,
            {/*"gazetteDate"     */"3",  "GAZETTE_DATE"     } ,
            {/*"expiryDate"      */"4",  "EXPIRY_DATE"      } ,
            {/*"removalDate"     */"5",  "REMOVAL_DATE"     } ,
            {/*"retentionDate"   */"6",  "RETENTION_DATE"   } ,
            {/*"restoreDate"     */"7",  "RESTORE_DATE"     } ,
            {/*"approvalDate"    */"8",  "APPROVAL_DATE"    }
        };
        //PM
        public const string SYSTEM_TYPE_VETTING_OFFICER = "VETTING_OFFICER";
        public const string SYSTEM_TYPE_INTERVIEW_RESULT = "INTERVIEW_RESULT";
        public const string SYSTEM_TYPE_SECRETARY = "SECRETARY";
        public const string SYSTEM_TYPE_ASSISTANT = "ASSISTANT";
        public const string SYSTEM_TYPE_APPLICATION_FORM_CGC_PROC_MON = "SYSTEM_TYPE_APPLICATION_FORM_CGC_PROC_MON";
        public const string SYSTEM_TYPE_COMMITTEE_ROLE = "COMMITTEE_ROLE";
        public const string SYSTEM_TYPE_CATEGORY_GROUP = "CATEGORY_GROUP";

        //AS
        public const string SYSTEM_TYPE_AS_DROPDOWN = "AS_DROPDOWN";
        public const string SYSTEM_TYPE_AS_CLASS_DROPDOWN = "AS_CLASS_DROPDOWN";


        public const string PROCESS_MONITOR_UPM = "UPM";
        public const string PROCESS_MONITOR_FASK_TRACK = "FaskTrack";
        public const string PROCESS_MONITOR_UPM_10DAYS = "UPM_10DAYS";


        public const string ALL_TYPE = "ALL";


        public const string VARIABLE_NULL = "null";

        public const string AUTHORITY_AUTHORITY_NAME = "AUTHORITY_NAME";
        public const string CATEGORY_CODE = "CATEGORY_CODE";
        public const string CATEGORY_CODE_FOR_CGC_PEPORT = "CATEGORY_CODE_FOR_CGC_PEPORT";
        public const string BUILDING_SAFETY_CODE_ITEM = "BUILDING_SAFETY_CODE_ITEM";
        public const string HTML_NOTES = "HTML_NOTES";
        public const string ROOM_DETAILS = "ROOM_DETAILS";
        public const string CATEGORY_CODE_DETAILS = "CATEGORY_CODE_DETAILS";
        public const string PUBLIC_HOLIDAY_MANAGEMENT = "PUBLIC_HOLIDAY_MANAGEMENT";
        public const string MINOR_WORKS_TYPE = "MINOR_WORKS_TYPE";
        public const string MINOR_WORKS_ITEM = "MINOR_WORKS_ITEM";
        public const string LETTER_TEMPLATE = "LETTER_TEMPLATE";


        //AS
        //AS Database	
        public const string CAT_ALL = "CAT_ALL";
	    public const string CAT_GBC = "GBC";
	    public const string CAT_SC_D = "SC(D)";
	    public const string CAT_SC_F = "SC(F)";
	    public const string CAT_SC_GI = "SC(GI)";
	    public const string CAT_SC_SF = "SC(SF)";
	    public const string CAT_SC_V = "SC(V)";
	    public const string CAT_MWC_CLASS_I_II_III = "CAT_MWC_CLASS_I_II_III";
	    public const string CAT_MWC_CLASS_II_III = "CAT_MWC_CLASS_II_III";
	    public const string CAT_MWC_CLASS_III = "CAT_MWC_CLASS_III";
	    public const string CAT_MWC = "MWC";
	    public const string FUNCTION_SEARCH_AUTHORIZED_SIGNATORY = "900";
	    public const string CONSET_TO_PUBLISH = "Consent To Publish";
	    public const string REFUSED_TO_PUBLISH = "Refused To Publish";
	    public const string NOT_INDICATED = "Not Indicated";


        public const string LEAVEFORM_PATH = "LeaveForm";
        public const string WARNINGLETTER_PATH = "docs\\";
        public const string ASFORM_PATH = "ASForm";
        public const string SIGNATURE_PATH = "registration\\docs\\";

        //exportLetter
        public const string LETTER_REMARK_APPROVED_LETTER = "APPROVEDLETTER";
        public const string BATCHUPLOAD_PATH = "BatchUpload";

        //Auth Right - CGC
        public const string CGC_ContractorName = "TYPE1";
        public const string CGC_AddrContact = "TYPE2";
        public const string CGC_BRNo = "TYPE3";
        public const string CGC_LeaveRecords = "TYPE4";
        public const string CGC_ASTDOO = "TYPE5";
        public const string CGC_AppliStatus = "TYPE6";
        public const string CGC_HKIDPASSPORT = "TYPE7";
        public const string CGC_NA = "TYPE8";
        public const string CGC_SpecimenSignature = "TYPE9";
        public const string CGC_NonSpecimenSignature = "TYPE10";

        //Auth Right - CMW
        public const string CMW_ContractorName = "TYPE1";
        public const string CMW_AddrContact = "TYPE2";
        public const string CMW_BRNo = "TYPE3";
        public const string CMW_LeaveRecords = "TYPE4";
        public const string CMW_ASTDOO = "TYPE5";
        public const string CMW_AppliStatus = "TYPE6";
        public const string CMW_HKIDPASSPORT = "TYPE7";
        public const string CMW_NA = "TYPE8";
        public const string CMW_SpecimenSignature = "TYPE9";
        public const string CMW_NonSpecimenSignature = "TYPE10";

        //Auth Right - IMW
        public const string IMW_Name_RegNo = "TYPE1";
        public const string IMW_Contact = "TYPE2";
        public const string IMW_HKIDPASSPORT = "TYPE3";
        public const string IMW_SpecimenSignature = "TYPE4";
        public const string IMW_LeaveRecords = "TYPE5";
        public const string IMW_CorrAddress = "TYPE6";
        public const string IMW_AppliStatus = "TYPE7";
        public const string IMW_OfficeAddress = "TYPE8";

        //Auth Right - IP
        public const string IP_Name_RegNo = "TYPE1";
        public const string IP_Contact = "TYPE2";
        public const string IP_HKIDPASSPORT = "TYPE3";
        public const string IP_SpecimenSignature = "TYPE4";
        public const string IP_LeaveRecords = "TYPE5"; 
        public const string IP_CorrAddress = "TYPE6";
        public const string IP_AppliStatus = "TYPE7";
        public const string IP_OfficeAddress = "TYPE8";

        //MWIS
        public const string SERVICE_IN_MWIS_NO_INDICATION = "-";
        public const string SERVICE_IN_MWIS_YES = "Y";

        public const string LETTER_TEMPLATE_PATH = "LETTER_TEMPLATE_PATH\\";
    }
}
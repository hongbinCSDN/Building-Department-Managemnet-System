using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Constant
{
    public class SignboardConstant
    {

        public const string WF_STATUS_OPEN = "WF_STATUS_OPEN";
        public const string WF_STATUS_CLOSE = "WF_STATUS_CLOSE";
        public const string WF_STATUS_DONE = "WF_STATUS_DONE";

        public static string CHECKING_TYPE_SUBMISSION_INFO_CHECKING = "submissionInfoChecking";
        public static string CHECKING_TYPE_ITEM_CHECKING = "itemChecking";
        public static string CHECKING_TYPE_SUMMARY = "summary";

        public static string MWITEM_VERSION = "MWITEM_VERSION";
        public static string WF_GO_OTHER = "WF_GO_OTHER";
        public static string WF_GO_SPO = "WF_GO_SPO";
        public static string WF_GO_NEXT = "WF_GO_NEXT";
        public static string WF_GO_BACK = "WF_GO_BACK";
        public static string WF_GO_END = "WF_END";

        public const string SV_SUBMISSION_STATUS_SCU_RECEIVED = "SCU_RECEIVED";
        public const string SV_SUBMISSION_RECORD_TYPE_SCU = "SCU";
        public const string SV_SUBMISSION_STATUS_SCU_DATA_ENTRY = "SCU_DATA_ENTRY";
        public const string SCAN_DOC_RECORD_TYPE_VALIDATION = "VALIDATION";
        public const string SCAN_DOC_DOCUMENT_TYPE_FORM = "Form";

        public const string SCANNED_DOC_FOLDER_TYPE_PRIVATE_SCU = "Private (SCU)";
        public static string SCANNED_DOC_FOLDER_TYPE_PUBLIC = "Public";
        public static string SCANNED_DOC_FOLDER_TYPE_PRIVATE_BD = "Private (BD)";


        public const string WF_MAP_VALIDATION_ISSUE_LETTER_TO = "WF_VALIDATION_LETTER_TO";

        public const string WF_MAP_VALIDATION_START = WF_MAP_VALIDATION_ISSUE_LETTER_TO;

        public const string WF_MAP_VALIDATION_TO = "WF_VALIDATION_TO";
        public const string WF_MAP_VALIDATION_PO = "WF_VALIDATION_PO";
        public const string WF_MAP_VALIDATION_SPO = "WF_VALIDATION_SPO";
        public const string WF_MAP_VALIDATION_END = "WF_VALIDATION_END";
        //AUDIT
        public const string WF_MAP_AUDIT_TO = "WF_AUDIT_TO";
        public const string WF_MAP_AUDIT_PO = "WF_AUDIT_PO";
        public const string WF_MAP_AUDIT_SPO = "WF_AUDIT_SPO";
        //-----S24------
        public const string WF_MAP_S24_SPO_APPROVE = "WF_S24_SPO_APPROVE";
        public const string WF_MAP_S24_PO = "WF_S24_PO";
        public const string WF_MAP_S24_SPO_COMPILE = "WF_MAP_S24_SPO_COMPILE";
        public const string WF_MAP_S24_END = "WF_S24_SPO_END";
        //-----GC------
        public const string WF_MAP_GC_SPO_APPROVE = "WF_GC_SPO_APPROVE";
        public const string WF_MAP_GC_PO = "WF_GC_PO";
        public const string WF_MAP_GC_SPO_COMPILE = "WF_MAP_GC_SPO_COMPILE";
        public const string WF_MAP_GC_PO_COMPLI = "WF_MAP_GC_PO_COMPILE";
        public const string WF_MAP_GC_END = "WF_GC_SPO_END";
        //END
        public const string WF_MAP_END = "WF_END";
        //-----Validation------
        public const string WF_MAP_ASSIGING = "WF_MAP_ASSIGING";
        //-----Validation------
        public const string DISPLAY_WF_MAP_ASSIGING = "Assigning";
        //-----Validation------
        public const string DISPLAY_WF_MAP_VALIDATION_TO = "SO/TO Validation";
        public const string DISPLAY_WF_MAP_VALIDATION_PO = "TL Validation";
        public const string DISPLAY_WF_MAP_VALIDATION_SPO = "SPO Validation";
        public const string DISPLAY_WF_MAP_AUDIT_TO = "SO/TO Audit";
        public const string DISPLAY_WF_MAP_AUDIT_PO = "TL Audit";
        public const string DISPLAY_WF_MAP_AUDIT_SPO = "SPO Audit";
        //-----S24------
        public const string DISPLAY_WF_MAP_S24_SPO_APPROVE = "SPO Approve";
        public const string DISPLAY_WF_MAP_S24_PO = "PO processing S24";
        public const string DISPLAY_WF_MAP_S24_SPO_COMPILE = "SPO Compliance";
        public const string DISPLAY_WF_MAP_S24_END = "Completed";
        //-----GC------
        public const string DISPLAY_WF_MAP_GC_SPO_APPROVE = "SPO Approve";
        public const string DISPLAY_WF_MAP_GC_PO = "TL processing GC";
        public const string DISPLAY_WF_MAP_GC_SPO_COMPILE = "SPO Compliance";
        public const string DISPLAY_WF_MAP_GC_PO_COMPLI = "TL Compliance";
        public const string DISPLAY_WF_MAP_GC_END = "Completed";
        //END
        public const string DISPLAY_WF_MAP_END = "Closed";
        public const string DISPLAY_WF_MAP_VALIDATION_END = DISPLAY_WF_MAP_END;
        public const string DISPLAY_WF_MAP_VALIDATION_ISSUE_LETTER_TO = "SO/TO Issue Letter";

        public const string FORM_CODE_SC01 = "SC01";
        public const string FORM_CODE_SC01C = "SC01C";
        public const string FORM_CODE_SC02 = "SC02";
        public const string FORM_CODE_SC02C = "SC02C";
        public const string FORM_CODE_SC03 = "SC03";
        public const string VALIDATION_RESULT_ACKNOWLEDGED = "A";
        public const string VALIDATION_RESULT_REFUSED = "R";
        public const string VALIDATION_RESULT_CONDITIONAL = "C";
        public const string VALIDATION_RESULT_WITHDRAW = "WD";

        public const string S24_STATUS_NOT_YET_ISSUED = "Not Yet Issued";
        public const string S24_STATUS_NOT_YET_EXPIRED = "Not Yet Expired";
        public const string S24_STATUS_APPEAL_LODGED = "Appeal Lodged";
        public const string S24_STATUS_PROSECUTION_TO_BE_INITIATED = "Prosecution To Be Initiated";
        public const string S24_STATUS_PROSECUTION_INITIATED = "Prosecution Initiated";
        public const string S24_STATUS_GC_ACTION = "GC Action";
        public const string S24_STATUS_PENDING_FOR_COMPLIANCE = "Pending For Compliance";
        public const string S24_STATUS_COMPLIED_WITH = "Complied With";
        public const string S24_STATUS_WITHDRAWN = "Withdrawn";
        public const string COMPLAIN_STATUS_OPEN = "Open";
        public const string COMPLAIN_STATUS_CLOSE = "Close";


        public static string getValidationAlterationFormCode(string formCode)
        {
            if (FORM_CODE_SC01C == formCode)
            {
                return FORM_CODE_SC01;
            }
            else if (FORM_CODE_SC02C == formCode)
            {
                return FORM_CODE_SC02;
            }
            else
            {
                return formCode;
            }
        }
        public const string COMPLAIN = "Report To SCU";
        public const string ENQUIRY = "Enquriy To SCU";

        public const string RECOMMEND_ACK_STR = "Accept";
        public const string RECOMMEND_REF_STR = "Refuse";
        public const string RECOMMEND_COND_STR = "Conditional Accept";
        public const string RECOMMEND_WITH_STR = "Withdraw";

        public static string SYSTEM_TYPE_S24_ORDER = "SO";
        public static string SYSTEM_TYPE_CLASS = "Class";
        public static string SYSTEM_TYPE_ITEM_NO = "Item No";
        public static string SYSTEM_TYPE_GC = "GC";
        public static string SYSTEM_TYPE_PARAMETER = "SYSTEM_TYPE_PARAMETER";
        public static string SYSTEM_TYPE_AUDIT_STATUS = "AuditStatus";
        public static string SYSTEM_TYPE_SIGNBOARD_LOCATION_TEMPLATE = "SignboardLocationTemplate";



        public const string CLASS_I = "CLASS I";
        public const string CLASS_II = "CLASS II";
        public const string CLASS_III = "CLASS III";

        public const string S_USER_ACCOUNT_SECURITY_LEVEL_SCU_INTERNAL = "SCU Internal";
        public const string S_USER_ACCOUNT_SECURITY_LEVEL_BD_STAFF = "BD Staff";
        public const string S_USER_ACCOUNT_STATUS_ACTIVE = "ACTIVE";
        public const string S_USER_ACCOUNT_STATUS_INACTIVE = "INACTIVE";
        public const string S_USER_ACCOUNT_RANK_TO = "TO";
        public const string S_USER_ACCOUNT_RANK_SPO = "SPO";
        public const string S_USER_ACCOUNT_RANK_PO = "PO";
        public const string S_USER_ACCOUNT_RANK_TC = "TC";

        public const string LOOK_UP_NAME_RANK_ALL = "LOOK_UP_NAME_RANK_ALL";
        public const string LOOK_UP_NAME_RANK_TO = "TO";
        public const string LOOK_UP_NAME_RANK_PO = "PO";
        public const string LOOK_UP_NAME_RANK_SPO = "SPO";

        public const string LED_YES = "YES";
        public const string LED_NO = "NO";
        public const string LED_NA = "N/A";

        public const string VALIDATED = "VALIDATED";
        public const string NOT_VALIDATED = "NOT_VALIDATED";

        public static string OK = "OK";
        public static string NOT_OK = "Not OK";
        public static string NA = "N/A";
        //Data Entry
        public const string PBP_CODE_AP = "AP";
        public const string PBP_CODE_RSE = "RSE";
        public const string PBP_CODE_RGE = "RGE";
        public const string PBP_CODE_RI = "RI";
        public const string PRC_CODE = "PRC";
        public const string CODE_GBC = "GBC";
        public const string CODE_MWC_W = "MWC(W)";
        public const string CODE_MWC = "MWC";
        public const string CODE_PBP = "PBP";
        public const string CODE_SB_OWNER = "SB_OWNER";


        public const string SV_Validation = "Validation";

        public const string WF_TYPE_VALIDATION = "WF_TYPE_VALIDATION";
        public const string WF_TYPE_AUDIT = "WF_TYPE_AUDIT";

        public static string  WF_TYPE_S24 = "WF_TYPE_S24";
	    public static string  WF_TYPE_GC = "WF_TYPE_GC";

        public const string DATA_ENTRY_LED_YES = "Y";
        public const string DATA_ENTRY_LED_NO = "N";
        public const string DATA_ENTRY_LED_NA = "N/A";


        public static string SYSTEM_TYPE_VALIDATION_ITEM = "ValidationItem";
        public static string SYSTEM_TYPE_REASON_FOR_REFUSE = "ReasonForRefuse";
        public static string SYSTEM_TYPE_BCIS_DISTRICT = "BcisDistrict";
        public static string SYSTEM_TYPE_LETTER_RESULT = "LetterResult";
        public static string SYSTEM_TYPE_LETTER_TYPE = "LetterType";
        public static string SYSTEM_TYPE_FORM_CODE = "FormCode";
        public static string SYSTEM_TYPE_LETTER_TEMPLATE_CONSTANT = "LetterTemplateConstant";


        public static string NuMBER_TYPE_SV_BATCH = "BT";
        public static string FORM_MODE_UPDATE_SIGNBOARD_APPLICATION = "2";
        public static string DEFAULT_LANG = LANG_CHINESE;
        public static string LANG_ENGLISH = "EN";
        public static string LANG_CHINESE = "ZH";

        public static string SIGNBOARD_TYPE_SHOPFRONT = "1. Shop Front";
        public static string SIGNBOARD_TYPE_PROJECTING = "2. Projecting";
        public static string SIGNBOARD_TYPE_WALL = "3. Wall";
        public static string SIGNBOARD_TYPE_CANOPY = "4. Canopy";
        public static string SIGNBOARD_TYPE_BALCONY = "5. Balcony";
        public static string SIGNBOARD_TYPE_ONGRADE = "6. On-grade";
        public static string SIGNBOARD_TYPE_ROOF = "7. Roof";


        public static string SAVE_MODE = "save";
        public static string SUBMIT_MODE = "submit";
        public static string PASS_MODE = "pass";
        public static string ROLLBACK_MODE = "rollback";
        public static string END_MODE = "end";
        public static string IMAGE_FILE_PATHa ="C:\\SMM_SCAN";
	    public static string IMAGE_FOLDER_TYPE_PRIVATE = "PRIVATE"; 
	    public static string IMAGE_SUBMIT_TYPE_UPLOAD_IMAGE = "UPLOAD_IMAGE";

        public static string SV_RECORD_STATUS_CODE_DATA_ENTRY_COMPLETED = "DATA_ENTRY_COMPLETED";
        public static string SV_RECORD_STATUS_CODE_DATA_ENTRY_DRAFT = "DATA_ENTRY_DRAFT";
        public static string SV_SUBMISSION_STATUS_VALIDATION = "VALIDATION";
        public static string DB_CHECKED = "Y";
	    public static string DB_UNCHECKED = "N";

        public static string SV_VALIDATION_STATUS_CODE_SPO_ASSIGNMENT = "SPO_ASSIGNMENT";
        public static string  SV_VALIDATION_STATUS_CODE_OPEN = "OPEN";
	    public static string  SV_VALIDATION_STATUS_CODE_CLOSE = "CLOSE";

        public static string[] COMPLETION_FORM_CODES = new string[] { FORM_CODE_SC01C, FORM_CODE_SC02C };

        public static string SV_AUDIT_STATUS_ASSIGN = "Assigned";
        public static string SV_AUDIT_STATUS_NOT_YET_ASSIGN = "Not yet assign";
        public static string SV_AUDIT_STATUS_COMPLETE = "Completed (In-order or final action taken)";

        public static string AUDIT_RESULT_COMPELETE = "Complete";
	    public static string AUDIT_RESULT_NOT_COMPELETE = "Not yet complete";

        public static string LETTER_TYPE_ACKNOWLEDGEMENT_LETTER_CODE = "AL";
	    public static string LETTER_TYPE_IO_LETTER_CODE = "IO";
	    public static string LETTER_TYPE_D_LETTER_CODE = "DL";
	    public static string LETTER_TYPE_ADUIT_FORM_CODE = "FM";
        public static string LETTER_TYPE_ADVISORYLETTER_CODE = "ADL";

        public static string LETTER_TYPE_ACKNOWLEDGEMENT_LETTER = "Acknowledgement letter";
	    public static string LETTER_TYPE_IO_LETTER = "I.O. letter";
	    public static string LETTER_TYPE_D_LETTER = "D-letter";
        public static string Letter_SPO_type = "SPO_NAME";
	    public static string Letter_SPO_CH_name_code = "SPO_CH_NAME";
        public static string Letter_SPO_ENG_name_code = "SPO_ENG_NAME";
        //public static string AUDIT_RESULT_NOT_COMPELETE = "Not yet complete";

        // Create, Edit, View
        public static string CREATE_MODE = "create";
        public static string EDIT_MODE = "edit";
        public static string VIEW_MODE = "view";

        // Save, Submit, Rollback, End
        //public static string SAVE_MODE = "save";
        //public static string PASS_MODE = "pass";
        //public static string SUBMIT_MODE = "submit";
        //public static string ROLLBACK_MODE = "rollback";
        //public static string END_MODE = "end";

        public const string RECORD_TYPE_VALIDATION = "VALIDATION";
        public const string RECORD_TYPE_AUDIT = "AUDIT";

        public const string SIGNBOARD_THUMBNAIL_WIDTH = "200px";

        public const string FORM_CODE_AUDIT_FORM = "AuditForm";
        public const string FORM_CODE_ADVISORY_LETTER = "AdvisoryLetter";

        public const string RECOMMENDATION_ACCEPT = "Accept";
	    public const string RECOMMENDATION_NOT_ACCEPT = "Not Accept";
	    public const string RECOMMENDATION_REFUSE = "Refuse";
	    public const string RECOMMENDATION_CONDITIONAL_ACCEPT = "Conditional Accept";
        public const string RECOMMENDATION_WITHDRAW = "Withdraw";

        public const string LETTER_TEMPLATE_FILE_PATH = "Template\\LetterTemplatePath\\";
        public const string LETTER_MODULE_TEMPLATE_FILE_PATH = "Template\\LetterModulTemplate\\";
        public const string LETTER_TEMPLATE_CHIN = "CH";
        public const string LETTER_TEMPLATE_ENG = "EN";

        public const string TDL_SEARCH_STATUS_VALIDATE = "Validate";
        public const string TDL_SEARCH_STATUS_EXPIRED_BUT_NOT_REVALIDATED = "Expired but not re-validated";
        public const string TDL_SEARCH_STATUS_EXPIRED_BUT_REVALIDATED = "Expired but re-validated";
        public const string TDL_SEARCH_STATUS_REVALIDATED = "Re-validated";

        public const string PHOTO_LIBRARY_URL = "https://dp2.bd.hksarg/wpls_prod/jsp/spa/spa0102.jsp?BK_ID=";
    }
}
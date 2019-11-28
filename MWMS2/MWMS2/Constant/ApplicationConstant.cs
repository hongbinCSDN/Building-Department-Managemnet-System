using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Constant
{
    public class ApplicationConstant
    {
        public static string AppPath => ConfigurationManager.AppSettings["AppPath"];

        public const string DRAFT_KEY_SEARCH = "DRAFT_KEY_SEARCH";

        public static string getSystemWorkingPath()
        {
            return ConfigurationManager.AppSettings["WorkingPath"];
        }

        public static string CRMFilePath => ConfigurationManager.AppSettings["CRMFilePath"];
        public static string PEMFilePath => ConfigurationManager.AppSettings["PEMFilePath"];
        public static string SMMFilePath => ConfigurationManager.AppSettings["SMMFilePath"];
        public static string SMMSCANFilePath => ConfigurationManager.AppSettings["SMMSCANFilePath"];
        public static string WLFilePath => ConfigurationManager.AppSettings["WLFilePath"];
        public static string CRMBATCHUPLOAD => ConfigurationManager.AppSettings["CRMBATCHUPLOAD"];
        public static string OtherFilePath => ConfigurationManager.AppSettings["FilePath"];

        public const string REGISTER_EXPORT_PATH = "REGISTER";

        public static string getTempPath(string tempPath)
        {   return System.IO.Path.GetTempPath()+ FileSeparator +
                    tempPath+ FileSeparator;
        }

        public static char FileSeparator => System.IO.Path.DirectorySeparatorChar;


        public const string DISPLAY_DATE_FORMAT = "dd/MM/yyyy";
        public const string DISPLAY_DATE_COLUMN_SUFFIX = "_DATE_DISPLAY";
        public static SelectListItem SELECT_LIST_ITEM_BLANK = new SelectListItem() { Text=" - SELECT -", Value = "" };


        public const string DRAFT_KEY_C_COMP_APPLICANT_INFO = "DRAFT_KEY_C_COMP_APPLICANT_INFO";
        public const string DELETE_KEY_C_COMP_APPLICANT_INFO = "DELETE_KEY_C_COMP_APPLICANT_INFO";

        public const string DRAFT_KEY_C_IND_CERTIFICATE = "DRAFT_KEY_C_IND_CERTIFICATE";
        public const string DELETE_KEY_C_IND_CERTIFICATE = "DELETE_KEY_C_IND_CERTIFICATE";

        public const string DRAFT_KEY_REG_ICC = "DRAFT_KEY_ICC";


        public const string DRAFT_KEY_MEMBER = "DRAFT_KEY_MEMBER";
        public const string DELETE_KEY_MEMBER = "DELETE_KEY_MEMBER";
        public const string DRAFT_KEY_TEMP_ID = "DRAFT_KEY_TEMP_ID";
        public const string DRAFT_KEY_APPLICANTS = "DRAFT_KEY_APPLICANTS";
        public const string DELETE_KEY_APPLICANTS = "DELETE_KEY_APPLICANTS";
        public const string DRAFT_KEY_APPLICANT_MASTERS = "DRAFT_KEY_APPLICANT_MASTERS";
        public const string DELETE_KEY_APPLICANT_MASTERS = "DELETE_KEY_APPLICANT_MASTERS";
        public const string DRAFT_KEY_APPLICANT_FILE = "DRAFT_KEY_APPLICANT_FILE";
        public const string DELETE_KEY_APPLICANT_FILE = "DELETE_KEY_APPLICANT_FILE";
        public const string DRAFT_KEY_QUALIFICATION = "DRAFT_KEY_QUALIFICATION";
        public const string DELETE_KEY_QUALIFICATION = "DELETE_KEY_QUALIFICATION";
        public const string DRAFT_KEY_CERTIFICATE = "DRAFT_KEY_CERTIFICATE";
        public const string DELETE_KEY_CERTIFICATE = "DELETE_KEY_CERTIFICATE";

        public const string DRAFT_KEY_MWITEM = "DRAFT_KEY_MWITEM";
        public const string DELETE_KEY_MWITEM = "DELETE_KEY_MWITEM";


        public const string DRAFT_KEY_NEWMWITEM = "DRAFT_KEY_NEWMWITEM";
        public const string DELETE_KEY_NEWMWITEM = "DELETE_KEY_NEWMWITEM";

        public const string DRAFT_KEY_COMMITTEE_MEMBER = "DRAFT_KEY_COMMITTEE_MEMBER";
        public const string DELETE_KEY_COMMITTEE_MEMBER = "DELETE_KEY_COMMITTEE_MEMBER";

        public const string DRAFT_KEY_SMMMWITEM = "DRAFT_KEY_SMMMWITEM";
        public const string DELETE_KEY_SMMMWITEM = "DELETE_KEY_SMMMWITEM";
        public const string DRAFT_KEY_SMMMWITEMDETAIL = "DRAFT_KEY_SMMMWITEMDETAIL";
        public const string DELETE_KEY_SMMMWITEMDETAIL = "DELETE_KEY_SMMMWITEMDETAIL";

        public const string DRAFT_KEY_PHOTOLIB = "DRAFT_KEY_PHOTOLIB";
        public const string DELETE_KEY_PHOTOLIB = "DELETE_KEY_PHOTOLIB";

        public const string DRAFT_KEY_SBRELATION = "DRAFT_KEY_SBRELATION";
        public const string DELETE_KEY_SBRELATION = "DELETE_KEY_SBRELATION";

        public const string PBP_CODE_AP = "AP";
        public const string PBP_CODE_RSE = "RSE";
        public const string PBP_CODE_RGE = "RGE";
        public const string PRC_CODE = "PRC";

        public const string RECOMMEND_ACK_STR = "Accept";
        public const string RECOMMEND_REF_STR = "Refuse";
        public const string RECOMMEND_COND_STR = "Conditional Accept";
        public const string RECOMMEND_WITH_STR = "Withdraw";
        public const string SV_VALIDATION_LETTER_STATUS_DONE = "DONE";
        public static string DB_CHECKED = "Y";
	    public static string DB_UNCHECKED = "N";

        public const string SAVE_MODE = "save";
	    public const string PASS_MODE = "pass";
	    public const string SUBMIT_MODE = "submit";
	    public const string ROLLBACK_MODE = "rollback";
	    public const string END_MODE = "end";

        public const string VALIDATION_STATUS_OPEN = "OPEN";
	    public const string VALIDATION_STATUS_CLOSE = "CLOSE";
	    public const string VALIDATION_STATUS_SPO_ASSIGNMENT = "SPO_ASSIGNMENT";
        public const string VALIDATION_STATUS_ROLLBACK = "ROLLBACK";

        public const string VALIDATION_RESULT_ACKNOWLEDGED ="A";
        public const string VALIDATION_RESULT_REFUSED ="R";
        public const string VALIDATION_RESULT_CONDITIONAL  ="C";
        public const string VALIDATION_RESULT_WITHDRAW  ="WD";

        public const string FORM_CODE_VALIDATION_FROM = "SC01/02";
	    public const string FORM_CODE_VALIDATION = "SC01";
	    public const string FORM_CODE_VALIDATION_STRENGTHENING = "SC02";
	    public const string FORM_CODE_COMPLETE = "SC03";

        public const string ADMIN = "ADMIN";

     //Submission Nature
        public const string SUBMISSION = "Submission";
        public const string ESUBMISSION = "eSubmission";
        public const string ICU = "ICU";
        public const string DIRECT_RETURN = "Direct Return";
        public const string REVISED_CASE = "Revised Case";
        public const string WITHDRAWAL = "Withdrawal";


    }
}

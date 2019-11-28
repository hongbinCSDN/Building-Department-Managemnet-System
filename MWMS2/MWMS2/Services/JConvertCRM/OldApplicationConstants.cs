using MWMS2.Areas.Registration.Models;
using MWMS2.Constant;
using MWMS2.Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;

namespace MWMS2.Services
{

    public class OldApplicationConstants
    {
        //List Management
        public  const string SYSTEM_TYPE_APPLICANT_ROLE = "APPLICANT_ROLE";
	public  const string SYSTEM_TYPE_APPLICANT_STATUS = "APPLICANT_STATUS";
	public  const string SYSTEM_TYPE_APPLICATION_FORM = "APPLICATION_FORM";
	public  const string SYSTEM_TYPE_DUE_DATE_CLASS_DROPDOWN = "DUE_DATE_CLASS_DROPDOWN";
	public  const string SYSTEM_TYPE_BUILDING_SAFETY_CODE = "BUILDING_SAFETY_CODE";
	public  const string SYSTEM_TYPE_CATEGORY_GROUP = "CATEGORY_GROUP";
	public  const string SYSTEM_TYPE_CATEGORY_GROUP_HEADER = "CATEGORY_GROUP_HEADER";
	public  const string SYSTEM_TYPE_COMMITTEE_TYPE = "COMMITTEE_TYPE";
	public  const string SYSTEM_TYPE_CONVICTION_CATEGORY = "CONVICTION_CATEGORY";
	public  const string SYSTEM_TYPE_INTERVIEW_RESULT = "INTERVIEW_RESULT";
	public  const string SYSTEM_TYPE_MEMBER_ROLE = "MEMBER_ROLE";
	public  const string SYSTEM_TYPE_MINOR_WORKS_CLASS = "MINOR_WORKS_CLASS";
	public  const string SYSTEM_TYPE_MINOR_WORKS_ITEM = "MINOR_WORKS_ITEM";
	public  const string SYSTEM_TYPE_MINOR_WORKS_TYPE = "MINOR_WORKS_TYPE";
	public  const string SYSTEM_TYPE_NB_CATEGORY = "NB_CATEGORY";
	public  const string SYSTEM_TYPE_NB_CODE = "NB_CODE";
	public  const string SYSTEM_TYPE_NON_BUILDING_WORKS_RELATED = "NON_BUILDING_WORKS_RELATED";
	public  const string SYSTEM_TYPE_PANEL_TYPE = "PANEL_TYPE";
	public  const string SYSTEM_TYPE_PANEL_ROLE = "PANEL_ROLE";
	public  const string SYSTEM_TYPE_COMMITTEE_ROLE = "COMMITTEE_ROLE";
	public  const string SYSTEM_TYPE_COMMITTEE_MEMBER_STATUS = "COMMITTEE_MEMBER_STATUS";
	public  const string SYSTEM_TYPE_PERIOD_OF_VALIDITY = "PERIOD_OF_VALIDITY";
	public  const string SYSTEM_TYPE_PRACTICE_NOTE = "PRACTICE_NOTES";
	public  const string SYSTEM_TYPE_PROF_REGISTRATION_BOARD = "PROF_REGISTRATION_BOARD";
	public  const string SYSTEM_TYPE_REGION_CODE = "REGION_CODE";
	public  const string SYSTEM_TYPE_SEARCHING_LEVEL = "SEARCHING_LEVEL";
	public  const string SYSTEM_TYPE_SOCIETY_NAME = "SOCIETY_NAME";
	public  const string SYSTEM_TYPE_TIME_SESSION = "TIME_SESSION";
	public  const string SYSTEM_TYPE_TITLE = "TITLE";
	public  const string SYSTEM_TYPE_CONVICTION_SOURCE = "CONVICTION_SOURCE";
	public  const string SYSTEM_TYPE_REGISTRATION_TYPE = "REGISTRATION_TYPE";
	public  const string SYSTEM_TYPE_ROLE_AS_TD_OO = "ROLE_AS_TD_OO";
	public  const string SYSTEM_TYPE_COMP_CATEGORY_CODE = "COMP_CATEGORY_CODE";
	public  const string SYSTEM_TYPE_AS_TD_OO_STATUS = "AS_TD_OO_STATUS";
	
	public  const string SEARCH_REPORT_CASE_IN_HAND = "CASE_IN_HAND_REPORT";
	public  const string SEARCH_NUMBER_OF_REGISTRATION = "NUMBER_OF_REGISTRATION_REPORT";
	//public  const string SYSTEM_TYPE_REGISTRATION_TYPE_IR = "REGISTRATION_TYPE_IR";
	public  const string REGISTERED_COMPANY = "REGISTERED_COMPANY_REPORT";
	public  const string REGISTERED_PERSON = "REGISTERED_PERSON_REPORT";

	public  const string SYSTEM_TYPE_WILLINGNESS_DROPDOWN = "WILLINGNESS_DROPDOWN";
	public  const string SYSTEM_TYPE_QPCOUNT_DATE = "QPCOUNT_DATE";
	
	public  const string SYSTEM_TYPE_FSS_DROPDOWN = "FSS_DROPDOWN";
	
	public  const string SYSTEM_TYPE_COMPANY_TYPE = "COMPANY_TYPE";
	public  const string SYSTEM_TYPE_COMM_PANEL_SECTION = "COMM_PANEL_SECTION";
	
	public  const string SYSTEM_TYPE_COMM_TYPE_SECTION = "COMM_TYPE_SECTION";
	
	public  const string SYSTEM_TYPE_REGISTRATION_MW_TYPE = "REGISTRATION_MW_TYPE";
	
	public  const string SYSTEM_TYPE_EXPORT_TITLE = "EXPORT_TITLE";
	
	public  const string SYSTEM_TYPE_APPROVED_BY_LIST = "APPROVED_BY_LIST";
	
	public  const string SYSTEM_TYPE_VETTING_OFFICER = "VETTING_OFFICER";
	public  const string SYSTEM_TYPE_SECRETARY = "SECRETARY";
	public  const string SYSTEM_TYPE_ASSISTANT = "ASSISTANT";
	public  const string SYSTEM_TYPE_LETTER_TEMPLATE = "LETTER_TEMPLATE";
	
	public  const string VALID_MEMBER_STATUS = "Valid";
	public  const string INVALID_MEMBER_STATUS = "Invalid";

	
	public  const string AUTHORITY_AUTHORITY_NAME = "AUTHORITY_NAME";
	public  const string HTML_NOTES = "HTML_NOTES";
	public  const string CATEGORY_CODE = "CATEGORY_CODE";
	
	public  const string CATEGORY_CODE_FOR_CGC_REPORT = "CATEGORY_CODE_FOR_CGC_REPORT";
	
	public  const string ROOM = "ROOM";
	public  const string PUBLIC_HOLIDAY_MANAGEMENT = "PUBLIC_HOLIDAY_MANAGEMENT";
	public  const string BUILDING_SAFETY_CODE_ITEM = "BUILDING_SAFETY_CODE_ITEM";
	//added on Sep 1,2010
	public  const string QUALIFICATION_ITEM = "QUALIFICATION_ITEM";
	public  const string SYSTEM_TYPE_QUALIFICATION_TYPE = "QUALIFICATION_TYPE";
	public  const string CATEGORY_CODE_DETAILS = "CATEGORY_CODE_DETAILS";
	
	
	
	//Administration
	public  const string USER_PROFILE = "USER_PROFILE";
	public  const string USER_EDIT_PROFILE = "USER_EDIT_PROFILE";
	public  const string USER_GROUP = "USER_GROUP";
	public  const string SEARCH_OPTIONS = "SEARCH_OPTIONS";
	public  const string PROGRAM_ACCESS = "PROGRAM_ACCESS";
	public  const string ADMIN_REPORT = "ADMIN_REPORT";
	
	//Committee Maintenance form tab
	public  const string CM_MEMBER_INFO = "memberInfo";
	public  const string CM_COMMITTEE_TYPE = "committeeType";
	public  const string CM_INSTITUTES = "institutes";
	
	
	//Status Code
	
	public  const string STATUS_ACTIVE = "1";
	public  const string STATUS_APPLICATION_IN_PROGRESS = "2";
	public  const string STATUS_DOCUMENT_MISSING = "3";
	public  const string STATUS_CERTIFICATE_PREPARED = "4";
	public  const string STATUS_SCHEDULED_FOR_INTERVIEW = "5";
	public  const string STATUS_REMOVAL_LETTER_PREPARED = "6";
	public  const string STATUS_REMOVED = "7";
	public  const string STATUS_INACTIVE = "8";
	public  const string STATUS_WITHDRAWN = "9";
	public  const string BUSINESS_CLOSED = "10";

	
	//PERIOD_OF_VALIDITY
	public  const string PERIOD_OF_VALIDITY_5 = "5";

	//Registration Type
	public  const string COMPANY_GENERAL_CONTRACTOR = "CGC";
	public  const string COMPANY_MINOR_WORK = "CMW";
	public  const string INDIVIDUAL_PROFESSIONAL = "IP";
	public  const string INDIVIDUAL_MINOR_WORK = "IMW";
	
//	public  const string LOOKUP_TYPE_INSPECTOR_REGISTER_A = "RI";
//  public  const string LOOKUP_TYPE_CATEGORY_CODE_QUALIFICATION_A = "CATEGORY_CODE_QUALIFICATION";

	
	public  const string ALL_TYPE = "ALL";
	
	public  const string MINOR_WORK = "MW";
	
	public  const string LIST_ALL = "LIST_ALL";
	
	//Application Status
	public  const string APPLICANT_HISTORY_STATUS_CONFIRMED = "Confirmed";
	public  const string APPLICANT_HISTORY_STATUS_WITHDRAW = "Withdrawn";
	public  const string APPLICANT_HISTORY_STATUS_REFUSE = "Refused";
	public  const string APPLICANT_HISTORY_STATUS_APPLICANTION_IN_PROGRESS = "Application in progress";
	
	public  const string COMP_APPLICANT_INFO_DETAIL_STATUS_APPLY = "APPLY";
	public  const string COMP_APPLICANT_INFO_DETAIL_STATUS_APPROVED = "APPROVED";

	public  const string IND_ITEM_DETAIL_STATUS_APPLY = "APPLY";
	public  const string IND_ITEM_DETAIL_STATUS_APPROVED = "APPROVED";

	
	
	public  const string S_CATEGORY_GROUP_RI = "RI";
	
	//Category Code
	public  const string S_CATEGORY_CODE_RI_A = "RI(A)";
	public  const string S_CATEGORY_CODE_RI_E = "RI(E)";
	public  const string S_CATEGORY_CODE_RI_S = "RI(S)";
	
	public  const string S_CATEGORY_CODE_MWC = "MWC";
	public  const string S_CATEGORY_CODE_MWC_P = "MWC(P)";
	public  const string S_CATEGORY_CODE_MWC_W = "MWC(W)";
		
	public  const string S_CATEGORY_CODE_RSE = "RSE";
	public  const string S_CATEGORY_CODE_RGE = "RGE";
	
	//Checkbox Util
	public  const string DISPLAY_CHECKED = "on";
	public  const string DISPLAY_UNCHECKED = ""; 

	public  const string DB_CHECKED = "Y";
	public  const string DB_UNCHECKED = "N";
	
	//conviction import source
	public  const string COMPANY_CONVICTION_LD = "LD";
	public  const string COMPANY_CONVICTION_FEHD  = "FEHD";
	public  const string COMPANY_CONVICTION_EPD  = "EPD";
	//history Type
	public  const string COMP_NAME = "COMP_NAME";

	//System List
	public  Hashtable APPLICATION_STATUS = new Hashtable();
        public  Hashtable FILE_REFERENCE = new Hashtable();
        public  Hashtable ROLE = new Hashtable();
        public  Hashtable TITLE = new Hashtable();
        public  Hashtable SEX = new Hashtable();
        public  Hashtable COMPANY_TYPE = new Hashtable();
        public  Hashtable PERIOD_OF_VALIDITY = new Hashtable();
        public  Hashtable FORM_USED = new Hashtable();

        // Create/Edit
        public  const string CREATE_MODE = "create";
	public  const string EDIT_MODE = "edit";
	
	//Application form tab
	public  const string CGC_COMP_INFO = "companyInfo";
	public  const string CGC_COMP_APPLICANT = "companyApplicant";
	public  const string CGC_COMP_BS = "companyBuildingSafty";
	public  const string CGC_COMP_STATUS = "companyStatus";
	
	public  const string CMW_COMP_INFO = "companyInfo";
	public  const string CMW_COMP_APPLICANT = "companyApplicant";
	
	
	public  const string CMW_COMP_BS = "companyBuildingSafty";
	public  const string CMW_COMP_STATUS = "companyStatus";
	
	public  const string IP_IND_INFO = "professionalInfo";
	public  const string IP_IND_QUALI = "professionalQuali";
	public  const string IP_IND_CERT = "professionalCert";
	public  const string IP_IND_BS = "professionalBuildingSafty";
	
	public  const string IMW_IND_INFO = "individualInfo";
	public  const string IMW_IND_QUALI = "individualQuali";
	public  const string IMW_IND_MWI = "individualMwItem";
	public  const string IMW_IND_BS = "individualBuildingSafty";
	
	public  const string FOLDER_EXPORT = "companyBuildingSafty";
	public  const string FOLDER_UPLOAD = "companyBuildingSafty";
	
	public  const string SYSTEM_DEFAULT_LANGUAGE = "companyBuildingSafty";
	
	//Searching Level
	public  const string SEARCHING_TYPE1 = "TYPE1";
	public  const string SEARCHING_TYPE2 = "TYPE2";
	public  const string SEARCHING_TYPE3 = "TYPE3";
	public  const string SEARCHING_TYPE4 = "TYPE4";
	public  const string SEARCHING_TYPE5 = "TYPE5";
	public  const string SEARCHING_TYPE6 = "TYPE6";
	public  const string SEARCHING_TYPE7 = "TYPE7";
	public  const string SEARCHING_TYPE8 = "TYPE8";
	public  const string SEARCHING_TYPE9 = "TYPE9";
	public  const string SEARCHING_TYPE10 = "TYPE10";
	
	
	
	
	//Module , Function and Security Right
	public  const string READ_ONLY ="R";
	public  const string READANDWRITE ="W";
	
	public  const string READ_AND_WRITE_AND_DELETE ="D";
	public  const string EDIT_WITHDRAW ="E";
	
	public  const string NO_RIGHT ="";
	public  const string PATH ="PATH";
	
	
	public  const string ACCESS_HASHTABLE = "accessHashTable";
	
	public  const string FIRST_PAGE = "FIRST_PAGE";

	public  const string MODULE_HEADER = "HeaderMenu";
	public  const string MODULE_SEARCH = "100";
	public  const string MODULE_CGC = "200";
	public  const string MODULE_PROF = "300";
	public  const string MODULE_MWC = "400";
	public  const string MODULE_MWI = "500";
	public  const string MODULE_COMMITTEE = "600";
	public  const string MODULE_LIST = "700";
	public  const string MODULE_ADMIN = "800";
	
		public  const string FUNCTION_CGC_SEARCH = "101";
	public  const string FUNCTION_PROF_SEARCH = "102";
	public  const string FUNCTION_CGC_CONV_SEARCH = "103";
	public  const string FUNCTION_IP_CONV_SEARCH = "104";
	public  const string FUNCTION_CGC_SEARCH_DEFER = "105";
	public  const string FUNCTION_PROF_SEARCH_DEFER = "106";
	public  const string FUNCTION_MWC_SEARCH = "107";
	public  const string FUNCTION_MWI_SEARCH = "108";
	public  const string FUNCTION_MWC_SEARCH_DEFER = "109";
	public  const string FUNCTION_MWI_SEARCH_DEFER = "110";
	public  const string FUNCTION_QP_SEARCH = "111";
	public  const string FUNCTION_RI_SEARCH = "112";
	public  const string FUNCTION_CASE_IN_HAND_SEARCH = "113";
	public  const string FUNCTION_NUMBER_OF_REGISTRATION = "114";
	public  const string FUNCTION_REGISTERED_COMPANY_REPORT = "115";
	public  const string FUNCTION_REGISTERED_PERSON_REPORT = "116";
	public  const string FUNCTION_PERSON_SEARCH = "117";
	public  const string FUNCTION_EXPORT_QP = "118";


	public  const string FUNCTION_CGC_REGISTRATION_NEW = "201";
	public  const string FUNCTION_CGC_REGISTRATION_EDIT = "202";
	public  const string FUNCTION_CGC_CONVICTION_CODE = "203";
	public  const string FUNCTION_CGC_ADDRESS_SYNC_CODE = "204";
	public  const string FUNCTION_CGC_UPDATE_STATUS_CODE = "205";
	public  const string FUNCTION_CGC_UPM = "206";
	public  const string FUNCTION_CGC_UPM_10DAYS = "207";
	public  const string FUNCTION_CGC_UPM_FASK_TRACK = "208";
	public  const string FUNCTION_CGC_EXPORT_CODE = "209";
	public  const string FUNCTION_CGC_LEAVE_FORM_CODE = "211";
	public  const string FUNCTION_CGC_GENERATE_CANDIDATE = "212";
	public  const string FUNCTION_CGC_ROOM_ARRANGEMENT = "213";
	public  const string FUNCTION_CGC_INTERVIEW_CANDIDATES = "214";
	public  const string FUNCTION_CGC_INTERVIEW_RESULT = "215";
	public  const string FUNCTION_CGC_EXPORT_LETTER = "216";
	
	
	public  const string FUNCTION_PROF_REGISTRATION_NEW = "301";
	public  const string FUNCTION_PROF_REGISTRATION_EDIT = "302";
	public  const string FUNCTION_PROF_CONVICTION_CODE = "303";
	public  const string FUNCTION_PROF_ADDRESS_SYNC_CODE = "304";
	public  const string FUNCTION_PROF_UPDATE_STATUS_CODE = "305";
	public  const string FUNCTION_PROF_UPM = "306";
	public  const string FUNCTION_PROF_EXPORT_CODE = "307";
	public  const string FUNCTION_IP_REPORT = "308";
	public  const string FUNCTION_PROF_LEAVE_FORM_CODE  = "309";
	public  const string FUNCTION_PROF_GENERATE_CANDIDATE = "310";
	public  const string FUNCTION_PROF_ROOM_ARRANGEMENT = "311";
	public  const string FUNCTION_PROF_INTERVIEW_CANDIDATES = "312";
	public  const string FUNCTION_PROF_INTERVIEW_RESULT = "313";
	public  const string FUNCTION_IP_EXPORT_LETTER = "314";
	public  const string FUNCTION_RI_REPORT = "350";
	
	public  const string FUNCTION_MWC_REGISTRATION_NEW = "401";
	public  const string FUNCTION_MWC_REGISTRATION_EDIT = "402";
	public  const string FUNCTION_MWC_CONVICTION_CODE = "403";
	public  const string FUNCTION_MWC_ADDRESS_SYNC_CODE = "404";
	public  const string FUNCTION_MWC_UPDATE_STATUS_CODE = "405";
	public  const string FUNCTION_MWC_UPM = "406";
	public  const string FUNCTION_MWC_UPM_10DAYS = "407";
	public  const string FUNCTION_MWC_UPM_FASK_TRACK = "408";
	public  const string FUNCTION_MWC_EXPORT_CODE = "409";
	public  const string FUNCTION_MWC_LEAVE_FORM_CODE  = "411";
	public  const string FUNCTION_MWC_GENERATE_CANDIDATE = "412";
	public  const string FUNCTION_MWC_ROOM_ARRANGEMENT = "413";
	public  const string FUNCTION_MWC_INTERVIEW_CANDIDATES = "414";
	public  const string FUNCTION_MWC_INTERVIEW_RESULT = "415";
	public  const string FUNCTION_MWCP_UPDATE_EXPIRY_STATUS_CODE = "416";
	public  const string FUNCTION_CMW_EXPORT_LETTER = "416";
	
	public  const string FUNCTION_MWI_REGISTRATION_NEW = "501";
	public  const string FUNCTION_MWI_REGISTRATION_EDIT = "502";
	public  const string FUNCTION_MWI_CONVICTION_CODE = "503";
	public  const string FUNCTION_MWI_ADDRESS_SYNC_CODE = "504";
	public  const string FUNCTION_MWI_UPDATE_STATUS_CODE = "505";
	public  const string FUNCTION_MWI_UPM = "506";
	public  const string FUNCTION_MWI_EXPORT_CODE = "507";
	public  const string FUNCTION_MWI_LEAVE_FORM_CODE = "509";
	public  const string FUNCTION_MWI_GENERATE_CANDIDATE = "510";
	public  const string FUNCTION_MWI_ROOM_ARRANGEMENT = "511";
	public  const string FUNCTION_MWI_INTERVIEW_CANDIDATES = "512";
	public  const string FUNCTION_MWI_INTERVIEW_RESULT = "513";
	public  const string FUNCTION_IMW_EXPORT_LETTER = "514";

	
	//Committee Maintenance
	public  const string FUNCTION_CM_MEMBER_PARTICULAR = "601";
	public  const string FUNCTION_CM_COMMITTEE_PANELS = "602";
	public  const string FUNCTION_CM_COMMITTEES = "603";
	public  const string FUNCTION_CM_COMMITTEE_GROUPS = "604";
	
	//List Management
	public  const string FUNCTION_APPLICANT_ROLE = "701";
	public  const string FUNCTION_APPLICANT_STATUS = "702";
	public  const string FUNCTION_APPLICATION_FORM = "703";
	public  const string FUNCTION_AUTHORITY_NAME = "704";
	public  const string FUNCTION_BUILDINGS_SAFETY_AND_CODE = "705";
	public  const string FUNCTION_BUILDINGS_SAFETY_AND_CODE_ITEM = "706";
	public  const string FUNCTION_COMPANY_TYPE = "707";
	public  const string FUNCTION_PANEL_TYPE = "708";
	public  const string FUNCTION_COMMITTEE_TYPE = "709";
	public  const string FUNCTION_CONVICTION_SOURCE = "710";
	public  const string FUNCTION_CATEGORY_CODE = "711";
	public  const string FUNCTION_CATEGORY_GROUP = "712";
	public  const string FUNCTION_HTML_NOTES = "713";
	public  const string FUNCTION_INTERVIEW_RESULT = "714";
	public  const string FUNCTION_MEMBER_ROLE = "715";
	public  const string FUNCTION_PANEL_ROLE = "716";
	public  const string FUNCTION_COMMITTEE_ROLE = "717";
	public  const string FUNCTION_NONBUILDING_WORKS_RELATED = "718";
	public  const string FUNCTION_PERIOD_OF_VALIDITY = "719";
	public  const string FUNCTION_PRACTICE_NOTES = "720";
	public  const string FUNCTION_PROFESSIONAL_REGISTRATION_BOARD = "721";
	public  const string FUNCTION_ROOM_DETAILS = "722";
	public  const string FUNCTION_SOCIETY_NAME = "723";
	public  const string FUNCTION_TITLE = "724";
	public  const string FUNCTION_PUBLIC_HOLIDAY_MANAGEMENT = "726";
	public  const string FUNCTION_MINOR_WORKS_CLASS = "727";
	public  const string FUNCTION_MINOR_WORKS_TYPE = "728";
	public  const string FUNCTION_MINOR_WORKS_ITEM = "729";
	public  const string FUNCTION_QUALIFICATION_TYPE = "730";
	public  const string FUNCTION_QUALIFICATION_ITEM = "731";
	public  const string FUNCTION_CATEGORY_CODE_DETAILS = "732";
	public  const string FUNCTION_VETTING_OFFICER = "733";
	public  const string FUNCTION_SECRETARY = "734";
	public  const string FUNCTION_ASSISTANT = "735";
	public  const string FUNCTION_LETTER_TEMPLATE = "736";
	
	//Administration
	public  const string FUNCTION_USER_PROFILE = "801";
	public  const string FUNCTION_USER_GROUP = "802";
	public  const string FUNCTION_PROGRAM_ACCESS = "803";
	public  const string FUNCTION_SEARCH_OPTIONS = "804";
	public  const string FUNCTION_REPORTS = "805";

	//Registration Form 
	public  const string TEMP_UUID = "Temp";
	public  const string LIST_MODE = "list";
	public  const string FORM_MODE = "form";
	public  const string FORM_ITEM_MODE = "FORM_ITEM_MODE";
	
	public  const string MW_CLASS_I = "Class 1";
	public  const string MW_CLASS_II = "Class 2";
	public  const string MW_CLASS_III = "Class 3";
	
	public  const string MW_TYPE_DISPLAY = "TYPE";
	
	public  const string MW_ITEM_DISPLAY = "Item";
	
	
	public   string[] MW_TYPE = new string[] { "Type A", "Type B", "Type C", "Type D", "Type E", "Type F", "Type G" };

        public static string FILE_SEPARATOR = ApplicationConstant.FileSeparator+"";
        
	
	public  const string MW_ITEM_SUPPORT_ALL = "A";
	public  const string MW_ITEM_SUPPORT_EXP = "E";
	public  const string MW_ITEM_SUPPORT_QUALI = "Q";
	
	public  const string PROCESS_MONITOR_UPM = "UPM";
	public  const string PROCESS_MONITOR_FASK_TRACK = "FaskTrack";
	public  const string PROCESS_MONITOR_UPM_10DAYS = "UPM_10DAYS";
	
	public  const string ROOM_AVAILABLE = "available";
	public  const string ROOM_RESERVED = "reserved";
	
	public  const string SESSION_AM = "AM";
	public  const string SESSION_PM = "PM";
	public  const string START_TIME_AM = "9:15 AM";
	public  const string START_TIME_PM = "2:15 PM";
	public  const string INTERVIEW_DURATION = "30";
	public  const string INTERVIEW = "I";
	public  const string ASSESSMENT = "A";
	
	public  const string CANDIDATE_NUMBER = "CANDIDATE_NUMBER";
//	public  const string CERTIFICATE_CGC = "CERTIFICATE_CGC";
//	public  const string CERTIFICATE_MW = "CERTIFICATE_MW";
//	public  const string CERTIFICATE_PRO = "CERTIFICATE_PRO";
	
	public  const string COMMITTEE_ROLE_MEMBER = "3";
	
	public  const int REPORT_MIN = 1;


        public  const string SESSION_SEARCHING_LEVEL ="SEARCHING_LEVEL";
	
	public  const string REGISTRATION_DATE ="31/12/2010";
	public  const string EXPIRY_DATE ="31/12/2013";
	
	public  const string MW_ITEM28 = "Item 2.8";
	public  const string MW_ITEM29 = "Item 2.9";
	public  const string MW_ITEM36 = "Item 3.6";
	public  const string MW_ITEM37 = "Item 3.7";
	
	public  const string MW_TYPE_A = "Type A";
	
	public  const string MW_TYPE_G = "Type G";
	
	public  const string AP = "AP";
	public  const string RSE = "RSE";
	public  const string RI = "RI";
	
	public  const string COLON = ":";
	public  const string ARROW = "->";
	
	public  const string WILLINGNESS_YES = "Y";
	public  const string WILLINGNESS_NO = "N";
	public  const string WILLINGNESS_NO_INDICATION = "I";
	
	public  const string INTERESTED_FSS_YES = "Y";
	public  const string INTERESTED_FSS_NO = "N";
	public  const string INTERESTED_FSS_NO_INDICATION = "I";
	
	public  const string EXTERNAL_CONSULTANT_IN_MBI = "External Consultant in MBI";
	public  const string STAFF_IN_MBI = "Staff in MBI";
	public  const string EXPORT_EXCEL_REGISTERED_COMPANY = "REG_COMPANY";
	public  const string EXPORT_EXCEL_REGISTERED_PERSON = "REG_PERSON";
	public  const string EXPORT_EXCEL_APPLICATION_STAGES_REPORT = "APPLICATION_STAGES_REPORT";
	
	public  const string EXPORT_LETTER_DIV_AUTHORITY = "AUTHORITY";
	public  const string EXPORT_LETTER_DIV_COMPANY = "COMPANY";
	public  const string EXPORT_LETTER_DIV_APPLICANT = "APPLICANT";
	public  const string EXPORT_LETTER_DIV_TD = "TD";
	public  const string EXPORT_LETTER_DIV_AS = "AS";
	
	public  const string LETTER_REMARK_APPROVED_LETTER = "APPROVEDLETTER";
	
	public  const string REGISTRATION_TYPE_RVC = "RVC";
	public  const string REGISTRATION_TYPE_RC = "RC";
	public  const string REGISTRATION_TYPE_SC = "SC";
	
	public  const string SYSTEM_TYPE_APPLICATION_FORM_IND_PROC_MON = "SYSTEM_TYPE_APPLICATION_FORM_IND_PROC_MON";
	public  const string SYSTEM_TYPE_APPLICATION_FORM_CGC_PROC_MON = "SYSTEM_TYPE_APPLICATION_FORM_CGC_PROC_MON";
	public  const string SYSTEM_TYPE_INTERVIEW_RESULT_PROC_MON = "SYSTEM_TYPE_INTERVIEW_RESULT_PROC_MON";
	public  const string SYSTEM_TYPE_APPLICATION_FORM_ALL = "SYSTEM_TYPE_APPLICATION_FORM_ALL";
	
	public  const string SYSTEM_TYPE_INTERVIEW_RESULT_REPORT = "SYSTEM_TYPE_INTERVIEW_RESULT_REPORT";
	
	public  const string APPLICATION_STAGES_REPORT_RESULT_ACCEPTED = "ACCEPTED";
	public  const string APPLICATION_STAGES_REPORT_RESULT_DEFERRED = "DEFERRED";
	public  const string APPLICATION_STAGES_REPORT_RESULT_REFUSED = "REFUSED";
	public  const string APPLICATION_STAGES_REPORT_RESULT_WITHDRAWN = "WITHDRAWN";
	public  const string APPLICATION_STAGES_REPORT_RESULT_OS_LETTER_ISSUED = "O/S LETTER ISSUED";
	
	
	//QP CARD
//	public  const string QPCARD_AP = "QPCARD_AP";
//	public  const string QPCARD_GBC = "QPCARD_GBC";
//	public  const string QPCARD_RSE = "QPCARD_RSE";
//	public  const string QPCARD_MWC_W = "QPCARD_MWC(W)";
//	public  const string QPCARD_MWC = "QPCARD_MWC";
//	public  const string QPCARD_RI = "QPCARD_RI";
//	
	
	public  const string QP_PREFIX_A = "A";
	public  const string QP_PREFIX_B = "B";
	public  const string QP_PREFIX_C = "C";
	public  const string QP_PREFIX_D = "D";
	
	public  const string IMPORT_CONVICTION_ACCIDENT_TRUE = "TRUE";
	public  const string IMPORT_CONVICTION_ACCIDENT_FALSE = "FALSE";
	
}


}
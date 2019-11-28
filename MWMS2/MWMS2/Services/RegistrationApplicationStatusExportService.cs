using MWMS2.Areas.Registration.Models;
using MWMS2.Entity;
using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Utility;
using MWMS2.Constant;
using System.Text;
using System.IO;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Globalization;

namespace MWMS2.Services
{
    public class RegistrationApplicationStatusService: BaseCommonService
    {
        private static log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private String SEPARATOR = ",";
        private String DOUBLEQUOTE =	"\""; 
       
        // common function to Export Cert/Letter/QpCode
        /*
        public String exportCompUpdateAppStatus(String exportType, String process, String certificateNo, String compAppUuid, String rptIssueDt, String rptRcvdDt, String signature
            , String missingItemNullChk, String missingItemChequeChk, String missingItemProfRegCertChk
            , String missingItemOthersChk, String missingItemIncompleteFormChk)
        {
            String certificateContent = "";
            if (RegistrationConstant.CERT.Equals(exportType))
            {
                certificateContent = populateCompExportCert(process, compAppUuid, rptIssueDt, rptRcvdDt, signature);
            }
            else if (RegistrationConstant.LETTER.Equals(exportType))
            {
                certificateContent = populateCompExportLetter(process, compAppUuid, rptIssueDt, rptRcvdDt, signature);
            }
            else if (RegistrationConstant.LETTER_WITH_CODE.Equals(exportType))
            {
                certificateContent = populateCompExportLetterWithCode(certificateNo, process, compAppUuid, rptIssueDt, rptRcvdDt, signature);
            }
            return certificateContent;
        }
        */

        // Common function export cert for Prof & MWC(W)
        public String populateProfExportCert(String process, String indCertUuid, String rptIssueDt, String rptRcvdDt, String signature)
        {
            int intProcess = int.Parse(process);
            String content = "";
            C_S_CATEGORY_CODE sCategoryCode = null;
            C_IND_APPLICATION indApplication = null;
            C_APPLICANT applicant = null;

            String catCode = "";

            // get ind_certificate by uuid
            C_IND_CERTIFICATE indCertificate = getIndCertificateByUuid(indCertUuid);

            if (indCertificate != null)
            {
                //define header
                List<string> certHeaderList = null;

                sCategoryCode = getSCategoryCodeByUuid(indCertificate.CATEGORY_ID);
                if (sCategoryCode != null)
                {
                    // get catGrp
                   // C_S_SYSTEM_VALUE catGrp = getSSystemValueByUuid(sCategoryCode.CATEGORY_GROUP_ID);
                    C_S_CATEGORY_CODE catGrp = getSCategoryCodeByUuid(indCertificate.CATEGORY_ID);
                    if (catGrp != null) { catCode = catGrp.CODE; }
                }

                if (catCode != null)
                {
                    if (RegistrationConstant.S_CATEGORY_CODE_MWC.Equals(catCode)
                        || RegistrationConstant.S_CATEGORY_CODE_MWC_P.Equals(catCode)
                        || RegistrationConstant.S_CATEGORY_CODE_MWC_W.Equals(catCode))
                    {
                        certHeaderList = new List<string>()
                        {
                            "file_ref","reg_no",
                            "title","surname","given_name","given_name_id","cname","id_no","sex","salutation",
                            "gazet_dt","reg_dt","expiry_dt","removal_dt","restore_dt","approval_dt",
                            "addr1","addr2","addr3","addr4","addr5",
                            "caddr1","caddr2","caddr3","caddr4","caddr5",
                            "c_o",
                            "tel_no1","fax_no1",
                            "cat_code","ctg_code","cat_grp","cat_cgrp","ctr_sub_reg","ctr_sub_creg",
                            "auth_name","auth_cname","auth_rank","auth_crank","auth_tel","auth_fax","acting",
                            "issue_dt","cert_cname","letter_file_ref", "sn","itemline1","itemline2","itemline3","itemline4","itemline5","itemline6"
                        };

                        // print header
                        foreach (String certHeader in certHeaderList)
                        {
                            content = appendCertContent(content, certHeader);
                        }
                        content += "\r\n";
                    }
                    else
                    {
                        certHeaderList = new List<string>()
                        {
                            "file_ref","reg_no",
                            "title","surname","given_name","given_name_id","cname","id_no","sex","salutation",
                            "gazet_dt","reg_dt","expiry_dt","removal_dt","restore_dt","approval_dt",
                            "addr1","addr2","addr3","addr4","addr5",
                            "caddr1","caddr2","caddr3","caddr4","caddr5",
                            "c_o",
                            "tel_no1","fax_no1",
                            "cat_code","ctg_code","cat_grp","cat_cgrp","ctr_sub_reg","ctr_sub_creg",
                            "auth_name","auth_cname","auth_rank","auth_crank","auth_tel","auth_fax","acting",
                            "issue_dt","cert_cname","letter_file_ref"
                        };

                        if (RegistrationConstant.S_CATEGORY_GROUP_RI.Equals(catCode))
                        {
                            certHeaderList = new List<string>()
                            {
                                "file_ref","reg_no",
                                "title","surname","given_name","given_name_id","cname","id_no","sex","salutation",
                                "gazet_dt","reg_dt","expiry_dt","removal_dt","restore_dt","approval_dt",
                                "addr1","addr2","addr3","addr4","addr5",
                                "caddr1","caddr2","caddr3","caddr4","caddr5",
                                "c_o",
                                "tel_no1","fax_no1",
                                "cat_code","ctg_code","cat_grp","cat_cgrp","ctr_sub_reg","ctr_sub_creg",
                                "auth_name","auth_cname","auth_rank","auth_crank","auth_tel","auth_fax","acting",
                                "issue_dt","cert_cname","letter_file_ref", "prb_cat_code_qualification", "sn"
                            };
                        }

                        // print header
                        foreach (String certHeader in certHeaderList)
                        {
                            content = appendCertContent(content, certHeader);
                        }
                        content += "\r\n";
                    }
                }
            }

            if (indCertificate != null)
            {
                indApplication = getIndApplicationByUuid(indCertificate.MASTER_ID);
                if (indApplication != null)
                {
                    content += appendDoubleQuote(indApplication.FILE_REFERENCE_NO);
                    content = appendCertContent(content, appendDoubleQuote(indCertificate.CERTIFICATION_NO));

                    applicant = getApplicantByUuid(indApplication.APPLICANT_ID);
                    if (applicant != null)
                    {
                        String decryptHKID = EncryptDecryptUtil.getDecryptHKID(applicant.HKID);
                        if (applicant.TITLE_ID != null)
                        {
                            // get sv Title
                            C_S_SYSTEM_VALUE svTitle = getSSystemValueByUuid(applicant.TITLE_ID);
                            if (svTitle != null)
                            {
                                content = appendCertContent(content, appendDoubleQuote(svTitle.ENGLISH_DESCRIPTION));
                            }
                        }
                        content = appendCertContent(content, appendDoubleQuote(applicant.SURNAME != null ? applicant.SURNAME.ToUpper() : ""));
                        content = appendCertContent(content, appendDoubleQuote(applicant.GIVEN_NAME_ON_ID));
                        content = appendCertContent(content, appendDoubleQuote(applicant.GIVEN_NAME_ON_ID));
                        content = appendCertContent(content, appendDoubleQuote(applicant.CHINESE_NAME));
                        content = appendCertContent(content, appendDoubleQuote(decryptHKID));
                        content = appendCertContent(content, appendDoubleQuote(applicant.GENDER));

                        if (applicant.GENDER != null)
                        {
                            if (RegistrationConstant.GENDER_M.Equals(applicant.GENDER))
                            {
                                content = appendCertContent(content, appendDoubleQuote("Sir"));
                            }
                            else if (RegistrationConstant.GENDER_F.Equals(applicant.GENDER))
                            {
                                content = appendCertContent(content, appendDoubleQuote("Madam"));
                            }
                            else
                            {
                                content = appendCertContent(content, appendDoubleQuote("Sir / Madam"));
                            }
                        }
                        content = appendCertContent(content, appendDoubleQuote(indCertificate.GAZETTE_DATE != null ? indCertificate.GAZETTE_DATE.ToString() : ""));
                        content = appendCertContent(content, appendDoubleQuote(indCertificate.REGISTRATION_DATE != null ? indCertificate.REGISTRATION_DATE.ToString() : ""));

                        if (RegistrationConstant.RETENTION.Equals(intProcess) || RegistrationConstant.RESTORATION.Equals(intProcess))
                        {
                            content = appendCertContent(content, appendDoubleQuote(indCertificate.EXTENDED_DATE != null ? indCertificate.EXTENDED_DATE.ToString() : ""));
                        }
                        else
                        {
                            if (RegistrationConstant.NEW_REG.Equals(intProcess))
                            {
                                content = appendCertContent(content, appendDoubleQuote(indCertificate.EXPIRY_DATE != null ? indCertificate.EXPIRY_DATE.ToString() : ""));
                            }
                            else
                            {
                                content = appendCertContent(content, appendDoubleQuote(indCertificate.EXPIRY_DATE != null ? indCertificate.EXPIRY_DATE.ToString() : ""));
                            }
                        }

                        content = appendCertContent(content, appendDoubleQuote(indCertificate.REMOVAL_DATE != null ? indCertificate.REMOVAL_DATE.ToString() : ""));
                        content = appendCertContent(content, appendDoubleQuote(indCertificate.RESTORE_DATE != null ? indCertificate.RESTORE_DATE.ToString() : ""));
                        content = appendCertContent(content, appendDoubleQuote(indCertificate.APPROVAL_DATE != null ? indCertificate.APPROVAL_DATE.ToString() : ""));
                    }

                    if ("H".Equals(indApplication.POST_TO) || "O".Equals(indApplication.POST_TO) || null == indApplication.POST_TO)
                    {
                        // get address
                        C_ADDRESS engAddress = getAddressByUuid(indApplication.ENGLISH_HOME_ADDRESS_ID);
                        C_ADDRESS chiAddress = getAddressByUuid(indApplication.CHINESE_OFFICE_ADDRESS_ID);

                        content = appendCertContent(content, appendDoubleQuote(engAddress.ADDRESS_LINE1));
                        content = appendCertContent(content, appendDoubleQuote(engAddress.ADDRESS_LINE2));
                        content = appendCertContent(content, appendDoubleQuote(engAddress.ADDRESS_LINE3));
                        content = appendCertContent(content, appendDoubleQuote(engAddress.ADDRESS_LINE4));
                        content = appendCertContent(content, appendDoubleQuote(engAddress.ADDRESS_LINE5));
                        content = appendCertContent(content, appendDoubleQuote(chiAddress.ADDRESS_LINE1));
                        content = appendCertContent(content, appendDoubleQuote(chiAddress.ADDRESS_LINE2));
                        content = appendCertContent(content, appendDoubleQuote(chiAddress.ADDRESS_LINE3));
                        content = appendCertContent(content, appendDoubleQuote(chiAddress.ADDRESS_LINE4));
                        content = appendCertContent(content, appendDoubleQuote(chiAddress.ADDRESS_LINE5));
                        content = appendCertContent(content, appendDoubleQuote(""));
                    }

                    content = appendCertContent(content, appendDoubleQuote(indApplication.TELEPHONE_NO1));
                    content = appendCertContent(content, appendDoubleQuote(indApplication.FAX_NO1));

                    content = appendCertContent(content, appendDoubleQuote(catCode));

                    // get sv
                    C_S_SYSTEM_VALUE svCatGrp = getSSystemValueByUuid(sCategoryCode.CATEGORY_GROUP_ID);
                    if (svCatGrp != null)
                    {
                        content = appendCertContent(content, appendDoubleQuote(svCatGrp.CODE));
                        content = appendCertContent(content, appendDoubleQuote(svCatGrp.ENGLISH_DESCRIPTION));
                        content = appendCertContent(content, appendDoubleQuote(svCatGrp.CHINESE_DESCRIPTION));
                        content = appendCertContent(content, appendDoubleQuote(sCategoryCode.ENGLISH_SUB_TITLE_DESCRIPTION));
                        content = appendCertContent(content, appendDoubleQuote(sCategoryCode.CHINESE_SUB_TITLE_DESCRIPTION));
                    }

                    C_S_CATEGORY_CODE sCatCode = getSCategoryCodeByUuid(indCertificate.CATEGORY_ID);
                }

                // get authority
                C_S_AUTHORITY sAuthority = getAuthorityByUuid(signature);
                if (sAuthority != null)
                {
                    content = appendCertContent(content, appendDoubleQuote(sAuthority.ENGLISH_NAME));
                    content = appendCertContent(content, appendDoubleQuote(sAuthority.CHINESE_NAME));
                    content = appendCertContent(content, appendDoubleQuote(sAuthority.ENGLISH_RANK));
                    content = appendCertContent(content, appendDoubleQuote(sAuthority.CHINESE_RANK));
                    content = appendCertContent(content, appendDoubleQuote(sAuthority.TELEPHONE_NO));
                    content = appendCertContent(content, appendDoubleQuote(sAuthority.FAX_NO));
                    content = appendCertContent(content, appendDoubleQuote(sAuthority.ENGLISH_ACTION_NAME));
                }
                else
                {
                    content = appendCertContent(content, appendDoubleQuote(""));
                    content = appendCertContent(content, appendDoubleQuote(""));
                    content = appendCertContent(content, appendDoubleQuote(""));
                    content = appendCertContent(content, appendDoubleQuote(""));
                    content = appendCertContent(content, appendDoubleQuote(""));
                    content = appendCertContent(content, appendDoubleQuote(""));
                    content = appendCertContent(content, appendDoubleQuote(""));
                }

                content = appendCertContent(content, appendDoubleQuote(rptIssueDt));

                if (applicant != null)
                {
                    content = appendCertContent(content, appendDoubleQuote(applicant.CHINESE_NAME));
                }

                if (!(RegistrationConstant.S_CATEGORY_CODE_MWC.Equals(catCode)
                    || RegistrationConstant.S_CATEGORY_CODE_MWC_W.Equals(catCode)
                    || RegistrationConstant.S_CATEGORY_CODE_MWC_P.Equals(catCode)))
                {
                    if (RegistrationConstant.S_CATEGORY_CODE_RSE.Equals(catCode))
                    {
                        content = appendCertContent(content, appendDoubleQuote(indApplication.FILE_REFERENCE_NO + "(SE)"));
                    }
                    else if (RegistrationConstant.S_CATEGORY_CODE_RGE.Equals(catCode))
                    {
                        content = appendCertContent(content, appendDoubleQuote(indApplication.FILE_REFERENCE_NO + "(GE)"));
                    }
                    else if (RegistrationConstant.S_CATEGORY_GROUP_RI.Equals(catCode))
                    {
                        content = appendCertContent(content, appendDoubleQuote(indApplication.FILE_REFERENCE_NO));

                        String prbQuali = "";
                        List<C_IND_QUALIFICATION> indQualList = getIndQualificationListByMasterId(indApplication.UUID);
                        if (indQualList != null)
                        {
                            for (int i = 0; i < indQualList.Count(); i++)
                            {
                                C_IND_QUALIFICATION indQual = indQualList.ElementAt(i);

                                if (i != 0) { prbQuali += ", "; }

                                C_S_SYSTEM_VALUE svPrb = getSSystemValueByUuid(indQual.PRB_ID);
                                if (svPrb != null)
                                {
                                    prbQuali += svPrb.CODE;
                                    prbQuali += RegistrationConstant.COLON;
                                }

                                C_S_CATEGORY_CODE sCatCodeTmp = getSCategoryCodeByUuid(indQual.CATEGORY_ID);
                                if (sCatCodeTmp != null)
                                {
                                    prbQuali += sCatCodeTmp.CODE;
                                }

                                // get qualification details
                                List<C_IND_QUALIFICATION_DETAIL> indQualDetailsList = getIndQualificationDetailsListByIndQualUuid(indQual.UUID);

                                for (int k = 0; k < indQualDetailsList.Count(); k++)
                                {
                                    C_IND_QUALIFICATION_DETAIL indQualDetail = indQualDetailsList.ElementAt(k);
                                    if (k == 0)
                                    {
                                        prbQuali += RegistrationConstant.ARROW;
                                    }
                                    else
                                    {
                                        prbQuali += "/";
                                    }

                                    C_S_CATEGORY_CODE_DETAIL sCategoryCodeDetail = getSCategoryCodeDetaiByUuid(indQualDetail.S_CATEGORY_CODE_DETAIL_ID);
                                    prbQuali += sCategoryCodeDetail != null ? sCategoryCodeDetail.ENGLISH_DESCRIPTION : "";
                                }
                            }
                        }
                        content = appendCertContent(content, appendDoubleQuote(prbQuali.Trim()));
                    }
                    else
                    {
                        content = appendCertContent(content, appendDoubleQuote(indApplication.FILE_REFERENCE_NO));
                    }
                }
                else
                {
                    if (RegistrationConstant.S_CATEGORY_CODE_RSE.Equals(catCode)) {
                        content = appendCertContent(content, appendDoubleQuote(indApplication.FILE_REFERENCE_NO + "(SE)"));
                    }
                    else if (RegistrationConstant.S_CATEGORY_CODE_RGE.Equals(catCode)) {
                        content = appendCertContent(content, appendDoubleQuote(indApplication.FILE_REFERENCE_NO + "(GE)"));
                    }
                    else
                    {
                        content = appendCertContent(content, appendDoubleQuote(indApplication.FILE_REFERENCE_NO));
                    }
                }
            }

            // Generate new sequence number;
            if (RegistrationConstant.S_CATEGORY_CODE_MWC.Equals(catCode)
                    || RegistrationConstant.S_CATEGORY_CODE_MWC_P.Equals(catCode)
                    || RegistrationConstant.S_CATEGORY_CODE_MWC_W.Equals(catCode))
            {
                String newSequenceNo = getCertSeqNo(catCode, "Y");
                content = appendCertContent(content, appendDoubleQuote(newSequenceNo.ToString()));
            }

            if (RegistrationConstant.S_CATEGORY_GROUP_RI.Equals(catCode)) {
                String newSequenceNo = getCertSeqNo(catCode, "Y");
                content += SEPARATOR + appendDoubleQuote(newSequenceNo.ToString());
            }

            if (RegistrationConstant.S_CATEGORY_CODE_GBC.Equals(catCode))
            {
                String newSequenceNo = getCertSeqNo(catCode, "Y");
                content = appendCertContent(content, appendDoubleQuote(newSequenceNo.ToString()));
                AuditLogService.logDebug("SequenceNumber: " + newSequenceNo + ", Code:" + catCode);

            }

            // get list of mwItems
            String tabChar = "\t";
            String mwItemsStr = "";
            List<C_S_SYSTEM_VALUE> mwItems = getIndApplicationMwItemByIndAppUuid(indApplication.UUID);
            for (int i = 0; i < mwItems.Count(); i++)
            {
                if (i == 0) { mwItemsStr += SEPARATOR; }
                if ((i % 7 == 0) && (i != (mwItems.Count() - 1)))
                {
                    mwItemsStr += DOUBLEQUOTE + mwItems.ElementAt(i).CODE.Substring(5) + tabChar;
                }
                if ((i % 7 > 0) && (i % 7 < 6) && (i != (mwItems.Count() - 1)))
                {
                    mwItemsStr += mwItems.ElementAt(i).CODE.Substring(5) + tabChar;
                }
                if ((i % 7 == 6) && (i != (mwItems.Count() - 1)))
                {
                    mwItemsStr += mwItems.ElementAt(i).CODE.Substring(5) + DOUBLEQUOTE + SEPARATOR;
                }
                if ((i % 7 == 0) && (i == (mwItems.Count() - 1)))
                {
                    mwItemsStr += DOUBLEQUOTE;
                }
                if (i == (mwItems.Count() - 1))
                {
                    mwItemsStr += mwItems.ElementAt(i).CODE.Substring(5) + DOUBLEQUOTE;
                }
            }

            if (mwItems.Count() <= 35)
            {
                mwItemsStr += SEPARATOR;
                //add blank items
                int groups = (mwItems.Count()) / 7;
                int lastGroupSize = (mwItems.Count()) % 7;
                if (groups == 5)
                {
                    mwItemsStr += DOUBLEQUOTE + DOUBLEQUOTE;
                }
                else if (groups < 5)
                {
                    if (lastGroupSize == 0)
                    {
                        mwItemsStr += DOUBLEQUOTE + DOUBLEQUOTE + SEPARATOR;
                    }
                    int emptyGroups = 1;
                    while (emptyGroups < (5 - groups))
                    {
                        mwItemsStr += DOUBLEQUOTE + DOUBLEQUOTE + SEPARATOR;
                        emptyGroups++;
                    }
                    mwItemsStr += DOUBLEQUOTE + DOUBLEQUOTE;
                }
            }
            content += mwItemsStr;

            return content;
        }
        
        // Common function export QP Card for Prof & MWC(W) 
        public String populateProfExportQPCard(String process, String indCertUuid, String rptIssueDt, String rptRcvdDt, String signature)
        {
            int intProcess = int.Parse(process);
            String content = "";
            C_S_CATEGORY_CODE sCategoryCode = null;
            C_IND_APPLICATION indApplication = null;
            C_APPLICANT applicant = null;
            

            String catCode = "";

            // get ind_certificate by uuid
            C_IND_CERTIFICATE indCertificate = getIndCertificateByUuid(indCertUuid);

            if (indCertificate != null)
            {
                //define header
                List<string> certHeaderList = null;

                sCategoryCode = getSCategoryCodeByUuid(indCertificate.CATEGORY_ID);
                if (sCategoryCode != null)
                {
                    // get catGrp
                    //C_S_SYSTEM_VALUE catGrp = getSSystemValueByUuid(sCategoryCode.CATEGORY_GROUP_ID);
                    C_S_CATEGORY_CODE catGrp = getSCategoryCodeByUuid(indCertificate.CATEGORY_ID);

                    if (catGrp != null) { catCode = catGrp.CODE; }
                }

                if (catCode != null)
                {
                    if (RegistrationConstant.S_CATEGORY_CODE_MWC.Equals(catCode)
                        || RegistrationConstant.S_CATEGORY_CODE_MWC_P.Equals(catCode)
                        || RegistrationConstant.S_CATEGORY_CODE_MWC_W.Equals(catCode))
                    {
                        certHeaderList = new List<string>()
                        { "file_ref","reg_no",
                            "title","surname","given_name","given_name_id","cname","id_no","sex","salutation",
                            "gazet_dt","reg_dt","expiry_dt","removal_dt","restore_dt","approval_dt",
                            "addr1","addr2","addr3","addr4","addr5",
                            "caddr1","caddr2","caddr3","caddr4","caddr5",
                            "c_o",
                            "tel_no1","fax_no1",
                            "cat_code","ctg_code","cat_grp","cat_cgrp","ctr_sub_reg","ctr_sub_creg",
                            "application_date_of_QP_Card","issue_date_of_QP_Card","expiry_date_of_QP_Card","serial_no_of_QP_Card","return_date_of_QP_Card",
                            "QPaddr1","QPaddr2","QPaddr3","QPaddr4","QPaddr5",
                            "QPcaddr1","QPcaddr2","QPcaddr3","QPcaddr4","QPcaddr5",
                            "auth_name","auth_cname","auth_rank","auth_crank","auth_tel","auth_fax","acting",
                            "issue_dt","cert_cname","letter_file_ref", "sn","itemline1","itemline2","itemline3","itemline4","itemline5","itemline6"
                        };

                        // print header
                        foreach (String certHeader in certHeaderList)
                        {
                            content = appendCertContent(content, certHeader);
                        }
                        content += "\r\n";
                    }
                    else
                    {
                        certHeaderList = new List<string>()
                        { "file_ref","reg_no",
                            "title","surname","given_name","given_name_id","cname","id_no","sex","salutation",
                            "gazet_dt","reg_dt","expiry_dt","removal_dt","restore_dt","approval_dt",
                            "addr1","addr2","addr3","addr4","addr5",
                            "caddr1","caddr2","caddr3","caddr4","caddr5",
                            "c_o",
                            "tel_no1","fax_no1",
                            "cat_code","ctg_code","cat_grp","cat_cgrp","ctr_sub_reg","ctr_sub_creg",
                            "application_date_of_QP_Card","issue_date_of_QP_Card","expiry_date_of_QP_Card","serial_no_of_QP_Card","return_date_of_QP_Card",
                            "QPaddr1","QPaddr2","QPaddr3","QPaddr4","QPaddr5",
                            "QPcaddr1","QPcaddr2","QPcaddr3","QPcaddr4","QPcaddr5",
                            "auth_name","auth_cname","auth_rank","auth_crank","auth_tel","auth_fax","acting",
                            "issue_dt","cert_cname","letter_file_ref"
                        };

                        if (RegistrationConstant.S_CATEGORY_GROUP_RI.Equals(catCode))
                        {
                            certHeaderList = new List<string>()
                            {   "file_ref","reg_no",
                                "title","surname","given_name","given_name_id","cname","id_no","sex","salutation",
                                "gazet_dt","reg_dt","expiry_dt","removal_dt","restore_dt","approval_dt",
                                "addr1","addr2","addr3","addr4","addr5",
                                "caddr1","caddr2","caddr3","caddr4","caddr5",
                                "c_o",
                                "tel_no1","fax_no1",
                                "cat_code","ctg_code","cat_grp","cat_cgrp","ctr_sub_reg","ctr_sub_creg",
                                "application_date_of_QP_Card","issue_date_of_QP_Card","expiry_date_of_QP_Card","serial_no_of_QP_Card","return_date_of_QP_Card",
                                "QPaddr1","QPaddr2","QPaddr3","QPaddr4","QPaddr5",
                                "QPcaddr1","QPcaddr2","QPcaddr3","QPcaddr4","QPcaddr5",
                                "auth_name","auth_cname","auth_rank","auth_crank","auth_tel","auth_fax","acting",
                                "issue_dt","cert_cname","letter_file_ref", "prb_cat_code_qualification", "sn"
                            };
                        }

                        // print header
                        foreach (String certHeader in certHeaderList)
                        {
                            content = appendCertContent(content, certHeader);
                        }
                        content += "\r\n";
                    }
                }
            }

            if (indCertificate != null)
            {
                indApplication = getIndApplicationByUuid(indCertificate.MASTER_ID);
                if (indApplication != null)
                {
                    content += appendDoubleQuote(indApplication.FILE_REFERENCE_NO);
                    content = appendCertContent(content, appendDoubleQuote(indCertificate.CERTIFICATION_NO));

                    applicant = getApplicantByUuid(indApplication.APPLICANT_ID);
                    if (applicant != null)
                    {
                        String decryptHKID = EncryptDecryptUtil.getDecryptHKID(applicant.HKID);
                        if (applicant.TITLE_ID != null)
                        {
                            // get sv Title
                            C_S_SYSTEM_VALUE svTitle = getSSystemValueByUuid(applicant.TITLE_ID);
                            if (svTitle != null)
                            {
                                content = appendCertContent(content, appendDoubleQuote(svTitle.ENGLISH_DESCRIPTION));
                            }
                        }
                        content = appendCertContent(content, appendDoubleQuote(applicant.SURNAME != null ? applicant.SURNAME.ToUpper() : ""));
                        content = appendCertContent(content, appendDoubleQuote(applicant.GIVEN_NAME_ON_ID));
                        content = appendCertContent(content, appendDoubleQuote(applicant.GIVEN_NAME_ON_ID));
                        content = appendCertContent(content, appendDoubleQuote(applicant.CHINESE_NAME));
                        content = appendCertContent(content, appendDoubleQuote(decryptHKID));
                        content = appendCertContent(content, appendDoubleQuote(applicant.GENDER));

                        if (applicant.GENDER != null)
                        {
                            if (RegistrationConstant.GENDER_M.Equals(applicant.GENDER))
                            {
                                content = appendCertContent(content, appendDoubleQuote("Sir"));
                            }
                            else if (RegistrationConstant.GENDER_F.Equals(applicant.GENDER))
                            {
                                content = appendCertContent(content, appendDoubleQuote("Madam"));
                            }
                            else
                            {
                                content = appendCertContent(content, appendDoubleQuote("Sir / Madam"));
                            }
                        }
                        content = appendCertContent(content, appendDoubleQuote(indCertificate.GAZETTE_DATE != null ? indCertificate.GAZETTE_DATE.ToString() : ""));
                        content = appendCertContent(content, appendDoubleQuote(indCertificate.REGISTRATION_DATE != null ? indCertificate.REGISTRATION_DATE.ToString() : ""));

                        if (RegistrationConstant.RETENTION.Equals(intProcess) || RegistrationConstant.RESTORATION.Equals(intProcess))
                        {
                            content = appendCertContent(content, appendDoubleQuote(indCertificate.EXTENDED_DATE != null ? indCertificate.EXTENDED_DATE.ToString() : ""));
                        }
                        else
                        {
                            if (RegistrationConstant.NEW_REG.Equals(intProcess))
                            {
                                content = appendCertContent(content, appendDoubleQuote(indCertificate.EXPIRY_DATE != null ? indCertificate.EXPIRY_DATE.ToString() : ""));
                            }
                            else
                            {
                                content = appendCertContent(content, appendDoubleQuote(indCertificate.EXPIRY_DATE != null ? indCertificate.EXPIRY_DATE.ToString() : ""));
                            }
                        }

                        content = appendCertContent(content, appendDoubleQuote(indCertificate.REMOVAL_DATE != null ? indCertificate.REMOVAL_DATE.ToString() : ""));
                        content = appendCertContent(content, appendDoubleQuote(indCertificate.RESTORE_DATE != null ? indCertificate.RESTORE_DATE.ToString() : ""));
                        content = appendCertContent(content, appendDoubleQuote(indCertificate.APPROVAL_DATE != null ? indCertificate.APPROVAL_DATE.ToString() : ""));
                    }

                    if ("H".Equals(indApplication.POST_TO) || "O".Equals(indApplication.POST_TO) || null == indApplication.POST_TO)
                    {
                        // get address
                        C_ADDRESS engAddress = getAddressByUuid(indApplication.ENGLISH_HOME_ADDRESS_ID);
                        C_ADDRESS chiAddress = getAddressByUuid(indApplication.CHINESE_OFFICE_ADDRESS_ID);

                        content = appendCertContent(content, appendDoubleQuote(engAddress.ADDRESS_LINE1));
                        content = appendCertContent(content, appendDoubleQuote(engAddress.ADDRESS_LINE2));
                        content = appendCertContent(content, appendDoubleQuote(engAddress.ADDRESS_LINE3));
                        content = appendCertContent(content, appendDoubleQuote(engAddress.ADDRESS_LINE4));
                        content = appendCertContent(content, appendDoubleQuote(engAddress.ADDRESS_LINE5));
                        content = appendCertContent(content, appendDoubleQuote(chiAddress.ADDRESS_LINE1));
                        content = appendCertContent(content, appendDoubleQuote(chiAddress.ADDRESS_LINE2));
                        content = appendCertContent(content, appendDoubleQuote(chiAddress.ADDRESS_LINE3));
                        content = appendCertContent(content, appendDoubleQuote(chiAddress.ADDRESS_LINE4));
                        content = appendCertContent(content, appendDoubleQuote(chiAddress.ADDRESS_LINE5));
                        content = appendCertContent(content, appendDoubleQuote(""));
                    }

                    content = appendCertContent(content, appendDoubleQuote(indApplication.TELEPHONE_NO1));
                    content = appendCertContent(content, appendDoubleQuote(indApplication.FAX_NO1));

                    content = appendCertContent(content, appendDoubleQuote(catCode));

                    // get sv
                    C_S_SYSTEM_VALUE svCatGrp = getSSystemValueByUuid(sCategoryCode.CATEGORY_GROUP_ID);
                    if (svCatGrp != null)
                    {
                        content = appendCertContent(content, appendDoubleQuote(svCatGrp.CODE));
                        content = appendCertContent(content, appendDoubleQuote(svCatGrp.ENGLISH_DESCRIPTION));
                        content = appendCertContent(content, appendDoubleQuote(svCatGrp.CHINESE_DESCRIPTION));
                        content = appendCertContent(content, appendDoubleQuote(sCategoryCode.ENGLISH_SUB_TITLE_DESCRIPTION));
                        content = appendCertContent(content, appendDoubleQuote(sCategoryCode.CHINESE_SUB_TITLE_DESCRIPTION));
                    }

                    C_S_CATEGORY_CODE sCatCode = getSCategoryCodeByUuid(indCertificate.CATEGORY_ID);
                }

                // Qp Card information


                // get authority
                C_S_AUTHORITY sAuthority = getAuthorityByUuid(signature);
                if (sAuthority != null)
                {
                    content = appendCertContent(content, appendDoubleQuote(sAuthority.ENGLISH_NAME));
                    content = appendCertContent(content, appendDoubleQuote(sAuthority.CHINESE_NAME));
                    content = appendCertContent(content, appendDoubleQuote(sAuthority.ENGLISH_RANK));
                    content = appendCertContent(content, appendDoubleQuote(sAuthority.CHINESE_RANK));
                    content = appendCertContent(content, appendDoubleQuote(sAuthority.TELEPHONE_NO));
                    content = appendCertContent(content, appendDoubleQuote(sAuthority.FAX_NO));
                    content = appendCertContent(content, appendDoubleQuote(sAuthority.ENGLISH_ACTION_NAME));
                }
                else
                {
                    content = appendCertContent(content, appendDoubleQuote(""));
                    content = appendCertContent(content, appendDoubleQuote(""));
                    content = appendCertContent(content, appendDoubleQuote(""));
                    content = appendCertContent(content, appendDoubleQuote(""));
                    content = appendCertContent(content, appendDoubleQuote(""));
                    content = appendCertContent(content, appendDoubleQuote(""));
                    content = appendCertContent(content, appendDoubleQuote(""));
                }

                content = appendCertContent(content, appendDoubleQuote(rptIssueDt));

                if (applicant != null)
                {
                    content = appendCertContent(content, appendDoubleQuote(applicant.CHINESE_NAME));
                }

                if (!(RegistrationConstant.S_CATEGORY_CODE_MWC.Equals(catCode)
                    || RegistrationConstant.S_CATEGORY_CODE_MWC_W.Equals(catCode)
                    || RegistrationConstant.S_CATEGORY_CODE_MWC_P.Equals(catCode)))
                {
                    if (RegistrationConstant.S_CATEGORY_CODE_RSE.Equals(catCode))
                    {
                        content = appendCertContent(content, appendDoubleQuote(indApplication.FILE_REFERENCE_NO + "(SE)"));
                    }
                    else if (RegistrationConstant.S_CATEGORY_CODE_RGE.Equals(catCode))
                    {
                        content = appendCertContent(content, appendDoubleQuote(indApplication.FILE_REFERENCE_NO + "(GE)"));
                    }
                    else if (RegistrationConstant.S_CATEGORY_GROUP_RI.Equals(catCode))
                    {
                        content = appendCertContent(content, appendDoubleQuote(indApplication.FILE_REFERENCE_NO));

                        String prbQuali = "";
                        List<C_IND_QUALIFICATION> indQualList = getIndQualificationListByMasterId(indApplication.UUID);
                        if (indQualList != null)
                        {
                            for (int i = 0; i < indQualList.Count(); i++)
                            {
                                C_IND_QUALIFICATION indQual = indQualList.ElementAt(i);

                                if (i != 0) { prbQuali += ", "; }

                                C_S_SYSTEM_VALUE svPrb = getSSystemValueByUuid(indQual.PRB_ID);
                                if (svPrb != null)
                                {
                                    prbQuali += svPrb.CODE;
                                    prbQuali += RegistrationConstant.COLON;
                                }

                                C_S_CATEGORY_CODE sCatCodeTmp = getSCategoryCodeByUuid(indQual.CATEGORY_ID);
                                if (sCatCodeTmp != null)
                                {
                                    prbQuali += sCatCodeTmp.CODE;
                                }

                                // get qualification details
                                List<C_IND_QUALIFICATION_DETAIL> indQualDetailsList = getIndQualificationDetailsListByIndQualUuid(indQual.UUID);

                                for (int k = 0; k < indQualDetailsList.Count(); k++)
                                {
                                    C_IND_QUALIFICATION_DETAIL indQualDetail = indQualDetailsList.ElementAt(k);
                                    if (k == 0)
                                    {
                                        prbQuali += RegistrationConstant.ARROW;
                                    }
                                    else
                                    {
                                        prbQuali += "/";
                                    }

                                    C_S_CATEGORY_CODE_DETAIL sCategoryCodeDetail = getSCategoryCodeDetaiByUuid(indQualDetail.S_CATEGORY_CODE_DETAIL_ID);
                                    prbQuali += sCategoryCodeDetail != null ? sCategoryCodeDetail.ENGLISH_DESCRIPTION : "";
                                }
                            }
                        }
                        content = appendCertContent(content, appendDoubleQuote(prbQuali.Trim()));
                    }
                    else
                    {
                        content = appendCertContent(content, appendDoubleQuote(indApplication.FILE_REFERENCE_NO));
                    }
                }
                else
                {
                    if (RegistrationConstant.S_CATEGORY_CODE_RSE.Equals(catCode))
                    {
                        content = appendCertContent(content, appendDoubleQuote(indApplication.FILE_REFERENCE_NO + "(SE)"));
                    }
                    else if (RegistrationConstant.S_CATEGORY_CODE_RGE.Equals(catCode))
                    {
                        content = appendCertContent(content, appendDoubleQuote(indApplication.FILE_REFERENCE_NO + "(GE)"));
                    }
                    else
                    {
                        content = appendCertContent(content, appendDoubleQuote(indApplication.FILE_REFERENCE_NO));
                    }
                }
            }

            // Generate new sequence number;
            if (RegistrationConstant.S_CATEGORY_CODE_MWC.Equals(catCode)
                    || RegistrationConstant.S_CATEGORY_CODE_MWC_P.Equals(catCode)
                    || RegistrationConstant.S_CATEGORY_CODE_MWC_W.Equals(catCode))
            {
                String newSequenceNo = getCertSeqNo(catCode, "Y");
                content = appendCertContent(content, appendDoubleQuote(newSequenceNo.ToString()));
            }

            if (RegistrationConstant.S_CATEGORY_GROUP_RI.Equals(catCode))
            {
                String newSequenceNo = getCertSeqNo(catCode, "Y");
                content += SEPARATOR + appendDoubleQuote(newSequenceNo.ToString());
            }

            // get list of mwItems
            String tabChar = "\t";
            String mwItemsStr = "";
            List<C_S_SYSTEM_VALUE> mwItems = getIndApplicationMwItemByIndAppUuid(indApplication.UUID);
            for (int i = 0; i < mwItems.Count(); i++)
            {
                if (i == 0) { mwItemsStr += SEPARATOR; }
                if ((i % 7 == 0) && (i != (mwItems.Count() - 1)))
                {
                    mwItemsStr += DOUBLEQUOTE + mwItems.ElementAt(i).CODE.Substring(5) + tabChar;
                }
                if ((i % 7 > 0) && (i % 7 < 6) && (i != (mwItems.Count() - 1)))
                {
                    mwItemsStr += mwItems.ElementAt(i).CODE.Substring(5) + tabChar;
                }
                if ((i % 7 == 6) && (i != (mwItems.Count() - 1)))
                {
                    mwItemsStr += mwItems.ElementAt(i).CODE.Substring(5) + DOUBLEQUOTE + SEPARATOR;
                }
                if ((i % 7 == 0) && (i == (mwItems.Count() - 1)))
                {
                    mwItemsStr += DOUBLEQUOTE;
                }
                if (i == (mwItems.Count() - 1))
                {
                    mwItemsStr += mwItems.ElementAt(i).CODE.Substring(5) + DOUBLEQUOTE;
                }
            }

            if (mwItems.Count() <= 35)
            {
                mwItemsStr += SEPARATOR;
                //add blank items
                int groups = (mwItems.Count()) / 7;
                int lastGroupSize = (mwItems.Count()) % 7;
                if (groups == 5)
                {
                    mwItemsStr += DOUBLEQUOTE + DOUBLEQUOTE;
                }
                else if (groups < 5)
                {
                    if (lastGroupSize == 0)
                    {
                        mwItemsStr += DOUBLEQUOTE + DOUBLEQUOTE + SEPARATOR;
                    }
                    int emptyGroups = 1;
                    while (emptyGroups < (5 - groups))
                    {
                        mwItemsStr += DOUBLEQUOTE + DOUBLEQUOTE + SEPARATOR;
                        emptyGroups++;
                    }
                    mwItemsStr += DOUBLEQUOTE + DOUBLEQUOTE;
                }
            }
            content += mwItemsStr;

            return content;
        }

        // Common function export Letter for Prof & MWC(W)
        public String populateProfExportLetter(String process, String indCertUuid, String rptIssueDt, String rptRcvdDt, String signature)
        {
            int intProcess = int.Parse(process);
            String content = "";
            C_S_CATEGORY_CODE sCategoryCode = null;
            C_IND_APPLICATION indApplication = null;
            C_APPLICANT applicant = null;
            C_S_SYSTEM_VALUE svTitle = null;

            // get ind_certificate by uuid
            C_IND_CERTIFICATE indCertificate = getIndCertificateByUuid(indCertUuid);

            // get indApplication
            if (indCertificate != null && indCertificate.MASTER_ID != null)
            {
                indApplication = getIndApplicationByUuid(indCertificate.MASTER_ID);
            }

            // get applicant
            if (indApplication != null)
            {
                applicant = getApplicantByUuid(indApplication.APPLICANT_ID);
            }

            // get sv by applicant
            if (applicant != null) {
                svTitle = getSSystemValueByUuid(applicant.TITLE_ID);
            }

            // get catCode
            String catCode = "";
            if (indCertificate != null)
            {
                sCategoryCode = getSCategoryCodeByUuid(indCertificate.CATEGORY_ID);
                if (sCategoryCode != null)
                {
                    // get catGrp
                    //C_S_SYSTEM_VALUE catGrp = getSSystemValueByUuid(sCategoryCode.CATEGORY_GROUP_ID);
                    C_S_CATEGORY_CODE catGrp = getSCategoryCodeByUuid(indCertificate.CATEGORY_ID);
                    if (catGrp != null) { catCode = catGrp.CODE; }
                }
            }

            // get authority
            C_S_AUTHORITY sAuthority = getAuthorityByUuid(signature);

            if (indCertificate != null)
            {
                //define header
                List<string> certHeaderList = null;
                certHeaderList = new List<string>()
                { "file_ref","reg_no",
                    "title","surname","given_name","given_name_id","cname","id_no","sex","salutation",
                    "gazet_dt","reg_dt","expiry_dt","removal_dt","restore_dt","approval_dt",
                    "addr1","addr2","addr3","addr4","addr5",
                    "caddr1","caddr2","caddr3","caddr4","caddr5",
                    "c_o",
                    "tel_no1","fax_no1",
                    "cat_code","ctg_code","cat_grp","cat_cgrp","ctr_sub_reg","ctr_sub_creg",
                    "auth_name","auth_cname","auth_rank","auth_crank","auth_tel","auth_fax","acting",
                    "issue_dt","cert_cname","letter_file_ref"
                };

                // print header
                foreach (String certHeader in certHeaderList)
                {
                    content = appendCertContent(content, certHeader);
                }
                if (RegistrationConstant.S_CATEGORY_CODE_MWC_W.Equals(catCode))
                {
                    content = appendCertContent(content, "itemline1");
                    content = appendCertContent(content, "itemline2");
                    content = appendCertContent(content, "itemline3");
                    content = appendCertContent(content, "itemline4");
                    content = appendCertContent(content, "itemline5");
                    content = appendCertContent(content, "itemline6");
                }

                // new line after print header
                content += "\r\n";

                if (indApplication != null)
                {
                    content += appendDoubleQuote(indApplication.FILE_REFERENCE_NO);
                    if (indCertificate != null)
                    {
                        content = appendCertContent(content, appendDoubleQuote(indCertificate.CERTIFICATION_NO));
                    }
                    content = appendCertContent(content, appendDoubleQuote(svTitle != null ? svTitle.ENGLISH_DESCRIPTION : ""));
                    content = appendCertContent(content, appendDoubleQuote(applicant != null ? applicant.SURNAME : ""));
                    content = appendCertContent(content, appendDoubleQuote(applicant != null ? applicant.GIVEN_NAME_ON_ID : ""));
                    content = appendCertContent(content, appendDoubleQuote(applicant != null ? applicant.GIVEN_NAME_ON_ID : ""));
                    content = appendCertContent(content, appendDoubleQuote(applicant != null ? applicant.CHINESE_NAME : ""));
                    //content = appendCertContent(content, appendDoubleQuote(applicant != null ? applicant.HKID : ""));
                    String decryptHKID = applicant != null ? EncryptDecryptUtil.getDecryptHKID(applicant.HKID) : "";
                    content = appendCertContent(content, appendDoubleQuote(decryptHKID));


                    if (applicant.GENDER != null)
                    {
                        if (RegistrationConstant.GENDER_M.Equals(applicant.GENDER))
                        {
                            content = appendCertContent(content, appendDoubleQuote("Sir"));
                        }
                        else if (RegistrationConstant.GENDER_F.Equals(applicant.GENDER))
                        {
                            content = appendCertContent(content, appendDoubleQuote("Madam"));
                        }
                        else
                        {
                            content = appendCertContent(content, appendDoubleQuote("Sir / Madam"));
                        }
                    }

                    if (indCertificate != null)
                    {
                        content = appendCertContent(content, appendDoubleQuote(indCertificate.GAZETTE_DATE != null ? indCertificate.GAZETTE_DATE.ToString() : ""));
                        content = appendCertContent(content, appendDoubleQuote(indCertificate.REGISTRATION_DATE != null ? indCertificate.REGISTRATION_DATE.ToString() : ""));
                    }
                    else
                    {
                        content = appendCertContent(content, appendDoubleQuote(""));
                    }


                    if (RegistrationConstant.RETENTION == intProcess || RegistrationConstant.RESTORATION == intProcess)
                    {
                        if (indCertificate != null)
                        {
                            content = appendCertContent(content, appendDoubleQuote(indCertificate.EXPIRY_DATE != null ? indCertificate.EXTENDED_DATE.ToString() : ""));
                        }
                    }
                    else
                    {
                        if (indCertificate != null)
                        {
                            content = appendCertContent(content, appendDoubleQuote(indCertificate.EXPIRY_DATE != null ? indCertificate.EXPIRY_DATE.ToString() : ""));
                        }
                    }

                    if (indCertificate != null)
                    {
                        content = appendCertContent(content, appendDoubleQuote(indCertificate.REMOVAL_DATE != null ? indCertificate.REMOVAL_DATE.ToString() : ""));
                        content = appendCertContent(content, appendDoubleQuote(indCertificate.RESTORE_DATE != null ? indCertificate.RESTORE_DATE.ToString() : ""));
                        content = appendCertContent(content, appendDoubleQuote(indCertificate.APPROVAL_DATE != null ? indCertificate.APPROVAL_DATE.ToString() : ""));
                    }

                    if ("H".Equals(indApplication.POST_TO) || "O".Equals(indApplication.POST_TO) || null == indApplication.POST_TO)
                    {
                        // get address
                        C_ADDRESS engAddress = getAddressByUuid(indApplication.ENGLISH_HOME_ADDRESS_ID);
                        C_ADDRESS chiAddress = getAddressByUuid(indApplication.CHINESE_OFFICE_ADDRESS_ID);

                        content = appendCertContent(content, appendDoubleQuote(engAddress.ADDRESS_LINE1));
                        content = appendCertContent(content, appendDoubleQuote(engAddress.ADDRESS_LINE2));
                        content = appendCertContent(content, appendDoubleQuote(engAddress.ADDRESS_LINE3));
                        content = appendCertContent(content, appendDoubleQuote(engAddress.ADDRESS_LINE4));
                        content = appendCertContent(content, appendDoubleQuote(engAddress.ADDRESS_LINE5));
                        content = appendCertContent(content, appendDoubleQuote(chiAddress.ADDRESS_LINE1));
                        content = appendCertContent(content, appendDoubleQuote(chiAddress.ADDRESS_LINE2));
                        content = appendCertContent(content, appendDoubleQuote(chiAddress.ADDRESS_LINE3));
                        content = appendCertContent(content, appendDoubleQuote(chiAddress.ADDRESS_LINE4));
                        content = appendCertContent(content, appendDoubleQuote(chiAddress.ADDRESS_LINE5));
                        content = appendCertContent(content, appendDoubleQuote(""));
                    }

                    content = appendCertContent(content, appendDoubleQuote(indApplication.TELEPHONE_NO1));
                    content = appendCertContent(content, appendDoubleQuote(indApplication.FAX_NO1));

                    content = appendCertContent(content, appendDoubleQuote(catCode));

                    // get sv
                    C_S_SYSTEM_VALUE svCatGrp = getSSystemValueByUuid(sCategoryCode.CATEGORY_GROUP_ID);
                    if (svCatGrp != null)
                    {
                        content = appendCertContent(content, appendDoubleQuote(svCatGrp.CODE));
                        content = appendCertContent(content, appendDoubleQuote(svCatGrp.ENGLISH_DESCRIPTION));
                        content = appendCertContent(content, appendDoubleQuote(svCatGrp.CHINESE_DESCRIPTION));
                        content = appendCertContent(content, appendDoubleQuote(sCategoryCode.ENGLISH_SUB_TITLE_DESCRIPTION));
                        content = appendCertContent(content, appendDoubleQuote(sCategoryCode.CHINESE_SUB_TITLE_DESCRIPTION));
                    }
                }

                // print sAuthority
                if (sAuthority != null)
                {
                    content = appendCertContent(content, appendDoubleQuote(sAuthority.ENGLISH_NAME));
                    content = appendCertContent(content, appendDoubleQuote(sAuthority.CHINESE_NAME));
                    content = appendCertContent(content, appendDoubleQuote(sAuthority.ENGLISH_RANK));
                    content = appendCertContent(content, appendDoubleQuote(sAuthority.CHINESE_RANK));
                    content = appendCertContent(content, appendDoubleQuote(sAuthority.TELEPHONE_NO));
                    content = appendCertContent(content, appendDoubleQuote(sAuthority.FAX_NO));
                    content = appendCertContent(content, appendDoubleQuote(sAuthority.ENGLISH_ACTION_NAME));
                }
                else
                {
                    content = appendCertContent(content, appendDoubleQuote(""));
                    content = appendCertContent(content, appendDoubleQuote(""));
                    content = appendCertContent(content, appendDoubleQuote(""));
                    content = appendCertContent(content, appendDoubleQuote(""));
                    content = appendCertContent(content, appendDoubleQuote(""));
                    content = appendCertContent(content, appendDoubleQuote(""));
                    content = appendCertContent(content, appendDoubleQuote(""));
                }

                content = appendCertContent(content, appendDoubleQuote(rptIssueDt));

                if (applicant != null)
                {
                    content = appendCertContent(content, appendDoubleQuote(applicant.CHINESE_NAME));
                }

                if (RegistrationConstant.S_CATEGORY_CODE_RSE.Equals(catCode))
                {
                    content = appendCertContent(content, appendDoubleQuote(indApplication.FILE_REFERENCE_NO + "(SE)"));
                }
                else if (RegistrationConstant.S_CATEGORY_CODE_RGE.Equals(catCode))
                {
                    content = appendCertContent(content, appendDoubleQuote(indApplication.FILE_REFERENCE_NO + "(GE)"));
                }
                else
                {
                    content = appendCertContent(content, appendDoubleQuote(indApplication.FILE_REFERENCE_NO));
                }

                String tabChar = "\t";
                String mwItemsStr = "";
                List<String> mwItems = new List<string>();
                if (RegistrationConstant.S_CATEGORY_CODE_MWC_W.Equals(catCode))
                {
                    List<C_S_SYSTEM_VALUE> detailList = getMwItemDetailByApplication(indApplication.UUID, intProcess);
                    for (int i = 0; i < detailList.Count(); i++)
                    {
                        C_S_SYSTEM_VALUE sv = detailList.ElementAt(i);
                        mwItems.Add(sv.CODE);
                    }
                }

               

                if (mwItems.Count() > 0)
                {
                    mwItemsStr += SEPARATOR;
                    for (int i = 0; i < mwItems.Count(); i++)
                    {
                        if (i == 0) { mwItemsStr += SEPARATOR; }
                        String mwItem = mwItems.ElementAt(i);
                        if ((i % 7 == 0) && (i != (mwItems.Count() - 1)))
                        {
                            mwItemsStr += DOUBLEQUOTE + mwItem + tabChar;
                        }
                        if ((i % 7 > 0) && (i % 7 < 6) && (i != (mwItems.Count() - 1)))
                        {
                            mwItemsStr += mwItem + tabChar;
                        }
                        if ((i % 7 == 6) && (i != (mwItems.Count() - 1)))
                        {
                            mwItemsStr += mwItem + DOUBLEQUOTE + SEPARATOR;
                        }
                        if ((i % 7 == 0) && (i == (mwItems.Count() - 1)))
                        {
                            mwItemsStr += DOUBLEQUOTE;
                        }
                        if (i == (mwItems.Count() - 1))
                        {
                            mwItemsStr += mwItem + DOUBLEQUOTE;
                        }
                    }

                    if (mwItems.Count() <= 35)
                    {
                        mwItemsStr += SEPARATOR;
                        //add blank items
                        int groups = (mwItems.Count()) / 7;
                        int lastGroupSize = (mwItems.Count()) % 7;
                        if (groups == 5)
                        {
                            mwItemsStr += DOUBLEQUOTE + DOUBLEQUOTE;
                        }
                        else if (groups < 5)
                        {
                            if (lastGroupSize == 0)
                            {
                                mwItemsStr += DOUBLEQUOTE + DOUBLEQUOTE + SEPARATOR;
                            }
                            int emptyGroups = 1;
                            while (emptyGroups < (5 - groups))
                            {
                                mwItemsStr += DOUBLEQUOTE + DOUBLEQUOTE + SEPARATOR;
                                emptyGroups++;
                            }
                            mwItemsStr += DOUBLEQUOTE + DOUBLEQUOTE;
                        }
                    }
                }
                else
                {
                    if (RegistrationConstant.S_CATEGORY_CODE_MWC_W.Equals(catCode))
                    {
                        mwItemsStr += SEPARATOR;
                        mwItemsStr += DOUBLEQUOTE + DOUBLEQUOTE + SEPARATOR;
                        mwItemsStr += DOUBLEQUOTE + DOUBLEQUOTE + SEPARATOR;
                        mwItemsStr += DOUBLEQUOTE + DOUBLEQUOTE + SEPARATOR;
                        mwItemsStr += DOUBLEQUOTE + DOUBLEQUOTE + SEPARATOR;
                        mwItemsStr += DOUBLEQUOTE + DOUBLEQUOTE;
                    }
                }
                content += mwItemsStr;
            }

            return content;
        }

        // Common function export Letter with code for Prof & MWC(W)
        public String populateProfExportLetterWithCode(String process, String certNo, String indCertUuid, String rptIssueDt, String rptRcvdDt, String signature)
        {
            int intProcess = int.Parse(process);
            String content = "";
            C_S_CATEGORY_CODE sCategoryCode = null;
            C_IND_APPLICATION indApplication = null;
            C_APPLICANT applicant = null;
            C_S_SYSTEM_VALUE svTitle = null;

            // get ind_certificate by uuid
            C_IND_CERTIFICATE indCertificate = getIndCertificateByUuid(indCertUuid);

            // get indApplication
            if (indCertificate != null && indCertificate.MASTER_ID != null)
            {
                indApplication = getIndApplicationByUuid(indCertificate.MASTER_ID);
            }

            // get applicant
            if (indApplication != null)
            {
                applicant = getApplicantByUuid(indApplication.APPLICANT_ID);
            }

            // get sv by applicant
            if (applicant != null)
            {
                svTitle = getSSystemValueByUuid(applicant.TITLE_ID);
            }

            // get catCode
            String catCode = "";
            if (indCertificate != null)
            {
                sCategoryCode = getSCategoryCodeByUuid(indCertificate.CATEGORY_ID);
                if (sCategoryCode != null)
                {
                    // get catGrp
                    C_S_SYSTEM_VALUE catGrp = getSSystemValueByUuid(sCategoryCode.CATEGORY_GROUP_ID);
                    if (catGrp != null) { catCode = catGrp.CODE; }
                }
            }

            // get authority
            C_S_AUTHORITY sAuthority = getAuthorityByUuid(signature);

            //define header
            List<string> certHeaderList = null;

            if (RegistrationConstant.LETTER_MMD0008A.Equals(certNo)
                || RegistrationConstant.LETTER_MMD0009A.Equals(certNo)
                || RegistrationConstant.LETTER_MMD0007A_1.Equals(certNo)
                || RegistrationConstant.LETTER_MMD0007B_1.Equals(certNo)
                || RegistrationConstant.LETTER_MMD0007C_1.Equals(certNo))
            {
                certHeaderList = new List<string>()
                { "file_ref","reg_no",
                    "title","surname","given_name","given_name_id","cname","id_no","sex","salutation",
                    "gazet_dt","reg_dt","expiry_dt","removal_dt","restore_dt","approval_dt",
                    "addr1","addr2","addr3","addr4","addr5",
                    "caddr1","caddr2","caddr3","caddr4","caddr5",
                    "c_o",
                    "tel_no1","fax_no1",
                    "cat_code","ctg_code","cat_grp","cat_cgrp","ctr_sub_reg","ctr_sub_creg",
                    "auth_name","auth_cname","auth_rank","auth_crank","auth_tel","auth_fax","acting",
                    "issue_dt","cert_cname","letter_file_ref"
                };

                // print header
                foreach (String certHeader in certHeaderList)
                {
                    content = appendCertContent(content, certHeader);
                }
                if (RegistrationConstant.S_CATEGORY_CODE_MWC_W.Equals(catCode))
                {
                    if (RegistrationConstant.LETTER_MMD0008A.Equals(certNo)
                        || RegistrationConstant.LETTER_MMD0009A.Equals(certNo))
                    {
                        content = appendCertContent(content, "itemline1");
                        content = appendCertContent(content, "itemline2");
                        content = appendCertContent(content, "itemline3");
                        content = appendCertContent(content, "itemline4");
                        content = appendCertContent(content, "itemline5");
                        content = appendCertContent(content, "itemline6");
                    }
                }
                // print new line after print header
                content += "\r\n";

                if (indCertificate != null)
                {
                    content += appendDoubleQuote(indApplication.FILE_REFERENCE_NO);
                    if (indCertificate != null)
                    {
                        content = appendCertContent(content, appendDoubleQuote(indCertificate.CERTIFICATION_NO));
                    }
                    content = appendCertContent(content, appendDoubleQuote(svTitle != null ? svTitle.ENGLISH_DESCRIPTION : ""));
                    content = appendCertContent(content, appendDoubleQuote(applicant != null ? applicant.SURNAME : ""));
                    content = appendCertContent(content, appendDoubleQuote(applicant != null ? applicant.GIVEN_NAME_ON_ID : ""));
                    content = appendCertContent(content, appendDoubleQuote(applicant != null ? applicant.GIVEN_NAME_ON_ID : ""));
                    content = appendCertContent(content, appendDoubleQuote(applicant != null ? applicant.CHINESE_NAME : ""));
                    String decryptHKID = applicant != null ? EncryptDecryptUtil.getDecryptHKID(applicant.HKID) : "";
                    content = appendCertContent(content, appendDoubleQuote(decryptHKID));
                }

                if (applicant != null && applicant.GENDER != null)
                {
                    if (RegistrationConstant.GENDER_M.Equals(applicant.GENDER))
                    {
                        content = appendCertContent(content, appendDoubleQuote("Sir"));
                    }
                    else if (RegistrationConstant.GENDER_F.Equals(applicant.GENDER))
                    {
                        content = appendCertContent(content, appendDoubleQuote("Madam"));
                    }
                    else
                    {
                        content = appendCertContent(content, appendDoubleQuote("Sir / Madam"));
                    }
                }

                if (indCertificate != null)
                {
                    content = appendCertContent(content, appendDoubleQuote(indCertificate.GAZETTE_DATE != null ? indCertificate.GAZETTE_DATE.ToString() : ""));
                    content = appendCertContent(content, appendDoubleQuote(indCertificate.REGISTRATION_DATE != null ? indCertificate.REGISTRATION_DATE.ToString() : ""));
                }
                else
                {
                    content = appendCertContent(content, appendDoubleQuote(""));
                }

                if (indApplication != null)
                {
                    if (RegistrationConstant.RETENTION == intProcess || RegistrationConstant.RESTORATION == intProcess)
                    {
                        if (indCertificate != null)
                        {
                            content = appendCertContent(content, appendDoubleQuote(indCertificate.EXPIRY_DATE != null ? indCertificate.EXTENDED_DATE.ToString() : ""));
                        }
                    }
                    else
                    {
                        if (indCertificate != null)
                        {
                            content = appendCertContent(content, appendDoubleQuote(indCertificate.EXPIRY_DATE != null ? indCertificate.EXPIRY_DATE.ToString() : ""));
                        }
                    }

                    content = appendCertContent(content, appendDoubleQuote(indCertificate.REMOVAL_DATE != null ? indCertificate.REMOVAL_DATE.ToString() : ""));
                    content = appendCertContent(content, appendDoubleQuote(indCertificate.RESTORE_DATE != null ? indCertificate.RESTORE_DATE.ToString() : ""));
                    content = appendCertContent(content, appendDoubleQuote(indCertificate.APPROVAL_DATE != null ? indCertificate.APPROVAL_DATE.ToString() : ""));

                    if ("H".Equals(indApplication.POST_TO) || "O".Equals(indApplication.POST_TO) || null == indApplication.POST_TO)
                    {
                        // get address
                        C_ADDRESS engAddress = getAddressByUuid(indApplication.ENGLISH_HOME_ADDRESS_ID);
                        C_ADDRESS chiAddress = getAddressByUuid(indApplication.CHINESE_OFFICE_ADDRESS_ID);

                        content = appendCertContent(content, appendDoubleQuote(engAddress.ADDRESS_LINE1));
                        content = appendCertContent(content, appendDoubleQuote(engAddress.ADDRESS_LINE2));
                        content = appendCertContent(content, appendDoubleQuote(engAddress.ADDRESS_LINE3));
                        content = appendCertContent(content, appendDoubleQuote(engAddress.ADDRESS_LINE4));
                        content = appendCertContent(content, appendDoubleQuote(engAddress.ADDRESS_LINE5));
                        content = appendCertContent(content, appendDoubleQuote(chiAddress.ADDRESS_LINE1));
                        content = appendCertContent(content, appendDoubleQuote(chiAddress.ADDRESS_LINE2));
                        content = appendCertContent(content, appendDoubleQuote(chiAddress.ADDRESS_LINE3));
                        content = appendCertContent(content, appendDoubleQuote(chiAddress.ADDRESS_LINE4));
                        content = appendCertContent(content, appendDoubleQuote(chiAddress.ADDRESS_LINE5));
                        content = appendCertContent(content, appendDoubleQuote(""));
                    }

                    content = appendCertContent(content, appendDoubleQuote(indApplication.TELEPHONE_NO1));
                    content = appendCertContent(content, appendDoubleQuote(indApplication.FAX_NO1));

                    content = appendCertContent(content, appendDoubleQuote(catCode));

                    // get sv
                    C_S_SYSTEM_VALUE svCatGrp = getSSystemValueByUuid(sCategoryCode.CATEGORY_GROUP_ID);
                    if (svCatGrp != null)
                    {
                        content = appendCertContent(content, appendDoubleQuote(svCatGrp.CODE));
                        content = appendCertContent(content, appendDoubleQuote(svCatGrp.ENGLISH_DESCRIPTION));
                        content = appendCertContent(content, appendDoubleQuote(svCatGrp.CHINESE_DESCRIPTION));
                        content = appendCertContent(content, appendDoubleQuote(sCategoryCode.ENGLISH_SUB_TITLE_DESCRIPTION));
                        content = appendCertContent(content, appendDoubleQuote(sCategoryCode.CHINESE_SUB_TITLE_DESCRIPTION));
                    }
                }

                // print sAuthority
                if (sAuthority != null)
                {
                    content = appendCertContent(content, appendDoubleQuote(sAuthority.ENGLISH_NAME));
                    content = appendCertContent(content, appendDoubleQuote(sAuthority.CHINESE_NAME));
                    content = appendCertContent(content, appendDoubleQuote(sAuthority.ENGLISH_RANK));
                    content = appendCertContent(content, appendDoubleQuote(sAuthority.CHINESE_RANK));
                    content = appendCertContent(content, appendDoubleQuote(sAuthority.TELEPHONE_NO));
                    content = appendCertContent(content, appendDoubleQuote(sAuthority.FAX_NO));
                    content = appendCertContent(content, appendDoubleQuote(sAuthority.ENGLISH_ACTION_NAME));
                }
                else
                {
                    content = appendCertContent(content, appendDoubleQuote(""));
                    content = appendCertContent(content, appendDoubleQuote(""));
                    content = appendCertContent(content, appendDoubleQuote(""));
                    content = appendCertContent(content, appendDoubleQuote(""));
                    content = appendCertContent(content, appendDoubleQuote(""));
                    content = appendCertContent(content, appendDoubleQuote(""));
                    content = appendCertContent(content, appendDoubleQuote(""));
                }

                content = appendCertContent(content, appendDoubleQuote(rptIssueDt));

                if (applicant != null)
                {
                    content = appendCertContent(content, appendDoubleQuote(applicant.CHINESE_NAME));
                }


                if (indCertificate != null)
                {

                    if (RegistrationConstant.S_CATEGORY_CODE_RSE.Equals(catCode))
                    {
                        content = appendCertContent(content, appendDoubleQuote(indApplication.FILE_REFERENCE_NO + "(SE)"));
                    }
                    else if (RegistrationConstant.S_CATEGORY_CODE_RGE.Equals(catCode))
                    {
                        content = appendCertContent(content, appendDoubleQuote(indApplication.FILE_REFERENCE_NO + "(GE)"));
                    }
                    else
                    {
                        content = appendCertContent(content, appendDoubleQuote(indApplication.FILE_REFERENCE_NO));
                    }

                    String tabChar = "\t";
                    String mwItemsStr = "";
                    List<String> mwItems = new List<string>();
                    if (RegistrationConstant.S_CATEGORY_CODE_MWC_W.Equals(catCode))
                    {
                        List<C_S_SYSTEM_VALUE> detailList = getMwItemDetailByApplication(indApplication.UUID, intProcess);
                        for (int i = 0; i < detailList.Count(); i++)
                        {
                            C_S_SYSTEM_VALUE sv = detailList.ElementAt(i);
                            mwItems.Add(sv.CODE);
                        }
                    }

                    if (mwItems.Count() > 0)
                    {
                        for (int i = 0; i < mwItems.Count(); i++)
                        {
                            String mwItem = mwItems.ElementAt(i);
                            if ((i % 7 == 0) && (i != (mwItems.Count() - 1)))
                            {
                                mwItemsStr += DOUBLEQUOTE + mwItem + tabChar;
                            }
                            if ((i % 7 > 0) && (i % 7 < 6) && (i != (mwItems.Count() - 1)))
                            {
                                mwItemsStr += mwItem + tabChar;
                            }
                            if ((i % 7 == 6) && (i != (mwItems.Count() - 1)))
                            {
                                mwItemsStr += mwItem + DOUBLEQUOTE + SEPARATOR;
                            }
                            if ((i % 7 == 0) && (i == (mwItems.Count() - 1)))
                            {
                                mwItemsStr += DOUBLEQUOTE;
                            }
                            if (i == (mwItems.Count() - 1))
                            {
                                mwItemsStr += mwItem + DOUBLEQUOTE;
                            }
                        }

                        if (mwItems.Count() <= 35)
                        {
                            mwItemsStr += SEPARATOR;
                            //add blank items
                            int groups = (mwItems.Count()) / 7;
                            int lastGroupSize = (mwItems.Count()) % 7;
                            if (groups == 5)
                            {
                                mwItemsStr += DOUBLEQUOTE + DOUBLEQUOTE;
                            }
                            else if (groups < 5)
                            {
                                if (lastGroupSize == 0)
                                {
                                    mwItemsStr += DOUBLEQUOTE + DOUBLEQUOTE + SEPARATOR;
                                }
                                int emptyGroups = 1;
                                while (emptyGroups < (5 - groups))
                                {
                                    mwItemsStr += DOUBLEQUOTE + DOUBLEQUOTE + SEPARATOR;
                                    emptyGroups++;
                                }
                                mwItemsStr += DOUBLEQUOTE + DOUBLEQUOTE;
                            }
                        }
                    }
                    else
                    {
                        if (RegistrationConstant.S_CATEGORY_CODE_MWC_W.Equals(catCode))
                        {
                            mwItemsStr += DOUBLEQUOTE + DOUBLEQUOTE + SEPARATOR;
                            mwItemsStr += DOUBLEQUOTE + DOUBLEQUOTE + SEPARATOR;
                            mwItemsStr += DOUBLEQUOTE + DOUBLEQUOTE + SEPARATOR;
                            mwItemsStr += DOUBLEQUOTE + DOUBLEQUOTE + SEPARATOR;
                            mwItemsStr += DOUBLEQUOTE + DOUBLEQUOTE;
                        }
                    }
                    content += mwItemsStr;
                }
            }
            return content;
        }


        // Common function export cert for GBC & MWC
        public String populateCompExportCert(String process, String compAppUuid, String issueDt, String lastDocRecDt, String signature)
        {
            int intProcess = int.Parse(process);
            String content = "";

            // certificate common colunn header
       //ADDED Serial_Number required in UR
            List<string> certHeaderList1 = new List<string>()
            {
            "file_ref", "reg_no", "br_no", "ename", "cname",
            "cat_code", "cat_grp_code", "cat_grp_desc",
            "reg_dt", "gazet_dt", "expiry_dt",
            "restore_app_dt", "restore_dt",
            "retent_app_dt",
            "removal_dt",
            "approval_dt",
            "addr1", "addr2", "addr3",
            "addr4", "addr5",
            "caddr1", "caddr2", "caddr3",
            "caddr4", "caddr5",
            "tel_no1",  "fax_no1",
            "auth_name", "auth_cname",
            "auth_rank", "auth_crank",
            "acting", "auth_tel",
            "auth_fax",
            "sub_reg", "sub_creg", "issue_dt", "doc_received_dt", "as_td",
            "cert_cname", "sn",
            "typeline1", "classline1", "typeline2", "classline2",
            "typeline3", "classline3", "typeline4", "classline4",
            "typeline5", "classline5", "typeline6", "classline6",
            "typeline7", "classline7", "Serial_Number"
            };

            List<string> certHeaderList2 = new List<string>()
            {
            "file_ref", "reg_no", "br_no", "ename", "cname",
            "cat_code", "cat_grp_code", "cat_grp_desc",
            "reg_dt",
            "gazet_dt",
            "expiry_dt",
            "restore_app_dt",
            "restore_dt",
            "retent_app_dt",
            "removal_dt",
            "approval_dt",
            "addr1", "addr2", "addr3",
            "addr4", "addr5",
            "caddr1", "caddr2", "caddr3",
            "caddr4", "caddr5",
            "tel_no1",  "fax_no1",
            "auth_name", "auth_cname",
            "auth_rank", "auth_crank",
            "acting", "auth_tel",
            "auth_fax",
            "sub_reg", "sub_creg", "issue_dt", "doc_received_dt",
            "as_td",
            "cert_cname", "Serial_Number"
            };

            // by default
            List<string> certHeaderList = certHeaderList1;

            // get compApp by uuid
            C_COMP_APPLICATION compApp = getCompApplicationByUuid(compAppUuid);

            if (compApp != null)
            {
                // get sCatCode
                C_S_CATEGORY_CODE sCatCode = getCompSCategoryCode(compApp);

                if (sCatCode != null)
                {
                    String catCodeStr = sCatCode.CODE;
                    catCodeStr = catCodeStr.Trim();
                    if ("MWC".Equals(catCodeStr) || "MWI".Equals(catCodeStr) || "MWC(P)".Equals(catCodeStr))
                    {
                        certHeaderList = certHeaderList1;
                    }
                    else
                    {
                        certHeaderList = certHeaderList2;
                    }
                }

                // header
                foreach (String certHeader in certHeaderList)
                {
                    content = appendCertContent(content, certHeader);
                }
                content += "\r\n";

                content += appendDoubleQuote(compApp.FILE_REFERENCE_NO);
                content = appendCertContent(content, appendDoubleQuote(compApp.CERTIFICATION_NO));
                content = appendCertContent(content, appendDoubleQuote(compApp.BR_NO));
                content = appendCertContent(content, appendDoubleQuote(compApp.ENGLISH_COMPANY_NAME.ToUpper()));
                content = appendCertContent(content, appendDoubleQuote(compApp.CHINESE_COMPANY_NAME));
                content = appendCertContent(content, sCatCode != null ? appendDoubleQuote(sCatCode.CODE) : "");

                // get svCatGrp
                C_S_SYSTEM_VALUE svCatGrp = getSSystemValueByUuid(sCatCode.CATEGORY_GROUP_ID);
                content = appendCertContent(content, svCatGrp != null ? appendDoubleQuote(svCatGrp.CODE) : "");
                content = appendCertContent(content, svCatGrp != null ? appendDoubleQuote(svCatGrp.ENGLISH_DESCRIPTION) : "");

                content = appendCertContent(content, appendDoubleQuote(compApp.REGISTRATION_DATE.ToString()));
                content = appendCertContent(content, appendDoubleQuote(compApp.GAZETTE_DATE.ToString()));

                if (RegistrationConstant.RETENTION == intProcess || RegistrationConstant.RESTORATION == intProcess)
                {
                    content = appendCertContent(content, appendDoubleQuote(compApp.EXTEND_DATE.ToString()));
                }
                else
                {
                    if (compApp.EXPIRY_DATE == null)
                    {
                        content = appendCertContent(content, appendDoubleQuote(compApp.EXTEND_DATE.ToString()));
                    }
                    else
                    {
                        content = appendCertContent(content, appendDoubleQuote(compApp.EXPIRY_DATE.ToString()));
                    }
                }

                content = appendCertContent(content, appendDoubleQuote(compApp.RESTORATION_APPLICATION_DATE.ToString()));
                content = appendCertContent(content, appendDoubleQuote(compApp.RESTORE_DATE.ToString()));
                content = appendCertContent(content, appendDoubleQuote(compApp.RETENTION_APPLICATION_DATE.ToString()));
                content = appendCertContent(content, appendDoubleQuote(compApp.REMOVAL_DATE.ToString()));
                content = appendCertContent(content, appendDoubleQuote(compApp.APPROVAL_DATE.ToString()));

                // get address
                C_ADDRESS engAddress = getAddressByUuid(compApp.ENGLISH_ADDRESS_ID);
                C_ADDRESS chiAddress = getAddressByUuid(compApp.CHINESE_ADDRESS_ID);

                content = appendCertContent(content, appendDoubleQuote(engAddress.ADDRESS_LINE1));
                content = appendCertContent(content, appendDoubleQuote(engAddress.ADDRESS_LINE2));
                content = appendCertContent(content, appendDoubleQuote(engAddress.ADDRESS_LINE3));
                content = appendCertContent(content, appendDoubleQuote(engAddress.ADDRESS_LINE4));
                content = appendCertContent(content, appendDoubleQuote(engAddress.ADDRESS_LINE5));
                content = appendCertContent(content, appendDoubleQuote(chiAddress.ADDRESS_LINE1));
                content = appendCertContent(content, appendDoubleQuote(chiAddress.ADDRESS_LINE2));
                content = appendCertContent(content, appendDoubleQuote(chiAddress.ADDRESS_LINE3));
                content = appendCertContent(content, appendDoubleQuote(chiAddress.ADDRESS_LINE4));
                content = appendCertContent(content, appendDoubleQuote(chiAddress.ADDRESS_LINE5));
                content = appendCertContent(content, appendDoubleQuote(compApp.TELEPHONE_NO1));
                content = appendCertContent(content, appendDoubleQuote(compApp.FAX_NO1));

                // get authority
                C_S_AUTHORITY sAuthority = getAuthorityByUuid(signature);
                if (sAuthority != null)
                {
                    content = appendCertContent(content, appendDoubleQuote(sAuthority.ENGLISH_NAME));
                    content = appendCertContent(content, appendDoubleQuote(sAuthority.CHINESE_NAME));
                    content = appendCertContent(content, appendDoubleQuote(sAuthority.ENGLISH_RANK));
                    content = appendCertContent(content, appendDoubleQuote(sAuthority.CHINESE_RANK));
                    content = appendCertContent(content, appendDoubleQuote(sAuthority.ENGLISH_ACTION_NAME));
                    content = appendCertContent(content, appendDoubleQuote(sAuthority.TELEPHONE_NO));
                    content = appendCertContent(content, appendDoubleQuote(sAuthority.FAX_NO));
                }
                else
                {
                    content = appendCertContent(content, appendDoubleQuote(""));
                    content = appendCertContent(content, appendDoubleQuote(""));
                    content = appendCertContent(content, appendDoubleQuote(""));
                    content = appendCertContent(content, appendDoubleQuote(""));
                    content = appendCertContent(content, appendDoubleQuote(""));
                    content = appendCertContent(content, appendDoubleQuote(""));
                    content = appendCertContent(content, appendDoubleQuote(""));
                }

                if (sCatCode != null)
                {
                    content = appendCertContent(content, appendDoubleQuote(sCatCode.ENGLISH_SUB_TITLE_DESCRIPTION));
                    content = appendCertContent(content, appendDoubleQuote(sCatCode.CHINESE_SUB_TITLE_DESCRIPTION));
                }
                else
                {
                    content = appendCertContent(content, appendDoubleQuote(""));
                    content = appendCertContent(content, appendDoubleQuote(""));
                }

                content = appendCertContent(content, appendDoubleQuote(issueDt));
                content = appendCertContent(content, appendDoubleQuote(lastDocRecDt));

                // get comp applicant infor list
                List<C_COMP_APPLICANT_INFO> resultList = getCompApplicantInfoForExportData(compApp.UUID, new DateTime());
                String dummySearchResult = "";
                foreach (C_COMP_APPLICANT_INFO compAppInfo in resultList)
                {
                    C_APPLICANT app = getApplicantByUuid(compAppInfo.APPLICANT_ID);
                    C_S_SYSTEM_VALUE svRole = getSSystemValueByUuid(compAppInfo.APPLICANT_ROLE_ID);
                    dummySearchResult += app.SURNAME + " " + app.GIVEN_NAME_ON_ID
                        + " (" + svRole.CODE + ")@ ";
                }
                content = appendCertContent(content, appendDoubleQuote(dummySearchResult));

                if (sCatCode != null)
                {
                    String catCodeStr = sCatCode.CODE;
                    catCodeStr = catCodeStr.Trim();
                    if ("MWC".Equals(catCodeStr) || "MWI".Equals(catCodeStr) || "MWC(P)".Equals(catCodeStr))
                    {
                        // update/print sequence no
                        String newSequenceNo = getCertSeqNo(catCodeStr, "Y");
                        content = appendCertContent(content, appendDoubleQuote(newSequenceNo.ToString()));
                        AuditLogService.logDebug("SequenceNumber: " + newSequenceNo + ", Code:" + catCodeStr);

                        // MwItem List
                        List<C_COMP_APPLICANT_MW_ITEM> companyMwItemList = new List<C_COMP_APPLICANT_MW_ITEM>();
                        List<String> mwClass1TypeList = new List<String>();
                        List<String> mwClass2TypeList = new List<String>();
                        List<String> mwClass3TypeList = new List<String>();

                        // get mw items
                        List<C_COMP_APPLICANT_INFO> applicantList = getCompApplicantInfo(compApp.UUID);
                        foreach (C_COMP_APPLICANT_INFO compAppInfo in applicantList)
                        {
                            List<C_COMP_APPLICANT_MW_ITEM> compApplicantMwItemList = getCompApplicantMwItem(compAppInfo.UUID);
                            if (compApplicantMwItemList != null && compApplicantMwItemList.Count > 0)
                            {
                                C_S_SYSTEM_VALUE svRole = getSSystemValueByUuid(compAppInfo.APPLICANT_ROLE_ID);
                                C_S_SYSTEM_VALUE svStatus = getSSystemValueByUuid(compAppInfo.APPLICANT_ROLE_ID);

                                if (RegistrationConstant.STATUS_ACTIVE.Equals(svStatus.CODE)
                                    && svRole.CODE.StartsWith("A"))
                                {
                                    companyMwItemList.AddRange(compApplicantMwItemList);
                                }
                            }
                        }

                        // get company Capability by Applicant
                        // add to class 1 item list
                        foreach (C_COMP_APPLICANT_MW_ITEM compAppMwItem in companyMwItemList)
                        {
                            C_S_SYSTEM_VALUE svMwClass = getSSystemValueByUuid(compAppMwItem.ITEM_CLASS_ID);
                            C_S_SYSTEM_VALUE svMwType = getSSystemValueByUuid(compAppMwItem.ITEM_TYPE_ID);
                            if (RegistrationConstant.MW_CLASS_I.Equals(svMwClass.CODE))
                            {
                                if (!mwClass1TypeList.Contains(svMwType.CODE))
                                {
                                    mwClass1TypeList.Add(svMwType.CODE);
                                }
                            }
                        }

                        // add to class 2 item list
                        foreach (C_COMP_APPLICANT_MW_ITEM compAppMwItem in companyMwItemList)
                        {
                            C_S_SYSTEM_VALUE svMwClass = getSSystemValueByUuid(compAppMwItem.ITEM_CLASS_ID);
                            C_S_SYSTEM_VALUE svMwType = getSSystemValueByUuid(compAppMwItem.ITEM_TYPE_ID);
                            if (RegistrationConstant.MW_CLASS_II.Equals(svMwClass.CODE))
                            {
                                if (!mwClass1TypeList.Contains(svMwType.CODE) && !mwClass2TypeList.Contains(svMwType.CODE))
                                {
                                    mwClass2TypeList.Add(svMwType.CODE);
                                }
                            }
                        }

                        // add to class 3 item list
                        foreach (C_COMP_APPLICANT_MW_ITEM compAppMwItem in companyMwItemList)
                        {
                            C_S_SYSTEM_VALUE svMwClass = getSSystemValueByUuid(compAppMwItem.ITEM_CLASS_ID);
                            C_S_SYSTEM_VALUE svMwType = getSSystemValueByUuid(compAppMwItem.ITEM_TYPE_ID);
                            if (RegistrationConstant.MW_CLASS_III.Equals(svMwClass.CODE))
                            {
                                if (!mwClass1TypeList.Contains(svMwType.CODE)
                                    && !mwClass2TypeList.Contains(svMwType.CODE)
                                    && !mwClass3TypeList.Contains(svMwType.CODE))
                                {
                                    mwClass3TypeList.Add(svMwType.CODE);
                                }
                            }
                        }

                        mwClass1TypeList.Sort();
                        mwClass2TypeList.Sort();
                        mwClass3TypeList.Sort();

                        String[] MW_CLASS_DESCRIPTION = { " I, II & III ", " II & III ", " III " };
                        int printLineCoutner = 0;
                        int MAX_LINE = RegistrationConstant.MW_TYPE_DESCRIPTION.Length;
                        for (int i = 0; i < RegistrationConstant.MW_TYPE.Length; i++)
                        {
                            String mwType = RegistrationConstant.MW_TYPE[i];
                            String mwTypeDesc = RegistrationConstant.MW_TYPE_DESCRIPTION[i];
                            if (mwClass1TypeList.Contains(mwType))
                            {
                                String mwClassDesc = MW_CLASS_DESCRIPTION[0];
                                content = appendCertContent(content, appendDoubleQuote(mwTypeDesc));
                                content = appendCertContent(content, appendDoubleQuote(mwClassDesc));
                            }
                            else if (mwClass2TypeList.Contains(mwType))
                            {
                                String mwClassDesc = MW_CLASS_DESCRIPTION[1];
                                content = appendCertContent(content, appendDoubleQuote(mwTypeDesc));
                                content = appendCertContent(content, appendDoubleQuote(mwClassDesc));
                            }
                            else if (mwClass3TypeList.Contains(mwType))
                            {
                                String mwClassDesc = MW_CLASS_DESCRIPTION[2];
                                content = appendCertContent(content, appendDoubleQuote(mwTypeDesc));
                                content = appendCertContent(content, appendDoubleQuote(mwClassDesc));
                            }
                        }

                        for (int i = printLineCoutner; i < MAX_LINE; i++)
                        {
                            content = appendCertContent(content, appendDoubleQuote(""));
                            content = appendCertContent(content, appendDoubleQuote(""));
                        }
                    }
                    else if ("GBC".Equals(catCodeStr)) {
                        String newSequenceNo = getCertSeqNo(catCodeStr, "Y");
                        content = appendCertContent(content, appendDoubleQuote(newSequenceNo.ToString()));
                        AuditLogService.logDebug("SequenceNumber: " + newSequenceNo + ", Code:" + catCodeStr);

                    }
                }
            }

            return content;
        }

        // Common function export letter for GBC & MWC
        public String populateCompExportLetter(String process, String compAppUuid, String rptIssueDt, String rptRcvdDt, String signature)
        {
            int intProcess = int.Parse(process);
            String content = "";

            // certificate common colunn header
            List<string> certHeaderList = new List<string>()
            {
                "file_ref", "reg_no", "br_no", "ename", "cname",
                "cat_code", "cat_grp_code", "cat_grp_desc",
                "reg_dt",
                "gazet_dt",
                "expiry_dt",
                "restore_app_dt",
                "restore_dt",
                "retent_app_dt",
                "removal_dt",
                "approval_dt",
                "addr1", "addr2", "addr3",
                "addr4", "addr5",
                "caddr1", "caddr2", "caddr3",
                "caddr4", "caddr5",
                "tel_no1",  "fax_no1",

                "auth_name", "auth_cname",
                "auth_rank", "auth_crank",
                "acting", "auth_tel",
                "auth_fax",

                "sub_reg", "sub_creg", "issue_dt", "doc_received_dt",
                "as_td",
                "cert_cname"
            };

            // append header
            foreach (String certHeader in certHeaderList)
            {
                content = appendCertContent(content, certHeader);
            }
            content += "\r\n";


            // get compApp by uuid
            C_COMP_APPLICATION compApp = getCompApplicationByUuid(compAppUuid);

            if (compApp != null)
            {
                // get sCatCode
                C_S_CATEGORY_CODE sCatCode = getCompSCategoryCode(compApp);

                content += appendDoubleQuote(compApp.FILE_REFERENCE_NO);
                content = appendCertContent(content, appendDoubleQuote(compApp.CERTIFICATION_NO));
                content = appendCertContent(content, appendDoubleQuote(compApp.BR_NO));
                content = appendCertContent(content, appendDoubleQuote(compApp.ENGLISH_COMPANY_NAME));
                content = appendCertContent(content, appendDoubleQuote(compApp.CHINESE_COMPANY_NAME));
                content = appendCertContent(content, appendDoubleQuote(sCatCode.CODE));

                // get svCatGrp
                C_S_SYSTEM_VALUE svCatGrp = getSSystemValueByUuid(sCatCode.CATEGORY_GROUP_ID);
                content = appendCertContent(content, svCatGrp != null ? appendDoubleQuote(svCatGrp.CODE) : "");
                content = appendCertContent(content, svCatGrp != null ? appendDoubleQuote(svCatGrp.ENGLISH_DESCRIPTION) : "");

                content = appendCertContent(content, compApp.REGISTRATION_DATE != null ? appendDoubleQuote(compApp.REGISTRATION_DATE.ToString()) : "");
                content = appendCertContent(content, compApp.GAZETTE_DATE != null ? appendDoubleQuote(compApp.GAZETTE_DATE.ToString()) : "");

                if (sCatCode != null)
                {
                    String catCodeStr = sCatCode.CODE;
                    catCodeStr = catCodeStr.Trim();
                    if (RegistrationConstant.RETENTION.Equals(intProcess) || RegistrationConstant.RESTORATION.Equals(intProcess))
                    {
                        content += appendDoubleQuote(compApp.EXTEND_DATE != null ? compApp.EXTEND_DATE.ToString() : "");
                    }
                    else
                    {
                        if (compApp.EXPIRY_DATE != null)
                        {
                            content += appendDoubleQuote(compApp.EXTEND_DATE != null ? compApp.EXTEND_DATE.ToString() : "");
                        }
                        else
                        {
                            content += appendDoubleQuote(compApp.EXPIRY_DATE != null ? compApp.EXPIRY_DATE.ToString() : "");
                        }
                    }
                }

                content = appendCertContent(content, appendDoubleQuote(compApp.RESTORATION_APPLICATION_DATE != null ? compApp.RESTORATION_APPLICATION_DATE.ToString() : ""));
                content = appendCertContent(content, appendDoubleQuote(compApp.RESTORE_DATE != null ? compApp.RESTORE_DATE.ToString() : ""));
                content = appendCertContent(content, appendDoubleQuote(compApp.EXTEND_DATE != null ? compApp.EXTEND_DATE.ToString() : ""));
                content = appendCertContent(content, appendDoubleQuote(compApp.REMOVAL_DATE != null ? compApp.REMOVAL_DATE.ToString() : ""));
                content = appendCertContent(content, appendDoubleQuote(compApp.APPROVAL_DATE != null ? compApp.APPROVAL_DATE.ToString() : ""));

                // get address
                C_ADDRESS engAddress = getAddressByUuid(compApp.ENGLISH_ADDRESS_ID);
                C_ADDRESS chiAddress = getAddressByUuid(compApp.CHINESE_ADDRESS_ID);

                content = appendCertContent(content, appendDoubleQuote(engAddress.ADDRESS_LINE1));
                content = appendCertContent(content, appendDoubleQuote(engAddress.ADDRESS_LINE2));
                content = appendCertContent(content, appendDoubleQuote(engAddress.ADDRESS_LINE3));
                content = appendCertContent(content, appendDoubleQuote(engAddress.ADDRESS_LINE4));
                content = appendCertContent(content, appendDoubleQuote(engAddress.ADDRESS_LINE5));
                content = appendCertContent(content, appendDoubleQuote(chiAddress.ADDRESS_LINE1));
                content = appendCertContent(content, appendDoubleQuote(chiAddress.ADDRESS_LINE2));
                content = appendCertContent(content, appendDoubleQuote(chiAddress.ADDRESS_LINE3));
                content = appendCertContent(content, appendDoubleQuote(chiAddress.ADDRESS_LINE4));
                content = appendCertContent(content, appendDoubleQuote(chiAddress.ADDRESS_LINE5));
                content = appendCertContent(content, appendDoubleQuote(compApp.TELEPHONE_NO1));
                content = appendCertContent(content, appendDoubleQuote(compApp.FAX_NO1));

                // get authority
                C_S_AUTHORITY sAuthority = getAuthorityByUuid(signature);
                if (sAuthority != null)
                {
                    content = appendCertContent(content, appendDoubleQuote(sAuthority.ENGLISH_NAME));
                    content = appendCertContent(content, appendDoubleQuote(sAuthority.CHINESE_NAME));
                    content = appendCertContent(content, appendDoubleQuote(sAuthority.ENGLISH_RANK));
                    content = appendCertContent(content, appendDoubleQuote(sAuthority.CHINESE_RANK));
                    content = appendCertContent(content, appendDoubleQuote(sAuthority.ENGLISH_ACTION_NAME));
                    content = appendCertContent(content, appendDoubleQuote(sAuthority.TELEPHONE_NO));
                    content = appendCertContent(content, appendDoubleQuote(sAuthority.FAX_NO));
                }
                else
                {
                    content = appendCertContent(content, appendDoubleQuote(""));
                    content = appendCertContent(content, appendDoubleQuote(""));
                    content = appendCertContent(content, appendDoubleQuote(""));
                    content = appendCertContent(content, appendDoubleQuote(""));
                    content = appendCertContent(content, appendDoubleQuote(""));
                    content = appendCertContent(content, appendDoubleQuote(""));
                    content = appendCertContent(content, appendDoubleQuote(""));
                }

                content = appendCertContent(content, appendDoubleQuote(sCatCode.ENGLISH_SUB_TITLE_DESCRIPTION));
                content = appendCertContent(content, appendDoubleQuote(sCatCode.CHINESE_SUB_TITLE_DESCRIPTION));

                // parameters from UI
                content = appendCertContent(content, appendDoubleQuote(rptIssueDt));
                content = appendCertContent(content, appendDoubleQuote(rptRcvdDt));

                // get comp applicant info list
                List<C_COMP_APPLICANT_INFO> resultList = getCompApplicantInfoForExportData(compApp.UUID, new DateTime());
                String dummySearchResult = "";
                foreach (C_COMP_APPLICANT_INFO compAppInfo in resultList)
                {
                    C_APPLICANT app = getApplicantByUuid(compAppInfo.APPLICANT_ID);
                    C_S_SYSTEM_VALUE svRole = getSSystemValueByUuid(compAppInfo.APPLICANT_ROLE_ID);
                    dummySearchResult += app.SURNAME + " " + app.GIVEN_NAME_ON_ID
                        + " (" + svRole.CODE + ")@ ";
                }
                content = appendCertContent(content, appendDoubleQuote(dummySearchResult));

                content = appendCertContent(content, appendDoubleQuote(compApp.CHINESE_COMPANY_NAME != null ? compApp.CHINESE_COMPANY_NAME.ToString() : ""));
            }

            return content;
        }

        // Common function export cert with code for GBC & MWC
        public String populateCompExportLetterWithCode(String certificateNo, String process, String compAppUuid,
            String rptIssueDt, String rptRcvdDt, String signature
            , String missingItemRetensionNullChk, String missingItemRetensionChequeChk, String missingItemRetensionProfRegCertChk
            , String missingItemRetensionOthersChk, String missingItemRetensionIncompleteFormChk
            , String missingItemRestorationNullChk, String missingItemRestorationChequeChk, String missingItemRestorationProfRegCertChk
            , String missingItemRestorationOthersChk, String missingItemRestorationIncompleteFormChk
            )
        {
            int intProcess = int.Parse(process);
            String content = "";

            if (RegistrationConstant.LETTER_MMD0008B_1.Equals(certificateNo)
                || RegistrationConstant.LETTER_MMD0008B_2.Equals(certificateNo)
                || RegistrationConstant.LETTER_MMD0009B.Equals(certificateNo)
                || RegistrationConstant.LETTER_MMD0007B_1.Equals(certificateNo)
                || RegistrationConstant.LETTER_MMD0007B_2.Equals(certificateNo))
            {
                // header
                List<string> certHeaderList = new List<string>()
                {   "file_ref", "reg_no", "br_no", "ename", "cname",
                    "cat_code", "cat_grp_code", "cat_grp_desc",
                    "reg_dt",
                    "gazet_dt",
                    "expiry_dt",
                    "restore_app_dt",
                    "restore_dt",
                    "retent_app_dt",
                    "removal_dt",
                    "approval_dt",
                    "addr1", "addr2", "addr3",
                    "addr4", "addr5",
                    "caddr1", "caddr2", "caddr3",
                    "caddr4", "caddr5",
                    "tel_no1",  "fax_no1",

                    "auth_name", "auth_cname",
                    "auth_rank", "auth_crank",
                    "acting", "auth_tel",
                    "auth_fax",

                    "sub_reg", "sub_creg", "issue_dt", "doc_received_dt",
                        "as_td",
                        "cert_cname"
                };

                // header
                foreach (String certHeader in certHeaderList)
                {
                    content = appendCertContent(content, certHeader);
                }
                content += "\r\n";

                // get compApp by uuid
                C_COMP_APPLICATION compApp = getCompApplicationByUuid(compAppUuid);

                if (compApp != null)
                {
                    // get sCatCode
                    C_S_CATEGORY_CODE sCatCode = getCompSCategoryCode(compApp);

                    String catCodeStr = "";
                    if (sCatCode != null)
                    {
                        catCodeStr = sCatCode.CODE;
                        catCodeStr = catCodeStr.Trim();
                    }

                    content += appendDoubleQuote(compApp.FILE_REFERENCE_NO != null ? compApp.FILE_REFERENCE_NO.ToString() : "");
                    content = appendCertContent(content, appendDoubleQuote(compApp.CERTIFICATION_NO != null ? compApp.CERTIFICATION_NO.ToString() : ""));
                    content = appendCertContent(content, appendDoubleQuote(compApp.BR_NO != null ? compApp.BR_NO.ToString() : ""));
                    content = appendCertContent(content, appendDoubleQuote(compApp.ENGLISH_COMPANY_NAME != null ? compApp.ENGLISH_COMPANY_NAME.ToString() : ""));
                    content = appendCertContent(content, appendDoubleQuote(compApp.CHINESE_COMPANY_NAME != null ? compApp.CHINESE_COMPANY_NAME.ToString() : ""));
                    content = appendCertContent(content, appendDoubleQuote(catCodeStr));

                    // get svCatGrp
                    C_S_SYSTEM_VALUE svCatGrp = getSSystemValueByUuid(sCatCode.CATEGORY_GROUP_ID);
                    content = appendCertContent(content, svCatGrp != null ? appendDoubleQuote(svCatGrp.CODE) : "");
                    content = appendCertContent(content, svCatGrp != null ? appendDoubleQuote(svCatGrp.ENGLISH_DESCRIPTION) : "");

                    content = appendCertContent(content, appendDoubleQuote(compApp.REGISTRATION_DATE != null ? compApp.REGISTRATION_DATE.ToString() : ""));
                    content = appendCertContent(content, appendDoubleQuote(compApp.GAZETTE_DATE != null ? compApp.GAZETTE_DATE.ToString() : ""));

                    if (RegistrationConstant.RETENTION.Equals(intProcess) || RegistrationConstant.RESTORATION.Equals(intProcess))
                    {
                        content = appendCertContent(content, appendDoubleQuote(compApp.EXTEND_DATE != null ? compApp.EXTEND_DATE.ToString() : ""));
                    }
                    else
                    {
                        if (compApp.EXPIRY_DATE != null)
                        {
                            content = appendCertContent(content, appendDoubleQuote(compApp.EXTEND_DATE != null ? compApp.EXTEND_DATE.ToString() : ""));
                        }
                        else
                        {
                            content = appendCertContent(content, appendDoubleQuote(compApp.EXPIRY_DATE != null ? compApp.EXPIRY_DATE.ToString() : ""));
                        }
                    }

                    if (RegistrationConstant.LETTER_MMD0009B.Equals(certificateNo))
                    {
                        content = appendCertContent(content, appendDoubleQuote(""));
                    }
                    else
                    {
                        content = appendCertContent(content, appendDoubleQuote(compApp.RESTORATION_APPLICATION_DATE != null ? compApp.RESTORATION_APPLICATION_DATE.ToString() : ""));
                    }

                    content = appendCertContent(content, appendDoubleQuote(compApp.RESTORE_DATE != null ? compApp.RESTORE_DATE.ToString() : ""));
                    content = appendCertContent(content, appendDoubleQuote(compApp.REMOVAL_DATE != null ? compApp.REMOVAL_DATE.ToString() : ""));
                    content = appendCertContent(content, appendDoubleQuote(compApp.APPROVAL_DATE != null ? compApp.APPROVAL_DATE.ToString() : ""));

                    // get address
                    C_ADDRESS engAddress = getAddressByUuid(compApp.ENGLISH_ADDRESS_ID);
                    C_ADDRESS chiAddress = getAddressByUuid(compApp.CHINESE_ADDRESS_ID);

                    if (engAddress != null)
                    {
                        content = appendCertContent(content, appendDoubleQuote(engAddress.ADDRESS_LINE1));
                        content = appendCertContent(content, appendDoubleQuote(engAddress.ADDRESS_LINE2));
                        content = appendCertContent(content, appendDoubleQuote(engAddress.ADDRESS_LINE3));
                        content = appendCertContent(content, appendDoubleQuote(engAddress.ADDRESS_LINE4));
                        content = appendCertContent(content, appendDoubleQuote(engAddress.ADDRESS_LINE5));
                    }
                    if (chiAddress != null)
                    {
                        content = appendCertContent(content, appendDoubleQuote(chiAddress.ADDRESS_LINE1));
                        content = appendCertContent(content, appendDoubleQuote(chiAddress.ADDRESS_LINE2));
                        content = appendCertContent(content, appendDoubleQuote(chiAddress.ADDRESS_LINE3));
                        content = appendCertContent(content, appendDoubleQuote(chiAddress.ADDRESS_LINE4));
                        content = appendCertContent(content, appendDoubleQuote(chiAddress.ADDRESS_LINE5));
                    }
                    content = appendCertContent(content, appendDoubleQuote(compApp.TELEPHONE_NO1));
                    content = appendCertContent(content, appendDoubleQuote(compApp.FAX_NO1));

                    // get authority
                    C_S_AUTHORITY sAuthority = getAuthorityByUuid(signature);
                    if (sAuthority != null)
                    {
                        content = appendCertContent(content, appendDoubleQuote(sAuthority.ENGLISH_NAME));
                        content = appendCertContent(content, appendDoubleQuote(sAuthority.CHINESE_NAME));
                        content = appendCertContent(content, appendDoubleQuote(sAuthority.ENGLISH_RANK));
                        content = appendCertContent(content, appendDoubleQuote(sAuthority.CHINESE_RANK));
                        content = appendCertContent(content, appendDoubleQuote(sAuthority.ENGLISH_ACTION_NAME));
                        content = appendCertContent(content, appendDoubleQuote(sAuthority.TELEPHONE_NO));
                        content = appendCertContent(content, appendDoubleQuote(sAuthority.FAX_NO));
                    }
                    else
                    {
                        content = appendCertContent(content, appendDoubleQuote(""));
                        content = appendCertContent(content, appendDoubleQuote(""));
                        content = appendCertContent(content, appendDoubleQuote(""));
                        content = appendCertContent(content, appendDoubleQuote(""));
                        content = appendCertContent(content, appendDoubleQuote(""));
                        content = appendCertContent(content, appendDoubleQuote(""));
                        content = appendCertContent(content, appendDoubleQuote(""));
                    }

                    content = appendCertContent(content, appendDoubleQuote(sCatCode.ENGLISH_SUB_TITLE_DESCRIPTION));
                    content = appendCertContent(content, appendDoubleQuote(sCatCode.CHINESE_SUB_TITLE_DESCRIPTION));

                    if (RegistrationConstant.LETTER_MMD0007B_2.Equals(certificateNo))
                    {
                        content = appendCertContent(content, appendDoubleQuote(""));
                    }
                    else
                    {
                        content = appendCertContent(content, appendDoubleQuote(rptIssueDt));
                    }
                    content = appendCertContent(content, appendDoubleQuote(rptRcvdDt));

                    // get comp applicant info list
                    List<C_COMP_APPLICANT_INFO> resultList = getCompApplicantInfoForExportData(compApp.UUID, new DateTime());
                    String dummySearchResult = "";
                    foreach (C_COMP_APPLICANT_INFO compAppInfo in resultList)
                    {
                        C_APPLICANT app = getApplicantByUuid(compAppInfo.APPLICANT_ID);
                        C_S_SYSTEM_VALUE svRole = getSSystemValueByUuid(compAppInfo.APPLICANT_ROLE_ID);
                        dummySearchResult += app.SURNAME + " " + app.GIVEN_NAME_ON_ID
                            + " (" + svRole.CODE + ")@ ";
                    }
                    content = appendCertContent(content, appendDoubleQuote(dummySearchResult));
                    content = appendCertContent(content, appendDoubleQuote(compApp.CHINESE_COMPANY_NAME != null ? compApp.CHINESE_COMPANY_NAME.ToString() : ""));
                }
            }
            else if (RegistrationConstant.FAX_TEMPLATE_RETENTION.Equals(certificateNo) || RegistrationConstant.FAX_TEMPLATE_RESTORATION.Equals(certificateNo))
            {
                // header
                List<string> certHeaderList = new List<string>()
                {
                    "file_ref", "reg_no", "br_no", "ename", "cname",
                    "cat_code", "cat_grp_code", "cat_grp_desc",
                    "reg_dt",
                    "gazet_dt",
                    "expiry_dt",
                    "restore_app_dt",
                    "restore_dt",
                    "retent_app_dt",
                    "removal_dt",
                    "approval_dt",
                    "addr1", "addr2", "addr3",
                    "addr4", "addr5",
                    "caddr1", "caddr2", "caddr3",
                    "caddr4", "caddr5",
                    "tel_no1",  "fax_no1",

                    "auth_name", "auth_cname",
                    "auth_rank", "auth_crank",
                    "acting", "auth_tel",
                    "auth_fax",

                    "sub_reg", "sub_creg", "issue_dt", "doc_received_dt",
                    "missing_item",
                    "as_td",
                    "cert_cname",
                    "form_used","miss_1","miss_2","miss_3","miss_4","reg_name"
                };

                // header
                foreach (String certHeader in certHeaderList)
                {
                    content = appendCertContent(content, certHeader);
                }
                content += "\r\n";

                // get compApp by uuid
                C_COMP_APPLICATION compApp = getCompApplicationByUuid(compAppUuid);

                if (compApp != null)
                {
                    // get sCatCode
                    C_S_CATEGORY_CODE sCatCode = getCompSCategoryCode(compApp);

                    String catCodeStr = "";
                    if (sCatCode != null)
                    {
                        catCodeStr = sCatCode.CODE;
                        catCodeStr = catCodeStr.Trim();
                    }

                    content += appendDoubleQuote(compApp.FILE_REFERENCE_NO != null ? compApp.FILE_REFERENCE_NO.ToString() : "");
                    content = appendCertContent(content, appendDoubleQuote(compApp.CERTIFICATION_NO != null ? compApp.CERTIFICATION_NO.ToString() : ""));
                    content = appendCertContent(content, appendDoubleQuote(compApp.BR_NO != null ? compApp.BR_NO.ToString() : ""));
                    content = appendCertContent(content, appendDoubleQuote(compApp.ENGLISH_COMPANY_NAME != null ? compApp.ENGLISH_COMPANY_NAME.ToString() : ""));
                    content = appendCertContent(content, appendDoubleQuote(compApp.CHINESE_COMPANY_NAME != null ? compApp.CHINESE_COMPANY_NAME.ToString() : ""));
                    content = appendCertContent(content, appendDoubleQuote(catCodeStr));

                    // get svCatGrp
                    C_S_SYSTEM_VALUE svCatGrp = getSSystemValueByUuid(sCatCode.CATEGORY_GROUP_ID);
                    content = appendCertContent(content, svCatGrp != null ? appendDoubleQuote(svCatGrp.CODE) : "");
                    content = appendCertContent(content, svCatGrp != null ? appendDoubleQuote(svCatGrp.ENGLISH_DESCRIPTION) : "");

                    content = appendCertContent(content, appendDoubleQuote(compApp.REGISTRATION_DATE != null ? compApp.REGISTRATION_DATE.ToString() : ""));
                    content = appendCertContent(content, appendDoubleQuote(compApp.GAZETTE_DATE != null ? compApp.GAZETTE_DATE.ToString() : ""));

                    if (RegistrationConstant.RETENTION.Equals(intProcess) || RegistrationConstant.RESTORATION.Equals(intProcess))
                    {
                        content = appendCertContent(content, appendDoubleQuote(compApp.EXTEND_DATE != null ? compApp.EXTEND_DATE.ToString() : ""));
                    }
                    else
                    {
                        content = appendCertContent(content, appendDoubleQuote(compApp.EXPIRY_DATE != null ? compApp.EXPIRY_DATE.ToString() : ""));
                    }
                    content = appendCertContent(content, appendDoubleQuote(compApp.RESTORATION_APPLICATION_DATE != null ? compApp.RESTORATION_APPLICATION_DATE.ToString() : ""));
                    content = appendCertContent(content, appendDoubleQuote(compApp.RESTORE_DATE != null ? compApp.RESTORE_DATE.ToString() : ""));
                    content = appendCertContent(content, appendDoubleQuote(compApp.RETENTION_APPLICATION_DATE != null ? compApp.RETENTION_APPLICATION_DATE.ToString() : ""));
                    content = appendCertContent(content, appendDoubleQuote(compApp.REMOVAL_DATE != null ? compApp.REMOVAL_DATE.ToString() : ""));
                    content = appendCertContent(content, appendDoubleQuote(compApp.APPROVAL_DATE != null ? compApp.APPROVAL_DATE.ToString() : ""));


                    // get address
                    C_ADDRESS engAddress = getAddressByUuid(compApp.ENGLISH_ADDRESS_ID);
                    C_ADDRESS chiAddress = getAddressByUuid(compApp.CHINESE_ADDRESS_ID);

                    if (engAddress != null)
                    {
                        content = appendCertContent(content, appendDoubleQuote(engAddress.ADDRESS_LINE1));
                        content = appendCertContent(content, appendDoubleQuote(engAddress.ADDRESS_LINE2));
                        content = appendCertContent(content, appendDoubleQuote(engAddress.ADDRESS_LINE3));
                        content = appendCertContent(content, appendDoubleQuote(engAddress.ADDRESS_LINE4));
                        content = appendCertContent(content, appendDoubleQuote(engAddress.ADDRESS_LINE5));
                    }
                    if (chiAddress != null)
                    {
                        content = appendCertContent(content, appendDoubleQuote(chiAddress.ADDRESS_LINE1));
                        content = appendCertContent(content, appendDoubleQuote(chiAddress.ADDRESS_LINE2));
                        content = appendCertContent(content, appendDoubleQuote(chiAddress.ADDRESS_LINE3));
                        content = appendCertContent(content, appendDoubleQuote(chiAddress.ADDRESS_LINE4));
                        content = appendCertContent(content, appendDoubleQuote(chiAddress.ADDRESS_LINE5));
                    }
                    content = appendCertContent(content, appendDoubleQuote(compApp.TELEPHONE_NO1));
                    content = appendCertContent(content, appendDoubleQuote(compApp.FAX_NO1));

                    // get authority
                    C_S_AUTHORITY sAuthority = getAuthorityByUuid(signature);
                    if (sAuthority != null)
                    {
                        content = appendCertContent(content, appendDoubleQuote(sAuthority.ENGLISH_NAME));
                        content = appendCertContent(content, appendDoubleQuote(sAuthority.CHINESE_NAME));
                        content = appendCertContent(content, appendDoubleQuote(sAuthority.ENGLISH_RANK));
                        content = appendCertContent(content, appendDoubleQuote(sAuthority.CHINESE_RANK));
                        content = appendCertContent(content, appendDoubleQuote(sAuthority.ENGLISH_ACTION_NAME));
                        content = appendCertContent(content, appendDoubleQuote(sAuthority.TELEPHONE_NO));
                        content = appendCertContent(content, appendDoubleQuote(sAuthority.FAX_NO));
                    }
                    else
                    {
                        content = appendCertContent(content, appendDoubleQuote(""));
                        content = appendCertContent(content, appendDoubleQuote(""));
                        content = appendCertContent(content, appendDoubleQuote(""));
                        content = appendCertContent(content, appendDoubleQuote(""));
                        content = appendCertContent(content, appendDoubleQuote(""));
                        content = appendCertContent(content, appendDoubleQuote(""));
                        content = appendCertContent(content, appendDoubleQuote(""));
                    }

                    // print parameters from UI
                    content = appendCertContent(content, appendDoubleQuote(rptIssueDt));
                    content = appendCertContent(content, appendDoubleQuote(rptRcvdDt));
                }

                if (RegistrationConstant.FAX_TEMPLATE_RETENTION.Equals(certificateNo)) {
                    if ("CHECKED".Equals(missingItemRetensionNullChk))
                    {
                        content = appendCertContent(content, appendDoubleQuote("NULL"));
                    }
                    else
                    {
                        content = appendCertContent(content, appendDoubleQuote(""));
                    }

                    // get comp applicant info list
                    List<C_COMP_APPLICANT_INFO> resultList = getCompApplicantInfoForExportData(compApp.UUID, new DateTime());
                    String dummySearchResult = "";
                    foreach (C_COMP_APPLICANT_INFO compAppInfo in resultList)
                    {
                        C_APPLICANT app = getApplicantByUuid(compAppInfo.APPLICANT_ID);
                        C_S_SYSTEM_VALUE svRole = getSSystemValueByUuid(compAppInfo.APPLICANT_ROLE_ID);
                        dummySearchResult += app.SURNAME + " " + app.GIVEN_NAME_ON_ID
                            + " (" + svRole.CODE + ")@ ";
                    }
                    content = appendCertContent(content, appendDoubleQuote(dummySearchResult));
                    content = appendCertContent(content, appendDoubleQuote(compApp.CHINESE_COMPANY_NAME != null ? compApp.CHINESE_COMPANY_NAME.ToString() : ""));

                    // get systemValue by form_ID
                    if (compApp.APPLICATION_FORM_ID != null)
                    {
                        C_S_SYSTEM_VALUE svForm = getSSystemValueByUuid(compApp.APPLICATION_FORM_ID);
                        if (svForm != null)
                        {
                            content = appendCertContent(content, appendDoubleQuote(svForm.CODE));
                        }
                    }
                    else
                    {
                        content = appendCertContent(content, appendDoubleQuote(""));
                    }

                    // handle parameters from UI
                    if ("CHECKED".Equals(missingItemRetensionNullChk))
                    {
                        content = appendCertContent(content, appendDoubleQuote("N"));
                        content = appendCertContent(content, appendDoubleQuote("N"));
                        content = appendCertContent(content, appendDoubleQuote("N"));
                        content = appendCertContent(content, appendDoubleQuote("N"));
                    }
                    else
                    {
                        content = appendCertContent(content, appendDoubleQuote("CHECKED".Equals(missingItemRetensionChequeChk) ? "Y" : "N"));
                        content = appendCertContent(content, appendDoubleQuote("CHECKED".Equals(missingItemRetensionProfRegCertChk) ? "Y" : "N"));
                        content = appendCertContent(content, appendDoubleQuote("CHECKED".Equals(missingItemRetensionOthersChk) ? "Y" : "N"));
                        content = appendCertContent(content, appendDoubleQuote("CHECKED".Equals(missingItemRetensionIncompleteFormChk) ? "Y" : "N"));
                    }

                    // get sCatCode
                    C_S_CATEGORY_CODE sCatCode = getCompSCategoryCode(compApp);
                    if (sCatCode != null)
                    {
                        content = appendCertContent(content, appendDoubleQuote(sCatCode.ENGLISH_DESCRIPTION));
                    }
                    else
                    {
                        content = appendCertContent(content, appendDoubleQuote(""));
                    }
                }
                else if (RegistrationConstant.FAX_TEMPLATE_RESTORATION.Equals(certificateNo))
                {
                    if ("CHECKED".Equals(missingItemRestorationNullChk))
                    {
                        content = appendCertContent(content, appendDoubleQuote("NULL"));
                    }
                    else
                    {
                        content = appendCertContent(content, appendDoubleQuote(""));
                    }

                    // get comp applicant info list
                    List<C_COMP_APPLICANT_INFO> resultList = getCompApplicantInfoForExportData(compApp.UUID, new DateTime());
                    String dummySearchResult = "";
                    foreach (C_COMP_APPLICANT_INFO compAppInfo in resultList)
                    {
                        C_APPLICANT app = getApplicantByUuid(compAppInfo.APPLICANT_ID);
                        C_S_SYSTEM_VALUE svRole = getSSystemValueByUuid(compAppInfo.APPLICANT_ROLE_ID);
                        dummySearchResult += app.SURNAME + " " + app.GIVEN_NAME_ON_ID
                            + " (" + svRole.CODE + ")@ ";
                    }
                    content = appendCertContent(content, appendDoubleQuote(dummySearchResult));
                    content = appendCertContent(content, appendDoubleQuote(compApp.CHINESE_COMPANY_NAME != null ? compApp.CHINESE_COMPANY_NAME.ToString() : ""));

                    // get systemValue by form_ID
                    if (compApp.APPLICATION_FORM_ID != null)
                    {
                        C_S_SYSTEM_VALUE svForm = getSSystemValueByUuid(compApp.APPLICATION_FORM_ID);
                        if (svForm != null)
                        {
                            content = appendCertContent(content, appendDoubleQuote(svForm.CODE));
                        }
                    }
                    else
                    {
                        content = appendCertContent(content, appendDoubleQuote(""));
                    }

                    // handle parameters from UI
                    if ("CHECKED".Equals(missingItemRestorationNullChk))
                    {
                        content = appendCertContent(content, appendDoubleQuote("N"));
                        content = appendCertContent(content, appendDoubleQuote("N"));
                        content = appendCertContent(content, appendDoubleQuote("N"));
                        content = appendCertContent(content, appendDoubleQuote("N"));
                    }
                    else
                    {
                        content = appendCertContent(content, appendDoubleQuote("CHECKED".Equals(missingItemRestorationChequeChk) ? "Y" : "N"));
                        content = appendCertContent(content, appendDoubleQuote("CHECKED".Equals(missingItemRestorationProfRegCertChk) ? "Y" : "N"));
                        content = appendCertContent(content, appendDoubleQuote("CHECKED".Equals(missingItemRestorationOthersChk) ? "Y" : "N"));
                        content = appendCertContent(content, appendDoubleQuote("CHECKED".Equals(missingItemRestorationIncompleteFormChk) ? "Y" : "N"));
                    }

                    // get sCatCode
                    C_S_CATEGORY_CODE sCatCode = getCompSCategoryCode(compApp);
                    if (sCatCode != null)
                    {
                        content = appendCertContent(content, appendDoubleQuote(sCatCode.ENGLISH_DESCRIPTION));
                    }
                    else
                    {
                        content = appendCertContent(content, appendDoubleQuote(""));
                    }
                }
            }
            return content;
        }

        // get cert seq no from oracle function
        public String getCertSeqNo(String type, String isUpdateSeq)
        {
            type = "CERTIFICATE_" + type;

            Dictionary<string, object> QueryParameters = new Dictionary<string, object>();
            QueryParameters.Add("type", type);
            QueryParameters.Add("isUpdateSeq", isUpdateSeq);

            String q = "select C_GET_CERT_SEQ_NO(:type, :isUpdateSeq) as CERT_SEQ_NO from dual";

            String resultSeqNo = "";
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                using (DbConnection conn = db.Database.Connection)
                {
                    DbDataReader dr = CommonUtil.GetDataReader(conn, q, QueryParameters);
                    List<Dictionary<string, object>> Data = CommonUtil.LoadDbData(dr);
                    if (Data.Count > 0)
                    {
                        resultSeqNo = Data[0]["CERT_SEQ_NO"].ToString();
                    }
                    conn.Close();
                }
            }

            return resultSeqNo;
        }

        public C_IND_CERTIFICATE getIndCertificateByUuid(String indCertUuid)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                C_IND_CERTIFICATE indCertificate = db.C_IND_CERTIFICATE.Where(o => o.UUID == indCertUuid).FirstOrDefault();
                return indCertificate;
            }
        }

        public C_IND_APPLICATION getIndApplicationByUuid(String uuid)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                C_IND_APPLICATION indApplication = db.C_IND_APPLICATION.Where(o => o.UUID == uuid).FirstOrDefault();
                return indApplication;
            }
        }

        public C_COMP_APPLICATION getCompApplicationByUuid(String compAppUuid)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                C_COMP_APPLICATION application = db.C_COMP_APPLICATION.Where(o => o.UUID == compAppUuid).FirstOrDefault();
                return application;
            }
        }

        public C_S_SYSTEM_VALUE getSSystemValueByUuid(String svUuid)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                C_S_SYSTEM_VALUE sv = db.C_S_SYSTEM_VALUE.Where(o => o.UUID == svUuid).FirstOrDefault();
                return sv;
            }
        }

        public C_S_CATEGORY_CODE getSCategoryCodeByUuid(String uuid)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                C_S_CATEGORY_CODE sCategoryCode = db.C_S_CATEGORY_CODE.Where(o => o.UUID == uuid).FirstOrDefault();
                return sCategoryCode;
            }
        }

        public C_S_CATEGORY_CODE_DETAIL getSCategoryCodeDetaiByUuid(String uuid)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                C_S_CATEGORY_CODE_DETAIL sCategoryCodeDetail = db.C_S_CATEGORY_CODE_DETAIL.Where(o => o.UUID == uuid).FirstOrDefault();
                return sCategoryCodeDetail;
            }
        }

        public C_S_CATEGORY_CODE getCompSCategoryCode(C_COMP_APPLICATION C_COMP_APPLICATION)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                C_S_CATEGORY_CODE sCatCode = db.C_S_CATEGORY_CODE.Where(o => o.UUID == C_COMP_APPLICATION.CATEGORY_ID).FirstOrDefault();
                return sCatCode;
            }
        }

        public C_ADDRESS getAddressByUuid(String uuid)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                C_ADDRESS address = db.C_ADDRESS.Where(o => o.UUID == uuid).FirstOrDefault();
                return address;
            }
        }

        public C_S_AUTHORITY getAuthorityByUuid(String uuid)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                C_S_AUTHORITY sAuthority = db.C_S_AUTHORITY.Where(o => o.UUID == uuid).FirstOrDefault();
                return sAuthority;
            }
        }

        public C_APPLICANT getApplicantByUuid(String uuid)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                C_APPLICANT app = db.C_APPLICANT.Where(o => o.UUID == uuid).FirstOrDefault();
                return app;
            }
        }

        public List<C_COMP_APPLICANT_MW_ITEM> getCompApplicantMwItem(String compAppInfoUuid)
        {
            List<C_COMP_APPLICANT_MW_ITEM> resultList = null;
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                resultList = (from compAppMwItem in db.C_COMP_APPLICANT_MW_ITEM
                              where 1 == 1
                              && compAppMwItem.COMPANY_APPLICANTS_ID == compAppInfoUuid
                              select compAppMwItem).ToList();
            }

            return resultList;
        }

        public List<C_COMP_APPLICANT_INFO> getCompApplicantInfo(String compAppUuid)
        {
            List<C_COMP_APPLICANT_INFO> resultList = null;
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                resultList = (from compAppInfo in db.C_COMP_APPLICANT_INFO
                              where 1 == 1
                              && compAppInfo.MASTER_ID == compAppUuid
                              select compAppInfo).ToList();
            }

            return resultList;
        }


        public List<C_IND_QUALIFICATION_DETAIL> getIndQualificationDetailsListByIndQualUuid(String indQualUuid)
        {
            List<C_IND_QUALIFICATION_DETAIL> resultList = null;

            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                resultList = (from indQualDetail in db.C_IND_QUALIFICATION_DETAIL
                              where 1 == 1
                              && indQualDetail.IND_QUALIFICATION_ID == indQualUuid
                              select indQualDetail).ToList();
            }
            return resultList;
        }

        public List<C_IND_APPLICATION_MW_ITEM> getIndApplicationMwItemByMasterId(String masterId)
        {
            List<C_IND_APPLICATION_MW_ITEM> resultList = null;

            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                resultList = (from indAppMwItem in db.C_IND_APPLICATION_MW_ITEM
                              where 1 == 1
                              && indAppMwItem.MASTER_ID == masterId
                              select indAppMwItem).ToList();
            }
            return resultList;
        }

        public List<C_S_SYSTEM_VALUE> getIndApplicationMwItemByIndAppUuid(String IndAppUuid)
        {
            List<C_S_SYSTEM_VALUE> resultList = null;

            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                resultList = (from indAppMwItem in db.C_IND_APPLICATION_MW_ITEM
                              join svMwItem in db.C_S_SYSTEM_VALUE on indAppMwItem.ITEM_DETAILS_ID equals svMwItem.UUID
                              where 1 == 1
                              && indAppMwItem.MASTER_ID == IndAppUuid
                              select svMwItem).ToList();
            }
            return resultList;
        }

        public List<C_IND_QUALIFICATION> getIndQualificationListByMasterId(String indAppUuid)
        {
            List<C_IND_QUALIFICATION> resultList = null;

            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                resultList = (from indQualification in db.C_IND_QUALIFICATION
                              where 1 == 1
                              && indQualification.MASTER_ID == indAppUuid
                              select indQualification).ToList();
            }
            return resultList;
        }

        public List<C_S_SYSTEM_VALUE> getMwItemDetailByApplication(String indApplicationUuid, int process)
        {
            List<C_S_SYSTEM_VALUE> resultList = new List<C_S_SYSTEM_VALUE>(); 
            List<C_IND_APPLICATION_MW_ITEM_MASTER> indAppMwItemMasterList = new List<C_IND_APPLICATION_MW_ITEM_MASTER>();

            // by default all forms
            var forms = new String[] { RegistrationConstant.FORM_BA26, RegistrationConstant.FORM_BA26A
                , RegistrationConstant.FORM_BA26B, RegistrationConstant.FORM_BA26C };
            if (RegistrationConstant.NEW_REG == process)
            {
                forms = new String[] { RegistrationConstant.FORM_BA26, RegistrationConstant.FORM_BA26C };
            }
            else if (RegistrationConstant.RETENTION == process)
            {
                forms = new String[] { RegistrationConstant.FORM_BA26A };
            }
            else if (RegistrationConstant.RESTORATION == process)
            {
                forms = new String[] { RegistrationConstant.FORM_BA26B };
            }

            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                var query1 = (from m in db.C_IND_APPLICATION_MW_ITEM_MASTER
                              join ia in db.C_IND_APPLICATION on m.MASTER_ID equals ia.UUID
                              join sv in db.C_S_SYSTEM_VALUE on m.APPLICATION_FORM_ID equals sv.UUID
                              where 1 == 1
                              && forms.Contains(sv.CHINESE_DESCRIPTION)
                              select m).ToList();
                indAppMwItemMasterList = query1;

                if (indAppMwItemMasterList.Count > 0)
                {
                    C_IND_APPLICATION_MW_ITEM_MASTER indAppMwItemMaster = indAppMwItemMasterList.ElementAt(0);
                    var query2 = (from sv in db.C_S_SYSTEM_VALUE
                                  join d in db.C_IND_APPLICATION_MW_ITEM_DETAIL on sv.UUID equals d.ITEM_DETAILS_ID
                                  where 1 == 1
                                  && d.STATUS_CODE == RegistrationConstant.IND_ITEM_DETAIL_STATUS_APPLY
                                  && d.IND_APP_MW_ITEM_MASTER_ID == indAppMwItemMaster.UUID
                                  orderby sv.CODE.Substring(0, 8) ascending 
                                  select sv).ToList();
                    resultList = query2;
                }
            }

            return resultList;
        }

        public List<C_COMP_APPLICANT_INFO> getCompApplicantInfoForExportData(String compAppUuid, DateTime date)
        {
            List<C_COMP_APPLICANT_INFO> resultList = null;


            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                resultList = (from compAppInfo in db.C_COMP_APPLICANT_INFO
                              join svRole in db.C_S_SYSTEM_VALUE on compAppInfo.APPLICANT_ROLE_ID equals svRole.UUID
                              join svStatus in db.C_S_SYSTEM_VALUE on compAppInfo.APPLICANT_STATUS_ID equals svStatus.UUID
                              join app in db.C_APPLICANT on compAppInfo.APPLICANT_ID equals app.UUID
                              where 1 == 1
                              && compAppInfo.MASTER_ID == compAppUuid
                              && (svRole.CODE == "AS" || svRole.CODE == "TD")
                              && svStatus.CODE == "1"
                              && compAppInfo.ACCEPT_DATE <= date
                              && (compAppInfo.REMOVAL_DATE == null || compAppInfo.REMOVAL_DATE >= date)
                              orderby app.SURNAME, app.GIVEN_NAME_ON_ID, svRole.CODE
                              select compAppInfo).ToList();

            }

            return resultList;
        }

        public string appendCertContent(String certContent, string appendStr)
        {
            certContent += String.IsNullOrEmpty(certContent) ? appendStr : "," + appendStr;
            return certContent;
        }
    }
}

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

namespace MWMS2.Services
{
    public class RegistrationCNVService
    {
     
        string SearchCNV_q = ""
           + "\r\n" + "\t" + "select * from (                                                                                                                                                      "
           + "\r\n" + "\t" + "(select con.UUID  ,case when con.REGISTRATION_TYPE ='CGC' then 'Company' end as ConvictionType ,con.BR_NO ,con.CR_SECTION , con.CR_OFFENCE_DATE as CR_OFFENCE_DATE, con.CR_JUDGE_DATE as CR_JUDGE_DATE, con.srr_Approval_Date as srr_Approval_Date, con.srr_Effective_Date as srr_Effective_Date, con.da_Decision_Date as da_Decision_Date, con.misc_Receiving_Date as  misc_Receiving_Date ,                      "
           + "\r\n" + "\t" + "case when con.CR_ACCIDENT= 'N' and con.CR_FATAL='N' then ''                                                                                                                                  "
           + "\r\n" + "\t" + "when con.CR_ACCIDENT= 'Y' and con.CR_FATAL='N' then 'A'                                                                                                                                      "
           + "\r\n" + "\t" + "when con.CR_ACCIDENT= 'N' and con.CR_FATAL='Y' then 'F'                                                                                                                                      "
           + "\r\n" + "\t" + "when con.CR_ACCIDENT= 'Y' and con.CR_FATAL='Y' then 'A / F' else '' end as AccidentFatal,                                                                                                    "
           + "\r\n" + "\t" + "con.REFERENCE,con.RECORD_TYPE as RECORD_TYPE  , cast(con.ENGLISH_NAME as varchar2(2000) )as name ,                                                                                                           "
           + "\r\n" + "\t" + "con.ENGLISH_NAME,null as HKID                                                                                                                                                                "
           + "\r\n" + "\t" + ",null as PASSPORT                                                                                                                                                                "
           + "\r\n" + "\t" + ", null as SURNAME"
           + "\r\n" + "\t" + ", null as GIVENAME"
           + "\r\n" + "\t" + ", null as CHINESE_NAME"
           + "\r\n" + "\t" + ", SRR_SUSPENSION_DETAILS"
           + "\r\n" + "\t" + ", PROPRI_NAME"
           + "\r\n" + "\t" + ", CONVICTION_SOURCE_ID"

           + "\r\n" + "\t" + "FROM C_Comp_Conviction con                                                                                                                                                                   "
           + "\r\n" + "\t" + "WHERE con.registration_Type = 'CGC'                                                                                                                                                          "
           + "\r\n" + "\t" + "and (                                                                                                                                                                                        "
           + "\r\n" + "\t" + "(con.Conviction_Source_Id                                                                                                                                                                    "
           + "\r\n" + "\t" + "in                                                                                                                                                                                           "
           + "\r\n" + "\t" + "(  select gc.CONVICTION_ID                                                                                                                                                                   "
           + "\r\n" + "\t" + "from C_S_User_Group_Conv_Info gc, SYS_ROLE ug                                                                                                                             "
           + "\r\n" + "\t" + "where gc.sys_role_id=ug.uuid and ug.uuid = '1' ) )                                                                                       "
           + "\r\n" + "\t" + "or  con.Conviction_Source_Id is null )                                                                                                                                                       "
           + "\r\n" + "\t" + ")                                                                                                                                                                                            "
          
           + "\r\n" + "\t" + "union                                                                                                                                                                                        "
           + "\r\n" + "\t" + "(                                                                                                                                                                                            "
           + "\r\n" + "\t" + "select Indcon.UUID, case when Indcon.REGISTRATION_TYPE ='IP' then 'Individual' end ,null,Indcon.CR_SECTION, Indcon.CR_OFFENCE_DATE as CR_OFFENCE_DATE, Indcon.CR_JUDGEMENT_DATE as CR_JUDGE_DATE,  Indcon.srr_Approval_Date as srr_Approval_Date, Indcon.SRR_EFFECT_DATE as srr_Effective_Date, Indcon.da_Decision_Date as da_Decision_Date, Indcon.misc_Receiving_Date as  misc_Receiving_Date ,                               "
           + "\r\n" + "\t" + "case when Indcon.CR_ACCIDENT= 'N' and Indcon.CR_FATAL='N' then ''                                                                                                                            "
           + "\r\n" + "\t" + "when Indcon.CR_ACCIDENT= 'Y' and Indcon.CR_FATAL='N' then 'A'                                                                                                                                "
           + "\r\n" + "\t" + "when Indcon.CR_ACCIDENT= 'N' and Indcon.CR_FATAL='Y' then 'F'                                                                                                                                "
           + "\r\n" + "\t" + "when Indcon.CR_ACCIDENT= 'Y' and Indcon.CR_FATAL='Y' then 'A / F' else '' end as AccidentFatal,                                                                                              "
           + "\r\n" + "\t" + "null,Indcon.RECORD_TYPE as RECORD_TYPE , cast(Indcon.SURNAME|| ' ' ||Indcon.GIVEN_NAME as varchar2(2000) ) as name,                                                                                         "
           + "\r\n" + "\t" + "Indcon.ENGLISH_COMPANY_NAME," + EncryptDecryptUtil.getDecryptSQL("Indcon.hkid") + " as HKID ,"
           + "\r\n" + "\t" + EncryptDecryptUtil.getDecryptSQL("Indcon.PASSPORT_NO") + " as PASSPORT "
           + "\r\n" + "\t" + ",Indcon.SURNAME"
           + "\r\n" + "\t" + ",Indcon.GIVEN_NAME"
           + "\r\n" + "\t" + ",Indcon.CHINESE_NAME"
           + "\r\n" + "\t" + ",Indcon.SRR_DETAILS"
           + "\r\n" + "\t" + ",null as PROPRI_NAME "
           + "\r\n" + "\t" + ",null as CONVICTION_SOURCE_ID"


           + "\r\n" + "\t" + "FROM C_ind_Conviction Indcon                                                                                                                                                                 "
           + "\r\n" + "\t" + "WHERE Indcon.registration_Type = 'IP'                                                                                                                                                        "
           + "\r\n" + "\t" + "and (                                                                                                                                                                                        "
           + "\r\n" + "\t" + "(Indcon.Conviction_Source_Id                                                                                                                                                                 "
           + "\r\n" + "\t" + "in                                                                                                                                                                                           "
           + "\r\n" + "\t" + "(  select gc.CONVICTION_ID                                                                                                                                                                   "
           + "\r\n" + "\t" + "from C_S_User_Group_Conv_Info gc, SYS_ROLE ug                                                                                                                             "
           + "\r\n" + "\t" + "where gc.sys_role_id=ug.uuid and ug.uuid = '1' ) )                                                                                       "
           + "\r\n" + "\t" + "or  Indcon.Conviction_Source_Id is null )                                                                                                                                                    "
           + "\r\n" + "\t" + ")                                                                                                                                                                                            "

           + "\r\n" + "\t" + "union                                                                                                                                                                                        "
            + "\r\n" + "\t" + "(select con.UUID  ,case when con.REGISTRATION_TYPE ='CMW' then 'Company' end as ConvictionType ,con.BR_NO ,con.CR_SECTION , con.CR_OFFENCE_DATE as CR_OFFENCE_DATE, con.CR_JUDGE_DATE as CR_JUDGE_DATE, con.srr_Approval_Date as srr_Approval_Date, con.srr_Effective_Date as srr_Effective_Date, con.da_Decision_Date as da_Decision_Date, con.misc_Receiving_Date as  misc_Receiving_Date ,                      "
           + "\r\n" + "\t" + "case when con.CR_ACCIDENT= 'N' and con.CR_FATAL='N' then ''                                                                                                                                  "
           + "\r\n" + "\t" + "when con.CR_ACCIDENT= 'Y' and con.CR_FATAL='N' then 'A'                                                                                                                                      "
           + "\r\n" + "\t" + "when con.CR_ACCIDENT= 'N' and con.CR_FATAL='Y' then 'F'                                                                                                                                      "
           + "\r\n" + "\t" + "when con.CR_ACCIDENT= 'Y' and con.CR_FATAL='Y' then 'A / F' else '' end as AccidentFatal,                                                                                                    "
           + "\r\n" + "\t" + "con.REFERENCE,con.RECORD_TYPE as RECORD_TYPE  , cast(con.ENGLISH_NAME as varchar2(2000) )as name ,                                                                                                           "
           + "\r\n" + "\t" + "con.ENGLISH_NAME,null as HKID                                                                                                                                                                "
           + "\r\n" + "\t" + ",null as PASSPORT                                                                                                                                                                "
           + "\r\n" + "\t" + ", null as SURNAME"
           + "\r\n" + "\t" + ", null as GIVENAME"
           + "\r\n" + "\t" + ", null as CHINESE_NAME"
           + "\r\n" + "\t" + ", SRR_SUSPENSION_DETAILS"
           + "\r\n" + "\t" + ", PROPRI_NAME"
           + "\r\n" + "\t" + ", CONVICTION_SOURCE_ID"

           + "\r\n" + "\t" + "FROM C_Comp_Conviction con                                                                                                                                                                   "
           + "\r\n" + "\t" + "WHERE con.registration_Type = 'CMW'                                                                                                                                                          "
           + "\r\n" + "\t" + "and (                                                                                                                                                                                        "
           + "\r\n" + "\t" + "(con.Conviction_Source_Id                                                                                                                                                                    "
           + "\r\n" + "\t" + "in                                                                                                                                                                                           "
           + "\r\n" + "\t" + "(  select gc.CONVICTION_ID                                                                                                                                                                   "
           + "\r\n" + "\t" + "from C_S_User_Group_Conv_Info gc, SYS_ROLE ug                                                                                                                             "
           + "\r\n" + "\t" + "where gc.sys_role_id=ug.uuid and ug.uuid = '1' ) )                                                                                       "
           + "\r\n" + "\t" + "or  con.Conviction_Source_Id is null )                                                                                                                                                       "
           + "\r\n" + "\t" + ")                                                                                                                                                                                            "

             + "\r\n" + "\t" + "union                                                                                                                                                                                        "
           + "\r\n" + "\t" + "(                                                                                                                                                                                            "
           + "\r\n" + "\t" + "select Indcon.UUID, case when Indcon.REGISTRATION_TYPE ='IMW' then 'Individual' end ,null,Indcon.CR_SECTION, Indcon.CR_OFFENCE_DATE as CR_OFFENCE_DATE, Indcon.CR_JUDGEMENT_DATE as CR_JUDGE_DATE,  Indcon.srr_Approval_Date as srr_Approval_Date, Indcon.SRR_EFFECT_DATE as srr_Effective_Date, Indcon.da_Decision_Date as da_Decision_Date, Indcon.misc_Receiving_Date as  misc_Receiving_Date ,                               "
           + "\r\n" + "\t" + "case when Indcon.CR_ACCIDENT= 'N' and Indcon.CR_FATAL='N' then ''                                                                                                                            "
           + "\r\n" + "\t" + "when Indcon.CR_ACCIDENT= 'Y' and Indcon.CR_FATAL='N' then 'A'                                                                                                                                "
           + "\r\n" + "\t" + "when Indcon.CR_ACCIDENT= 'N' and Indcon.CR_FATAL='Y' then 'F'                                                                                                                                "
           + "\r\n" + "\t" + "when Indcon.CR_ACCIDENT= 'Y' and Indcon.CR_FATAL='Y' then 'A / F' else '' end as AccidentFatal,                                                                                              "
           + "\r\n" + "\t" + "null,Indcon.RECORD_TYPE as RECORD_TYPE , cast(Indcon.SURNAME|| ' ' ||Indcon.GIVEN_NAME as varchar2(2000) ) as name,                                                                                         "
           + "\r\n" + "\t" + "Indcon.ENGLISH_COMPANY_NAME," + EncryptDecryptUtil.getDecryptSQL("Indcon.hkid") + " as HKID ,"
           + "\r\n" + "\t" + EncryptDecryptUtil.getDecryptSQL("Indcon.PASSPORT_NO") + " as PASSPORT "
           + "\r\n" + "\t" + ",Indcon.SURNAME"
           + "\r\n" + "\t" + ",Indcon.GIVEN_NAME"
           + "\r\n" + "\t" + ",Indcon.CHINESE_NAME"
           + "\r\n" + "\t" + ",Indcon.SRR_DETAILS"
           + "\r\n" + "\t" + ",null as PROPRI_NAME "
           + "\r\n" + "\t" + ",null as CONVICTION_SOURCE_ID"


           + "\r\n" + "\t" + "FROM C_ind_Conviction Indcon                                                                                                                                                                 "
           + "\r\n" + "\t" + "WHERE Indcon.registration_Type = 'IMW'                                                                                                                                                        "
           + "\r\n" + "\t" + "and (                                                                                                                                                                                        "
           + "\r\n" + "\t" + "(Indcon.Conviction_Source_Id                                                                                                                                                                 "
           + "\r\n" + "\t" + "in                                                                                                                                                                                           "
           + "\r\n" + "\t" + "(  select gc.CONVICTION_ID                                                                                                                                                                   "
           + "\r\n" + "\t" + "from C_S_User_Group_Conv_Info gc, SYS_ROLE ug                                                                                                                             "
           + "\r\n" + "\t" + "where gc.sys_role_id=ug.uuid and ug.uuid = '1' ) )                                                                                       "
           + "\r\n" + "\t" + "or  Indcon.Conviction_Source_Id is null )                                                                                                                                                    "
           + "\r\n" + "\t" + ")                                                                                                                                                                                            "                 

           + "\r\n" + "\t" + " )                                                                                                                                                                                            "
           + "\r\n" + "\t" + "where 1=1                                                                                                                                                                                    ";



        public string ExportCNV(Fn07CNV_CNVSearchModel model)
        {
            model.Query = SearchCNV_q;
            model.QueryWhere = SearchCNV_whereQ(model);
            //DisplayGrid dlr = new DisplayGrid() { Query = SearchGCA_q, Columns = Columns, Parameters = post };
            return model.Export("ExportData");
        }

        //FN07
        public Fn07CNV_CNVSearchModel SearchCNV(Fn07CNV_CNVSearchModel model)
        {
            model.Query = SearchCNV_q;
            model.QueryWhere = SearchCNV_whereQ(model);
            model.Search();
            return model;
        }

        private string SearchCNV_whereQ(Fn07CNV_CNVSearchModel model)
        {
            string whereQ = "";
            if (!string.IsNullOrWhiteSpace(model.HKID))
            {
                whereQ += "\r\n\t" + "AND " + "HKID" + " LIKE :HKID";
                model.QueryParameters.Add("HKID", "%" + model.HKID.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.PassportNo))
            {
                whereQ += "\r\n\t" + "AND " + "PASSPORT" + " LIKE :PassportNo";
                model.QueryParameters.Add("PassportNo", "%" + model.PassportNo.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.Surname))
            {
                whereQ += "\r\n\t" + "AND UPPER(SURNAME) LIKE :Surname";
                model.QueryParameters.Add("Surname", "%" + model.Surname.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.GivenName))
            {
                whereQ += "\r\n\t" + "AND UPPER(GIVENAME) LIKE :GivenName";
                model.QueryParameters.Add("GivenName", "%" + model.GivenName.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.ChiName))
            {
                whereQ += "\r\n\t" + "AND CHINESE_NAME LIKE :ChiName";
                model.QueryParameters.Add("ChiName", "%" + model.ChiName + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.ProprietorName))
            {
                whereQ += "\r\n\t" + "AND UPPER(PROPRI_NAME) LIKE :ProprietorName";
                model.QueryParameters.Add("ProprietorName", "%" + model.ProprietorName.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.BRNo))
            {
                whereQ += "\r\n\t" + "AND UPPER(BR_NO) LIKE :BRNo";
                model.QueryParameters.Add("BRNo", "%" + model.BRNo.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.ComName))
            {
                whereQ += "\r\n\t" + "AND UPPER(ENGLISH_NAME) LIKE :ComName";
                model.QueryParameters.Add("ComName", "%" + model.ComName.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.OrdReg))
            {
                whereQ += "\r\n\t" + "AND UPPER(CR_SECTION) LIKE :OrdReg";
                model.QueryParameters.Add("OrdReg", "%" + model.OrdReg.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.SuspReason))
            {
                whereQ += "\r\n\t" + "AND UPPER(SRR_SUSPENSION_DETAILS) LIKE :SuspReason";
                model.QueryParameters.Add("SuspReason", "%" + model.SuspReason.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.Ref))
            {
                whereQ += "\r\n\t" + "AND UPPER(REFERENCE) LIKE :Ref";
                model.QueryParameters.Add("Ref", "%" + model.Ref.Trim().ToUpper() + "%");
            }

            if (!string.IsNullOrWhiteSpace(model.CnvSource))
            {
                whereQ += "\r\n\t" + "AND (select sv.uuid from C_S_SYSTEM_VALUE sv where sv.uuid= CONVICTION_SOURCE_ID ) = :CnvSource";
                model.QueryParameters.Add("CnvSource", model.CnvSource);
            }
            if (model.FromDateOfOffence != null)
            {
                whereQ += "\r\n\t" + "AND CR_OFFENCE_DATE >= :DateFrom";
                model.QueryParameters.Add("DateFrom", model.FromDateOfOffence);
            }
            if (model.ToDateOfOffence != null)
            {
                whereQ += "\r\n\t" + "AND CR_OFFENCE_DATE <= :DateTo";
                model.QueryParameters.Add("DateTo", model.ToDateOfOffence);
            }
            if (model.FromDateOfJudgement != null)
            {// OR ( RECORD_TYPE='S'  AND srr_Approval_Date >= :DateFrom) )OR ( RECORD_TYPE='S'  AND (srr_Effective_Date >= :DateFrom) )OR ( RECORD_TYPE='D'  AND ( da_Decision_Date >= :DateFrom   ) )OR ( RECORD_TYPE='M'  AND ( misc_Receiving_Date >= :DateFrom  ) )
                whereQ += "\r\n\t" + "AND ((RECORD_TYPE='C'  AND ( CR_JUDGE_DATE >= :DateFrom))  OR ( RECORD_TYPE='S'  AND ( srr_Approval_Date >= :DateFrom)) OR ( RECORD_TYPE='S'  AND (srr_Effective_Date >= :DateFrom)) OR ( RECORD_TYPE='D'  AND ( da_Decision_Date >= :DateFrom   )) OR ( RECORD_TYPE='M'  AND ( misc_Receiving_Date >= :DateFrom  )  ))";
                model.QueryParameters.Add("DateFrom", model.FromDateOfJudgement);
            }
            if (model.ToDateOfJudgement != null)
            {
                whereQ += "\r\n\t" + "AND ((RECORD_TYPE='C'  AND ( CR_JUDGE_DATE <= :DateTo))  OR ( RECORD_TYPE='S'  AND ( srr_Approval_Date <= :DateTo)) OR ( RECORD_TYPE='S'  AND (srr_Effective_Date <= :DateTo)) OR ( RECORD_TYPE='D'  AND ( da_Decision_Date <= :DateTo   )) OR ( RECORD_TYPE='M'  AND ( misc_Receiving_Date <= :DateTo  )  ))";

                model.QueryParameters.Add("DateTo", model.ToDateOfJudgement);
            }
            if (model.CnvType != null)
            {
                whereQ += "\r\n\t" + "AND ConvictionType = :CnvType";
                model.QueryParameters.Add("CnvType", model.CnvType);
            }




            return whereQ;
        }

        public Fn07CNV_CNVDisplayModel ViewCompCNV(string id)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                var query = from CompCNV in db.C_COMP_CONVICTION
                            where CompCNV.UUID == id
                            select CompCNV;
                return new Fn07CNV_CNVDisplayModel()
                { C_COMP_CONVICTION = query.FirstOrDefault()
                };
            }

        }
        public Fn07CNV_CNVDisplayModel ViewIndCNV(string id)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                var IndQuery = from IndCNV in db.C_IND_CONVICTION
                               where IndCNV.UUID == id
                               select IndCNV;
                return new Fn07CNV_CNVDisplayModel()
                { C_IND_CONVICTION = IndQuery.FirstOrDefault() };
            }

        }

        public void CreateCNV(Fn07CNV_CNVSearchModel model)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                C_S_SYSTEM_VALUE sv = new C_S_SYSTEM_VALUE();
                C_COMP_CONVICTION cmp = new C_COMP_CONVICTION();
                C_IND_CONVICTION ind = new C_IND_CONVICTION();

                var RegType = model.RegType;

                if (RegType == "CGC" || RegType == "CMW")
                {
                    cmp.UUID = Guid.NewGuid().ToString();

                    cmp.REGISTRATION_TYPE = model.RegType;
                    cmp.ENGLISH_NAME = model.ComName;
                    cmp.CHINESE_NAME = model.ChiComName;
                    cmp.PROPRI_NAME = model.ProprietorName;
                    cmp.BR_NO = model.BRNo;
                    cmp.REFERENCE = model.Ref;
                    cmp.COMPANY_TYPE_ID = model.CompanyType;
                    cmp.SITE_DESCRIPTION = model.SiteDesc;
                    cmp.CONVICTION_SOURCE_ID = model.ConvictionSource;
                    cmp.REMARKS = model.Remarks;
                    cmp.RECORD_TYPE = model.RecordType;
                    //CR
                    cmp.CR_SECTION = model.OrdReg;
                    cmp.CR_OFFENCE_DATE = model.DateOfOffence;
                    cmp.CR_JUDGE_DATE = model.DateOfJudgement;
                    cmp.CR_FINE = model.Fine;
                    cmp.CR_ACCIDENT = model.Accident;
                    cmp.CR_FATAL = model.Fatal;
                    cmp.CR_REPORT = model.CrReport == true ? "Y" : "N";

                    //SSR
                    cmp.SRR_ACTION = model.SrrAction;
                    cmp.SRR_EFFECTIVE_DATE = model.EffectiveDate;
                    cmp.SRR_APPROVAL_DATE = model.DateOfApproval;
                    cmp.SRR_SUSPENSION_FROM_DATE = model.SuspFromDate;
                    cmp.SRR_SUSPENSION_TO_DATE = model.SuspToDate;
                    cmp.SRR_CATEGORY = model.Category;
                    cmp.SRR_SUSPENSION_DETAILS = model.SrrDetails;
                    cmp.SRR_REPORT = model.SrrReport == true ? "Y" : "N";
                    //DA
                    cmp.DA_DECISION_DATE = model.DateOfDecision;
                    cmp.DA_DETAILS = model.DaDetails;
                    cmp.DA_REPORT = model.DaReport == true ? "Y" : "N";
                    //MISC
                    cmp.MISC_RECEIVING_DATE = model.ReceivingDate;
                    cmp.MISC_DETAILS = model.MiscDetails;
                    cmp.MISC_REPORT = model.MiscReport == true ? "Y" : "N";

                    //new
                    cmp.AS_NAME_ENG = model.AsTdOoEngName;
                    cmp.AS_NAME_CHN = model.AsTdOoChiName;
                    cmp.ID_NO = model.C_APPLICANT.HKID == null ? null : (EncryptDecryptUtil.getEncrypt(model.C_APPLICANT.HKID.Trim().ToUpper()));
                    cmp.RECORD_CLEARED = model.RecordClear;
                    cmp.CRC_INTERVIEW = model.CRCInterview;
                    cmp.CRC_INTERVIEW_DATE = model.CRCInterviewDt;
                    
                    //new add 7 fields to save 7 checkbox selection
                    cmp.GBC_SELECT = model.Category1 == true ? "Y" : "N";
                    cmp.SC_D_SELECT = model.Category2 == true ? "Y" : "N";
                    cmp.SC_F_SELECT = model.Category3 == true ? "Y" : "N";
                    cmp.SC_GI_SELECT = model.Category4 == true ? "Y" : "N";
                    cmp.SC_SF_SELECT = model.Category5 == true ? "Y" : "N";
                    cmp.SC_V_SELECT = model.Category6 == true ? "Y" : "N";
                    cmp.MWC_CO_SELECT = model.Category7 == true ? "Y" : "N";

                    cmp.MODIFIED_DATE = System.DateTime.Now;
                    cmp.MODIFIED_BY = SystemParameterConstant.UserName;
                    cmp.CREATED_DATE = System.DateTime.Now;
                    cmp.CREATED_BY = SystemParameterConstant.UserName;

                    db.C_COMP_CONVICTION.Add(cmp);
                    db.SaveChanges();

                }

                else if (RegType == "IP" || RegType == "IMW")
                {

                    ind.UUID = Guid.NewGuid().ToString();

                    ind.REGISTRATION_TYPE = model.RegType;
                    ind.SURNAME = model.Surname;
                    ind.GIVEN_NAME = model.GivenName;
                    ind.CHINESE_NAME = model.ChiName;
                    ind.GENDER = model.Sex;
                    ind.HKID = model.HKID == null ? null : (EncryptDecryptUtil.getEncrypt(model.HKID.Trim().ToUpper()));
                    ind.PASSPORT_NO = model.PassportNo == null ? null : (EncryptDecryptUtil.getEncrypt(model.PassportNo.Trim().ToUpper()));
                    ind.ENGLISH_COMPANY_NAME = model.ComName;
                    ind.CHINESE_COMPANY_NAME = model.ChiComName;
                    ind.SITE_DESCRIPTION = model.SiteDesc;
                    ind.CONVICTION_SOURCE_ID = model.ConvictionSource;
                    ind.REMARKS = model.Remarks;
                    ind.RECORD_TYPE = model.RecordType;

                    //CR
                    ind.CR_SECTION = model.OrdReg;
                    ind.CR_OFFENCE_DATE = model.DateOfOffence;
                    ind.CR_JUDGEMENT_DATE = model.DateOfJudgement;
                    ind.CR_FINE = model.Fine;
                    ind.CR_ACCIDENT = model.Accident;
                    ind.CR_FATAL = model.Fatal;
                    ind.CR_REPORT = model.CrReport == true ? "Y" : "N";

                    //SSR
                    ind.SRR_ACTION = model.SrrAction;
                    ind.SRR_EFFECT_DATE = model.EffectiveDate;
                    ind.SRR_APPROVAL_DATE = model.DateOfApproval;
                    ind.SRR_SUSPENSION_FROM_DATE = model.SuspFromDate;
                    ind.SRR_SUSPENSION_TO_DATE = model.SuspToDate;
                    ind.SRR_CATEGORY = model.Category;
                    ind.SRR_DETAILS = model.SrrDetails;
                    ind.SRR_REPORT = model.SrrReport == true ? "Y" : "N";
                    //DA
                    ind.DA_DECISION_DATE = model.DateOfDecision;
                    ind.DA_DETAILS = model.DaDetails;
                    ind.DA_REPORT = model.DaReport == true ? "Y" : "N";
                    //MISC
                    ind.MISC_RECEIVING_DATE = model.ReceivingDate;
                    ind.MISC_DETAILS = model.MiscDetails;
                    ind.MISC_REPORT = model.MiscReport == true ? "Y" : "N";

                    ind.MODIFIED_DATE = System.DateTime.Now;
                    ind.MODIFIED_BY = SystemParameterConstant.UserName;
                    ind.CREATED_DATE = System.DateTime.Now;
                    ind.CREATED_BY = SystemParameterConstant.UserName;

                    db.C_IND_CONVICTION.Add(ind);
                    db.SaveChanges();
                }
            }
        }

        public void SaveComp(Fn07CNV_CNVDisplayModel model)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                var query = db.C_COMP_CONVICTION.Find(model.C_COMP_CONVICTION.UUID);

                query.ENGLISH_NAME = model.C_COMP_CONVICTION.ENGLISH_NAME;
                query.CHINESE_NAME = model.C_COMP_CONVICTION.CHINESE_NAME;
                query.PROPRI_NAME = model.C_COMP_CONVICTION.PROPRI_NAME;
                query.BR_NO = model.C_COMP_CONVICTION.BR_NO;
                query.REFERENCE = model.C_COMP_CONVICTION.REFERENCE;
                query.COMPANY_TYPE_ID = model.C_COMP_CONVICTION.COMPANY_TYPE_ID;
                query.SITE_DESCRIPTION = model.C_COMP_CONVICTION.SITE_DESCRIPTION;
                query.CONVICTION_SOURCE_ID = model.C_COMP_CONVICTION.CONVICTION_SOURCE_ID;
                query.REMARKS = model.C_COMP_CONVICTION.REMARKS;
                query.RECORD_TYPE = model.C_COMP_CONVICTION.RECORD_TYPE;

                query.CR_SECTION = model.C_COMP_CONVICTION.CR_SECTION;
                query.CR_OFFENCE_DATE = model.C_COMP_CONVICTION.CR_OFFENCE_DATE;
                query.CR_JUDGE_DATE = model.C_COMP_CONVICTION.CR_JUDGE_DATE;
                query.CR_FINE = model.C_COMP_CONVICTION.CR_FINE;
                query.CR_ACCIDENT = model.C_COMP_CONVICTION.CR_ACCIDENT;
                query.CR_FATAL = model.C_COMP_CONVICTION.CR_FATAL;
                query.REMARKS = model.C_COMP_CONVICTION.REMARKS;
                query.CR_REPORT = model.C_COMP_CONVICTION.CR_REPORT;

                query.SRR_ACTION = model.C_COMP_CONVICTION.SRR_ACTION;
                query.SRR_EFFECTIVE_DATE = model.C_COMP_CONVICTION.SRR_EFFECTIVE_DATE;
                query.SRR_APPROVAL_DATE = model.C_COMP_CONVICTION.SRR_APPROVAL_DATE;
                query.SRR_SUSPENSION_FROM_DATE = model.C_COMP_CONVICTION.SRR_SUSPENSION_FROM_DATE;
                query.SRR_SUSPENSION_TO_DATE = model.C_COMP_CONVICTION.SRR_SUSPENSION_TO_DATE;
                query.SRR_CATEGORY = model.C_COMP_CONVICTION.SRR_CATEGORY;
                query.SRR_SUSPENSION_DETAILS = model.C_COMP_CONVICTION.SRR_SUSPENSION_DETAILS;
                query.SRR_REPORT = model.C_COMP_CONVICTION.SRR_REPORT;

                query.DA_DECISION_DATE = model.C_COMP_CONVICTION.DA_DECISION_DATE;
                query.DA_DETAILS = model.C_COMP_CONVICTION.DA_DETAILS;
                query.DA_REPORT = model.C_COMP_CONVICTION.DA_REPORT;

                query.MISC_RECEIVING_DATE = model.C_COMP_CONVICTION.MISC_RECEIVING_DATE;
                query.MISC_DETAILS = model.C_COMP_CONVICTION.MISC_DETAILS;
                query.MISC_REPORT = model.C_COMP_CONVICTION.MISC_REPORT;

                query.AS_NAME_ENG = model.C_COMP_CONVICTION.AS_NAME_ENG;
                query.AS_NAME_CHN = model.C_COMP_CONVICTION.AS_NAME_CHN;
                query.ID_NO = model.C_COMP_CONVICTION.ID_NO == null ? null : (EncryptDecryptUtil.getEncrypt(model.C_COMP_CONVICTION.ID_NO.Trim().ToUpper()));
                query.RECORD_CLEARED = model.C_COMP_CONVICTION.RECORD_CLEARED;
                query.CRC_INTERVIEW = model.C_COMP_CONVICTION.CRC_INTERVIEW;
                query.CRC_INTERVIEW_DATE = model.C_COMP_CONVICTION.CRC_INTERVIEW_DATE;
                query.GBC_SELECT = model.C_COMP_CONVICTION.GBC_SELECT;
                query.SC_D_SELECT = model.C_COMP_CONVICTION.SC_D_SELECT;
                query.SC_F_SELECT = model.C_COMP_CONVICTION.SC_F_SELECT;
                query.SC_GI_SELECT = model.C_COMP_CONVICTION.SC_GI_SELECT;
                query.SC_SF_SELECT = model.C_COMP_CONVICTION.SC_SF_SELECT;
                query.SC_V_SELECT = model.C_COMP_CONVICTION.SC_V_SELECT;
                query.MWC_CO_SELECT = model.C_COMP_CONVICTION.MWC_CO_SELECT;

                query.MODIFIED_DATE = System.DateTime.Now;
                query.MODIFIED_BY = SystemParameterConstant.UserName; //"Admin";

                db.SaveChanges();
            }
        }

        public void SaveIndForm(Fn07CNV_CNVDisplayModel model)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                var query = db.C_IND_CONVICTION.Find(model.C_IND_CONVICTION.UUID);

                query.SURNAME = model.C_IND_CONVICTION.SURNAME;
                query.GIVEN_NAME = model.C_IND_CONVICTION.GIVEN_NAME;
                query.CHINESE_NAME = model.C_IND_CONVICTION.CHINESE_NAME;
                query.HKID = model.C_IND_CONVICTION.HKID == null ? null : (EncryptDecryptUtil.getEncrypt(model.C_IND_CONVICTION.HKID.Trim().ToUpper()));
                query.PASSPORT_NO = model.C_IND_CONVICTION.PASSPORT_NO == null ? null : (EncryptDecryptUtil.getEncrypt(model.C_IND_CONVICTION.PASSPORT_NO.Trim().ToUpper()));
                query.ENGLISH_COMPANY_NAME = model.C_IND_CONVICTION.ENGLISH_COMPANY_NAME;
                query.CHINESE_COMPANY_NAME = model.C_IND_CONVICTION.CHINESE_COMPANY_NAME;
                query.SITE_DESCRIPTION = model.C_IND_CONVICTION.SITE_DESCRIPTION;
                query.CONVICTION_SOURCE_ID = model.C_IND_CONVICTION.CONVICTION_SOURCE_ID;
                query.REMARKS = model.C_IND_CONVICTION.REMARKS;
                query.RECORD_TYPE = model.C_IND_CONVICTION.RECORD_TYPE;

                query.CR_SECTION = model.C_IND_CONVICTION.CR_SECTION;
                query.CR_OFFENCE_DATE = model.C_IND_CONVICTION.CR_OFFENCE_DATE;
                query.CR_JUDGEMENT_DATE = model.C_IND_CONVICTION.CR_JUDGEMENT_DATE;
                query.CR_FINE = model.C_IND_CONVICTION.CR_FINE;
                query.CR_ACCIDENT = model.C_IND_CONVICTION.CR_ACCIDENT;
                query.CR_FATAL = model.C_IND_CONVICTION.CR_FATAL;
                query.CR_REPORT = model.C_IND_CONVICTION.CR_REPORT;

                query.SRR_ACTION = model.C_IND_CONVICTION.SRR_ACTION;
                query.SRR_EFFECT_DATE = model.C_IND_CONVICTION.SRR_EFFECT_DATE;
                query.SRR_APPROVAL_DATE = model.C_IND_CONVICTION.SRR_APPROVAL_DATE;
                query.SRR_SUSPENSION_FROM_DATE = model.C_IND_CONVICTION.SRR_SUSPENSION_FROM_DATE;
                query.SRR_SUSPENSION_TO_DATE = model.C_IND_CONVICTION.SRR_SUSPENSION_TO_DATE;
                query.SRR_CATEGORY = model.C_IND_CONVICTION.SRR_CATEGORY;
                query.SRR_DETAILS = model.C_IND_CONVICTION.SRR_DETAILS;
                query.SRR_REPORT = model.C_IND_CONVICTION.SRR_REPORT;

                query.DA_DECISION_DATE = model.C_IND_CONVICTION.DA_DECISION_DATE;
                query.DA_DETAILS = model.C_IND_CONVICTION.DA_DETAILS;
                query.DA_REPORT = model.C_IND_CONVICTION.DA_REPORT;

                query.MISC_RECEIVING_DATE = model.C_IND_CONVICTION.MISC_RECEIVING_DATE;
                query.MISC_DETAILS = model.C_IND_CONVICTION.MISC_DETAILS;
                query.MISC_REPORT = model.C_IND_CONVICTION.MISC_REPORT;

                query.MODIFIED_DATE = System.DateTime.Now;
                query.MODIFIED_BY = SystemParameterConstant.UserName; //"Admin";

                db.SaveChanges();

            }
        }

        public void DeleteRecord(string id)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                C_COMP_CONVICTION compCNV = db.C_COMP_CONVICTION.Where(o => o.UUID == id).FirstOrDefault();
                db.C_COMP_CONVICTION.Remove(compCNV);
                db.SaveChanges();
                
            }
        }
        public void DeleteIndRecord(string id)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                C_IND_CONVICTION compCNVInd = db.C_IND_CONVICTION.Where(o => o.UUID == id).FirstOrDefault();
                db.C_IND_CONVICTION.Remove(compCNVInd);
                db.SaveChanges();
            }
        }

        public Fn07CNV_CNVSearchModel GetApplicant(Fn07CNV_CNVSearchModel model, string hkid)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                if (string.IsNullOrWhiteSpace(hkid))
                {
                    model.ErrorMessage = "HKID cannot be empty.";
                    return model;
                }
                string tempHKID = EncryptDecryptUtil.getEncrypt(hkid);
                bool tempResult = string.IsNullOrWhiteSpace(tempHKID);
                //string tempPASSPORTNO = string.IsNullOrWhiteSpace(passportno) ? EncryptDecryptUtil.getEncrypt("nopasssport") : EncryptDecryptUtil.getEncrypt(passportno);
                var queryApp = (from IndApp in db.C_IND_APPLICATION
                                join Applicant in db.C_APPLICANT on IndApp.APPLICANT_ID equals Applicant.UUID
                                where !tempResult ? Applicant.HKID == tempHKID : Applicant.HKID == null
                                select new {Applicant }).FirstOrDefault();

                if (queryApp != null)
                {
                    model.HKID = queryApp.Applicant.HKID;
                    //model.Surname = queryApp.SURNAME;
                    //model.GivenName = queryApp.GIVEN_NAME_ON_ID;
                  
                    model.AsTdOoEngName = queryApp.Applicant.FULL_NAME_DISPLAY;
                    model.AsTdOoChiName = queryApp.Applicant.CHINESE_NAME;
                    model.HKID = EncryptDecryptUtil.getDecryptHKID(queryApp.Applicant.HKID);

                    //model.C_APPLICANT.CHINESE_NAME = queryApp.Applicant.FULL_NAME_DISPLAY;

                    // model.AsTdOoEngName = queryApp.Applicant.SURNAME + " " + queryApp.Applicant.GIVEN_NAME_ON_ID;
                }
                else
                {
                    ///  model.result = false;
                    model.ErrorMessage = "HKID does not exists.";

                }

            }

            return model;
        }

    }
}
using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Models
{
    public class ProcessMonitorDisplayModel : DisplayGrid, IValidatableObject
    {
        public bool SaveSuccess { get; set; }
        public string CandidateName { get; set; }
        public string RoleType { get; set; }
        public string MonitorType { get; set; }
        public string pmUUID { get; set; }
        public string certUUID { get; set; }
        public string RegType { get; set; }
        public C_APPLICANT C_APPLICANT { get; set; }
        public C_COMP_PROCESS_MONITOR C_COMP_PROCESS_MONITOR { get; set; }
        public C_COMP_APPLICANT_INFO C_COMP_APPLICANT_INFO { get; set; }
        public C_COMP_APPLICATION C_COMP_APPLICATION { get; set; }

        public C_IND_PROCESS_MONITOR C_IND_PROCESS_MONITOR { get; set; }
        public C_IND_APPLICATION C_IND_APPLICATION { get; set; }
        public C_IND_CERTIFICATE C_IND_CERTIFICATE { get; set; }
        public C_S_CATEGORY_CODE C_S_CATEGORY_CODE { get; set; }
        public C_INTERVIEW_SCHEDULE C_INTERVIEW_SCHEDULE { get; set; }
        public IEnumerable<SelectListItem> SecretaryList
        {
            get { return SystemListUtil.GetSecretaryList(); }
        }
        public IEnumerable<SelectListItem> AssistantList
        {
            get { return SystemListUtil.GetAssistantList(); }
        }
        public IEnumerable<SelectListItem> VetOfficerList
        {
            get {
                if (C_COMP_PROCESS_MONITOR != null) { return SystemListUtil.GetEditVetOfficer(); }
                else { return SystemListUtil.GetVetOfficer(); }
            }
        }
        public IEnumerable<SelectListItem> VetIndOfficerList
        {
            get
            {
                if (C_IND_PROCESS_MONITOR != null) { return SystemListUtil.GetEditVetOfficer(); }
                else { return SystemListUtil.GetVetOfficer(); }
            }
        }

        public IEnumerable<SelectListItem> interviewDateList
        {
            get; set;
        }
        public DateTime? SuppleDocumentDate
        {
            get
            {   
                if(C_IND_PROCESS_MONITOR != null)
                {
                    return C_IND_PROCESS_MONITOR.SUPPLE_DOCUMENT_DATE;
                }
                else
                {
                    return null;
                }
            }
            set { }
        }
        public IEnumerable<SelectListItem> CGATypeOfAppList
        {
            get {
                if(C_COMP_PROCESS_MONITOR != null)
                {
                    List<SelectListItem> A = new List<SelectListItem>();
                    foreach (var item in SystemListUtil.GetNewTypeOfAppList(RegistrationConstant.REGISTRATION_TYPE_CGA))
                    {
                        if (item.Text!= "BA1" && item.Text!="BA1A"&&item.Text!="BA1B"&& item.Text !="BA24"&& item.Text !="NA")
                        {
                            A.Add(item);
                        }
                    }
                    return A;
                }
                else
                {
                    List<SelectListItem> A = new List<SelectListItem>();
                    foreach (var item in SystemListUtil.GetNewTypeOfAppList(RegistrationConstant.REGISTRATION_TYPE_CGA))
                    {
                        if (item.Text != "BA1" && item.Text != "BA1A" && item.Text != "BA1B" && item.Text != "BA24" && item.Text != "NA")
                        {
                            if (item.Text == "BA2C")
                            {
                                item.Selected = true;
                            }
                            A.Add(item);
                        }
                    }
                    return A;
                }

            }
        }
        public string CGCAppList { get; set; }
        public IEnumerable<SelectListItem> MWCTypeOfAppList
        {
            get
            {
                if (C_COMP_PROCESS_MONITOR != null)
                {
                    List<SelectListItem> A = new List<SelectListItem>();
                    foreach (var item in SystemListUtil.GetNewTypeOfAppList(RegistrationConstant.REGISTRATION_TYPE_MWCA))
                    {
                        if (item.Text != "BA1" && item.Text != "BA1A" && item.Text != "BA1B" && item.Text != "BA24" && item.Text != "NA")
                        {
                            A.Add(item);
                        }
                    }
                    return A;
                }
                else
                {
                    List<SelectListItem> A = new List<SelectListItem>();
                    foreach (var item in SystemListUtil.GetNewTypeOfAppList(RegistrationConstant.REGISTRATION_TYPE_MWCA))
                    {
                        if (item.Text != "BA1" && item.Text != "BA1A" && item.Text != "BA1B" && item.Text != "BA24" && item.Text != "NA")
                        {
                            A.Add(item);
                        }
                    }
                    return A;
                }

            }
        }
        public string MWCAppList{ get; set; }
        //Ind
        public IEnumerable<SelectListItem> IPTypeOfAppList
        {
            get
            {
                if (C_IND_PROCESS_MONITOR != null)
                {
                    List<SelectListItem> A = new List<SelectListItem>();
                    foreach (var item in SystemListUtil.GetNewTypeOfAppList(RegistrationConstant.REGISTRATION_TYPE_IP))
                    {
                        if (item.Text != "BA2" && item.Text != "BA2A" && item.Text != "BA2B" && item.Text != "BA2C" && item.Text != "BA2D" && item.Text != "BA25" && item.Text != "BA25A" && item.Text != "BA25B" && item.Text != "BA25C" && item.Text != "BA25D" && item.Text != "BA25E" && item.Text != "BA25F" && item.Text != "BA25G")
                        {
                            A.Add(item);
                        }
                    }
                    return A;
                }
                else
                {
                    List<SelectListItem> A = new List<SelectListItem>();
                    foreach (var item in SystemListUtil.GetNewTypeOfAppList(RegistrationConstant.REGISTRATION_TYPE_IP))
                    {
                        if (item.Text != "BA2" && item.Text != "BA2A" && item.Text != "BA2B" && item.Text != "BA2C" && item.Text != "BA2D" && item.Text != "BA25" && item.Text != "BA25A" && item.Text != "BA25B" && item.Text != "BA25C" && item.Text != "BA25D" && item.Text != "BA25E" && item.Text != "BA25F" && item.Text != "BA25G")
                        {
                            A.Add(item);
                        }
                    }
                    return A;
                }

            }
        }
        public string IPAppList{ get; set; }
        public IEnumerable<SelectListItem> MWIATypeOfAppList
        {
            get
            {
                if (C_IND_PROCESS_MONITOR != null)
                {
                    List<SelectListItem> A = new List<SelectListItem>();
                    foreach (var item in SystemListUtil.GetNewTypeOfAppList(RegistrationConstant.REGISTRATION_TYPE_MWIA))
                    {
                        if (item.Text != "BA1" && item.Text != "BA1A" && item.Text != "BA1B" && item.Text != "BA24" && item.Text != "NA" && item.Text != "BA2" && item.Text != "BA2A" && item.Text != "BA2B" && item.Text != "BA2C" && item.Text != "BA2D" && item.Text != "BA25" && item.Text != "BA25A" && item.Text != "BA25B" && item.Text != "BA25C" && item.Text != "BA25D" && item.Text != "BA25E" && item.Text != "BA25F" && item.Text != "BA25G")
                        {
                            A.Add(item);
                        }
                    }
                    return A;
                }
                else
                {
                    List<SelectListItem> A = new List<SelectListItem>();
                    foreach (var item in SystemListUtil.GetNewTypeOfAppList(RegistrationConstant.REGISTRATION_TYPE_MWIA))
                    {
                        if (item.Text != "BA1" && item.Text != "BA1A" && item.Text != "BA1B" && item.Text != "BA24" && item.Text != "NA" && item.Text != "BA2" && item.Text != "BA2A" && item.Text != "BA2B" && item.Text != "BA2C" && item.Text != "BA2D" && item.Text != "BA25" && item.Text != "BA25A" && item.Text != "BA25B" && item.Text != "BA25C" && item.Text != "BA25D" && item.Text != "BA25E" && item.Text != "BA25F" && item.Text != "BA25G")
                        {
                            A.Add(item);
                        }
                    }
                    return A;
                }
            }
        }
        public string MWIAAppList { get; set; }
        public string HKID { get; set; }
        public string PASSPORTNO { get; set; }
        public string InterResultID { get; set; }
        public IEnumerable<SelectListItem> InterResult
        {
            get
            {
                if (C_COMP_PROCESS_MONITOR != null)
                {
                    return SystemListUtil.GetInterResult(RegistrationConstant.ALL_TYPE);
                }
                else
                {
                    List<SelectListItem> A = new List<SelectListItem>();
                    foreach (var item in SystemListUtil.GetInterResult(RegistrationConstant.ALL_TYPE))
                    {
                            if (item.Text == null)
                            {
                                item.Selected = true;
                            }
                            A.Add(item);
                        }
                    return A;
                }
            }
        }
        public IEnumerable<SelectListItem> TypeList
        {
            get
            {
                if (C_COMP_PROCESS_MONITOR != null)
                    return SystemListUtil.GetTypeOfPM(C_COMP_PROCESS_MONITOR.PROCESS_MONITOR_TYPE);
                else
                    return SystemListUtil.GetTypeOfPM(" ");
            }
        }
        public IEnumerable<SelectListItem> CGANature
        {
            get {
                if (C_COMP_PROCESS_MONITOR != null)
                    return SystemListUtil.GetNature(C_COMP_PROCESS_MONITOR.NATURE);
                else
                    return SystemListUtil.GetNature("N");
            }
        }
        public IEnumerable<SelectListItem> CGATwoMonthCase
        {
            get {
                if (C_COMP_PROCESS_MONITOR != null)
                    return SystemListUtil.GetTwoMonthCase(C_COMP_PROCESS_MONITOR.TWO_MONTH_CASE);
                else
                    return SystemListUtil.GetTwoMonthCase("Y");
            }
        }
        public IEnumerable<SelectListItem> CGAFastTrack
        {
            get
            {
                if (C_COMP_PROCESS_MONITOR != null)
                    return SystemListUtil.GetFastTrack(C_COMP_PROCESS_MONITOR.FAST_TRCK);
                else
                    return SystemListUtil.GetFastTrack("Y");
            }
        }
        public IEnumerable<SelectListItem> AppApplyByType
        {
            get {
                if (C_COMP_PROCESS_MONITOR != null)
                    return SystemListUtil.GetAppApplyType(C_COMP_PROCESS_MONITOR.APPLY_STATUS);
                else
                    return SystemListUtil.GetAppApplyType("A");
            }
        }
        public IEnumerable<SelectListItem> IndAppApplyByType
        {
            get
            {
                if (C_IND_PROCESS_MONITOR != null)
                {
                    if (C_IND_PROCESS_MONITOR.APPLY_STATUS != null && !C_IND_PROCESS_MONITOR.APPLY_STATUS.Equals(""))
                        return SystemListUtil.GetIndAppApplyType(C_IND_PROCESS_MONITOR.APPLY_STATUS);
                    else
                        return SystemListUtil.GetIndAppApplyType("A");
                }
                  else
                    return SystemListUtil.GetIndAppApplyType("A");
            }
        }
        public IEnumerable<SelectListItem> MWCAppApplyByType
        {
            get
            {
                if (C_COMP_PROCESS_MONITOR != null)
                    return SystemListUtil.GetCMWAppApplyType(C_COMP_PROCESS_MONITOR.APPLY_STATUS);
                else
                    return SystemListUtil.GetCMWAppApplyType("A");
            }
        }
        public List<string> SelectedAppApplyType { get; set; }
        public bool AdditionClass
        {
            get { return C_COMP_PROCESS_MONITOR != null && "on".Equals(C_COMP_PROCESS_MONITOR.ADDITION_CLASS); }
            set { C_COMP_PROCESS_MONITOR.ADDITION_CLASS = value ? "on" : null; }
        }
        public bool AdditionType
        {
            get { return C_COMP_PROCESS_MONITOR != null && "on".Equals(C_COMP_PROCESS_MONITOR.ADDITION_TYPE); }
            set { C_COMP_PROCESS_MONITOR.ADDITION_TYPE = value ? "on" : null; }
        }
        public bool AdditionASorTD  
        {
            get { return C_COMP_PROCESS_MONITOR != null && "on".Equals(C_COMP_PROCESS_MONITOR.ADDITION_AUTH_SIGN); }
            set { C_COMP_PROCESS_MONITOR.ADDITION_AUTH_SIGN = value ? "on" : null; }
        }
        public string AUDIT {
            get
            {
                if (C_IND_PROCESS_MONITOR is null)
                {
                    return "Audit";
                }
                else
                {
                    return C_IND_PROCESS_MONITOR.AUDIT_TEXT;
                }
            }
        }
        public DateTime? interviewDate { get; set; }
        public string VettingOfficer{ get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (C_COMP_PROCESS_MONITOR != null)
            {

            yield return ValidationUtil.Validate_Mandatory(this, "C_COMP_PROCESS_MONITOR.VETTING_OFFICER");
            yield return ValidationUtil.Validate_Mandatory(this, "C_COMP_PROCESS_MONITOR.RECEIVED_DATE");
            }
           else if (C_IND_PROCESS_MONITOR != null)
            {
                yield return ValidationUtil.Validate_Mandatory(this, "C_IND_PROCESS_MONITOR.VETTING_OFFICER");
                yield return ValidationUtil.Validate_Mandatory(this, "C_IND_PROCESS_MONITOR.RECEIVED_DATE");
            }
        }
    }
}

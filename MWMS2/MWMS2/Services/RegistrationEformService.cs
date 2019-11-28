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
using MWMS2.Areas.Registration.Models;
using MWMS2.Constant;
using System.Data.Entity;

namespace MWMS2.Services
{
    public class RegistrationEformService : RegistrationCommonService
    {
        public EformModel initialiseEformModel(string fileRefNo, string selectedEformApplication)
        {
            EformModel model = new EformModel();
            model.FileRefSuffix = fileRefNo;
            model.FileRefNo = fileRefNo;
            model.EditMode = "edit";

            model.Fields = new List<string>();
            model.Displays = new List<string>();
            model.DisplayFields = new List<string>();
            model.statusMap = new Dictionary<string, string>();

            if (selectedEformApplication.Equals("C_EFSS_COMPANY"))
            {
                model.Fields.AddRange(model.EFSS_COMPANY_FIELD);
                model.Displays.AddRange(model.EFSS_COMPANY_DISPLAY);
                model.DisplayFields.AddRange(model.EFSS_COMPANY_DISPLAYFIELD);
                //EformFunc = "javascript:selectEformEFSS_COMPANY";
                model.SelectedEformApplication = "C_EFSS_COMPANY";
                model.EformSelectFunc = "javascript:selectEformEFSS_COMPANY";
                model.EformSaveFunc = "javscript:saveEformEFSS_COMPANY";
            }
            else if (selectedEformApplication.Equals("C_EFSS_PROFESSIONAL"))
            {
                model.Fields.AddRange(model.EFSS_PROFESSIONAL_FIELD);
                model.Displays.AddRange(model.EFSS_PROFESSIONAL_DISPLAY);
                model.DisplayFields.AddRange(model.EFSS_PROFESSIONAL_DISPLAYFIELD);
                //EformFunc = "javascript:selectEformEFSS_PROFESSIONAL";
                model.SelectedEformApplication = "C_EFSS_PROFESSIONAL";
                model.EformSelectFunc = "javascript:selectEformEFSS_PROFESSIONAL";
                model.EformSaveFunc = "javscript:saveEformEFSS_PROFESSIONAL";
            }
            else if (selectedEformApplication.Equals("C_EFSS_MWC"))
            {
                model.Fields.AddRange(model.EFSS_MWC_FIELD);
                model.Displays.AddRange(model.EFSS_MWC_DISPLAY);
                model.DisplayFields.AddRange(model.EFSS_MWC_DISPLAYFIELD);
                //EformFunc = "javascript:selectEformEFSS_MWC";
                model.SelectedEformApplication = "C_EFSS_MWC";
                model.EformSelectFunc = "javascript:selectEformEFSS_MWC";
                model.EformSaveFunc = "javscript:saveEformEFSS_MWC";
            }
            else if (selectedEformApplication.Equals("C_EFSS_MWCW"))
            {
                model.Fields.AddRange(model.EFSS_MWCW_FIELD);
                model.Displays.AddRange(model.EFSS_MWCW_DISPLAY);
                model.DisplayFields.AddRange(model.EFSS_MWCW_DISPLAYFIELD);
                //EformFunc = "javascript:selectEformEFSS_MWCW";
                model.SelectedEformApplication = "C_EFSS_MWCW";
                model.EformSelectFunc = "javascript:selectEformEFSS_MWCW";
                model.EformSaveFunc = "javscript:saveEformEFSS_MWCW";
            }
            return model;
        }

        public EformModel populateEformSelection(EformModel model)
        {
            string registrationType = model.RegistrationType;
            string fileRefSuffix = model.FileRefSuffix;
            string tableName = model.SelectedEformApplication;
            string editMode = model.EditMode;

            RegistrationCompAppService registrationCompAppService = new RegistrationCompAppService();

            try
            {
                var result = registrationCompAppService.getEformData(registrationType, fileRefSuffix, tableName, editMode);

                if (result != null && result.Count() > 0)
                {
                    model.SelectedEformList = result;
                }
                else
                {
                    model.SelectedEformList = null;
                }
            }
            catch (Exception ex)
            {
                model.ErrMSg = "Error in populateEformSelection(): " + ex.Message;
            }

            return DataPreRenderToView(model);
        }

        public EformModel DataPreRenderToView(EformModel model)
        {
            if (model.SelectedEformList != null && model.SelectedEformList.Count() > 0)
            {
                model.EformitemList = new List<Dictionary<string, string>>();

                for (int i = 0; i < model.SelectedEformList.Count(); i++)
                {
                    var history = model.SelectedEformList[i];
                    Dictionary<string, string> list = new Dictionary<string, string>();

                    for (int j = 0; j < model.Fields.Count(); j++)
                    {
                        if (model.Fields[j].Equals("SPECSIGN") && history[j] != DBNull.Value)
                        {
                            string SPECSIGN = (string)history[j];
                            SPECSIGN = SPECSIGN.Replace("\\", "\\\\");
                            list.Add(model.Fields[j], SPECSIGN);
                        }
                        else if (model.Fields[j].Equals("LASTUPDDATE"))
                        {
                            list.Add(model.Fields[j], DateUtil.ConvertToDateTimeDisplay((DateTime)history[j]));
                        }
                        else if(model.Fields[j].Equals("STATUS"))
                        {
                            //model.statusMap.Add(history[0].ToString(), history[j].ToString());
                            list.Add(model.Fields[j], history[j].ToString());
                        }
                        else {
                            list.Add(model.Fields[j], history[j].ToString());
                        }
                        //model.EformitemList.Add(list);
                    }
                    model.EformitemList.Add(list);
                }
            }
            return model;
        }

        public ServiceResult updateEformStatus_COMPANY(string UUID, string STATUS)
        {
            using(EntitiesRegistration db = new EntitiesRegistration())
            {
                C_EFSS_COMPANY target_EFSS_COMPANY = db.C_EFSS_COMPANY.Find(UUID);
                C_EFSS_BA24 target_EFSS_BA24 = db.C_EFSS_BA24.Find(UUID);
                if (target_EFSS_COMPANY != null && target_EFSS_BA24 == null)
                {
                    target_EFSS_COMPANY.STATUS = STATUS;
                    db.SaveChanges();
                    return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS }; ;
                }
                else if (target_EFSS_COMPANY == null && target_EFSS_BA24 != null)
                {
                    target_EFSS_BA24.STATUS = STATUS;
                    db.SaveChanges();
                    return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS }; ;
                }
                return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE };
            }
        }

        public ServiceResult updateEformStatus_PROFESSIONAL(string UUID, string STATUS)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                C_EFSS_PROFESSIONAL target_EFSS_PROFESSIONAL = db.C_EFSS_PROFESSIONAL.Find(UUID);
                C_EFSS_BA24 target_EFSS_BA24 = db.C_EFSS_BA24.Find(UUID);
                if (target_EFSS_PROFESSIONAL != null && target_EFSS_BA24 == null)
                {
                    target_EFSS_PROFESSIONAL.STATUS = STATUS;
                    db.SaveChanges();
                    return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS }; ;
                }
                else if (target_EFSS_PROFESSIONAL == null && target_EFSS_BA24 != null)
                {
                    target_EFSS_BA24.STATUS = STATUS;
                    db.SaveChanges();
                    return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS }; ;
                }
                return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE };
            }
        }
        public ServiceResult updateEformStatus_MWC(string UUID, string STATUS)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                C_EFSS_MWC target_EFSS_MWC = db.C_EFSS_MWC.Find(UUID);
                C_EFSS_BA24 target_EFSS_BA24 = db.C_EFSS_BA24.Find(UUID);
                if (target_EFSS_MWC != null && target_EFSS_BA24 == null)
                {
                    target_EFSS_MWC.STATUS = STATUS;
                    db.SaveChanges();
                    return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS }; ;
                }
                else if (target_EFSS_MWC == null && target_EFSS_BA24 != null)
                {
                    target_EFSS_BA24.STATUS = STATUS;
                    db.SaveChanges();
                    return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS }; ;
                }
                return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE };
            }
        }
        public ServiceResult updateEformStatus_MWCW(string UUID, string STATUS)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                C_EFSS_MWCW target_EFSS_MWCW = db.C_EFSS_MWCW.Find(UUID);
                C_EFSS_BA24 target_EFSS_BA24 = db.C_EFSS_BA24.Find(UUID);
                if (target_EFSS_MWCW != null && target_EFSS_BA24 == null)
                {
                    target_EFSS_MWCW.STATUS = STATUS;
                    db.SaveChanges();
                    return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS }; ;
                }
                else if (target_EFSS_MWCW == null && target_EFSS_BA24 != null)
                {
                    target_EFSS_BA24.STATUS = STATUS;
                    db.SaveChanges();
                    return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS }; ;
                }
                return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE };
            }
        }

        public List<List<string>> getDatafromEFSSPROFESSIONAL(string Uuid, string RefNo, string FileType) 
        {
            var EditMode = "edit";
            RegistrationCompAppService rs = new RegistrationCompAppService();
            var EfssProfessionalData = rs.getEFSS_PROFESSIONALData(Uuid, RefNo, FileType, EditMode);

            EformModel model = new EformModel();
            List<string> Fields = new List<string>();
            Fields.AddRange(model.EFSS_PROFESSIONAL_FIELD);

            Dictionary<string, string> EformItem = new Dictionary<string, string>();
            for (int j = 0; j < Fields.Count(); j++)
            {
                if (Fields[j].Equals("SPECSIGN"))
                {
                    //System.out.println(StringUtil.getDisplay(history[j]));
                    //String SPECSIGN =StringUtil.getDisplay(EfssProfessionalData.get(0)[j]);
                    //SPECSIGN = SPECSIGN.replace("\\", "\\\\");
                    //EformItem.put(Fields[j],SPECSIGN);
                }
                else
                {
                    string data = "";
                    if (EfssProfessionalData[0][j] != DBNull.Value)
                    {
                        if(Fields[j].Equals("CREATEDATE") || 
                            Fields[j].Equals("LASTUPDDATE") || 
                            Fields[j].Equals("APPLICATIONDATE") )
                        {
                            EformItem.Add(Fields[j], DateUtil.ConvertToDateTimeDisplay((DateTime)EfssProfessionalData[0][j]));
                        }
                        else
                        {
                            EformItem.Add(Fields[j], (string)EfssProfessionalData[0][j]);
                        }
                    }
                    else
                    {
                        EformItem.Add(Fields[j], "");
                    }
                }
            }

            var ApplicantData = rs.getEFSS_ApplicantData(Uuid, FileType).FirstOrDefault(); // form id, form type

            // web element id : sql field data
            List<string> eformFields = new List<string>();
            List<string> eformItems = new List<string>();
            if (ApplicantData != null)
            {
                // EFSS_APPLICANT
                if (!string.IsNullOrEmpty(ApplicantData.HKID))
                {
                    eformFields.Add("C_APPLICANT_HKID");
                    eformItems.Add(EncryptDecryptUtil.getDecryptHKID(ApplicantData.HKID));
                }
                if (!string.IsNullOrEmpty(ApplicantData.PASSPORTNO))
                {
                    eformFields.Add("C_APPLICANT_PASSPORT_NO");
                    eformItems.Add(EncryptDecryptUtil.getDecryptHKID(ApplicantData.PASSPORTNO));
                }
                if (!string.IsNullOrEmpty(ApplicantData.SURNAME))
                {
                    eformFields.Add("C_APPLICANT_SURNAME");
                    eformItems.Add(ApplicantData.SURNAME.ToUpper());
                }
                if (!string.IsNullOrEmpty(ApplicantData.GIVENNAME))
                {
                    eformFields.Add("C_APPLICANT_GIVEN_NAME_ON_ID");
                    eformItems.Add(ApplicantData.GIVENNAME);
                }
                if (!string.IsNullOrEmpty(ApplicantData.CHNNAME))
                {
                    eformFields.Add("C_APPLICANT_CHINESE_NAME");
                    eformItems.Add(ApplicantData.CHNNAME);
                }
                if (!string.IsNullOrEmpty(ApplicantData.SEX))
                {
                    eformFields.Add("C_APPLICANT_GENDER");
                    eformItems.Add(ApplicantData.SEX);
                }
                if (!string.IsNullOrEmpty(ApplicantData.COMLANG))
                {
                    eformFields.Add("C_IND_APPLICATION_CORRESPONDENCE_LANG_ID");
                    eformItems.Add(ApplicantData.COMLANG);
                }
            }
            // EFSS_PROFESSIONAL
            string newAddress_eng = EformItem["ADDRESS_LINE1"] + EformItem["ADDRESS_LINE2"] + EformItem["ADDRESS_LINE3"] + EformItem["ADDRESS_LINE4"] + EformItem["ADDRESS_LINE5"];
            if(newAddress_eng.Length > 0)
            {
                eformFields.Add("HOME_ADDRESS_ENG_ADDRESS_LINE1");
                eformItems.Add(EformItem["ADDRESS_LINE1"]);
                eformFields.Add("HOME_ADDRESS_ENG_ADDRESS_LINE2");
                eformItems.Add(EformItem["ADDRESS_LINE2"]);
                eformFields.Add("HOME_ADDRESS_ENG_ADDRESS_LINE3");
                eformItems.Add(EformItem["ADDRESS_LINE3"]);
                eformFields.Add("HOME_ADDRESS_ENG_ADDRESS_LINE4");
                eformItems.Add(EformItem["ADDRESS_LINE4"]);
                eformFields.Add("HOME_ADDRESS_ENG_ADDRESS_LINE5");
                eformItems.Add(EformItem["ADDRESS_LINE5"]);
            }
            string newAddress_chin = EformItem["C_ADDRESS_LINE1"] + EformItem["C_ADDRESS_LINE2"] + EformItem["C_ADDRESS_LINE3"] + EformItem["C_ADDRESS_LINE4"] + EformItem["C_ADDRESS_LINE5"];
            if(newAddress_chin.Length > 0)
            {
                eformFields.Add("HOME_ADDRESS_CHI_ADDRESS_LINE1");
                eformItems.Add(EformItem["C_ADDRESS_LINE1"]);
                eformFields.Add("HOME_ADDRESS_CHI_ADDRESS_LINE2");
                eformItems.Add(EformItem["C_ADDRESS_LINE2"]);
                eformFields.Add("HOME_ADDRESS_CHI_ADDRESS_LINE3");
                eformItems.Add(EformItem["C_ADDRESS_LINE3"]);
                eformFields.Add("HOME_ADDRESS_CHI_ADDRESS_LINE4");
                eformItems.Add(EformItem["C_ADDRESS_LINE4"]);
                eformFields.Add("HOME_ADDRESS_CHI_ADDRESS_LINE5");
                eformItems.Add(EformItem["C_ADDRESS_LINE5"]);
            }
            string newOfficeAddress_eng = EformItem["OFFICE_ADDRESS_LINE1"] + EformItem["OFFICE_ADDRESS_LINE2"] + EformItem["OFFICE_ADDRESS_LINE3"] + EformItem["OFFICE_ADDRESS_LINE4"] + EformItem["OFFICE_ADDRESS_LINE5"];
            if (newOfficeAddress_eng.Length > 0)
            {
                eformFields.Add("OFFICE_ADDRESS_ENG_ADDRESS_LINE1");
                eformItems.Add(EformItem["OFFICE_ADDRESS_LINE1"]);
                eformFields.Add("OFFICE_ADDRESS_ENG_ADDRESS_LINE2");
                eformItems.Add(EformItem["OFFICE_ADDRESS_LINE2"]);
                eformFields.Add("OFFICE_ADDRESS_ENG_ADDRESS_LINE3");
                eformItems.Add(EformItem["OFFICE_ADDRESS_LINE3"]);
                eformFields.Add("OFFICE_ADDRESS_ENG_ADDRESS_LINE4");
                eformItems.Add(EformItem["OFFICE_ADDRESS_LINE4"]);
                eformFields.Add("OFFICE_ADDRESS_ENG_ADDRESS_LINE5");
                eformItems.Add(EformItem["OFFICE_ADDRESS_LINE5"]);
            }
            string newOfficeAddress_chin = EformItem["C_OFFICE_ADDRESS_LINE1"] + EformItem["C_OFFICE_ADDRESS_LINE2"] + EformItem["C_OFFICE_ADDRESS_LINE3"] + EformItem["C_OFFICE_ADDRESS_LINE4"] + EformItem["C_OFFICE_ADDRESS_LINE5"];
            if (newOfficeAddress_chin.Length > 0)
            {
                eformFields.Add("OFFICE_ADDRESS_CHI_ADDRESS_LINE1");
                eformItems.Add(EformItem["C_OFFICE_ADDRESS_LINE1"]);
                eformFields.Add("OFFICE_ADDRESS_CHI_ADDRESS_LINE2");
                eformItems.Add(EformItem["C_OFFICE_ADDRESS_LINE2"]);
                eformFields.Add("OFFICE_ADDRESS_CHI_ADDRESS_LINE3");
                eformItems.Add(EformItem["C_OFFICE_ADDRESS_LINE3"]);
                eformFields.Add("OFFICE_ADDRESS_CHI_ADDRESS_LINE4");
                eformItems.Add(EformItem["C_OFFICE_ADDRESS_LINE4"]);
                eformFields.Add("OFFICE_ADDRESS_CHI_ADDRESS_LINE5");
                eformItems.Add(EformItem["C_OFFICE_ADDRESS_LINE5"]);
            }
            if(!string.IsNullOrEmpty(EformItem["TELNO1"]))
            {
                eformFields.Add("C_IND_APPLICATION_TELEPHONE_NO1");
                eformItems.Add(EformItem["TELNO1"]);
            }
            if(!string.IsNullOrEmpty(EformItem["TELNO2"]))
            {
                eformFields.Add("C_IND_APPLICATION_TELEPHONE_NO2");
                eformItems.Add(EformItem["TELNO2"]);
            }
            if(!string.IsNullOrEmpty(EformItem["TELNO3"]))
            {
                eformFields.Add("C_IND_APPLICATION_TELEPHONE_NO3");
                eformItems.Add(EformItem["TELNO3"]);
            }
            if(!string.IsNullOrEmpty(EformItem["FAXNO"]))
            {
                eformFields.Add("C_IND_APPLICATION_FAX_NO1");
                eformItems.Add(EformItem["FAXNO"]);
            }
            if (!string.IsNullOrEmpty(EformItem["FAXNO2"]))
            {
                eformFields.Add("C_IND_APPLICATION_FAX_NO2");
                eformItems.Add(EformItem["FAXNO2"]);
            }
            if (!string.IsNullOrEmpty(EformItem["EMAIL"]))
            {
                eformFields.Add("C_IND_APPLICATION_EMAIL");
                eformItems.Add(EformItem["EMAIL"].ToLower());
            }
            if (!string.IsNullOrEmpty(EformItem["EMERGNO1"]))
            {
                eformFields.Add("C_IND_APPLICATION_EMERGENCY_NO1");
                eformItems.Add(EformItem["EMERGNO1"]);
            }
            if (!string.IsNullOrEmpty(EformItem["EMERGNO2"]))
            {
                eformFields.Add("C_IND_APPLICATION_EMERGENCY_NO2");
                eformItems.Add(EformItem["EMERGNO2"]);
            }
            if (!string.IsNullOrEmpty(EformItem["ISINTERESTQP"]))
            {
                eformFields.Add("C_IND_APPLICATION_WILLINGNESS_QP");
                eformItems.Add(EformItem["ISINTERESTQP"]);
            }

            string formType = FileType;
            string referenceNo = RefNo;
            string RefSuffix = RefNo.Substring(referenceNo.IndexOf(' ') + 1);
            string RegistrationNo = "";

            if (!string.IsNullOrEmpty(EformItem["CERTNO"]))
            {
                RegistrationNo = EformItem["CERTNO"];
            }
            else
            {
                if (referenceNo.IndexOf(' ') >= 0)
                {
                    RegistrationNo = (string.IsNullOrEmpty(EformItem["CATCODE"]) ? "" : EformItem["CATCODE"]) + " " + RefSuffix;
                }
                else if (referenceNo.Count() > 0)
                {
                    RegistrationNo = referenceNo;
                }
            }

            if (EformItem["BSCODE1"].Equals("Y"))
            {
                eformFields.Add("BsItems_1__Checked");
                eformItems.Add(EformItem["BSCODE1"]);
            }
            if(EformItem["BSCODE2"].Equals("Y"))
            {
                eformFields.Add("BsItems_2__Checked");
                eformItems.Add(EformItem["BSCODE2"]);
            }
            if(EformItem["BSCODE3"].Equals("Y"))
            {
                eformFields.Add("BsItems_3__Checked");
                eformItems.Add(EformItem["BSCODE3"]);
            }
            if(EformItem["BSCODE4"].Equals("Y"))
            {
                eformFields.Add("BsItems_4__Checked");
                eformItems.Add(EformItem["BSCODE4"]);
            }

            //if (!string.IsNullOrEmpty(EformItem["FORMTYPE"]) && !EformItem["FORMTYPE"].Equals("BA24"))
            //{
            //    string FormUsedUUID = RegistrationCommonService.getUUIDbyFormTypeCode(EformItem["FORMTYPE"], "IP");
            //    if (!string.IsNullOrEmpty(FormUsedUUID))
            //    {
            //        eformFields.Add("C_IND_CERTIFICATE.APPLICATION_FORM_ID");
            //        eformItems.Add(FormUsedUUID);
            //    }
            //}

            RegistrationCommonService cs = new RegistrationCommonService();
            string CategoryUUID = "";
            if(!string.IsNullOrEmpty(EformItem["CATCODE"]))
            {
                CategoryUUID = getUUIDbyCODE(EformItem["CATCODE"]);
            }
 
            List<List<string>> eformDisplayMap = new List<List<string>>();
            eformDisplayMap.Add(eformFields);
            eformDisplayMap.Add(eformItems);


            return eformDisplayMap;
        }

        public List<List<string>> getDatafromEFSSCOMPANY(string Uuid, string RefNo, string FileType)
        {
            var EditMode = "edit";
            RegistrationCompAppService rs = new RegistrationCompAppService();
            var EfssCompanyData = rs.getEFSS_COMPANYData(Uuid, RefNo, FileType, EditMode);

            EformModel model = new EformModel();
            List<string> Fields = new List<string>();
            Fields.AddRange(model.EFSS_COMPANY_FIELD);

            Dictionary<string, string> EformItem = new Dictionary<string, string>();
            for (int j = 0; j < Fields.Count(); j++)
            {
                string data = "";
                if (EfssCompanyData[0][j] != DBNull.Value)
                {
                    if (Fields[j].Equals("CREATEDATE") ||
                        Fields[j].Equals("LASTUPDDATE") ||
                        Fields[j].Equals("APPLICATIONDATE"))
                    {
                        //EformItem.Add(Fields[j], DateUtil.ConvertToDateTimeDisplay((DateTime)EfssCompanyData[0][j]));
                        DateTime dt = (DateTime)EfssCompanyData[0][j];
                        EformItem.Add(Fields[j], dt.ToShortDateString());
                    }
                    else
                    {
                        EformItem.Add(Fields[j], (string)EfssCompanyData[0][j]);
                    }
                }
                else
                {
                    EformItem.Add(Fields[j], "");
                }
            }

            // web element id : sql field data
            List<string> eformFields = new List<string>();
            List<string> eformItems = new List<string>();

            // EFSS_COMPANY
            string CompanyTypeUUID = RegistrationCommonService.getUUIDbyCompanyType(RegistrationConstant.SYSTEM_TYPE_COMPANY_TYPE, EformItem["COMTYPE"]);
            if (!string.IsNullOrEmpty(CompanyTypeUUID))
            {
                eformFields.Add("C_COMP_APPLICATION_COMPANY_TYPE_ID");
                eformItems.Add(CompanyTypeUUID);
            }
            if (!string.IsNullOrEmpty(EformItem["BRNO"]))
            {
                eformFields.Add("C_COMP_APPLICATION_BR_NO");
                eformItems.Add(EformItem["BRNO"]);
            }
            if (!string.IsNullOrEmpty(EformItem["ENGCOMPNAME"]))
            {
                eformFields.Add("C_COMP_APPLICATION_ENGLISH_COMPANY_NAME");
                eformItems.Add(EformItem["ENGCOMPNAME"]);
            }
            if (!string.IsNullOrEmpty(EformItem["CHNCOMPNAME"]))
            {
                eformFields.Add("C_COMP_APPLICATION_CHINESE_COMPANY_NAME");
                eformItems.Add(EformItem["CHNCOMPNAME"]);
            }
            string newAddress_eng = EformItem["ADDRESS_LINE1"] + EformItem["ADDRESS_LINE2"] + EformItem["ADDRESS_LINE3"] + EformItem["ADDRESS_LINE4"] + EformItem["ADDRESS_LINE5"];
            if (newAddress_eng.Length > 0)
            {
                eformFields.Add("C_COMP_APPLICATION_C_ADDRESS2_ADDRESS_LINE1");
                eformItems.Add(EformItem["ADDRESS_LINE1"]);
                eformFields.Add("C_COMP_APPLICATION_C_ADDRESS2_ADDRESS_LINE2");
                eformItems.Add(EformItem["ADDRESS_LINE2"]);
                eformFields.Add("C_COMP_APPLICATION_C_ADDRESS2_ADDRESS_LINE3");
                eformItems.Add(EformItem["ADDRESS_LINE3"]);
                eformFields.Add("C_COMP_APPLICATION_C_ADDRESS2_ADDRESS_LINE4");
                eformItems.Add(EformItem["ADDRESS_LINE4"]);
                eformFields.Add("C_COMP_APPLICATION_C_ADDRESS2_ADDRESS_LINE5");
                eformItems.Add(EformItem["ADDRESS_LINE5"]);
            }
            string newAddress_chin = EformItem["C_ADDRESS_LINE1"] + EformItem["C_ADDRESS_LINE2"] + EformItem["C_ADDRESS_LINE3"] + EformItem["C_ADDRESS_LINE4"] + EformItem["C_ADDRESS_LINE5"];
            if (newAddress_chin.Length > 0)
            {
                eformFields.Add("C_COMP_APPLICATION_C_ADDRESS_ADDRESS_LINE1");
                eformItems.Add(EformItem["C_ADDRESS_LINE1"]);
                eformFields.Add("C_COMP_APPLICATION_C_ADDRESS_ADDRESS_LINE2");
                eformItems.Add(EformItem["C_ADDRESS_LINE2"]);
                eformFields.Add("C_COMP_APPLICATION_C_ADDRESS_ADDRESS_LINE3");
                eformItems.Add(EformItem["C_ADDRESS_LINE3"]);
                eformFields.Add("C_COMP_APPLICATION_C_ADDRESS_ADDRESS_LINE4");
                eformItems.Add(EformItem["C_ADDRESS_LINE4"]);
                eformFields.Add("C_COMP_APPLICATION_C_ADDRESS_ADDRESS_LINE5");
                eformItems.Add(EformItem["C_ADDRESS_LINE5"]);
            }
            if (!string.IsNullOrEmpty(EformItem["TELNO"]))
            {
                eformFields.Add("C_COMP_APPLICATION_TELEPHONE_NO1");
                eformItems.Add(EformItem["TELNO"]);
            }
            if (!string.IsNullOrEmpty(EformItem["TELNO2"]))
            {
                eformFields.Add("C_COMP_APPLICATION_TELEPHONE_NO2");
                eformItems.Add(EformItem["TELNO2"]);
            }
            if (!string.IsNullOrEmpty(EformItem["TELNO3"]))
            {
                eformFields.Add("C_COMP_APPLICATION_TELEPHONE_NO3");
                eformItems.Add(EformItem["TELNO3"]);
            }
            if (!string.IsNullOrEmpty(EformItem["FAXNO"]))
            {
                eformFields.Add("C_COMP_APPLICATION_FAX_NO1");
                eformItems.Add(EformItem["FAXNO"]);
            }
            if (!string.IsNullOrEmpty(EformItem["FAXNO2"]))
            {
                eformFields.Add("C_COMP_APPLICATION_FAX_NO2");
                eformItems.Add(EformItem["FAXNO2"]);
            }
            if (!string.IsNullOrEmpty(EformItem["EMAIL"]))
            {
                eformFields.Add("C_COMP_APPLICATION_EMAIL_ADDRESS");
                eformItems.Add(EformItem["EMAIL"].ToLower());
            }
            if (!string.IsNullOrEmpty(EformItem["APPNAME"]))
            {
                eformFields.Add("C_COMP_APPLICATION_APPLICANT_NAME");
                eformItems.Add(EformItem["APPNAME"]);
            }
            if (!string.IsNullOrEmpty(EformItem["APPLICATIONDATE"]))
            {
                eformFields.Add("C_COMP_APPLICATION_APPLICATION_DATE");
                eformItems.Add(EformItem["APPLICATIONDATE"]);
            }
            if (!string.IsNullOrEmpty(EformItem["FORMTYPE"]) && !EformItem["FORMTYPE"].Equals("BA24"))
            {
                string FormUsedUUID = RegistrationCommonService.getUUIDbyFormTypeCode(EformItem["FORMTYPE"], "CGC");
                if (!string.IsNullOrEmpty(FormUsedUUID))
                {
                    eformFields.Add("C_COMP_APPLICATION_APPLICATION_FORM_ID");
                    eformItems.Add(FormUsedUUID);
                }
            }

            List<List<string>> eformDisplayMap = new List<List<string>>();
            eformDisplayMap.Add(eformFields);
            eformDisplayMap.Add(eformItems);


            return eformDisplayMap;
        }
        public List<List<string>> getDatafromEFSSMWC(string Uuid, string RefNo, string FileType)
        {
            var EditMode = "edit";
            RegistrationCompAppService rs = new RegistrationCompAppService();
            var EfssMWCData = rs.getEFSS_MWCData(Uuid, RefNo, FileType, EditMode);

            EformModel model = new EformModel();
            List<string> Fields = new List<string>();
            Fields.AddRange(model.EFSS_MWC_FIELD);

            Dictionary<string, string> EformItem = new Dictionary<string, string>();
            for (int j = 0; j < Fields.Count(); j++)
            {
                string data = "";
                if (EfssMWCData[0][j] != DBNull.Value)
                {
                    if (Fields[j].Equals("CREATEDATE") ||
                        Fields[j].Equals("LASTUPDDATE") ||
                        Fields[j].Equals("APPLICATIONDATE"))
                    {
                        DateTime dt = (DateTime)EfssMWCData[0][j];
                        EformItem.Add(Fields[j], dt.ToShortDateString());
                    }
                    else
                    {
                        EformItem.Add(Fields[j], (string)EfssMWCData[0][j]);
                    }
                }
                else
                {
                    EformItem.Add(Fields[j], "");
                }
            }

            // web element id : sql field data
            List<string> eformFields = new List<string>();
            List<string> eformItems = new List<string>();

            // EFSS_MWC
            if (!string.IsNullOrEmpty(EformItem["BRNO"]))
            {
                eformFields.Add("C_COMP_APPLICATION_BR_NO");
                eformItems.Add(EformItem["BRNO"]);
            }
            if (!string.IsNullOrEmpty(EformItem["ENGCOMNAME"]))
            {
                eformFields.Add("C_COMP_APPLICATION_ENGLISH_COMPANY_NAME");
                eformItems.Add(EformItem["ENGCOMNAME"]);
            }
            if (!string.IsNullOrEmpty(EformItem["CHNCOMNAME"]))
            {
                eformFields.Add("C_COMP_APPLICATION_CHINESE_COMPANY_NAME");
                eformItems.Add(EformItem["CHNCOMNAME"]);
            }
            string CompanyTypeUUID = RegistrationCommonService.getUUIDbyCompanyType(RegistrationConstant.SYSTEM_TYPE_COMPANY_TYPE, EformItem["COMTYPE"]);
            if (!string.IsNullOrEmpty(CompanyTypeUUID))
            {
                eformFields.Add("C_COMP_APPLICATION_COMPANY_TYPE_ID");
                eformItems.Add(CompanyTypeUUID);
            }
            if (!string.IsNullOrEmpty(EformItem["APPNAME"]))
            {
                eformFields.Add("C_COMP_APPLICATION_APPLICANT_NAME");
                eformItems.Add(EformItem["APPNAME"]);
            }
            string newAddress_eng = EformItem["E_ADDRESS_LINE1"] + EformItem["E_ADDRESS_LINE2"] + EformItem["E_ADDRESS_LINE3"] + EformItem["E_ADDRESS_LINE4"] + EformItem["E_ADDRESS_LINE5"];
            if (newAddress_eng.Length > 0)
            {
                eformFields.Add("C_COMP_APPLICATION_C_ADDRESS2_ADDRESS_LINE1");
                eformItems.Add(EformItem["E_ADDRESS_LINE1"]);
                eformFields.Add("C_COMP_APPLICATION_C_ADDRESS2_ADDRESS_LINE2");
                eformItems.Add(EformItem["E_ADDRESS_LINE2"]);
                eformFields.Add("C_COMP_APPLICATION_C_ADDRESS2_ADDRESS_LINE3");
                eformItems.Add(EformItem["E_ADDRESS_LINE3"]);
                eformFields.Add("C_COMP_APPLICATION_C_ADDRESS2_ADDRESS_LINE4");
                eformItems.Add(EformItem["E_ADDRESS_LINE4"]);
                eformFields.Add("C_COMP_APPLICATION_C_ADDRESS2_ADDRESS_LINE5");
                eformItems.Add(EformItem["E_ADDRESS_LINE5"]);
            }
            string newAddress_chin = EformItem["C_ADDRESS_LINE1"] + EformItem["C_ADDRESS_LINE2"] + EformItem["C_ADDRESS_LINE3"] + EformItem["C_ADDRESS_LINE4"] + EformItem["C_ADDRESS_LINE5"];
            if (newAddress_chin.Length > 0)
            {
                eformFields.Add("C_COMP_APPLICATION_C_ADDRESS_ADDRESS_LINE1");
                eformItems.Add(EformItem["C_ADDRESS_LINE1"]);
                eformFields.Add("C_COMP_APPLICATION_C_ADDRESS_ADDRESS_LINE2");
                eformItems.Add(EformItem["C_ADDRESS_LINE2"]);
                eformFields.Add("C_COMP_APPLICATION_C_ADDRESS_ADDRESS_LINE3");
                eformItems.Add(EformItem["C_ADDRESS_LINE3"]);
                eformFields.Add("C_COMP_APPLICATION_C_ADDRESS_ADDRESS_LINE4");
                eformItems.Add(EformItem["C_ADDRESS_LINE4"]);
                eformFields.Add("C_COMP_APPLICATION_C_ADDRESS_ADDRESS_LINE5");
                eformItems.Add(EformItem["C_ADDRESS_LINE5"]);
            }
            if (!string.IsNullOrEmpty(EformItem["TELNO1"]))
            {
                eformFields.Add("C_COMP_APPLICATION_TELEPHONE_NO1");
                eformItems.Add(EformItem["TELNO1"]);
            }
            if (!string.IsNullOrEmpty(EformItem["TELNO2"]))
            {
                eformFields.Add("C_COMP_APPLICATION_TELEPHONE_NO2");
                eformItems.Add(EformItem["TELNO2"]);
            }
            if (!string.IsNullOrEmpty(EformItem["TELNO3"]))
            {
                eformFields.Add("C_COMP_APPLICATION_TELEPHONE_NO3");
                eformItems.Add(EformItem["TELNO3"]);
            }
            if (!string.IsNullOrEmpty(EformItem["FAXNO1"]))
            {
                eformFields.Add("C_COMP_APPLICATION_FAX_NO1");
                eformItems.Add(EformItem["FAXNO1"]);
            }
            if (!string.IsNullOrEmpty(EformItem["FAXNO2"]))
            {
                eformFields.Add("C_COMP_APPLICATION_FAX_NO2");
                eformItems.Add(EformItem["FAXNO2"]);
            }
            if (!string.IsNullOrEmpty(EformItem["EMAIL"]))
            {
                eformFields.Add("C_COMP_APPLICATION_EMAIL_ADDRESS");
                eformItems.Add(EformItem["EMAIL"].ToLower());
            }
            if (!string.IsNullOrEmpty(EformItem["FORMTYPE"]) && !EformItem["FORMTYPE"].Equals("BA24"))
            {
                string FormUsedUUID = RegistrationCommonService.getUUIDbyFormTypeCode(EformItem["FORMTYPE"], "CMW");
                if (!string.IsNullOrEmpty(FormUsedUUID))
                {
                    eformFields.Add("C_COMP_APPLICATION_APPLICATION_FORM_ID");
                    eformItems.Add(FormUsedUUID);
                }
            }
            if (!string.IsNullOrEmpty(EformItem["APPLICATIONDATE"]))
            {
                eformFields.Add("APPLICATIONDATE");
                eformItems.Add(EformItem["APPLICATIONDATE"]);
            }
            

            List<List<string>> eformDisplayMap = new List<List<string>>();
            eformDisplayMap.Add(eformFields);
            eformDisplayMap.Add(eformItems);


            return eformDisplayMap;
        }

        public List<List<string>> getDatafromEFSSMWCW(string Uuid, string RefNo, string FileType)
        {
            var EditMode = "edit";
            RegistrationCompAppService rs = new RegistrationCompAppService();
            var EfssMWCWData = rs.getEFSS_MWCWData(Uuid, RefNo, FileType, EditMode);

            EformModel model = new EformModel();
            List<string> Fields = new List<string>();
            Fields.AddRange(model.EFSS_MWCW_FIELD);

            Dictionary<string, string> EformItem = new Dictionary<string, string>();
            for (int j = 0; j < Fields.Count(); j++)
            {
                if (Fields[j].Equals("SPECSIGN"))
                {
                    //System.out.println(StringUtil.getDisplay(history[j]));
                    //String SPECSIGN =StringUtil.getDisplay(EfssProfessionalData.get(0)[j]);
                    //SPECSIGN = SPECSIGN.replace("\\", "\\\\");
                    //EformItem.put(Fields[j],SPECSIGN);
                }
                else
                {
                    string data = "";
                    if (EfssMWCWData[0][j] != DBNull.Value)
                    {
                        if (Fields[j].Equals("CREATEDATE") ||
                            Fields[j].Equals("LASTUPDDATE") ||
                            Fields[j].Equals("APPLICATIONDATE"))
                        {
                            EformItem.Add(Fields[j], ((DateTime)EfssMWCWData[0][j]).ToShortDateString());
                        }
                        else
                        {
                            EformItem.Add(Fields[j], (string)EfssMWCWData[0][j]);
                        }
                    }
                    else
                    {
                        EformItem.Add(Fields[j], "");
                    }
                }
            }

            var ApplicantData = rs.getEFSS_ApplicantData(Uuid, FileType).FirstOrDefault(); // form id, form type

            // web element id : sql field data
            List<string> eformFields = new List<string>();
            List<string> eformItems = new List<string>();
            if(ApplicantData != null)
            {
                // EFSS_APPLICANT
                if (!string.IsNullOrEmpty(ApplicantData.HKID))
                {
                    eformFields.Add("C_APPLICANT_HKID");
                    eformItems.Add(EncryptDecryptUtil.getDecryptHKID(ApplicantData.HKID));
                }
                if (!string.IsNullOrEmpty(ApplicantData.PASSPORTNO))
                {
                    eformFields.Add("C_APPLICANT_PASSPORT_NO");
                    eformItems.Add(EncryptDecryptUtil.getDecryptHKID(ApplicantData.PASSPORTNO));
                }
                if (!string.IsNullOrEmpty(ApplicantData.SURNAME))
                {
                    eformFields.Add("C_APPLICANT_SURNAME");
                    eformItems.Add(ApplicantData.SURNAME.ToUpper());
                }
                if (!string.IsNullOrEmpty(ApplicantData.GIVENNAME))
                {
                    eformFields.Add("C_APPLICANT_GIVEN_NAME_ON_ID");
                    eformItems.Add(ApplicantData.GIVENNAME);
                }
                if (!string.IsNullOrEmpty(ApplicantData.CHNNAME))
                {
                    eformFields.Add("C_APPLICANT_CHINESE_NAME");
                    eformItems.Add(ApplicantData.CHNNAME);
                }
                if (!string.IsNullOrEmpty(ApplicantData.SEX))
                {
                    eformFields.Add("C_APPLICANT_GENDER");
                    eformItems.Add(ApplicantData.SEX);
                }
                if (!string.IsNullOrEmpty(ApplicantData.COMLANG))
                {
                    eformFields.Add("C_IND_APPLICATION_CORRESPONDENCE_LANG_ID");
                    eformItems.Add(ApplicantData.COMLANG);
                }
            }
            // EFSS_MWCW
            if(EditMode.Equals("create"))
            {
                string AppStatusUUID = RegistrationCommonService.getUUIDbyTypeAndCode(RegistrationConstant.SYSTEM_TYPE_APPLICANT_STATUS, "2");
                if(!string.IsNullOrWhiteSpace(AppStatusUUID))
                {
                    eformFields.Add("C_IND_CERTIFICATE_APPLICATION_STATUS_ID");
                    eformItems.Add(AppStatusUUID);
                }

                if(!string.IsNullOrWhiteSpace(model.FileRefNo))
                {
                    eformFields.Add("C_IND_CERTIFICATE_CERTIFICATION_NO");
                    eformItems.Add(model.FileRefNo);
                }
            }
            if(!string.IsNullOrWhiteSpace(EformItem["FORMTYPE"]) && !EformItem["FORMTYPE"].Equals("BA24"))
            {
                String FormUsedUUID = RegistrationCommonService.getUUIDbyFormTypeCode(EformItem["FORMTYPE"], "IMW");
                eformFields.Add("C_IND_CERTIFICATE_APPLICATION_FORM_ID");
                eformItems.Add(FormUsedUUID);
            }
            if(!string.IsNullOrWhiteSpace(EformItem["APPLICATIONDATE"]))
            {
                eformFields.Add("C_IND_CERTIFICATE_APPLICATION_DATE");
                eformItems.Add(EformItem["APPLICATIONDATE"]);
            }

            
            C_ADDRESS engHomeAddress = new C_ADDRESS();
            C_ADDRESS engOfficeAddress = new C_ADDRESS();
            C_ADDRESS chinHomeAddress = new C_ADDRESS();
            C_ADDRESS chinOfficeAddress = new C_ADDRESS();

            using(EntitiesRegistration db = new EntitiesRegistration())
            {
                C_IND_APPLICATION indApplication = db.C_IND_APPLICATION.Where(x => x.FILE_REFERENCE_NO == RefNo).FirstOrDefault();
                chinHomeAddress = db.C_ADDRESS.Find(indApplication.CHINESE_HOME_ADDRESS_ID);
                chinOfficeAddress = db.C_ADDRESS.Find(indApplication.CHINESE_OFFICE_ADDRESS_ID);
            }
            bool copyChiCorrAddresstoOfficeAddress = checkChiOfficeAddressequalsCorrAddress(chinHomeAddress.ADDRESS_LINE1, chinHomeAddress.ADDRESS_LINE2, chinHomeAddress.ADDRESS_LINE3, chinHomeAddress.ADDRESS_LINE4, chinHomeAddress.ADDRESS_LINE5, chinOfficeAddress.ADDRESS_LINE1, chinOfficeAddress.ADDRESS_LINE2, chinOfficeAddress.ADDRESS_LINE3, chinOfficeAddress.ADDRESS_LINE4, chinOfficeAddress.ADDRESS_LINE5);

            if(FileType.Equals("BA24"))
            {
                bool engaddress = false;
                string tempAddress = EformItem["CORR_E_ADDRESS_LINE1"] + EformItem["CORR_E_ADDRESS_LINE2"] + EformItem["CORR_E_ADDRESS_LINE3"] + EformItem["CORR_E_ADDRESS_LINE4"] + EformItem["CORR_E_ADDRESS_LINE5"];
                if(tempAddress.Length > 0)
                {
                    engaddress = true;
                }
                if(engaddress)
                {
                    eformFields.Add("HOME_ADDRESS_CHI_ADDRESS_LINE1");
                    eformItems.Add(EformItem["CORR_E_ADDRESS_LINE1"]);
                    eformFields.Add("HOME_ADDRESS_CHI_ADDRESS_LINE2");
                    eformItems.Add(EformItem["CORR_E_ADDRESS_LINE2"]);
                    eformFields.Add("HOME_ADDRESS_CHI_ADDRESS_LINE3");
                    eformItems.Add(EformItem["CORR_E_ADDRESS_LINE3"]);
                    eformFields.Add("HOME_ADDRESS_CHI_ADDRESS_LINE4");
                    eformItems.Add(EformItem["CORR_E_ADDRESS_LINE4"]);
                    eformFields.Add("HOME_ADDRESS_CHI_ADDRESS_LINE5");
                    eformItems.Add(EformItem["CORR_E_ADDRESS_LINE5"]);

                    if(copyChiCorrAddresstoOfficeAddress && tempAddress.Length > 0)
                    {
                        eformFields.Add("OFFICE_ADDRESS_CHI_ADDRESS_LINE1");
                        eformItems.Add(EformItem["CORR_E_ADDRESS_LINE1"]);
                        eformFields.Add("OFFICE_ADDRESS_CHI_ADDRESS_LINE2");
                        eformItems.Add(EformItem["CORR_E_ADDRESS_LINE2"]);
                        eformFields.Add("OFFICE_ADDRESS_CHI_ADDRESS_LINE3");
                        eformItems.Add(EformItem["CORR_E_ADDRESS_LINE3"]);
                        eformFields.Add("OFFICE_ADDRESS_CHI_ADDRESS_LINE4");
                        eformItems.Add(EformItem["CORR_E_ADDRESS_LINE4"]);
                        eformFields.Add("OFFICE_ADDRESS_CHI_ADDRESS_LINE5");
                        eformItems.Add(EformItem["CORR_E_ADDRESS_LINE5"]);
                    }
                }
                else
                {
                    tempAddress = EformItem["CORR_C_ADDRESS_LINE1"] + EformItem["CORR_C_ADDRESS_LINE2"] + EformItem["CORR_C_ADDRESS_LINE3"] + EformItem["CORR_C_ADDRESS_LINE4"] + EformItem["CORR_C_ADDRESS_LINE5"];
                    if(tempAddress.Length > 0)
                    {
                        eformFields.Add("OFFICE_ADDRESS_CHI_ADDRESS_LINE1");
                        eformItems.Add(EformItem["CORR_C_ADDRESS_LINE1"]);
                        eformFields.Add("OFFICE_ADDRESS_CHI_ADDRESS_LINE2");
                        eformItems.Add(EformItem["CORR_C_ADDRESS_LINE2"]);
                        eformFields.Add("OFFICE_ADDRESS_CHI_ADDRESS_LINE3");
                        eformItems.Add(EformItem["CORR_C_ADDRESS_LINE3"]);
                        eformFields.Add("OFFICE_ADDRESS_CHI_ADDRESS_LINE4");
                        eformItems.Add(EformItem["CORR_C_ADDRESS_LINE4"]);
                        eformFields.Add("OFFICE_ADDRESS_CHI_ADDRESS_LINE5");
                        eformItems.Add(EformItem["CORR_C_ADDRESS_LINE5"]);
                    }
                    if (copyChiCorrAddresstoOfficeAddress && tempAddress.Length > 0)
                    {
                        eformFields.Add("OFFICE_ADDRESS_CHI_ADDRESS_LINE1");
                        eformItems.Add(EformItem["CORR_C_ADDRESS_LINE1"]);
                        eformFields.Add("OFFICE_ADDRESS_CHI_ADDRESS_LINE2");
                        eformItems.Add(EformItem["CORR_C_ADDRESS_LINE2"]);
                        eformFields.Add("OFFICE_ADDRESS_CHI_ADDRESS_LINE3");
                        eformItems.Add(EformItem["CORR_C_ADDRESS_LINE3"]);
                        eformFields.Add("OFFICE_ADDRESS_CHI_ADDRESS_LINE4");
                        eformItems.Add(EformItem["CORR_C_ADDRESS_LINE4"]);
                        eformFields.Add("OFFICE_ADDRESS_CHI_ADDRESS_LINE5");
                        eformItems.Add(EformItem["CORR_C_ADDRESS_LINE5"]);
                    }
                }
            }
            else
            {
                string newAddress = EformItem["CORR_E_ADDRESS_LINE1"] + EformItem["CORR_E_ADDRESS_LINE2"] + EformItem["CORR_E_ADDRESS_LINE3"] + EformItem["CORR_E_ADDRESS_LINE4"] + EformItem["CORR_E_ADDRESS_LINE5"];
                if(newAddress.Length > 0)
                {
                    eformFields.Add("HOME_ADDRESS_ENG_ADDRESS_LINE1");
                    eformItems.Add(EformItem["CORR_E_ADDRESS_LINE1"]);
                    eformFields.Add("HOME_ADDRESS_ENG_ADDRESS_LINE2");
                    eformItems.Add(EformItem["CORR_E_ADDRESS_LINE2"]);
                    eformFields.Add("HOME_ADDRESS_ENG_ADDRESS_LINE3");
                    eformItems.Add(EformItem["CORR_E_ADDRESS_LINE3"]);
                    eformFields.Add("HOME_ADDRESS_ENG_ADDRESS_LINE4");
                    eformItems.Add(EformItem["CORR_E_ADDRESS_LINE4"]);
                    eformFields.Add("HOME_ADDRESS_ENG_ADDRESS_LINE5");
                    eformItems.Add(EformItem["CORR_E_ADDRESS_LINE5"]);
                }
                newAddress = EformItem["CORR_C_ADDRESS_LINE1"] + EformItem["CORR_C_ADDRESS_LINE2"] + EformItem["CORR_C_ADDRESS_LINE3"] + EformItem["CORR_C_ADDRESS_LINE4"] + EformItem["CORR_C_ADDRESS_LINE5"];
                if(newAddress.Length > 0)
                {
                    eformFields.Add("HOME_ADDRESS_CHI_ADDRESS_LINE1");
                    eformItems.Add(EformItem["CORR_C_ADDRESS_LINE1"]);
                    eformFields.Add("HOME_ADDRESS_CHI_ADDRESS_LINE2");
                    eformItems.Add(EformItem["CORR_C_ADDRESS_LINE2"]);
                    eformFields.Add("HOME_ADDRESS_CHI_ADDRESS_LINE3");
                    eformItems.Add(EformItem["CORR_C_ADDRESS_LINE3"]);
                    eformFields.Add("HOME_ADDRESS_CHI_ADDRESS_LINE4");
                    eformItems.Add(EformItem["CORR_C_ADDRESS_LINE4"]);
                    eformFields.Add("HOME_ADDRESS_CHI_ADDRESS_LINE5");
                    eformItems.Add(EformItem["CORR_C_ADDRESS_LINE5"]);
                }
                bool Chiofficecopyied = false;
                if(copyChiCorrAddresstoOfficeAddress && newAddress.Length > 0)
                {
                    eformFields.Add("OFFICE_ADDRESS_CHI_ADDRESS_LINE1");
                    eformItems.Add(EformItem["CORR_C_ADDRESS_LINE1"]);
                    eformFields.Add("OFFICE_ADDRESS_CHI_ADDRESS_LINE2");
                    eformItems.Add(EformItem["CORR_C_ADDRESS_LINE2"]);
                    eformFields.Add("OFFICE_ADDRESS_CHI_ADDRESS_LINE3");
                    eformItems.Add(EformItem["CORR_C_ADDRESS_LINE3"]);
                    eformFields.Add("OFFICE_ADDRESS_CHI_ADDRESS_LINE4");
                    eformItems.Add(EformItem["CORR_C_ADDRESS_LINE4"]);
                    eformFields.Add("OFFICE_ADDRESS_CHI_ADDRESS_LINE5");
                    eformItems.Add(EformItem["CORR_C_ADDRESS_LINE5"]);

                    Chiofficecopyied = true;
                }
                newAddress = EformItem["COMP_E_ADDRESS_LINE1"] + EformItem["COMP_E_ADDRESS_LINE2"] + EformItem["COMP_E_ADDRESS_LINE3"] + EformItem["COMP_E_ADDRESS_LINE4"] + EformItem["COMP_E_ADDRESS_LINE5"];
                if(newAddress.Length > 0)
                {
                    eformFields.Add("OFFICE_ADDRESS_ENG_ADDRESS_LINE1");
                    eformItems.Add(EformItem["COMP_E_ADDRESS_LINE1"]);
                    eformFields.Add("OFFICE_ADDRESS_ENG_ADDRESS_LINE2");
                    eformItems.Add(EformItem["COMP_E_ADDRESS_LINE2"]);
                    eformFields.Add("OFFICE_ADDRESS_ENG_ADDRESS_LINE3");
                    eformItems.Add(EformItem["COMP_E_ADDRESS_LINE3"]);
                    eformFields.Add("OFFICE_ADDRESS_ENG_ADDRESS_LINE4");
                    eformItems.Add(EformItem["COMP_E_ADDRESS_LINE4"]);
                    eformFields.Add("OFFICE_ADDRESS_ENG_ADDRESS_LINE5");
                    eformItems.Add(EformItem["COMP_E_ADDRESS_LINE5"]);
                }
                newAddress = EformItem["COMP_C_ADDRESS_LINE1"] + EformItem["COMP_C_ADDRESS_LINE2"] + EformItem["COMP_C_ADDRESS_LINE3"] + EformItem["COMP_C_ADDRESS_LINE4"] + EformItem["COMP_C_ADDRESS_LINE5"];
                if(newAddress.Length > 0 && !Chiofficecopyied)
                {
                    eformFields.Add("OFFICE_ADDRESS_CHI_ADDRESS_LINE1");
                    eformItems.Add(EformItem["COMP_C_ADDRESS_LINE1"]);
                    eformFields.Add("OFFICE_ADDRESS_CHI_ADDRESS_LINE2");
                    eformItems.Add(EformItem["COMP_C_ADDRESS_LINE2"]);
                    eformFields.Add("OFFICE_ADDRESS_CHI_ADDRESS_LINE3");
                    eformItems.Add(EformItem["COMP_C_ADDRESS_LINE3"]);
                    eformFields.Add("OFFICE_ADDRESS_CHI_ADDRESS_LINE4");
                    eformItems.Add(EformItem["COMP_C_ADDRESS_LINE4"]);
                    eformFields.Add("OFFICE_ADDRESS_CHI_ADDRESS_LINE5");
                    eformItems.Add(EformItem["COMP_C_ADDRESS_LINE5"]);
                }
            }
            if (!string.IsNullOrEmpty(EformItem["TELNO1"]))
            {
                eformFields.Add("C_IND_APPLICATION_TELEPHONE_NO1");
                eformItems.Add(EformItem["TELNO1"]);
            }
            if (!string.IsNullOrEmpty(EformItem["TELNO2"]))
            {
                eformFields.Add("C_IND_APPLICATION_TELEPHONE_NO2");
                eformItems.Add(EformItem["TELNO2"]);
            }
            if (!string.IsNullOrEmpty(EformItem["TELNO3"]))
            {
                eformFields.Add("C_IND_APPLICATION_TELEPHONE_NO3");
                eformItems.Add(EformItem["TELNO3"]);
            }
            if (!string.IsNullOrEmpty(EformItem["FAXNO1"]))
            {
                eformFields.Add("C_IND_APPLICATION_FAX_NO1");
                eformItems.Add(EformItem["FAXNO1"]);
            }
            if (!string.IsNullOrEmpty(EformItem["FAXNO2"]))
            {
                eformFields.Add("C_IND_APPLICATION_FAX_NO2");
                eformItems.Add(EformItem["FAXNO2"]);
            }
            if (!string.IsNullOrEmpty(EformItem["EMAIL"]))
            {
                eformFields.Add("C_IND_APPLICATION_EMAIL");
                eformItems.Add(EformItem["EMAIL"].ToLower());
            }
            //if (!string.IsNullOrEmpty(EformItem["EMERGNO1"]))
            //{
            //    eformFields.Add("C_IND_APPLICATION_EMERGENCY_NO1");
            //    eformItems.Add(EformItem["EMERGNO1"]);
            //}
            //if (!string.IsNullOrEmpty(EformItem["EMERGNO2"]))
            //{
            //    eformFields.Add("C_IND_APPLICATION_EMERGENCY_NO2");
            //    eformItems.Add(EformItem["EMERGNO2"]);
            //}


            List<List<string>> eformDisplayMap = new List<List<string>>();
            eformDisplayMap.Add(eformFields);
            eformDisplayMap.Add(eformItems);


            return eformDisplayMap;
        }

        public bool checkChiOfficeAddressequalsCorrAddress(string h1, string h2, string h3, string h4, string h5, string o1, string o2, string o3, string o4, string o5)
        {
            bool result = false;
            h1 = !string.IsNullOrWhiteSpace(h1) ? h1 : "";
            h2 = !string.IsNullOrWhiteSpace(h2) ? h2 : "";
            h3 = !string.IsNullOrWhiteSpace(h3) ? h3 : "";
            h4 = !string.IsNullOrWhiteSpace(h4) ? h4 : "";
            h5 = !string.IsNullOrWhiteSpace(h5) ? h5 : "";
            o1 = !string.IsNullOrWhiteSpace(o1) ? o1 : "";
            o2 = !string.IsNullOrWhiteSpace(o1) ? o2 : "";
            o3 = !string.IsNullOrWhiteSpace(o1) ? o3 : "";
            o4 = !string.IsNullOrWhiteSpace(o1) ? o4 : "";
            o5 = !string.IsNullOrWhiteSpace(o1) ? o5 : "";

            if(h1.Equals(o1) && h2.Equals(o2) && h3.Equals(o3) && h4.Equals(o4) && h5.Equals(o5))
            {
                result = true;
            }

            return result;
        }
    }
}
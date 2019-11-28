using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Web;
using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Services.ProcessingDAO.DAO;
using NPOI.OpenXmlFormats.Wordprocessing;
using NPOI.XWPF.UserModel;

namespace MWMS2.Services
{
    public class ProcessingTdlBLService
    {
        private ProcessingTdlDAOService DAOService;
        protected ProcessingTdlDAOService DA
        {
            get { return DAOService ?? (DAOService = new ProcessingTdlDAOService()); }
        }

        private MwCrmInfoDaoImpl _crmService;
        protected MwCrmInfoDaoImpl crmService
        {
            get { return _crmService ?? (_crmService = new MwCrmInfoDaoImpl()); }
        }

        private P_S_TO_DETAILS_DAOService _toDetailsService;
        protected P_S_TO_DETAILS_DAOService toDetailsService
        {
            get { return _toDetailsService ?? (_toDetailsService = new P_S_TO_DETAILS_DAOService()); }
        }

        public string SearchMod_whereq(Fn03TSK_TdlSearchModel model)
        {
            string whereq = "";
            if (!string.IsNullOrWhiteSpace(model.RefNo))
            {
                whereq += @" and T1.REFERENCE_NO like :RefNo ";
                model.QueryParameters.Add("RefNo", "%" + model.RefNo + "%");
            }
            return whereq;
        }
        public Fn03TSK_TdlSearchModel SearchMOD(Fn03TSK_TdlSearchModel model)
        {
            model.QueryWhere = SearchMod_whereq(model);
            return DA.SearchMOD(model);
        }

        public void ExcelMod(Fn03TSK_TdlSearchModel model) {
            DA.SearchMOD(model);
            model.Export("Todo List - Modification Records");
        }

        public Fn03TSK_TdlSearchModel SearchDR(Fn03TSK_TdlSearchModel model)
        {
            return DA.SearchDR(model);
        }

        public Fn03TSK_TdlSearchModel GetDRDetail(string uuid)
        {
            Fn03TSK_TdlSearchModel model = DA.GetDRByUUID(uuid);
            model = DA.GetDRDetailLanguage(model);
            model.ListCheckbox = DA.GetDRCheckboxList();
            List<P_MW_DIRECT_RETURN_IRREGULARITIES> CheckboxSelected = DA.GetDRCheckboxSelected(model);
            foreach (var item in CheckboxSelected)
            {
                Fn03TSK_DRCheckboxList SelectedDR = model.ListCheckbox.Where(m => m.UUID == item.SV_IRREGULARITIES_ID && item.IS_CHECKED == "True").FirstOrDefault();
                if (SelectedDR != null)
                {
                    SelectedDR.IsChecked = true;
                }
            }
            model.DWRemark2 = DA.GetDWRemark2();
            P_S_SYSTEM_VALUE dwRemark3 = DA.GetDWRemark3();
            //model.DR.LANGUAGE = "CN"; //Temporarily restricted to Chinese
            model.DWRemark3 = new Fn03TSK_DRCheckboxList()
            {
                IsChecked = DA.IsCheckedDWRemark3(dwRemark3, model)
                ,
                Description_C = dwRemark3.DESCRIPTION
                ,
                Description_E = dwRemark3.DESCRIPTION_E
                ,
                IsActive = dwRemark3.IS_ACTIVE
                ,
                UUID = dwRemark3.UUID
                ,
                SystemTypeID = dwRemark3.SYSTEM_TYPE_ID
            };
            model.FileType = "P";

            //Get CRM Info
            model.V_CRM_INFO = crmService.findByCertNo(model.DR.CONTRACTOR_REG_NO);
            if(model.V_CRM_INFO == null)
            {
                model.V_CRM_INFO = new V_CRM_INFO();
            }

            model.P_S_TO_DETAILS = toDetailsService.GetP_S_TO_DETAILSByToPost(model.DR.HANDING_STAFF_2);
            if(model.P_S_TO_DETAILS == null)
            {
                model.P_S_TO_DETAILS = new P_S_TO_DETAILS();
            }

            return model;
        }

        public Fn03TSK_TdlPrintDRModel GetPrintDRModel(string uuid)
        {
           
            Fn03TSK_TdlSearchModel DRModel = GetDRDetail(uuid);
            //DRModel.DR.LANGUAGE = "CN"; //Temporarily restricted to Chinese
            Fn03TSK_TdlPrintDRModel model = new Fn03TSK_TdlPrintDRModel();
            V_CRM_INFO CrmModel = DA.GetVCRMInfo(DRModel.DR.CONTRACTOR_REG_NO);
            P_S_TO_DETAILS ToDetailsModel = DA.GetToDetails(DRModel.DR.HANDING_STAFF_2);
            List<P_MW_DIRECT_RETURN_IRREGULARITIES> CheckboxSelected = DA.GetDRCheckboxSelected(DRModel);
            model.Dsn = DRModel.DR.DSN;
            if (DRModel.DR.LANGUAGE == "EN")
            {
                model.ReceiveDate = DRModel.DR.RECEIVED_DATE.Value.ToString("dd MMMM, yyyy", CultureInfo.CreateSpecificCulture("en-GB"));
            }
            else
            {
                model.ReceiveDate = DRModel.DR.RECEIVED_DATE.Value.ToString("yyyy 年 MM 月 dd 日");
            }

            DRModel.P_S_TO_DETAILS = toDetailsService.GetP_S_TO_DETAILSByToPost(DRModel.DR.HANDING_STAFF_2);
            if (DRModel.P_S_TO_DETAILS == null)
            {
                DRModel.P_S_TO_DETAILS = new P_S_TO_DETAILS();
            }
            model.TOContactNo = DRModel.P_S_TO_DETAILS.TO_CONTACT;
            model.TOName = DRModel.DR.LANGUAGE == "EN" ? DRModel.P_S_TO_DETAILS.TO_NAME_ENG : DRModel.P_S_TO_DETAILS.TO_NAME_CHI;
            model.POName = DRModel.DR.LANGUAGE == "EN" ? DRModel.P_S_TO_DETAILS.PO_NAME_ENG: DRModel.P_S_TO_DETAILS.PO_NAME_CHI;

            if (CrmModel != null)
            {
                model.Fax = CrmModel.FAX_NO;
                model.GivenName = DRModel.DR.LANGUAGE == "EN" ? CrmModel.GIVEN_NAME : CrmModel.CHINESE_NAME;
                model.Surname = DRModel.DR.LANGUAGE == "EN" ? CrmModel.SURNAME : "";
                model.AddressLine1 = DRModel.DR.LANGUAGE == "EN" ? CrmModel.EN_ADDRESS_LINE1 : CrmModel.CN_ADDRESS_LINE1;
                model.AddressLine2 = DRModel.DR.LANGUAGE == "EN" ? CrmModel.EN_ADDRESS_LINE2 : CrmModel.CN_ADDRESS_LINE2;
                model.AddressLine3 = DRModel.DR.LANGUAGE == "EN" ? CrmModel.EN_ADDRESS_LINE3 : CrmModel.CN_ADDRESS_LINE3;
                model.AddressLine4 = DRModel.DR.LANGUAGE == "EN" ? CrmModel.EN_ADDRESS_LINE4 : CrmModel.CN_ADDRESS_LINE4;
                model.AddressLine5 = DRModel.DR.LANGUAGE == "EN" ? CrmModel.EN_ADDRESS_LINE5 : CrmModel.CN_ADDRESS_LINE5;
            }
            //else
            //{
            //    model.Fax = ""; model.GivenName = ""; model.Surname = ""; model.AddressLine1 = ""; model.AddressLine2 = ""; model.AddressLine3 = ""; model.AddressLine4 = "";
            //}
            if(ToDetailsModel != null)
            {
                model.SpoPost = DRModel.DR.LANGUAGE == "EN" ? ToDetailsModel.SPO_POST_ENG : ToDetailsModel.SPO_POST_CHI;
                model.SpoName = DRModel.DR.LANGUAGE == "EN" ? ToDetailsModel.SPO_NAME_ENG : ToDetailsModel.SPO_NAME_CHI;
            }
            //else
            //{
            //    model.SpoPost = ""; model.SpoName = "";
            //}
            model.Address = DRModel.DR.ADDRESS;
            model.DWRemark2 = DRModel.DR.LANGUAGE == "EN" ? DRModel.DWRemark2.DESCRIPTION_E : DRModel.DWRemark2.DESCRIPTION;
            if (DRModel.DWRemark3.IsChecked)
            {
                model.DWRemark3 = DRModel.DR.LANGUAGE == "EN" ? DRModel.DWRemark3.Description_E : DRModel.DWRemark3.Description_C;
            }
            else
            {
                model.DWRemark3 = DRModel.DR.LANGUAGE == "EN" ? "Not applicable" : "不適用";
            }
            
            model.FormType = DRModel.DR.FORM_TYPE;
            model.Language = DRModel.DR.LANGUAGE;
            foreach (var item in DRModel.ListCheckbox)
            {
                var checkItem = CheckboxSelected.Where(m => m.SV_IRREGULARITIES_ID == item.UUID).FirstOrDefault();
                switch (item.Code)
                {
                    case "a":
                    case "A":
                        model.CodeDecription0 = DRModel.DR.LANGUAGE == "EN" ? item.Description_E : item.Description_C;
                        model.CodeImage0 = checkItem != null ? "☑" : "□";
                        break;
                    case "b":
                    case "B":
                        model.CodeDecription1 = DRModel.DR.LANGUAGE == "EN" ? item.Description_E : item.Description_C;
                        model.CodeImage1 = checkItem != null ? "☑" : "□";
                        break;
                    case "c":
                    case "C":
                        model.CodeDecription2 = DRModel.DR.LANGUAGE == "EN" ? item.Description_E : item.Description_C;
                        model.CodeImage2 = checkItem != null ? "☑" : "□";
                        break;
                    case "d":
                    case "D":
                        model.CodeDecription3 = DRModel.DR.LANGUAGE == "EN" ? item.Description_E : item.Description_C;
                        model.CodeImage3 = checkItem != null ? "☑" : "□";
                        break;
                    case "e":
                    case "E":
                        model.CodeDecription4 = DRModel.DR.LANGUAGE == "EN" ? item.Description_E : item.Description_C;
                        model.CodeImage4 = checkItem != null ? "☑" : "□";
                        break;
                    case "f":
                    case "F":
                        model.CodeDecription5 = DRModel.DR.LANGUAGE == "EN" ? item.Description_E : item.Description_C;
                        model.CodeImage5 = checkItem != null ? "☑" : "□";
                        break;
                    case "g":
                    case "G":
                        model.CodeDecription6 = DRModel.DR.LANGUAGE == "EN" ? item.Description_E : item.Description_C;
                        model.CodeImage6 = checkItem != null ? "☑" : "□";
                        break;
                    case "h":
                    case "H":
                        model.CodeDecription7 = DRModel.DR.LANGUAGE == "EN" ? item.Description_E : item.Description_C;
                        model.CodeImage7 = checkItem != null ? "☑" : "□";
                        break;
                    case "i":
                    case "I":
                        model.CodeDecription8 = DRModel.DR.LANGUAGE == "EN" ? item.Description_E : item.Description_C;
                        model.CodeImage8 = checkItem != null ? "☑" : "□";
                        break;
                    case "j":
                    case "J":
                        model.CodeDecription9 = DRModel.DR.LANGUAGE == "EN" ? item.Description_E : item.Description_C;
                        model.CodeImage9 = checkItem != null ? "☑" : "□";
                        break;
                    case "k":
                    case "K":
                        model.CodeDecription10 = DRModel.DR.LANGUAGE == "EN" ? item.Description_E : item.Description_C;
                        model.CodeImage10 = checkItem != null ? "☑" : "□";
                        break;
                    case "l":
                    case "L":
                        model.CodeDecription11 = DRModel.DR.LANGUAGE == "EN" ? item.Description_E : item.Description_C;
                        model.CodeImage11 = checkItem != null ? "☑" : "□";
                        break;
                    case "m":
                    case "M":
                        model.CodeDecription12 = DRModel.DR.LANGUAGE == "EN" ? item.Description_E : item.Description_C;
                        model.CodeImage12 = checkItem != null ? "☑" : "□";
                        break;
                    case "n":
                    case "N":
                        model.CodeDecription13 = DRModel.DR.LANGUAGE == "EN" ? item.Description_E : item.Description_C;
                        model.CodeImage13 = checkItem != null ? "☑" : "□";
                        break;
                }
            }

            return model;
        }

        public XWPFDocument PrintDRWord(string uuid,string tempPath)
        {
            var model = GetPrintDRModel(uuid);
            if(model.Language == "EN")
                return BaseCommonService.GetWordDocument<Fn03TSK_TdlPrintDRModel>(tempPath, model, null, ProcessingConstant.ACKNOWLEDGEMENT_LETTER_ENG_FONTFAMILY);
            else
                return BaseCommonService.GetWordDocument<Fn03TSK_TdlPrintDRModel>(tempPath, model, null, ProcessingConstant.ACKNOWLEDGEMENT_LETTER_CHI_FONTFAMILY);
        }

        public byte[] PrintDRPDF(string uuid,string tempPath,string tmpPDFDir)
        {
            var model = GetPrintDRModel(uuid);
            string tmpPDFPath = tmpPDFDir + "tmpPDF" + Guid.NewGuid() + ".pdf";
            string tmpWordPath = tmpPDFDir + "tmpWord.docx";
            CT_SectPr sectPr = new CT_SectPr();
            sectPr.pgMar = new CT_PageMar();
            sectPr.pgMar.bottom = "0";
            sectPr.pgMar.top = "0";
            sectPr.pgMar.left = 1;
            sectPr.pgMar.right = 1;
            if (model.Language == "EN")
                return BaseCommonService.WordToPDF(tempPath, tmpPDFPath, tmpWordPath, model, null, ProcessingConstant.ACKNOWLEDGEMENT_LETTER_ENG_FONTFAMILY);
            else
                return BaseCommonService.WordToPDF(tempPath, tmpPDFPath, tmpWordPath, model, null, ProcessingConstant.ACKNOWLEDGEMENT_LETTER_CHI_FONTFAMILY);
        }

        public ServiceResult RemoveFromTDL(Fn03TSK_TdlSearchModel model)
        {
            return DA.RemoveFromTDL(model);
        }
        public ServiceResult UpdateDRDetail(Fn03TSK_TdlSearchModel model)
        {
            return DA.UpdateDRDetail(model);
        }

        public void SearchComplaints(Fn03TSK_TdlSearchModel model) { DA.SearchGeneralRecords("Complaint", model); }
        public void ExcelComplaints(Fn03TSK_TdlSearchModel model) { DA.ExcelGeneralRecords("Complaint", model); }

        public void SearchEnquirys(Fn03TSK_TdlSearchModel model) { DA.SearchGeneralRecords("Enquiry", model); }
        public void ExcelEnquirys(Fn03TSK_TdlSearchModel model) { DA.ExcelGeneralRecords("Enquiry", model); }

        public void SearchAudits(Fn03TSK_TdlSearchModel model) { DA.SearchAudits(model); }
        public void ExcelAudits(Fn03TSK_TdlSearchModel model) { DA.ExcelAudits(model); }


        public void SearchSubmissions(Fn03TSK_TdlSearchModel model) { DA.SearchSubmissions(model); }
        public void ExcelSubmissions(Fn03TSK_TdlSearchModel model) { DA.ExcelSubmissions(model); }

    }
}
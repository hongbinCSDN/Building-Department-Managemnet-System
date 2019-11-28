using MWMS2.Areas.Signboard.Models;
using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using NPOI.SS.Formula.Functions;
using MWMS2.Services.Signborad.SignboardDAO;
using MWMS2.Services.Signborad.WorkFlow;
using NPOI.XWPF.UserModel;
using NPOI.OpenXmlFormats.Wordprocessing;
using System.Text;
using System.Globalization;
using System.IO;

namespace MWMS2.Services.Signborad.SignboardServices
{
    public class SignboardLetterModuleService
    {
        private LetterTemplateService lts = new LetterTemplateService();
        public XWPFDocument PrintWord(DataEntryDisplayModel DisplayModel, string RegType, B_S_LETTER_TEMPLATE letter)
        {
            try
            {
                string tempPath = lts.getFilePathByLetterType(letter.LETTER_TYPE);
                string filePath = tempPath + letter.FILE_NAME;

                DataEntryPrintModel model = GetPrintModel(DisplayModel.SvSubmission.UUID);

                CT_SectPr sectPr = new CT_SectPr();
                if (Directory.Exists(tempPath))
                {
                    using (FileStream fs = File.OpenRead(filePath))
                    {
                        XWPFDocument doc = new XWPFDocument(fs);
                        sectPr = doc.Document.body.sectPr;
                    }
                }
              
                //sectPr.pgMar = new CT_PageMar();
                //sectPr.pgMar.bottom = "0";
                //sectPr.pgMar.top = "0";
                //sectPr.pgMar.left = UInt64.MinValue;
                //sectPr.pgMar.right = UInt64.MinValue;

                if (SignboardConstant.LANG_CHINESE.Equals(DisplayModel.SvRecord.LANGUAGE_CODE))
                {
                    return BaseCommonService.GetWordDocument(filePath, model, sectPr, SignboardConstant.LANG_CHINESE);
                }
                else
                {
                    return BaseCommonService.GetWordDocument(filePath, model, sectPr, SignboardConstant.LANG_ENGLISH);
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public string GetLMTemplate(DataEntryDisplayModel model)
        {
            StringBuilder path = new StringBuilder();
            if (SignboardConstant.LANG_ENGLISH.Equals(model.SvRecord.LANGUAGE_CODE))
            {
                if (SignboardConstant.FORM_CODE_SC01.Equals(model.FormCode) || SignboardConstant.FORM_CODE_SC02.Equals(model.FormCode))
                {
                    path.Append("LM-SL-SC01_02_EN.docx");
                }
                else if (SignboardConstant.FORM_CODE_SC01C.Equals(model.FormCode) || SignboardConstant.FORM_CODE_SC02C.Equals(model.FormCode))
                {
                    path.Append("LM-SL-SC01_02C_EN.docx");
                }
                else if (SignboardConstant.FORM_CODE_SC03.Equals(model.FormCode))
                {
                    path.Append("LM-SL-SC01_03_EN.docx");
                }
            }
            else if (SignboardConstant.LANG_CHINESE.Equals(model.SvRecord.LANGUAGE_CODE))
            {
                if (SignboardConstant.FORM_CODE_SC01.Equals(model.FormCode) || SignboardConstant.FORM_CODE_SC02.Equals(model.FormCode))
                {
                    path.Append("LM-SL-SC01_02_CH.docx");
                }
                else if (SignboardConstant.FORM_CODE_SC01C.Equals(model.FormCode) || SignboardConstant.FORM_CODE_SC02C.Equals(model.FormCode))
                {
                    path.Append("LM-SL-SC01C_02C_CH.docx");
                }
                else if (SignboardConstant.FORM_CODE_SC03.Equals(model.FormCode))
                {
                    path.Append("LM-SL-SC03_CH.docx");
                }
            }
            return path.ToString();
        }
        public string GetDETemplate(DataEntryDisplayModel model)
        {
            StringBuilder path = new StringBuilder();
            LetterTemplateDAOService letterTemplateDAOService = new LetterTemplateDAOService();
            B_S_LETTER_TEMPLATE Letter = letterTemplateDAOService.GetLetterbyID(model.SelectedLetter);

            if(Letter != null)
            {
                path.Append(Letter.FILE_NAME);
            }

            //if (SignboardConstant.LANG_ENGLISH.Equals(model.SvRecord.LANGUAGE_CODE))
            //{
                //if (SignboardConstant.FORM_CODE_SC01.Equals(Letter.FORM_CODE) && Letter.LETTER_NAME== "SL-SC01A_EN")
                //{
                //    path.Append("SL-SC01A_EN_01.docx");
                //}
                //else if(SignboardConstant.FORM_CODE_SC01.Equals(Letter.FORM_CODE) && Letter.LETTER_NAME == "SL-SC03_EN")
                //{
                //    path.Append("SL-SC03_EN_01.docx");
                //}
                //else if (SignboardConstant.FORM_CODE_SC01.Equals(Letter.FORM_CODE) && Letter.LETTER_NAME == "SL-SC01_EN")
                //{
                //    path.Append("SL-SC01_EN_01.docx");
                //}
                //else if (SignboardConstant.FORM_CODE_SC01C.Equals(Letter.FORM_CODE) && Letter.LETTER_NAME == "SL-SC03_EN")
                //{
                //    path.Append("SL-SC03_EN_01C.docx");
                //}
                //else if (SignboardConstant.FORM_CODE_SC01C.Equals(Letter.FORM_CODE) && Letter.LETTER_NAME == "SL-SC02_EN")
                //{
                //    path.Append("SL-SC02 _EN_01.docx");
                //}
                //else if (SignboardConstant.FORM_CODE_SC02.Equals(Letter.FORM_CODE) && Letter.LETTER_NAME == "SL-SC01A_EN")
                //{
                //    path.Append("SL-SC01A_EN_02.docx");
                //}
                //else if (SignboardConstant.FORM_CODE_SC02.Equals(Letter.FORM_CODE) && Letter.LETTER_NAME == "SL-SC03_EN")
                //{
                //    path.Append("SL-SC03_EN_02.docx");
                //}
                //else if (SignboardConstant.FORM_CODE_SC02.Equals(Letter.FORM_CODE) && Letter.LETTER_NAME == "SL-SC01_EN")
                //{
                //    path.Append("SL-SC01_EN_02.docx");
                //}
                //else if (SignboardConstant.FORM_CODE_SC02C.Equals(Letter.FORM_CODE) && Letter.LETTER_NAME == "SL-SC02_EN")
                //{
                //    path.Append("SL-SC02 _EN_02.docx");
                //}
                //else if (SignboardConstant.FORM_CODE_SC02C.Equals(Letter.FORM_CODE) && Letter.LETTER_NAME == "SL-SC03_EN")
                //{
                //    path.Append("SL-SC03_EN_02C.docx");
                //}
                //else if (SignboardConstant.FORM_CODE_SC03.Equals(Letter.FORM_CODE) && Letter.LETTER_NAME == "SL-SC03_EN")
                //{
                //    path.Append("SL-SC03_EN_03.docx");
                //}
                //else if (SignboardConstant.FORM_CODE_SC03.Equals(Letter.FORM_CODE) && Letter.LETTER_NAME == "SL-SC01_EN")
                //{
                //    path.Append("SL-SC01_EN_03.docx");
                //}
            //}
            //else if (SignboardConstant.LANG_CHINESE.Equals(model.SvRecord.LANGUAGE_CODE))
            //{
                //if (SignboardConstant.FORM_CODE_SC01.Equals(Letter.FORM_CODE) && Letter.LETTER_NAME == "SL-SC01A_CH")
                //{
                //    path.Append("SL-SC01A_CH_01.docx");
                //}
                //else if (SignboardConstant.FORM_CODE_SC01.Equals(Letter.FORM_CODE) && Letter.LETTER_NAME == "SL-SC03_CH")
                //{
                //    path.Append("SL-SC03_CH_01.docx");
                //}
                //else if (SignboardConstant.FORM_CODE_SC01.Equals(Letter.FORM_CODE) && Letter.LETTER_NAME == "SL-SC01_CH")
                //{
                //    path.Append("SL-SC01_CH_01.docx");
                //}
                //else if (SignboardConstant.FORM_CODE_SC01C.Equals(Letter.FORM_CODE) && Letter.LETTER_NAME == "SL-SC02_CH")
                //{
                //    path.Append("SL-SC02 _CH_01.docx");
                //}
                //else if (SignboardConstant.FORM_CODE_SC01C.Equals(Letter.FORM_CODE) && Letter.LETTER_NAME == "SL-SC03_CH")
                //{
                //    path.Append("SL-SC03_CH_01C.docx");
                //}
                //else if (SignboardConstant.FORM_CODE_SC02.Equals(Letter.FORM_CODE) && Letter.LETTER_NAME == "SL-SC01A_CH")
                //{
                //    path.Append("SL-SC01A_CH_02.docx");
                //}
                //else if (SignboardConstant.FORM_CODE_SC02.Equals(Letter.FORM_CODE) && Letter.LETTER_NAME == "SL-SC03_CH")
                //{
                //    path.Append("SL-SC03_CH_02.docx");
                //}
                //else if (SignboardConstant.FORM_CODE_SC02.Equals(Letter.FORM_CODE) && Letter.LETTER_NAME == "SL-SC01_CH")
                //{
                //    path.Append("SL-SC01_CH_02.docx");
                //}
                //else if (SignboardConstant.FORM_CODE_SC02C.Equals(Letter.FORM_CODE) && Letter.LETTER_NAME == "SL-SC02_CH")
                //{
                //    path.Append("SL-SC02 _CH_02.docx");
                //}
                //else if (SignboardConstant.FORM_CODE_SC02C.Equals(Letter.FORM_CODE) && Letter.LETTER_NAME == "SL-SC03_CH")
                //{
                //    path.Append("SL-SC03_CH_02C.docx");
                //}
                //else if (SignboardConstant.FORM_CODE_SC03.Equals(Letter.FORM_CODE) && Letter.LETTER_NAME == "SL-SC03_CH")
                //{
                //    path.Append("SL-SC03_CH_03.docx");
                //}
                //else if (SignboardConstant.FORM_CODE_SC03.Equals(Letter.FORM_CODE) && Letter.LETTER_NAME == "SL-SC01_CH")
                //{
                //    path.Append("SL-SC01_CH_03.docx");
                //}
            //}
            return path.ToString();

        }
        public DataEntryPrintModel GetPrintModel(string id)
        {
            DataEntryPrintModel model = GetDEAndLMPrintModel(id);
            //GetSvRecordInfo(model);
            //GetPRCInfo(model);
            //GetLetterAdress(model);
            //GetLetterPaw(model);
            //GetSPO(model);
            return model;
        }
        public DataEntryPrintModel GetDEAndLMPrintModel(string id)
        {
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                SvRecordDAOService svRecordDAOService = new SvRecordDAOService();
                SvSubmissionDAOService svSubmissionDAOService = new SvSubmissionDAOService();
                SvAppointedProfessionalDAOService svAppointedProfessionalDAOService = new SvAppointedProfessionalDAOService();

                //B_SV_SUBMISSION svSubmission = svSubmissionDAOService.getByUuid(id);

                B_SV_RECORD svRecord = svRecordDAOService.getSVRecordBySvSubmissionUUID(id);

                B_SV_SIGNBOARD svSignboard = db.B_SV_SIGNBOARD.Where
                        (o => o.UUID == svRecord.SV_SIGNBOARD_ID).Include(o => o.B_SV_PERSON_CONTACT)
                        .Include(o => o.B_SV_ADDRESS).FirstOrDefault();

                B_SV_PERSON_CONTACT owner = db.B_SV_PERSON_CONTACT.Where
                    (o => o.UUID == svSignboard.OWNER_ID).Include(o => o.B_SV_ADDRESS).FirstOrDefault();

                B_SV_PERSON_CONTACT paw = db.B_SV_PERSON_CONTACT.Where
                    (o => o.UUID == svRecord.PAW_ID).Include(o => o.B_SV_ADDRESS).FirstOrDefault();

                B_SV_PERSON_CONTACT oi = db.B_SV_PERSON_CONTACT.Where
                    (o => o.UUID == svRecord.OI_ID).Include(o => o.B_SV_ADDRESS).FirstOrDefault();

                B_SV_ADDRESS ownerAddress = (from svaddre in db.B_SV_ADDRESS
                                join svpc in db.B_SV_PERSON_CONTACT on svaddre.UUID equals svpc.SV_ADDRESS_ID
                                join svs in db.B_SV_SIGNBOARD on svpc.UUID equals svSignboard.OWNER_ID
                                select svaddre).FirstOrDefault();

                B_SV_APPOINTED_PROFESSIONAL ap = svAppointedProfessionalDAOService.getSvAppointedProfessional(svRecord.UUID, SignboardConstant.PBP_CODE_AP);

                B_SV_APPOINTED_PROFESSIONAL rge = svAppointedProfessionalDAOService.getSvAppointedProfessional(svRecord.UUID, SignboardConstant.PBP_CODE_RGE);

                B_SV_APPOINTED_PROFESSIONAL rse = svAppointedProfessionalDAOService.getSvAppointedProfessional(svRecord.UUID, SignboardConstant.PBP_CODE_RSE);

                B_SV_APPOINTED_PROFESSIONAL prc = svAppointedProfessionalDAOService.getSvAppointedProfessional(svRecord.UUID, SignboardConstant.PRC_CODE);

                B_SV_APPOINTED_PROFESSIONAL ri = svAppointedProfessionalDAOService.getSvAppointedProfessional(svRecord.UUID, SignboardConstant.PBP_CODE_RI);
                string RD_NAME_E = "";
                string RD_NAME_C = "";
                string RD_CONTACT = "";
                DateTime Today = DateTime.Now;


                if (SignboardConstant.FORM_CODE_SC01.Equals(svRecord.FORM_CODE)|| SignboardConstant.FORM_CODE_SC01C.Equals(svRecord.FORM_CODE))
                {
                    RD_NAME_E = prc.ENGLISH_NAME;
                    RD_NAME_C = prc.CHINESE_NAME;
                    RD_CONTACT = prc.CONTACT_NO;
                }
                else
                {
                    RD_NAME_E = rge.ENGLISH_NAME;
                    RD_NAME_C = rge.CHINESE_NAME;
                    RD_CONTACT = rge.CONTACT_NO;
                    //putFaxOraddr(RD_FAX_OR_ADDR_E,RD_FAX_OR_ADDR_C,rge)
                }
                string PAW_FAX_OR_ADDR = "";
                if (!string.IsNullOrWhiteSpace(paw.FAX_NO))
                {
                    PAW_FAX_OR_ADDR = paw.FAX_NO;
                }
                else
                {
                    PAW_FAX_OR_ADDR = paw.B_SV_ADDRESS.FULL_ADDRESS;
                }
                DataEntryPrintModel model = new DataEntryPrintModel()
                {
                    //svRecord svSignboard SVaddress
                    FLOOR = svSignboard.B_SV_ADDRESS.FLOOR != null ? svSignboard.B_SV_ADDRESS.FLOOR : ""
                    ,
                    FLAT = svSignboard.B_SV_ADDRESS.FLAT != null ? svSignboard.B_SV_ADDRESS.FLOOR : ""
                    ,
                    STREET = svSignboard.B_SV_ADDRESS.STREET != null ? svSignboard.B_SV_ADDRESS.STREET : ""
                    ,
                    BLOCK = svSignboard.B_SV_ADDRESS.BLOCK != null ? svSignboard.B_SV_ADDRESS.BLOCK : ""
                    ,
                    STREET_NO = svSignboard.B_SV_ADDRESS.STREET_NO != null ? svSignboard.B_SV_ADDRESS.STREET_NO : ""
                    ,
                    BUILDINGNAME = svSignboard.B_SV_ADDRESS.BUILDINGNAME != null ? svSignboard.B_SV_ADDRESS.BUILDINGNAME : ""
                    ,
                    DISTRICT = svSignboard.B_SV_ADDRESS.DISTRICT != null ? svSignboard.B_SV_ADDRESS.DISTRICT : ""
                    ,
                    REGION = svSignboard.B_SV_ADDRESS.REGION != null ? svSignboard.B_SV_ADDRESS.REGION : ""
                    ,
                    FULL_ADDRESS = svSignboard.B_SV_ADDRESS.FULL_ADDRESS != null ? svSignboard.B_SV_ADDRESS.FULL_ADDRESS : ""
                    ,
                    SUBMISSION_NO = svRecord.REFERENCE_NO != null ? svRecord.REFERENCE_NO : ""
                    ,
                    PAW_NAME_E = paw.NAME_ENGLISH != null ? paw.NAME_ENGLISH : ""
                    ,
                    PAW_NAME_C = paw.NAME_CHINESE != null ? paw.NAME_CHINESE : ""
                    ,
                    PAW_CONTACT = paw.CONTACT_NO != null ? paw.CONTACT_NO : ""
                    ,
                    PAW_FAX_OR_ADDR = PAW_FAX_OR_ADDR != null ? PAW_FAX_OR_ADDR : ""
                    ,
                    PBP_NAME_E = paw.NAME_ENGLISH != null ? paw.NAME_ENGLISH : ""
                    ,
                    PBP_NAME_C = paw.NAME_CHINESE != null ? paw.NAME_CHINESE : ""
                    ,
                    PBP_NAME = paw.NAME_ENGLISH != null ? paw.NAME_ENGLISH : ""
                    ,
                    PRC_NAME_E = prc.ENGLISH_NAME != null ? prc.ENGLISH_NAME : ""
                    ,
                    PRC_NAME_C = prc.CHINESE_NAME != null ? prc.CHINESE_NAME : ""
                    ,
                    PRC_NAME = prc.ENGLISH_NAME != null ? prc.ENGLISH_NAME : ""
                    ,
                    PRC_CONTACT = prc.CONTACT_NO != null ? prc.CONTACT_NO : ""
                    ,
                    PRC_FAX_OR_ADDR_E = prc.FAX_NO != null ? prc.FAX_NO : ""
                    ,
                    PRC_FAX_OR_ADDR_C = prc.FAX_NO != null ? prc.FAX_NO : ""
                    ,
                    AP_NAME_C = ap.CHINESE_NAME != null ? ap.CHINESE_NAME : ""
                    ,
                    AP_NAME_E = ap.ENGLISH_NAME != null ? ap.ENGLISH_NAME : ""
                    ,
                    AP_CONTACT = ap.CONTACT_NO != null ? ap.CONTACT_NO : ""
                    ,
                    AP_FAX_OR_ADDR_C = ap.FAX_NO != null ? ap.CONTACT_NO : ""
                    ,
                    AP_FAX_OR_ADDR_E = ap.CONTACT_NO != null ? ap.CONTACT_NO : ""
                    ,
                    RSE_NAME_E = rse.ENGLISH_NAME != null ? rse.ENGLISH_NAME : ""
                    ,
                    RSE_NAME_C = rse.CHINESE_NAME != null ? rse.CHINESE_NAME : ""
                    ,
                    RSE_CONTACT = rse.CONTACT_NO != null ? rse.CONTACT_NO : ""
                    ,
                    RSE_FAX_OR_ADDR_C = rse.FAX_NO != null ? rse.FAX_NO : ""
                    ,
                    RSE_FAX_OR_ADDR_E = rse.FAX_NO != null ? rse.FAX_NO : ""
                    ,
                    RI_NAME_C = ri.CHINESE_NAME != null ? ri.CHINESE_NAME : ""
                    ,
                    RI_NAME_E = ri.ENGLISH_NAME != null ? ri.ENGLISH_NAME : ""
                    ,
                    RD_NAME_C = RD_NAME_C != null ? RD_NAME_C : ""
                    ,
                    RD_NAME_E = RD_NAME_E != null ? RD_NAME_E : ""
                    ,
                    RD_CONTACT = RD_CONTACT != null ? RD_CONTACT : ""
                    ,
                    APPOINTED_PERSON_NAME_C = ap.ENGLISH_NAME != null ? ap.ENGLISH_NAME : ""
                    ,
                    APPOINTED_PERSON_NAME_E = ap.CHINESE_NAME != null ? ap.CHINESE_NAME : ""
                    ,
                    //APPOINTED_PESON_COMPANY_NAME_C = != null ? APPOINTED_PESON_COMPANY_NAME_C : ""
                    //,
                    //APPOINTED_PESON_COMPANY_NAME_E = != null ? APPOINTED_PESON_COMPANY_NAME_E : ""
                    //,
                    //APPOINTED_PESON_ADDRESS_ROOM_FLAT_BLOCK_E = != null ? APPOINTED_PESON_ADDRESS_ROOM_FLAT_BLOCK_E : ""
                    //,
                    //APPOINTED_PESON_ADDRESS_ROOM_FLAT_BLOCK_C = != null ? APPOINTED_PESON_ADDRESS_ROOM_FLAT_BLOCK_C : ""
                    //,
                    //APPOINTED_PESON_STREET_C = != null ? APPOINTED_PESON_STREET_C : ""
                    //,
                    //APPOINTED_PESON_STREET_E = != null ? APPOINTED_PESON_STREET_E : ""
                    //,
                    //APPOINTED_PESON_DISTRICT_E = != null ? APPOINTED_PESON_DISTRICT_E : ""
                    //,
                    //APPOINTED_PESON_DISTRICT_C = != null ? APPOINTED_PESON_DISTRICT_C : ""
                    //,
                    //APPOINTED_PESON_REGION_E = != null ? APPOINTED_PESON_REGION_E : ""
                    //,
                    //APPOINTED_PESON_REGION_C = != null ? APPOINTED_PESON_REGION_C : ""
                    //,
                    //Signboard
                    SIGNBOARD_OWNER_NAME_E = owner.NAME_ENGLISH != null ? owner.NAME_ENGLISH : ""
                    ,
                    SIGNBOARD_OWNER_NAME_C = owner.NAME_CHINESE != null ? owner.NAME_CHINESE : ""
                    ,
                    LOCATION_OF_SIGNBOARD = svSignboard.LOCATION_OF_SIGNBOARD != null ? svSignboard.LOCATION_OF_SIGNBOARD : ""
                    ,
                    SIGNBOARD_DESCRIPTION = svSignboard.DESCRIPTION != null ? svSignboard.DESCRIPTION : ""
                    ,
                    NOTIFY_DATE = svRecord.ACK_LETTERISS_DATE.ToString() != null ? svRecord.ACK_LETTERISS_DATE.ToString() : ""
                    ,
                    NOTIFY_DATE_C = DateUtil.getChineseFormatDate(svRecord.ACK_LETTERISS_DATE).ToString() != null ? DateUtil.getChineseFormatDate(svRecord.ACK_LETTERISS_DATE).ToString() : ""
                    ,
                    RECEIVED_DATE = svRecord.RECEIVED_DATE.ToString() != null ? svRecord.RECEIVED_DATE.ToString() : ""
                    ,
                    RECEIVED_DATE_C = DateUtil.getChineseFormatDate(svRecord.RECEIVED_DATE).ToString() != null ? DateUtil.getChineseFormatDate(svRecord.RECEIVED_DATE).ToString() : ""
                    ,
                    RDATE_Y = svRecord.RECEIVED_DATE.Value.Year.ToString()
                    ,
                    RDATE_M = svRecord.RECEIVED_DATE.Value.Month.ToString()
                    ,
                    RDATE_M_STR = svRecord.RECEIVED_DATE.Value.ToString("MMM", new CultureInfo("zh-cn"))
                    ,
                    RDATE_D = svRecord.RECEIVED_DATE.Value.Day.ToString("dd")
                    ,
                    TODAY_Y = Today.Year.ToString()
                    ,
                    TODAY_M = Today.Month.ToString()
                    ,
                    TODAY_M_STR = Today.ToString("MMM", new CultureInfo("zh-cn"))
                    //Today.Month.ToString("MMMM")
                    ,
                    TODAY_D = Today.ToString("dd")
                    ,
                    FORM_CODE = svRecord.FORM_CODE
                };
                return model;
            }
        }
        public string FormName(DataEntryDisplayModel model)
        {
            string FormName = "";
            if (SignboardConstant.LANG_ENGLISH.Equals(model.SvRecord.LANGUAGE_CODE))
            {
                if (SignboardConstant.FORM_CODE_SC01.Equals(model.FormCode) || SignboardConstant.FORM_CODE_SC02.Equals(model.FormCode))
                {
                    FormName="LM-SL-SC01_02_EN.docx";
                }
                else if (SignboardConstant.FORM_CODE_SC01C.Equals(model.FormCode) || SignboardConstant.FORM_CODE_SC02C.Equals(model.FormCode))
                {
                    FormName="LM-SL-SC01_02C_EN.docx";
                }
                else if (SignboardConstant.FORM_CODE_SC03.Equals(model.FormCode))
                {
                    FormName="LM-SL-SC01_03_EN.docx";
                }
            }
            else if (SignboardConstant.LANG_CHINESE.Equals(model.SvRecord.LANGUAGE_CODE))
            {
                if (SignboardConstant.FORM_CODE_SC01.Equals(model.FormCode) || SignboardConstant.FORM_CODE_SC02.Equals(model.FormCode))
                {
                    FormName="LM-SL-SC01_02_CH.docx";
                }
                else if (SignboardConstant.FORM_CODE_SC01C.Equals(model.FormCode) || SignboardConstant.FORM_CODE_SC02C.Equals(model.FormCode))
                {
                    FormName="LM-SL-SC01C_02C_CH.docx";
                }
                else if (SignboardConstant.FORM_CODE_SC03.Equals(model.FormCode))
                {
                    FormName="LM-SL-SC03_CH.docx";
                }
            }
            return FormName;
        }

        public B_S_LETTER_TEMPLATE getLetterModuleLetterTemplate(DataEntryDisplayModel model)
        {
            B_S_LETTER_TEMPLATE letter = new B_S_LETTER_TEMPLATE();
            string lang = "";
            if(SignboardConstant.LANG_ENGLISH.Equals(model.SvRecord.LANGUAGE_CODE)) // EN
            {
                lang = SignboardConstant.LETTER_TEMPLATE_ENG;
            }
            else // ZH
            {
                lang = SignboardConstant.LETTER_TEMPLATE_CHIN;
            }

            using(EntitiesSignboard db = new EntitiesSignboard())
            {
                letter = db.B_S_LETTER_TEMPLATE.Where(x => x.FORM_CODE == model.FormCode && x.LETTER_NAME.Contains(lang)).FirstOrDefault();
            }

            return letter;
        }

        public DataEntryDisplayModel getLetter(DataEntryDisplayModel model)
        {
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                //var Vquery = db.B_SV_RECORD.Find(model.B_SV_VALIDATION.B_SV_RECORD.UUID);
                var query = db.B_S_LETTER_TEMPLATE.Where(x => x.FORM_CODE == model.FormCode && x.RESULT == "Accept" && x.LETTER_TYPE == model.SelectedLetterType);
                model.B_S_LETTER_TEMPLATE_List = query.ToList();
                return model;
            }

        }
    }
}
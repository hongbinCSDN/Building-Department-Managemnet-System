using MWMS2.Areas.WarningLetter.Models;
using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Services
{
    public class WarningLetterSearchService
    {
        //distinct(AS_UUID) AS AS_UUID, v.SURNAME || v.GIVEN_NAME AS ASCOMPNAME, v.CHINESE_NAME AS ASCOMPCHNNAME,AS_CHINESE_NAME, AS_SURNAME || AS_GIVEN_NAME AS ASFULLNAME,
        //private const string SearchWL_q = ""
        //                                + "\r\n" + "\t" + "select  distinct(AS_UUID) AS AS_APP_UUID,  v.SURNAME ||' '|| v.GIVEN_NAME AS ASCOMPNAME, v.CHINESE_NAME AS ASCOMPCHNNAME,AS_CHINESE_NAME, AS_SURNAME || AS_GIVEN_NAME AS ASFULLNAME,wl.*,wl.FILE_REF_FOUR|| '/' ||FILE_REF_TWO as FileReference ,                                                                             "
        //                                + "\r\n" + "\t" + "(SELECT LISTAGG( offense.WL_TYPE_OF_OFFENSE_ENG, ';')  WITHIN GROUP (ORDER BY offense.WL_TYPE_OF_OFFENSE_ENG)              "
        //                                + "\r\n" + "\t" + "          FROM W_WL_TYPE_OF_OFFENSE offense                                                                                "
        //                                + "\r\n" + "\t" + "          WHERE offense.WL_UUID = wl.UUID ) as Offense,                                                                    "
        //                                + "\r\n" + "\t" + "(SELECT LISTAGG( mwitem.WL_MW_ITEM_ENG, ',')  WITHIN GROUP (ORDER BY LPAD( LPAD(substr(mwitem.WL_MW_ITEM_ENG,3,2),2,'0'),4,substr(mwitem.WL_MW_ITEM_ENG,1,2)))                                "
        //                                + "\r\n" + "\t" + " FROM W_WL_MW_ITEM mwitem                                                                                                  "
        //                                + "\r\n" + "\t" + " WHERE mwitem.WL_UUID = wl.UUID ) as MW_Items                                                                              "
        //                                 + "\r\n" + "\t" + "from W_WL wl                                                                                                               "
        //                                 + "\r\n" + "\t" + " LEFT JOIN V_CRM_INFO v ON wl.AS_UUID = v.UUID AND wl.REGISTRATION_NO = v.CERTIFICATION_NO"
        //                                  + "\r\n" + "\t" + "where 1=1           
        //";
        private const string SearchWL_q = ""
                                + "\r\n" + "\t" + "select  (SELECT max(SURNAME ||' '|| v.GIVEN_NAME)  FROM V_CRM_INFO v WHERE v.CERTIFICATION_NO = wl.REGISTRATION_NO) AS ASCOMPNAME,  wl.*,wl.FILE_REF_FOUR|| '/' ||FILE_REF_TWO as FileReference ,                                                                         "
                               + "\r\n" + "\t" + "(SELECT max(CHINESE_NAME) FROM V_CRM_INFO v WHERE v.CERTIFICATION_NO = wl.REGISTRATION_NO)  AS ASCOMPCHNNAME,"
                                + "\r\n" + "\t" + "(SELECT LISTAGG( offense.WL_TYPE_OF_OFFENSE_ENG, ';')  WITHIN GROUP (ORDER BY offense.WL_TYPE_OF_OFFENSE_ENG)              "
                                + "\r\n" + "\t" + "          FROM W_WL_TYPE_OF_OFFENSE offense                                                                                "
                                + "\r\n" + "\t" + "          WHERE offense.WL_UUID = wl.UUID ) as Offense,                                                                    "
                                + "\r\n" + "\t" + "(SELECT LISTAGG( mwitem.WL_MW_ITEM_ENG, ',')  WITHIN GROUP (ORDER BY LPAD( LPAD(substr(mwitem.WL_MW_ITEM_ENG,3,2),2,'0'),4,substr(mwitem.WL_MW_ITEM_ENG,1,2)))                                "
                                + "\r\n" + "\t" + " FROM W_WL_MW_ITEM mwitem                                                                                                  "
                                + "\r\n" + "\t" + " WHERE mwitem.WL_UUID = wl.UUID ) as MW_Items                                                                              "
                                 + "\r\n" + "\t" + "from W_WL wl                                                                                                               "

                                  + "\r\n" + "\t" + "where 1=1                                                                                              ";

        //and(wl.STATUS !='Deleted' or wl.status is null)

        public WLModel SearchWarningLetter(WLModel model)
        {
            model.Query = SearchWL_q;
            model.QueryWhere = SearchWL_whereQ(model);
            model.Search();
            return model;
        }
        private string SearchWL_whereQ(WLModel wl)
        {
            string whereQ = "";
            if (wl.v_Offense_Type_CheckBox != null && wl.v_Offense_Type_CheckBox.Count() != 0)
            {
                whereQ += "\r\n\t" + "AND Exists (select 1 from W_WL_TYPE_OF_OFFENSE j1 where j1.WL_UUID = wl.UUID and j1.WL_TYPE_OF_OFFENSE_ENG in  ( :Offense) ) ";
                wl.QueryParameters.Add("Offense", wl.v_Offense_Type_CheckBox);
            }
           
              if(!string.IsNullOrWhiteSpace(wl.Section))
            {
                whereQ += "\r\n\t" + "AND SECTION_UNIT =   :Sect ";
                wl.QueryParameters.Add("Sect", wl.Section);


            }
            if (wl.v_Source_checkbox != null && wl.v_Source_checkbox.Count() != 0)
            {
                whereQ += "\r\n\t" + "AND SOURCE in  ( :Source) ";
                wl.QueryParameters.Add("Source", wl.v_Source_checkbox);
            }
            if (wl.COM_Creation_Status != null && wl.COM_Creation_Status.Count() != 0)
            {
                whereQ += "\r\n\t" + "AND COMP_CONTRACTOR_STATUS in  (:CStatus) ";
                wl.QueryParameters.Add("CStatus", wl.COM_Creation_Status);
            }
            if (wl.AS_Creation_Status != null && wl.AS_Creation_Status.Count() != 0)
            {
                whereQ += "\r\n\t" + "AND AS_STATUS in  (:ASStatus) ";
                wl.QueryParameters.Add("ASStatus", wl.AS_Creation_Status);
            }
            if (!String.IsNullOrEmpty(wl.SUBJECT))
            {
                whereQ += "\r\n\t" + "AND UPPER(SUBJECT) LIKE  :SUBJECT ";
                wl.QueryParameters.Add("SUBJECT", "%" + wl.SUBJECT.ToUpper() + "%");
            }
            if (!String.IsNullOrEmpty(wl.REGISTRATION_NO))
            {
                whereQ += "\r\n\t" + "AND UPPER(REGISTRATION_NO) LIKE   :REGISTRATION_NO ";
                wl.QueryParameters.Add("REGISTRATION_NO", "%" + wl.REGISTRATION_NO.ToUpper() + "%");
            }
            if (!String.IsNullOrEmpty(wl.FILE_REF_FOUR))
            {
                whereQ += "\r\n\t" + "AND FILE_REF_FOUR LIKE   :REFFOUR ";
                wl.QueryParameters.Add("REFFOUR", "%" + wl.FILE_REF_FOUR + "%");
            }
            if (!String.IsNullOrEmpty(wl.FILE_REF_TWO))
            {
                whereQ += "\r\n\t" + "AND FILE_REF_TWO LIKE   :REFTWO ";
                wl.QueryParameters.Add("REFTWO", "%" + wl.FILE_REF_TWO + "%");
            }
          
            if (!String.IsNullOrEmpty(wl.COMP_CONTRACTOR_NAME_ENG))
            {
                if (wl.COMP_CONTRACTOR_NAME_ENG.Any(c => (uint)c >= 0x4E00 && (uint)c <= 0x2FA1F))
                {
                    whereQ += "\r\n\t" + "AND ( COMP_CONTRACTOR_NAME_CHI LIKE  ( :CCNChi) or AUTHORIZED_SIGNATORY_NAME_CHI LIKE (:CCNChi)  )";
                    wl.QueryParameters.Add("CCNChi", "%" + wl.COMP_CONTRACTOR_NAME_ENG + "%");
                }
                else
                {
                    whereQ += "\r\n\t" + "AND ( UPPER(COMP_CONTRACTOR_NAME_ENG) LIKE  ( :CCNEng) or AUTHORIZED_SIGNATORY_NAME_ENG LIKE (:CCNEng) ) ";
                    wl.QueryParameters.Add("CCNEng", "%" + wl.COMP_CONTRACTOR_NAME_ENG.Trim().ToUpper() + "%");
                }
            }
            if (!String.IsNullOrEmpty(wl.CASE_OFFICER))
            {
                whereQ += "\r\n\t" + "AND UPPER(CASE_OFFICER) LIKE   :CASE_OFFICER  ";
                wl.QueryParameters.Add("CASE_OFFICER", "%" + wl.CASE_OFFICER.ToUpper() + "%");
            }
            if (wl.SearchString_CreateStartDate != null)
            {
                whereQ += "\r\n\t" + "AND CREATED_DATE >= :DateFrom";
                wl.QueryParameters.Add("DateFrom", wl.SearchString_CreateStartDate);
            }
            if (wl.SearchString_CreateEndDate != null)
            {
                whereQ += "\r\n\t" + "AND CREATED_DATE <= :DateTo";
                wl.QueryParameters.Add("DateTo", wl.SearchString_CreateEndDate);
            }

            if (wl.SearchString_IssuedStartDate != null)
            {
                whereQ += "\r\n\t" + "AND LETTER_ISSUE_DATE >= :IDateFrom";
                wl.QueryParameters.Add("IDateFrom", wl.SearchString_IssuedStartDate);
            }
            if (wl.SearchString_IssuedEndDate != null)
            {
                whereQ += "\r\n\t" + "AND LETTER_ISSUE_DATE <= :IDateTo";
                wl.QueryParameters.Add("IDateTo", wl.SearchString_IssuedEndDate);
            }
            if (!String.IsNullOrEmpty(wl.REMARK))
            {
                whereQ += "\r\n\t" + "AND UPPER(REMARK) LIKE   :REMARK  ";
                wl.QueryParameters.Add("REMARK", "%" + wl.REMARK.Trim().ToUpper() + "%");
            }
            return whereQ;
        }

















        public ModelWN SearchWL(ModelWN model)
        {
            model.Query = SearchWL_q;
            model.QueryWhere = SearchWL_whereQ(model);
            model.Search();
            return model;
        }
        private string SearchWL_whereQ(ModelWN wN)
        {
            string whereQ = "";
            if (wN.v_Offense_Type_CheckBox != null && wN.v_Offense_Type_CheckBox.Count() != 0)
            {
               whereQ += "\r\n\t" + "AND Exists (select 1 from W_WL_TYPE_OF_OFFENSE j1 where j1.WL_UUID = wl.UUID and j1.WL_TYPE_OF_OFFENSE_ENG in  ( :Offense) ) ";
               wN.QueryParameters.Add("Offense", wN.v_Offense_Type_CheckBox);
            }
            if (wN.v_MWItems_Type_CheckBox != null && wN.v_MWItems_Type_CheckBox.Count() != 0)
            {
              whereQ += "\r\n\t" + "AND Exists (select 1 from W_WL_MW_ITEM j2 where j2.WL_UUID = wl.UUID and j2.WL_MW_ITEM_ENG in  ( :MWItems) )";
              wN.QueryParameters.Add("MWItems", wN.v_MWItems_Type_CheckBox);
            }
            if (wN.v_Cat_Checkbox != null && wN.v_Cat_Checkbox.Count() != 0)
            {

                whereQ += "\r\n\t" + "AND CATEGORY in  ( :Cat) ";
                wN.QueryParameters.Add("Cat", wN.v_Cat_Checkbox);
            }
            if (wN.v_Section_checkbox != null && wN.v_Section_checkbox.Count() != 0)
            {
                whereQ += "\r\n\t" + "AND SECTION_UNIT in  ( :Sect) ";
                wN.QueryParameters.Add("Sect", wN.v_Section_checkbox);


            }
            if (wN.v_Related_checkbox != null && wN.v_Related_checkbox.Count() != 0)
            {
                whereQ += "\r\n\t" + "AND ( RELATED_TO in  ( :Rel) ) ";
                //whereQ += "\r\n\t" + "AND ( RELATED_TO in  ( :Rel) or RELATED_TO IS NULL) ";
                wN.QueryParameters.Add("Rel", wN.v_Related_checkbox);

            }
          

            if (wN.v_Source_checkbox != null && wN.v_Source_checkbox.Count() != 0)
            {
                whereQ += "\r\n\t" + "AND SOURCE in  ( :Source) ";
                wN.QueryParameters.Add("Source", wN.v_Source_checkbox);


            }
            if (wN.COM_Creation_Status != null && wN.COM_Creation_Status.Count() != 0)
            {
                whereQ += "\r\n\t" + "AND COMP_CONTRACTOR_STATUS in  (:CStatus) ";
                wN.QueryParameters.Add("CStatus", wN.COM_Creation_Status);


            }
            if (wN.AS_Creation_Status != null && wN.AS_Creation_Status.Count() != 0)
            {
                whereQ += "\r\n\t" + "AND AS_STATUS in  (:ASStatus) ";
                wN.QueryParameters.Add("ASStatus", wN.AS_Creation_Status);


            }
            // COM_Creation_Status
            // AS_Creation_Status
            // COM_Current_Status
            // AS_Current_Status




            if (!String.IsNullOrEmpty(wN.SUBJECT))
            {
                whereQ += "\r\n\t" + "AND UPPER(SUBJECT) LIKE  :SUBJECT ";
                wN.QueryParameters.Add("SUBJECT","%"+ wN.SUBJECT.ToUpper() +"%");

            }
            if (!String.IsNullOrEmpty(wN.REGISTRATION_NO))
            {
                whereQ += "\r\n\t" + "AND UPPER(REGISTRATION_NO) LIKE   :REGISTRATION_NO ";
                wN.QueryParameters.Add("REGISTRATION_NO", "%" + wN.REGISTRATION_NO.ToUpper() +"%");
            }
            if (!String.IsNullOrEmpty(wN.FILE_REF_FOUR))
            {
                whereQ += "\r\n\t" + "AND FILE_REF_FOUR LIKE   :REFFOUR ";
                wN.QueryParameters.Add("REFFOUR", "%" + wN.FILE_REF_FOUR + "%");
            }
            if (!String.IsNullOrEmpty(wN.FILE_REF_TWO))
            {
                whereQ += "\r\n\t" + "AND FILE_REF_TWO LIKE   :REFTWO ";
                wN.QueryParameters.Add("REFTWO", "%" + wN.FILE_REF_TWO + "%");
            }
            if (!String.IsNullOrEmpty(wN.MW_SUBMISSION_NO))
            {
                
                whereQ += "\r\n\t" + "AND UPPER(MW_SUBMISSION_NO) LIKE   :MWSUBNO  ";
                wN.QueryParameters.Add("MWSUBNO", "%" + wN.MW_SUBMISSION_NO.ToUpper() + "%");

            }
            if (!String.IsNullOrEmpty(wN.COMP_CONTRACTOR_NAME_ENG))
            {
                if (wN.COMP_CONTRACTOR_NAME_ENG.Any(c => (uint)c >= 0x4E00 && (uint)c <= 0x2FA1F))
                {
                    whereQ += "\r\n\t" + "AND ( COMP_CONTRACTOR_NAME_CHI LIKE  ( :CCNChi) or AUTHORIZED_SIGNATORY_NAME_CHI LIKE (:CCNChi)  )";
                    wN.QueryParameters.Add("CCNChi", "%"+wN.COMP_CONTRACTOR_NAME_ENG+"%");
                }
                else
                {
                      whereQ += "\r\n\t" + "AND ( UPPER(COMP_CONTRACTOR_NAME_ENG) LIKE  ( :CCNEng) or AUTHORIZED_SIGNATORY_NAME_ENG LIKE (:CCNEng) ) ";
                      wN.QueryParameters.Add("CCNEng","%" +wN.COMP_CONTRACTOR_NAME_ENG.Trim().ToUpper()+"%");
                }

            }


            if (!String.IsNullOrEmpty(wN.CASE_OFFICER))
            {
                whereQ += "\r\n\t" + "AND UPPER(CASE_OFFICER) LIKE   :CASE_OFFICER  ";
                wN.QueryParameters.Add("CASE_OFFICER", "%" + wN.CASE_OFFICER.ToUpper() + "%");
            }
            if (wN.SearchString_CreateStartDate != null )
            {
                whereQ += "\r\n\t" + "AND CREATED_DATE >= :DateFrom";
                wN.QueryParameters.Add("DateFrom", wN.SearchString_CreateStartDate);
            }
            if (wN.SearchString_CreateEndDate != null)
            {
                whereQ += "\r\n\t" + "AND CREATED_DATE <= :DateTo";
                wN.QueryParameters.Add("DateTo", wN.SearchString_CreateEndDate);
            }

            if (wN.SearchString_IssuedStartDate != null)
            {
                whereQ += "\r\n\t" + "AND LETTER_ISSUE_DATE >= :IDateFrom";
                wN.QueryParameters.Add("IDateFrom", wN.SearchString_IssuedStartDate);
            }
            if (wN.SearchString_IssuedEndDate != null)
            {
                whereQ += "\r\n\t" + "AND LETTER_ISSUE_DATE <= :IDateTo";
                wN.QueryParameters.Add("IDateTo", wN.SearchString_IssuedEndDate);
            }
            if (!String.IsNullOrEmpty(wN.WL_ISSUED_BY))
            {
             

            }
            if (!String.IsNullOrEmpty(wN.REMARK))
            {
                whereQ += "\r\n\t" + "AND UPPER(REMARK) LIKE   :REMARK  ";
                wN.QueryParameters.Add("REMARK", "%" + wN.REMARK.Trim().ToUpper() + "%");
            }
            if (!String.IsNullOrEmpty(wN.NOTICE_NO))
            {
                whereQ += "\r\n\t" + "AND UPPER(NOTICE_NO) LIKE   :NOTICE  ";
                wN.QueryParameters.Add("NOTICE", "%" + wN.NOTICE_NO.Trim().ToUpper() + "%");
            }








            return whereQ;
        }

        // Export_WN

        public string Export_WN(ModelWN model)
        {
            model.Query = SearchWL_q;
            model.QueryWhere = SearchWL_whereQ(model);

            return model.Export("Warning Letter Search Report");
        }



        public ValidationResult Validate_RegNo(string propName, W_WL model)
        {
            string regNo = model.REGISTRATION_NO;

            // valid: return null
            // invalid: return new ValidationResult("Registration No. not found.", new List<string> { propName });

            using(EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                var query = db.V_CRM_INFO.Where(s => s.CERTIFICATION_NO.Equals(regNo)).Distinct().ToList();

                if(query != null && query.Count() > 0)
                {
                    return null;
                }
                else
                {
                    return new ValidationResult("Registration No. not found.", new List<string> { propName });
                }
            }

            return null;
        }

        public FileResult DownloadFile(string uuid, string path, string pathName)
        {
            try
            {
                RegistrationCommonService rs = new RegistrationCommonService();
                var supportedTypes = new[] { "pdf", "gif", "jpg" };
                var fileExt = "";
                // fileExt = System.IO.Path.GetExtension(path).Substring(1);
                string tempPath = ApplicationConstant.WLFilePath;

                //tempPath += RegistrationConstant.WARNINGLETTER_PATH;
                string tPath = "";
                using (EntitiesWarningLetter db = new EntitiesWarningLetter())
                {
                    var q = db.W_WL_FILE.Where(x => x.UUID == uuid).Include(x => x.W_WL).FirstOrDefault();
                    tPath = q.FILE_PATH;
                    //tempPath  += getUploadFolderPath(q.W_WL.REGISTRATION_NO);
                    fileExt = System.IO.Path.GetExtension(tPath).Substring(1);
                }
                byte[] fileBytesrs = System.IO.File.ReadAllBytes(tempPath + tPath);
                //rs.DownloadFile(tempPath + tPath);
                //byte[] fileBytesrs = rs.DownloadFile(tempPath + ApplicationConstant.FileSeparator + path);
                //byte[] fileBytesrs = rs.DownloadFile(Server.MapPath(leaveFormPath), path);
                if (fileExt == "gif")
                {
                    return new FileContentResult(fileBytesrs, System.Net.Mime.MediaTypeNames.Image.Gif);
                }
                else if (fileExt == "jpg")
                {
                    return new FileContentResult(fileBytesrs, System.Net.Mime.MediaTypeNames.Image.Jpeg);
                }
                else
                {
                    return new FileContentResult(fileBytesrs, "application/pdf");
                }
            }
            catch(Exception ex)
            {
                return null;
            }
        }

    }
}
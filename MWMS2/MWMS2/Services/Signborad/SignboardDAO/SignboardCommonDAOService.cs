using MWMS2.Areas.Signboard.Models;
using MWMS2.Constant;
using MWMS2.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Services.Signborad.SignboardDAO
{
    public class SignboardCommonDAOService
    {
        private const string getSignboardByAddress_q = ""
                 + "\r\n\t" + "SELECT sb.*,r.reference_no,a.STREET, a.STREET_NO,a.BUILDINGNAME,a.FLOOR,a.FLAT,a.DISTRICT FROM b_sv_signboard sb,                                 "
                 + "\r\n\t" + "( select * from ( select r.* , max(r.created_date) "
                 + "\r\n\t" + "over (partition by r.reference_no)                            "
                 + "\r\n\t" + "as max_created_date from b_sv_record r )                   "
                 + "\r\n\t" + "where created_date = max_created_date ) r, b_sv_address a                                  "
                 + "\r\n\t" + "WHERE sb.uuid = r.sv_signboard_id AND sb.location_address_id = a.uuid                       ";



        private string SearchAddress_whereQ(SignboardAddressSearchModel model)
        {
            string whereQ = "";
            if (!string.IsNullOrWhiteSpace(model.BuildingName))
            {
                whereQ += "\r\n\t" + "AND " + "upper(a.buildingname)" + " LIKE :BN";
                model.QueryParameters.Add("BN", "%" + model.BuildingName.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.Street))
            {
                whereQ += "\r\n\t" + "AND " + "upper(a.street)" + " LIKE :street";
                model.QueryParameters.Add("street", "%" + model.Street.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.StreetNo))
            {
                whereQ += "\r\n\t" + "AND " + "upper(a.street_no)" + " LIKE :sno";
                model.QueryParameters.Add("sno", "%" + model.StreetNo.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.Floor))
            {
                whereQ += "\r\n\t" + "AND " + "upper(a.floor)" + " LIKE :floor";
                model.QueryParameters.Add("floor", "%" + model.Floor.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.Flat))
            {
                whereQ += "\r\n\t" + "AND " + "upper(a.flat)" + " LIKE :Flat";
                model.QueryParameters.Add("Flat", "%" + model.Flat.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.District))
            {
                whereQ += "\r\n\t" + "AND " + "upper(a.district)" + " LIKE :dist";
                model.QueryParameters.Add("dist", "%" + model.District.Trim().ToUpper() + "%");
            }


            return whereQ;
        }

        public DataEntrySearchModel getSignboardLocation(DataEntrySearchModel model)
        {
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                var m_uuid = db.B_S_SYSTEM_TYPE.Where(x => x.TYPE == "SignboardLocationTemplate").First().UUID;

                var query = db.B_S_SYSTEM_VALUE.Where(x => x.SYSTEM_TYPE_ID == m_uuid).OrderBy(y => y.ORDERING);
                model.SignboardTypeList = query.ToList();
            }
         
            return model;
        }
        public SignboardAddressSearchModel getSignboardByAddress(SignboardAddressSearchModel model)
        {

            model.Query = getSignboardByAddress_q;
            model.QueryWhere = SearchAddress_whereQ(model);
            model.Search();
            return model;

        }


        public string GetNumber()
        {
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                var dsnQuery = db.B_SV_REFERENCE_NO.Where(x => x.PREFIX == "d");
                if (dsnQuery.Any())
                {
                    dsnQuery.First().CURRENT_NUMBER += 1;
                    dsnQuery.First().MODIFIED_DATE = DateTime.Now;
                    var DSNNextNumber = dsnQuery.First().CURRENT_NUMBER;
                    db.SaveChanges();
                    return "D" + DSNNextNumber.ToString().PadLeft(10, '0');
                }
                else
                {
                    B_SV_REFERENCE_NO svRefNo = new B_SV_REFERENCE_NO();
                    svRefNo.PREFIX = "d";
                    svRefNo.TYPE = "D";
                    svRefNo.CURRENT_NUMBER = 1;
                    db.SaveChanges();
                    return "D" + svRefNo.CURRENT_NUMBER.ToString().PadLeft(10, '0');

                }
            
            }

         
        }

        public List<string> getSignboardAdminList()
        {
            using(EntitiesSignboard db = new EntitiesSignboard())
            {
                using(EntitiesAuth auth = new EntitiesAuth())
                {
                    List<string> result = new List<string>();

                    List<string> scuUuidList = db.B_S_SCU_TEAM.Select(x => x.SYS_POST_ID).ToList();

                    SYS_ROLE sysRole = auth.SYS_ROLE.Where(x => x.CODE == "ADMIN").FirstOrDefault();
                    List<string> sysPostRoles = auth.SYS_POST_ROLE.Where(x => x.SYS_ROLE_ID == sysRole.UUID).Select(x => x.SYS_POST_ID).ToList();

                    foreach(var sysPostRole in sysPostRoles)
                    {
                        if(scuUuidList.Contains(sysPostRole))
                        {
                            result.Add(sysPostRole);
                        }
                    }
                    return result;
                }
            }

            
        }

        public FileResult getProfessionalImagebycertificationNo(string certNo)
        {
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                var query = db.B_CRM_PBP_PRC.Where(x => x.CERTIFICATION_NO == certNo).FirstOrDefault();
                byte[] fileBytes = new byte[] { };

                if (query != null)
                {
                    string filePath = ApplicationConstant.CRMFilePath + query.FILE_PATH_NONRESTRICTED;
                    fileBytes = getFileByte(filePath);
                    if (File.Exists(filePath))
                    {
                        if (query.FILE_PATH_NONRESTRICTED.Contains("jpg"))
                        {
                            return new FileContentResult(fileBytes, System.Net.Mime.MediaTypeNames.Image.Jpeg);
                        }
                        else if (query.FILE_PATH_NONRESTRICTED.Contains("gif"))
                        {
                            return new FileContentResult(fileBytes, System.Net.Mime.MediaTypeNames.Image.Gif);
                        }
                    }
                }
                return null;
            }
        }
        public byte[] getFileByte(String fullPath)
        {
            try
            {
                return System.IO.File.ReadAllBytes(@fullPath);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public string viewPhotoLibImage(string uuid)
        {
            using(EntitiesSignboard db = new EntitiesSignboard())
            {
                var query = db.B_SV_PHOTO_LIBRARY.Where(x => x.UUID == uuid).FirstOrDefault();
                if(query != null)
                {
                    string urlLink = query.URL;
                    return urlLink;
                }
                return SignboardConstant.PHOTO_LIBRARY_URL;
            }
        }

        public FileResult viewScannedDocumentFile(string uuid)
        {
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                var query = db.B_SV_SCANNED_DOCUMENT.Where(x => x.UUID == uuid).FirstOrDefault();
                if (query != null)
                {
                    string filePath = ApplicationConstant.SMMSCANFilePath + query.RELATIVE_FILE_PATH;
                    byte[] fileBytes = getFileByte(filePath);
                    if (File.Exists(filePath))
                    {
                        if (query.RELATIVE_FILE_PATH.Contains("jpg"))
                        {
                            return new FileContentResult(fileBytes, System.Net.Mime.MediaTypeNames.Image.Jpeg);
                        }
                        else if (query.RELATIVE_FILE_PATH.Contains("gif"))
                        {
                            return new FileContentResult(fileBytes, System.Net.Mime.MediaTypeNames.Image.Gif);
                        }
                        else if (query.RELATIVE_FILE_PATH.Contains("pdf"))
                        {
                            return new FileContentResult(fileBytes, "application/pdf");
                        }
                    }
                }
                return null;
            }
        }

    }
}
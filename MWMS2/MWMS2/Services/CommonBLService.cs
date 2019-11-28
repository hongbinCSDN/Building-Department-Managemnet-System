using MWMS2.Areas.Admin.Models;
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
using System.Data.Entity;
using MWMS2.Controllers;
using MWMS2.Constant;
using System.IO;
using MWMS2.Areas.Registration.Models;

namespace MWMS2.Services
{
    public class CommonBLService
    {
        public byte[] getFileByte(String fullPath)
        {
            try { 
            return System.IO.File.ReadAllBytes(@fullPath);
            }catch(Exception e)
            {   return new byte[] { };
            }
        }

        public FileResult ViewCRMImage(String V_CRM_INFO_UUID)
        {
            EntitiesMWProcessing db = new EntitiesMWProcessing();
            V_CRM_INFO query = db.V_CRM_INFO.Where( a=> a.UUID == V_CRM_INFO_UUID).FirstOrDefault();
            byte[] fileBytes = new byte[] { };
            if (query != null){
                fileBytes = getFileByte(ApplicationConstant.CRMFilePath +
                               query.FILE_PATH_NONRESTRICTED);
            }
            return new FileContentResult(fileBytes, System.Net.Mime.MediaTypeNames.Image.Jpeg);
        }
        public FileResult ViewCRMImageByUUID(String UUID)
        {
            int du;
            if (int.TryParse(UUID, out du))
            {
                List<CertificateDisplayListModel> draft = SessionUtil.DraftList<CertificateDisplayListModel>(ApplicationConstant.DRAFT_KEY_CERTIFICATE);

             //   var temp = draft.Where(x => x.UUID == UUID).FirstOrDefault();
                for (int i = 0; i < draft.Count(); i++)
                {
                    using (MemoryStream file = new MemoryStream())
                    {
                        //if (draft[i].UploadDocStream[0] != null)
                        if (draft[i].UploadDocStream.Count() != 0 && draft[i].UploadDocStream[0] != null)
                        {
                          
                            return new FileContentResult(draft[i].UploadDocStream[0], System.Net.Mime.MediaTypeNames.Image.Jpeg);

                        }
                    }

                }
           
                   
                //temp.Applicant_File
       


            }
            else
            {
                using (EntitiesRegistration db = new EntitiesRegistration())
                {
                    var query = db.C_IND_CERTIFICATE.Where(a => a.UUID == UUID).FirstOrDefault();
                    byte[] fileBytes = new byte[] { };
                    if (query != null)
                    {
                        string filePath = ApplicationConstant.CRMFilePath + query.FILE_PATH_NONRESTRICTED;
                        fileBytes = getFileByte(filePath);
                        if(File.Exists(filePath))
                        {
                            if (query.FILE_PATH_NONRESTRICTED.Contains("jpg"))
                            {
                                return new FileContentResult(fileBytes, System.Net.Mime.MediaTypeNames.Image.Jpeg);
                            }
                            else if(query.FILE_PATH_NONRESTRICTED.Contains("gif"))
                            {
                                return new FileContentResult(fileBytes, System.Net.Mime.MediaTypeNames.Image.Gif);
                            }
                            else
                            {
                                return new FileContentResult(fileBytes, "application/pdf");
                            }
                        }
                    }
                    return null;
                }
         

            }
            return null;
        }

        public FileResult ViewCompCRMImageByUUID(string uuid)
        {
            int du ;
            if (int.TryParse(uuid,out du))
            {
                List<C_COMP_APPLICANT_INFO> draftApplicant = SessionUtil.DraftList<C_COMP_APPLICANT_INFO>(ApplicationConstant.DRAFT_KEY_APPLICANTS);

               var temp = draftApplicant.Where(x => x.UUID == uuid).FirstOrDefault();
                //temp.Applicant_File
                using (MemoryStream file = new MemoryStream())
                {
                    if (temp.Applicant_File != null)
                    {
                        temp.Applicant_File.InputStream.CopyTo(file);
                        return new FileContentResult(file.ToArray(), System.Net.Mime.MediaTypeNames.Image.Jpeg);

                    }
                }
             

            }
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                var query = db.C_COMP_APPLICANT_INFO.Where(a => a.UUID == uuid).FirstOrDefault();
                byte[] fileBytes = new byte[] { };
                if (query != null)
                {
                    string filePath = ApplicationConstant.CRMFilePath + query.FILE_PATH_NONRESTRICTED;
                    if(File.Exists(filePath))
                    {
                        fileBytes = getFileByte(filePath);
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
                    //new FileContentResult(new byte[0], System.Net.Mime.MediaTypeNames.Image.Jpeg);

            }


            // C_COMP_APPLICANT_INFO
        }


        public FileResult ViewSMMSDImageByUUID(String UUID)
        {
                using (EntitiesSignboard db = new EntitiesSignboard())
                {
                    var query = db.B_SV_SCANNED_DOCUMENT.Where(a => a.UUID == UUID).FirstOrDefault();
                    byte[] fileBytes = new byte[] { };
                    if (query != null)
                    {
                        fileBytes = getFileByte(query.FILE_PATH);
                    if (query.FILE_PATH.Contains("jpg"))
                    {
                        return new FileContentResult(fileBytes, System.Net.Mime.MediaTypeNames.Image.Jpeg);

                    }
                    else if (query.FILE_PATH.Contains("gif"))
                    {
                        return new FileContentResult(fileBytes, System.Net.Mime.MediaTypeNames.Image.Gif);

                    }
                    else if (query.FILE_PATH.Contains(".docx"))
                    {
                     
                         return new FileContentResult(fileBytes, "application/msword") { FileDownloadName="temp.docx"};
                  
                      
                    }
                    else
                    {
                        return new FileContentResult(fileBytes, "application/pdf");

                    }
                    }
                }


            
            return new FileContentResult(new byte[0], System.Net.Mime.MediaTypeNames.Image.Jpeg);





        }
    }
}
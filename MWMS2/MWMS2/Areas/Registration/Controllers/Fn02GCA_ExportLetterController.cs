using MWMS2.Models;
using MWMS2.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Areas.Registration.Models;
using MWMS2.Constant;
using MWMS2.Entity;
using System.Text;
using System.IO;

namespace MWMS2.Areas.Registration.Controllers
{
    public class Fn02GCA_ExportLetterController : Controller
    {

        public virtual string registrationType()
        {
            return RegistrationConstant.REGISTRATION_TYPE_CGA;
        }

        public virtual ActionResult Index()
        {
            Fn02GCA_ExportLetterSearchModel model = new Fn02GCA_ExportLetterSearchModel();
            model.RegType = RegistrationConstant.REGISTRATION_TYPE_CGA;
            return View("SearchExportLetter", model);
        }

        public ActionResult checkFileExist(Fn02GCA_ExportLetterSearchModel model)
        {
            LetterTemplateService lts = new LetterTemplateService();
            RegistrationExportLetterService els = new RegistrationExportLetterService();
            model = els.getLetterByUUID(model);

            string filePath = model.FilePath;
            string fileName = model.FileName;

            var isExist = lts.checkFileExist(filePath + fileName);
            if (isExist)
            {
                return Json(new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS });
            }
            else
            {
                return Json(new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = new List<string>() { "File not found." } });
            }
        }

        public FileStreamResult ExportTemplate(Fn02GCA_ExportLetterSearchModel dataExport)
        {
            try
            {
                BaseCommonService rs = new BaseCommonService();
                RegistrationExportLetterService els = new RegistrationExportLetterService();
                dataExport = els.getLetterByUUID(dataExport);

                string filePath = dataExport.FilePath;
                string fileName = dataExport.FileName;

                return rs.ExportFile(filePath, fileName);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Object"))
                {
                    Response.Write("Please Select letter No.");
                }
                else
                Response.Write(ex.Message);
            }

            return null;
        }

        public ActionResult CheckFileRef(Fn02GCA_ExportLetterSearchModel model)
        {
            model.RegType = registrationType();
            RegistrationExportLetterService rs = new RegistrationExportLetterService();
            return Content(JsonConvert.SerializeObject(rs.CheckFileRef(model), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }


        // Export Comp certificate
        public FileStreamResult ExportLetterFunc(Fn02GCA_ExportLetterSearchModel model ,String FileRef, String RegType)
        {
            
            String SelectAsUuid = model.AS;
            String SelectTdUuid = model.TD;//model.TD;
            String SelectAuthUuid = model.AuthName;
            String SelectPrbUuid = model.PRB;
            String SelectCommitteeUuid = model.Committee;
            String SelectCertUuid = model.Category;
            String SelectIvCandUuid = model.DIA;
            String ExportLetterUUID = model.selectedLetterUuid.Trim();
            RegistrationExportLetterService rs = new RegistrationExportLetterService();
            String certificateContent = rs.PopulateExportLetter(FileRef, RegType, ExportLetterUUID, SelectAsUuid, SelectTdUuid
                , SelectPrbUuid, SelectCommitteeUuid, SelectCertUuid, SelectIvCandUuid, SelectAuthUuid);
          
            var byteArray = Encoding.UTF8.GetBytes(certificateContent);
            var stream = new MemoryStream(byteArray);
            return File(stream, "text/plain", "tmp.txt");
        }

        public ActionResult SearchCompany(Fn02GCA_ExportLetterSearchModel model)
        {
            model.RegType = registrationType();
            String FileRef = model.FileRef;
            RegistrationExportLetterService rs = new RegistrationExportLetterService();

            rs.LookUpSelection(model);
            //C_S_EXPORT_LETTER exportLetter = rc.getExportLetterByUuid(model.selectedLetterUuid);
            // C_COMP_APPLICATION cCompAppli = rc.getCompApplicationByFileRef(FileRef);
            return Content(JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }



    }
}
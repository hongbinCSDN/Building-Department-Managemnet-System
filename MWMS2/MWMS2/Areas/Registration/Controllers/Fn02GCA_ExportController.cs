using MWMS2.Models;
using MWMS2.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Services;
using MWMS2.Areas.Registration.Models;
using MWMS2.Constant;

namespace MWMS2.Areas.Registration.Controllers
{
    public class Fn02GCA_ExportController : Controller
    {
        // GET: Registration/Fn02GCA_PM
        public ActionResult Index()
        {
            return View("IndexExport");
        }

        public virtual ActionResult IndexExportApplicationData()
        {
            CRMDataExportModel dataExport = new CRMDataExportModel();
            dataExport.RegType = registrationType();
            return View("IndexExport_Comp", dataExport);

        }

        public virtual string registrationType(){
            return RegistrationConstant.REGISTRATION_TYPE_CGA;
        }
        private string getTitle(string type)
        {
            if (type.Equals(RegistrationConstant.REGISTRATION_TYPE_CGA))
            {
                return "&nbsp;General Contractor Application &gt; Export Data";
            }
            else if (type.Equals(RegistrationConstant.REGISTRATION_TYPE_MWCA))
            {
                return "&nbsp;MW Company Application &gt; Export Data";
            }
            else if (type.Equals(RegistrationConstant.REGISTRATION_TYPE_IP))
            {
                return "&nbsp;Professional Application &gt; Export Data";
            }
            else if (type.Equals(RegistrationConstant.REGISTRATION_TYPE_MWIA))
            {
                return "&nbsp;MW Individual Application  &gt; Export Data";
            }
            else
            {
                return "";
            }
        }


        private string getMenuLink(String type)
        {
            if (type.Equals(RegistrationConstant.REGISTRATION_TYPE_CGA))
            {
                return "menu.do?method=LoadGCAIndexPage";
            }
            else if (type.Equals(RegistrationConstant.REGISTRATION_TYPE_MWCA))
            {
                return "menu.do?method=LoadMWCIndexPage";
            }
            else if (type.Equals(RegistrationConstant.REGISTRATION_TYPE_IP))
            {
                return "menu.do?method=LoadPAIndexPage";
            }
            else if (type.Equals(RegistrationConstant.REGISTRATION_TYPE_MWIA))
            {
                return "menu.do?method=LoadMWIIndexPage";
            }
            else
            {
                return "";
            }
        }

        public FileStreamResult ExportALLRegistrationData(ExportDataForm exportDataForm)
        {
            exportDataForm.SearchTitle = getTitle(registrationType());
            exportDataForm.MenuLink = getMenuLink(registrationType());
            exportDataForm.RegisterType = registrationType();
            exportDataForm.OutputType = "EXCEL";
            ExportDataManager exportDataManager = new ExportDataManager();
           return exportDataManager.exportRegistersData(Request, Response, exportDataForm);
            /*if (exportDataForm.getOutputType().Equals("PDF"))
            {
                //exportDataManager.exportRegistersDataPDF(request, response, exportDataForm);
            }
            else
            {
                if ("EXCEL".Equals(exportDataForm.getOutputType()))
                {
                    exportDataManager.exportRegistersData(Request, Response, exportDataForm);
                }
                else
                {
                    exportDataManager.exportRegistersData(Request, Response, exportDataForm);
                    exportDataForm.setRegisterType("QP");
                    exportDataManager.exportRegistersData(Request, Response, exportDataForm);
                }

            }*/


        }

        public FileStreamResult ExportRegistrationData()
        {
            RegistrationDataExportService registrationCommonService = new RegistrationDataExportService();
            return registrationCommonService.exportExcelRegistrationData();

        }
        public virtual FileStreamResult ExportApplicationData(CRMDataExportModel dataExport)
        {
            RegistrationDataExportService registrationCommonService = new RegistrationDataExportService();
            dataExport.RegType = registrationType();
            return registrationCommonService.exportCompApplicationData(dataExport);
        }

        public FileStreamResult ExportQPExcelData(CRMDataExportModel dataExport)
        {
            RegistrationDataExportService registrationCommonService = new RegistrationDataExportService();
            dataExport.RegType = registrationType();
            return registrationCommonService.exportQPExcelData();

        }

        public FileStreamResult ExportQPCSVData(CRMDataExportModel dataExport)
        {
            RegistrationDataExportService registrationCommonService = new RegistrationDataExportService();
            dataExport.RegType = registrationType();
            return registrationCommonService.exportQPCSVData();

        }
    }
}
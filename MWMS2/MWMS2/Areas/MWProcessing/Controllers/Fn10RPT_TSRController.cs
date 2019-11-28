using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Areas.Registration.Controllers;
using MWMS2.Constant;
using MWMS2.Services;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.MWProcessing.Controllers
{
    public class Fn10RPT_TSRController : ValidationController
    {
        //ProcessingTSRBLService
        private ProcessingTSRBLService BLService;
        protected ProcessingTSRBLService BL
        {
            get { return BLService ?? (BLService = new ProcessingTSRBLService()); }
        }

        // GET: MWProcessing/Fn10RPT_TSR
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Generate()
        {
            Fn10RPT_TSRGenerateModel model = new Fn10RPT_TSRGenerateModel();
            model.TypeOfForms = new List<TypeOfForm>();
            foreach (var item in ProcessingConstant.validMWFormNos)
            {
                if (item == ProcessingConstant.FORM_MW06_01 || item == ProcessingConstant.FORM_MW06_02 || item == ProcessingConstant.FORM_MW06_03)
                    continue;
                if (item == ProcessingConstant.FORM_MW01)
                {
                    TypeOfForm form = new TypeOfForm()
                    {
                        Form = item
    ,
                        Aug = "7"
    ,
                        Total = "7"
                    };
                    model.TypeOfForms.Add(form);
                }
                else if (item == ProcessingConstant.FORM_MW02 || item == ProcessingConstant.FORM_MW03 || item == ProcessingConstant.FORM_MW04 || item == ProcessingConstant.FORM_MW06)
                {
                    TypeOfForm form = new TypeOfForm()
                    {
                        Form = item
    ,
                        Aug = "0"
    ,
                        Total = "0"
                    };
                    model.TypeOfForms.Add(form);
                }
                else
                {
                    TypeOfForm form = new TypeOfForm()
                    {
                        Form = item
                        ,
                        Aug = "1"
                        ,
                        Total = "1"
                    };
                    model.TypeOfForms.Add(form);
                }
                if (item == ProcessingConstant.FORM_MW05)
                {
                    TypeOfForm form = new TypeOfForm()
                    {
                        Form = item + "(item 3.6)"
                    ,
                        Aug = "0"
                    ,
                        Total = "0"
                    };
                    model.TypeOfForms.Add(form);
                }
            }
            return View(model);
        }

        public ActionResult Print()
        {
            string excelPath = Server.MapPath("~/Template/Report_TypeOfForm/Type_of_Submission_Record.xls");
            MemoryStream ms = new MemoryStream();
            using (FileStream fs = System.IO.File.OpenRead(excelPath))
            {
                IWorkbook workbook = new HSSFWorkbook(fs);

                workbook.Write(ms);
            }
            byte[] fileContent = ms.ToArray();
            return File(fileContent, "application/x-xls", "Export.xls");
        }
    }
}
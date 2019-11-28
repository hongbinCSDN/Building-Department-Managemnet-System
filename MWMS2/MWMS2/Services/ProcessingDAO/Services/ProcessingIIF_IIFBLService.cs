using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Models;
using MWMS2.Services.ProcessingDAO.DAO;
using System.Linq;
using MWMS2.Entity;
using System.Collections.Generic;
using MWMS2.Utility;
using System;

namespace MWMS2.Services.ProcessingDAO.Services
{
    public class ProcessingIIF_IIFBLService
    {
        private ProcessingIIF_IIFDAOService _DA;
        protected ProcessingIIF_IIFDAOService DA
        {
            get
            {
                return _DA ?? (_DA = new ProcessingIIF_IIFDAOService());
            }
        }

        public Fn05IIF_IIFModel Search(Fn05IIF_IIFModel model)
        {
            DA.Search(model);
            return model;
        }

        public ServiceResult ImportExcel(Fn05IIF_IIFModel model)
        {
            try
            {
                var package = model.ExcelPackage;
                var currentSheet = package.Workbook.Worksheets;
                var workSheet = currentSheet.First();

                int startRow = 2;

                var noOfCol = workSheet.Dimension.End.Column;
                var noOfRow = workSheet.Dimension.End.Row;

                // set data to Master Table: P_IMPORT_36
                DA.AddImport36(model);

                // set data to Master Table: P_IMPORT_36_ITEM
                model.Import36ItemList = new List<P_IMPORT_36_ITEM>();
                for (int rowIterator = startRow; rowIterator <= noOfRow; rowIterator++)
                {
                    P_IMPORT_36_ITEM pImport36Item = new P_IMPORT_36_ITEM();

                    pImport36Item.P_IMPORT_36_UUID = model.Import36.UUID;
                    pImport36Item.BLOCK_ID = CommonUtil.getDisplay(workSheet.Cells[rowIterator, 1].Value);
                    pImport36Item.UNIT_ID = CommonUtil.getDisplay(workSheet.Cells[rowIterator, 2].Value);
                    pImport36Item.NATURE = CommonUtil.getDisplay(workSheet.Cells[rowIterator, 3].Value);
                    pImport36Item.RECEIVED_DATE = CommonUtil.getDisplay(workSheet.Cells[rowIterator, 4].Value);
                    pImport36Item.FORM_TYPE = CommonUtil.getDisplay(workSheet.Cells[rowIterator, 5].Value);
                    pImport36Item.MW_NO = CommonUtil.getDisplay(workSheet.Cells[rowIterator, 6].Value);
                    pImport36Item.PBP = CommonUtil.getDisplay(workSheet.Cells[rowIterator, 7].Value);
                    pImport36Item.PRC = CommonUtil.getDisplay(workSheet.Cells[rowIterator, 8].Value);
                    pImport36Item.WORK_LOCATION = CommonUtil.getDisplay(workSheet.Cells[rowIterator, 9].Value);
                    pImport36Item.COMM_DATE = CommonUtil.getDisplay(workSheet.Cells[rowIterator, 10].Value);
                    pImport36Item.COMP_DATE = CommonUtil.getDisplay(workSheet.Cells[rowIterator, 11].Value);
                    pImport36Item.PAW = CommonUtil.getDisplay(workSheet.Cells[rowIterator, 12].Value);
                    pImport36Item.PAW_CONTACT = CommonUtil.getDisplay(workSheet.Cells[rowIterator, 13].Value);
                    pImport36Item.LETTER_DATE = CommonUtil.getDisplay(workSheet.Cells[rowIterator, 14].Value);
                    pImport36Item.UMW_NOTICE_NO = CommonUtil.getDisplay(workSheet.Cells[rowIterator, 15].Value);
                    pImport36Item.BD_REF = CommonUtil.getDisplay(workSheet.Cells[rowIterator, 16].Value);
                    pImport36Item.V_SUBMISSION_CASE_NO = CommonUtil.getDisplay(workSheet.Cells[rowIterator, 17].Value);
                    pImport36Item.STATUTORY_NOTICE_NO = CommonUtil.getDisplay(workSheet.Cells[rowIterator, 18].Value);

                    model.Import36ItemList.Add(pImport36Item);
                }
                return DA.AddImport36Item(model);
            }
            catch(Exception ex)
            {
                return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { "Please upload the file as template file format." } };
            }
           
        }

        public string ExportExcel(Fn05IIF_IIFModel model)
        {
            model.Query = @"Select * from P_IMPORT_36_ITEM Where P_IMPORT_36_UUID = :P_IMPORT_36_UUID";
            model.QueryParameters.Add("P_IMPORT_36_UUID", model.UUID);
            model.Columns = new List<Dictionary<string, string>>()
              .Append(new Dictionary<string, string> { ["columnName"] = "BLOCK_ID", ["displayName"] = "BLOCK ID" })
              .Append(new Dictionary<string, string> { ["columnName"] = "UNIT_ID", ["displayName"] = "UNIT ID" })
              .Append(new Dictionary<string, string> { ["columnName"] = "NATURE", ["displayName"] = "NATURE" })
              .Append(new Dictionary<string, string> { ["columnName"] = "RECEIVED_DATE", ["displayName"] = "RECEIVED DATE" })
              .Append(new Dictionary<string, string> { ["columnName"] = "FORM_TYPE", ["displayName"] = "FORM TYPE" })
              .Append(new Dictionary<string, string> { ["columnName"] = "MW_NO", ["displayName"] = "MW NO." })
              .Append(new Dictionary<string, string> { ["columnName"] = "PBP", ["displayName"] = "PBP" })
              .Append(new Dictionary<string, string> { ["columnName"] = "PRC", ["displayName"] = "PRC" })
              .Append(new Dictionary<string, string> { ["columnName"] = "WORK_LOCATION", ["displayName"] = "WORK LOCATION" })
              .Append(new Dictionary<string, string> { ["columnName"] = "COMM_DATE", ["displayName"] = "Commencement Date" })
              .Append(new Dictionary<string, string> { ["columnName"] = "COMP_DATE", ["displayName"] = "Completion Date" })
              .Append(new Dictionary<string, string> { ["columnName"] = "PAW", ["displayName"] = "PAW" })
              .Append(new Dictionary<string, string> { ["columnName"] = "PAW_CONTACT", ["displayName"] = "PAW CONTACT" })
              .Append(new Dictionary<string, string> { ["columnName"] = "LETTER_DATE", ["displayName"] = "LETTER DATE" })
              .Append(new Dictionary<string, string> { ["columnName"] = "UMW_NOTICE_NO", ["displayName"] = "UMW NOTICE NO" })
              .Append(new Dictionary<string, string> { ["columnName"] = "BD_REF", ["displayName"] = "BD_REF" })
              .Append(new Dictionary<string, string> { ["columnName"] = "V_SUBMISSION_CASE_NO", ["displayName"] = "V SUBMISSION CASE NO" })
              .Append(new Dictionary<string, string> { ["columnName"] = "STATUTORY_NOTICE_NO", ["displayName"] = "STATUTORY NOTICE NO" })
              .ToArray();
            return model.Export(model.FileName);
        }
    }
}
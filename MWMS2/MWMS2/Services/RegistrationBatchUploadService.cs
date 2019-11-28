using MWMS2.Entity;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace MWMS2.Services
{
    public class RegistrationBatchUploadService
    {
        public List<List<string>> ReadExcel(string path )
        {
            ISheet sheet;
            IWorkbook workbook = null;
            //HSSFWorkbook hssfwb;
            using (FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                workbook = WorkbookFactory.Create(file);          //Abre tanto XLS como XLSX
                sheet = workbook.GetSheetAt(0);
                // hssfwb = new HSSFWorkbook(file);
                // sheet = hssfwb.GetSheetAt(0);
            }
     
  
        
            List<List<string>> rows = new List<List<string>>();
            for (int row = 0; row <= sheet.LastRowNum; row++) 
            {
                if (sheet.GetRow(row) != null) //null is when the row only contains empty cells 
                {
                      
                    List<string> eachRow = new List<string>();
                   
                    foreach (var item in sheet.GetRow(row).Cells)
                    {
                        eachRow.Add(item.StringCellValue);                        
                    }
                    rows.Add(eachRow);
                     //  MessageBox.Show(string.Format("Row {0} = {1}", row, sheet.GetRow(row).GetCell(0).StringCellValue));
                }
            }
            return rows;
            //BatchUploadSave(rows);

        }
        public void SaveBatchUpload(string path)
        {
            try {
                List<List<string>> rows = ReadExcel(path);
                using (EntitiesRegistration db = new EntitiesRegistration())
                {
                    foreach (var item in rows)
                    {
                        string refNo = item[0];
                        var query = db.C_IND_APPLICATION.Where(x => x.FILE_REFERENCE_NO == refNo).FirstOrDefault();//Excel col 1 find record by reference_no
                        if (query != null)
                        {
                            query.WILLINGNESS_QP = item[1]; // Excel col 2  Interested in Providing Services of QP

                            query.INTERESTED_FSS = item[2]; // Excel col 3 Interested in Providing Services in Fire Safety

                            query.SERVICE_IN_MWIS = item[3];//Excel col 4 Had provided related services under the MWIS

                        }


                    }

                    db.SaveChanges();

                }

            }
            catch (Exception ex)
            {

            }
   

        }
        public void SaveBatchUploadHistory(string fileName , string path)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                C_BATCH_UPLOAD_QP_EXPERIENCE bu = new C_BATCH_UPLOAD_QP_EXPERIENCE();
                bu.FILE_NAME = fileName;
                bu.FILE_PATH = path;
                db.C_BATCH_UPLOAD_QP_EXPERIENCE.Add(bu);
                db.SaveChanges();
            }
        }
       


    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;
using OfficeOpenXml;

namespace MWMS2ScanTools
{
    public partial class ControlBatch : UserControl
    {
        public ControlBatch()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();
            List<ImageFile> imageFiles = new List<ImageFile>();
            if (result == DialogResult.OK)
            {
                for (int i = 0; i < openFileDialog1.FileNames.Length; i++)
                {
                    string filename = openFileDialog1.FileNames[i];
                    ReadExcel(filename);
                }
            }
        }
        private void ReadSheet(string sheetName, ExcelWorksheet sheet)
        {
            if (sheet == null || sheet.Dimension == null) return;
            int startRowNumber = sheet.Dimension.Start.Row;
            int endRowNumber = sheet.Dimension.End.Row;
            int startColumn = sheet.Dimension.Start.Column;
            int endColumn = sheet.Dimension.End.Column;
            List<List<object>> data = new List<List<object>>();
            for (int i = startRowNumber; i < endRowNumber + 1; i++)
            {
                List<object> rc = new List<object>();
                for (int j = startColumn; j < endColumn + 1; j++)
                {
                    rc.Add(sheet.Cells[i, j].Value);
                }
                data.Add(rc);
            }
            TabPage tp = new TabPage(sheetName);
            ControlBatchSheet controlBatchSheet = new ControlBatchSheet(tabControl1, tp, sheetName, data);
            
            controlBatchSheet.Dock = DockStyle.Fill;
            tp.Controls.Add(controlBatchSheet);
            tabControl1.TabPages.Add(tp);
            //tabControl1.SelectedTab = tp;



        }
        private void ReadExcel(string filename)
        {
            using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                try
                {
                    using (ExcelPackage ep = new ExcelPackage(fs))
                    {
                        ExcelWorkbook wb = ep.Workbook;
                        if (wb == null) return;
                        for (int i = 0; i < wb.Worksheets.Count; i++)
                        {
                            ReadSheet(wb.Worksheets[i + 1].Name, wb.Worksheets[i + 1]);
                        }
                        wb.Dispose();
                    }
                }
                catch (Exception) { MessageBox.Show("Invalid Excel File"); }
            }
        }
    }
}

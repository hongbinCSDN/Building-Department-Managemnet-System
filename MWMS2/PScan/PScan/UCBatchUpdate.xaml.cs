using Microsoft.Win32;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PScan
{
    /// <summary>
    /// UCBatchUpdate.xaml 的互動邏輯
    /// </summary>
    public partial class UCBatchUpdate : UserControl
    {


        public UCBatchUpdate()
        {
            InitializeComponent();
        }

        private void ButtonFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog { Multiselect = false };
            List<string> files = new List<string>();

            if (openFileDialog.ShowDialog() == true)
            {
                for (int i = 0; i < openFileDialog.FileNames.Length; i++)
                {
                    string filename = openFileDialog.FileNames[i];
                    ReadExcel(filename);
                }
            }
        }
        private void ReadExcel(string filename)
        {
            using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                try
                {
                    using (ExcelPackage ep = new ExcelPackage(fs))
                    {
                        ExcelWorkbook wb = ep.Workbook;
                        if (wb == null) return;
                        for (int i = 0; i < wb.Worksheets.Count; i++)
                        {
                            TabItem tab = new TabItem();
                            tab.Header = wb.Worksheets[i + 1].Name;
                            TabControlSheets.Items.Add(tab);
                            UCBatchUpdateSheet sheet = new UCBatchUpdateSheet(TabControlSheets, tab, wb.Worksheets[i + 1], filename, wb.Worksheets[i + 1].Name);
                            tab.Content = sheet;
                        }
                        wb.Dispose();
                    }
                }
                catch (Exception) { System.Windows.MessageBox.Show("Invalid Excel File"); }
                fs.Close();
                fs.Dispose();
            }
        }

    }
}

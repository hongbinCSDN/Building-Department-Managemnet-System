using Microsoft.Win32;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// Interaction logic for UserControlSearchResult.xaml
    /// </summary>
    public partial class UCSearchResult : UserControl
    {
        TabControl _tabControl;
        TabItem _tab;
        List<DRMS_DOCUMENT_META_DATA> _DRMS_DOCUMENT_META_DATAs;
        public UCSearchResult(TabControl tabControl, TabItem tab, List<DRMS_DOCUMENT_META_DATA> DRMS_DOCUMENT_META_DATAs)
        {
            _tabControl = tabControl;
            _tab = tab;
            _DRMS_DOCUMENT_META_DATAs = DRMS_DOCUMENT_META_DATAs;
            InitializeComponent();

            GridView gridView = ListViewResult.View as GridView;
            gridView.Columns.Clear();
            if (DRMS_DOCUMENT_META_DATAs.Count > 0)
            {
                Type t = DRMS_DOCUMENT_META_DATAs[0].GetType();
                PropertyInfo[] PropertyInfos = t.GetProperties();
                foreach (PropertyInfo propertyInfo in  PropertyInfos)
                {
                    string header = "";
                    if (propertyInfo.Name == "MW_SUBMISSION_NUMBER") header = "Submission No.";
                    if (propertyInfo.Name == "DSN") header = "DSN";
                    if (propertyInfo.Name == "FORM")  header = "Form";
                    if (propertyInfo.Name == "DOCUMENT_TYPE") header = "Document Type";
                    //if (propertyInfo.Name != "MW_ITEM_NUMBER")  header = "Submission No.";
                    if (propertyInfo.Name == "BD_FILE_REF")  header = "BD File Reference";
                    if (propertyInfo.Name == "TYPE_OF_CORRESPONDENCE")  header = "Type of Correspondence";
                    if (propertyInfo.Name == "FILE_CLASSIFICATION")  header = "File Classification";
                    if (propertyInfo.Name == "DOCUMENT_CONTROL_STAGE")  header = "Document Control Stage";
                    if (header == "") continue;
                    gridView.Columns.Add(new GridViewColumn() { Header = header, DisplayMemberBinding = new Binding(propertyInfo.Name) });
                }
                ListViewResult.ItemsSource = DRMS_DOCUMENT_META_DATAs;
            }
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender == null) return;
            DRMS_DOCUMENT_META_DATA data = (sender as ListViewItem).Content as DRMS_DOCUMENT_META_DATA;
            WDoc _wDoc = MainService.Instance.OpenWDoc();
            P_MW_DSN dsn = MainService.Instance.LoadDocInfo(data.DSN);
            _wDoc.AddDoc(dsn);
        }

        private void ButtonExport_Click(object sender, RoutedEventArgs e)
        {
            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet ws = package.Workbook.Worksheets.Add("Uploaded Document Data");

                Type t = typeof(DRMS_DOCUMENT_META_DATA);
                for (int i = 0; i < t.GetProperties().Length; i++)
                {
                    ws.Cells[1, i + 1].Value = t.GetProperties()[i].Name;
                }

                for (int i = 0; i < t.GetProperties().Length; i++)
                {
                    for (int j = 0; j < _DRMS_DOCUMENT_META_DATAs.Count; j++)
                    {
                        object value = _DRMS_DOCUMENT_META_DATAs[j].GetType().GetProperty(t.GetProperties()[i].Name).GetValue(_DRMS_DOCUMENT_META_DATAs[j], null);
                        ws.Cells[j+2, i+1].Value = value == null ? "" : value;
                    }
                }

                ws.Cells.AutoFitColumns();

                byte[] bin = package.GetAsByteArray();

                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Title = "Save Excel sheet";
                saveFileDialog1.Filter = "Excel files|*.xlsx|All files|*.*";
                saveFileDialog1.FileName = "ExcelSheet_" + DateTime.Now.ToString("dd-MM-yyyy") + ".xlsx";

                bool? rs = saveFileDialog1.ShowDialog();
                //check if user clicked the save button
                if (rs != null && rs.Value == true)
                {
                    File.WriteAllBytes(saveFileDialog1.FileName, bin);
                }

            }
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            _tabControl.Items.Remove(_tab);
        }
    }
}
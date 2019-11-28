using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    /// UCBatchUpdateSheet.xaml 的互動邏輯
    /// </summary>
    public partial class UCBatchUpdateSheet : UserControl
    {
        private TabControl _tabControlSheets;
        private TabItem _tab;
        private List<List<object>> _data;
        private List<DRMS_DOCUMENT_META_DATA> _dataSource;
        private string _filename;
        private string _sheetname;
        public UCBatchUpdateSheet(TabControl tabControlSheets, TabItem tab, ExcelWorksheet sheet, string filename, string sheetname)
        {//List<List<object>> data
            _tabControlSheets = tabControlSheets;
            _tab = tab;
            _data = ReadSheet(sheet) ;
            InitializeComponent();
            RenderData();
            _tab.Focus();
            _filename = filename;
            _sheetname = sheetname;
        }


        private List<List<object>> ReadSheet(ExcelWorksheet sheet)
        {
            //if (sheet == null || sheet.Dimension == null) return;
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
            return data;
        }


        private void RenderData()
        {
            if (_data == null) return;
            if (_data.Count == 0) return;
            if (_data[0].Count == 0) return;
           // _DRMS_DOCUMENT_META_DATAs = DRMS_DOCUMENT_META_DATAs;
          //  InitializeComponent();

            GridView gridView = ListViewSheet.View as GridView;
            gridView.Columns.Clear();
            if (_data[0].Count > 0)
            {

                for(int i= 0;i < _data[0].Count; i++)
                {
                    gridView.Columns.Add(new GridViewColumn() { Header = _data[0][i], DisplayMemberBinding = new Binding(_data[0][i] as string) });
                }

                _dataSource = new List<DRMS_DOCUMENT_META_DATA>();

                for (int i = 1; i < _data.Count; i++)
                {
                    
                    DRMS_DOCUMENT_META_DATA row = new DRMS_DOCUMENT_META_DATA();
                    for (int j = 0; j < _data[i].Count; j++)
                    {
                        try
                        {
                            PropertyInfo prop = row.GetType().GetProperty(_data[0][j] as string, BindingFlags.Public | BindingFlags.Instance);
                            if (null != prop && prop.CanWrite)
                            {
                                prop.SetValue(row, _data[i][j]+"", null);
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                    _dataSource.Add(row);
                }
                ListViewSheet.ItemsSource = _dataSource;
            }
            

        }
        private void ButtonUpdate_Click(object sender, RoutedEventArgs e)
        {
            MainService.Instance.BatchUpdate(_filename, _sheetname);
            MessageBox.Show("Batch Update Complete.");
            _tabControlSheets.Items.Remove(_tab);
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            _tabControlSheets.Items.Remove(_tab);
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }
    }
}

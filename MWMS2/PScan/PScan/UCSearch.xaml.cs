using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for UserControlBatch.xaml
    /// </summary>
    public partial class UCSearch : UserControl
    {
        private int _tabItemCount = 0;


        public UCSearch()
        {
            InitializeComponent();
        }
        public void CloseSearchResultTab(TabItem tabItem)
        {
            TabControlSearchResults.Items.Remove(tabItem);
        }
        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<DRMS_DOCUMENT_META_DATA> DRMS_DOCUMENT_META_DATAs = MainService.Instance.SearchDoc(null, TextBoxStage.Text, TextBoxDsn.Text, DatePickerDocumentDateFrom.SelectedDate, DatePickerDocumentDateTo.SelectedDate);

                TabItem newTab = new TabItem();
                newTab.Header = "Result " + ++_tabItemCount;
                UCSearchResult ucSearchResult = new UCSearchResult(TabControlSearchResults, newTab, DRMS_DOCUMENT_META_DATAs);
                newTab.Content = ucSearchResult;
                TabControlSearchResults.Items.Add(newTab);
                newTab.Focus();
            }
            catch(Exception ex)
            {

            }
        }
    }
}
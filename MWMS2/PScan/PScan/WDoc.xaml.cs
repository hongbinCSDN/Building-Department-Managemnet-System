using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace PScan
{
    /// <summary>
    /// Interaction logic for WDoc.xaml
    /// </summary>
    public partial class WDoc : Window
    {
        private P_MW_DSN _mwDsn { get; set; }
        public WDoc()
        {
            InitializeComponent();
        }
        private TabItem GetDocumentTab(string dsn)
        {
            for(int i= 0;i < TabControlDocs.Items.Count; i++)
            {
                TabItem temp = TabControlDocs.Items[i] as TabItem;
                if (temp.Header as string == dsn) return temp;
            }
            return null;
        }
        public void RemoveDoc(string dsn)
        {
           for(int i = 0;i < TabControlDocs.Items.Count; i++)
            {
                TabItem tab = TabControlDocs.Items[i] as TabItem;
                MessageBox.Show("Upload Success");
                if (tab != null && tab.Header as string == dsn) TabControlDocs.Items.Remove(tab);
            }
        }
        public TabItem AddDoc(P_MW_DSN mwDsn)
        {
            if (mwDsn == null) return null;
            _mwDsn = mwDsn;
            TabItem tab = null;

            for (int i = 0; i < TabControlDocs.Items.Count; i++)
            {
                TabItem temp = TabControlDocs.Items[i] as TabItem;
                if(temp.Header as string == mwDsn.DSN)
                {
                    tab = temp;
                    break;
                }
            }
            if (tab == null)
            {
                tab = new TabItem() { Header = mwDsn.DSN };
                TabControlDocs.Items.Add(tab);


                /*TabItem newTab = new TabItem();
                newTab.Header = "Result " + ++_tabItemCount;
                UCSearchResult ucSearchResult = new UCSearchResult(DRMS_DOCUMENT_META_DATAs);
                newTab.Content = ucSearchResult;
                TabControlSearchResults.Items.Add(newTab);
                newTab.Focus();*/
            }

            mwDsn.Related = MainService.Instance.LoadDocRelated(mwDsn.DSN);
            if (mwDsn.Related != null && mwDsn.Related.AUDIT_RELATED == "Y")
            {
                MessageBox.Show("Please break the document for this audit case.");
            }
            UCDocument newDoc = new UCDocument(TabControlDocs, tab, mwDsn);
            tab.Content = newDoc;

            TabControlDocs.SelectedItem = tab;
            tab.Focus();
            return tab;
        }
    }
}

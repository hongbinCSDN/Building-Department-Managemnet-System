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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PScan
{
    /// <summary>
    /// Interaction logic for UCSubDocument.xaml
    /// </summary>
    public partial class UCSubDocument : UserControl
    {
        private decimal? _pageCount { get; set; }
        private string _filePath = null;
        private Dictionary<string, string> defaultSubDsn = new Dictionary<string, string>()
        {
            ["Form"] = "A",
            ["Photo"] = "B",
            ["Plan"] = "C",
            ["Other"] = "D"

           ,


            ["All"] = "",
            ["Amended Plan"] = "C",
            ["Amended Form"] = "A",
            ["Amended Photo"] = "B",
            ["Letter"] = "D",
            ["SSP"] = "D",
            ["Other"] = "D"
            
        };


        private UCDocument _ucDocument; private TabItem _tab;
        public UCSubDocument(UCDocument ucDocument, TabItem tab, P_MW_SCANNED_DOCUMENT doc, bool showImage = true)
        {
            _ucDocument = ucDocument;
            _tab = tab;
            InitializeComponent();
            if (!showImage) WebBrowserView.Visibility = Visibility.Hidden;
            //AcrobatViewer AcrobatViewer;
            if (!string.IsNullOrWhiteSpace(doc.FILE_PATH))
            {
                _filePath = doc.FILE_PATH;
                _pageCount = doc.PAGE_COUNT;
                WebBrowserView.Source = new Uri(doc.FILE_PATH);
            }
            ComboBoxDocumentType.Items.Add("All");
            ComboBoxDocumentType.Items.Add("Plan");
            //ComboBoxDocumentType.Items.Add("Amended Plan");
            ComboBoxDocumentType.Items.Add("Form");
            //ComboBoxDocumentType.Items.Add("Amended Form");
            ComboBoxDocumentType.Items.Add("Photo");
            //ComboBoxDocumentType.Items.Add("Amended Photo");
            ComboBoxDocumentType.Items.Add("Letter");
            ComboBoxDocumentType.Items.Add("SSP");
            ComboBoxDocumentType.Items.Add("Other");
            ComboBoxDocumentType.SelectedIndex = 0;



            ComboBoxSubDsn.Items.Add("");
            for(int i= 0;i< 26; i++)
            {
                ComboBoxSubDsn.Items.Add(((char)(i + 65)).ToString());
            }

            if (!string.IsNullOrWhiteSpace(doc.DOCUMENT_TYPE)) ComboBoxDocumentType.Text = doc.DOCUMENT_TYPE;
            if (!string.IsNullOrWhiteSpace(doc.FOLDER_TYPE)) CheckBoxPublic.IsChecked = doc.FOLDER_TYPE == "Public";
            if (!string.IsNullOrWhiteSpace(doc.DSN_SUB))
            {
                if (doc.DSN_SUB.Length == 12) ComboBoxSubDsn.Text = doc.DSN_SUB[doc.DSN_SUB.Length - 1] +"";
            }
        }




        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            _ucDocument.RemoveSubDocument(_tab);
        }

        public void SetDocType(string docType)
        {
            for(int i = 0;i < ComboBoxDocumentType.Items.Count; i++)
            {
                if(ComboBoxDocumentType.Items[i] as string == docType)
                {
                    ComboBoxDocumentType.SelectedIndex = i;
                    break;
                }
            }
            for (int i = 0; i < ComboBoxDocumentType.Items.Count; i++)
            {
                if (ComboBoxDocumentType.Items[i] as string == "Other")
                {
                    ComboBoxDocumentType.SelectedItem = ComboBoxDocumentType.Items[i];
                    return;
                }
            }
        }

        private void ComboBoxDocumentType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _tab.Header = ComboBoxDocumentType.SelectedItem as string;
            if (defaultSubDsn.ContainsKey(ComboBoxDocumentType.SelectedItem as string))
            {
                ComboBoxSubDsn.Text = defaultSubDsn[ComboBoxDocumentType.SelectedItem as string];
                return;
            }
           
        }

        private void ComboBoxSubDsn_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(ComboBoxSubDsn.Text.Length > 1)
            {
                ComboBoxSubDsn.SelectedIndex = 0;
            }
        }

        private void ComboBoxSubDsn_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ComboBoxSubDsn.Text.Length > 1)
            {
                ComboBoxSubDsn.SelectedIndex = 0;
            }
        }
        public string FilePath { get { return _filePath; } }

        public P_MW_SCANNED_DOCUMENT Form2Data(string docTitle, string dsn)
        {
            P_MW_SCANNED_DOCUMENT d = new P_MW_SCANNED_DOCUMENT();
            d.FOLDER_TYPE = CheckBoxPublic.IsChecked != null && CheckBoxPublic.IsChecked .Value == true? "Public" : "Private";
            d.DOC_TITLE = docTitle;
            d.DOCUMENT_TYPE = ComboBoxDocumentType.Text;
            d.DSN_SUB = dsn + ComboBoxSubDsn.Text;
            d.PAGE_COUNT = _pageCount;
            return d;
        }
    }

}

using Microsoft.Win32;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
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
using TwainDotNet;

namespace PScan
{
    /// <summary>
    /// Interaction logic for UCScan.xaml
    /// </summary>
    public partial class UCScan : UserControl
    {
        private bool _ucLoaded = false;
        private int _scanTime { get; set; } = 1;
        private int _scanDocNo { get; set; } = 0;
        Twain _twain = null;
        private int itemWidth = 150;
        public int ItemColumns { get; set; } = 6;
        Dictionary<string, ListViewFileItem> _listViewFileSource = new Dictionary<string, ListViewFileItem>();
        public UCScan()
        {
            InitializeComponent();
            Application.Current.MainWindow.SizeChanged += MainWindow_SizeChanged;
            ListViewFiles.ItemsSource = _listViewFileSource;
            RefreshUI();
            Loaded += delegate { InitTwain(); };
           

        }

        private void _twain_ScanningComplete(object sender, ScanningCompleteEventArgs e)
        {
            if (e.Exception != null) Scan((Twain)sender);
            else ButtonScan.IsEnabled = true;
        }

        private void _twain_TransferImage(object sender, TransferImageEventArgs args)
        {
            string fileName = "Scan." + _scanTime + "." + ++_scanDocNo + ".jpg";
            if (args.Image != null)
            {
                Bitmap resultImage = args.Image;
                MemoryStream ms = new MemoryStream();
                resultImage.Save(ms, ImageFormat.Jpeg);
                string savedFilePath = MainService.Instance.SaveTempFile(fileName, ms);
                AddImage(savedFilePath);
            }
        }

        private void Scan(Twain _twain)
        {
            if (_twain == null) return;
            _scanTime++;
            _scanDocNo = 0;
            var sourceList = _twain.SourceNames;
            ScanSettings settings = new ScanSettings()
            {
                ShowTwainUI = true,
                ShouldTransferAllPages = true,
                Resolution = ResolutionSettings.Photocopier,
                UseDuplex = true,
                Page = PageSettings.Default
                ,
                UseDocumentFeeder = true
                ,
                UseAutoScanCache = true
                ,
                AbortWhenNoPaperDetectable = false
                ,
                ShowProgressIndicatorUI = true
                ,
                UseAutoFeeder = true
                //, TransferCount = 5

            };
            _twain.SelectSource(ComboBoxScanner.Text);
            try
            {
                _twain.StartScanning(settings);
            }
            catch (FeederEmptyException)
            {
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResetColumnItems();
        }
        private void ResetColumnItems()
        {
            var temp = ((int)(ListViewFiles.Width - 10 / itemWidth)) + 1;
            if (ItemColumns != temp)
            {
                ItemColumns = temp;
            }
        }
        private void InitTwain()
        {
            if (_ucLoaded == false)
            {
                _ucLoaded = true;
                _twain = new Twain(new TwainDotNet.Wpf.WpfWindowMessageHook(MainService.Instance.MainWindow));
                _twain.TransferImage += _twain_TransferImage;
                _twain.ScanningComplete += _twain_ScanningComplete;
                if (_twain != null)
                {
                    IList<string> sourceList = _twain.SourceNames;
                    for (int i = 0; i < sourceList.Count; i++)
                    {
                        ComboBoxScanner.Items.Add(sourceList[i]);
                        if (sourceList[i].IndexOf("TWAIN") >= 0) ComboBoxScanner.SelectedIndex = ComboBoxScanner.Items.Count - 1;
                    }
                }
            }
        }
       

        private void ButtonImport_Click(object sender, RoutedEventArgs e)
        {
            //ListViewFile.View = ListViewFile.View.l  View.LargeIcon;
            OpenFileDialog openFileDialog = new OpenFileDialog { Multiselect = true };
            List<ImageFile> imageFiles = new List<ImageFile>();

            if (openFileDialog.ShowDialog() == true)
            {
                for (int i = 0; i < openFileDialog.FileNames.Length; i++)
                {
                    string filename = openFileDialog.FileNames[i];
                    AddImage(filename);
                }
            }
            RefreshUI();

        }
        private void AddImage(string filename)
        {
            try
            {
                _listViewFileSource.Add(filename, new ListViewFileItem() { ImagePath = filename });
                RefreshUI();
            }
            catch (Exception ex)
            {

            }
        }

        private void RefreshUI()
        {
            CollectionViewSource.GetDefaultView(_listViewFileSource).Refresh();
            LabelImageCount.Content = _listViewFileSource.Count;
        }
        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            foreach (KeyValuePair<string, ListViewFileItem> item in ListViewFiles.SelectedItems)
            {
                _listViewFileSource.Remove(item.Key);
            }
            RefreshUI();
            //ListViewFiles.ItemsSource = _listViewFileSource;
        }

        private void ButtonMerge_Click(object sender, RoutedEventArgs e)
        {
            if (ListViewFiles.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select image(s).");
                return;
            }
            WAskDsn wAskDsn = new WAskDsn();
            if (wAskDsn.ShowDialog() == true)
            {
                List<string> files = new List<string>();
                for (int i = 0; i < ListViewFiles.SelectedItems.Count; i++)
                {
                    KeyValuePair<string, ListViewFileItem>? item = ListViewFiles.SelectedItems[i] as KeyValuePair<string, ListViewFileItem>?;
                    if (item != null) files.Add(item.Value.Key);
                }
                P_MW_DSN mwDsn = null;
                if (wAskDsn.MwDsn != null)
                {
                    mwDsn = wAskDsn.MwDsn;
                }
                else if (wAskDsn.DSN != null && !string.IsNullOrWhiteSpace(wAskDsn.DSN))
                {
                    mwDsn = MainService.Instance.LoadDocInfo(wAskDsn.DSN);
                    if (mwDsn != null)
                    {
                        mwDsn.Related = MainService.Instance.LoadDocRelated(wAskDsn.DSN);
                        if (mwDsn.Related != null && mwDsn.Related.AUDIT_RELATED == "Y")
                        {
                            MessageBox.Show("Please break the document for this audit case.");
                        }
                    }
                }
                if (mwDsn == null) mwDsn = new P_MW_DSN() { DSN = wAskDsn.DSN };

                PdfDocument myDoc = new PdfDocument();
                myDoc.Info.Title = "Document by MWMS2 Scanning Tools";
                myDoc.Info.Author = "MWMS2 Scanning Tools";
                foreach (KeyValuePair<string, ListViewFileItem> kvItem in ListViewFiles.SelectedItems)
                {
                    if (Util.isJPG(kvItem.Key))
                    {
                        PdfPage myPage = myDoc.AddPage();
                        XGraphics g = XGraphics.FromPdfPage(myPage);
                        XImage img = XImage.FromFile(kvItem.Key);
                        g.DrawImage(img, new XRect(0, 0, myPage.Width - 0, myPage.Height - 0));
                        img.Dispose();
                        g.Dispose();
                        myPage.Close();
                    }
                    else if (Util.isPDF(kvItem.Key))
                    {
                        PdfDocument doc = PdfReader.Open(kvItem.Key, PdfDocumentOpenMode.Import);
                        foreach (PdfPage page in doc.Pages)
                        {
                            myDoc.AddPage(page);
                        }
                        doc.Close();
                    }
                }
               
                int pageSize = myDoc.PageCount;
                string tempName = System.IO.Path.Combine(System.IO.Path.GetTempPath(), System.IO.Path.GetRandomFileName()) + ".pdf";
                using (FileStream fs = new FileStream(tempName, FileMode.OpenOrCreate))
                {
                    myDoc.Save(fs);
                    myDoc.Close();
                    myDoc.Dispose();
                    //memoryStream.CopyTo(fs);
                    fs.Flush();
                }

                if (mwDsn != null)
                {
                    WDoc _wDoc = MainService.Instance.OpenWDoc();
                    TabItem newTab = _wDoc.AddDoc(mwDsn);

                    UCDocument newDoc = newTab.Content as UCDocument;

                    P_MW_SCANNED_DOCUMENT subDocData = new P_MW_SCANNED_DOCUMENT();
                    subDocData.FILE_PATH = tempName;
                    newDoc.AddSubDocument(subDocData, true);
                    _wDoc.Show();
                    newDoc.ComboBoxDocumentType_SelectionChanged(null, null);
                }
            }
            //ListViewFiles.SelectedItems
        }

        private void ButtonScan_Click(object sender, RoutedEventArgs e)
        {

            if (_twain == null)
            {
                MessageBox.Show("No scanner connected.");
                return;
            }
            else if (ComboBoxScanner.Text == "")
            {
                MessageBox.Show("Please select scanner.");
                ComboBoxScanner.Focus();
                //ComboBoxScanner.DroppedDown = true;
                return;
            }
            ButtonScan.IsEnabled = false;
            Scan(_twain);
        }
    }
}
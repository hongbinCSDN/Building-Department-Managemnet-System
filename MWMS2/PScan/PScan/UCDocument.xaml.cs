using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
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

namespace PScan
{
    /// <summary>
    /// Interaction logic for UCDocument.xaml
    /// </summary>
    public partial class UCDocument : UserControl
    {
        public P_MW_DSN MwDsn { get; set; }
        public DRMS_DOCUMENT_META_DATA DrmsData { get; set; }


        public void SetDefault()
        {
            DatePickerSubmissionLetterDate.SelectedDate = DateTime.Now;
            //TextBoxSelectedForMwAuditCheck.
            //fSELECTED_FOR_MW_AUDIT_CHECK.Items.Add("N");
            //fSELECTED_FOR_MW_AUDIT_CHECK.Items.Add("Y");

            ComboBoxBacklogUpload.Items.Add("N");
            ComboBoxBacklogUpload.Items.Add("Y");
            ComboBoxBacklogUpload.SelectedIndex = 0;

            ComboBoxFileClassification.Items.Add("Confidential");
            ComboBoxFileClassification.Items.Add("Restricted");
            ComboBoxFileClassification.Items.Add("Secret");
            ComboBoxFileClassification.Items.Add("Top Secret");
            ComboBoxFileClassification.Items.Add("Unclassified");
            ComboBoxFileClassification.SelectedIndex = 4;

            //fDISPOSAL_OF_RECORD_STATUS.Items.Add("Hardcopy disposed");
            //fDISPOSAL_OF_RECORD_STATUS.Items.Add("Not yet disposed");

            ComboBoxDocumentType.Items.Add("Form");
            ComboBoxDocumentType.Items.Add("Letter");
            ComboBoxDocumentType.Items.Add("Memo");
            ComboBoxDocumentType.Items.Add("Others");
            ComboBoxDocumentType.Items.Add("Photo");
            ComboBoxDocumentType.Items.Add("Plan");
            ComboBoxDocumentType.Items.Add("Safety Inspection Report");
            ComboBoxDocumentType.Items.Add("Structural Calculation");
            ComboBoxDocumentType.Items.Add("Supervision Plan");
            ComboBoxDocumentType.SelectedIndex = 0;

            ComboBoxForm.Items.Add("BD106");
            ComboBoxForm.Items.Add("MW01");
            ComboBoxForm.Items.Add("MW02");
            ComboBoxForm.Items.Add("MW03");
            ComboBoxForm.Items.Add("MW04");
            ComboBoxForm.Items.Add("MW05");
            ComboBoxForm.Items.Add("MW06");
            ComboBoxForm.Items.Add("MW07");
            ComboBoxForm.Items.Add("MW08");
            ComboBoxForm.Items.Add("MW09");
            ComboBoxForm.Items.Add("MW10");
            ComboBoxForm.Items.Add("MW11");
            ComboBoxForm.Items.Add("MW12");
            ComboBoxForm.Items.Add("MW31");
            ComboBoxForm.Items.Add("MW32");
            ComboBoxForm.Items.Add("MW33");
            ComboBoxForm.Items.Add("PNRC72");
            ComboBoxForm.Items.Add("SC01");
            ComboBoxForm.Items.Add("SC01C");
            ComboBoxForm.Items.Add("SC02");
            ComboBoxForm.Items.Add("SC02C");
            ComboBoxForm.Items.Add("SC03");
            ComboBoxForm.Items.Add("SC03C");

            ComboBoxMwItemNo.Items.Add("1.1");
            ComboBoxMwItemNo.Items.Add("1.10");
            ComboBoxMwItemNo.Items.Add("1.11");
            ComboBoxMwItemNo.Items.Add("1.12");
            ComboBoxMwItemNo.Items.Add("1.13");
            ComboBoxMwItemNo.Items.Add("1.14");
            ComboBoxMwItemNo.Items.Add("1.15");
            ComboBoxMwItemNo.Items.Add("1.16");
            ComboBoxMwItemNo.Items.Add("1.17");
            ComboBoxMwItemNo.Items.Add("1.18");
            ComboBoxMwItemNo.Items.Add("1.19");
            ComboBoxMwItemNo.Items.Add("1.2");
            ComboBoxMwItemNo.Items.Add("1.20");
            ComboBoxMwItemNo.Items.Add("1.21");
            ComboBoxMwItemNo.Items.Add("1.22");
            ComboBoxMwItemNo.Items.Add("1.23");
            ComboBoxMwItemNo.Items.Add("1.24");
            ComboBoxMwItemNo.Items.Add("1.25");
            ComboBoxMwItemNo.Items.Add("1.26");
            ComboBoxMwItemNo.Items.Add("1.27");
            ComboBoxMwItemNo.Items.Add("1.28");
            ComboBoxMwItemNo.Items.Add("1.29");
            ComboBoxMwItemNo.Items.Add("1.3");
            ComboBoxMwItemNo.Items.Add("1.30");
            ComboBoxMwItemNo.Items.Add("1.31");
            ComboBoxMwItemNo.Items.Add("1.32");
            ComboBoxMwItemNo.Items.Add("1.33");
            ComboBoxMwItemNo.Items.Add("1.34");
            ComboBoxMwItemNo.Items.Add("1.35");
            ComboBoxMwItemNo.Items.Add("1.36");
            ComboBoxMwItemNo.Items.Add("1.37");
            ComboBoxMwItemNo.Items.Add("1.38");
            ComboBoxMwItemNo.Items.Add("1.39");
            ComboBoxMwItemNo.Items.Add("1.4");
            ComboBoxMwItemNo.Items.Add("1.40");
            ComboBoxMwItemNo.Items.Add("1.41");
            ComboBoxMwItemNo.Items.Add("1.42");
            ComboBoxMwItemNo.Items.Add("1.43");
            ComboBoxMwItemNo.Items.Add("1.44");
            ComboBoxMwItemNo.Items.Add("1.5");
            ComboBoxMwItemNo.Items.Add("1.6");
            ComboBoxMwItemNo.Items.Add("1.7");
            ComboBoxMwItemNo.Items.Add("1.8");
            ComboBoxMwItemNo.Items.Add("1.9");
            ComboBoxMwItemNo.Items.Add("2.1");
            ComboBoxMwItemNo.Items.Add("2.10");
            ComboBoxMwItemNo.Items.Add("2.11");
            ComboBoxMwItemNo.Items.Add("2.12");
            ComboBoxMwItemNo.Items.Add("2.13");
            ComboBoxMwItemNo.Items.Add("2.14");
            ComboBoxMwItemNo.Items.Add("2.15");
            ComboBoxMwItemNo.Items.Add("2.16");
            ComboBoxMwItemNo.Items.Add("2.17");
            ComboBoxMwItemNo.Items.Add("2.18");
            ComboBoxMwItemNo.Items.Add("2.19");
            ComboBoxMwItemNo.Items.Add("2.2");
            ComboBoxMwItemNo.Items.Add("2.20");
            ComboBoxMwItemNo.Items.Add("2.21");
            ComboBoxMwItemNo.Items.Add("2.22");
            ComboBoxMwItemNo.Items.Add("2.23");
            ComboBoxMwItemNo.Items.Add("2.24");
            ComboBoxMwItemNo.Items.Add("2.25");
            ComboBoxMwItemNo.Items.Add("2.26");
            ComboBoxMwItemNo.Items.Add("2.27");
            ComboBoxMwItemNo.Items.Add("2.28");
            ComboBoxMwItemNo.Items.Add("2.29");
            ComboBoxMwItemNo.Items.Add("2.3");
            ComboBoxMwItemNo.Items.Add("2.30");
            ComboBoxMwItemNo.Items.Add("2.31");
            ComboBoxMwItemNo.Items.Add("2.32");
            ComboBoxMwItemNo.Items.Add("2.33");
            ComboBoxMwItemNo.Items.Add("2.34");
            ComboBoxMwItemNo.Items.Add("2.35");
            ComboBoxMwItemNo.Items.Add("2.36");
            ComboBoxMwItemNo.Items.Add("2.37");
            ComboBoxMwItemNo.Items.Add("2.38");
            ComboBoxMwItemNo.Items.Add("2.39");
            ComboBoxMwItemNo.Items.Add("2.4");
            ComboBoxMwItemNo.Items.Add("2.40");
            ComboBoxMwItemNo.Items.Add("2.5");
            ComboBoxMwItemNo.Items.Add("2.6");
            ComboBoxMwItemNo.Items.Add("2.7");
            ComboBoxMwItemNo.Items.Add("2.8");
            ComboBoxMwItemNo.Items.Add("2.9");
            ComboBoxMwItemNo.Items.Add("3.1");
            ComboBoxMwItemNo.Items.Add("3.10");
            ComboBoxMwItemNo.Items.Add("3.11");
            ComboBoxMwItemNo.Items.Add("3.12");
            ComboBoxMwItemNo.Items.Add("3.13");
            ComboBoxMwItemNo.Items.Add("3.14");
            ComboBoxMwItemNo.Items.Add("3.15");
            ComboBoxMwItemNo.Items.Add("3.16");
            ComboBoxMwItemNo.Items.Add("3.17");
            ComboBoxMwItemNo.Items.Add("3.18");
            ComboBoxMwItemNo.Items.Add("3.19");
            ComboBoxMwItemNo.Items.Add("3.2");
            ComboBoxMwItemNo.Items.Add("3.20");
            ComboBoxMwItemNo.Items.Add("3.21");
            ComboBoxMwItemNo.Items.Add("3.22");
            ComboBoxMwItemNo.Items.Add("3.23");
            ComboBoxMwItemNo.Items.Add("3.24");
            ComboBoxMwItemNo.Items.Add("3.25");
            ComboBoxMwItemNo.Items.Add("3.26");
            ComboBoxMwItemNo.Items.Add("3.27");
            ComboBoxMwItemNo.Items.Add("3.28");
            ComboBoxMwItemNo.Items.Add("3.29");
            ComboBoxMwItemNo.Items.Add("3.3");
            ComboBoxMwItemNo.Items.Add("3.30");
            ComboBoxMwItemNo.Items.Add("3.31");
            ComboBoxMwItemNo.Items.Add("3.32");
            ComboBoxMwItemNo.Items.Add("3.33");
            ComboBoxMwItemNo.Items.Add("3.34");
            ComboBoxMwItemNo.Items.Add("3.35");
            ComboBoxMwItemNo.Items.Add("3.36");
            ComboBoxMwItemNo.Items.Add("3.37");
            ComboBoxMwItemNo.Items.Add("3.38");
            ComboBoxMwItemNo.Items.Add("3.39");
            ComboBoxMwItemNo.Items.Add("3.4");
            ComboBoxMwItemNo.Items.Add("3.40");
            ComboBoxMwItemNo.Items.Add("3.41");
            ComboBoxMwItemNo.Items.Add("3.42");
            ComboBoxMwItemNo.Items.Add("3.5");
            ComboBoxMwItemNo.Items.Add("3.6");
            ComboBoxMwItemNo.Items.Add("3.7");
            ComboBoxMwItemNo.Items.Add("3.8");
            ComboBoxMwItemNo.Items.Add("3.9");

            //ComboBoxMwItemNo.Items.Add("1(a)-3");
            //ComboBoxMwItemNo.Items.Add("1(b)-3");
            //ComboBoxMwItemNo.Items.Add("1(c)-3");
            //ComboBoxMwItemNo.Items.Add("1.1");
            //ComboBoxMwItemNo.Items.Add("1.10");
            //ComboBoxMwItemNo.Items.Add("1.11");
            //ComboBoxMwItemNo.Items.Add("1.12");
            //ComboBoxMwItemNo.Items.Add("1.13");
            //ComboBoxMwItemNo.Items.Add("1.14");
            //ComboBoxMwItemNo.Items.Add("1.15");
            //ComboBoxMwItemNo.Items.Add("1.16");
            //ComboBoxMwItemNo.Items.Add("1.17");
            //ComboBoxMwItemNo.Items.Add("1.18");
            //ComboBoxMwItemNo.Items.Add("1.19");
            //ComboBoxMwItemNo.Items.Add("1.2");
            //ComboBoxMwItemNo.Items.Add("1.20");
            //ComboBoxMwItemNo.Items.Add("1.21");
            //ComboBoxMwItemNo.Items.Add("1.22");
            //ComboBoxMwItemNo.Items.Add("1.23");
            //ComboBoxMwItemNo.Items.Add("1.24");
            //ComboBoxMwItemNo.Items.Add("1.25");
            //ComboBoxMwItemNo.Items.Add("1.26");
            //ComboBoxMwItemNo.Items.Add("1.27");
            //ComboBoxMwItemNo.Items.Add("1.28");
            //ComboBoxMwItemNo.Items.Add("1.29");
            //ComboBoxMwItemNo.Items.Add("1.3");
            //ComboBoxMwItemNo.Items.Add("1.30");
            //ComboBoxMwItemNo.Items.Add("1.31");
            //ComboBoxMwItemNo.Items.Add("1.32");
            //ComboBoxMwItemNo.Items.Add("1.33");
            //ComboBoxMwItemNo.Items.Add("1.34");
            //ComboBoxMwItemNo.Items.Add("1.35");
            //ComboBoxMwItemNo.Items.Add("1.36");
            //ComboBoxMwItemNo.Items.Add("1.37");
            //ComboBoxMwItemNo.Items.Add("1.38");
            //ComboBoxMwItemNo.Items.Add("1.39");


            ComboBoxOriginOfRecord.Items.Add("Electronic");
            ComboBoxOriginOfRecord.Items.Add("Paper");
            ComboBoxOriginOfRecord.SelectedIndex = 1;

            //fPROVISIONS_MODIFIED_EXEMPTED.Items.Add("B(P)R 30");
            //fPROVISIONS_MODIFIED_EXEMPTED.Items.Add("B(P)R 31");
            //fPROVISIONS_MODIFIED_EXEMPTED.Items.Add("B(P)R 35A");
            //fPROVISIONS_MODIFIED_EXEMPTED.Items.Add("B(P)R 36");
            //fPROVISIONS_MODIFIED_EXEMPTED.Items.Add("B(P)R 45");
            //fPROVISIONS_MODIFIED_EXEMPTED.Items.Add("B(SSFPDWL)R 29(1)&(2), 44(4) & 50(2)");

            //fREASON_OF_DISPOSAL.Items.Add("Record has been digitised and retention period lapsed");

            //fSCAN_QUALITY_CHECK_STATUS.Items.Add("Acceptable");
            //fSCAN_QUALITY_CHECK_STATUS.Items.Add("Acceptable after re-scan");
            //fSCAN_QUALITY_CHECK_STATUS.Items.Add("Not acceptable");
            //fSCAN_QUALITY_CHECK_STATUS.Items.Add("Not selected for scan quality check");
            ComboBoxTypeOfCorrespondence.Items.Add("Incoming");
            ComboBoxTypeOfCorrespondence.Items.Add("Outgoning");
            ComboBoxTypeOfCorrespondence.SelectedIndex = 0;
            TextBoxLocationOfRecord.Text = "MWU";
            TextBoxLocationOfRecord.IsEnabled = false;
            if (MainService.Instance.HIST_DocumentType != null)
            {
                ComboBoxDocumentType.Text = MainService.Instance.HIST_DocumentType;
            }
            if (MainService.Instance.HIST_LastSubmission != null)
            {
                DatePickerSubmissionLetterDate.SelectedDate = MainService.Instance.HIST_LastSubmission;
            }
        }
        public void SetRecordData()
        {
            TextBoxDsn.Text = MwDsn.DSN;
            TextBoxMwSubmissionNo.Text = MwDsn.RECORD_ID;
            ComboBoxForm.Text = MwDsn.FORM_CODE;
            if(MwDsn.Related != null)
            {
                CheckBoxAuditCase.IsChecked = MwDsn.Related.AUDIT_RELATED == "Y";
                CheckBoxOrderRelated.IsChecked = MwDsn.Related.ORDER_RELATED == "Y";
                CheckBoxSignboardRelated.IsChecked = MwDsn.Related.SIGNBOARD_RELATED == "Y";
                CheckBoxSSPIncluded.IsChecked = MwDsn.Related.SSP == "Y";
            }
            List<DRMS_DOCUMENT_META_DATA> subDocData = MainService.Instance.SearchDoc(null, null, MwDsn.DSN, null, null);
            int maxVersion = 0;
            if (subDocData != null)
            {
                for (int i = 0; i < subDocData.Count; i++)
                {
                    try
                    {
                        int ver = int.Parse(subDocData[i].DSN_VERSION_NUMBER);
                        if (ver >= maxVersion)
                        {
                            maxVersion = ver;
                            DrmsData = subDocData[i];
                        }
                    }
                    catch { }
                }
            }
            if (DrmsData == null && subDocData != null && subDocData.Count > 0) DrmsData = subDocData[0];
            if (DrmsData != null)
            {


                this.ComboBoxBacklogUpload.Text = DrmsData.BACKLOG_UPLOAD;
                this.ComboBoxDocumentType.Text = DrmsData.DOCUMENT_TYPE;
                this.ComboBoxFileClassification.Text = DrmsData.FILE_CLASSIFICATION;
                this.ComboBoxForm.Text = DrmsData.FORM;
                //this.ComboBoxMwItemNo.Text = DrmsData.MW_ITEM_NUMBER;
                this.ComboBoxOriginOfRecord.Text = DrmsData.ORIGIN_OF_RECORD;
                this.ComboBoxTypeOfCorrespondence.Text = DrmsData.TYPE_OF_CORRESPONDENCE;

                this.TextBoxBdFileRef.Text = DrmsData.BD_FILE_REF;
                this.TextBoxDocumentControlStage.Text = DrmsData.DOCUMENT_CONTROL_STAGE;
                this.TextBoxDsn.Text = DrmsData.DSN;
                this.TextBoxEffsSubmissionNo.Text = DrmsData.EFSS_SUBMISSION_REF_NO;
                this.TextBoxLocationOfRecord.Text = DrmsData.LOCATION_OF_RECORD;
                this.TextBoxMwSubmissionNo.Text = DrmsData.MW_SUBMISSION_NUMBER;
                this.TextBoxSelectedForMwAuditCheck.Text = DrmsData.SELECTED_FOR_MW_AUDIT_CHECK;
                this.TextBoxWorkLocation.Text = DrmsData.WORKS_LOCATION;
                if(!string.IsNullOrWhiteSpace(DrmsData.SUBMISSION_LETTER_DATE))
                {
                    DateTime d = DateTime.ParseExact(DrmsData.SUBMISSION_LETTER_DATE, "yyyy-MM-dd", CultureInfo.CurrentCulture);
                    this.DatePickerSubmissionLetterDate.SelectedDate = d;
                }
                if (!string.IsNullOrWhiteSpace(DrmsData.MW_ITEM_NUMBER))
                {
                    string[] items = DrmsData.MW_ITEM_NUMBER.Split(';');
                    for(int i= 0;i < items.Length; i++)
                    {
                        this.ComboBoxMwItemNo.SelectedItems.Add(items[i]);

                    }
                }

                //this.DatePickerSubmissionLetterDate = DrmsData.SUBMISSION_LETTER_DATE;
                List<P_MW_SCANNED_DOCUMENT> subDoc = MainService.Instance.LoadSubDocs(MwDsn.DSN);
                for (int i = 0; i < subDoc.Count; i++)
                {//subDoc

                    byte[] bs = Convert.FromBase64String(subDoc[i].FILE_PATH);
                    string tempName = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName()+".pdf");
                    System.IO.File.WriteAllBytes(tempName, bs);
                    subDoc[i].FILE_PATH = tempName;
                    AddSubDocument(subDoc[i], DrmsData.FILE_CLASSIFICATION != "Confidential");
                }
                //ButtonUpload.IsEnabled = false;
            }
        }
        private void LoadMataData()
        {
            if (DrmsData == null) return;
            //DatePickerSubmissionLetterDate.SelectedDate = drmsData.SUBMISSION_LETTER_DATE;
            TextBoxMwSubmissionNo.Text = DrmsData.MW_SUBMISSION_NUMBER;
            //TextBoxDsn.Text = DrmsData.DSN;
            ComboBoxForm.Text = DrmsData.DSN;
            //selectedITEMNo
            //docType
            TextBoxBdFileRef.Text = DrmsData.BD_FILE_REF;
            TextBoxWorkLocation.Text = DrmsData.WORKS_LOCATION;
            ComboBoxFileClassification.Text = DrmsData.FILE_CLASSIFICATION;
            ComboBoxOriginOfRecord.Text = DrmsData.ORIGIN_OF_RECORD;
            ComboBoxTypeOfCorrespondence.Text = DrmsData.TYPE_OF_CORRESPONDENCE;
            TextBoxLocationOfRecord.Text = DrmsData.LOCATION_OF_RECORD;
            TextBoxSelectedForMwAuditCheck.Text = DrmsData.SELECTED_FOR_MW_AUDIT_CHECK;
            ComboBoxBacklogUpload.Text = DrmsData.BACKLOG_UPLOAD;
            TextBoxEffsSubmissionNo.Text = DrmsData.EFSS_SUBMISSION_REF_NO;




        }

        private TabControl _tabControl { get; set; }
        private TabItem _tab { get; set; }
        public UCDocument(TabControl tabControl, TabItem tab, P_MW_DSN data)
        {
            _tabControl = tabControl;
            _tab = tab;
            MwDsn = data;
            InitializeComponent();
            SetDefault();
            SetRecordData();
        }
        public void AddSubDocument(P_MW_SCANNED_DOCUMENT data, bool showImage)
        {
            TabItem tab = new TabItem();
            UCSubDocument ucSubDocument = new UCSubDocument(this, tab, data, showImage);
            tab.Content = ucSubDocument;
            TabControlSubDocument.Items.Add(tab);
            tab.Focus();
            
        }
        public void RemoveSubDocument(TabItem tab)
        {
            TabControlSubDocument.Items.Remove(tab);
            if(TabControlSubDocument.Items.Count <= 0)
            {
                _tabControl.Items.Remove(_tab);
            }
        }

        private void ButtonUpload_Click(object sender, RoutedEventArgs e)
        {
            if (TextBoxDsn.Text == "") { MessageBox.Show("Plesae input DSN."); return; }
            //if (ComboBoxMwItemNo.SelectedItems.Count == 0) { MessageBox.Show("Plesae input MW Items."); return; }
            //if (ComboBoxForm.Text == "") { MessageBox.Show("Plesae input form."); return; }
            if (ComboBoxDocumentType.Text == "") { MessageBox.Show("Plesae input document type."); return; }
            if (DatePickerSubmissionLetterDate.SelectedDate == null) { MessageBox.Show("Plesae input submission/ letter Date."); return; }
            Dictionary<string, string> curType = new Dictionary<string, string>();
            for (int i = 0; i < TabControlSubDocument.Items.Count; i++)
            {
                TabItem tab = TabControlSubDocument.Items[i] as TabItem;
                if (curType.ContainsKey(tab.Header as string))
                {
                    MessageBox.Show("Duplicate sub-document type : " + tab.Header as string);
                    return;
                }
                curType.Add(tab.Header as string, "");
            }
            ButtonUpload.IsEnabled = false;
            DRMS_DOCUMENT_META_DATA saveData = Form2Data();
            string excelPath = MainService.Instance.Save2Local(saveData);


            List<string> allImages = GetAllFilePath();
            allImages.Add(excelPath);



            List<P_MW_SCANNED_DOCUMENT> subDocList = new List<P_MW_SCANNED_DOCUMENT>();

            for (int i = 0; i < TabControlSubDocument.Items.Count; i++)
            {
                TabItem tab = TabControlSubDocument.Items[i] as TabItem;
                UCSubDocument subDoc = tab.Content as UCSubDocument;
                P_MW_SCANNED_DOCUMENT subDocData = subDoc.Form2Data(ComboBoxForm.Text, TextBoxDsn.Text);
                subDocList.Add(subDocData);
            }
            string ser = JsonConvert.SerializeObject(subDocList, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            saveData.SubDocList = ser;
            NameValueCollection nv = new NameValueCollection { { "SUB", ser } };
            MainService.Instance.Upload(allImages, nv);

            MainService.Instance.WindowDoc.RemoveDoc(saveData.DSN);
            MainService.Instance.HIST_DocumentType = ComboBoxDocumentType.Text;
            MainService.Instance.HIST_LastSubmission = DatePickerSubmissionLetterDate.SelectedDate;
        }
        private DRMS_DOCUMENT_META_DATA Form2Data()
        {

            DRMS_DOCUMENT_META_DATA data = new DRMS_DOCUMENT_META_DATA();
            data.MW_SUBMISSION_NUMBER = TextBoxMwSubmissionNo.Text;
            data.DSN = TextBoxDsn.Text;
            data.SUBMISSION_LETTER_DATE = DatePickerSubmissionLetterDate.SelectedDate == null ? "":
                DatePickerSubmissionLetterDate.SelectedDate.Value.Ticks.ToString();
            data.FORM = ComboBoxForm.Text;
            //data.MW_ITEM_NUMBER = ComboBoxMwItemNo.SelectedItems.ToString() ;
            data.DOCUMENT_TYPE = ComboBoxDocumentType.Text;
            data.BD_FILE_REF = TextBoxBdFileRef.Text;
            data.WORKS_LOCATION = TextBoxWorkLocation.Text;
            data.FILE_CLASSIFICATION = ComboBoxFileClassification.Text;
            data.ORIGIN_OF_RECORD = ComboBoxOriginOfRecord.Text;
            data.TYPE_OF_CORRESPONDENCE = ComboBoxTypeOfCorrespondence.Text;
            data.LOCATION_OF_RECORD = TextBoxLocationOfRecord.Text;
            data.SELECTED_FOR_MW_AUDIT_CHECK = TextBoxSelectedForMwAuditCheck.Text;
            data.BACKLOG_UPLOAD = ComboBoxBacklogUpload.Text;
            data.EFSS_SUBMISSION_REF_NO = TextBoxEffsSubmissionNo.Text;
            data.DOCUMENT_CONTROL_STAGE = TextBoxDocumentControlStage.Text;

            string it = "";
            for (int i= 0;i < ComboBoxMwItemNo.SelectedItems.Count; i++)
            {
                it = it + ComboBoxMwItemNo.SelectedItems[i] + ";";
            }
            data.MW_ITEM_NUMBER = it;
            return data;
        }

        public void ComboBoxDocumentType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            for (int i = 0; i < TabControlSubDocument.Items.Count; i++)
            {
                TabItem item = TabControlSubDocument.Items[i] as TabItem;
                UCSubDocument subDoc = item.Content as UCSubDocument;
                subDoc.SetDocType(ComboBoxDocumentType.SelectedItem as string);
            }
        }
        
        private List<string> GetAllFilePath()
        {
            List<string> files = new List<string>();
            for (int i = 0; i < TabControlSubDocument.Items.Count; i++)
            {
                TabItem tab = TabControlSubDocument.Items[i] as TabItem;
                UCSubDocument uCSubDocument = tab.Content as UCSubDocument;
                files.Add(uCSubDocument.FilePath);
            }
            return files;
        }

        private void TextBoxDocumentControlStage_TextChanged(object sender, TextChangedEventArgs e)
        {
           
        }

        private void TextBoxDocumentControlStage_LostFocus(object sender, RoutedEventArgs e)
        {
            if(TextBoxDocumentControlStage.Text != "1" && TextBoxDocumentControlStage.Text != "2" 
                && TextBoxDocumentControlStage.Text != "4" && TextBoxDocumentControlStage.Text != "3")
            {
                TextBoxDocumentControlStage.Text = "1";
            }
        }
    }
}
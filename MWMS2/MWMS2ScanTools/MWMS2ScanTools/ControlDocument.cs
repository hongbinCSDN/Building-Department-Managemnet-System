using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Collections.Specialized;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Runtime.InteropServices;
using System.Configuration;

namespace MWMS2ScanTools
{

    public partial class ControlDocument : UserControl
    {

        string localDataPath = ConfigurationManager.AppSettings["localDocument"];
        NameValueCollection _data;
        FormMain _formMain;
        TabPage tabPage;
        public ControlDocument(FormMain formMain, TabPage t, NameValueCollection data, string pdfPath)
        {
            _data = data;
            _formMain = formMain;
            tabPage = t;
            InitializeComponent();


            fSELECTED_FOR_MW_AUDIT_CHECK.Items.Add("N");
            fSELECTED_FOR_MW_AUDIT_CHECK.Items.Add("Y");

            fBACKLOG_UPLOAD.Items.Add("N");
            fBACKLOG_UPLOAD.Items.Add("Y");

            fFILE_CLASSIFICATION.Items.Add("Confidential");
            fFILE_CLASSIFICATION.Items.Add("Restricted");
            fFILE_CLASSIFICATION.Items.Add("Secret");
            fFILE_CLASSIFICATION.Items.Add("Top Secret");
            fFILE_CLASSIFICATION.Items.Add("Unclassified");

            fDISPOSAL_OF_RECORD_STATUS.Items.Add("Hardcopy disposed");
            fDISPOSAL_OF_RECORD_STATUS.Items.Add("Not yet disposed");







            fDOCUMENT_TYPE.Items.Add("Form");
            fDOCUMENT_TYPE.Items.Add("Letter");
            fDOCUMENT_TYPE.Items.Add("Memo");
            fDOCUMENT_TYPE.Items.Add("Others");
            fDOCUMENT_TYPE.Items.Add("Photo");
            fDOCUMENT_TYPE.Items.Add("Plan");
            fDOCUMENT_TYPE.Items.Add("Safety Inspection Report");
            fDOCUMENT_TYPE.Items.Add("Structural Calculation");
            fDOCUMENT_TYPE.Items.Add("Supervision Plan");

            fFORM.Items.Add("BD106");
            fFORM.Items.Add("MW01");
            fFORM.Items.Add("MW02");
            fFORM.Items.Add("MW03");
            fFORM.Items.Add("MW04");
            fFORM.Items.Add("MW05");
            fFORM.Items.Add("MW06");
            fFORM.Items.Add("MW07");
            fFORM.Items.Add("MW08");
            fFORM.Items.Add("MW09");
            fFORM.Items.Add("MW10");
            fFORM.Items.Add("MW11");
            fFORM.Items.Add("MW12");
            fFORM.Items.Add("MW31");
            fFORM.Items.Add("MW32");
            fFORM.Items.Add("MW33");
            fFORM.Items.Add("PNRC72");
            fFORM.Items.Add("SC01");
            fFORM.Items.Add("SC01C");
            fFORM.Items.Add("SC02");
            fFORM.Items.Add("SC02C");
            fFORM.Items.Add("SC03");
            fFORM.Items.Add("SC03C");


            fMW_ITEM_NUMBER.Items.Add("1(a)-3");
            fMW_ITEM_NUMBER.Items.Add("1(b)-3");
            fMW_ITEM_NUMBER.Items.Add("1(c)-3");
            fMW_ITEM_NUMBER.Items.Add("1.1");
            fMW_ITEM_NUMBER.Items.Add("1.10");
            fMW_ITEM_NUMBER.Items.Add("1.11");
            fMW_ITEM_NUMBER.Items.Add("1.12");
            fMW_ITEM_NUMBER.Items.Add("1.13");
            fMW_ITEM_NUMBER.Items.Add("1.14");
            fMW_ITEM_NUMBER.Items.Add("1.15");
            fMW_ITEM_NUMBER.Items.Add("1.16");
            fMW_ITEM_NUMBER.Items.Add("1.17");
            fMW_ITEM_NUMBER.Items.Add("1.18");
            fMW_ITEM_NUMBER.Items.Add("1.19");
            fMW_ITEM_NUMBER.Items.Add("1.2");
            fMW_ITEM_NUMBER.Items.Add("1.20");
            fMW_ITEM_NUMBER.Items.Add("1.21");
            fMW_ITEM_NUMBER.Items.Add("1.22");
            fMW_ITEM_NUMBER.Items.Add("1.23");
            fMW_ITEM_NUMBER.Items.Add("1.24");
            fMW_ITEM_NUMBER.Items.Add("1.25");
            fMW_ITEM_NUMBER.Items.Add("1.26");
            fMW_ITEM_NUMBER.Items.Add("1.27");
            fMW_ITEM_NUMBER.Items.Add("1.28");
            fMW_ITEM_NUMBER.Items.Add("1.29");
            fMW_ITEM_NUMBER.Items.Add("1.3");
            fMW_ITEM_NUMBER.Items.Add("1.30");
            fMW_ITEM_NUMBER.Items.Add("1.31");
            fMW_ITEM_NUMBER.Items.Add("1.32");
            fMW_ITEM_NUMBER.Items.Add("1.33");
            fMW_ITEM_NUMBER.Items.Add("1.34");
            fMW_ITEM_NUMBER.Items.Add("1.35");
            fMW_ITEM_NUMBER.Items.Add("1.36");
            fMW_ITEM_NUMBER.Items.Add("1.37");
            fMW_ITEM_NUMBER.Items.Add("1.38");
            fMW_ITEM_NUMBER.Items.Add("1.39");

            fORIGIN_OF_RECORD.Items.Add("Electronic");
            fORIGIN_OF_RECORD.Items.Add("Paper");

            fPROVISIONS_MODIFIED_EXEMPTED.Items.Add("B(P)R 30");
            fPROVISIONS_MODIFIED_EXEMPTED.Items.Add("B(P)R 31");
            fPROVISIONS_MODIFIED_EXEMPTED.Items.Add("B(P)R 35A");
            fPROVISIONS_MODIFIED_EXEMPTED.Items.Add("B(P)R 36");
            fPROVISIONS_MODIFIED_EXEMPTED.Items.Add("B(P)R 45");
            fPROVISIONS_MODIFIED_EXEMPTED.Items.Add("B(SSFPDWL)R 29(1)&(2), 44(4) & 50(2)");

            fREASON_OF_DISPOSAL.Items.Add("Record has been digitised and retention period lapsed");

            fSCAN_QUALITY_CHECK_STATUS.Items.Add("Acceptable");
            fSCAN_QUALITY_CHECK_STATUS.Items.Add("Acceptable after re-scan");
            fSCAN_QUALITY_CHECK_STATUS.Items.Add("Not acceptable");
            fSCAN_QUALITY_CHECK_STATUS.Items.Add("Not selected for scan quality check");

            fTYPE_OF_CORRESPONDENCE.Items.Add("Incoming");
            fTYPE_OF_CORRESPONDENCE.Items.Add("Outgoning");



            fDOCUMENT_TYPE.SelectedIndex = 0;

            SetUIData(data);
            if (fUPLOADED.Checked)
            {
                buttonUpload.Enabled = false;
                buttonSave.Enabled = false;
            }

            /*if (File.Exists(pdfPath))
            {
                TabPage myTabPage = new TabPage();
                ControlSubDocument subDocument = new ControlSubDocument(tabControlSubDoc, myTabPage, null, pdfPath);

                subDocument.Dock = DockStyle.Fill;
                myTabPage.Controls.Add(subDocument);
                tabControlSubDoc.TabPages.Add(myTabPage);
                tabControlSubDoc.SelectedTab = myTabPage;


            }*/
            //SetUIData(_data);

            /*
            if (fUPLOADED.Checked)
            {
                buttonUpload.Enabled = false;
                buttonSave.Enabled = false;
            }*/
            //fDOCUMENT_TYPE.Items.Add("(A) Form");
            //fDOCUMENT_TYPE.Items.Add("(B) Photo");
            //fDOCUMENT_TYPE.Items.Add("(C) Plan");
            //fDOCUMENT_TYPE.Items.Add("(D) Other");
            //fDOCUMENT_TYPE.Items.Add("(E) SSP");
            //fDOCUMENT_TYPE.Items.Add("(F) Letter");
            //fDOCUMENT_TYPE.SelectedIndex = 0;
            //fDOCUMENT_TYPE.SelectedIndexChanged += new EventHandler(LoadDetails_TextChanged);
            //fATT_SRC.TextChanged += new EventHandler(LoadDetails_TextChanged);

        }
        //private void LoadDetails_TextChanged(object sender, EventArgs e)
        //{
        //    if (fATT_SRC.Text != "")
        //    {
        //        fDSN.Text = fATT_SRC.Text;// + (fDOCUMENT_TYPE.Text.Length > 1 ? fDOCUMENT_TYPE.Text[1] + "" : "");
        //        fATT_NAME.Text = fATT_SRC.Text;// + (fDOCUMENT_TYPE.Text.Length > 1 ? fDOCUMENT_TYPE.Text[1] + "" : "");
        //    } else
        //    {
        //        fDSN.Text = "";
        //        fATT_NAME.Text = "";
        //    }
        //}
        /*
          private void buttonClose_Click(object sender, EventArgs e)
          {
              if(!buttonSave.Enabled)
              {
                  _formMain.tabControl2.TabPages.Remove(tabPage);
              }
              else if ( MessageBox.Show("Close without save PDF?", "Confirm Close", MessageBoxButtons.YesNo) == DialogResult.Yes)
              {
                  _formMain.tabControl2.TabPages.Remove(tabPage);
              }
          }*/
        private string[] GetAllFilePath()
        {
            string[] files = new string[tabControlSubDoc.TabPages.Count];
            for (int i = 0; i < tabControlSubDoc.TabPages.Count; i++)
            {
                files[i] = (tabControlSubDoc.TabPages[i].Controls[0] as ControlSubDocument).axAcroPDF1.src.Replace("file://", "");
            }
            return files;
        }
        private void FileDelete(string path)
        {
            if (File.Exists(path))
                File.Move(path, path + "." + DateTime.Now.ToString("yyyyMMdd.HHmmss"));
        }
        private void RunSaveFlow(bool askReplace)
        {
            while (true)
            {
                try
                {
                    ToFile(askReplace);
                    break;
                }
                catch (LocalFileExistException ex)
                {
                    try
                    {
                        if (askReplace)
                        {
                            DialogResult dialogResult = MessageBox.Show("Local file  exist, replace?", "Warning", MessageBoxButtons.OKCancel);
                            if (dialogResult == DialogResult.OK) for (int i = 0; i < ex.Filepath.Length; i++) FileDelete(ex.Filepath[i]);
                            else if (dialogResult == DialogResult.Cancel) break;
                        }
                        else for (int i = 0; i < ex.Filepath.Length; i++) FileDelete(ex.Filepath[i]);

                    }
                    catch (Exception ex2)
                    {
                        MessageBox.Show(ex2.Message);
                        break;
                    }
                }
            }
        }
        private void ToFile(bool askReplace)
        {
            if (!Directory.Exists(localDataPath)) Directory.CreateDirectory(localDataPath);

            string datPath = localDataPath + "/" + fDSN.Text + ".dat";
            bool datExist = File.Exists(datPath);
            if (askReplace)
            {
                if (datExist)
                {
                    List<string> filepaths = new List<string>();
                    if (datExist) filepaths.Add(datPath);
                    if (filepaths.Count > 0) throw LocalFileExistException.Create(filepaths);
                }
            }
            else
            {
                FileDelete(datPath);
            }
            string docData = GetDocData();
            File.WriteAllText(datPath, docData);
        }

        private string GetDocData()
        {
            NameValueCollection data = GetUIData();
            string docData = "";
            for (int j = 0; j < data.Keys.Count; j++)
            {
                docData = docData + data.Keys[j] + "\r\n";
                docData = docData + data[data.Keys[j]] + "\r\n";
            }

            string[] files = GetAllFilePath();
            for (int i = 0; i < files.Length; i++)
            {
                ControlSubDocument subDoc = tabControlSubDoc.TabPages[i].Controls[0] as ControlSubDocument;
                JObject usbDocData = subDoc.GetUIData();
                string pdfPath = localDataPath + "/" + fDSN.Text + "_" + i + ".pdf";
                FileInfo oldFile = new FileInfo(files[i]);
                FileInfo newFile = new FileInfo(pdfPath);
                if (oldFile.FullName != newFile.FullName) { if (File.Exists(pdfPath)) { FileDelete(pdfPath); } File.Copy(files[i], pdfPath); }
                usbDocData["fFILE_PATH"] = newFile.FullName;
                docData = docData + "_SUB" + i + "\r\n";
                docData = docData + usbDocData.ToString().Replace('\r', ' ').Replace('\n', ' ') + "\r\n";
            }
            return Util.Base64Encode(docData);
        }





        public NameValueCollection GetUIData()
        { /*	UUID
            , REC_TYPE
            , REC_ID
            ---------, MW_SUBMISSION_NUMBER
            ---------, DSN
            ---------, DSN_VERSION_NUMBER
            ---------, FORM
            ---------, DOCUMENT_TYPE
            ---------, MW_ITEM_NUMBER
            ---------, BD_FILE_REF
            ---------, SUBMISSION_LETTER_DATE
            ---------, WORKS_LOCATION

            ---------, SELECTED_FOR_MW_AUDIT_CHECK
            ---------, ORIGIN_OF_RECORD
            ---------, EFSS_SUBMISSION_REF_NO
            ---------, TYPE_OF_CORRESPONDENCE

            ---------, FILE_CLASSIFICATION

            ---------, BACKLOG_UPLOAD
            , LOCATION_OF_RECORD
            , DOCUMENT_CONTROL_STAGE

            ---------, REASON_OF_RESCANNING
            ---------, SCAN_QUALITY_CHECK_STATUS
            ---------, SCAN_QUALITY_CHECK_DATETIME
            ---------, SCAN_QUALITY_CHECK_OFFICER_NAME_AND_POST

            ---------, DISPOSAL_OF_RECORD_STATUS
            ---------, DISPOSAL_OF_RECORD_DISPOSAL_DATE
            ---------, REASON_OF_DISPOSAL
            ---------, DISPOSAL_OFFICER_NAME_AND_POST

            ---------, BLOCK_ID
            ---------, UNIT_ID
            ---------, MODIFICATION_PERMIT_NUMBER
            ---------, MODIFICATION_GRANTED_ON
            ---------, PROVISIONS_MODIFIED_EXEMPTED
            , CREATED_BY
            , CREATED_DATE
            , MODIFIED_BY
            , MODIFIED_DATE
            */
            NameValueCollection parameters = new NameValueCollection();
            //parameters.Add("doc.ATT_NAME", fATT_NAME.Text);
            //parameters.Add("doc.ATT_SRC", fATT_SRC.Text);

            parameters.Add("doc.MW_SUBMISSION_NUMBER", fMW_SUBMISSION_NUMBER.Text);
            parameters.Add("doc.DSN", fDSN.Text);
            parameters.Add("doc.DSN_VERSION_NUMBER", fDSN_VERSION_NUMBER.Text);
            parameters.Add("doc.FORM", fFORM.Text);
            parameters.Add("doc.DOCUMENT_TYPE", fDOCUMENT_TYPE.Text);
            parameters.Add("doc.MW_ITEM_NUMBER", fMW_ITEM_NUMBER.Text);
            parameters.Add("doc.BD_FILE_REF", fBD_FILE_REF.Text);
            parameters.Add("doc.SUBMISSION_LETTER_DATE", fSUBMISSION_LETTER_DATE.Text);
            parameters.Add("doc.WORKS_LOCATION", fWORKS_LOCATION.Text);

            parameters.Add("doc.FILE_CLASSIFICATION", fFILE_CLASSIFICATION.Text);

            parameters.Add("doc.SELECTED_FOR_MW_AUDIT_CHECK", fSELECTED_FOR_MW_AUDIT_CHECK.Text);
            parameters.Add("doc.ORIGIN_OF_RECORD", fORIGIN_OF_RECORD.Text);
            parameters.Add("doc.EFSS_SUBMISSION_REF_NO", fEFSS_SUBMISSION_REF_NO.Text);
            parameters.Add("doc.TYPE_OF_CORRESPONDENCE", fTYPE_OF_CORRESPONDENCE.Text);
            parameters.Add("doc.BACKLOG_UPLOAD", fBACKLOG_UPLOAD.Text);
            parameters.Add("doc.LOCATION_OF_RECORD", fLOCATION_OF_RECORD.Text);
            parameters.Add("doc.DOCUMENT_CONTROL_STAGE", fDOCUMENT_CONTROL_STAGE.Text);



            parameters.Add("doc.REASON_OF_RESCANNING", fREASON_OF_RESCANNING.Text);
            parameters.Add("doc.SCAN_QUALITY_CHECK_STATUS", fSCAN_QUALITY_CHECK_STATUS.Text);
            parameters.Add("doc.SCAN_QUALITY_CHECK_DATETIME", fSCAN_QUALITY_CHECK_DATETIME.Text);
            parameters.Add("doc.SCAN_QUALITY_CHECK_OFFICER_NAME_AND_POST", fSCAN_QUALITY_CHECK_OFFICER_NAME_AND_POST.Text);


            parameters.Add("doc.DISPOSAL_OF_RECORD_STATUS", fDISPOSAL_OF_RECORD_STATUS.Text);
            parameters.Add("doc.DISPOSAL_OF_RECORD_DISPOSAL_DATE", fDISPOSAL_OF_RECORD_DISPOSAL_DATE.Text);
            parameters.Add("doc.REASON_OF_DISPOSAL", fREASON_OF_DISPOSAL.Text);
            parameters.Add("doc.DISPOSAL_OFFICER_NAME_AND_POST", fDISPOSAL_OFFICER_NAME_AND_POST.Text);


            parameters.Add("doc.BLOCK_ID", fBLOCK_ID.Text);
            parameters.Add("doc.UNIT_ID", fUNIT_ID.Text);
            parameters.Add("doc.MODIFICATION_PERMIT_NUMBER", fMODIFICATION_PERMIT_NUMBER.Text);
            parameters.Add("doc.MODIFICATION_GRANTED_ON", fMODIFICATION_GRANTED_ON.Text);
            parameters.Add("doc.PROVISIONS_MODIFIED_EXEMPTED", fPROVISIONS_MODIFIED_EXEMPTED.Text);

            parameters.Add("UPLOADED", fUPLOADED.Checked ? "Y" : "N");
            return parameters;
        }

        public void SetUIData(NameValueCollection parameters)
        { //= parameters[""];
            //NameValueCollection parameters = new NameValueCollection();
            if (parameters == null) return;
            //if (parameters.AllKeys.Contains("doc.ATT_NAME")) fATT_NAME.Text = parameters["doc.ATT_NAME"];
            //if (parameters.AllKeys.Contains("doc.ATT_SRC")) fATT_SRC.Text = parameters["doc.ATT_SRC"];

            if (parameters["doc.DOCUMENT_TYPE"] != null) fDOCUMENT_TYPE.Text = parameters["doc.DOCUMENT_TYPE"];
            if (parameters["doc.MW_SUBMISSION_NUMBER"] != null) fMW_SUBMISSION_NUMBER.Text = parameters["doc.MW_SUBMISSION_NUMBER"];
            if (parameters["doc.DSN"] != null) fDSN.Text = parameters["doc.DSN"];
            if (parameters["doc.SUBMISSION_LETTER_DATE"] != null) fSUBMISSION_LETTER_DATE.Text = parameters["doc.SUBMISSION_LETTER_DATE"];
            if (parameters["doc.DSN_VERSION_NUMBER"] != null) fDSN_VERSION_NUMBER.Text = parameters["doc.DSN_VERSION_NUMBER"];
            if (parameters["doc.FORM"] != null) fFORM.Text = parameters["doc.FORM"];
            if (parameters["doc.MW_ITEM_NUMBER"] != null) fMW_ITEM_NUMBER.Text = parameters["doc.MW_ITEM_NUMBER"];
            if (parameters["doc.BD_FILE_REF"] != null) fBD_FILE_REF.Text = parameters["doc.BD_FILE_REF"];
            if (parameters["doc.WORKS_LOCATION"] != null) fWORKS_LOCATION.Text = parameters["doc.WORKS_LOCATION"];
            if (parameters["doc.FILE_CLASSIFICATION"] != null) fFILE_CLASSIFICATION.Text = parameters["doc.FILE_CLASSIFICATION"];

            if (parameters["doc.SELECTED_FOR_MW_AUDIT_CHECK"] != null)
                fSELECTED_FOR_MW_AUDIT_CHECK.Text = parameters["doc.SELECTED_FOR_MW_AUDIT_CHECK"];

            if (parameters["doc.ORIGIN_OF_RECORD"] != null)
                fORIGIN_OF_RECORD.Text = parameters["doc.ORIGIN_OF_RECORD"];

            if (parameters["doc.EFSS_SUBMISSION_REF_NO"] != null)
                fEFSS_SUBMISSION_REF_NO.Text = parameters["doc.EFSS_SUBMISSION_REF_NO"];

            if (parameters["doc.TYPE_OF_CORRESPONDENCE"] != null)
                fTYPE_OF_CORRESPONDENCE.Text = parameters["doc.TYPE_OF_CORRESPONDENCE"];

            if (parameters["doc.BACKLOG_UPLOAD"] != null)
                fBACKLOG_UPLOAD.Text = parameters["doc.BACKLOG_UPLOAD"];

            if (parameters["doc.LOCATION_OF_RECORD"] != null)
                fLOCATION_OF_RECORD.Text = parameters["doc.LOCATION_OF_RECORD"];

            if (parameters["doc.DOCUMENT_CONTROL_STAGE"] != null)
                fDOCUMENT_CONTROL_STAGE.Text = parameters["doc.DOCUMENT_CONTROL_STAGE"];



            if (parameters["doc.REASON_OF_RESCANNING"] != null)
                fREASON_OF_RESCANNING.Text = parameters["doc.REASON_OF_RESCANNING"];

            if (parameters["doc.SCAN_QUALITY_CHECK_STATUS"] != null)
                fSCAN_QUALITY_CHECK_STATUS.Text = parameters["doc.SCAN_QUALITY_CHECK_STATUS"];

            if (parameters["doc.SCAN_QUALITY_CHECK_DATETIME"] != null)
                fSCAN_QUALITY_CHECK_DATETIME.Text = parameters["doc.SCAN_QUALITY_CHECK_DATETIME"];

            if (parameters["doc.SCAN_QUALITY_CHECK_OFFICER_NAME_AND_POST"] != null)
                fSCAN_QUALITY_CHECK_OFFICER_NAME_AND_POST.Text = parameters["doc.SCAN_QUALITY_CHECK_OFFICER_NAME_AND_POST"];


            if (parameters["doc.DISPOSAL_OF_RECORD_STATUS"] != null)
                fDISPOSAL_OF_RECORD_STATUS.Text = parameters["doc.DISPOSAL_OF_RECORD_STATUS"];

            if (parameters["doc.DISPOSAL_OF_RECORD_DISPOSAL_DATE"] != null)
                fDISPOSAL_OF_RECORD_DISPOSAL_DATE.Text = parameters["doc.DISPOSAL_OF_RECORD_DISPOSAL_DATE"];

            if (parameters["doc.REASON_OF_DISPOSAL"] != null)
                fREASON_OF_DISPOSAL.Text = parameters["doc.REASON_OF_DISPOSAL"];

            if (parameters["doc.DISPOSAL_OFFICER_NAME_AND_POST"] != null)
                fDISPOSAL_OFFICER_NAME_AND_POST.Text = parameters["doc.DISPOSAL_OFFICER_NAME_AND_POST"];




            if (parameters["doc.BLOCK_ID"] != null) fBLOCK_ID.Text = parameters["doc.BLOCK_ID"];
            if (parameters["doc.UNIT_ID"] != null) fUNIT_ID.Text = parameters["doc.UNIT_ID"];
            if (parameters["doc.MODIFICATION_PERMIT_NUMBER"] != null) fMODIFICATION_PERMIT_NUMBER.Text = parameters["doc.MODIFICATION_PERMIT_NUMBER"];
            if (parameters["doc.MODIFICATION_GRANTED_ON"] != null) fMODIFICATION_GRANTED_ON.Text = parameters["doc.MODIFICATION_GRANTED_ON"];
            if (parameters["doc.PROVISIONS_MODIFIED_EXEMPTED"] != null) fPROVISIONS_MODIFIED_EXEMPTED.Text = parameters["doc.PROVISIONS_MODIFIED_EXEMPTED"];
            if (parameters["UPLOADED"] != null) fUPLOADED.Checked = parameters["UPLOADED"] == "Y";
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            _formMain.CloseTab(tabPage, buttonSave.Enabled);

        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (fDSN.Text == "")
            {
                MessageBox.Show("Plesae input Attachment Name Source.");
            }
            else
            {
                RunSaveFlow(true);
                _formMain.CloseTab(tabPage, false);
                _formMain.ReloadLocalFile();
            }
        }

        private void fDSN_TextChanged(object sender, EventArgs e)
        {
            return;
            JObject rsp = Program.docServerKernal.LoadDocInfo(fDSN.Text);
            JToken data = rsp["DATA"];
            if (data != null)
            {
                fMW_SUBMISSION_NUMBER.Text = data["RECORD_ID"].ToString();
                fFORM.Text = data["FORM_CODE"].ToString();
            }
            else
            {
                fMW_SUBMISSION_NUMBER.Text = "";
                fFORM.Text = "";
            }
            ControlSubDocument point;
            for (int i = 0; i < tabControlSubDoc.TabPages.Count; i++)
            {
                point = tabControlSubDoc.TabPages[i].Controls[0] as ControlSubDocument;
                point.fDSN_SUB.Text = fDSN.Text;
                //point.fFORM_TYPE
            }
        }

        private void buttonUpload_Click(object sender, EventArgs e)
        {
            if (fDSN.Text == "")
            {
                MessageBox.Show("Plesae input DSN."); return;
            }

            Dictionary<string, string> curType = new Dictionary<string, string>();
            for (int i = 0; i < tabControlSubDoc.TabPages.Count; i++)
            {
                if (curType.ContainsKey(tabControlSubDoc.TabPages[i].Text))
                {
                    MessageBox.Show("Duplicate sub-document type : " + tabControlSubDoc.TabPages[i].Text);
                    return;
                }
                curType.Add(tabControlSubDoc.TabPages[i].Text, "");
            }

            fUPLOADED.Checked = true;
            RunSaveFlow(false);
            try
            {
                NameValueCollection docData = new NameValueCollection();
                docData.Add("Data", GetDocData());
                Program.docServerKernal.Upload(GetAllFilePath(), docData);
                MessageBox.Show("Upload Success.");
                fUPLOADED.Checked = true;
                RunSaveFlow(false);
                //Program.docServerKernal.Upload(axAcroPDF1.src.Replace("file://", ""), GetUIData());
            }
            catch (Exception ex)
            {
                MessageBox.Show("File saved but cannot upload to server.");
            }
            _formMain.CloseTab(tabPage, false);
            _formMain.ReloadLocalFile();
        }

    }
}
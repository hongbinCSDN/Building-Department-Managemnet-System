using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections.Specialized;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using System.IO;
using PdfSharp.Pdf.IO;
using PdfSharp.Pdf.Advanced;
using TwainDotNet;
using TwainDotNet.WinFroms;
using System.Drawing.Imaging;
using Newtonsoft.Json.Linq;
using Microsoft.VisualBasic;
using System.Configuration;

namespace MWMS2ScanTools
{
    public partial class FormMain : Form
    {
        private string dir = ConfigurationManager.AppSettings["localDocument"];
        private string ext = "dat";
        private string tempDir = ConfigurationManager.AppSettings["temp"];

        Twain _twain = null;
        FormLoading fl;
        int pdfCounter = 0;
        int scanSeq = 0;
        Dictionary<string, NameValueCollection> localData = new Dictionary<string, NameValueCollection>();
        Dictionary<string, NameValueCollection> serverData = new Dictionary<string, NameValueCollection>();
        Dictionary<string, TabPage> openedTabPage = new Dictionary<string, TabPage>();
        TreeNode treeNode = new TreeNode("Local Scanned Documents");
        public FormMain()
        {
            try
            {
                _twain = new Twain(new WinFormsWindowMessageHook(this));
                _twain.TransferImage += _twain_TransferImage;
                _twain.ScanningComplete += _twain_ScanningComplete;
            } catch(Exception ex)
            {

            }


            if (!Directory.Exists(tempDir)) Directory.CreateDirectory(tempDir);
            DirectoryInfo di = new DirectoryInfo(tempDir);
            FileInfo[] s = di.GetFiles("*.pdf");
            List<string> tempFiles = new List<string>();
            for (int i = 0; i <s.Length; i++)
            {
                tempFiles.Add(s[i].Name);
            }
            //List<string> tempFiles = di.GetFiles("*.pdf").Select(o => o.Name).ToList();
            
            for(int i= 0;i < tempFiles.Count; i++)
            {
                string[] splited = tempFiles[i].Split('.');
                if (splited.Length != 4) continue;
                if (splited[0] != "Scan") continue;
                if (splited[3].ToLower() != "pdf") continue;
                int tempScanId;
                try { tempScanId = int.Parse(splited[1]); } catch { continue; }
                pdfCounter = Math.Max(pdfCounter, tempScanId);
            }
            InitializeComponent();


            ImageList myImageList = new ImageList();
            treeView1.ImageList = myImageList;
            
            fl = new FormLoading(this);

            //TreeNode node_2 = new TreeNode(" 2013");
            treeView1.ImageList = myImageList;
            //TreeNode node_3 = new TreeNode(" 2014");
            //TreeNode node_4 = new TreeNode(" 2015");
            //TreeNode node_5 = new TreeNode(" 2016");
            //TreeNode node_6 = new TreeNode(" 2017");
            //TreeNode node_7 = new TreeNode(" 2018");
            //T/reeNode node_8 = new TreeNode(" 2019");
            //TreeNode[] array_ = new TreeNode[] { node_2, node_3, node_4, node_5, node_6, node_7, node_8 };
            //
            // Final node.
            //
            TreeNode treeNode2 = new TreeNode("Server Documents");

            treeView1.Nodes.Add(treeNode);
            treeView1.Nodes.Add(treeNode2);
            treeView1.NodeMouseDoubleClick += TreeView1_NodeMouseDoubleClick;
            ReloadLocalFile();
            if (_twain != null)
            {
                IList<string> sourceList = _twain.SourceNames;
                for(int i=  0;i < sourceList.Count; i++)
                {
                    comboBoxScanner.Items.Add(sourceList[i]);
                    if (sourceList[i].IndexOf("TWAIN") >= 0) comboBoxScanner.SelectedIndex = comboBoxScanner.Items.Count - 1;
                }
            }
        }

        private void _twain_ScanningComplete(object sender, ScanningCompleteEventArgs e)
        {
            if (e.Exception != null) Scan((Twain)sender);
            else buttonScan.Enabled = true;
        }
        private void _twain_TransferImage(object sender, TransferImageEventArgs args)
        {
            string fileName = "Scan." + pdfCounter + "." + ++scanSeq + ".jpg";
            if (args.Image != null)
            {
                Bitmap resultImage = args.Image;
                MemoryStream ms = new MemoryStream();
                resultImage.Save(ms, ImageFormat.Jpeg);
                if (!Directory.Exists(tempDir)) Directory.CreateDirectory(tempDir);
                DirectoryInfo di = new DirectoryInfo(tempDir);
                //List<string> tempFiles = di.GetFiles("*.jpg").Select(o => o.Name).ToList();
                FileInfo[] s = di.GetFiles("*.jpg");
                List<string> tempFiles = new List<string>();
                for (int i = 0; i < s.Length; i++)
                {
                    tempFiles.Add(s[i].Name);
                }
                FileStream file = new FileStream(tempDir + "/" + fileName, FileMode.Create, FileAccess.Write);
                ms.WriteTo(file);
                file.Close();
                ms.Close();
                AddImage(tempDir + "/" + fileName);
            }
        }
        private void Scan(Twain _twain)
        {
            if (_twain == null) return;
            pdfCounter++;
               scanSeq = 0;
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
            _twain.SelectSource(comboBoxScanner.Text);
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


        public void ReloadServerFile ()
        {
            treeView1.Nodes[1].Nodes.Clear();
            serverData.Clear();
            JObject rs = Program.docServerKernal.SearchDoc(textBoxSearchKeyword.Text);
            if (rs.GetValue("RESULT").ToString() == "True")
            {
                JToken data = rs.GetValue("data");
                /*for (int i = 0; i < data.; i++)
                {
                    try
                    {
                        JObject each = data[i] as JObject;
                        NameValueCollection nvc = new NameValueCollection();
                        foreach (JProperty prop in each.Properties())
                        {
                            nvc.Add("doc." + prop.Name, prop.Value.ToString());
                        }
                        if (nvc.Get("doc.DSN") != null) serverData.Add(nvc.Get("doc.DSN"), nvc);
                    }
                    catch(Exception ex)
                    {

                    }
                }*/
                foreach (var k in serverData.Keys)
                {

                    string dsn = serverData[k]["doc.DSN"];
                    TreeNode node = new TreeNode(dsn);
                    treeView1.Nodes[1].Nodes.Add(node);
                    treeView1.Nodes[0].Collapse(true);
                    treeView1.Nodes[1].Expand();
                }
            }
        }

        public void ReloadLocalFile()
        {
            treeView1.Nodes[0].Nodes.Clear();
            localData.Clear();
            try
            {
                if (Directory.Exists(dir))
                {
                    DirectoryInfo d = new DirectoryInfo(dir);
                    FileInfo[] fi = d.GetFiles("*." + ext);
                    for (int i = 0; i < fi.Length; i++)
                    {
                        NameValueCollection data = new NameValueCollection();
                        string text = File.ReadAllText(fi[i].FullName);
                        text =  Util.Base64Decode(text);
                        string[] lines = text.Replace("\r\n","\n").Split('\n');
                        //string[] lines = File.ReadAllLines(fi[i].FullName);
                        for (int j = 0; j < lines.Length; j++)
                        {if (lines[j] == "") break;
                            data.Add(lines[j], lines[++j]);
                        }
                        localData.Add(fi[i].Name.Substring(0, fi[i].Name.Length - ext.Length - 1), data);
                    }
                    foreach (var k in localData.Keys)
                    {

                        string dsn = localData[k]["doc.DSN"];
                        TreeNode node = new TreeNode(dsn);
                        treeView1.Nodes[0].Nodes.Add(node);
                    }
                }
                else Directory.CreateDirectory(dir);
            }
            catch
            {

            }
        }











        private void TreeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if(e.Node.Parent == treeNode)
            {
                if(openedTabPage.ContainsKey(e.Node.Text))
                {
                    tabControlDoc.SelectedTab = openedTabPage[e.Node.Text];
                } else
                {
                    AddDoc(e.Node.Text, null, localData[e.Node.Text]);
                }
            }
        }

        public void AddImage(string filename)
        {
            ImageFile imageFile;
            try
            {
                if (Util.isJPG(filename)) imageFile = (new ImageFile(filename, "JPG", listView1.View != View.List));
                else if (Util.isPDF(filename)) imageFile = (new ImageFile(filename, "PDF", listView1.View != View.List));
                else return;
            }
            catch (Exception ex) { return; }
            if (listView1.View != View.List && listView1.Items.Count > 50) listView1.View = View.List;
            if (listView1.LargeImageList == null)
            {
                listView1.LargeImageList = new ImageList();
                listView1.LargeImageList.ImageSize = new Size(64, 64);
                listView1.LargeImageList.ColorDepth = ColorDepth.Depth32Bit;
            }
            if (!listView1.LargeImageList.Images.ContainsKey(imageFile.FileInfo.FullName))
            {
               // if (imageFile.Thumb != null)
                    listView1.LargeImageList.Images.Add(imageFile.FileInfo.FullName, imageFile.Thumb);
                ListViewItem item = new ListViewItem();
                item.Text = imageFile.FileType == "JPG" ? imageFile.FileInfo.Name : imageFile.FileInfo.Name;
                item.ImageKey = imageFile.FileType == "JPG" ? imageFile.FileInfo.FullName : imageFile.FileInfo.FullName;
                listView1.Items.Add(item);
            }
        }

        private void buttonLoad_Click(object sender, EventArgs e)
        {

            listView1.View = View.LargeIcon;
            DialogResult result = openFileDialog1.ShowDialog();
            List<ImageFile> imageFiles = new List<ImageFile>();

            if (result == DialogResult.OK)
            {
                for (int i = 0; i < openFileDialog1.FileNames.Length; i++)
                {
                    string filename = openFileDialog1.FileNames[i];
                    AddImage(filename);
                }
            }
        }
        private void buttonMerge_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select image(s).");
                return;
            }
            string dsn = Interaction.InputBox("Please input DSN", "", "");
            if (dsn == "") return;
            JObject rsp = Program.docServerKernal.LoadDocInfo(dsn);
            if(rsp["RESULT"].ToString().ToLower() == "false") {
                MessageBox.Show("DSN no valid");
                return;
            }
            /*
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

            */


            PdfDocument myDoc = new PdfDocument();
            myDoc.Info.Title = "Document by MWMS2 Scanning Tools";
            myDoc.Info.Author = "MWMS2 Scanning Tools";
          
            for (int i = 0 ; i < listView1.LargeImageList.Images.Keys.Count; i++)
            {
                if (!listView1.Items[i].Selected) continue;
                if (Util.isJPG(listView1.LargeImageList.Images.Keys[i]))
                {
                    PdfPage myPage = myDoc.AddPage();
                    XGraphics g = XGraphics.FromPdfPage(myPage);
                    XImage img = XImage.FromFile(listView1.LargeImageList.Images.Keys[i]);
                    g.DrawImage(img, new XRect(10, 10, myPage.Width - 20, myPage.Height - 20));
                    img.Dispose();
                    g.Dispose();
                    myPage.Close();
                }
                else if (Util.isPDF(listView1.LargeImageList.Images.Keys[i]))
                {
                    PdfDocument doc = PdfReader.Open(listView1.LargeImageList.Images.Keys[i], PdfDocumentOpenMode.Import);
                    foreach (PdfPage page in doc.Pages)
                    {
                        myDoc.AddPage(page);
                    }
                    doc.Close();
                }
            }
            //MemoryStream memoryStream = new MemoryStream();
            //myDoc.Save(@"C:\TEMP\TEST.PDF");

            //myDoc.Save(memoryStream);

            string tempName = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            using (FileStream fs = new FileStream(tempName, FileMode.OpenOrCreate))
            {
                myDoc.Save(fs);
                myDoc.Close();
                myDoc.Dispose();
                //memoryStream.CopyTo(fs);
                fs.Flush();
            }
            ControlDocument cdoc = AddDoc(dsn, tempName, null);
            cdoc.fMW_SUBMISSION_NUMBER.Text = rsp["data"]["RECORD_ID"].ToString();
            cdoc.fFORM.Text = rsp["data"]["FORM_CODE"].ToString();


            //AddDocTab(tempName, null);
        }
        private ControlDocument AddDoc(string dsn, string pdfPath, NameValueCollection data)
        {
            TabPage tabPage = AddDocTab(dsn, pdfPath, data);

            ControlDocument controlDocument = tabPage.Controls[0] as ControlDocument;
            if (pdfPath != null)
            {
                TabPage subTabPage = new TabPage();
                ControlSubDocument controlSubDocument = new ControlSubDocument(controlDocument.tabControlSubDoc, subTabPage, null, pdfPath);
                controlSubDocument.Dock = DockStyle.Fill;
                subTabPage.Controls.Add(controlSubDocument);
                controlDocument.tabControlSubDoc.TabPages.Add(subTabPage);
                controlDocument.tabControlSubDoc.SelectedTab = subTabPage;
            }
            if(data != null) for(int i= 0;i < data.Count; i++)
            {
                if(data.Keys[i].StartsWith("_SUB"))
                {
                    JObject jo = JObject.Parse(data[data.Keys[i]]);
                    TabPage subTabPage = new TabPage();
                    ControlSubDocument controlSubDocument = new ControlSubDocument(controlDocument.tabControlSubDoc, subTabPage, jo, null);
                    controlSubDocument.Dock = DockStyle.Fill;
                    subTabPage.Controls.Add(controlSubDocument);
                    controlDocument.tabControlSubDoc.TabPages.Add(subTabPage);
                    controlDocument.tabControlSubDoc.SelectedTab = subTabPage;
                }
            }
            return controlDocument;

        }
        private TabPage AddDocTab(string dsn, string pdfPath, NameValueCollection data)
        {
            if (openedTabPage.ContainsKey(dsn)) return openedTabPage[dsn];
            TabPage myTabPage = new TabPage();
            ControlDocument myUserControl = new ControlDocument(this, myTabPage, data, pdfPath);

            openedTabPage.Add(dsn, myTabPage);
            myUserControl.fDSN.Text = dsn;
            myTabPage.Text = dsn;

            myUserControl.Dock = DockStyle.Fill;
            myTabPage.Controls.Add(myUserControl);
            tabControlDoc.TabPages.Add(myTabPage);
            tabControlDoc.SelectedTab = myTabPage;
            return myTabPage;
        }
        public void CloseTab(TabPage tabPage, bool ask)
        {
            if (!ask)
            {
                tabControlDoc.TabPages.Remove(tabPage);
                foreach (string key in openedTabPage.Keys)
                {
                    if (openedTabPage[key] == tabPage) { openedTabPage.Remove(key); break; }
                }
            }
            else if (MessageBox.Show("Close without save PDF?", "Confirm Close", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                tabControlDoc.TabPages.Remove(tabPage);
                foreach (string key in openedTabPage.Keys)
                {
                    if (openedTabPage[key] == tabPage) { openedTabPage.Remove(key); break; }
                }
            }


            /*
            tabControlDoc.TabPages.Remove(tabPage);
            if (openedTabPage.Where(o => o.Value == tabPage).FirstOrDefault().Key != null)
            {
                openedTabPage.Remove(openedTabPage.Where(o => o.Value == tabPage).FirstOrDefault().Key);
            }*/
            //openedTabPage.Remove

        }

        private void buttonScan_Click(object sender, EventArgs e)
        {
            if (_twain == null)
            {
                MessageBox.Show("No scanner connected.");
                return;
            } else if (comboBoxScanner.Text == "")
            {
                MessageBox.Show("Please select scanner.");
                comboBoxScanner.Focus();
                comboBoxScanner.DroppedDown = true;
                return;
            }
            buttonScan.Enabled = false;
            Scan(_twain);
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            ReloadServerFile();
        }
    }


}

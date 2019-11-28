using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Collections.Specialized;
using Newtonsoft.Json.Linq;

namespace MWMS2ScanTools
{
    public partial class ControlSubDocument : UserControl
    { TabPage _t;
        public ControlSubDocument(TabControl tabControl, TabPage t, JObject data, string pdfPath)
        {
            _t = t;
            InitializeComponent();
            fSUBMIT_TYPE.Items.Add("Submission/ Amended Form");
            fSUBMIT_TYPE.Items.Add("Enquiry");
            fSUBMIT_TYPE.Items.Add("Complaint");
            fSUBMIT_TYPE.Items.Add("Outgoing Correspondence");
            fSUBMIT_TYPE.Items.Add("Incoming Correspondence");
            fSUBMIT_TYPE.Items.Add("Unknown");

            fFOLDER_TYPE.Items.Add("Public");
            fFOLDER_TYPE.Items.Add("Private");

            fDOCUMENT_TYPE.Items.Add("Plan");
            fDOCUMENT_TYPE.Items.Add("Amended Plan");
            fDOCUMENT_TYPE.Items.Add("Form");
            fDOCUMENT_TYPE.Items.Add("Amended Form");
            fDOCUMENT_TYPE.Items.Add("Photo");
            fDOCUMENT_TYPE.Items.Add("Amended Photo");
            fDOCUMENT_TYPE.Items.Add("Letter");
            fDOCUMENT_TYPE.Items.Add("SSP");
            fDOCUMENT_TYPE.Items.Add("Other");


            fDOCUMENT_TYPE.SelectedIndexChanged += FDOCUMENT_TYPE_SelectedIndexChanged;
            fSUBMIT_TYPE.SelectedIndex = 0;
            fFOLDER_TYPE.SelectedIndex = 0;
            fDOCUMENT_TYPE.SelectedIndex = 0;


            if (pdfPath != null) axAcroPDF1.src = pdfPath;
            if(data != null) SetUIData(data);
            axAcroPDF1.setPageMode("thumbnails");
            axAcroPDF1.setShowToolbar(false);

            //fDOCUMENT_TYPE.SelectedIndexChanged += new EventHandler(LoadDetails_TextChanged);
            //fATT_SRC.TextChanged += new EventHandler(LoadDetails_TextChanged);
        }

        private void FDOCUMENT_TYPE_SelectedIndexChanged(object sender, EventArgs e)
        {
            fDSN_SUB.Text = fDOCUMENT_TYPE.Text;
            _t.Text = fDOCUMENT_TYPE.Text;
        }

        private void buttonThumbnails_Click(object sender, EventArgs e)
        {
            axAcroPDF1.setPageMode("thumbnails");
            axAcroPDF1.setShowToolbar(false);
        }







        public JObject GetUIData()
        {
            JObject r = new JObject();
            r.Add("fDSN_SUB", fDSN_SUB.Text);
            r.Add("fSUBMIT_TYPE", fSUBMIT_TYPE.Text);
            r.Add("fFORM_TYPE", fFORM_TYPE.Text);
            r.Add("fDOCUMENT_TYPE", fDOCUMENT_TYPE.Text);
            r.Add("fFOLDER_TYPE", fFOLDER_TYPE.Text);
            r.Add("fDOC_DESC", fDOC_DESC.Text);
            r.Add("fFILE_PATH", axAcroPDF1.src);
            return r;
        }

        public void SetUIData(JObject jo)
        {
            fDSN_SUB.Text = jo["fDSN_SUB"].ToString();
            fSUBMIT_TYPE.Text = jo["fSUBMIT_TYPE"].ToString();
            fFORM_TYPE.Text = jo["fFORM_TYPE"].ToString();
            fDOCUMENT_TYPE.Text = jo["fDOCUMENT_TYPE"].ToString();
            fFOLDER_TYPE.Text = jo["fFOLDER_TYPE"].ToString();
            fDOC_DESC.Text = jo["fDOC_DESC"].ToString();
            axAcroPDF1.src = jo["fFILE_PATH"].ToString();

        }


















    }
}

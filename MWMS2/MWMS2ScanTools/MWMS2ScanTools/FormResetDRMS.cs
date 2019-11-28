using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MWMS2ScanTools
{
    public partial class FormResetDRMS : Form
    {
        JObject details;
        public FormResetDRMS(JObject jObject)
        {
            details = jObject;
            InitializeComponent();
            if (details.ContainsKey("CODE")) label7.Text = details["CODE"].ToString();
            if (details.ContainsKey("BD_PORTAL_LOGIN")) label6.Text = details["BD_PORTAL_LOGIN"].ToString();
            if (details.ContainsKey("DSMS_USERNAME")) textBox1.Text = details["DSMS_USERNAME"].ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                MessageBox.Show("Please input DRMS Username.");
                textBox1.Focus();
                return;
            }
            if (textBox2.Text == "")
            {
                MessageBox.Show("Please input DRMS Password.");
                textBox2.Focus();
                return;
            }
            if (textBox2.Text != textBox3.Text) {
                MessageBox.Show("Invalid Confirm Password.");
                textBox3.Focus();
                return;
            }
            try
            {
                Program.docServerKernal.ChangeInfo(textBox1.Text, textBox2.Text);
               // Program.OpenMain();
                this.Close();
            }
            catch(DocServerException ex)
            {
                MessageBox.Show(ex.Data[DATA_KEY.MESSAGE].ToString());
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

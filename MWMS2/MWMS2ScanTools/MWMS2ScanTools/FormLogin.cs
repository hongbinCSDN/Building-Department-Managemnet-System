using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MWMS2ScanTools
{
    public partial class FormLogin : Form
    {
        public FormLogin()
        {
            InitializeComponent();
            //if (details.ContainsKey("CODE")) label7.Text = details["CODE"].ToString();
            //if (details.ContainsKey("BD_PORTAL_LOGIN")) label6.Text = details["BD_PORTAL_LOGIN"].ToString();
            //if (details.ContainsKey("DSMS_USERNAME")) textBox1.Text = details["DSMS_USERNAME"].ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                MessageBox.Show("Please input Username.");
                textBox1.Focus();
                return;
            }
            if (textBox2.Text == "")
            {
                MessageBox.Show("Please input Password.");
                textBox2.Focus();
                return;
            }
            try
            {
                if (radioButton1.Checked)
                {
                    string url = ConfigurationManager.AppSettings["url"];
                    string req = Program.docServerKernal.htmlRequester.ReqText(url,null);
                    string findstr = "name=\"goto\" value=\"";
                    int first = req.IndexOf(findstr) + findstr.Length;
                    int len = req.IndexOf("\"", first)- first;
                    string key = req.Substring(first, len);

                    Dictionary<string, string> parameters = new Dictionary<string, string>();
                    parameters.Add("IDToken1", textBox1.Text);
                    parameters.Add("IDToken2", textBox2.Text);
                    parameters.Add("IDButton", "Log In");
                    parameters.Add("goto", "key");
                    parameters.Add("gotoOnFail", "");
                    parameters.Add("SunQueryParamsString", "");
                    parameters.Add("encoded", "true");
                    parameters.Add("gx_charset", "UTF-8");

                    string req2 = Program.docServerKernal.htmlRequester.ReqText("https://dp2.bd.hksarg:8443/sso/UI/Login", parameters);
                    bool sucesslogin = req2.IndexOf("Please Wait While Redirecting to console") > 0;
                    if (sucesslogin)
                    {
                        string req3 = Program.docServerKernal.htmlRequester.ReqText("https://dp2.bd.hksarg/MWMS2", parameters);
                        bool sucesslogin2 = req3.IndexOf("Buildings Department - Minor Works Management System 2.0") > 0;
                        if(sucesslogin2)
                        {
                            Program.docServerKernal.alive = true;
                            this.Close();
                            Program.OpenMain();
                            return;
                        }
                    }
                    throw DocServerException.Create(null) ;
                }
                else if (radioButton2.Checked)
                {
                    Program.docServerKernal.Login(textBox1.Text, textBox2.Text);
                    this.Close();
                    Program.OpenMain();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Invalid Username or Password.");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

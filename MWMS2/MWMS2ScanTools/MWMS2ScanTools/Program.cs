using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MWMS2ScanTools
{


    static class Program
    {


        public static DocServerKernal docServerKernal;
        public static FormMain mainForm;
        public static FormLogin formLogin;
        //public static DebugWindow dewi;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {            docServerKernal = new DocServerKernal();


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            mainForm = new FormMain();
            formLogin = new FormLogin();
            mainForm.Show();
            formLogin.ShowDialog();
            if(docServerKernal.alive)Application.Run(mainForm);
          
        }

        /*private static void FormLogin_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!docServerKernal.alive) mainForm.Close();
        }*/

        public static void OpenMain()
        {
            mainForm.Show();
        }

        public static void log(string message)
        {
            //dewi.textBox1.AppendText("\r\n" + message);
        }
    }
}

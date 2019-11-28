using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace MWMS2EmailSender
{
    class Program
    {
        static void Main(string[] args)
        {
           
            try
            {
                string CertificatePath = @"cert.p12";
                string CertificatePassword = "Pass1234@";
                string MailServer = "smtpx.cis.hksarg";
                string EmailSender = "bcis_notices@bd.gov.hk";


                using (EntitiesEmail db = new EntitiesEmail())
                {
                    var query = db.SYS_EMAIL_SENDER.Where(x => x.STATUS == "READY");
                    
                    foreach (var item in query)
                    {
                        if (item.RECIPIENT == null || item.SUBJECT == null || item.EMAILCONTENT == null)
                        {
                        }
                        else
                        {
                            string EmailRecipient = item.RECIPIENT;


                            string EmailSubject = item.SUBJECT;
                            string EmailBody = item.EMAILCONTENT;



                            X509Certificate2 EncryptCert = new X509Certificate2(CertificatePath, CertificatePassword);
                            StringBuilder Message = new StringBuilder();
                            Message.AppendLine(EmailBody);
                            MailMessage Msg = new MailMessage();
                            Msg.To.Add(new MailAddress(EmailRecipient));
                            Msg.From = new MailAddress(EmailSender);
                            Msg.Subject = EmailSubject;
                            Msg.IsBodyHtml = true;
                            Msg.Body = EmailBody;

                            SmtpClient SmtpServer = new SmtpClient(MailServer, 587);
                            SmtpServer.ClientCertificates.Add(EncryptCert);

                            SmtpServer.EnableSsl = true;
                            Console.WriteLine("mail Send");

                            SmtpServer.Send(Msg);
                            item.STATUS = "COMPLETED";
                        }
                    
                    }
                    db.SaveChanges();

                }


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}





/*














using System;
using System.Windows.Forms;
using System.Net.Mail;

namespace WindowsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

                mail.From = new MailAddress("your_email_address@gmail.com");
                mail.To.Add("to_address");
                mail.Subject = "Test Mail";
                mail.Body = "This is for testing SMTP mail from GMAIL";

                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential("username", "password");
                SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);
                MessageBox.Show("mail Send");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}













    */
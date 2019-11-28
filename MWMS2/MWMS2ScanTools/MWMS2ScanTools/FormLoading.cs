using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Windows.Forms;
using TwainDotNet;
using TwainDotNet.WinFroms;

namespace MWMS2ScanTools
{
    public partial class FormLoading : Form
    {


        //Twain _twain = null;


        private FormMain formMain;
        public FormLoading(FormMain formMain)
        {
            this.formMain = formMain;
            InitializeComponent();
        }
        private void _twain_ScanningComplete(object sender, ScanningCompleteEventArgs e)
        {
            if(e.Exception != null)
            {
                Scan((Twain)sender);
            }
            else
                Close();
        }

        private void _twain_TransferImage(object sender, TransferImageEventArgs args)
        {
            string fileName = Util.NewUuid();
            if (args.Image != null)
            {
                Bitmap resultImage = args.Image;
                //scannedImage.Add(resultImage);
                Bitmap newBitmap = new Bitmap(resultImage);
                //resultImage.Dispose();
                //resultImage = null;
                MemoryStream ms = new MemoryStream();
                newBitmap.Save(ms, ImageFormat.Jpeg);
                if (!Directory.Exists("temp")) Directory.CreateDirectory("temp");
                FileStream file = new FileStream("temp/" + fileName, FileMode.Create, FileAccess.Write);
                ms.WriteTo(file);
                file.Close();
                ms.Close();





                //newBitmap.Save("temp/" + fileName, ImageFormat.Jpeg);
                //ScannedPath.Add("temp/" + fileName);
                formMain.AddImage("temp/" + fileName); 
            }
        }

        private void FormLoading_Load(object sender, EventArgs e)
        {
            //TwainDotNet.TwainNative.ConditionCode

            //Scan(_twain);
            

        }
        private void Scan(Twain _twain)
        {
            var sourceList = _twain.SourceNames;
            ScanSettings settings = new ScanSettings()
            {
                ShowTwainUI = false,
                ShouldTransferAllPages = true,
                Resolution = ResolutionSettings.Photocopier,
                UseDuplex = true,
                Page = PageSettings.Default
                ,
                UseDocumentFeeder = true
                ,
                UseAutoScanCache = true
                ,
                AbortWhenNoPaperDetectable = true
                ,
                ShowProgressIndicatorUI = false
                ,
                UseAutoFeeder = true
                //, TransferCount = 5

            };
            _twain.SelectSource("HP Scanjet N9120 TWAIN");
            try
            {
                _twain.StartScanning(settings);
            }
            catch (FeederEmptyException )
            {
                //Close();
                //Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                //Hide();
                Close();
            }
        }
    }
}

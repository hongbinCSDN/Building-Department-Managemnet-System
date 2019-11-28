using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;

namespace MWMS2ScanTools
{
    public partial class ControlEdit : UserControl
    {
        private Bitmap originalBitmap = null;
        private Bitmap previewBitmap = null;
        private Bitmap resultBitmap = null;
        public ControlEdit(Bitmap bitmap)
        {
            InitializeComponent();

            originalBitmap = bitmap;
            //previewBitmap = originalBitmap.CopyToSquareCanvas(picPreview.Width);
            picPreview.Image = previewBitmap;

            ApplyFilter(true);
        }
        private void ApplyFilter(bool preview)
        {
            if (previewBitmap == null)
            {
                return;
            }

            if (preview == true)
            {
                //picPreview.Image = previewBitmap.Contrast(trcThreshold.Value);
            }
            else
            {
                //resultBitmap = originalBitmap.Contrast(trcThreshold.Value);
            }
        }
        private void btnSaveNewImage_Click(object sender, EventArgs e)
        {
            ApplyFilter(false);

            if (resultBitmap != null)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Title = "Specify a file name and file path";
                sfd.Filter = "Png Images(*.png)|*.png|Jpeg Images(*.jpg)|*.jpg";
                sfd.Filter += "|Bitmap Images(*.bmp)|*.bmp";

                if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string fileExtension = Path.GetExtension(sfd.FileName).ToUpper();
                    ImageFormat imgFormat = ImageFormat.Png;

                    if (fileExtension == "BMP")
                    {
                        imgFormat = ImageFormat.Bmp;
                    }
                    else if (fileExtension == "JPG")
                    {
                        imgFormat = ImageFormat.Jpeg;
                    }

                    StreamWriter streamWriter = new StreamWriter(sfd.FileName, false);
                    resultBitmap.Save(streamWriter.BaseStream, imgFormat);
                    streamWriter.Flush();
                    streamWriter.Close();

                    resultBitmap = null;
                }
            }
        }
    }
}

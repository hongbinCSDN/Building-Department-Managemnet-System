using PdfSharp.Pdf;
using PdfSharp.Pdf.Advanced;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Media.Imaging;

namespace PScan
{
    class ImageFile
    {
        private static Size thumbsMaxSize = new Size(64, 64);
        public bool Valid { get; set; } = false;
        public System.IO.FileInfo FileInfo { get; set; }
        public Image Image { get; set; }
        public PdfDocument PdfDocument { get; set; }
        public BitmapImage Thumb { get; set; }
        public string FileType;
        public bool loadThu;
        public static Image StaticImage = new Bitmap(64, 64);

        public ImageFile(string path, string fileType, bool loadThu)
        {
            FileInfo = new System.IO.FileInfo(path);
            this.loadThu = loadThu;
            this.FileType = fileType;
            loadImage();
        }
        
        private void loadImage()
        {/*
            if (!loadThu) Thumb = StaticImage;
            else
            {
                if ("JPG" == FileType)
                {

                    using (var bmpTemp = new Bitmap(FileInfo.FullName))
                    {
                        Image = new Bitmap(bmpTemp);
                    }

                    imageFile.Thumb.Save(memoryStream, imageFile.Thumb.RawFormat);
                    var bmp = new BitmapImage();
                    bmp.BeginInit();
                    bmp.StreamSource = memoryStream;
                    bmp.EndInit();

                    _listViewFileSource.Add(new ListViewFileItem() { Title = "test", ImageData = bmp });


                    Thumb = Image.GetThumbnailImage(64, 64, () => false, IntPtr.Zero);

                }
                else if ("PDF" == FileType)
                {
                    //const string filename = "../../../../../PDFs/SomeLayout.pdf";

                    PdfDocument = PdfReader.Open(FileInfo.FullName);
                    PdfDocument.Close();


                    Image = new Bitmap(64, 64);
                    Thumb = Image.GetThumbnailImage(64, 64, () => false, IntPtr.Zero);
                }
            }*/
            //}
        }

        MemoryStream ExportImage(PdfDictionary image, ref int count)
        {
            string filter = image.Elements.GetName("/Filter");
            if (filter == "/DCTDecode")
            {
                return ExportJpegImage(image);
            }
            else if (filter == "/DCTDecode")
            {
                return ExportAsPngImage(image);
            }
            else return null;

        }
        MemoryStream ExportAsPngImage(PdfDictionary image)
        {
            int width = image.Elements.GetInteger(PdfImage.Keys.Width);
            int height = image.Elements.GetInteger(PdfImage.Keys.Height);
            int bitsPerComponent = image.Elements.GetInteger(PdfImage.Keys.BitsPerComponent);
            return null;
            // TODO: You can put the code here that converts vom PDF internal image format to a Windows bitmap
            // and use GDI+ to save it in PNG format.
            // It is the work of a day or two for the most important formats. Take a look at the file
            // PdfSharp.Pdf.Advanced/PdfImage.cs to see how we create the PDF image formats.
            // We don't need that feature at the moment and therefore will not implement it.
            // If you write the code for exporting images I would be pleased to publish it in a future release
            // of PDFsharp.
        }
        MemoryStream ExportJpegImage(PdfDictionary image)
        {
            // Fortunately JPEG has native support in PDF and exporting an image is just writing the stream to a file.
            byte[] stream = image.Stream.Value;
            //FileStream fs = new FileStream(String.Format("Image{0}.jpeg", count++), FileMode.Create, FileAccess.Write);
            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);
            bw.Write(stream);
            bw.Close();
            ms.Close();
            return ms;
        }

    }
}

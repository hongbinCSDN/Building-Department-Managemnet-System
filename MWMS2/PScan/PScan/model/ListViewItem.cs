using PScan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

public class ListViewFileItem
{
    private string _Title;
    public string Title
    {
        get { return this._Title; }
        set { this._Title = value; }
    }

    private BitmapImage _ImageData;
    public BitmapImage ImageData
    {
        get { return this._ImageData; }
    }
    private string _imagePath;
    public string ImagePath
    {
        set
        {
            _imagePath = value;
            if (Util.isJPG(value))
            {
                Uri r = new Uri(value);
                _ImageData = new BitmapImage(r);
                if (r.Segments != null && r.Segments.Length > 0)
                {
                    Title = r.Segments[r.Segments.Length - 1];
                }

            }
            else if (Util.isPDF(value))
            {
                Uri r = new Uri(value);
                if (r.Segments != null && r.Segments.Length > 0)
                {
                    Title = r.Segments[r.Segments.Length - 1];
                }
            }
            else throw new Exception("FileType ");
        }
        get { return _imagePath; }
    }

}
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySelfie.Scraper
{
    public interface IAzureBlobRepositoy
    {
        Uri AddImageFromURL(string url, string fileName);
        Uri AddImage(Image img, string fileName);
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoTopWar.Common.Utilities
{
    public static class ImageUtil
    {
        public static void SaveImageToFile(Bitmap drawImage, string path)
        {
            drawImage.Save(path, ImageFormat.Png);
        }
    }
}

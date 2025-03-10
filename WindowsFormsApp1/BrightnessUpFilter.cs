using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    class BrightnessUpFilter : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            Color sourceColor = sourceImage.GetPixel(x, y);
            int r = Clamp(sourceColor.R + 20, 0, 255);
            int g = Clamp(sourceColor.G + 20, 0, 255);
            int b = Clamp(sourceColor.B + 20, 0, 255);
            Color resultColor = Color.FromArgb(r, g, b);
            return resultColor;
        }
    }
}

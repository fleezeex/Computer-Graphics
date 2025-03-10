using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    class SepiaFilter : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            Color sourceColor = sourceImage.GetPixel(x, y);
            int k = 20;
            int intensity = Convert.ToInt32(0.299 * sourceColor.R + 0.587 * sourceColor.G + 0.114 * sourceColor.B);
            int r = Clamp(intensity + 2 * k, 0, 255);
            int g = Clamp(intensity + Convert.ToInt32(0.5 * k), 0, 255);
            int b = Clamp(intensity - 1 * k, 0, 255);
            Color resultColor = Color.FromArgb(r, g, b);
            return resultColor;
        }
    }
}

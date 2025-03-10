using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Xml.Schema;
using System.ComponentModel;

namespace WindowsFormsApp1
{
    abstract class Filters
    {
        public Bitmap processImage (Bitmap sourceImage)
        {
            Bitmap resultImage = new Bitmap(sourceImage.Width, sourceImage.Height);
            for (int i = 0; i < sourceImage.Width; i++)
            {
                for (int j = 0; j < sourceImage.Height; j++)
                {
                    resultImage.SetPixel(i, j, calculateNewPixelColor(sourceImage, i, j));
                }
            }
            return resultImage;
        }

        protected abstract Color calculateNewPixelColor(Bitmap sourceImage, int i, int j);


        public int Clamp (int value, int min, int max)
        {
            if (value < min) { return min; }
            if (value > max) { return max; }
            else { return value; }
        }

        
    }
}

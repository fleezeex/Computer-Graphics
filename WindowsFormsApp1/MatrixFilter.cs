using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    class MatrixFilter : Filters
    {
        protected float[,] kernel = null; // float[,] - объявление двумерного массива в c#; float[,,,] - четырёхмерный и т. д.
        protected MatrixFilter() { }
        
        protected MatrixFilter(float[,] kernel)
        {
            this.kernel = kernel;
        }

        protected override Color calculateNewPixelColor(Bitmap sourceImage, int i, int j)
        {
            int radiusX = kernel.GetLength(0) / 2;
            int radiusY = kernel.GetLength(1) / 2;
        }
    }
}

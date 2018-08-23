using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class ImageColor : IDisposable
    {
        public enum Colors { Blue = 0, Green = 1, Red = 2 };

        private ImageWrapper<Bgr, byte> _inputImage;

        public double BlueTone;
        public double GreenTone;
        public double RedTone;

        public ImageColor(ImageWrapper<Bgr, byte> image)
        {
            _inputImage = image.Copy();
        }

        public void QualifityPhotoTone()
        {
            ProgressManager.AddSteps(_inputImage.Image.Height/100);

            double blue = 0, green = 0, red = 0, sum = 1;
            double sizeOfImage = _inputImage.Image.Height * _inputImage.Image.Width;

            for (int x = 0; x < _inputImage.Image.Height; x++)
            {
                for (int y = 0; y < _inputImage.Image.Width; y++)
                {
                    blue += _inputImage.Image.Data[x, y, (int)Colors.Blue];
                    green += _inputImage.Image.Data[x, y, (int)Colors.Green];
                    red += _inputImage.Image.Data[x, y, (int)Colors.Red];
                }

                if (x % 100 == 0)
                    ProgressManager.DoStep();
            }

            sum = blue + green + red;
            BlueTone = blue / sum;
            GreenTone = green / sum;
            RedTone = red / sum;           
        }

        public void Dispose()
        {
            _inputImage.Dispose();
        }
    }
}

using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class ImageColor
    {
        public enum PhotoType { Sepia, HalfSepia, Gray };
        public enum Colors { Blue = 0, Green = 1, Red = 2 };

        public double BlueTone;
        public double GreenTone;
        public double RedTone;

        public void QualifityPhotoTone(ImageWrapper<Bgr, byte> image)
        {
            using (image)
            {
                double blue = 0, green = 0, red = 0, sum = 1;
                double sizeOfImage = image.Image.Height * image.Image.Width;

                for (int x = 0; x < image.Image.Height; x++)
                {
                    for (int y = 0; y < image.Image.Width; y++)
                    {
                        blue += image.Image.Data[x, y, (int)Colors.Blue];
                        green += image.Image.Data[x, y, (int)Colors.Green];
                        red += image.Image.Data[x, y, (int)Colors.Red];
                    }
                }
                sum = blue + green + red;
                BlueTone = blue / sizeOfImage;
                GreenTone = green / sizeOfImage;
                RedTone = red / sizeOfImage;
            }
        }
    }
}

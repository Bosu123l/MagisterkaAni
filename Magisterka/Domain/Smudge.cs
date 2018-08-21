using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using static Domain.ImageColor;

namespace Domain
{
    public class Smudge: IDisposable
    {
        private ImageWrapper<Bgr, byte> _image;

        public Smudge(ImageWrapper<Bgr, byte> image)
        {
            _image = image.Copy();
        }      

        public ImageWrapper<Bgr, byte> OtherColorDetector(double blue, double green, double red)        
        {
            ImageWrapper<Bgr, byte> mask = _image.Copy();           

            double abs = 0.25;

            double blueMin = Math.Round(blue - abs * blue, 2);
            double blueMax = Math.Round(blue + abs * blue, 2);

            double greenMin = Math.Round(green - abs*green, 2);
            double greenMax = Math.Round(green + abs * green, 2);

            double redMin = Math.Round(red - abs*red, 2);
            double redMax = Math.Round(red + abs*red, 2);

            double blueCont = 0.0;
            double greenCont = 0.0;
            double redCont = 0.0;
            double sum;

            int steps = _image.Image.Height / 100;
            ProgressManager.AddSteps(steps);

            for (int x = 0; x < _image.Image.Height; x++)
            {
                for (int y = 0; y < _image.Image.Width; y++)
                {                    
                    blue = _image.Image.Data[x, y, (int)Colors.Blue];
                    green = _image.Image.Data[x, y, (int)Colors.Green]; 
                    red = _image.Image.Data[x, y, (int)Colors.Red];

                    sum = blue + green + red;

                    //try
                    //{
                    //    blueCont = blue / sum;
                    //    greenCont =  green/sum;
                    //    redCont =  red/sum;
                    //}
                    //catch (DivideByZeroException)
                    //{
                    //    sum = 1;
                    //    continue;
                    //}

                    if(greenCont<greenMin || greenCont>greenMax || redCont<redMin || redCont>redMax || blueCont < blueMin || blueCont > blueMax)
                    {
                        mask.Image.Data[x, y, (int)Colors.Blue] = 0;
                        mask.Image.Data[x, y, (int)Colors.Green] = 0;
                        mask.Image.Data[x, y, (int)Colors.Red] = 255;
                    }
                }
                if (x % 100 == 0)
                    ProgressManager.DoStep();
            }
            return mask;
        }

        public void Dispose()
        {
            _image.Dispose();
        }
    }
}
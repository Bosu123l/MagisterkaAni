using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain
{
    public class Smudge
    {
        private Image<Bgr, byte> _image;

        public Smudge(Image<Bgr, byte> image)
        {
            _image = image;
        }

        public Image<Bgr, byte> OtherColorDetector(int threshold = 40)
        //prog dla szarosci=22, prog dla polsepii=40, prog dla sepii=50
        {
            List<double> data = new List<double>();

            Image<Gray, byte> gray = _image.Convert<Gray, byte>();
            Image<Bgr, byte> retImage = _image;
            double max = 0;
            double avg = 0;
            double mismatch = 0;
            int count = 0;

            //for (int i = 2000; i < _image.Height; i++)
            //{
            //    for (int j = 2000; j < _image.Width; j++)
            //    {
            //        int a, b, c;

            //        a=Math.Abs(_image.Data[i, j, 0]- _image.Data[i, j, 1]);
            //        b = Math.Abs(_image.Data[i, j, 1] - _image.Data[i, j, 2]);
            //        c = Math.Abs(_image.Data[i, j, 2] - _image.Data[i, j, 0]);
            //        double tmp = (double) (a + b + c)/3.0;
            //        data.Add(tmp);
            //        if (max < tmp)
            //            max = tmp;
            //        avg += tmp;

            //        if (tmp > 20.0)
            //            tmp = max;
            //    }
            //}

            //avg = avg/data.Count();

            for (int i = 0; i < _image.Height; i++)
            {
                for (int j = 0; j < _image.Width; j++)
                {
                    bool p = false;
                    for (int k = 0; k < 3; k++)
                    {
                        double tmp = Math.Abs(_image.Data[i, j, k] - gray.Data[i, j, 0]);

                        count++;
                        if (tmp > max)
                            max = tmp;

                        if (tmp > threshold)
                        {
                            p = true;
                            mismatch++;
                        }

                        avg += tmp;
                    }

                    if (p)
                    {
                        retImage.Data[i, j, 0] = 0;
                        retImage.Data[i, j, 1] = 0;
                        retImage.Data[i, j, 2] = 255;
                    }
                }
            }

            mismatch = mismatch / (count>0 ? 1 :count);
            avg = avg / (count > 0 ? 1 : count);

            return retImage;
        }
    }
}
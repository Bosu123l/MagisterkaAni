using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Domain
{
    public class DustRemoval
    {
        private Image<Bgr, byte> image;
        private float _minFactor=0.0000002f;
        private float _maxFactor=0.0002f;

        public DustRemoval(Image<Bgr, byte> im)
        {
            if (im != null)
                image = im;
        }

        public Image<Bgr, byte> RemoveDust()
        {
            Image<Gray, float> grayImage = image.Convert<Gray, float>();
            Image<Gray, float>laplaceImge= grayImage.Laplace(9);

            //cleaned = cleaned.Sobel(1, 0, 9);           

            DenseHistogram histogram = new DenseHistogram(256, new RangeF(0.0f, 255.0f));
            histogram.Calculate(new Image<Gray, byte>[] { laplaceImge.Convert<Gray, byte>() }, false, null);

            List<float> hist = new List<float>(histogram.GetBinValues());
            int pixelsSum=(int)hist.Sum(x => x);

            int a1 = hist.IndexOf(hist.FirstOrDefault(x => x > pixelsSum * _minFactor));
            int a2 = hist.IndexOf(hist.FirstOrDefault(x => x > pixelsSum * _maxFactor));

            int b1 = hist.LastIndexOf(hist.LastOrDefault(x => x > pixelsSum * _minFactor));
            int b2 = hist.LastIndexOf(hist.LastOrDefault(x => x > pixelsSum * _maxFactor));

            Image<Gray, byte>cannyImg1 = laplaceImge.Convert<Gray, byte>().Canny(a1, a2);
            Image<Gray, byte>cannyImg2 = laplaceImge.Convert<Gray, byte>().Canny(b1, b2);

            Image<Gray, byte> cannyImg = cannyImg1.Add(cannyImg2);


            Image<Gray, byte> dilatedImage1 = cannyImg.Dilate(1);
            Image<Gray, byte> dilatedImage2 = cannyImg.Dilate(7);


            Image<Gray, byte> dilatedDelta = dilatedImage2.Sub(dilatedImage1);

            Image<Gray, float> backgroundPatternImage = grayImage.SmoothBlur(30,30);
            //backgroundPatternImage = backgroundPatternImage.SmoothMedian(5);
            //backgroundPatternImage = backgroundPatternImage.SmoothMedian(5);

            Image<Gray, float>patchImage=grayImage.Mul(dilatedImage2.Convert<Gray, float>());

            //Image<Bgr, byte> imout = new Image<Bgr, byte>(grayImage.Convert<Bgr,byte>());
            Image<Gray, byte> imageOutput = dilatedImage1.Convert<Gray, byte>().ThresholdBinary(new Gray(100), new Gray(255));
            Emgu.CV.Util.VectorOfVectorOfPoint contours = new Emgu.CV.Util.VectorOfVectorOfPoint();
            Mat hier = new Mat();

            CvInvoke.FindContours(imageOutput, contours, hier, Emgu.CV.CvEnum.RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple);
            CvInvoke.DrawContours(image, contours, -1, new MCvScalar(0, 255, 255));
            



            //List<Point> whitePoints = new List<Point>();

            //for (int h=0; h<cannyImg.Height; h++)
            //{
            //    for(int w=0; w<cannyImg.Width; w++)
            //    {
            //        if(cannyImg.Data[h, w, 0] == 255)
            //        {
            //            whitePoints.Add(new Point(h, w));
            //        }                       
            //    }
            //}


            //LineSegment2D[][] edges = cannyImg.HoughLinesBinary(0.5,Math.PI /100, 2, 2, 0);
            //List<Point> points = new List<Point>();

            //foreach (var p in edges[0])
            //{
            //    image.Draw(p, new Bgr(0,255,0), 3);
            //}
            //cleaned= (cleaned.Convert<Gray, byte>().Mul(3)).Convert<Gray, float>();

            //var bwImage = cleaned.ThresholdBinary(new Gray(250), new Gray(255));
            //return image;
            return image.Convert<Bgr, byte>();

            //Image<Gray, byte> grey_org = image.Convert<Gray, byte>();
            //Image<Gray, byte> erosion = grey_org.Erode(2);
            //Image<Gray, byte> diff = grey_org.Sub(erosion);
            //Image<Gray, byte> temp = diff.Not();
            //Image<Gray, byte> temp2 = grey_org.Erode(3);

            //temp2.Sub(temp)._EqualizeHist();
            // temp.Erode(5);

            //cleaned = temp2;
            //cleaned=grey_org.SmoothGaussian(3);

            //Image<Gray, byte> grey_org_black = cleaned.Not();
            //Image<Gray, byte> erosion2 = grey_org_black.Erode(2);
            //Image<Gray, byte> diff2 = grey_org.Sub(erosion);
            //Image<Gray, byte> temp3 = diff.Not();
            //Image<Gray, byte> temp4 = grey_org.Erode(10);

            //temp4.Sub(temp3)._EqualizeHist();

            //cleaned=diff;

            //for (int i = 0; i < 5; i++)
            //{
            //    cleaned._Erode(5);
            //    cleaned._Dilate(2);
            //}

            //cleaned._Not();

            //for (int i = 0; i <5; i++)
            //{
            //    cleaned._Erode(5);
            //    cleaned._Dilate(2);
            //}

            //cleaned._Not();
            //cleaned._SmoothGaussian(3);

            //cleaned.Sobel(2, 0, 5);





        }
    }
}

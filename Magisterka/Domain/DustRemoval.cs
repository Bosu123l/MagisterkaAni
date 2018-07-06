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
        private float _minFactor = 0.0000002f;
        private float _maxFactor = 0.0002f;
        private int _maxSizeOfDustContour = 100;

        Image<Bgr, byte> returnImage;

        public DustRemoval(Image<Bgr, byte> im)
        {
            if (im != null)
                image = im;
        }

        public Image<Bgr, byte> RemoveDust()
        {
            Image<Gray, float> grayImage = image.Convert<Gray, float>();
            Image<Gray, float> laplaceImge = grayImage.Laplace(9);

            //cleaned = cleaned.Sobel(1, 0, 9);           

            DenseHistogram histogram = new DenseHistogram(256, new RangeF(0.0f, 255.0f));
            histogram.Calculate(new Image<Gray, byte>[] { laplaceImge.Convert<Gray, byte>() }, false, null);

            List<float> hist = new List<float>(histogram.GetBinValues());
            int pixelsSum = (int)hist.Sum(x => x);

            int a1 = hist.IndexOf(hist.FirstOrDefault(x => x > pixelsSum * _minFactor));
            int a2 = hist.IndexOf(hist.FirstOrDefault(x => x > pixelsSum * _maxFactor));

            int b1 = hist.LastIndexOf(hist.LastOrDefault(x => x > pixelsSum * _minFactor));
            int b2 = hist.LastIndexOf(hist.LastOrDefault(x => x > pixelsSum * _maxFactor));

            Image<Gray, byte> cannyImg1 = laplaceImge.Convert<Gray, byte>().Canny(a1, a2);
            Image<Gray, byte> cannyImg2 = laplaceImge.Convert<Gray, byte>().Canny(b1, b2);

            Image<Gray, byte> cannyImg = cannyImg1.Add(cannyImg2);


            Image<Gray, byte> dilatedImage1 = Morpho(cannyImg);//cannyImg.Dilate(1);
            Image<Gray, byte> dilatedImage2 = Morpho(dilatedImage1);//cannyImg.Dilate(7);
            Image<Gray, byte> dilatedImage3 = Morpho(dilatedImage2);

            //Image<Gray, byte> dilatedDelta = dilatedImage2.Sub(dilatedImage1);

            //Image<Gray, float> patchImage = grayImage.Mul(dilatedImage2.Convert<Gray, float>());

            //Image<Bgr, byte> imout = new Image<Bgr, byte>(grayImage.Convert<Bgr,byte>());
            Image<Gray, byte> imageOutput = dilatedImage3.Convert<Gray, byte>().ThresholdBinary(new Gray(100), new Gray(255));
            Emgu.CV.Util.VectorOfVectorOfPoint contours = new Emgu.CV.Util.VectorOfVectorOfPoint();
            Mat hier = new Mat();

            CvInvoke.FindContours(imageOutput, contours, hier, Emgu.CV.CvEnum.RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple);

            // ,                                                                           RemoveDustParticles(contours, image, dilatedImage2);
           // image = image.Convert<Gray, byte>().ThresholdBinary(new Gray(100), new Gray(255)).Convert<Bgr, byte>();
            CvInvoke.DrawContours(image, contours, -1, new MCvScalar(255, 0, 255));
            
            return image.Convert<Bgr, byte>();
        }

        public Image<Gray, byte> Morpho(Image<Gray, byte> input)
        {
            Mat kernel = CvInvoke.GetStructuringElement(Emgu.CV.CvEnum.ElementShape.Rectangle, new Size(5, 5), new Point(-1, -1));
            return input.MorphologyEx(Emgu.CV.CvEnum.MorphOp.Dilate, kernel, new Point(-1, -1), 2, Emgu.CV.CvEnum.BorderType.Default, new MCvScalar(1.0));
        }

        public Image<Bgr, byte> Erode()
        {
            Mat kernel = CvInvoke.GetStructuringElement(Emgu.CV.CvEnum.ElementShape.Rectangle, new Size(5, 5), new Point(-1, -1));
            return image.MorphologyEx(Emgu.CV.CvEnum.MorphOp.Erode, kernel, new Point(-1, -1), 2, Emgu.CV.CvEnum.BorderType.Default, new MCvScalar(1.0));
        }

        private void RemoveDustParticles(Emgu.CV.Util.VectorOfVectorOfPoint defectsContours, Image<Bgr, byte> orgImage, Image<Gray, byte> dustMask)
        {
            var defects = defectsContours.ToArrayOfArray().OrderByDescending(item => item.Count()).ToArray();         

            for(int i = 0; i < defects.Count() && defects[i].Count() > _maxSizeOfDustContour; i++)
            {
                RemoveSpeckOfDust(defects[i], orgImage, dustMask);
            }
        }
        private void RemoveSpeckOfDust(Point[] defectContour, Image<Bgr, byte> orgImage, Image<Gray, byte> dustMask)
        {
            Rectangle roi;
            Image<Gray, byte> dustMaskNegative;
            Image<Bgr, byte> temp;
            Image<Bgr, byte> tempMask=new Image<Bgr, byte>(dustMask.Bitmap);

            roi = GetRectangleFromContour(defectContour);
            orgImage.ROI = roi;
            dustMask.ROI = roi;
            tempMask.ROI = roi;
            dustMaskNegative = dustMask.Not();

            temp = tempMask.CopyBlank();
            tempMask.CopyTo(temp);
            tempMask.ROI = Rectangle.Empty;

            temp.CopyTo(orgImage);
            CvInvoke.cvCopy(temp, orgImage, IntPtr.Zero);

            //Image mask = new Image<> 


            orgImage.ROI = Rectangle.Empty;
            dustMask.ROI = Rectangle.Empty;

            dustMaskNegative.Dispose();
            temp.Dispose();
            tempMask.Dispose();
        }

        private Rectangle GetRectangleFromContour(Point[] points)
        {
            int x1, x2, y1, y2;
            int width, height;
            int abs = 0;

            x1 = points.Min(x => x.X);
            y1 = points.Min(y => y.Y);
            x2 = points.Max(x => x.X);
            y2 = points.Max(y => y.Y);

            x1 = (x1 - abs) < 0 ? 0 : (x1 - abs);
            y1 = (y1 - abs) < 0 ? 0 : (y1 - abs);
            x2 = (x2 + abs) > image.Width ? image.Width : (x2 + abs);
            y2 = (y2 + abs) > image.Height ? image.Height : (y2 + abs);

            width = Math.Abs(x2 - x1);
            height = Math.Abs(y2 - y1);

            return new Rectangle(x1, y1, width, height);
        }
    }
}

using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.Drawing;

namespace Domain
{
    public class MorphologicalProcessing
    {
        public static Image<Bgr, byte> Dilate(Image<Bgr, byte> input, Size ksize, int iterations = 1)
        {
            Mat kernel = CvInvoke.GetStructuringElement(ElementShape.Ellipse, ksize, new Point(-1, -1));
            return input.MorphologyEx(MorphOp.Dilate, kernel, new Point(-1, -1), iterations, BorderType.Default, new MCvScalar(1.0));
        }
        public static Image<Gray, byte> Dilate(Image<Gray, byte> input, Size ksize, int iterations = 1)
        {
            Mat kernel = CvInvoke.GetStructuringElement(ElementShape.Ellipse, ksize, new Point(-1, -1));
            return input.MorphologyEx(MorphOp.Dilate, kernel, new Point(-1, -1), iterations, BorderType.Default, new MCvScalar(1.0));
        }
        public static Image<Bgr, byte> Erode(Image<Bgr, byte> input, Size ksize, int iterations = 1)
        {
            Mat kernel = CvInvoke.GetStructuringElement(ElementShape.Ellipse, ksize, new Point(-1, -1));
            return input.MorphologyEx(MorphOp.Erode, kernel, new Point(-1, -1), iterations, BorderType.Default, new MCvScalar(1.0));
        }

        #region Binaryzation
        public static Image<Gray, byte> GeneralImageBinary(Image<Bgr,byte>image)
        {         
            Gray avg = image.Convert<Gray,byte>().GetAverage();

            using (Image<Bgr, byte> bluredImage = image.CopyBlank())
            {
                CvInvoke.MedianBlur(image, bluredImage, 31);
                using (Image<Gray, byte> mask = bluredImage.Copy().Convert<Gray, byte>())
                {
                    return CreateBinaryImage(mask, (int)avg.Intensity);
                }
            }                
        }

        public static Image<Gray, byte> CreateBinaryImage(Image<Bgr, byte> image, int threashold=1)
        {
            return image.Convert<Gray, byte>().ThresholdBinary(new Gray(threashold), new Gray(255));
        }

        public static Image<Gray, byte> CreateBinaryImage(Image<Gray, byte> image, int threashold = 1)
        {
            return image.ThresholdBinary(new Gray(threashold), new Gray(255));
        }
        public static Image<Gray, byte> GenerateBinaryImageNegative(Image<Gray, byte> image)
        {
            return image.Not();
        }
        public static Image<Gray, byte> CreateMaskFromPoints(Image<Gray, byte> image, Point[][] conturMatrix)
        {
            Image<Gray, byte> mask = image.CopyBlank();

            using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint(conturMatrix))
            {
                CvInvoke.FillPoly(mask, contours, new MCvScalar(255, 255, 255), LineType.AntiAlias);
                mask._Dilate(2);
                return mask;
            }
        }
        #endregion Binaryzation
        public static Image<Bgr, byte> MultipleMaskAndImage(Image<Bgr, byte> image, Image<Gray, byte> mask)
        {
            return image.Convert<Bgr, float>().Mul(mask.Convert<Bgr, float>()).Convert<Bgr, byte>();
        }
        public static Image<Bgr, byte> MultipleImages(Image<Bgr, byte> image1, Image<Bgr, byte> image2)
        {
            return image1.Convert<Bgr, float>().Mul(image2.Convert<Bgr, float>()).Convert<Bgr, byte>();
        }
        public static Image<Gray, byte> MultipleImages(Image<Gray, byte> image1, Image<Gray, byte> image2)
        {
            return image1.Convert<Gray, float>().Mul(image2.Convert<Gray, float>()).Convert<Gray, byte>();
        }       

        public static Image<Bgr, byte> CombineTwoImages(Image<Bgr, byte> image, Image<Bgr, byte> imagePattern, Image<Gray, byte> mask, bool dilate=true)
        {
            ProgressManager.AddSteps(4);
            if(dilate)
            {
                mask = Dilate(mask, new Size(2, 2), 2);
            }

            //imagePattern.Image = imagePattern.Image.SmoothBlur(3, 3);

            ProgressManager.DoStep();
            using (Image<Bgr, byte> image1 = MultipleMaskAndImage(imagePattern, mask))
            {
                ProgressManager.DoStep();
                using (Image<Bgr, byte> image2 = MultipleMaskAndImage(image, GenerateBinaryImageNegative(mask)))
                {
                    ProgressManager.DoStep();
                    Image<Bgr, byte> outImage = image1.Add(image2);

                    ProgressManager.DoStep();
                    return outImage;                   
                }
            }
        }
    }
}

using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System.Drawing;

namespace Domain
{
    public class MorphologicalProcessing
    {
        public static ImageWrapper<Bgr, byte> Dilate(ImageWrapper<Bgr, byte> input, Size ksize, int iterations = 1)
        {
            Mat kernel = CvInvoke.GetStructuringElement(ElementShape.Ellipse, ksize, new Point(-1, -1));
            return input.MorphologyEx(MorphOp.Dilate, kernel, new Point(-1, -1), iterations, BorderType.Default, new MCvScalar(1.0));
        }
        public static ImageWrapper<Gray, byte> Dilate(ImageWrapper<Gray, byte> input, Size ksize, int iterations = 1)
        {
            using (Mat kernel = CvInvoke.GetStructuringElement(ElementShape.Ellipse, ksize, new Point(-1, -1)))
            {
                return input.MorphologyEx(MorphOp.Dilate, kernel, new Point(-1, -1), iterations, BorderType.Default, new MCvScalar(1.0));
            }             
        }
        public static ImageWrapper<Bgr, byte> Erode(ImageWrapper<Bgr, byte> input, Size ksize, int iterations = 1)
        {
            using (Mat kernel = CvInvoke.GetStructuringElement(ElementShape.Ellipse, ksize, new Point(-1, -1)))
            {
                return input.MorphologyEx(MorphOp.Erode, kernel, new Point(-1, -1), iterations, BorderType.Default, new MCvScalar(1.0));
            }               
        }

        public static ImageWrapper<Gray, byte> CreateBinaryImage(ImageWrapper<Bgr, byte> image, int threashold)
        {
            return image.Convert<Gray, byte>().ThresholdBinary(new Gray(threashold), new Gray(255));
        }
        public static ImageWrapper<Gray, byte> GenerateBinaryImageNegative(ImageWrapper<Gray, byte> image)
        {
            return image.Not();
        }
        public static ImageWrapper<Gray,byte>CreateMaskFromPoints(ImageWrapper<Gray, byte> image, Point[][] conturMatrix)
        {
            using (ImageWrapper<Gray, byte> mask = image.CopyBlank())
            {
                VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint(conturMatrix);
                CvInvoke.FillPoly(mask.Image, contours, new MCvScalar(255, 255, 255), Emgu.CV.CvEnum.LineType.AntiAlias);
                contours.Dispose();
                return mask;
            }               
        }
        public static ImageWrapper<Bgr, byte> MultipleMaskAndImage(ImageWrapper<Bgr, byte> image, ImageWrapper<Gray, byte> mask)
        {
            return image.Convert<Bgr, float>().Mul(mask.Convert<Bgr, float>()).Convert<Bgr, byte>();
        }
        public static ImageWrapper<Bgr,byte> MultipleImages(ImageWrapper<Bgr, byte> image1, ImageWrapper<Bgr, byte> image2)
        {
            return image1.Convert<Bgr, float>().Mul(image2.Convert<Bgr, float>()).Convert<Bgr, byte>();
        }
        public static ImageWrapper<Gray, byte> MultipleImages(ImageWrapper<Gray, byte> image1, ImageWrapper<Gray, byte> image2)
        {
            return image1.Convert<Gray, float>().Mul(image2.Convert<Gray, float>()).Convert<Gray, byte>();
        }
        public static ImageWrapper<Bgr, byte> CombineTwoImages(ImageWrapper<Bgr, byte> image, ImageWrapper<Bgr, byte> imagePattern, ImageWrapper<Gray, byte> mask)
        {
            mask = Dilate(mask, new Size(2, 2), 1);
            using (ImageWrapper<Bgr, byte> image1 = MultipleMaskAndImage(imagePattern, mask))
            {
                using (ImageWrapper<Bgr, byte> image2 = MultipleMaskAndImage(image, GenerateBinaryImageNegative(mask)))
                {
                    using (ImageWrapper<Bgr, byte> outImage = image1.Add(image2))
                    {
                        return outImage;               
                    }                   
                }
            }          
        }
    }
}

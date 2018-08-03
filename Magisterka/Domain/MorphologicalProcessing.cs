using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
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

        public static Image<Gray, byte> CreateBinaryImage(Image<Bgr, byte> image, int threashold)
        {
            return image.Convert<Gray, byte>().ThresholdBinary(new Gray(threashold), new Gray(255));
        }
        public static Image<Gray, byte> GenerateBinaryImageNegative(Image<Gray, byte> image)
        {
            return image.Not();
        }
        public static Image<Gray,byte>CreateMaskFromPoints(Image<Gray, byte> image, Point[][] conturMatrix)
        {
            Image<Gray, byte> mask = image.CopyBlank();
            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint(conturMatrix);
            CvInvoke.FillPoly(mask, contours, new MCvScalar(255, 255, 255), Emgu.CV.CvEnum.LineType.AntiAlias);
            contours.Dispose();
            return mask;
        }
        public static Image<Bgr, byte> MultipleMaskAndImage(Image<Bgr, byte> image, Image<Gray, byte> mask)
        {
            return image.Convert<Bgr, float>().Mul(mask.Convert<Bgr, float>()).Convert<Bgr, byte>();
        }
        public static Image<Bgr,byte> MultipleImages(Image<Bgr, byte> image1, Image<Bgr, byte> image2)
        {
            return image1.Convert<Bgr, float>().Mul(image2.Convert<Bgr, float>()).Convert<Bgr, byte>();
        }
        public static Image<Gray, byte> MultipleImages(Image<Gray, byte> image1, Image<Gray, byte> image2)
        {
            return image1.Convert<Gray, float>().Mul(image2.Convert<Gray, float>()).Convert<Gray, byte>();
        }
        public static Image<Bgr, byte> CombineTwoImages(Image<Bgr, byte> image, Image<Bgr, byte> imagePattern, Image<Gray, byte> mask)
        {
            mask = Dilate(mask, new Size(2, 2), 1);
            Image<Bgr, byte> image1 = MultipleMaskAndImage(imagePattern, mask);
            Image<Bgr, byte> image2 = MultipleMaskAndImage(image, GenerateBinaryImageNegative(mask));
            Image<Bgr, byte> outImage = image1.Add(image2);
            image1.Dispose();
            image2.Dispose();
            return outImage;
        }
    }
}

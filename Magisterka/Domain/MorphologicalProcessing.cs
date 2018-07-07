using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.Drawing;

namespace Domain
{
    public class MorphologicalProcessing
    {
        public static Image<Bgr, byte> Dilate(Image<Bgr, byte> input, Size ksize, int iterations = 1)
        {
            Mat kernel = CvInvoke.GetStructuringElement(ElementShape.Rectangle, ksize, new Point(-1, -1));
            return input.MorphologyEx(MorphOp.Dilate, kernel, new Point(-1, -1), iterations, BorderType.Default, new MCvScalar(1.0));
        }

        public static Image<Bgr, byte> Erode(Image<Bgr, byte> input, Size ksize, int iterations = 1)
        {
            Mat kernel = CvInvoke.GetStructuringElement(ElementShape.Rectangle, ksize, new Point(-1, -1));
            return input.MorphologyEx(MorphOp.Erode, kernel, new Point(-1, -1), iterations, BorderType.Default, new MCvScalar(1.0));
        }
    }
}

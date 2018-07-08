using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV.CvEnum;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class DefectsFinder
    {

        private float _minFactor = 0.0000003f;
        private float _maxFactor = 0.0003f;
        private Image<Bgr, byte> _inputImage;
        private int _maxThreasholdOfDustContourSize = 100;
        private VectorOfVectorOfPoint _defectsContoursMatrix;
        private Point[][] _smallDefectsContoursMatrix;
        private Point[][] _largeDefectsContoursMatrix;
        private Image<Gray, byte> _patchMask;
        private Image<Gray, byte> _maskOfDefects;

        public VectorOfVectorOfPoint DefectsContoursMatrix
        {
            get
            {
                if (_defectsContoursMatrix == null)
                {
                    SearchDefects();
                }

                return _defectsContoursMatrix;
            }
            set
            {

                if (value != _defectsContoursMatrix)
                {
                    if (value == null)
                    {
                        _defectsContoursMatrix = new VectorOfVectorOfPoint();
                    }
                    else
                    {
                        _defectsContoursMatrix = value;
                    }
                }
            }
        }
        public Point[][] SmallDefectsContoursMatrix
        {
            get
            {
                if (_smallDefectsContoursMatrix == null)
                {
                    SplitDefectContoursBySize();
                }
                return _smallDefectsContoursMatrix;
            }
            set
            {

                if (value != _smallDefectsContoursMatrix)
                {
                    if (value == null)
                    {
                        _smallDefectsContoursMatrix = new Point[][] { };
                    }
                    else
                    {
                        _smallDefectsContoursMatrix = value;
                    }
                }
            }
        }
        public Point[][] LargeDefectsContoursMatrix
        {
            get
            {
                if (_largeDefectsContoursMatrix == null)
                {
                    SplitDefectContoursBySize();
                }

                return _largeDefectsContoursMatrix;
            }
            set
            {

                if (value != _largeDefectsContoursMatrix)
                {
                    if (value == null)
                    {
                        _smallDefectsContoursMatrix = new Point[][] { };
                    }
                    else
                    {
                        _largeDefectsContoursMatrix = value;
                    }
                }
            }
        }

        public Image<Gray, byte> PatchMask
        {
            get
            {
                if (_patchMask == null)
                {
                    SearchDefects();
                }

                return _patchMask;
            }
        }
        public Image<Gray, byte> MaskOfDefects
        {
            get
            {
                if (_maskOfDefects == null)
                {
                    SearchDefects();
                }

                return _maskOfDefects;
            }
        }

        public DefectsFinder(Image<Bgr, byte> image)
        {
            if (image != null)
                _inputImage = image;
        }

        public void SearchDefects()
        {
            Image<Gray, float> grayImage = _inputImage.Convert<Gray, float>();
            Image<Gray, float> laplaceImge = grayImage.Laplace(9);

            int a1 = 0, a2 = 0, b1 = 0, b2 = 0;
            GetThresholds(out a1, out a2, out b1, out b2, laplaceImge);

            _patchMask = GetMaskOfDefects(a1, a2, b1, b2, laplaceImge);
            _maskOfDefects = MorphologicalProcessing.Dilate(_patchMask.Convert<Bgr, byte>(), new Size(5, 5), 3).Convert<Gray, byte>();

            Image<Gray, byte> imageOutput = _maskOfDefects.Convert<Gray, byte>().ThresholdBinary(new Gray(100), new Gray(255));
            _defectsContoursMatrix = new VectorOfVectorOfPoint();
            Mat hier = new Mat();

            CvInvoke.FindContours(_maskOfDefects, _defectsContoursMatrix, hier, RetrType.External, ChainApproxMethod.ChainApproxSimple);

            //_inputImage = MorphologicalProcessing.Erode(_inputImage, new Size(3, 3), 3);

            //CvInvoke.DrawContours(_inputImage, _defectsContoursMatrix, -1, new MCvScalar(255, 0, 255));

            //return _inputImage.Convert<Bgr, byte>();
        }

        private void GetThresholds(out int a1, out int a2, out int b1, out int b2, Image<Gray, float> sourceImage)
        {
            DenseHistogram histogram = new DenseHistogram(256, new RangeF(0.0f, 255.0f));
            histogram.Calculate(new Image<Gray, byte>[] { sourceImage.Convert<Gray, byte>() }, false, null);

            List<float> hist = new List<float>(histogram.GetBinValues());
            int pixelsSum = (int)hist.Sum(x => x);

            a1 = hist.IndexOf(hist.FirstOrDefault(x => x > pixelsSum * _minFactor));
            a2 = hist.IndexOf(hist.FirstOrDefault(x => x > pixelsSum * _maxFactor));

            b1 = hist.LastIndexOf(hist.LastOrDefault(x => x > pixelsSum * _minFactor));
            b2 = hist.LastIndexOf(hist.LastOrDefault(x => x > pixelsSum * _maxFactor));
        }
        private Image<Gray, byte> GetMaskOfDefects(int a1, int a2, int b1, int b2, Image<Gray, float> sourceImage)
        {
            Image<Gray, byte> cannyImg1 = sourceImage.Convert<Gray, byte>().Canny(a1, a2);
            Image<Gray, byte> cannyImg2 = sourceImage.Convert<Gray, byte>().Canny(b1, b2);
            Image<Gray, byte> cannyImg = cannyImg1.Add(cannyImg2);
            Image<Gray, byte> dilatedImage = MorphologicalProcessing.Dilate(cannyImg.Convert<Bgr, byte>(), new Size(5, 5), 5).Convert<Gray, byte>();

            cannyImg1.Dispose();
            cannyImg2.Dispose();
            cannyImg.Dispose();

            return dilatedImage;
        }
        private void SplitDefectContoursBySize()
        {
            SmallDefectsContoursMatrix = DefectsContoursMatrix.ToArrayOfArray().Where(x => x.Count() <= _maxThreasholdOfDustContourSize).OrderBy(x => x.Count()).ToArray();
            LargeDefectsContoursMatrix = DefectsContoursMatrix.ToArrayOfArray().Where(x => x.Count() > _maxThreasholdOfDustContourSize).OrderBy(x => x.Count()).ToArray();
        }
    }
}

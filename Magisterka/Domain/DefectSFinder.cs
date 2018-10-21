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
    public class DefectsFinder : IDisposable
    {
        private const float _minFactor = 0.00000015f;
        private const float _maxFactor = 0.00015f;
        private Image<Bgr, byte> _inputImage;
        private double _maxThreasholdOfDustContourSize = 3000;
        private VectorOfVectorOfPoint _defectsContoursMatrix;
        private VectorOfVectorOfPoint _smallDefectsContoursMatrix;
        private VectorOfVectorOfPoint _largeDefectsContoursMatrix;
        private Image<Gray, byte> _maskOfDefects;

        public Image<Bgr, byte> ReturnTmpImg;

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
        public VectorOfVectorOfPoint SmallDefectsContoursMatrix
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
                        _smallDefectsContoursMatrix = new VectorOfVectorOfPoint();
                    }
                    else
                    {
                        _smallDefectsContoursMatrix = value;
                    }
                }
            }
        }
        public VectorOfVectorOfPoint LargeDefectsContoursMatrix
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
                        _smallDefectsContoursMatrix = new VectorOfVectorOfPoint();
                    }
                    else
                    {
                        _largeDefectsContoursMatrix = value;
                    }
                }
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
            _inputImage = image.Copy();
        }

        public void SearchDefects()
        {
            if (_inputImage == null)
                throw new ArgumentNullException(nameof(_inputImage));

            int a1 = 0, a2 = 0, b1 = 0, b2 = 0;

            ProgressManager.AddSteps(7);

            using (Image<Gray, float> grayImage = _inputImage.Convert<Gray, float>())
            {
                ProgressManager.DoStep();
                using (Image<Gray, float> laplaceImge = grayImage.Laplace(9))
                {
                    //ReturnTmpImg = laplaceImge.Convert<Bgr,byte>();
                    ProgressManager.DoStep();

                    GetThresholds(out a1, out a2, out b1, out b2, laplaceImge); //ProgressManager.DoneStep();
                    _maskOfDefects = GetMaskOfDefects(a1, a2, b1, b2, laplaceImge); //ProgressManager.DoneStep();          
                                     
                    DefectsContoursMatrix = new VectorOfVectorOfPoint();
                    using (Mat hier = new Mat())
                    {
                        CvInvoke.FindContours(_maskOfDefects, DefectsContoursMatrix, hier, RetrType.External, ChainApproxMethod.ChainApproxSimple);
                        ProgressManager.DoStep();
                    }                    
                }                     
            }

            SplitDefectContoursBySize();//DoStep 2
        }

        private void GetThresholds(out int a1, out int a2, out int b1, out int b2, Image<Gray, float> sourceImage)
        {
            ProgressManager.AddSteps(5);
            using (DenseHistogram histogram = new DenseHistogram(256, new RangeF(0.0f, 255.0f)))
            {
                ProgressManager.DoStep();
                histogram.Calculate(new Image<Gray, byte>[] { sourceImage.Convert<Gray, byte>() }, false, null);

                ProgressManager.DoStep();
                List<float> hist = new List<float>(histogram.GetBinValues());
                int pixelsSum = (int)hist.Sum(x => x);
                double min = pixelsSum * _minFactor;
                double max = pixelsSum * _maxFactor;
                ProgressManager.DoStep();

                a1 = hist.IndexOf(hist.FirstOrDefault(x => x > min));
                a2 = hist.IndexOf(hist.FirstOrDefault(x => x > max));
                ProgressManager.DoStep();

                b1 = hist.LastIndexOf(hist.LastOrDefault(x => x > max));
                b2 = hist.LastIndexOf(hist.LastOrDefault(x => x > min));
                ProgressManager.DoStep();
            }                
        }
        private Image<Gray, byte> GetMaskOfDefects(int a1, int a2, int b1, int b2, Image<Gray, float> sourceImage)
        {
            ProgressManager.AddSteps(3);
            using (Image<Gray, byte> cannyImg1 = sourceImage.Convert<Gray, byte>().Canny(a1, a2))
            {
                ProgressManager.DoStep();
                using (Image<Gray, byte> cannyImg2 = sourceImage.Convert<Gray, byte>().Canny(b1, b2))
                {                    
                    ProgressManager.DoStep();
                    using (Image<Gray, byte> cannyImg = cannyImg1.Add(cannyImg2))
                    {                       
                        ProgressManager.DoStep();
                        return MorphologicalProcessing.Dilate(cannyImg, new Size(3, 3), 6); 
                    }
                }                   
            } 
        }

        Dictionary<VectorOfPoint, double> CalculateDefectsFields()
        {
            Dictionary<VectorOfPoint, double> fieldsSize = new Dictionary<VectorOfPoint, double>();

            for (int i = 0; i < DefectsContoursMatrix.Size; i++)
            {
                fieldsSize.Add(DefectsContoursMatrix[i], CvInvoke.ContourArea(DefectsContoursMatrix[i]));
            }

            return fieldsSize.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
        }

        private void SplitDefectContoursBySize()
        {           
            var fieldSize = CalculateDefectsFields();
            ProgressManager.DoStep();
            var largeFields = fieldSize.Where(x => x.Value >= _maxThreasholdOfDustContourSize).ToDictionary(x => x.Key, x => x.Value).Keys.ToList(); 
            var smallFields = fieldSize.Where(x => x.Value < _maxThreasholdOfDustContourSize).ToDictionary(x => x.Key, x => x.Value).Keys.ToList();

            LargeDefectsContoursMatrix = new VectorOfVectorOfPoint();
            largeFields.ForEach(x => LargeDefectsContoursMatrix.Push(x));
            SmallDefectsContoursMatrix = new VectorOfVectorOfPoint();
            smallFields.ForEach(x => SmallDefectsContoursMatrix.Push(x));
            ProgressManager.DoStep();
        }

        public void Dispose()
        {
            if (_inputImage != null) _inputImage.Dispose();
            if (_defectsContoursMatrix != null) _defectsContoursMatrix.Dispose();
            if (_maskOfDefects != null) _maskOfDefects.Dispose();
            _largeDefectsContoursMatrix = null;
            _smallDefectsContoursMatrix = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}

﻿using Emgu.CV;
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
        private const float _minFactor = 0.0000002f;
        private const float _maxFactor = 0.0002f;
        private Image<Bgr, byte> _inputImage;
        private int _maxThreasholdOfDustContourSize = 100;
        private VectorOfVectorOfPoint _defectsContoursMatrix;
        private Point[][] _smallDefectsContoursMatrix;
        private Point[][] _largeDefectsContoursMatrix;
        private Image<Gray, byte> _patchMask;
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
            _inputImage = image.Copy();
        }

        public void SearchDefects()
        {
            if (_inputImage == null)
                throw new ArgumentNullException(nameof(_inputImage));

            int a1 = 0, a2 = 0, b1 = 0, b2 = 0;

            ProgressManager.AddSteps(6);

            using (Image<Gray, float> grayImage = _inputImage.Convert<Gray, float>())
            {
                ProgressManager.DoStep();
                using (Image<Gray, float> laplaceImge = grayImage.Laplace(9))
                {
                    ProgressManager.DoStep();                   

                    GetThresholds(out a1, out a2, out b1, out b2, laplaceImge); //ProgressManager.DoneStep();
                    _patchMask = GetMaskOfDefects(a1, a2, b1, b2, laplaceImge); //ProgressManager.DoneStep();

                    ReturnTmpImg = laplaceImge.Copy().Convert<Bgr,byte>();

                    _maskOfDefects = MorphologicalProcessing.Dilate(_patchMask, new Size(3, 3), 2).Copy();
                    ProgressManager.DoStep();

                    
                    _defectsContoursMatrix = new VectorOfVectorOfPoint();
                    using (Mat hier = new Mat())
                    {
                        CvInvoke.FindContours(_patchMask, _defectsContoursMatrix, hier, RetrType.External, ChainApproxMethod.ChainApproxSimple);
                    }
                    ProgressManager.DoStep();
                   
                    ReturnTmpImg = _inputImage.Copy();
                    
                    CvInvoke.DrawContours(ReturnTmpImg, _defectsContoursMatrix, -1, new MCvScalar(255, 0, 255));                    
                }                     
            }

            //ReturnTmpImg = MorphologicalProcessing.CreateMaskFromPoints(imageOutput, SmallDefectsContoursMatrix).Convert<Bgr,byte>();
            //ReturnTmpImg.Image = CvInvoke.DrawContours(_inputImage, _defectsContoursMatrix, -1, new MCvScalar(255, 0, 255));
            //_inputImage = MorphologicalProcessing.Erode(_inputImage, new Size(3, 3), 3);           
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
                        return MorphologicalProcessing.Dilate(cannyImg, new Size(3, 3), 3); 
                    }
                }                   
            } 
        }
        private void SplitDefectContoursBySize()
        {
            SmallDefectsContoursMatrix = DefectsContoursMatrix.ToArrayOfArray().Where(x => x.Count() <= _maxThreasholdOfDustContourSize).OrderByDescending(x => x.Count()).ToArray();
            LargeDefectsContoursMatrix = DefectsContoursMatrix.ToArrayOfArray().Where(x => x.Count() > _maxThreasholdOfDustContourSize).OrderByDescending(x => x.Count()).ToArray();
        }

        public void Dispose()
        {
            if (_inputImage != null) _inputImage.Dispose();
            if (_defectsContoursMatrix != null) _defectsContoursMatrix.Dispose();
            if (_patchMask != null) _patchMask.Dispose();
            if (_maskOfDefects != null) _maskOfDefects.Dispose();
            _largeDefectsContoursMatrix = null;
            _smallDefectsContoursMatrix = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}

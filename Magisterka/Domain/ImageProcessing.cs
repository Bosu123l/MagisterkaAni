using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace Domain
{
    public static class ImageProcessing
    {
        private static ImageWrapper<Bgr, byte> _imageBefor;
        private static ImageWrapper<Bgr, byte> _imageAfter;

        public static ImageWrapper<Bgr, byte> ImageBefor
        {
            get
            {
                return _imageBefor;
            }
            set
            {
                if (_imageBefor != value)
                {
                    _imageBefor = value.Copy();
                }
            }
        }
        public static ImageWrapper<Bgr, byte> ImageAfter
        {
            get
            {
                return _imageAfter;
            }
            set
            {
                if (_imageAfter != value)
                {
                    _imageAfter = value.Copy();
                }
            }
        }                     
           
        public static void SetImage(ImageWrapper<Bgr, byte> image)
        {            
            if (image != null)
            {
                ImageBefor = image;
                ImageAfter = image;
            }
        }       

        public static void ReduceDust()
        {
            ProgressManager.AddSteps(3);
            using (DefectsFinder defectsFinder = new DefectsFinder(ImageBefor))
            {
                ProgressManager.DoStep();
                using (Dust dust = new Dust(ImageAfter, defectsFinder.MaskOfDefects, defectsFinder.SmallDefectsContoursMatrix))
                {
                    ProgressManager.DoStep();

                    _imageAfter = dust.RemoveDust();
                    ProgressManager.DoStep();
                }
            }       
        }

        public static void CutImage() { }
        public static void ReduceSmudges() { }
        
        public static void RotateImage()
        {
            ImageAfter = Aligning.RotateOn90(ImageAfter);
        }
        public static void AlignImage(double angle)
        {
            ImageAfter = Aligning.Rotate(ImageAfter, angle);
        }

        public static void Test()
        {
            using (DefectsFinder defectsFinder = new DefectsFinder(ImageBefor))
            {
                defectsFinder.SearchDefects();
                ImageAfter = defectsFinder.ReturnTmpImg;
            }
            //CvInvoke.DrawContours(ImageAfter, _defectsFinder.DefectsContoursMatrix, -1, new MCvScalar(255, 0, 255));
            //_imageAfter = MorphologicalProcessing.CreateBinaryImage(_imageAfter, 192).Convert<Bgr,byte>();
            //_imageAfter = _defectsFinder.SearchDefects();//MorphologicalProcessing.Erode(_imageAfter, new Size(2,2), 1);
        }       
    }
}

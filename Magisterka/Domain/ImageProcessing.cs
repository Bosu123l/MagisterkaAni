using Emgu.CV;
using Emgu.CV.Structure;

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

        private static double _blueTone;
        private static double _greenTone;
        private static double _redTone;       

        public static void SetImage(ImageWrapper<Bgr, byte> image)
        {            
            if (image != null)
            {
                ImageBefor = image.Copy();
                ImageAfter = image.Copy();

                #region QualifityPhotoTone

                ImageColor imageColor = new ImageColor();
                imageColor.QualifityPhotoTone(image);
                _blueTone = imageColor.BlueTone;
                _greenTone = imageColor.GreenTone;
                _redTone = imageColor.RedTone;
                #endregion QualifityPhotoTone
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

        public static void ReduceSmudges()
        {
            using (Smudge smudge = new Smudge(ImageAfter))
            {
                ImageAfter = smudge.OtherColorDetector(_blueTone, _greenTone, _redTone);
            }
        }
        
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
            //using (DefectsFinder defectsFinder = new DefectsFinder(ImageBefor))
            //{
            //    defectsFinder.SearchDefects();
            //    ImageAfter = defectsFinder.ReturnTmpImg;
            //}

            using (ImageWrapper<Gray, byte> gray = ImageBefor.Copy().Convert<Gray, byte>()) 
            {
                //ImageWrapper<Bgr,byte> mask = 
                ImageAfter.Image = ImageBefor.Image.Sub(gray.Image.Convert<Bgr, byte>());
                ImageAfter = MorphologicalProcessing.CreateBinaryImage(ImageAfter, 3).Convert<Bgr,byte>();
            }

            //CvInvoke.DrawContours(ImageAfter, _defectsFinder.DefectsContoursMatrix, -1, new MCvScalar(255, 0, 255));
            //_imageAfter = MorphologicalProcessing.CreateBinaryImage(_imageAfter, 192).Convert<Bgr,byte>();
            //_imageAfter = _defectsFinder.SearchDefects();//MorphologicalProcessing.Erode(_imageAfter, new Size(2,2), 1);
        }       
    }
}

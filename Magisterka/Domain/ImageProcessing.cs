using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.Drawing;

namespace Domain
{
    public static class ImageProcessing
    {
        private static Image<Bgr, byte> _imageBefor;
        private static Image<Bgr, byte> _imageAfter;


        public static event EventHandler<Image<Bgr, byte>> ImageAfterChange;

        public static void OnImageAfterChange(Image<Bgr, byte> image)
        {
            ImageAfterChange?.Invoke(null, image);
        }

        public static Image<Bgr, byte> ImageBefor
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
        public static Image<Bgr, byte> ImageAfter
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
                    OnImageAfterChange(_imageAfter);
                }
            }
        }

        public static void SetImage(Image<Bgr, byte> image)
        {
            if (image != null)
            {
                ImageBefor = image.Copy();
                ImageAfter = image.Copy();
            }
        }

        public static void ReduceDust()
        {
            ProgressManager.AddSteps(3);
            using (DefectsFinder defectsFinder = new DefectsFinder(ImageBefor))
            {
                ProgressManager.AddSteps(defectsFinder.SmallDefectsContoursMatrix.Length);
                using (Dust dust = new Dust(ImageBefor, defectsFinder.MaskOfDefects, defectsFinder.SmallDefectsContoursMatrix))
                {
                    ImageAfter = defectsFinder.ReturnTmpImg;
                    //for (int i=0; i< defectsFinder.SmallDefectsContoursMatrix.Length; i++)
                    //{
                    //    ImageAfter = dust.RemoveDefect(defectsFinder.SmallDefectsContoursMatrix[i]);
                    //    ProgressManager.DoStep();
                    //}                   
                }
            }
        }

        public static void CutImage() { }

        public static void ReduceSmudges()
        {
            using (Smudge smudge = new Smudge(ImageBefor))
            {
                ImageAfter = smudge.CleanSmudges();
            }
        }

        public static void RotateImage()
        {
            ImageAfter = Aligning.RotateOn90(ImageAfter);
            ImageBefor = Aligning.RotateOn90(ImageBefor);
        }
        public async static void Test()
        {
            Image<Gray, byte> canny = ImageBefor.Convert<Gray, byte>().CopyBlank();
            CvInvoke.Canny(ImageBefor, canny, 100, 150, 3, true);
            using (VectorOfPointF vp = new VectorOfPointF())
            {
                CvInvoke.HoughLines(canny, vp, 1, Math.PI / 180, 150, 0, 0);

                Image<Bgr, Byte> lineImage = ImageBefor.Copy();

                var elemnetsOfVp = vp.ToArray();
                for (int i = 0; i < elemnetsOfVp.Length - 1; i++)
                {
                    var line = new LineSegment2DF(elemnetsOfVp[i], elemnetsOfVp[i + 1]);
                    lineImage.Draw(line, new Bgr(Color.Red), 10);
                }

                ImageAfter = lineImage.Copy();
            }
        }
    }

    public static class CopyOfImageProcessing
    {
        private static Image<Bgr, byte> _imageBefor;
        private static Image<Bgr, byte> _imageAfter;


        public static event EventHandler<Image<Bgr, byte>> ImageAfterChange;

        public static void OnImageAfterChange(Image<Bgr, byte> image)
        {
            ImageAfterChange?.Invoke(null, image);
        }

        public static Image<Bgr, byte> ImageBefor
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
        public static Image<Bgr, byte> ImageAfter
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
                    OnImageAfterChange(_imageAfter);
                }
            }
        }

        public static void SetImage(Image<Bgr, byte> image)
        {
            if (image != null)
            {
                ImageBefor = image.Copy();
                ImageAfter = image.Copy();
            }
        }

        public static void ReduceDust()
        {
            ProgressManager.AddSteps(3);
            using (DefectsFinder defectsFinder = new DefectsFinder(ImageBefor))
            {
                ProgressManager.AddSteps(defectsFinder.SmallDefectsContoursMatrix.Length);
                using (Dust dust = new Dust(ImageBefor, defectsFinder.MaskOfDefects, defectsFinder.SmallDefectsContoursMatrix))
                {
                    ImageAfter = defectsFinder.ReturnTmpImg;
                    //for (int i=0; i< defectsFinder.SmallDefectsContoursMatrix.Length; i++)
                    //{
                    //    ImageAfter = dust.RemoveDefect(defectsFinder.SmallDefectsContoursMatrix[i]);
                    //    ProgressManager.DoStep();
                    //}                   
                }
            }
        }

        public static void CutImage() { }

        public static void ReduceSmudges()
        {
            using (Smudge smudge = new Smudge(ImageBefor))
            {
                ImageAfter = smudge.CleanSmudges();
            }
        }

        public static void RotateImage()
        {
            ImageAfter = Aligning.RotateOn90(ImageAfter);
            ImageBefor = Aligning.RotateOn90(ImageBefor);
        }
        public static void AlignImage(double angle)
        {
            ImageAfter = Aligning.Rotate(ImageAfter, angle);
            ImageBefor = Aligning.Rotate(ImageBefor, angle);
        }

        public async static void Test()
        {
            //CvInvoke.PyrMeanShiftFiltering(ImageBefor.Image, ImageAfter.Image, 40, 60, 3, new  MCvTermCriteria());
            ImageAfter = ImageBefor.Rotate(17, new Bgr(Color.Black), false);
            //ImageAfter.Image.Data[1, 1, 0] = 0;
            //ImageAfter.Image.Data[1, 1, 1] = 0;
            //ImageAfter.Image.Data[1, 1, 2] = 255;

        }
    }
}

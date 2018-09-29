using Emgu.CV;
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

        public static void AutomaticRepair()
        {
            if (ImageBefor != null)
            {
            }
            else
            {
                throw new ArgumentNullException(nameof(ImageBefor));
            }
        }
        public async static void Test()
        {
            if (ImageBefor != null)
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
            else
            {
                throw new ArgumentNullException(nameof(ImageBefor));
            }           
        }       
        public static void ReduceDust()
        {
            if(ImageBefor!=null)
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
            }else
            {
                throw new ArgumentNullException(nameof(ImageBefor));
            }
        }
        public static void ReduceScratches()
        {
            if (ImageBefor != null) {
            }
            else
            {
                throw new ArgumentNullException(nameof(ImageBefor));
            }
        }
        public static void ReduceSmudges()
        {
            if (ImageBefor != null)
            {
                using (Smudge smudge = new Smudge(ImageBefor))
                {
                    ImageAfter = smudge.CleanSmudges();
                }
            }
            else
            {
                throw new ArgumentNullException(nameof(ImageBefor));
            }
        }        
        public static void SetRegionWithoutRepair()
        {
            if (ImageBefor != null)
            {
            }
            else
            {
                throw new ArgumentNullException(nameof(ImageBefor));
            }
        }
        public static void CutImage(Size viewWindowSize, Rectangle rectangleToCut)
        {
            if (ImageBefor != null)
            {
                CutPhoto ci = new CutPhoto(ImageBefor.Size, viewWindowSize, rectangleToCut);               
                ImageBefor = ci.CutImageByRectangle(ImageBefor);
                ImageAfter = ci.CutImageByRectangle(ImageAfter);               
            }
            else
            {
                throw new ArgumentNullException(nameof(ImageBefor));
            }
        }
        public static void RotateImage()
        {
            if (ImageBefor != null)
            {
                ImageAfter = Aligning.RotateOn90(ImageAfter);
                ImageBefor = Aligning.RotateOn90(ImageBefor);
            }
            else
            {
                throw new ArgumentNullException(nameof(ImageBefor));
            }
        }       
    }
}

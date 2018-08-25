using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Domain
{
    public static class ImageProcessing
    {
        private static ImageWrapper<Bgr, byte> _imageBefor;
        private static ImageWrapper<Bgr, byte> _imageAfter;


        public static event EventHandler<ImageWrapper<Bgr, byte>> ImageAfterChange;

        public static void OnImageAfterChange(ImageWrapper<Bgr, byte> image)
        {
            ImageAfterChange?.Invoke(null, image);
        }

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
                    OnImageAfterChange(_imageAfter);
                }
            }
        }

        public static void SetImage(ImageWrapper<Bgr, byte> image)
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
            using (Smudge smudge = new Smudge(ImageBefor))
            {
                ImageAfter = smudge.AveragePictureColors().Copy();
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
            //using (DefectsFinder defectsFinder = new DefectsFinder(ImageBefor))
            //{
            //    defectsFinder.SearchDefects();
            //    ImageAfter = defectsFinder.ReturnTmpImg;
            //}

            //using (ImageWrapper<Gray, byte> gray = ImageBefor.Copy().Convert<Gray, byte>()) 
            //{
            //    //ImageWrapper<Bgr,byte> mask = 
            //    ImageAfter.Image = ImageBefor.Image.Sub(gray.Image.Convert<Bgr, byte>());
            //    ImageAfter = MorphologicalProcessing.CreateBinaryImage(ImageAfter, 3).Convert<Bgr,byte>();
            //}

            Image<Gray, byte>[] splitedImages = ImageBefor.Image.Split();
            for (int i = 0; i < splitedImages.Length; i++)
            {
                ImageAfter = new ImageWrapper<Bgr, byte>(splitedImages[i].Convert<Bgr, byte>());
                await Task.Run(() =>
                {
                    Thread.Sleep(10000);
                    System.Diagnostics.Debug.WriteLine($"Techno: {i}");
                });
            }

            //ImageAfter = new ImageWrapper<Bgr, byte>(splitedImages[1].Convert<Bgr, byte>());


            ////ImageAfter =    // MorphologicalProcessing.Dilate(ImageBefor, new System.Drawing.Size(7, 7), 10);
            //ImageWrapper<Gray, float> Floataowy = ImageAfter.Convert<Gray, float>().Add(ImageBefor.Convert<Gray,float>());
            //ImageWrapper<Gray, byte> Byte = ImageAfter.Convert<Gray, byte>();
            //CvInvoke.DrawContours(ImageAfter, _defectsFinder.DefectsContoursMatrix, -1, new MCvScalar(255, 0, 255));
            //_imageAfter = MorphologicalProcessing.CreateBinaryImage(_imageAfter, 192).Convert<Bgr,byte>();
            //_imageAfter = _defectsFinder.SearchDefects();//MorphologicalProcessing.Erode(_imageAfter, new Size(2,2), 1);
            //ImageAfter = new ImageWrapper<Bgr, byte>(ImageBefor.Image.AbsDiff(new Bgr(10, 100, 100)));

            //Bgr bgr = new Bgr();
            //MCvScalar mCvScalar = new MCvScalar();
            //ImageAfter.Image.AvgSdv(out bgr, out mCvScalar);
            //Smudge smudge = new Smudge(ImageBefor);
            //ImageAfter = smudge.ClearOtherColorsSmudges();
        }
    }
}

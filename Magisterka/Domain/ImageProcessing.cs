using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
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
                ProgressManager.AddSteps(defectsFinder.SmallDefectsContoursMatrix.Length);
                using (Dust dust = new Dust(ImageBefor, defectsFinder.MaskOfDefects, defectsFinder.SmallDefectsContoursMatrix))
                {                    
                    for (int i=0; i< defectsFinder.SmallDefectsContoursMatrix.Length; i++)
                    {
                        ImageAfter = dust.RemoveDefect(defectsFinder.SmallDefectsContoursMatrix[i]);
                        ProgressManager.DoStep();
                    }                   
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
            ////////////List<ImageWrapper<Bgr, byte>> splitedImages2 = new List<ImageWrapper<Bgr, byte>>();
            ////////////Image<Gray, byte>[] splitedImages = ImageBefor.Image.Split();
            ////////////for (int i = 0; i < splitedImages.Length; i++)
            ////////////{
            ////////////    //Smudge smudge = new Smudge(new ImageWrapper<Bgr, byte>(splitedImages[i].Convert<Bgr, byte>()));
            ////////////    //var tempImage = smudge.ClearOtherColorsSmudges();
            ////////////    var imageTemp = new ImageWrapper<Bgr, byte>(splitedImages[i].Convert<Bgr, byte>());
            ////////////    ImageAfter = new ImageWrapper<Bgr, byte>(imageTemp);
            ////////////    splitedImages2.Add(imageTemp);
            ////////////    System.Diagnostics.Debug.WriteLine($"Techno: {i}");

            ////////////    //await Task.Run(() =>
            ////////////    //{
            ////////////    //    //Thread.Sleep(10000);

            ////////////    //});
            ////////////}

            //splitedImages2[0] = new ImageWrapper<Bgr, byte>(splitedImages2[1].Image.AbsDiff(splitedImages2[0].Image.AbsDiff(splitedImages2[2].Image)));

            //ImageAfter = splitedImages2[0];
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

            //using (DefectsFinder defectsFinder = new DefectsFinder(ImageBefor))
            //{               
            //    CvInvoke.DrawContours(ImageAfter.Image, defectsFinder.DefectsContoursMatrix, -1, new MCvScalar(255, 0, 255));
            //}

            //import numpy as np
            //import cv2 as cv
            //from matplotlib import pyplot as plt
            //img = cv.imread('die.png')
            //dst = cv.fastNlMeansDenoisingColored(img, None, 10, 10, 7, 21)
            //plt.subplot(121),plt.imshow(img)
            //plt.subplot(122),plt.imshow(dst)
            //plt.show()

            ImageAfter = MorphologicalProcessing.CreateBinaryImage(ImageBefor, 1).Convert<Bgr,byte>();

            //CvInvoke.FastNlMeansDenoisingColored(ImageBefor.Image, ImageAfter.Image, 10, 10, 7, 21);
            //ImageAfter.Image = ImageBefor.Image.AbsDiff(ImageAfter.Image);

        }
    }
}

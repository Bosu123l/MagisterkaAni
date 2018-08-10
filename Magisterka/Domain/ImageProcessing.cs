﻿using Emgu.CV;
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

        public static DenseHistogram Histogam;


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

        public static BitmapImage BitmapImageBefor
        {
            get
            {
                return BitmapToImageSource(ImageBefor);
            }
        }
        public static BitmapImage BitmapImageAfter
        {
            get
            {
                return BitmapToImageSource(ImageAfter);
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

        public static BitmapImage BitmapToImageSource(ImageWrapper<Bgr, byte> image)
        {
            if (image == null)
                return null;

            using (MemoryStream memory = new MemoryStream())
            {               
                image.Bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();
                return bitmapimage;
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

        public static void Test()
        {           
            var defectsFinder = new DefectsFinder(ImageBefor);
            defectsFinder.SearchDefects();
            ImageAfter = defectsFinder.ReturnTmpImg;
            //CvInvoke.DrawContours(ImageAfter, _defectsFinder.DefectsContoursMatrix, -1, new MCvScalar(255, 0, 255));
            //_imageAfter = MorphologicalProcessing.CreateBinaryImage(_imageAfter, 192).Convert<Bgr,byte>();
            //_imageAfter = _defectsFinder.SearchDefects();//MorphologicalProcessing.Erode(_imageAfter, new Size(2,2), 1);
        }       
    }
}

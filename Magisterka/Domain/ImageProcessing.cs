﻿using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;

namespace Domain
{
    public static class ImageProcessing
    {
        private static Image<Bgr, byte> _imageBefor;
        private static Image<Bgr, byte> _imageAfter;
        private static Image<Gray, byte> _exclFromCleaning;
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

        public static void AutomaticRepair(bool dust, bool scratches, bool smudges)
        {
            int steps = 0;

            steps = dust ? steps + 1 : steps;
            steps = scratches ? steps + 1 : steps;
            steps = smudges ? steps + 1 : steps;
            steps = (scratches || smudges) ? steps + 1 : steps;
            ProgressManager.AddSteps(steps);

            if (ImageBefor != null)
            {
                if (smudges)
                {
                    //Progress 9 steps
                    using (Smudge smudge = new Smudge(ImageBefor))
                    {
                        ImageAfter = smudge.CleanSmudges();
                    }
                }
                
                if(dust || scratches)
                {
                    using (DefectsFinder defectsFinder = new DefectsFinder(ImageBefor))
                    {
                        //Progress 8 steps
                        defectsFinder.SearchDefects();
                        if(dust)
                        {
                            using (Dust dustCleaner = new Dust(ImageAfter, defectsFinder.SmallDefectsContoursMatrix, _exclFromCleaning))
                            {   
                                //Progress 100 steps
                                ImageAfter = dustCleaner.DustReductionLeftToRight();
                            }
                        }
                       
                        if (scratches)
                        {
                            using (Scratches scratchesCleaner = new Scratches(ImageAfter, defectsFinder.LargeDefectsContoursMatrix, _exclFromCleaning))
                            {
                                //Progres scratches count
                                ImageAfter = scratchesCleaner.RemoveScrates();
                            }
                        }                      
                    }
                }               
            }
        }
        public static void Test()
        {
            if (ImageBefor != null)
            {
                //ToHSV();
                Mat kernel = CvInvoke.GetStructuringElement(Emgu.CV.CvEnum.ElementShape.Rectangle, new Size(9,9), new Point(-1, -1));
                ImageAfter = ImageBefor.MorphologyEx(Emgu.CV.CvEnum.MorphOp.Tophat, kernel, new Point(-1, -1), 1, BorderType.Default, new MCvScalar(1.0));

                //Image<Hsv, byte> bgraImage = ImageBefor.Convert<Hsv, byte>();
                ////for (int i = 0; i < 2; i++)
                ////{
                //    ImageAfter = bgraImage[2].Convert<Bgr, byte>(); //Thread.Sleep(3000);
                ////}

                //ImageAfter = ImageBefor.Convert<Gray, float>().Laplace(13).Convert<Bgr,byte>();

                //Image<Hsv, byte> hsvImage = ImageBefor.Convert<Hsv, byte>();
                //hsvImage[1] = hsvImage[1].Add(new Gray(255));
                //hsvImage[2] = hsvImage[2].Add(new Gray(255));

                //List<double> hue = new List<double>();

                //for (int x = 0; x < hsvImage.Height; x++)
                //{
                //    for (int y = 0; y < hsvImage.Width; y++)
                //    {
                //        hue.Add(hsvImage[x, y].Hue);
                //    }
                //}

                //var max = hue.GroupBy(x => x).OrderByDescending(x => x.Count()).First().Key;
                //hue = hue.GroupBy(x => x).Select(x => x.Key).ToList();
                //var median = hue[hue.Count / 2];

                //for (int x = 0; x < hsvImage.Height; x++)
                //{
                //    for (int y = 0; y < hsvImage.Width; y++)
                //    {
                //        var pix = hsvImage[x, y].Hue;

                //        if (pix > max + 10 || pix < max - 10)
                //        {
                //            hsvImage.Data[x, y, 0] = 90;
                //        }
                //        else
                //        {
                //            hsvImage.Data[x, y, 0] = 180;
                //        }
                //    }
                //}

                //ImageAfter = hsvImage.Convert<Bgr, byte>();

                //Image<Bgra, byte> bgraImage = ImageBefor.Convert<Bgra, byte>();
                //for (int i = 0; i < 4; i++)
                //{
                //    ImageAfter = bgraImage[i].Convert<Bgr, byte>(); Thread.Sleep(3000);
                //}

                //Image<Hls, byte> hsvImage = ImageBefor.Convert<Hls, byte>();
                //for (int i = 0; i < 3; i++)
                //{
                //    ImageAfter = hsvImage[i].Convert<Bgr, byte>();Thread.Sleep(3000);
                //}

                //Image<Lab, byte> LabImage = ImageBefor.Convert<Lab, byte>();
                //for (int i = 0; i < 3; i++)
                //{
                //    ImageAfter = LabImage[i].Convert<Bgr, byte>(); Thread.Sleep(3000);
                //}

                //Image<Luv, byte> LuvImage = ImageBefor.Convert<Luv, byte>();
                //for (int i = 0; i < 3; i++)
                //{
                //    ImageAfter = LuvImage[i].Convert<Bgr, byte>(); Thread.Sleep(3000);
                //}



                //Image<Ycc, byte> YccImage = ImageBefor.Convert<Ycc, byte>();
                //for (int i = 0; i < 3; i++)
                //{
                //    ImageAfter = YccImage[i].Convert<Bgr, byte>(); Thread.Sleep(3000);
                //}

                //using (DefectsFinder df = new DefectsFinder(ImageBefor))
                //{
                //    df.SearchDefects();
                //    //CvInvoke.FillPoly(ImageAfter, new VectorOfVectorOfPoint( df.LargeDefectsContoursMatrix[2]), new MCvScalar(255, 0, 255), LineType.FourConnected);
                //    using (ImageCleaner ci = new ImageCleaner(ImageBefor))
                //    {
                //        //ImageAfter = ci.ClearImageByDefects();
                //        ImageAfter = ci.SpiralCleanLargeDefects(df.LargeDefectsContoursMatrix);

                //        using (Dust dust = new Dust(ImageAfter, df.SmallDefectsContoursMatrix))
                //        {
                //            //CvInvoke.FillPoly(ImageAfter, defectsFinder.SmallDefectsContoursMatrix, new MCvScalar(255,0,255), LineType.FourConnected);
                //            ImageAfter = dust.RemoveDustDefects();
                //        }
                //    }
                //}


                //using (DefectsFinder df = new DefectsFinder(ImageBefor))
                //{
                //    df.SearchDefects();
                //    //CvInvoke.FillPoly(ImageAfter, new VectorOfVectorOfPoint( df.LargeDefectsContoursMatrix[2]), new MCvScalar(255, 0, 255), LineType.FourConnected);
                //    using (ImageCleaner ci = new ImageCleaner(ImageBefor))
                //    {
                //        ImageAfter = ci.InPaintMethod(df.LargeDefectsContoursMatrix, InpaintType.NS);
                //    }
                //}
            }
            else
            {
                throw new ArgumentNullException(nameof(ImageBefor));
            }
        }


        private static void ToHSV()
        {
            //int max = 0;
            //int saturation = 0;
            //var hsvImg = ImageBefor.Convert<Hsv, byte>();
           
            //    using (DenseHistogram histogram = new DenseHistogram(256, new RangeF(0.0f, 180.0f)))
            //    {                    
            //        histogram.Calculate(new Image<Gray, byte>[] { hsvImg[0] }, false, null);     
            //        List<float> hist = new List<float>(histogram.GetBinValues());
            //        max = hist.IndexOf((int)hist.Max());
                    
            //    }
            //    var imgMax = ImageBefor.Convert<Hsv,byte>()[0].CopyBlank().Add(new Gray(max));
            //    hsvImg[0] = imgMax;

            //using (DenseHistogram histogram = new DenseHistogram(256, new RangeF(0.0f, 255.0f)))
            //{
            //    histogram.Calculate(new Image<Gray, byte>[] { hsvImg[1] }, false, null);
            //    List<float> hist = new List<float>(histogram.GetBinValues());
            //    saturation = hist.IndexOf((int)hist.Max());

            //}
            //var satMask = ImageBefor.Convert<Hsv, byte>()[0].CopyBlank().Add(new Gray(saturation));
            //hsvImg[1] = imgMax;

            //using (DenseHistogram histogram = new DenseHistogram(256, new RangeF(0.0f, 255.0f)))
            //{
            //    histogram.Calculate(new Image<Gray, byte>[] { hsvImg[2] }, false, null);
            //    List<float> hist = new List<float>(histogram.GetBinValues());               

            //}

            //ImageAfter = hsvImg[2].Convert<Bgr, byte>();


            int max = 0;
            int saturation = 0;
            var hsvImg = ImageBefor.Convert<Hsv, byte>();

            using (DenseHistogram histogram = new DenseHistogram(256, new RangeF(0.0f, 180.0f)))
            {
                histogram.Calculate(new Image<Gray, byte>[] { hsvImg[0] }, false, null);
                List<float> hist = new List<float>(histogram.GetBinValues());
                max = hist.IndexOf((int)hist.Max());

            }
            var imgMax = ImageBefor.Convert<Hsv, byte>()[0].CopyBlank().Add(new Gray(max));
            hsvImg[0] = imgMax;

            using (DenseHistogram histogram = new DenseHistogram(256, new RangeF(0.0f, 255.0f)))
            {
                histogram.Calculate(new Image<Gray, byte>[] { hsvImg[1] }, false, null);
                List<float> hist = new List<float>(histogram.GetBinValues());
                saturation = hist.IndexOf((int)hist.Max());

            }
            //var saturationMask = 
            //var satMask = ImageBefor.Convert<Hsv, byte>()[0].CopyBlank().Add(new Gray(saturation));
            //hsvImg[1] = imgMax;

            //using (DenseHistogram histogram = new DenseHistogram(256, new RangeF(0.0f, 255.0f)))
            //{
            //    histogram.Calculate(new Image<Gray, byte>[] { hsvImg[2] }, false, null);
            //    List<float> hist = new List<float>(histogram.GetBinValues());

            //}

            ImageAfter = hsvImg[2].Convert<Bgr, byte>();
        }

        #region Dust
        public static void ReduceDust()
        {
            DustReductionSpiralAveragingDefectsMethod();
        }
        public static void DustReductionLeftToRightAveragingDefectsMethod()
        {
            if (ImageBefor != null)
            {
                ProgressManager.AddSteps(3);
                using (DefectsFinder defectsFinder = new DefectsFinder(ImageBefor))
                {
                    defectsFinder.SearchDefects();
                    using (Dust dust = new Dust(ImageBefor, defectsFinder.SmallDefectsContoursMatrix, _exclFromCleaning))
                    {
                        ImageAfter = dust.DustReductionLeftToRight();
                    }
                }
            }
        }
        public static void DustReductionSpiralAveragingDefectsMethod()
        {
            if (ImageBefor != null)
            {
                ProgressManager.AddSteps(2);
                using (DefectsFinder defectsFinder = new DefectsFinder(ImageBefor))
                {
                    defectsFinder.SearchDefects();
                    using (Dust dust = new Dust(ImageBefor, defectsFinder.SmallDefectsContoursMatrix, _exclFromCleaning))
                    {
                        ImageAfter = dust.DustReductionSpiralAveranging();
                        ProgressManager.DoStep();
                    }
                }
            }
        }
        #endregion Dust

        #region Scratches
        public static void ReduceScratches()
        {
            ScratchesReductionSpiralSingleDefectsMethod();
        }

        public static void ScratchesReductionInPaintNSMethod()
        {
            if (ImageBefor != null)
            {
                using (DefectsFinder defectsFinder = new DefectsFinder(ImageBefor))
                {
                    defectsFinder.SearchDefects();
                    using (Scratches scratches = new Scratches(ImageBefor, defectsFinder.LargeDefectsContoursMatrix, _exclFromCleaning))
                    {
                        ImageAfter = scratches.InpaintNSMethod();
                    }
                }
            }
        }
        public static void ScratchesReductionInPaintTeleaMethod()
        {
            if (ImageBefor != null)
            {
                using (DefectsFinder defectsFinder = new DefectsFinder(ImageBefor))
                {
                    defectsFinder.SearchDefects();
                    using (Scratches scratches = new Scratches(ImageBefor, defectsFinder.LargeDefectsContoursMatrix, _exclFromCleaning))
                    {
                        ImageAfter = scratches.InpaintTeleaMethod();
                    }
                }
            }
        }
        public static void ScratchesReductionSpiralSingleDefectsMethod()
        {
            if (ImageBefor != null)
            {
                ProgressManager.AddSteps(2);
                using (DefectsFinder defectsFinder = new DefectsFinder(ImageBefor))
                {
                    defectsFinder.SearchDefects();
                    using (Scratches scratches = new Scratches(ImageBefor, defectsFinder.LargeDefectsContoursMatrix, _exclFromCleaning))
                    {
                        ImageAfter = scratches.ScratchesReductionSpiralAveranging();
                    }
                }
            }
        }
        #endregion Scratches

        public static void ReduceSmudges()
        {
            ReduceSmudgesBGR();
        }
        
        public static void ReduceSmudgesBGR()
        {
            if (ImageBefor != null)
            {
                using (Smudge smudge = new Smudge(ImageBefor))
                {
                    ImageAfter = smudge.CleanSmudges();
                }
            }
        }

        public static void ReduceSmudgesHSV()
        {
            if (ImageBefor != null)
            {
                using (Smudge smudge = new Smudge(ImageBefor))
                {
                    ImageAfter = smudge.CleanHSV();
                }
            }
        }

        public static void SetRegionWithoutRepair()
        {
            if (ImageBefor != null)
            {

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
        }
        public static void RotateImage()
        {
            if (ImageBefor != null)
            {
                ImageAfter = Aligning.RotateOn90(ImageAfter);
                ImageBefor = Aligning.RotateOn90(ImageBefor);
            }
        }       
    }
}


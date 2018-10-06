using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

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

                //Image<Hsv, byte> bgraImage = ImageBefor.Convert<Hsv, byte>();
                //for (int i = 0; i < 3; i++)
                //{
                //    ImageAfter = bgraImage[i].Convert<Bgr, byte>(); Thread.Sleep(3000);
                //}

                Image<Hsv, byte> hsvImage = ImageBefor.Convert<Hsv, byte>();
                hsvImage[1] = hsvImage[1].Add(new Gray(255));
                hsvImage[2] = hsvImage[2].Add(new Gray(255));

                List<double> hue = new List<double>();

                for (int x = 0; x < hsvImage.Height; x ++)
                {
                    for (int y = 0; y < hsvImage.Width; y ++)
                    {
                        hue.Add(hsvImage[x, y].Hue);
                    }
                }

                var max = hue.GroupBy(x=>x).OrderByDescending(x=>x.Count()).First().Key;
                hue = hue.GroupBy(x => x).Select(x=>x.Key).ToList();
                var median = hue[hue.Count / 2];
                
                for (int x = 0; x < hsvImage.Height; x++)
                {
                    for (int y = 0; y < hsvImage.Width; y++)
                    {
                        var pix = hsvImage[x, y].Hue;

                        if(pix>max+10 || pix<max-10)
                        {
                            hsvImage.Data[x,y,0] = 90;
                        }else
                        {
                            hsvImage.Data[x, y, 0] = 180;
                        }
                    }
                }

                ImageAfter = hsvImage.Convert<Bgr, byte>(); 
             
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
                //    using (ClearImage ci = new ClearImage(ImageBefor))
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
                //    using (ClearImage ci = new ClearImage(ImageBefor))
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

        #region Dust
        public static void ReduceDust()
        {
            if(ImageBefor!=null)
            {
                ProgressManager.AddSteps(3);
                using (DefectsFinder defectsFinder = new DefectsFinder(ImageBefor))
                {
                    defectsFinder.SearchDefects();
                    using (Dust dust = new Dust(ImageBefor, defectsFinder.DefectsContoursMatrix))
                    {
                        //CvInvoke.FillPoly(ImageAfter, defectsFinder.SmallDefectsContoursMatrix, new MCvScalar(255,0,255), LineType.FourConnected);
                        ImageAfter = dust.RemoveDustDefects();
                    }
                }
            }else
            {
                throw new ArgumentNullException(nameof(ImageBefor));
            }
        }
        #endregion Dust

        #region Scratches
        public static void ReduceScratches()
        {
            if (ImageBefor != null) {
                using (DefectsFinder defectsFinder = new DefectsFinder(ImageBefor))
                {
                    defectsFinder.SearchDefects();
                    using (Scratches dust = new Scratches(ImageBefor, defectsFinder.LargeDefectsContoursMatrix))
                    {
                        CvInvoke.FillPoly(ImageAfter, defectsFinder.LargeDefectsContoursMatrix, new MCvScalar(255, 0, 255), LineType.FourConnected);
                        ImageAfter = dust.RemoveScrates();
                    }
                }
            }
            else
            {
                throw new ArgumentNullException(nameof(ImageBefor));
            }
        }
        #endregion Scratches

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

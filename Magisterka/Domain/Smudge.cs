﻿using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Drawing;

namespace Domain
{
    public class Smudge : IDisposable
    {
        enum BgrColor { Blue = 0, Green = 1, Red = 2 };       
        public double BlueTone
        {
            get
            {
                if (_blueTone == 0.0)
                {
                    SetColorProportions();
                }
                return _blueTone;
            }
        }
        public double GreenTone
        {
            get
            {
                if (_greenTone == 0.0)
                {
                    SetColorProportions();
                }
                return _greenTone;
            }
        }
        public double RedTone
        {
            get
            {
                if (_redTone == 0.0)
                {
                    SetColorProportions();
                }
                return _redTone;
            }
        }

        private Image<Bgr, byte> _image;

        private double _blueTone;
        private double _greenTone;
        private double _redTone;

        public Smudge(Image<Bgr, byte> image)
        {
            _image = image.Copy();
        }

        public void SetColorProportions()
        {
            Bgr bgr = new Bgr();
            MCvScalar mCvScalar = new MCvScalar();
            double blue = 0, green = 0, red = 0, sum = 1;
            _image.AvgSdv(out bgr, out mCvScalar);

            blue = bgr.Blue;
            green = bgr.Green;
            red = bgr.Red;

            sum = blue + green + red;
            _blueTone = blue / sum;
            _greenTone = green / sum;
            _redTone = red / sum;
        }

        private void RepairColor(ref Image<Bgr, byte> cleanedImage, Image<Gray, byte> grayImage, Image<Gray, byte> generalImageMask, BgrColor bgrColor, double colorTone, CmpType cmpType)
        {
            using(Image<Gray,byte>defectsMask = CreateMaskOfOverInappropriateColorProportions(grayImage,cleanedImage/*_image*/, colorTone, bgrColor, cmpType))
            {
                using (Image<Gray, byte> repairMask = generalImageMask.Mul(defectsMask))
                {                    
                    Image<Bgr, byte> cleanedPatchImage = cleanedImage.CopyBlank();
                    cleanedPatchImage[0] = cleanedImage[(int)bgrColor].Mul(BlueTone * 3);
                    cleanedPatchImage[1] = cleanedImage[(int)bgrColor].Mul(GreenTone * 3);
                    cleanedPatchImage[2] = cleanedImage[(int)bgrColor].Mul(RedTone * 3);

                    cleanedImage = MorphologicalProcessing.CombineTwoImages(cleanedImage, cleanedPatchImage, repairMask);
                }
            }
        }
        private Image<Gray, byte> CreateMaskOfOverInappropriateColorProportions(Image<Gray, byte> grayImage, Image<Bgr, byte> image, double tone, BgrColor color, CmpType cmpType, double margin=0.00)
        {
            double interval = 0;
            float count = (image?.CountNonzero()[(int)color]) ?? 1;
            float nonZeroCount;
            Image<Gray, byte> resultMask = grayImage.CopyBlank();

            switch (cmpType)
            {
                case CmpType.LessThan: { interval = Math.Round((1 - margin) * tone, 3); }break;                    
                case CmpType.GreaterThan: { interval = Math.Round((1 + margin) * tone, 3); } break;
                default: { interval = tone; }break;
            }

            using (Image<Gray, byte> model = grayImage.Mul(interval * 3.0))
            {
                using (Image<Gray, byte> cmpImage = _image[(int)color].Cmp(model[0], cmpType))
                {
                    using (Image<Gray, byte> disColorMask = cmpImage.ThresholdBinary(new Gray(1), new Gray(255)))
                    {                        
                        Mat kernel = CvInvoke.GetStructuringElement(ElementShape.Ellipse, new Size(3, 3), new Point(-1, -1));
                        CvInvoke.MorphologyEx(disColorMask, resultMask, MorphOp.Open, kernel, new Point(-1, -1), 1, BorderType.Replicate, new MCvScalar(1.0));

                        nonZeroCount = resultMask[0]?.CountNonzero()[0] ?? 0;
                    }                                       
                }
            }
            
            if (nonZeroCount/count>0.20 && margin < 1.0)
            {
                margin += 0.05;
                return CreateMaskOfOverInappropriateColorProportions(grayImage, image, tone, color, cmpType, margin);
            }else
            {
                return resultMask;
            }
        }

        public Image<Bgr, byte> CleanSmudges()
        {
            SetColorProportions();
            Image<Bgr, byte> cleanedImage = _image.Copy();

            using (Image<Gray, byte> grayImage = _image.Convert<Gray, byte>().Copy())
            {
                #region BritherRegions
                using (Image<Gray, byte> bwGeneralMask = MorphologicalProcessing.GeneralBluredBinaryImage(_image))
                {                   
                    #region Blue
                    RepairColor(ref cleanedImage, grayImage, bwGeneralMask, BgrColor.Blue, BlueTone, CmpType.GreaterThan);
                    #endregion Blue
                    #region Green
                    RepairColor(ref cleanedImage, grayImage, bwGeneralMask, BgrColor.Green, GreenTone, CmpType.GreaterThan);
                    #endregion Green
                    #region Red
                    RepairColor(ref cleanedImage, grayImage, bwGeneralMask, BgrColor.Red, RedTone, CmpType.GreaterThan);
                    #endregion Red

                    using (Image<Gray, byte> bwGeneralMaskNegativ = MorphologicalProcessing.GenerateBinaryImageNegative(bwGeneralMask))
                    {
                        #region Blue
                        RepairColor(ref cleanedImage, grayImage, bwGeneralMaskNegativ, BgrColor.Blue, BlueTone, CmpType.LessThan);
                        #endregion Blue
                        #region Green
                        RepairColor(ref cleanedImage, grayImage, bwGeneralMaskNegativ, BgrColor.Green, GreenTone, CmpType.LessThan);
                        #endregion Green
                        #region Red
                        RepairColor(ref cleanedImage, grayImage, bwGeneralMaskNegativ, BgrColor.Red, RedTone, CmpType.LessThan);
                        #endregion Red
                    }
                }
                #endregion BritherRegions
            }

            //cleanedImage = AligneColor(cleanedImage);
            //CvInvoke.MedianBlur(cleanedImage.Image, cleanedImage.Image, 3);

            return cleanedImage;
        }
        
        private Image<Bgr, byte> AligneColor(Image<Bgr, byte> image)
        {
            using (Image<Gray, byte> grayImage = image.Convert<Gray,byte>().Copy())
            {
                image[(int)BgrColor.Blue] = grayImage.Mul(BlueTone * 3);
                image[(int)BgrColor.Green] = grayImage.Mul(GreenTone * 3);
                image[(int)BgrColor.Red] = grayImage.Mul(RedTone * 3);
            }
            return image;
        }

        public void Dispose()
        {
            _image.Dispose();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}
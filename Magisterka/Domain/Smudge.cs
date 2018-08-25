using Emgu.CV;
using Emgu.CV.Structure;
using System;

namespace Domain
{
    public class Smudge: IDisposable
    {
        enum BgrColor { Blue=0, Green=1, Red=2 };
        public ImageWrapper<Gray, byte> MaskOfSmudges
        {
            get
            {
                if(_maskOfSmudges==null)
                {
                    SearchSmudges();
                }
                return _maskOfSmudges;
            }
        }

        public double BlueTone
        {
            get
            {
                if (_blueTone == 0.0)
                {
                    SearchSmudges();
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
                    SearchSmudges();
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
                    SearchSmudges();
                }
                return _redTone;
            }
        }

        private ImageWrapper<Bgr, byte> _image;
        private ImageWrapper<Gray, byte> _maskOfSmudges;

        private ImageWrapper<Gray, byte> _blueMaskOfSmudges;
        private ImageWrapper<Gray, byte> _greenMaskOfSmudges;
        private ImageWrapper<Gray, byte> _redMaskOfSmudges;

        private double _blueTone;
        private double _greenTone;
        private double _redTone;
        private double _margin = 0.20;

        public Smudge(ImageWrapper<Bgr, byte> image)
        {
            _image = image.Copy();
        }

        public void SetColorProportions()
        {
            Bgr bgr = new Bgr();
            MCvScalar mCvScalar = new MCvScalar();
            double blue = 0, green = 0, red = 0, sum = 1;
            _image.Image.AvgSdv(out bgr, out mCvScalar);

            blue = bgr.Blue;
            green = bgr.Green;
            red = bgr.Red;

            sum = blue + green + red;
            _blueTone = blue / sum;
            _greenTone = green / sum;
            _redTone=red / sum;
        }

        public void OtherColorDetector()        
        {
            _blueMaskOfSmudges = _image.Convert<Gray, byte>().CopyBlank();
            _greenMaskOfSmudges = _image.Convert<Gray, byte>().CopyBlank();
            _redMaskOfSmudges = _image.Convert<Gray, byte>().CopyBlank();

            double blueMin = Math.Round(BlueTone - _margin * BlueTone, 2);
            double blueMax = Math.Round(BlueTone + _margin * BlueTone, 2);

            double greenMin = Math.Round(GreenTone - _margin * GreenTone, 2);
            double greenMax = Math.Round(GreenTone + _margin * GreenTone, 2);

            double redMin = Math.Round(RedTone - _margin * RedTone, 2);
            double redMax = Math.Round(RedTone + _margin * RedTone, 2);

            double blueCont = 0.0;
            double greenCont = 0.0;
            double redCont = 0.0;
            double sum;

            ProgressManager.AddSteps(_image.Image.Height / 100);

            for (int x = 0; x < _image.Image.Height; x++)
            {
                for (int y = 0; y < _image.Image.Width; y++)
                {
                    int blue = _image.Image.Data[x, y, (int)BgrColor.Blue];
                    int green = _image.Image.Data[x, y, (int)BgrColor.Green];
                    int red = _image.Image.Data[x, y, (int)BgrColor.Red];

                    sum = blue + green + red;

                    try
                    {
                        blueCont = blue / sum;
                        greenCont = green / sum;
                        redCont = red / sum;
                    }
                    catch (DivideByZeroException)
                    {
                        sum = 1;
                        continue;
                    }

                    //if (greenCont < greenMin || greenCont > greenMax ||
                    //    redCont < redMin || redCont > redMax ||
                    //    blueCont < blueMin || blueCont > blueMax)
                    //{
                    //    _maskOfSmudges.Image.Data[x, y, 0] = 255;
                    //}

                    if (blueCont > blueMax)
                    {
                        _blueMaskOfSmudges.Image.Data[x, y, 0] = 255;
                    }

                    if (greenCont > greenMax)
                    {
                        _greenMaskOfSmudges.Image.Data[x, y, 0] = 255;
                    }                    

                    if (redCont > redMax)
                    {
                        _redMaskOfSmudges.Image.Data[x, y, 0] = 255;
                    }

                }
                if (x % 100 == 0)
                    ProgressManager.DoStep();
            }
        }

        private void SearchSmudges()
        {
            SetColorProportions();
            OtherColorDetector();
            //_maskOfSmudges = MorphologicalProcessing.Erode(_maskOfSmudges.Convert<Bgr, byte>(), new Size(3, 3), 1).Convert<Gray, byte>();
            ProgressManager.DoStep();           
        }

        public ImageWrapper<Bgr, byte> ClearOtherColorsSmudges()
        {
            ImageWrapper<Bgr, byte> cleanedImage = _image.Copy();
            OtherColorDetector();
            cleanedImage = ClearColor(cleanedImage, _blueMaskOfSmudges, BgrColor.Blue);
            cleanedImage = ClearColor(cleanedImage, _greenMaskOfSmudges, BgrColor.Green);
            cleanedImage = ClearColor(cleanedImage, _redMaskOfSmudges, BgrColor.Red);
            return cleanedImage;
        }

        private ImageWrapper<Bgr,byte>ClearColor(ImageWrapper<Bgr,byte>image, ImageWrapper<Gray,byte>maskOfSmudges, BgrColor color)
        {
            Image<Gray, byte>[] splitedImages = _image.Image.Split();

            using (ImageWrapper<Bgr, byte> patch = _image.CopyBlank())
            {
                patch.Image[(int)BgrColor.Blue] = splitedImages[(int)color].Mul(BlueTone * 3);
                patch.Image[(int)BgrColor.Green] = splitedImages[(int)color].Mul(GreenTone * 3);
                patch.Image[(int)BgrColor.Red] = splitedImages[(int)color].Mul(RedTone * 3);

                image = MorphologicalProcessing.CombineTwoImages(image, patch, maskOfSmudges);
            }              
            return image;           
        }

        public ImageWrapper<Bgr, byte>AveragePictureColors()
        {            
            ImageWrapper<Bgr, byte> outputImage;
            ProgressManager.AddSteps(5);

            using (ImageWrapper<Bgr, byte> patternImage = _image.Convert<Gray, byte>().Copy().Convert<Bgr,byte>())
            {
                ProgressManager.DoStep();
                patternImage.Image[(int)BgrColor.Blue] = patternImage.Image[(int)BgrColor.Blue].Mul(BlueTone*3);
                patternImage.Image[(int)BgrColor.Green] = patternImage.Image[(int)BgrColor.Green].Mul(GreenTone*3);
                patternImage.Image[(int)BgrColor.Red] = patternImage.Image[(int)BgrColor.Red].Mul(RedTone*3);
                ProgressManager.DoStep();
                outputImage = MorphologicalProcessing.CombineTwoImages(_image, patternImage, MaskOfSmudges);
                ProgressManager.DoStep();
                return outputImage;
            }               
        }

        public ImageWrapper<Bgr,byte>Test()
        {            
            OtherColorDetector();
            //return MorphologicalProcessing.MultipleMaskAndImage(_image, _blueMaskOfSmudges);

            Image<Gray, byte>[] splitedImages = _image.Image.Split();


            ImageWrapper<Bgr, byte> patch = _image.CopyBlank();
            patch.Image[(int)BgrColor.Blue] = splitedImages[2].Mul(BlueTone * 3);
            patch.Image[(int)BgrColor.Green] = splitedImages[2].Mul(GreenTone * 3);
            patch.Image[(int)BgrColor.Red] = splitedImages[2].Mul(RedTone * 3);
            return new ImageWrapper<Bgr, byte>(_image.Image.AbsDiff(patch.Image));

            OtherColorDetector();
            splitedImages = _image.Image.Split();
            _image.Image[(int)BgrColor.Blue] = splitedImages[2].Mul(BlueTone * 3);
            _image.Image[(int)BgrColor.Green] = splitedImages[2].Mul(GreenTone * 3);
            _image.Image[(int)BgrColor.Red] = splitedImages[2].Mul(RedTone * 3);

            return _image;
        }

        public void Dispose()
        {
            _image.Dispose();
            _maskOfSmudges.Dispose();
        }
    }
}
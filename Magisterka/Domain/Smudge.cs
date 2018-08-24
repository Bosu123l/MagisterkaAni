using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Drawing;
using static Domain.ImageColor;

namespace Domain
{
    public class Smudge: IDisposable
    {
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
        private double _margin = 0.10;

        public Smudge(ImageWrapper<Bgr, byte> image)
        {
            _image = image.Copy();
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
                    int blue = _image.Image.Data[x, y, (int)Colors.Blue];
                    int green = _image.Image.Data[x, y, (int)Colors.Green];
                    int red = _image.Image.Data[x, y, (int)Colors.Red];

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
            using (ImageColor imageColor = new ImageColor(_image))
            {
                ProgressManager.AddSteps(3);
                imageColor.QualifityPhotoTone();                
                _blueTone = imageColor.BlueTone;
                _greenTone = imageColor.GreenTone;
                _redTone = imageColor.RedTone;
                OtherColorDetector();
                //_maskOfSmudges = MorphologicalProcessing.Erode(_maskOfSmudges.Convert<Bgr, byte>(), new Size(3, 3), 1).Convert<Gray, byte>();
                ProgressManager.DoStep();
            }
        }

        public ImageWrapper<Bgr, byte> ClearOtherColorsSmudges()
        {
            ImageWrapper<Bgr, byte> cleanedImage = _image.Copy();
            OtherColorDetector();
            cleanedImage = ClearColor(cleanedImage, _blueMaskOfSmudges, Colors.Blue);
            cleanedImage = ClearColor(cleanedImage, _greenMaskOfSmudges, Colors.Green);
            cleanedImage = ClearColor(cleanedImage, _redMaskOfSmudges, Colors.Red);
            return cleanedImage;
        }

        private ImageWrapper<Bgr,byte>ClearColor(ImageWrapper<Bgr,byte>image, ImageWrapper<Gray,byte>maskOfSmudges, Colors color)
        {
            Image<Gray, byte>[] splitedImages = _image.Image.Split();

            using (ImageWrapper<Bgr, byte> patch = _image.CopyBlank())
            {
                patch.Image[(int)Colors.Blue] = splitedImages[(int)color].Mul(BlueTone * 3);
                patch.Image[(int)Colors.Green] = splitedImages[(int)color].Mul(GreenTone * 3);
                patch.Image[(int)Colors.Red] = splitedImages[(int)color].Mul(RedTone * 3);

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
                patternImage.Image[(int)Colors.Blue] = patternImage.Image[(int)Colors.Blue].Mul(BlueTone*3);
                patternImage.Image[(int)Colors.Green] = patternImage.Image[(int)Colors.Green].Mul(GreenTone*3);
                patternImage.Image[(int)Colors.Red] = patternImage.Image[(int)Colors.Red].Mul(RedTone*3);
                ProgressManager.DoStep();
                outputImage = MorphologicalProcessing.CombineTwoImages(_image, patternImage, MaskOfSmudges);
                ProgressManager.DoStep();
                return outputImage;
            }               
        }

        public void Dispose()
        {
            _image.Dispose();
            _maskOfSmudges.Dispose();
        }
    }
}
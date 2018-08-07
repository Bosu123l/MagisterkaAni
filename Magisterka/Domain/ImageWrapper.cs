using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Drawing;

namespace Domain
{
    public class ImageWrapper<TColor, TDepth> : IDisposable
        where TColor : struct, IColor
        where TDepth : new()
    {
        public Image<TColor, TDepth> Image
        {
            get { return _image; }
            set
            {
                if (value != null && value != _image)
                {
                    _image = value.Copy();
                }
            }
        }
        public Bitmap Bitmap
        {
           get { return this.Image.Bitmap; }
        }
        private Image<TColor, TDepth> _image;

        #region ctor
        public ImageWrapper()
        {
        }
        public ImageWrapper(string fileName)
        {
            this.Image = new Image<TColor, TDepth>(fileName);
        }
        public ImageWrapper(Image<TColor, TDepth> image)
        {
            if (image == null || image.Data == null)
                throw new ArgumentNullException();

            _image = image.Copy();
        }
        public ImageWrapper(ImageWrapper<TColor, TDepth> imageWrapper)
        {
            if (imageWrapper.Image == null || imageWrapper.Image?.Data == null)
                throw new ArgumentNullException(nameof(imageWrapper));

            this.Image = imageWrapper.Image.Copy();
        }
        #endregion ctor

        #region Methods
        public ImageWrapper<TOtherColor, TOtherDepth> Convert<TOtherColor, TOtherDepth>()
           where TOtherColor : struct, IColor
           where TOtherDepth : new()
        {
            return new ImageWrapper<TOtherColor, TOtherDepth>(this.Image.Convert<TOtherColor, TOtherDepth>());
        }
        public ImageWrapper<TColor, TDepth>Copy()
        {
            if (this.Image.Data == null || this.Image == null)
                throw new ArgumentNullException(nameof(this.Image));

            return new ImageWrapper<TColor, TDepth>(this);
        }
        public ImageWrapper<TColor, TDepth>CopyBlank()
        {           
            return new ImageWrapper<TColor, TDepth>(this.Image.CopyBlank());        
        }
        public ImageWrapper<TColor, TDepth>ThresholdBinary(TColor threashold, TColor maxValue)
        {            
            return new ImageWrapper<TColor, TDepth>(this.Image.ThresholdBinary(threashold, maxValue));
        }
        public ImageWrapper<TColor, TDepth>MorphologyEx(MorphOp operation, IInputArray kernel, Point anchor, int iterations, BorderType borderType, MCvScalar borderValue)
        {
            return new ImageWrapper<TColor,TDepth>(this.Image.MorphologyEx(operation, kernel, anchor, iterations, borderType, borderValue));
        }
        public ImageWrapper<TColor, TDepth> Mul(ImageWrapper<TColor, TDepth> img2)
        {
            return new ImageWrapper<TColor, TDepth>(this.Image.Mul(img2.Image));
        }
        public ImageWrapper<TColor, float> Laplace(int apertureSize)
        {
            return new ImageWrapper<TColor, float>(this.Image.Laplace(apertureSize));
        }
        public ImageWrapper<TColor, TDepth> Add(ImageWrapper<TColor, TDepth> img2)
        {
            return new ImageWrapper<TColor, TDepth>(this.Image.Add(img2.Image));
        }
        public ImageWrapper<TColor, TDepth> Not()
        {
            return new ImageWrapper<TColor, TDepth>(this.Image.Not());
        }
        public ImageWrapper<Gray, byte> Canny(double thresh, double threshLinking)
        {
            return new ImageWrapper<Gray, byte>(this.Image.Canny(thresh, threshLinking));
        }     
        public ImageWrapper<TColor, TDepth> SmoothBlur(int width, int height)
        {
            return new ImageWrapper<TColor, TDepth>(this.Image.SmoothBlur(width, height));
        }
        public void Save(string fileName)
        {
            this.Image.Save(fileName);
        }
        #endregion Methods

        #region Dispose
        public void Dispose()
        {
            if (_image.Mat != null) _image.Mat.Dispose();
            if (_image != null) _image.Dispose();

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
        #endregion Dispose
    }
}

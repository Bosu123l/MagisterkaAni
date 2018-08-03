using Emgu.CV;
using System;

namespace Domain
{
    public class ImageWrapper<TColor, TDepth> : IDisposable
        where TColor : struct, IColor
        where TDepth : new()
    {
        private Image<TColor, TDepth> _image;
        public Image<TColor, TDepth> Image
        {
            get { return _image; }
            set
            {
                if(value!=null && value!=_image)
                {
                    _image = value;
                }
            }
        }
        public ImageWrapper()
        {
        }

        public ImageWrapper(Image<TColor, TDepth> image)
        {
            if (image == null || image.Data==null)
                throw new ArgumentNullException();

            _image = image;
        }   
    
        public void Dispose()
        {
            if (_image.Mat != null) _image.Mat.Dispose();
            if (_image != null) _image.Dispose();

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}

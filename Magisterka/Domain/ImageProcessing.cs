using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace Domain
{
    public class ImageProcessing : IDisposable
    {       
        private Image<Bgr, byte> _imageBefor;
        private Image<Bgr, byte> _imageAfter;

        public Image<Bgr, byte> ImageBefor
        {
            get
            {
                return _imageBefor;
            }
        }
        public Image<Bgr, byte> ImageAfter
        {
            get
            {
                return _imageAfter;
            }
        }

        public BitmapImage BitmapImageBefor
        {
            get
            {
                return BitmapToImageSource(_imageBefor.Bitmap);
            }
        }
        public BitmapImage BitmapImageAfter
        {
            get
            {
                return BitmapToImageSource(_imageAfter.Bitmap);
            }
        }

        private DefectsFinder _defectsFinder;

        public ImageProcessing()
        { }
        public ImageProcessing(Image<Bgr, byte> image)
        {            
            if (image != null)
            {
                _imageBefor = image;
                _imageAfter = image;
            }else
            {                
                throw new ArgumentNullException();
            }
        }

        public static BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();
                return bitmapimage;
            }
        }

        public void ReduceDust()
        {
            if(_defectsFinder==null)
            {
                _defectsFinder = new DefectsFinder(ImageBefor);
            }

            Dust dustRemoval = new Dust(ImageAfter, _defectsFinder.MaskOfDefects, _defectsFinder.SmallDefectsContoursMatrix);
            _imageAfter=dustRemoval.RemoveDust();
        }

        public void CutImage() { }
        public void ReduceSmudges() { }

        public void Test()
        {
            if (_defectsFinder == null)
            {
                _defectsFinder = new DefectsFinder(ImageBefor);
            }
            //_imageAfter = _defectsFinder.SearchDefects();//MorphologicalProcessing.Erode(_imageAfter, new Size(2,2), 1);
        }

        public void Dispose()
        {
            if(_imageBefor!=null)
            {
                _imageBefor.Dispose();
            }
            if(_imageAfter!=null)
            {
                _imageAfter.Dispose();
            }
        }
    }
}

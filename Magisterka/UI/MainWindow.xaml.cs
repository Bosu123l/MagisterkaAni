using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Emgu.CV;
using Emgu.CV.Structure;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using Domain;

namespace UI
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public BitmapImage CleanedImage
        {
            get
            {
                return _cleanedImage;
            }
            set
            {
                if (value != _cleanedImage)
                {
                    _cleanedImage = value;
                    OnPropertyChanged(nameof(CleanedImage));
                }
            }
        }

        private BitmapImage _cleanedImage;
        private Image<Bgr, byte> _imageBefor;
        private Image<Bgr, byte> _imageAfter;

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void PS_ImageReceiver(object sender, System.Drawing.Image e)
        {
            int i = 0;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private static BitmapImage BitmapToImageSource(Bitmap bitmap)
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

        private void _openFromFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog()
            {
                DefaultExt = ".jpg",
                Filter = "TIF (.tif)|*.tif|TIFF  (.tiff)|*.tiff"
            };

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                string filename = dlg.FileName;
                _imageBefor = new Image<Bgr, byte>(filename);
                _imageAfter = _imageBefor;
                CleanedImage=BitmapToImageSource(_imageBefor.ToBitmap());
            }
        }

        private void _scanPhoto_Click(object sender, RoutedEventArgs e)
        {            
            try
            {
                using (PhotoScanner PS = new PhotoScanner())
                {
                    PS.ImageReceiver += PS_ImageReceiver;
                    PS.Show();
                }

            }
            catch (Exception exception)
            {
                throw;
            }            
        }

        private void _savePhoto_Click(object sender, RoutedEventArgs e)
        {
            _imageAfter.Save("C:\\Users\\Ania\\Desktop\\Edytowany" + DateTime.Now.Ticks + ".tif");
        }

        private void _dustReduction_Click(object sender, RoutedEventArgs e)
        {
            DustRemoval dr = new DustRemoval(_imageBefor);
            _imageAfter = dr.RemoveDust().Convert<Bgr, byte>();
            CleanedImage = BitmapToImageSource(_imageAfter.ToBitmap());
        }

        private void _savePhotoAs_Click(object sender, RoutedEventArgs e)
        {

        }

        private void _cutPhoto_Click(object sender, RoutedEventArgs e)
        {
            CutPhoto cp = new CutPhoto(_imageAfter);
            _imageAfter = cp.AlignPhoto(_imageBefor);
            //_imageAfter = cp.SetLines();
            //_imageAfter = cp.Cut(_imageAfter);
            //_imageBefor = cp.Cut(_imageBefor);

            CleanedImage = BitmapToImageSource(_imageAfter.ToBitmap());            
        }
        
        private void _smudgeCleaner_Click(object sender, RoutedEventArgs e)
        {
            SmudgeCleaner sc = new SmudgeCleaner(_imageAfter);
            _imageAfter = sc.OtherColorDetector();
            CleanedImage = BitmapToImageSource(_imageAfter.ToBitmap());
        }

        private void _preview_Click(object sender, RoutedEventArgs e)
        {
            PreviewWindow PW = new PreviewWindow(CleanedImage);
            PW.Show();
        }

        private void _showOryginal_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

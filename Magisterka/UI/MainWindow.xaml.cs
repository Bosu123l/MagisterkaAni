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
        public BitmapImage OryginalImage
        {
            get
            {
                return _oryginalImage;
            }
            set
            {
                if (value != _oryginalImage)
                {
                    _oryginalImage = value;
                    OnPropertyChanged(nameof(OryginalImage));
                }
            }
        }

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

        private BitmapImage _oryginalImage;
        private BitmapImage _cleanedImage;
        private Image<Bgr, byte> _bitmapImageBefor;
        private Image<Bgr, byte> _bitmapImageAfter;

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void DustReductionButton_Click(object sender, RoutedEventArgs e)
        {
            DustRemoval dr = new DustRemoval(_bitmapImageBefor);
            _bitmapImageAfter = dr.RemoveDust().Convert<Bgr, byte>();
            CleanedImage = BitmapToImageSource(_bitmapImageAfter.ToBitmap());
        }

        private void BCut_Click(object sender, RoutedEventArgs e)//Przyciecie zdjecia
        {
            CutPhoto cp = new CutPhoto(_bitmapImageAfter);
            _bitmapImageAfter = cp.AlignPhoto(_bitmapImageBefor);
            //_bitmapImageAfter = cp.SetLines();
            //_bitmapImageAfter = cp.Cut(_bitmapImageAfter);
            //_bitmapImageBefor = cp.Cut(_bitmapImageBefor);

            CleanedImage = BitmapToImageSource(_bitmapImageAfter.ToBitmap());
            OryginalImage = BitmapToImageSource(_bitmapImageBefor.ToBitmap());
        }

        private void BSmudgeClaner_Click(object sender, RoutedEventArgs e)
        {
            SmudgeCleaner sc = new SmudgeCleaner(_bitmapImageAfter);
            _bitmapImageAfter = sc.OtherColorDetector();
            CleanedImage = BitmapToImageSource(_bitmapImageAfter.ToBitmap());
        }

        private void BSave_Click(object sender, RoutedEventArgs e)
        {
            _bitmapImageAfter.Save("C:\\Users\\Ania\\Desktop\\Edytowany" + DateTime.Now.Ticks + ".tif");
        }

        private void BPreview_Click(object sender, RoutedEventArgs e)
        {
            PreviewWindow PW = new PreviewWindow(CleanedImage);
            PW.Show();
        }

        private void BPhotoScanner_Click(object sender, RoutedEventArgs e)
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

        private void PS_ImageReceiver(object sender, System.Drawing.Image e)
        {
            int i = 0;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public static BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                //bitmap = b
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

        private void BrowsePictureButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog()
            {
                DefaultExt = ".jpg",
                Filter = "Pictures (.tif)|*.tif"
            };


            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                string filename = dlg.FileName;
                BrowsePictureURL.Content = filename;

                _bitmapImageAfter = new Image<Bgr, byte>(filename);
                _bitmapImageBefor = _bitmapImageAfter;
                CleanedImage = OryginalImage = BitmapToImageSource(_bitmapImageBefor.ToBitmap());
            }
        }
    }
}

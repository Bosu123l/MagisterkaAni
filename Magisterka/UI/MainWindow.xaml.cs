using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Emgu.CV;
using Emgu.CV.Structure;
using Domain;
using System.Diagnostics;
using System.Windows.Forms;

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

        public string ScanningPath
        {
            get
            {
                return _scanningPath;
            }
            set
            {
                if (value != _scanningPath)
                {
                    _scanningPath = value;
                    OnPropertyChanged(nameof(ScanningPath));
                }
            }
        }

        public string BlockControls
        {
            get
            {
                return EnableControl ? "True" : "False";
            }
        }

        public bool EnableControl
        {
            get
            {
                return _enableControl;
            }
            set
            {
                if (value != _enableControl)
                {
                    _enableControl = value;
                    OnPropertyChanged(nameof(BlockControls));
                }
            }
        }

        private BitmapImage _cleanedImage;
        private Image<Bgr, byte> _imageBefor;
        private Image<Bgr, byte> _imageAfter;
        private string _scanningPath;
        private bool _enableControl;      

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindow()
        {            
            InitializeComponent();
            EnableControl = true;
            ScanningPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "Scanner");
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

        private void openFromFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EnableControl = false;

                Image<Bgr,byte> image = FileOperations.GetImageFromDirectory();
                if(image!=null)
                {
                    _imageBefor = image;
                    _imageAfter = image;
                    CleanedImage = BitmapToImageSource(_imageBefor.ToBitmap());
                }                
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                EnableControl = true;
            }
        }

        private void scanPhoto_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EnableControl = false;
                Image<Bgr, byte> image = FileOperations.GetImageFromScanner(ScanningPath);
                if (image != null)
                {
                    _imageBefor = image;
                    _imageAfter = image;
                    CleanedImage = BitmapToImageSource(_imageBefor.ToBitmap());
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                EnableControl = true;
            }
        }

        private void savePhoto_Click(object sender, RoutedEventArgs e)
        {
            FileOperations.SaveImageFileAs(_imageAfter);
        }

        private void dustReduction_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EnableControl = false;
                DustRemoval dr = new DustRemoval(_imageBefor);
                _imageAfter = dr.RemoveDust();
                CleanedImage = BitmapToImageSource(_imageAfter.ToBitmap());
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                EnableControl = true;
            }
        }

        private void savePhotoAs_Click(object sender, RoutedEventArgs e)
        {
            FileOperations.SaveImageFileAs(_imageAfter);
        }

        private void cutPhoto_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EnableControl = false;
                CutPhoto cp = new CutPhoto(_imageAfter);
                _imageAfter = cp.AlignPhoto(_imageBefor);
                //_imageAfter = cp.SetLines();
                //_imageAfter = cp.Cut(_imageAfter);
                //_imageBefor = cp.Cut(_imageBefor);

                CleanedImage = BitmapToImageSource(_imageAfter.ToBitmap());        
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                EnableControl = true;
            }
               
        }
        
        private void smudgeCleaner_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EnableControl = false;
                SmudgeCleaner sc = new SmudgeCleaner(_imageAfter);
                _imageAfter = sc.OtherColorDetector();
                CleanedImage = BitmapToImageSource(_imageAfter.ToBitmap());
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                EnableControl = true;
            }
           
        }

        private void preview_Click(object sender, RoutedEventArgs e)
        {
            PreviewWindow PW = new PreviewWindow(CleanedImage);
            PW.Show();
        }

        private void showOryginal_Click(object sender, RoutedEventArgs e)
        {

        }

        private void scanPath_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();            
            DialogResult result = dialog.ShowDialog();

            if(result==System.Windows.Forms.DialogResult.OK)
            {
                ScanningPath = dialog.SelectedPath;
            }
        }
    }
}

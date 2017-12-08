using Domain;
using Saraff.Twain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows;

namespace ScannerManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged, IDisposable
    {
        public event EventHandler<Image> ImageReceiver;
        public event PropertyChangedEventHandler PropertyChanged;

        #region Property

        public List<string> Scanners
        {
            get
            {
                return _scanners;
            }
            set
            {
                if (_scanners != value)
                {
                    _scanners = value;
                    OnPropertyChanged(nameof(Scanners));
                }
            }
        }

        public string SelectedScanner
        {
            get;
            set;
        } 

        #endregion Property

        private Twain32 _twain32;        
        private List<string> _scanners;
        
        private void LoadParametes()
        {
            try
            {
                _twain32 = new Twain32();

                //_twain32.ShowUI = false;
                //_twain32.IsTwain2Enable = true;
                _twain32.OpenDSM();

                #region Get scanners

                for (int i = 0; i < _twain32.SourcesCount; i++)
                {
                    _scanners.Add(_twain32.GetSourceProductName(i));
                }
                #endregion  


                if (Scanners.Count > 0)
                    SelectedScanner = Scanners.FirstOrDefault();

                _twain32?.CloseDSM();
                _twain32?.CloseDataSource();
            }
            catch (Exception ex)
            {               
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        private void _twain32_AcquireError(object sender, Twain32.AcquireErrorEventArgs e)
        {
            MessageBox.Show(e.Exception.Message);
        }

        private void _twain32_EndXfer(object sender, Twain32.EndXferEventArgs e)
        {
            var _file = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), Path.ChangeExtension(Path.GetFileName(Path.GetTempFileName()), ".Tiff"));
            e.Image.Save(_file, ImageFormat.Tiff);
            Console.WriteLine();
            Console.WriteLine(string.Format("Saved in: {0}", _file));
            e.Image.Dispose();
        }

        private void _twain32_AcquireCompleted(object sender, EventArgs e)
        {
            //if (_twain32.ImageCount > 0)
            //    OnImageReceiver(_twain32.GetImage(0));
        }


        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void OnImageReceiver(Image image)
        {
            ImageReceiver?.Invoke(this, image);
        }

        public void Dispose()
        {
            _twain32?.Dispose();
        }

        private void BMakeScan_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private void CBScannersList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
           
        }

        public MainWindow()
        {          
             _scanners = new List<string>();
            InitializeComponent();            

            LoadParametes();          
        }

        private void _scannersListPreview_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            //try
            //{ 
            _twain32?.Dispose();
            _twain32 = new Twain32();
            _twain32.ShowUI = true;
            _twain32.IsTwain2Enable = true;


            _twain32.OpenDSM();
            _twain32.SourceIndex = Scanners.IndexOf(SelectedScanner);
            _twain32.OpenDataSource();
            //_twain32.Capabilities.XResolution.Set(100);
            //_twain32.Capabilities.YResolution.Set(100);
            //_twain32.Capabilities.PixelType.Set(TwPixelType.RGB);

            _twain32.EndXfer += _twain32_EndXfer;
            _twain32.AcquireCompleted += _twain32_AcquireCompleted;
            _twain32.AcquireError += _twain32_AcquireError;

            _twain32.Acquire();
            //}catch(Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}




            //_twain32.EndXfer += (object sender, Twain32.EndXferEventArgs e) => {
            //    try
            //    {
            //        //e to obrazek w pamieci

            //        var _file = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), Path.ChangeExtension(Path.GetFileName(Path.GetTempFileName()), ".Tiff"));
            //        e.Image.Save(_file, ImageFormat.Tiff);
            //        Console.WriteLine();
            //        Console.WriteLine(string.Format("Saved in: {0}", _file));
            //        e.Image.Dispose();
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine("{0}: {1}{2}{3}{2}", ex.GetType().Name, ex.Message, Environment.NewLine, ex.StackTrace);
            //    }
            //};
        }
    }
}

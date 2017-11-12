using Domain;
using Saraff.Twain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows;

namespace UI
{
    /// <summary>
    /// Interaction logic for PhotoScanner.xaml
    /// </summary>
    public partial class PhotoScanner : Window, IDisposable, INotifyPropertyChanged
    {
        public event EventHandler<Image> ImageReceiver;

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

        public List<string> Resolutions
        {
            get
            {
                return _resolutions;
            }
            set
            {
                if (_resolutions != value)
                {
                    _resolutions = value;
                    OnPropertyChanged("Resolutions");
                }
            }
        }

        public int SelectedResolutionIndex
        {
            get
            {
                return _selectedResolutionIndex;
            }
            set
            {
                if (_selectedResolutionIndex != value)
                {
                    _selectedResolutionIndex = value;
                    OnPropertyChanged(nameof(SelectedResolutionIndex));
                }
            }
        }

        public string SelectedResolution
        {
            get;
            set;
        }

        public string SelectedScanner
        {
            get;
            set;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Property

        private Twain32 _twain32;

        private int _selectedResolutionIndex;
        private List<string> _resolutions;
        private List<string> _scanners;

        public PhotoScanner()
        {
            SelectedResolutionIndex = -1;
            var scanners = new List<string>();
            Resolutions = new List<string>();

            InitializeComponent();

            try
            {

                _twain32 = new Twain32();
                _twain32.Country = Saraff.Twain.TwCountry.POLAND;
                _twain32.Language = Saraff.Twain.TwLanguage.POLISH;

                _twain32.ShowUI = false;
                _twain32.IsTwain2Enable = true;
               
                _twain32.OpenDSM();
                for (int i = 0; i < _twain32.SourcesCount; i++)
                    scanners.Add(_twain32.GetSourceProductName(i));

                Scanners = new List<string>(scanners);

                if (Scanners.Count > 0)
                {
                    SelectedResolution = Scanners.FirstOrDefault();
                }

                _twain32.AcquireCompleted += _twain32_AcquireCompleted;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        private void _twain32_AcquireCompleted(object sender, EventArgs e)
        {
            if (_twain32.ImageCount > 0)
                OnImageReceiver(_twain32.GetImage(0));
        }

        private void CBScannersList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var resolutions = new List<string>();
            try
            {
    
                if (_twain32.OpenDataSource())
                    _twain32.CloseDataSource();

                _twain32.OpenDataSource();

                _twain32.SourceIndex = Scanners.IndexOf(SelectedScanner);

                var xResolutions = _twain32.Capabilities.XResolution.Get();

                for (int i = 0; i < xResolutions.Count && !(Int32.Parse(xResolutions[i].ToString()) > 2400); i++)
                {
                    if (Int32.Parse(xResolutions[i].ToString()) % 50 == 0)
                    {
                        resolutions.Add(string.Format("{0} dpi", xResolutions[i]));
                    }
                }

                Resolutions = new List<string>(resolutions);
            }
            catch (Exception ex)
            {
                _twain32.CloseDataSource();
                //_twain32.CloseDSM();
                Resolutions = new List<string>();
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
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
            _twain32?.CloseDataSource();
            _twain32?.CloseDSM();
            _twain32?.Dispose();
        }

        private void BMakeScan_Click(object sender, RoutedEventArgs e)
        {
            _twain32.Capabilities.PixelType.Set(TwPixelType.RGB);

            _twain32.Acquire();

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
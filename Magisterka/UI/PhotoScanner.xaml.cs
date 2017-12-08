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

namespace UI
{
    /// <summary>
    /// Interaction logic for PhotoScanner.xaml
    /// </summary>
    public partial class PhotoScanner : Window, INotifyPropertyChanged, IDisposable
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
                return _selectedResolutions;
            }
            set
            {
                if (_selectedResolutions != value)
                {
                    _selectedResolutions = value;
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
        private List<string> _selectedResolutions;       
        private List<List<string>> _resolutions;//all resolution
        private List<string> _scanners;//list of all scanners

        public PhotoScanner()
        {
            SelectedResolutionIndex = -1;
            _scanners = new List<string>();
            _resolutions = new List<List<string>>();

            LoadParametes();

            InitializeComponent();
        }

        private void LoadParametes()
        {
            try
            {
                _twain32 = new Twain32();                

                _twain32.ShowUI = false;
                _twain32.IsTwain2Enable = true;
                _twain32.OpenDSM();


                for (int i = 0; i < _twain32.SourcesCount; i++)
                {
                    #region Get scanner
                    _scanners.Add(_twain32.GetSourceProductName(i));


                    #region Get resolutions of scanner

                        _resolutions.Add(new List<string>());

                        
                        _twain32.CloseDataSource();
                        _twain32.SourceIndex = Scanners.IndexOf(_twain32.GetSourceProductName(i));
                        _twain32.OpenDataSource();
                        Twain32.Enumeration xResolutions = _twain32.Capabilities.XResolution.Get();

                        if (xResolutions.Count < 20)
                        {
                            for (int j = 0; j < xResolutions.Count && !(Int32.Parse(xResolutions[j].ToString()) > 2400); j++)
                            {                              
                                _resolutions[i].Add(string.Format("{0}", xResolutions[j]));                                
                            }
                        }else
                            {
                                foreach (var _val in new float[] { 100f, 200f, 300f, 600f, 1200f, 2400f })
                                {
                                    _resolutions[i].Add(string.Format("{0}", _val));                                                              
                                }
                            }

                    #endregion

                    #endregion                   
                }

                _twain32?.CloseDSM();
                _twain32?.CloseDataSource();
                
                SelectedScanner = Scanners.FirstOrDefault();
            }
            catch (Exception ex)
            {
                //_twain32.CloseDataSource();                
                Resolutions = new List<string>();              
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
            if (_twain32.ImageCount > 0)
                OnImageReceiver(_twain32.GetImage(0));
        }

        private void CBScannersList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            int id = Scanners.IndexOf(SelectedScanner);

            _selectedResolutions = new List<string>(_resolutions[id]);
            _selectedResolutionIndex = int.Parse(_selectedResolutions.FirstOrDefault());
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
            //_twain32?.CloseDataSource();
            //_twain32?.CloseDSM();
            _twain32?.Dispose();
        }

        private void BMakeScan_Click(object sender, RoutedEventArgs e)
        {
            //try
            //{ 
                _twain32?.Dispose();
                _twain32 = new Twain32();
                _twain32.ShowUI = true;
                _twain32.IsTwain2Enable = true;


            //_twain32.OpenDSM();
            //_twain32.SourceIndex = Scanners.IndexOf(SelectedScanner);
            //_twain32.OpenDataSource();
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

        private void ComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            int i=SelectedResolutionIndex;
        }
    }
}
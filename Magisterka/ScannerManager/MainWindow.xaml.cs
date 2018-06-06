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
        private string _destinationPath;

        private void LoadParametes()
        {
            try
            {
                _twain32 = new Twain32();

                _twain32.ShowUI = true;
                _twain32.IsTwain2Enable = true;
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
                MessageBox.Show(ex.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                _twain32.Dispose();
                this.Close();
            }
        }

        #region ScanningEnd

        private void _twain32_AcquireError(object sender, Twain32.AcquireErrorEventArgs e)
        {

            MessageBox.Show(e.Exception.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            _twain32.Dispose();
            this.Close();

        }

        private void _twain32_EndXfer(object sender, Twain32.EndXferEventArgs e)
        {
            string fileName = $"{DateTime.Now.ToString("yyyyMMdd")}_SC";//poprawić date w sciezce
            string regex = @"*" + fileName + @"*.Tiff";
            int seqence = 1;
            string max;
            string[] files;

            files = Directory.EnumerateFiles(_destinationPath, regex).ToArray();

            try
            {
                if (files.Count() > 0)
                {
                    max = files.OrderBy(x => x).LastOrDefault();
                    max = max.Substring(max.Length - 4 - ".Tiff".Length, 4);               

                    int.TryParse(max, out seqence);
                    seqence = seqence + 1;                
                }

                fileName += seqence.ToString("D4");

                var _file = Path.Combine(_destinationPath, fileName+".Tiff");
                e.Image.Save(_file, ImageFormat.Tiff);          
           

                if(MessageBox.Show($"Zapisano obraz{fileName}.Tiff\n{_destinationPath}", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information)==MessageBoxResult.OK)
                {
                    e.Image.Dispose();
                    _twain32.Dispose();
                    this.Close();
                }            
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                _twain32.Dispose();
                this.Close();
            }            
        }

        private void _twain32_AcquireCompleted(object sender, EventArgs e)
        {
            if (_twain32.ImageCount > 0)
                OnImageReceiver(_twain32.GetImage(0));
        }

        #endregion ScanningEnd

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

        public MainWindow(string desinationPath)
        {
            _scanners = new List<string>();
            _destinationPath = desinationPath;

            InitializeComponent();           
            LoadParametes();          
        }

        private void _scannersListPreview_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                _twain32?.Dispose();
                _twain32 = new Twain32();
                _twain32.ShowUI = true;
                _twain32.IsTwain2Enable = true;


                _twain32.OpenDSM();
                _twain32.SourceIndex = Scanners.IndexOf(SelectedScanner);
                _twain32.OpenDataSource();

                _twain32.EndXfer += _twain32_EndXfer;
                _twain32.AcquireCompleted += _twain32_AcquireCompleted;
                _twain32.AcquireError += _twain32_AcquireError;

                _twain32.Acquire();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                _twain32.Dispose();
                Environment.ExitCode = (int)ExitCodes.ExitCode.ERROR_DEVICE_UNREACHABLE;
                this.Close();
            }            
        }

        private void bOK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _twain32.Dispose();
                Environment.ExitCode = (int)ExitCodes.ExitCode.SUCCESS;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                Environment.ExitCode = (int)ExitCodes.ExitCode.ERROR_FILE_NOT_FOUND;
                this.Close();
            }
            
        }

        private void bCancel_Click(object sender, RoutedEventArgs e)
        {
            _twain32.Dispose();
            Environment.ExitCode=(int)ExitCodes.ExitCode.ERROR_CANCELLED;
            this.Close();
        }
    }
}

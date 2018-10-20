using Domain;
using Saraff.Twain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ScannerManager
{
    public partial class MainWindow : Window, INotifyPropertyChanged, IDisposable
    {
        public event EventHandler<Image> ImageReceiver;
        public event PropertyChangedEventHandler PropertyChanged;
        
        private Image _scannedImage;

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
        private string _filePath;

        private void LoadParametes()
        {
            try
            {
                _twain32 = new Twain32
                {
                    ShowUI = true,
                    IsTwain2Enable = true
                };
                _twain32.OpenDSM();

                #region GetScanners

                for (int i = 0; i < _twain32.SourcesCount; i++)
                {
                    _scanners.Add(_twain32.GetSourceProductName(i));
                }
                #endregion GetScanners

                if (Scanners.Count > 0)
                    SelectedScanner = Scanners.FirstOrDefault();
            }
            catch (Exception ex)
            {
                var messagebox = CustomMessageBox.Show(ex.Message, "Error!", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Error);
                if (messagebox == System.Windows.Forms.DialogResult.Yes)
                {
                    _twain32?.CloseDSM();
                    _twain32?.CloseDataSource();
                    _twain32.Dispose();
                    Environment.ExitCode = (int)ExitCodes.ExitCode.ERROR_DEVICE_UNREACHABLE;
                    this.Close();
                }
            }
        }

        #region ScanningEnd

        private void _twain32_AcquireError(object sender, Twain32.AcquireErrorEventArgs e)
        {
            if (MessageBox.Show(e.Exception.Message, "Error!", MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes)
            {
                _twain32?.CloseDSM();
                _twain32?.CloseDataSource();
                _twain32.Dispose();
                Environment.ExitCode = (int)ExitCodes.ExitCode.ERROR_NOT_ENOUGH_MEMORY;
                this.Close();
            }
        }

        private void SaveImage()
        {
            string fileName = $"{DateTime.Now.ToString("yyyyMMdd")}_SC";//poprawić date w sciezce
            string regex = @"*" + fileName + @"*.Tiff";
            int seqence = 1;
            string max;
            string[] files;

            files = Directory.EnumerateFiles(_destinationPath, regex).ToArray();
         
            if (files.Count() > 0)
            {
                max = files.OrderBy(x => x).LastOrDefault();
                max = max.Substring(max.Length - 4 - ".Tiff".Length, 4);

                int.TryParse(max, out seqence);
                seqence = seqence + 1;
            }

            fileName += seqence.ToString("D4");
            _filePath = Path.Combine(_destinationPath, fileName + ".Tiff");
                                
            _scannedImage.Save(_filePath, ImageFormat.Tiff);           
        }

        private void _twain32_EndXfer(object sender, Twain32.EndXferEventArgs e)
        {
            try {

                _scannedImage = (Image)e.Image.Clone();                           
            }
            catch (Exception ex)
            {
                var messagebox = CustomMessageBox.Show(ex.Message, "Error!", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                if (messagebox == System.Windows.Forms.DialogResult.OK)
                {
                    Environment.ExitCode = (int)ExitCodes.ExitCode.ERROR_DEVICE_UNREACHABLE;
                }
            }finally
            {
                e.Image.Dispose();
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
            Environment.ExitCode = (int)ExitCodes.ExitCode.ERROR_FILE_NOT_FOUND;

            InitializeComponent();
            LoadParametes();
        }

        private void _scannersListPreview_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                _twain32?.Dispose();
                _twain32 = new Twain32
                {
                    ShowUI = true,
                    IsTwain2Enable = true
                };

                _twain32.OpenDSM();
                _twain32.SourceIndex = Scanners.IndexOf(SelectedScanner);
                _twain32.OpenDataSource();

                _twain32.EndXfer += _twain32_EndXfer;
                _twain32.AcquireCompleted += _twain32_AcquireCompleted;
                _twain32.AcquireError += _twain32_AcquireError;
                _twain32.MemXferEvent += _twain32_MemXferEvent;

                _twain32.Acquire();

                if(_scannedImage!=null)
                {
                    SaveImage();
                }

                Environment.ExitCode = (int)ExitCodes.ExitCode.SUCCESS;

                CustomMessageBox.Show($"Save photo: {Path.GetFileName(_filePath)}\n{_destinationPath}", "Succes", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);      
            }
            catch (Exception ex)
            {
                var messagebox = CustomMessageBox.Show(ex.Message, "Scanner Error!", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Error);
                if (messagebox == System.Windows.Forms.DialogResult.Yes)
                {
                    Environment.ExitCode = (int)ExitCodes.ExitCode.ERROR_DEVICE_UNREACHABLE;
                }
            }
            finally
            {
                _twain32?.CloseDSM();
                _twain32?.CloseDataSource();
                _twain32.Dispose();
                GC.Collect();
                GC.WaitForPendingFinalizers();
                this.Close();
            }
        }

        private void _twain32_MemXferEvent(object sender, Twain32.MemXferEventArgs e)
        {
            CustomMessageBox.Show("Memory error!", e.ToString(), System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
        }

        private void BCancel_Click(object sender, RoutedEventArgs e)
        {
            _twain32.Dispose();
            Environment.ExitCode = (int)ExitCodes.ExitCode.ERROR_CANCELLED;
            this.Close();
        }
    }
}

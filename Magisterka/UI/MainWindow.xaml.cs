using System;
using System.ComponentModel;
using System.Windows;
using Emgu.CV.Structure;
using Domain;
using System.Threading.Tasks;

namespace UI
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {    
        private ImageWrapper<Bgr,byte> _imgeView
        {
            set
            {
                ImgeView.ViewedImage=value;
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
        private bool _enableControl;

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindow()
        {
            EnableControl = true;            
            InitializeComponent();
            InitializeButtonsEvents();
        }

        private void InitializeButtonsEvents()
        {
            this.FileControl.GetPhotoFromScannerClicked += GetPhotoFromScanner;
            this.FileControl.GetPhotoFromFolderClicked += GetPhotoFromFile;
            this.FileControl.ChangeScanDestynationFolderClicked += SetScanPath;
            this.FileControl.SavePhotoAsClicked += SavePhotoAs;
            this.FileControl.SavePhotoClicked += SavePhoto;

            this.PhotoEditionControl.DustReductionClicked += DustReduction;
            this.PhotoEditionControl.SmudgeReductionClick += Morpho;
            this.PhotoEditionControl.CutPhotoClick += CutPhotoBorder;

            this.PhotoEditionControl.RotateImageClick += RotateImage;
            this.PhotoEditionControl.AlignImageValueChanged += AlignImageClick;


            this.ViewControl.OpenOldPhotoInNewWindowClicked += PreviewOrginalPhoto;
            this.ViewControl.OpenPhotoInNewWindowClicked += PreviewEditPhoto;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region GetPhotoToEdit
        private void GetPhotoFromFile(object sender, EventArgs e)
        {            
            try
            {
                EnableControl = false;

                using (var image = (FileOperations.GetImageFromDirectory()))
                {
                    if (image != null)
                    {                      
                       ImageProcessing.SetImage(image);
                       _imgeView = ImageProcessing.ImageAfter;                       
                    }
                }
            }
            catch (Exception ex)
            {                
                MessageBox.Show(ex.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                EnableControl = true;
            }
        }

        private void GetPhotoFromScanner(object sender, EventArgs e)
        {
            try
            {                
                EnableControl = false;
                using (var image = FileOperations.GetImageFromScanner())
                {
                    if (image != null)
                    { 
                        ImageProcessing.SetImage(image);
                        _imgeView = ImageProcessing.ImageAfter;   
                    }
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
        #endregion GetPhotoToEdit

        #region FileOperation
        private void SavePhoto(object sender, EventArgs e)
        {           
            try
            {           
                FileOperations.SaveImageFile(ImageProcessing.ImageAfter);                            
            }
            catch (Exception ex)
            {                
                MessageBox.Show(ex.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void SavePhotoAs(object sender, EventArgs e)
        {
            try
            {
                FileOperations.SaveImageFileAs(ImageProcessing.ImageAfter);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void SetScanPath(object sender, EventArgs e)
        {
            try
            {
                FileOperations.SetScanPath();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion FileOperation

        #region OperationsOnPhoto
        private async void DustReduction(object sender, EventArgs e)
        {
            ProgressBar progressBar = new ProgressBar();
            try
            {
                EnableControl = false;
                progressBar.Show();
                await Task.Run(() => {
                    ImageProcessing.ReduceDust();
                });
                _imgeView = ImageProcessing.ImageAfter;   
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ex.StackTrace, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                progressBar.Close();
                EnableControl = true;
            }
        }
        private void CutPhotoBorder(object sender, EventArgs e)
        {
            ProgressBar progressBar = new ProgressBar();
            try
            {
                EnableControl = false;
                progressBar.Show();
                Task.Run(() =>
                {
                    ImageProcessing.CutImage();
                });
                _imgeView = ImageProcessing.ImageAfter;   
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                progressBar.Close();
                EnableControl = true;
            }
        }
        private void SmudgeCleaner(object sender, EventArgs e)
        {
            ProgressBar progressBar = new ProgressBar();
            try
            {
                EnableControl = false;
                progressBar.Show();
                Task.Run(() =>
                {
                    
                });
                _imgeView = ImageProcessing.ImageAfter;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                progressBar.Close();
                EnableControl = true;
            }
        }
        private void RotateImage(object sender, EventArgs e)
        {
            try
            {
                EnableControl = false;
                ImageProcessing.RotateImage();
                _imgeView = ImageProcessing.ImageAfter;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                EnableControl = true;
            }
        }
        private void AlignImageClick(object sender, EventArgs e)
        {
            try
            {
                EnableControl = false;
                RoutedPropertyChangedEventArgs<double> args = (RoutedPropertyChangedEventArgs<double>)e;               
                ImageProcessing.AlignImage(args.NewValue);
                _imgeView = ImageProcessing.ImageAfter;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }    
            finally
            {
                EnableControl = true;
            }
        }

        private async void Morpho(object sender, EventArgs e)
        {
            ProgressBar progressBar = new ProgressBar();
            try
            {
                EnableControl = false;
                progressBar.Show();
                await Task.Run(() =>
                 {
                     ImageProcessing.Test();                         
                 });

                _imgeView = ImageProcessing.ImageAfter;
            }
            catch (Exception ex)
            {                
                System.Windows.MessageBox.Show(ex.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                progressBar.Close();
                EnableControl = true;
            }
        }
        #endregion OperationsOnPhoto

        #region ViewOperations
        private void PreviewEditPhoto(object sender, EventArgs e)
        {
            PreviewWindow PW = new PreviewWindow(ImageProcessing.ImageAfter.Bitmap);
            PW.Show();
        }
        private void PreviewOrginalPhoto(object sender, EventArgs e)
        {
            PreviewWindow PW = new PreviewWindow(ImageProcessing.ImageBefor.Bitmap);
            PW.Show();
        }

        #endregion ViewOperations     

        private void ShowProgressBar()
        {
            ProgressBar progressBar = new ProgressBar();
        }

        private void PhotoZoomBox_OnZoomScaleChange(object sender, EventArgs e)
        {

        }
    }
}

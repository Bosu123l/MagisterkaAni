using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Imaging;
using Emgu.CV.Structure;
using Domain;

namespace UI
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public BitmapImage ViewedImage
        {
            get
            {
                return ImageProcessing.BitmapImageAfter;
            }
            set
            {
               OnPropertyChanged(nameof(ViewedImage));                
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
            this.PhotoEditionControl.RotateImageLeftClick += RotateImageLeft;
            this.PhotoEditionControl.RotateImageRightClick += RotateImageRight;

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
                using (ImageWrapper<Bgr, byte> image = new ImageWrapper<Bgr, byte>(FileOperations.GetImageFromDirectory()))
                {
                    if (image != null)
                    {
                        ImageProcessing.SetImage(image);
                        OnPropertyChanged(nameof(ViewedImage));
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
                        OnPropertyChanged(nameof(ViewedImage));
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
        private void DustReduction(object sender, EventArgs e)
        {
            try
            {
                EnableControl = false;
                ImageProcessing.ReduceDust();
                OnPropertyChanged(nameof(ViewedImage));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ex.StackTrace, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                EnableControl = true;
            }
        }
        private void CutPhotoBorder(object sender, EventArgs e)
        {
            try
            {
                EnableControl = false;
                ImageProcessing.CutImage();
                OnPropertyChanged(nameof(ViewedImage));
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
        private void SmudgeCleaner(object sender, EventArgs e)
        {
            try
            {
                EnableControl = false;
                OnPropertyChanged(nameof(ViewedImage));
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
        private void RotateImageLeft(object sender, EventArgs e)
        { }
        private void RotateImageRight(object sender, EventArgs e)
        { }

        private void Morpho(object sender, EventArgs e)
        {
            try
            {
                EnableControl = false;
                ImageProcessing.Test();
                ViewedImage = ImageProcessing.BitmapImageAfter;
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
        #endregion OperationsOnPhoto

        #region ViewOperations
        private void PreviewEditPhoto(object sender, EventArgs e)
        {
            PreviewWindow PW = new PreviewWindow(ImageProcessing.BitmapImageAfter);
            PW.Show();
        }
        private void PreviewOrginalPhoto(object sender, EventArgs e)
        {
            PreviewWindow PW = new PreviewWindow(ImageProcessing.BitmapImageBefor);
            PW.Show();
        }

        #endregion ViewOperations       
    }
}

using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using Emgu.CV;
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
                return _viewedImage;
            }
            set
            {
                if (value != _viewedImage)
                {
                    _viewedImage = value;
                    OnPropertyChanged(nameof(ViewedImage));
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

        private BitmapImage _viewedImage;
        private ImageProcessing _imageProcessing;
        private bool _enableControl;

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindow()
        {
            InitializeComponent();
            EnableControl = true;

            _imageProcessing = new ImageProcessing();
            
            #region InitializeButtonsEvents
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
            #endregion InitializeButtonsEvents
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
                if (_imageProcessing != null)
                {
                    _imageProcessing.Dispose();
                }
                _imageProcessing = new ImageProcessing(FileOperations.GetImageFromDirectory());
                ViewedImage = _imageProcessing.BitmapImageAfter;
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
                if (_imageProcessing != null)
                {
                    _imageProcessing.Dispose();
                }
                _imageProcessing = new ImageProcessing(FileOperations.GetImageFromScanner());
                ViewedImage = _imageProcessing.BitmapImageAfter;               
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
                FileOperations.SaveImageFile(_imageProcessing.ImageAfter);
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
                FileOperations.SaveImageFileAs(_imageProcessing.ImageAfter);
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
                _imageProcessing.ReduceDust();
                ViewedImage = _imageProcessing.BitmapImageAfter;

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
        private void CutPhotoBorder(object sender, EventArgs e)
        {
            try
            {
                EnableControl = false;
                _imageProcessing.CutImage();
                ViewedImage = _imageProcessing.BitmapImageAfter;
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

                ViewedImage = _imageProcessing.BitmapImageAfter;
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
                _imageProcessing.Test();
                ViewedImage = _imageProcessing.BitmapImageAfter;
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
            PreviewWindow PW = new PreviewWindow(ViewedImage);
            PW.Show();
        }
        private void PreviewOrginalPhoto(object sender, EventArgs e)
        {

        }
        #endregion ViewOperations

       
       
    }
}

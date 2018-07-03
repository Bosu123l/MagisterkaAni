﻿using System;
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
                return Domain.FileOperations.ScanDirectory;
            }
            set
            {                
                OnPropertyChanged(nameof(ScanningPath));
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
        private bool _enableControl;

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindow()
        {
            InitializeComponent();
            EnableControl = true;
            ScanningPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "Scanner");
            
            #region InitializeButtonsEvents
            this.FileControl.GetPhotoFromScannerClicked += GetPhotoFromScanner;
            this.FileControl.GetPhotoFromFolderClicked += GetPhotoFromFile;
            this.FileControl.ChangeScanDestynationFolderClicked += SetScanPath;
            this.FileControl.SavePhotoAsClicked += SavePhotoAs;
            this.FileControl.SavePhotoClicked += SavePhoto;

            this.PhotoEditionControl.DustReductionClicked += DustReduction;
            this.PhotoEditionControl.SmudgeReductionClick += SmudgeCleaner;
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

                Image<Bgr, byte> image = FileOperations.GetImageFromDirectory();
                if (image != null)
                {
                    _imageBefor = image;
                    _imageAfter = image;
                    CleanedImage = Domain.ImageProcessing.BitmapToImageSource(_imageBefor.ToBitmap());
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
        private void GetPhotoFromScanner(object sender, EventArgs e)
        {
            try
            {
                EnableControl = false;
                Image<Bgr, byte> image = FileOperations.GetImageFromScanner();
                if (image != null)
                {
                    _imageBefor = image;
                    _imageAfter = image;
                    CleanedImage = Domain.ImageProcessing.BitmapToImageSource(_imageBefor.ToBitmap());
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
                FileOperations.SaveImageFile(_imageAfter);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void SavePhotoAs(object sender, EventArgs e)
        {
            try
            {
                FileOperations.SaveImageFileAs(_imageAfter);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void SetScanPath(object sender, EventArgs e)
        {
            try
            {
                FileOperations.SetScanPath();
                OnPropertyChanged(nameof(ScanningPath));
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }            
        }
        #endregion FileOperation

        #region OperationsOnPhoto
        private void DustReduction(object sender, EventArgs e)
        {
            try
            {
                EnableControl = false;
                DustRemoval dr = new DustRemoval(_imageBefor);
                _imageAfter = dr.RemoveDust();
                CleanedImage = Domain.ImageProcessing.BitmapToImageSource(_imageAfter.ToBitmap());
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
        private void CutPhotoBorder(object sender, EventArgs e)
        {
            try
            {
                EnableControl = false;
                CutPhoto cp = new CutPhoto(_imageAfter);
                _imageAfter = cp.AlignPhoto(_imageBefor);
                //_imageAfter = cp.SetLines();
                //_imageAfter = cp.Cut(_imageAfter);
                //_imageBefor = cp.Cut(_imageBefor);

                CleanedImage = Domain.ImageProcessing.BitmapToImageSource(_imageAfter.ToBitmap());
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
                SmudgeCleaner sc = new SmudgeCleaner(_imageAfter);
                _imageAfter = sc.OtherColorDetector();
                CleanedImage = Domain.ImageProcessing.BitmapToImageSource(_imageAfter.ToBitmap());
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
        #endregion OperationsOnPhoto

        #region ViewOperations
        private void PreviewEditPhoto(object sender, EventArgs e)
        {
            PreviewWindow PW = new PreviewWindow(CleanedImage);
            PW.Show();
        }
        private void PreviewOrginalPhoto(object sender, EventArgs e)
        {

        }
        #endregion ViewOperations
       
    }
}

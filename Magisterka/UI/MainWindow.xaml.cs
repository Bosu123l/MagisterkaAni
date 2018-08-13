using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Imaging;
using Emgu.CV.Structure;
using Domain;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Drawing;
using Emgu.CV.UI;

namespace UI
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public Image ViewedImage
        {
            get
            {
                return PhotoZoomBox.Image;
            }
            set
            {
                PhotoZoomBox.Image=value;
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
        private async void GetPhotoFromFile(object sender, EventArgs e)
        {
            ProgressBar progressBar = new ProgressBar(true);
            try
            {
                EnableControl = false;

                using (var image = (FileOperations.GetImageFromDirectory()))
                {
                    if (image != null)
                    {
                        progressBar.Show();
                        await Task.Run(() =>
                        {
                            ImageProcessing.SetImage(image);                            
                        });
                        ViewedImage = ImageProcessing.ImageAfter.Bitmap;                       
                    }
                }
            }
            catch (Exception ex)
            {                
                MessageBox.Show(ex.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                progressBar.Close();
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
                        ViewedImage = ImageProcessing.ImageAfter.Bitmap;
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
            ProgressBar progressBar = new ProgressBar();
            try
            {
                EnableControl = false;
                progressBar.Show();
                Task.Run(() => {
                    ImageProcessing.ReduceDust();
                });
                ViewedImage = ImageProcessing.ImageAfter.Bitmap;
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
                ViewedImage = ImageProcessing.ImageAfter.Bitmap;
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
                    ViewedImage = ImageProcessing.ImageAfter.Bitmap;
                });
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
        private void RotateImageLeft(object sender, EventArgs e)
        { }
        private void RotateImageRight(object sender, EventArgs e)
        { }

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
                     //ImageZoomPanel.Image = ImageProcessing.ImageAfter.Bitmap;
                     //ViewedImage = ImageProcessing.BitmapImageAfter;
                     //new HistogramForm(ImageProcessing.ImageAfter).Show();                     
                 });
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

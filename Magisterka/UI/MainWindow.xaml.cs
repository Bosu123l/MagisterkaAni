using System;
using System.ComponentModel;
using System.Windows;
using Emgu.CV.Structure;
using Domain;
using System.Threading.Tasks;
using Emgu.CV;
using System.Drawing;

namespace UI
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private Image<Bgr, byte> _imgeView
        {
            set
            {
                ImageView.ViewedImage = value;
            }
        }
        public string BlockControls
        {
            get
            {
                return EnableControl.ToString();
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

            ImageProcessing.ImageAfterChange += ImageProcessing_ImageAfterChange; 
        }

        private void ImageProcessing_ImageAfterChange(object sender, Image<Bgr, byte> e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                _imgeView = e;
            });
        }

        private void InitializeButtonsEvents()
        {
            this.FileControl.GetPhotoFromScannerClicked += GetPhotoFromScanner;
            this.FileControl.GetPhotoFromFolderClicked += GetPhotoFromFile;
            this.FileControl.ChangeScanDestynationFolderClicked += SetScanPath;
            this.FileControl.SavePhotoClicked += SavePhoto;
            this.FileControl.SavePhotoAsClicked += SavePhotoAs;
                     
            this.PhotoEditionControl.AutomaticRepairClicked += AutomaticRepair;
            this.PhotoEditionControl.DoExperimenClicked += Test;
            this.PhotoEditionControl.DustReductionClicked += DustReduction;
            this.PhotoEditionControl.ScratchesClicked += ScratchesReduction;
            this.PhotoEditionControl.SmudgeReductionClicked += SmudgeCleaner;
            this.PhotoEditionControl.SetRegionWithoutRepairClicked += SetRegionWithoutRepair;
            this.PhotoEditionControl.CutPhotoClicked += CutPhotoBorder;
            this.PhotoEditionControl.RotateImageClicked += RotateImage;
            this.PhotoEditionControl.SettingsClicked += OpenSettings;

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
        private void AutomaticRepair(object sender, EventArgs e)
        {
            InvokeAction(new Action(ImageProcessing.AutomaticRepair), true);
        }
        private void DustReduction(object sender, EventArgs e)
        {
            InvokeAction(new Action(ImageProcessing.ReduceDust), true);
        }
        private void CutPhotoBorder(object sender, EventArgs e)
        {
            ImageView.CutBorderInitialize();
            ImageView.RectangleLoaded += GetCutImageRectangle;
        }
        private void GetCutImageRectangle(object sender, Rectangle e)
        {           
            ImageProcessing.CutImage(ImageView.ZoomPhotoView.Size, e);
        }

        private  void SmudgeCleaner(object sender, EventArgs e)
        {
            InvokeAction(new Action(ImageProcessing.ReduceSmudges), true);
        }
        private  void ScratchesReduction(object sender, EventArgs e)
        {
            InvokeAction(new Action(ImageProcessing.ReduceScratches), true);
        }
        private  void RotateImage(object sender, EventArgs e)
        {
            InvokeAction(new Action(ImageProcessing.RotateImage), true, true);
        }
        private  void SetRegionWithoutRepair(object sender, EventArgs e)
        {
            ImageProcessing.SetRegionWithoutRepair();
        }
        private  void OpenSettings(object sender, EventArgs e)
        {

        }
        private  void Test(object sender, EventArgs e)
        {
            InvokeAction(new Action(ImageProcessing.Test), true, true);
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
        
        private async void InvokeAction(Action action, bool progressbar=false, bool progressbarIndeterminate=false)
        {
            ProgressBar progressBar = new ProgressBar(progressbarIndeterminate);
            try
            {
                EnableControl = false;
                if (progressbar)
                {
                    progressBar.Show();
                } 
                await Task.Run(() => { action.Invoke(); });
            }
            catch(ArgumentNullException anEx)
            {
                if(anEx.Message==nameof(ImageProcessing.ImageBefor))
                {
                    MessageBox.Show("Load image at first!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                }else
                {
                    throw new Exception(anEx.Message, anEx);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (progressbar)
                {
                    progressBar.Close();
                }                   
                EnableControl = true;
            }
        }
    }
}

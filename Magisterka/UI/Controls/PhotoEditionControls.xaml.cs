using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UI
{
    /// <summary>
    /// Interaction logic for PhotoEditionControls.xaml
    /// </summary>
    public partial class PhotoEditionControls : UserControl, INotifyPropertyChanged
    {
        public string AlignPhotoToggled
        {
            get
            {
                return _alignPhotoToggled.ToString();
            }
            set
            {
                if (value != null)
                {
                    _alignPhotoToggled = bool.Parse(value);
                    OnPropertyChanged(nameof(AlignPhotoToggled));
                    OnPropertyChanged(nameof(AlignPhotoVisibility));
                }
            }
        }
        public string AlignPhotoVisibility
        {
            get {
                if (_alignPhotoToggled == true)
                {
                    return "Visible";
                }
                else
                {
                    return "Collapsed";
                }
            }
        }
        public List<double> AlignTicks
        {
            get{ 
                if(_alignTicks ==null)
                {
                    _alignTicks  = new List<double>();

                    int value = -45;
                    do {
                        _alignTicks.Add(value++);
                    } while (value < 45);
                }
                return _alignTicks ;
            }
        }
        private bool _alignPhotoToggled=false;
        private List<double> _alignTicks ;


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region DustReduction
        public event EventHandler DustReductionClicked;

        public ICommand DustReductionClickedCommand
        {
            get { return new RelayCommand(DustReductionClickedCommandExecute); }
        }

        private void DustReductionClickedCommandExecute(object obj)
        {
            DustReductionClicked.Invoke(this, EventArgs.Empty);
        }
        #endregion DustReduction

        #region SmudgeReduction
        public event EventHandler SmudgeReductionClick;
        public ICommand SmudgeReductionClickCommand
        {
            get { return new RelayCommand(SmudgeReductionClickCommandExecute); }
        }
        private void SmudgeReductionClickCommandExecute(object obj)
        {
            SmudgeReductionClick.Invoke(this, EventArgs.Empty);
        }
        #endregion SmudgeReduction

        #region CutPhoto
        public event EventHandler CutPhotoClick;
        public ICommand CutPhotoClickCommand
        {
            get { return new RelayCommand(CutPhotoClickCommandExecute); }
        }
        private void CutPhotoClickCommandExecute(object obj)
        {
            CutPhotoClick.Invoke(this, EventArgs.Empty);
        }
        #endregion CutPhoto

        #region RotateImage
        public event EventHandler RotateImageClick;
        public ICommand RotateImageClickCommand
        {
            get { return new RelayCommand(RotateImageClickExecute); }
        }
        private void RotateImageClickExecute(object obj)
        {
            RotateImageClick.Invoke(this, EventArgs.Empty);
        }
        #endregion RotateImage

        #region AlignImageClick 
        public event EventHandler AlignImageValueChanged;
              
        private void AlignImageValueChangedExecute(object sender, EventArgs e)
        {
            AlignImageValueChanged.Invoke(this, e);
        }
        #endregion AlignImageClick

        public PhotoEditionControls()
        {
            InitializeComponent();
        }
    }
}

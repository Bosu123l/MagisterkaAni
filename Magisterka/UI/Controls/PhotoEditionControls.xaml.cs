using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace UI
{
    /// <summary>
    /// Interaction logic for PhotoEditionControls.xaml
    /// </summary>
    public partial class PhotoEditionControls : UserControl
    {
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

        #region RotateImageLeft
        public event EventHandler RotateImageLeftClick;
        public ICommand RotateImageLeftClickCommand
        {
            get { return new RelayCommand(RotateImageLeftClickExecute); }
        }
        private void RotateImageLeftClickExecute(object obj)
        {
            RotateImageLeftClick.Invoke(this, EventArgs.Empty);
        }
        #endregion RotateImageLeft

        #region RotateImageRight 
        public event EventHandler RotateImageRightClick;
        public ICommand RotateImageRightClickCommand
        {
            get { return new RelayCommand(RotateImageRightClickCommandExecute); }
        }
        private void RotateImageRightClickCommandExecute(object obj)
        {
            RotateImageRightClick.Invoke(this, EventArgs.Empty);
        }
        #endregion RotateImageRight

        public PhotoEditionControls()
        {
            InitializeComponent();
        }      
    }
}

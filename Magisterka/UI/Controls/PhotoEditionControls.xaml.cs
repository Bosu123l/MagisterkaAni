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
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region AutomaticRepair
        public event EventHandler AutomaticRepairClicked;
        public ICommand AutomaticRepairClickedCommand
        {
            get { return new RelayCommand(AutomaticRepairClickExecute); }
        }
        private void AutomaticRepairClickExecute(object obj)
        {
            AutomaticRepairClicked?.Invoke(this, EventArgs.Empty);
        }
        #endregion AutomaticRepair

        #region DoExperiment
        public event EventHandler DoExperimenClicked;

        public ICommand DoExperimenClickedCommand
        {
            get { return new RelayCommand(DoExperimenClickedCommandExecute); }
        }

        private void DoExperimenClickedCommandExecute(object obj)
        {
            DoExperimenClicked?.Invoke(this, EventArgs.Empty);
        }
        #endregion DoExperiment

        #region DustReduction
        public event EventHandler DustReductionClicked;

        public ICommand DustReductionClickedCommand
        {
            get { return new RelayCommand(DustReductionClickedCommandExecute); }
        }

        private void DustReductionClickedCommandExecute(object obj)
        {
            DustReductionClicked?.Invoke(this, EventArgs.Empty);
        }

        #endregion DustReduction

        #region Scratches
        public event EventHandler ScratchesClicked;
        public ICommand ScratchesClickedCommand
        {
            get { return new RelayCommand(ScratchesClickExecute); }
        }
        private void ScratchesClickExecute(object obj)
        {
            ScratchesClicked?.Invoke(this, EventArgs.Empty);
        }
        #endregion Scratches

        #region SmudgeReduction
        public event EventHandler SmudgeReductionClicked;
        public ICommand SmudgeReductionClickedCommand
        {
            get { return new RelayCommand(SmudgeReductionClickedCommandExecute); }
        }
        private void SmudgeReductionClickedCommandExecute(object obj)
        {
            SmudgeReductionClicked?.Invoke(this, EventArgs.Empty);
        }
        #endregion SmudgeReduction

        #region SetRegionWithoutRepair
        public event EventHandler SetRegionWithoutRepairClicked;
        public ICommand SetRegionWithoutRepairClickedCommand
        {
            get { return new RelayCommand(SetRegionWithoutRepairClickExecute); }
        }
        private void SetRegionWithoutRepairClickExecute(object obj)
        {
            SetRegionWithoutRepairClicked?.Invoke(this, EventArgs.Empty);
        }
        #endregion SetRegionWithoutRepair

        #region CutPhoto
        public event EventHandler CutPhotoClicked;
        public ICommand CutPhotoClickedCommand
        {
            get { return new RelayCommand(CutPhotoClickedCommandExecute); }
        }
        private void CutPhotoClickedCommandExecute(object obj)
        {
            CutPhotoClicked?.Invoke(this, EventArgs.Empty);
        }
        #endregion CutPhoto

        #region RotateImage
        public event EventHandler RotateImageClicked;
        public ICommand RotateImageClickedCommand
        {
            get { return new RelayCommand(RotateImageClickExecute); }
        }
        private void RotateImageClickExecute(object obj)
        {
            RotateImageClicked?.Invoke(this, EventArgs.Empty);
        }
        #endregion RotateImage

        #region Settings
        public event EventHandler SettingsClicked;
        public ICommand SettingsClickedCommand
        {
            get { return new RelayCommand(SettingsClickExecute); }
        }
        private void SettingsClickExecute(object obj)
        {
            SettingsClicked?.Invoke(this, EventArgs.Empty);
        }
        #endregion Settings 

        public event EventHandler ScratchesReductionInPaintNSMethodClick;
        public event EventHandler ScratchesReductionInPaintTeleaMethodClick;
        public event EventHandler ScratchesReductionSpiralSingleDefectsMethodClick;
        public event EventHandler DustReductionLeftToRightAveragingDefectsMethodClick;
        public event EventHandler DustReductionSpiralAveragingDefectsMethodClick;

        public PhotoEditionControls()
        {
            InitializeComponent();
        }       

        private void ScratchesReductionInPaintNSMethod(object sender, RoutedEventArgs e)
        {
            ScratchesReductionInPaintNSMethodClick?.Invoke(sender, e);
        } 
        private void ScratchesReductionInPaintTeleaMethod(object sender, RoutedEventArgs e)
        {
            ScratchesReductionInPaintTeleaMethodClick?.Invoke(sender, e);
        }
        private void ScratchesReductionSpiralSingleDefectsMethod(object sender, RoutedEventArgs e)
        {
            ScratchesReductionSpiralSingleDefectsMethodClick?.Invoke(sender, e);
        }        
        private void DustReductionLeftToRightAveragingDefectsMethod(object sender, RoutedEventArgs e)
        {
            DustReductionLeftToRightAveragingDefectsMethodClick?.Invoke(sender, e);
        }
        private void DustReductionSpiralAveragingDefectsMethod(object sender, RoutedEventArgs e)
        {
            DustReductionSpiralAveragingDefectsMethodClick?.Invoke(sender, e);
        }
    }
}

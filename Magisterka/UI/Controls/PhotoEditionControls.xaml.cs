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

        #region DustReductionAlternativeMethods
            #region DustReductionLeftToRightAveragingDefectsMethod
                public event EventHandler DustReductionLeftToRightAveragingDefectsMethodClicked;

                public ICommand DustReductionLeftToRightAveragingDefectsMethodClickedCommand
                {
                    get { return new RelayCommand(DustReductionLeftToRightAveragingDefectsMethodClickedCommandExecute); }
                }

                private void DustReductionLeftToRightAveragingDefectsMethodClickedCommandExecute(object obj)
                {
                    DustReductionLeftToRightAveragingDefectsMethodClicked?.Invoke(this, EventArgs.Empty);
                }
            #endregion DustReductionLeftToRightAveragingDefectsMethod
            #region DustReductionSpiralAveragingDefectsMethod
                public event EventHandler DustReductionSpiralAveragingDefectsMethodClicked;

                public ICommand DustReductionSpiralAveragingDefectsMethodClickedCommand
                {
                    get { return new RelayCommand(DustReductionSpiralAveragingDefectsMethodClickedCommandExecute); }
                }

                private void DustReductionSpiralAveragingDefectsMethodClickedCommandExecute(object obj)
                {
                    DustReductionSpiralAveragingDefectsMethodClicked?.Invoke(this, EventArgs.Empty);
                }
            #endregion DustReductionSpiralAveragingDefectsMethod
        #endregion DustReductionAlternativeMethods

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


        #region ScratchesAlternativeMethods
            #region ScratchesReductionInPaintNSMethodClickedCommand
            public event EventHandler ScratchesReductionInPaintNSMethodClicked;
            public ICommand ScratchesReductionInPaintNSMethodClickedCommand
            {
                get { return new RelayCommand(ScratchesReductionInPaintNSMethodClickExecute); }
            }
            private void ScratchesReductionInPaintNSMethodClickExecute(object obj)
            {
                ScratchesReductionInPaintNSMethodClicked?.Invoke(this, EventArgs.Empty);
            }
            #endregion ScratchesReductionInPaintNSMethodClickedCommand

            #region ScratchesReductionInPaintTeleaMethodClickedCommand
            public event EventHandler ScratchesReductionInPaintTeleaMethodClicked;
            public ICommand ScratchesReductionInPaintTeleaMethodClickedCommand
            {
                get { return new RelayCommand(ScratchesReductionInPaintTeleaMethodClickExecute); }
            }
            private void ScratchesReductionInPaintTeleaMethodClickExecute(object obj)
            {
                ScratchesReductionInPaintTeleaMethodClicked?.Invoke(this, EventArgs.Empty);
            }
            #endregion ScratchesReductionInPaintTeleaMethodClickedCommand

            #region ScratchesReductionSpiralSingleDefectsMethodClickedCommand
            public event EventHandler cratchesReductionSpiralSingleDefectsMethodClicked;
            public ICommand cratchesReductionSpiralSingleDefectsMethodClickedCommand
            {
                get { return new RelayCommand(cratchesReductionSpiralSingleDefectsMethodClickExecute); }
            }
            private void cratchesReductionSpiralSingleDefectsMethodClickExecute(object obj)
            {
                cratchesReductionSpiralSingleDefectsMethodClicked?.Invoke(this, EventArgs.Empty);
            }
            #endregion ScratchesReductionSpiralSingleDefectsMethodClickedCommand

            #region ScratchesReductionSpiralWholePhotoMethodClickedCommand
            public event EventHandler ScratchesReductionSpiralWholePhotoMethodClicked;
            public ICommand ScratchesReductionSpiralWholePhotoMethodClickedCommand
            {
                get { return new RelayCommand(ScratchesReductionSpiralWholePhotoMethodClickExecute); }
            }
            private void ScratchesReductionSpiralWholePhotoMethodClickExecute(object obj)
            {
                ScratchesReductionSpiralWholePhotoMethodClicked?.Invoke(this, EventArgs.Empty);
            }
            #endregion ScratchesReductionSpiralWholePhotoMethodClickedCommand
        #endregion ScratchesAlternativeMethods


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

        public PhotoEditionControls()
        {
            InitializeComponent();           
        }
    }
}

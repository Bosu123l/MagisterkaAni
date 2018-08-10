using Domain;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows;

namespace UI
{
    /// <summary>
    /// Interaction logic for ProgressBar.xaml
    /// </summary>
    public partial class ProgressBar : Window, INotifyPropertyChanged
    {
        public int OperationProgress
        {
            get
            {
                return _operationProgress;
            }
            set
            {
                _operationProgress = value;
                OnPropertyChanged(nameof(OperationProgress));
                OnPropertyChanged(nameof(OperationProgressPercent));               
            }
        }
        public string IsIndeterminate
        {
            get { return _isIndeterminate.ToString(); }
            set
            {
                OnPropertyChanged(nameof(IsIndeterminate));
            }
        }
        public string OperationProgressPercent
        {
            get
            {
                if(_isIndeterminate)
                {
                    return string.Empty;
                }else
                {
                    return $"{((int)((OperationProgress * 100) / MaxValueProgressBar)).ToString()}%";
                }
               
            }
        }
        public int MaxValueProgressBar
        {
            get
            {
                return ProgressManager.MaxValueProgressBar;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private int _operationProgress = 0;
        private bool _isIndeterminate;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public ProgressBar(bool isIndeterminate = false)
        {
            _isIndeterminate = isIndeterminate;
            InitializeComponent();            
            if (isIndeterminate == false)
            {
                ProgressManager.ProgressStatusChanged += new ProgressManager.RefreshProgress(Refresh);
            }
        }
        public void Refresh(int progressPercent)
        {
            OperationProgress = progressPercent;
        }

        private void BeforeClose(object sender, CancelEventArgs e)
        {
            ProgressManager.FinallySteps();
        }
    }
}

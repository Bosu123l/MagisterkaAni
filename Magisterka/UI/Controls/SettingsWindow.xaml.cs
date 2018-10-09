using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Domain;
using Domain.Settings;

namespace UI.Controls
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window, INotifyPropertyChanged
    {
        public int ScratchesKernelSize
        {
            get { return Settings.ScratchesKernelSize; }
            set
            {
                Settings.ScratchesKernelSize = value;
                OnPropertyChanged(nameof(ScratchesKernelSize));
            }
        }
        public int DustKernelSize
        {
            get { return Settings.DustKernelSize; }
            set
            {
                Settings.DustKernelSize = value;
                OnPropertyChanged(nameof(DustKernelSize));
            }
        }
        public float SmudgesMargin
        {
            get { return Settings.SmudgesMargin; }
            set
            {
                Settings.SmudgesMargin = value;
                OnPropertyChanged(nameof(SmudgesMargin));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }    

        public SettingsWindow()
        {
            InitializeComponent();            
        }

        private void SaveSettingsClick(object sender, RoutedEventArgs e)
        {
            Settings.SaveSettings();
            this.Close();
        }
    }
}

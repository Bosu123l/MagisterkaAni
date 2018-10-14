using Domain;
using System.ComponentModel;
using System.Windows;

namespace UI.Controls
{   
    public partial class AutoRepairWindow : Window, INotifyPropertyChanged
    {
        public bool CleanSmudges
        {
            get { return _cleanSmudges; }
            set {
                if (_cleanSmudges != value)
                {
                    _cleanSmudges = value;
                    OnPropertyChanged(nameof(CleanSmudges));
                }
            }
        } 
        public bool CleanScrates
        {
            get { return _cleanScrates; }
            set
            {
                if (_cleanScrates != value)
                {
                    _cleanScrates = value;
                    OnPropertyChanged(nameof(CleanScrates));
                }
            }
        }
        public bool CleanDust
        {
            get { return _cleanDust; }
            set
            {
                if (_cleanDust != value)
                {
                    _cleanDust = value;
                    OnPropertyChanged(nameof(CleanDust));
                }
            }
        }

        public bool _cleanSmudges; 
        public bool _cleanScrates;
        public bool _cleanDust;

        public event PropertyChangedEventHandler PropertyChanged;

        public AutoRepairWindow()
        {
            InitializeComponent();
            CleanSmudges = true;
            CleanScrates = true;
            CleanDust = true;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void btStart_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}

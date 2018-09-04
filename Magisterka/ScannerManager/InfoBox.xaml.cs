using System.Windows;
namespace ScannerManager
{
    /// <summary>
    /// Interaction logic for InfoBox.xaml
    /// </summary>
    public partial class InfoBox : Window
    {

        public bool YesButtonVisible { get; set; }
        public bool NoButtonVisible { get; set; }
        public bool OKButtonVisible { get; set; }
        public bool CancelButtonVisible { get; set; }

        public string WindowHeader { get; set; }
        public string Information { get; set; }

        public InfoBox(MessageBoxButton buttonType, string header, string information)
        {
            InitializeComponent();

            WindowHeader = header;
            Information = information;

            switch (buttonType)
            {
                case MessageBoxButton.OK: {
                        YesButtonVisible = false;
                        NoButtonVisible = false;
                        OKButtonVisible = true;
                        CancelButtonVisible = false;
                    } break;
                case MessageBoxButton.OKCancel: {
                        YesButtonVisible = false;
                        NoButtonVisible = false;
                        OKButtonVisible = true;
                        CancelButtonVisible = true;
                    } break;
                case MessageBoxButton.YesNo: {
                        YesButtonVisible = true;
                        NoButtonVisible = true;
                        OKButtonVisible = false;
                        CancelButtonVisible = false;
                    } break;
                case MessageBoxButton.YesNoCancel: {
                        YesButtonVisible = true;
                        NoButtonVisible = true;
                        OKButtonVisible = false;
                        CancelButtonVisible = true;
                    } break;
            }

            InitializeComponent();
        }

        private void PositivResult_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void NegativResult_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}

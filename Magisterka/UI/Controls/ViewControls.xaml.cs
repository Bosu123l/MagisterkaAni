using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UI
{
    /// <summary>
    /// Interaction logic for ViewControls.xaml
    /// </summary>
    public partial class ViewControls : UserControl
    {
        #region OpenPhotoInNewWindow
        public event EventHandler OpenPhotoInNewWindowClicked;
        public ICommand OpenPhotoInNewWindowClickedCommand
        {
            get { return new RelayCommand(OpenPhotoInNewWindowClickedCommandExecute); }
        }
        private void OpenPhotoInNewWindowClickedCommandExecute(object obj)
        {
            OpenPhotoInNewWindowClicked.Invoke(this, EventArgs.Empty);
        }
        #endregion OpenPhotoInNewWindow

        #region OpenOldPhotoInNewWindow
        public event EventHandler OpenOldPhotoInNewWindowClicked;
        public ICommand OpenOldPhotoInNewWindowClickedCommand
        {
            get { return new RelayCommand(OpenOldPhotoInNewWindowClickedCommandExecute); }
        }
        private void OpenOldPhotoInNewWindowClickedCommandExecute(object obj)
        {
            OpenOldPhotoInNewWindowClicked.Invoke(this, EventArgs.Empty);
        }
        #endregion OpenOldPhotoInNewWindow

        public ViewControls()
        {
            InitializeComponent();
        }
    }
}

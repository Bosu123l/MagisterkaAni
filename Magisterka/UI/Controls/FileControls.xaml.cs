using System;
using System.Windows.Controls;
using System.Windows.Input;
namespace UI
{
    public partial class FileControls : UserControl
    {
        #region GetPhotoFromScanner
        public event EventHandler GetPhotoFromScannerClicked;

        public ICommand GetPhotoFromScannerClickedCommand
        {
            get
            {
                return new RelayCommand(GetPhotoFromScannerClickedCommandExecute);
            }
        }

        private void GetPhotoFromScannerClickedCommandExecute(object obj)
        {
            GetPhotoFromScannerClicked?.Invoke(this, EventArgs.Empty);
        }
        #endregion GetPhotoFromScanner

        #region GetPhotoFromFolder 
        public event EventHandler GetPhotoFromFolderClicked;

        public ICommand GetPhotoFromFolderClickedCommand
        {
            get
            {
                return new RelayCommand(GetPhotoFromFolderClickedCommandExecute);
            }
        }

        private void GetPhotoFromFolderClickedCommandExecute(object obj)
        {
            GetPhotoFromFolderClicked.Invoke(this, EventArgs.Empty);
        }
        #endregion GetPhotoFromFolder

        #region ChangeScanDestynationFolder
        public event EventHandler ChangeScanDestynationFolderClicked;

        public ICommand ChangeScanDestynationFolderClickedCommand
        {
            get
            {
                return new RelayCommand(ChangeScanDestynationFolderClickedCommandExecute);
            }
        }

        private void ChangeScanDestynationFolderClickedCommandExecute(object obj)
        {
            ChangeScanDestynationFolderClicked.Invoke(this, EventArgs.Empty);
        }
        #endregion ChangeScanDestynationFolder

        #region SavePhoto
        public event EventHandler SavePhotoClicked;

        public ICommand SavePhotoClickedCommand
        {
            get
            {
                return new RelayCommand(SavePhotoClickedCommandExecute);
            }
        }

        private void SavePhotoClickedCommandExecute(object obj)
        {
            SavePhotoClicked.Invoke(this, EventArgs.Empty);
        }
        #endregion SavePhoto

        #region SavePhotoAs...
        public event EventHandler SavePhotoAsClicked;

        public ICommand SavePhotoAsClickedCommand
        {
            get
            {
                return new RelayCommand(SavePhotoAsClickedCommandExecute);
            }
        }

        private void SavePhotoAsClickedCommandExecute(object obj)
        {
            SavePhotoAsClicked.Invoke(this, EventArgs.Empty);
        }
        #endregion SavePhotoAs...

        #region Constructor
        public FileControls()
        {
            InitializeComponent();
        }
        #endregion Constructor

    }
}

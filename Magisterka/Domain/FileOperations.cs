using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Domain
{
    public static class FileOperations
    {
        #region ConstPatameters
        private const string _readFileExtensions = "TIFF |*.tif;*.tiff";
        private const string _writeOtherFileExtensions = "TIFF |*.tif;*.tiff|" +
                                                          "PNG  |*.png| " +
                                                          "JPEG |*.jpg;*jpeg;";
        private static string _defaultDestinationDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "OldPhotos");
        private static string _defaultSourcDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "OldPhotos");
        private static string _defaultScanDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "OldPhotos");
        #endregion ConstPatameters

        #region privateParameters
        private static string _destinationDirectory;
        private static string _sourceDirectory;
        private static string _scanDirectory;
       
        private static string _filePath;
        private static string _fileName
        {
            get
            {
                return string.IsNullOrEmpty(FilePath) ? string.Empty : Path.GetFileName(FilePath);
            }
        }
        private static string _fileDirectory
        {
            get
            {
                return string.IsNullOrEmpty(FilePath) ? string.Empty : Path.GetDirectoryName(FilePath);
            }
        }
        private static string _fileExtension
        {
            get
            {
                return string.IsNullOrEmpty(FilePath) ? "tiff" : Path.GetExtension(_filePath).Replace(".", string.Empty);
            }
        }
        #endregion privateParameters

        #region publicParameters       
        public static string DestinationPath
        {
            get
            {

                if (!Directory.Exists(_destinationDirectory) || string.IsNullOrEmpty(_destinationDirectory))
                {
                    return _defaultDestinationDirectory;
                }
                else
                {
                    return _destinationDirectory;
                }
            }
            set
            {

                if (value != _destinationDirectory && string.IsNullOrEmpty(value) == false)
                {
                    _destinationDirectory = value;
                }
            }
        }
        public static string ScanDirectory
        {
            get
            {

                if (!Directory.Exists(_scanDirectory) || string.IsNullOrEmpty(_scanDirectory))
                {
                    return _defaultScanDirectory;
                }
                else
                {
                    return _scanDirectory;
                }
            }
            set
            {

                if (value != _scanDirectory && string.IsNullOrEmpty(value) == false)
                {
                    _scanDirectory = value;
                }
            }
        }
        public static string SourceDirectory
        {
            get
            {

                if (!Directory.Exists(_sourceDirectory) || string.IsNullOrEmpty(_sourceDirectory))
                {
                    return _defaultSourcDirectory;
                }
                else
                {
                    return _sourceDirectory;
                }
            }
            set
            {

                if (value != _sourceDirectory && string.IsNullOrEmpty(value) == false)
                {
                    _sourceDirectory = value;
                }
            }
        }
        public static string FilePath
        {
            get
            {
                return _filePath;
            }
            set
            {
                if (value != null)
                {
                    _filePath = value;
                    SourceDirectory = _fileDirectory;
                }
            }
        }

        #endregion publicParameters

        public static void SaveImageFileAs(ImageWrapper<Bgr, byte> image)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.AddExtension = true;
            saveFileDialog.Filter = _writeOtherFileExtensions;
            saveFileDialog.FileName = Path.GetFileNameWithoutExtension(_fileName);

            if (Directory.Exists(DestinationPath))
            {
                saveFileDialog.InitialDirectory = DestinationPath;
            }
            else
            {
                saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            }

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                DestinationPath = Path.GetDirectoryName(saveFileDialog.FileName);
                image.Save(saveFileDialog.FileName);
                _defaultDestinationDirectory = Path.GetDirectoryName(saveFileDialog.FileName);
            }
        }

        public static void SaveImageFile(ImageWrapper<Bgr, byte> image)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.AddExtension = true;
            saveFileDialog.Filter = $"{_fileExtension.ToUpper()} |*.{_fileExtension}";
            saveFileDialog.FileName = _fileName;

            if (Directory.Exists(DestinationPath))
            {
                saveFileDialog.InitialDirectory = DestinationPath;
            }
            else
            {
                saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            }

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                var path = Path.Combine(_fileDirectory, saveFileDialog.FileName);
                DestinationPath = _fileDirectory;
                image.Save(path);
                _defaultDestinationDirectory = Path.GetDirectoryName(saveFileDialog.FileName);
            }
        }

        public static Image<Bgr, byte> GetImageFromDirectory()
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog()
                {
                    Filter = _readFileExtensions
                };

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    FilePath = openFileDialog.FileName;                    
                    return new Image<Bgr, byte>(FilePath);
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static ImageWrapper<Bgr, byte> GetImageFromScanner()
        {
            Process scannerManager = new Process();
            string scanFilePattrn = @"^\d{8}_SC\d{4}";
            string[] files;
            string lastAddedFilePath;
            ExitCodes.ExitCode exCode = ExitCodes.ExitCode.ERROR_FILE_NOT_FOUND;

            try
            {
                scannerManager.StartInfo.Arguments = ScanDirectory;

                scannerManager.StartInfo.FileName = Path.Combine(Environment.CurrentDirectory, @"..\..\..\ScannerManager\bin\Debug\ScannerManager.exe"); 
                scannerManager.EnableRaisingEvents = true;

                scannerManager.Start();

                scannerManager.WaitForExit();
                exCode = (ExitCodes.ExitCode)scannerManager.ExitCode;

                switch (exCode)
                {
                    case ExitCodes.ExitCode.SUCCESS:
                        {
                            files = Directory.GetFiles(ScanDirectory);
                            files = files.Where(x => Regex.IsMatch(Path.GetFileName(x), scanFilePattrn)).ToArray();

                            lastAddedFilePath = files.OrderByDescending(x => Path.GetFileName(x)).First();

                            if (string.IsNullOrEmpty(lastAddedFilePath))
                            {
                                throw new Exception("Scanned file not found!");
                            }
                            FilePath = lastAddedFilePath;
                            return new ImageWrapper<Bgr, byte>(FilePath);
                        }
                    case ExitCodes.ExitCode.ERROR_CANCELLED:
                        { return null; }
                    default:
                        { throw new Exception(ExitCodes.ExitMessage(exCode)); }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void SetScanPath()
        {
            var dialog = new FolderBrowserDialog();

            if (Directory.Exists(ScanDirectory))
            {
                dialog.SelectedPath = ScanDirectory;
            }

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ScanDirectory = dialog.SelectedPath;
            }
        }
    }
}

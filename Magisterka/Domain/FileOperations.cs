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
    public class FileOperations
    {
        #region ConstPatameters
        private static string _readFileExtensions  = "TIFF |*.tif;*.tiff";
        private static string _writeOtherFileExtensions = "TIFF |*.tif;*.tiff|" +
                                                          "PNG  |*.png| " +
                                                          "JPEG |*.jpg;*jpeg;";
        private static string _writeTiffFileExtensions =  "TIFF |*.tiff;";
        private static string _defaultDestinationDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "OldPhotos");
        private static string _defaultSourcDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "OldPhotos");
        private static string _defaultScanDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "OldPhotos");
        #endregion ConstPatameters

        #region privateParameters
        private static string _destinationDirectory;
        private static string _scanPath;
        private static string _filePath;
        #endregion privateParameters

        #region publicParameters       
        public static string DestinationPath
        {
            get {

                if (!Directory.Exists(_destinationDirectory) || string.IsNullOrEmpty(_destinationDirectory))
                {
                    return _defaultDestinationDirectory;
                }
                else
                {
                    return _destinationDirectory;
                }
            }
            set {

                if(value!=_destinationDirectory && string.IsNullOrEmpty(value)==false)
                {
                    _destinationDirectory = value;
                }
            }
        }
        public static string ScanPath
        {
            get {

                if (!Directory.Exists(_scanPath) || string.IsNullOrEmpty(_scanPath))
                {
                    return _defaultScanDirectory;
                }
                else
                {
                    return _defaultScanDirectory;
                }
            }
            set {

                if(value!=_destinationDirectory && string.IsNullOrEmpty(value)==false)
                {
                    _scanPath = value;
                }
            }
        }
        public static string FilePath
        {
            get{
                return _filePath;
            } 
            set
            {
                if(value!=null)
                {
                    _filePath = value;
                }
            }
        }
        public static string FileName
        {
            get {
                if (string.IsNullOrEmpty(FilePath))
                {
                    return Path.GetFileName(FilePath);
                }else
                {
                    return string.Empty;
                }
            }
        }
        #endregion publicParameters

        public static void SaveImageFileAs(Image<Bgr, byte> image)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.AddExtension = true;
            saveFileDialog.Filter = _writeOtherFileExtensions;
            saveFileDialog.FileName = Path.GetFileNameWithoutExtension(FileName);

            if (Directory.Exists(_defaultDestinationDirectory))
            {
                saveFileDialog.InitialDirectory = _defaultDestinationDirectory;
            }
            else
            {
                saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            }

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                image.Save(saveFileDialog.FileName);
                _defaultDestinationDirectory = Path.GetDirectoryName(saveFileDialog.FileName);
            }
        }

        public static void SaveAsTiffImageFile(Image<Bgr, byte> image)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.AddExtension = false;
            if (Directory.Exists(_defaultDestinationDirectory))
            {
                saveFileDialog.InitialDirectory = _defaultDestinationDirectory;
            }
            else
            {
                saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            }

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                var path = Path.Combine(Path.GetDirectoryName(_filePath),FileName);
                image.Save(path);
                _defaultDestinationDirectory = Path.GetDirectoryName(saveFileDialog.FileName);
            }
        }      

        public static Image<Bgr, byte> GetImageFromDirectory()
        {
            string filename;
            Image<Bgr, byte> image;

            try
            {
                OpenFileDialog dlg = new OpenFileDialog()
                {
                    Filter = _readFileExtensions
                };

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    filename = dlg.FileName;
                    image = new Image<Bgr, byte>(filename);
                    return image;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }            
        }

        public static Image<Bgr, byte> GetImageFromScanner()
        {
            Process sm = new Process();
            string scanFilePattrn = @"^\d{8}_SC\d{4}";
            Image<Bgr, byte> image;
            string[] files;
            string lastAddedFilePath;
            ExitCodes.ExitCode exCode=ExitCodes.ExitCode.ERROR_FILE_NOT_FOUND;

            try
            {
                sm.StartInfo.Arguments = ScanPath;

                sm.StartInfo.FileName = @"C:\Users\annaj\Documents\GitHub\MagisterkaAni\Magisterka\ScannerManager\bin\Debug\ScannerManager.exe";
                sm.EnableRaisingEvents = true;

                sm.Start();

                sm.WaitForExit();
                exCode=(ExitCodes.ExitCode)sm.ExitCode;

                switch (exCode)
                {
                    case ExitCodes.ExitCode.SUCCESS:
                        {
                            files = Directory.GetFiles(ScanPath);
                            files = files.Where(x => Regex.IsMatch(Path.GetFileName(x), scanFilePattrn)).ToArray();

                            lastAddedFilePath = files.OrderByDescending(x => Path.GetFileName(x)).First();

                            if (string.IsNullOrEmpty(lastAddedFilePath))
                            {
                                throw new Exception("Scanned file not found!");
                            }

                            image = new Image<Bgr, byte>(lastAddedFilePath);
                            return image;
                        } break;
                    case ExitCodes.ExitCode.ERROR_CANCELLED:
                        { return null; } break;
                    default:
                        { throw new Exception(exCode.ToString()); }break;
                } 
            }
            catch (Exception ex)
            {
                throw ex;
            }         
        }

        public static string SetScanPath()
        {
            var dialog = new FolderBrowserDialog();

            if (Directory.Exists(ScanPath))
            {
                dialog.SelectedPath = ScanPath;
            }
            
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ScanPath = dialog.SelectedPath;
            }

            return ScanPath;
        }
    }
}

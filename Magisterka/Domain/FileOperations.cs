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
        private static string _readFileExtensions  = "TIFF |*.tif;*.tiff";
        private static string _writeFileExtensions = "TIFF |*.tif;*.tiff|" +
                                                     "PNG  |*.png| " +
                                                     "JPEG |*.jpg;*jpeg;";
        private static string _defaultDestinationDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "OldPhotos");
        private static string _defaultSourcDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "OldPhotos");


        
        public static void SaveImageFileAs(Image<Bgr, byte> image)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.AddExtension = true;
            saveFileDialog.Filter = _writeFileExtensions;
            if(Directory.Exists(_defaultDestinationDirectory))
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

        public static Image<Bgr, byte> GetImageFromScanner(string scanPath)
        {
            Process sm = new Process();
            string scanFilePattrn = @"^\d{8}_SC\d{4}";
            Image<Bgr, byte> image;
            string[] files;
            string lastAddedFilePath;
            ExitCodes.ExitCode exCode=ExitCodes.ExitCode.ERROR_FILE_NOT_FOUND;

            try
            {
                sm.StartInfo.Arguments = scanPath;

                sm.StartInfo.FileName = @"C:\Users\annaj\Documents\GitHub\MagisterkaAni\Magisterka\ScannerManager\bin\Debug\ScannerManager.exe";
                sm.EnableRaisingEvents = true;

                sm.Start();

                sm.WaitForExit();
                exCode=(ExitCodes.ExitCode)sm.ExitCode;

                switch (exCode)
                {
                    case ExitCodes.ExitCode.SUCCESS:
                        {
                            files = Directory.GetFiles(scanPath);
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
    }
}

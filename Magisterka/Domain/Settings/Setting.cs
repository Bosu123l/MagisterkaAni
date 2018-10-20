using System.IO;
using System.Xml.Serialization;

namespace Domain.Settings
{
    public class Settings
    {
        private static string _settingsFilePath = "../PhotoCleaner.config";
        
        public static int ScratchesKernelSize
        {
            get { return _largeDefectsKernel; }
            set
            {
                if(value>2 && value<100)
                {
                    _largeDefectsKernel = value;
                }
            }
        }
        public static int DustKernelSize
        {
            get { return _smallDefectsKernel; }
            set
            {
                if (value > 2 && value < 100)
                {
                    _smallDefectsKernel = value;
                }
            }
        }
        public static float SmudgesMargin
        {
            get { return _smudgesMargin; }
            set
            {
                if (value >=0.0f && value < 1.0f)
                {
                    _smudgesMargin = value;
                }
            }
        }

        public static int _largeDefectsKernel = 5;
        public static int _smallDefectsKernel = 5;
        public static float _smudgesMargin = 0.0f;


        public static void LoadSettings()
        {
            if (File.Exists(_settingsFilePath))
            {
                using (FileStream stream = new FileStream(_settingsFilePath, FileMode.Open))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(XMLSetting));
                    if (stream.Length > 0)
                    {
                        XMLSetting xml = (XMLSetting)serializer.Deserialize(stream);

                        ScratchesKernelSize = xml.ScratchesKernelSize;
                        DustKernelSize = xml.DustKernelSize;
                        SmudgesMargin = xml.SmudgesMargin;
                    }

                    stream.Close();
                }
            }          
        }

        public static void SaveSettings()
        {
            if (!File.Exists(_settingsFilePath))
            {
                File.Create(_settingsFilePath).Close();
            }

            using (var stream = new FileStream(_settingsFilePath, FileMode.OpenOrCreate))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(XMLSetting));
                XMLSetting xml = new XMLSetting()
                {
                    ScratchesKernelSize = ScratchesKernelSize,
                    DustKernelSize = DustKernelSize,
                    SmudgesMargin = SmudgesMargin
                };     

                serializer.Serialize(stream, xml);

                stream.Close();                
            }
        }
    }
}

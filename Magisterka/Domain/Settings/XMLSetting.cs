using System.Xml.Serialization;

namespace Domain.Settings
{
    [XmlType(nameof(Settings))]
    public class XMLSetting
    {
        [XmlAttribute(nameof(Settings.ScratchesKernelSize))]
        public int ScratchesKernelSize
        {
            get;
            set;
        }
        [XmlAttribute(nameof(Settings.DustKernelSize))]
        public int DustKernelSize
        {
            get;
            set;
        }
        [XmlAttribute(nameof(Settings.SmudgesMargin))]
        public float SmudgesMargin
        {
            get;
            set;
        }
    }
}

using System.Xml.Serialization;

namespace Domain.Settings
{
    [XmlType(nameof(Setting))]
    public class XMLSetting
    {
        [XmlAttribute(nameof(Setting.LargeDefectsKernel))]
        public int LargeDefectsKernel
        {
            get;
            set;
        }
        [XmlAttribute(nameof(Setting.SmallDefectsKernel))]
        public int SmallDefectsKernel
        {
            get;
            set;
        }
        [XmlAttribute(nameof(Setting.SmudgesMargin))]
        public float SmudgesMargin
        {
            get;
            set;
        }
    }
}

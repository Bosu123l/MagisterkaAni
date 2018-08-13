using System.Drawing;
using System.Windows;
namespace UI
{
    /// <summary>
    /// Interaction logic for PreviewWindow.xaml
    /// </summary>
    public partial class PreviewWindow : Window
    {
        public Image ViewedImage
        {
            get
            {
                return PhotoZoomBox.Image;
            }
            set
            {
                PhotoZoomBox.Image = value;
            }
        }

        public PreviewWindow(Image photo)
        {           
            InitializeComponent();
            if (photo != null)
            {
                ViewedImage = photo;
            }
        }
    }
}

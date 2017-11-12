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
using System.Windows.Shapes;

namespace UI
{
    /// <summary>
    /// Interaction logic for PreviewWindow.xaml
    /// </summary>
    public partial class PreviewWindow : Window
    {
        public BitmapImage PreviweImage
        {
            get;
            set;
        }
        public PreviewWindow(BitmapImage pi)
        {
            if (pi != null)
            {
                PreviweImage = pi;
            }
            InitializeComponent();
        }
    }
}

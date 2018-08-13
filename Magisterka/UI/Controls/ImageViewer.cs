using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Domain;
using Emgu.CV.Structure;

namespace UI.Controls
{
    public partial class ImageViewer : UserControl
    {



        public ImageViewer()
        {
            InitializeComponent();
        }

        public void SetImage(ImageWrapper<Bgr,byte>imageWrapper)
        {

        }

        public void SetImage(ImageWrapper<Gray, byte> imageWrapper)
        {

        }
    }
}

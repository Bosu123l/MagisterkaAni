using Domain;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;
using System.Windows.Forms;

namespace UI
{
    public partial class HistogramForm : Form
    {
        public HistogramForm(ImageWrapper<Bgr, byte>image)
        {
             InitializeComponent();

            ibImageBox.Image = image.Image;
            zoomBox.Image = image.Image.Bitmap;

            DenseHistogram histogram = new DenseHistogram(256, new RangeF(0.0f, 255.0f));
            
            histogram.Calculate(new Image<Gray, byte>[] { image.Convert<Gray, byte>().Image }, false, null);

            Mat mat = new Mat();
            bhHistogram.AddHistogram("Laplace histogram", Color.Black, mat, 256, new float[] {0f, 256f});
            bhHistogram.Refresh();
        }
    }
}

using Domain;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using System.Drawing;
using System.Windows.Controls;

namespace UI
{
    /// <summary>
    /// Interaction logic for ImgeViewer.xaml
    /// </summary>
    public partial class ImgeViewer : UserControl
    {
        public ImageWrapper<Bgr, byte> ViewedImage
        {
            set
            {
                LoadImage(value);
            }
        }
        private ImageWrapper<Bgr, byte> _image;

        public ImgeViewer()
        {
            InitializeComponent();
        }

        public void LoadImage(ImageWrapper<Bgr, byte> image)
        {
            PhotoView.Image = image.Bitmap;
            _image = image.Copy();
            
            AddHistogram(image, Color.Black,BlackHistogram);
            AddHistogram(image, Color.Blue, BlueHistogram);
            AddHistogram(image, Color.Green, GreenHistogram);
            AddHistogram(image, Color.Red, RedHistogram);          
        }
        private void AddHistogram(ImageWrapper<Bgr, byte> image, Color color, HistogramBox histogramBox)
        {            
            DenseHistogram histogram;
            histogram = new DenseHistogram(256, new RangeF(0f, 255f));        
            Mat mat = new Mat();

            if (color == Color.Black)       { histogram.Calculate(new Image<Gray, byte>[] { image.Image.Convert<Gray, byte>()}, false, null); }
            else if (color == Color.Blue)   { histogram.Calculate(new Image<Gray, byte>[] { image.Image[0] }, false, null); }
            else if (color == Color.Red)    { histogram.Calculate(new Image<Gray, byte>[] { image.Image[1] }, false, null); }
            else if (color == Color.Green)  { histogram.Calculate(new Image<Gray, byte>[] { image.Image[2] }, false, null); }
            else { return; }              
                     
            histogram.CopyTo(mat);

            histogramBox.ClearHistogram();
            histogramBox.AddHistogram(color.ToString(), color, mat, 256, new float[] { 0f, 256f });
            histogramBox.Refresh();
        }

        private void ResetView()
        {
            BlueHistogram.ClearHistogram();
        }
    }
}

using Domain;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using System;
using System.Drawing;
using System.Windows.Controls;

namespace UI
{    
    public partial class ImgeViewer : UserControl
    {
        public Image<Bgr, byte> ViewedImage
        {
            set
            {
                LoadImage(value);
            }
        }   

        private Image<Bgr, byte> _image;

        public ImgeViewer()
        {
            InitializeComponent();         
            PhotoView.OnZoomScaleChange += PhotoView_OnZoomScaleChange;
        }

        private void PhotoView_OnZoomScaleChange(object sender, EventArgs e)
        {
            if(sender is PanAndZoomPictureBox)
            {
                PanAndZoomPictureBox pictureBox = (PanAndZoomPictureBox)sender;               

                if (pictureBox.ZoomScale < 1)
                {                    
                    pictureBox.SetZoomScale(1,new Point(1,1));
                    return;
                }
            }
        }

        public void LoadImage(Image<Bgr, byte> image)
        {
            PhotoView.Image = image.Bitmap;           

            _image = image.Copy();

            AddHistogram(image, Color.Black, BlackHistogram);
            AddHistogram(image, Color.Blue, BlueHistogram);
            AddHistogram(image, Color.Green, GreenHistogram);
            AddHistogram(image, Color.Red, RedHistogram);
            AddSummaryHistogram();
        }
        private void AddHistogram(Image<Bgr, byte> image, Color color, HistogramBox histogramBox)
        {
            using (DenseHistogram histogram = new DenseHistogram(256, new RangeF(0f, 255f)))
            {
                Mat mat = new Mat();

                if (color == Color.Black) { histogram.Calculate(new Image<Gray, byte>[] { image.Convert<Gray, byte>() }, false, null); }
                else if (color == Color.Blue) { histogram.Calculate(new Image<Gray, byte>[] { image[0] }, false, null); }
                else if (color == Color.Red) { histogram.Calculate(new Image<Gray, byte>[] { image[1] }, false, null); }
                else if (color == Color.Green) { histogram.Calculate(new Image<Gray, byte>[] { image[2] }, false, null); }
                else { return; }

                histogram.CopyTo(mat);

                histogramBox.ClearHistogram();
                histogramBox.AddHistogram(color.ToString(), color, mat, 256, new float[] { 0f, 255f });
                histogramBox.Refresh();
            }               
        }

        private void AddSummaryHistogram()
        {
           using (DenseHistogram histogram = new DenseHistogram(256, new RangeF(0f, 255f)))
           {
                Mat mat = new Mat();
                               
                SummaryHistogram.ClearHistogram();
                SummaryHistogram.GenerateHistograms(_image, 256);

                histogram.Calculate(new Image<Gray, byte>[] {_image.Convert<Gray,byte>()}, false, null);
                histogram.CopyTo(mat);

                SummaryHistogram.AddHistogram("", Color.Black, mat, 256, new float[] { 0f, 255f });
                SummaryHistogram.Refresh();
            }
        }

        private void ResetView()
        {
            BlueHistogram.ClearHistogram();
        }

    }
}

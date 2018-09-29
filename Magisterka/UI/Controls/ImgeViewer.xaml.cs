using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows;
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
            ZoomPhotoView.OnZoomScaleChange += ZoomPhotoView_OnZoomScaleChange;
        }

        private void ZoomPhotoView_OnZoomScaleChange(object sender, EventArgs e)
        {
            if(sender is PanAndZoomPictureBox)
            {
                PanAndZoomPictureBox pictureBox = (PanAndZoomPictureBox)sender;               

                if (pictureBox.ZoomScale <= 1 || mouseDown)
                {                    
                    pictureBox.SetZoomScale(1,new System.Drawing.Point(1,1));
                    pictureBox.HorizontalScrollBar.Visible = false;
                    pictureBox.VerticalScrollBar.Visible = false;
                    return;
                }
            }
        }

        public void LoadImage(Image<Bgr, byte> image)
        {
            ZoomPhotoView.Image = image.Bitmap;

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

        public event EventHandler<Rectangle> RectangleLoaded;
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region SelectROI
        public Rectangle CutRectangle
        {
            get
            {
                return rect;
            }
        }
        Rectangle rect;
        System.Drawing.Point startPoint;
        System.Drawing.Point endPoint;
        bool mouseDown = false;

        public void CutBorderInitialize()
        {
            ZoomPhotoView.SetZoomScale(1, new System.Drawing.Point(1, 1));           
            ZoomPhotoView.MouseDown += PhotoView_MouseDown;
            ZoomPhotoView.MouseUp += PhotoView_MouseUp;
            ZoomPhotoView.MouseMove += PhotoView_MouseMove;
            ZoomPhotoView.Paint += PhotoView_PaintRectangle;            
        }

        public void CutBorderFinalize()
        {
            mouseDown = false;
            ZoomPhotoView.MouseDown -= PhotoView_MouseDown;
            ZoomPhotoView.MouseUp -= PhotoView_MouseUp;
            ZoomPhotoView.MouseMove -= PhotoView_MouseMove;
            ZoomPhotoView.Paint -= PhotoView_PaintRectangle;
        }
        

        private void PhotoView_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            startPoint = e.Location;
            mouseDown = true;
        }

        private void PhotoView_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if(mouseDown)
            {
                endPoint = e.Location;
                ZoomPhotoView.Invalidate();
            }
        }

        private void PhotoView_PaintRectangle(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            if(rect!=null)
            {
                e.Graphics.DrawRectangle(Pens.DeepSkyBlue, GetRectangle());
            }
        }        

        private Rectangle GetRectangle()
        {
            rect = new Rectangle()
            {
                Location = startPoint,
                Width = endPoint.X - startPoint.X,
                Height = endPoint.Y - startPoint.Y
            };

            return rect;
        }

        private void PhotoView_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            endPoint = e.Location;
            GetRectangle();          
            RectangleLoaded?.Invoke(sender, rect);
            CutBorderFinalize();           
        }       
        #endregion SelectROI    
    }
}

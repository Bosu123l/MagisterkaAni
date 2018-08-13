namespace UI.Controls
{
    partial class ImageViewer
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.scMainContainer = new System.Windows.Forms.SplitContainer();
            this.ShowedImage = new Emgu.CV.UI.PanAndZoomPictureBox();
            this.flpImageSizeControler = new System.Windows.Forms.FlowLayoutPanel();
            this.tpHistogramsPanel = new System.Windows.Forms.TableLayoutPanel();
            this.hbBlueHistogram = new Emgu.CV.UI.HistogramBox();
            this.hbGreenHistogram = new Emgu.CV.UI.HistogramBox();
            this.hbRedHistogram = new Emgu.CV.UI.HistogramBox();
            this.hbBlackHistogram = new Emgu.CV.UI.HistogramBox();
            ((System.ComponentModel.ISupportInitialize)(this.scMainContainer)).BeginInit();
            this.scMainContainer.Panel1.SuspendLayout();
            this.scMainContainer.Panel2.SuspendLayout();
            this.scMainContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ShowedImage)).BeginInit();
            this.tpHistogramsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // scMainContainer
            // 
            this.scMainContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scMainContainer.Location = new System.Drawing.Point(0, 0);
            this.scMainContainer.Name = "scMainContainer";
            this.scMainContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // scMainContainer.Panel1
            // 
            this.scMainContainer.Panel1.Controls.Add(this.flpImageSizeControler);
            this.scMainContainer.Panel1.Controls.Add(this.ShowedImage);
            // 
            // scMainContainer.Panel2
            // 
            this.scMainContainer.Panel2.Controls.Add(this.tpHistogramsPanel);
            this.scMainContainer.Size = new System.Drawing.Size(1009, 696);
            this.scMainContainer.SplitterDistance = 540;
            this.scMainContainer.TabIndex = 0;
            // 
            // ShowedImage
            // 
            this.ShowedImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ShowedImage.Location = new System.Drawing.Point(0, 0);
            this.ShowedImage.Name = "ShowedImage";
            this.ShowedImage.Size = new System.Drawing.Size(1009, 540);
            this.ShowedImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.ShowedImage.TabIndex = 0;
            this.ShowedImage.TabStop = false;
            // 
            // flpImageSizeControler
            // 
            this.flpImageSizeControler.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.flpImageSizeControler.Location = new System.Drawing.Point(0, 440);
            this.flpImageSizeControler.Name = "flpImageSizeControler";
            this.flpImageSizeControler.Size = new System.Drawing.Size(1009, 100);
            this.flpImageSizeControler.TabIndex = 1;
            // 
            // tpHistogramsPanel
            // 
            this.tpHistogramsPanel.ColumnCount = 2;
            this.tpHistogramsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tpHistogramsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tpHistogramsPanel.Controls.Add(this.hbBlueHistogram, 0, 0);
            this.tpHistogramsPanel.Controls.Add(this.hbGreenHistogram, 0, 1);
            this.tpHistogramsPanel.Controls.Add(this.hbRedHistogram, 1, 0);
            this.tpHistogramsPanel.Controls.Add(this.hbBlackHistogram, 1, 1);
            this.tpHistogramsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tpHistogramsPanel.Location = new System.Drawing.Point(0, 0);
            this.tpHistogramsPanel.Name = "tpHistogramsPanel";
            this.tpHistogramsPanel.RowCount = 2;
            this.tpHistogramsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tpHistogramsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tpHistogramsPanel.Size = new System.Drawing.Size(1009, 152);
            this.tpHistogramsPanel.TabIndex = 0;
            // 
            // hbBlueHistogram
            // 
            this.hbBlueHistogram.AutoSize = true;
            this.hbBlueHistogram.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.hbBlueHistogram.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hbBlueHistogram.Location = new System.Drawing.Point(3, 3);
            this.hbBlueHistogram.Name = "hbBlueHistogram";
            this.hbBlueHistogram.Size = new System.Drawing.Size(498, 70);
            this.hbBlueHistogram.TabIndex = 0;
            // 
            // hbGreenHistogram
            // 
            this.hbGreenHistogram.AutoSize = true;
            this.hbGreenHistogram.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.hbGreenHistogram.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hbGreenHistogram.Location = new System.Drawing.Point(3, 79);
            this.hbGreenHistogram.Name = "hbGreenHistogram";
            this.hbGreenHistogram.Size = new System.Drawing.Size(498, 70);
            this.hbGreenHistogram.TabIndex = 1;
            // 
            // hbRedHistogram
            // 
            this.hbRedHistogram.AutoSize = true;
            this.hbRedHistogram.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.hbRedHistogram.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hbRedHistogram.Location = new System.Drawing.Point(507, 3);
            this.hbRedHistogram.Name = "hbRedHistogram";
            this.hbRedHistogram.Size = new System.Drawing.Size(499, 70);
            this.hbRedHistogram.TabIndex = 2;
            // 
            // hbBlackHistogram
            // 
            this.hbBlackHistogram.AutoSize = true;
            this.hbBlackHistogram.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.hbBlackHistogram.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hbBlackHistogram.Location = new System.Drawing.Point(507, 79);
            this.hbBlackHistogram.Name = "hbBlackHistogram";
            this.hbBlackHistogram.Size = new System.Drawing.Size(499, 70);
            this.hbBlackHistogram.TabIndex = 3;
            // 
            // ImageViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.scMainContainer);
            this.Name = "ImageViewer";
            this.Size = new System.Drawing.Size(1009, 696);
            this.scMainContainer.Panel1.ResumeLayout(false);
            this.scMainContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scMainContainer)).EndInit();
            this.scMainContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ShowedImage)).EndInit();
            this.tpHistogramsPanel.ResumeLayout(false);
            this.tpHistogramsPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer scMainContainer;
        private System.Windows.Forms.FlowLayoutPanel flpImageSizeControler;
        private Emgu.CV.UI.PanAndZoomPictureBox ShowedImage;
        private System.Windows.Forms.TableLayoutPanel tpHistogramsPanel;
        private Emgu.CV.UI.HistogramBox hbBlueHistogram;
        private Emgu.CV.UI.HistogramBox hbGreenHistogram;
        private Emgu.CV.UI.HistogramBox hbRedHistogram;
        private Emgu.CV.UI.HistogramBox hbBlackHistogram;
    }
}

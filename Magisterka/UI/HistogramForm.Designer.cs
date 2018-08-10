namespace UI
{
    partial class HistogramForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.ibImageBox = new Emgu.CV.UI.ImageBox();
            this.bhHistogram = new Emgu.CV.UI.HistogramBox();
            this.zoomBox = new Emgu.CV.UI.PanAndZoomPictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.ibImageBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.zoomBox)).BeginInit();
            this.SuspendLayout();
            // 
            // ibImageBox
            // 
            this.ibImageBox.Location = new System.Drawing.Point(12, 12);
            this.ibImageBox.Name = "ibImageBox";
            this.ibImageBox.Size = new System.Drawing.Size(584, 267);
            this.ibImageBox.TabIndex = 2;
            this.ibImageBox.TabStop = false;
            // 
            // bhHistogram
            // 
            this.bhHistogram.Location = new System.Drawing.Point(602, 12);
            this.bhHistogram.Name = "bhHistogram";
            this.bhHistogram.Size = new System.Drawing.Size(671, 426);
            this.bhHistogram.TabIndex = 3;
            // 
            // zoomBox
            // 
            this.zoomBox.Location = new System.Drawing.Point(12, 285);
            this.zoomBox.Name = "zoomBox";
            this.zoomBox.Size = new System.Drawing.Size(584, 490);
            this.zoomBox.TabIndex = 4;
            this.zoomBox.TabStop = false;
            // 
            // HistogramForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1285, 787);
            this.Controls.Add(this.zoomBox);
            this.Controls.Add(this.bhHistogram);
            this.Controls.Add(this.ibImageBox);
            this.Name = "HistogramForm";
            this.Text = "HistogramForm";
            ((System.ComponentModel.ISupportInitialize)(this.ibImageBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.zoomBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Emgu.CV.UI.ImageBox ibImageBox;
        private Emgu.CV.UI.HistogramBox bhHistogram;
        private Emgu.CV.UI.PanAndZoomPictureBox zoomBox;
    }
}
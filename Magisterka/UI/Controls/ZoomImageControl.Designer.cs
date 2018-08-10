namespace UI.Controls
{
    partial class ZoomImageControl
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
            this.panAndZoomPictureBox1 = new Emgu.CV.UI.PanAndZoomPictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.panAndZoomPictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // panAndZoomPictureBox1
            // 
            this.panAndZoomPictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panAndZoomPictureBox1.Location = new System.Drawing.Point(0, 0);
            this.panAndZoomPictureBox1.Name = "panAndZoomPictureBox1";
            this.panAndZoomPictureBox1.Size = new System.Drawing.Size(701, 440);
            this.panAndZoomPictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.panAndZoomPictureBox1.TabIndex = 0;
            this.panAndZoomPictureBox1.TabStop = false;
            // 
            // ZoomImageControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panAndZoomPictureBox1);
            this.Name = "ZoomImageControl";
            this.Size = new System.Drawing.Size(701, 440);
            ((System.ComponentModel.ISupportInitialize)(this.panAndZoomPictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Emgu.CV.UI.PanAndZoomPictureBox panAndZoomPictureBox1;
    }
}

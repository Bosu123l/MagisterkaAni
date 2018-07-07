using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Scratches
    {
        private void RemoveDustParticles()
        {
            //for (int i = 0; i < defects.Count(); i++)
            //{
            //    RemoveSpeckOfDust(defects[i], orgImage, dustMask);
            //}
        }
        private void RemoveSpeckOfDust(Point[] defectContour, Image<Bgr, byte> orgImage, Image<Gray, byte> dustMask)
        {
            Rectangle roi;
            Image<Gray, byte> dustMaskNegative;
            Image<Bgr, byte> temp;
            Image<Bgr, byte> tempMask = new Image<Bgr, byte>(dustMask.Bitmap);

            roi = GetRectangleFromContour(defectContour, orgImage.Width, orgImage.Height);
            orgImage.ROI = roi;
            dustMask.ROI = roi;
            tempMask.ROI = roi;
            dustMaskNegative = dustMask.Not();

            temp = tempMask.CopyBlank();
            tempMask.CopyTo(temp);
            tempMask.ROI = Rectangle.Empty;

            temp.CopyTo(orgImage);
            CvInvoke.cvCopy(temp, orgImage, IntPtr.Zero);

            orgImage.ROI = Rectangle.Empty;
            dustMask.ROI = Rectangle.Empty;

            dustMaskNegative.Dispose();
            temp.Dispose();
            tempMask.Dispose();
        }

        private Rectangle GetRectangleFromContour(Point[] points, int maxWidth, int maxHeight)
        {
            int x1, x2, y1, y2;
            int width, height;
            int abs = 0;

            x1 = points.Min(x => x.X);
            y1 = points.Min(y => y.Y);
            x2 = points.Max(x => x.X);
            y2 = points.Max(y => y.Y);

            x1 = (x1 - abs) < 0 ? 0 : (x1 - abs);
            y1 = (y1 - abs) < 0 ? 0 : (y1 - abs);
            x2 = (x2 + abs) > maxWidth ? maxWidth : (x2 + abs);
            y2 = (y2 + abs) > maxHeight ? maxHeight : (y2 + abs);

            width = Math.Abs(x2 - x1);
            height = Math.Abs(y2 - y1);

            return new Rectangle(x1, y1, width, height);
        }
    }
}

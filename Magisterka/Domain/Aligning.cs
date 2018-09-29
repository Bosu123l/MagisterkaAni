using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public static class Aligning
    {
        const double maxAngle= 360;

        public static Image<Bgr, byte> Rotate(Image<Bgr, byte> image, double angle = 1)
        {
            return image.Rotate(angle, new Bgr(), false);
        }

        public static Image<Bgr, byte> RotateOn90(Image<Bgr, byte> image)
        {
            return Rotate(image, 90);
        }
    }
}

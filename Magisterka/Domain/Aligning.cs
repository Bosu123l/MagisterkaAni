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

        public static ImageWrapper<Bgr, byte> Rotate(ImageWrapper<Bgr, byte> image, double angle = 1)
        {
            return image.Rotate(angle, new Bgr());
        }

        public static ImageWrapper<Bgr, byte> RotateOn90(ImageWrapper<Bgr, byte> image)
        {
            return Rotate(image, 90);
        }
    }
}

using Emgu.CV;
using Emgu.CV.Structure;

namespace Domain
{
    public class DustRemoval
    {
        private Image<Bgr, byte> image;

        public DustRemoval(Image<Bgr, byte> im)
        {
            if (im != null)
                image = im;
        }

        public Image<Gray, byte> RemoveDust()
        {
            Image<Gray, byte> cleaned = image.Convert<Gray, byte>();
            //Image<Gray, byte> grey_org = image.Convert<Gray, byte>();
            //Image<Gray, byte> erosion = grey_org.Erode(2);
            //Image<Gray, byte> diff = grey_org.Sub(erosion);
            //Image<Gray, byte> temp = diff.Not();
            //Image<Gray, byte> temp2 = grey_org.Erode(3);

            //temp2.Sub(temp)._EqualizeHist();
            // temp.Erode(5);

            //cleaned = temp2;
            //cleaned=grey_org.SmoothGaussian(3);

            //Image<Gray, byte> grey_org_black = cleaned.Not();
            //Image<Gray, byte> erosion2 = grey_org_black.Erode(2);
            //Image<Gray, byte> diff2 = grey_org.Sub(erosion);
            //Image<Gray, byte> temp3 = diff.Not();
            //Image<Gray, byte> temp4 = grey_org.Erode(10);

            //temp4.Sub(temp3)._EqualizeHist();

            //cleaned=diff;

            //for (int i = 0; i < 5; i++)
            //{
            //    cleaned._Erode(5);
            //    cleaned._Dilate(2);
            //}

            //cleaned._Not();

            //for (int i = 0; i <5; i++)
            //{
            //    cleaned._Erode(5);
            //    cleaned._Dilate(2);
            //}

            //cleaned._Not();
            //cleaned._SmoothGaussian(3);

            //cleaned.Sobel(2, 0, 5);

            cleaned = image.Canny(250, 190);

            return cleaned;

        }
    }
}

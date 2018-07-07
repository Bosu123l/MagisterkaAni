using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Domain
{
    public class Dust
    {
        private Point[][] _conturMatrix;
        private Image<Bgr, byte> _orgImage;
        private Image<Gray, byte> _dustMask;
        private Image<Bgr, byte> _cleanedImage;

        public Dust(Image<Bgr, byte> orgImage, Image<Gray, byte> dustMask, Point[][] conturMatrix)
        {
            if (orgImage != null) 
            {
                _orgImage = orgImage;
            }
            else
            {
                throw new ArgumentNullException(nameof(orgImage));
            }
            if (dustMask != null)
            {
                _dustMask = dustMask;
            }
            else
            {
                throw new ArgumentNullException(nameof(dustMask));
            }
            if (conturMatrix != null)
            {
                _conturMatrix = conturMatrix;
            }
            else
            {
                throw new ArgumentNullException(nameof(conturMatrix));
            }
        }

        public Image<Bgr, byte> RemoveDust()
        {
            RemoveBrigtherDefects();
            RemoveDarkerDefects();

            return _cleanedImage;
        } 
        
        private void RemoveBrigtherDefects()
        {
            Image<Bgr, float> patchImage;
            Image<Bgr, float> brigtherMask;

            #region WhiteOnBlack
            brigtherMask = MorphologicalProcessing.CreateBinaryImage(_orgImage, 100).Convert<Bgr,float>().Mul(_dustMask.Convert<Bgr,float>());
            patchImage = MorphologicalProcessing.Erode(_orgImage, new Size(5, 5), 5).Convert<Bgr, float>();
            _cleanedImage = patchImage.Convert<Bgr,float>().Mul(brigtherMask).Convert<Bgr,byte>().Add(MorphologicalProcessing.GreateBinaryImageNegative(brigtherMask.Convert<Gray,byte>()).Convert<Bgr,float>().Mul(_orgImage.Convert<Bgr,float>()).Convert<Bgr,byte>());
            #endregion WhiteOnBlack

            #region WhiteOnWhite
            #endregion WhiteOnWhite
        }

        private void RemoveDarkerDefects()
        {

        }       
    }
}

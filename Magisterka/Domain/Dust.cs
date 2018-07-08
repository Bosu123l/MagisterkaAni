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
            Image<Bgr, byte> patchImage;
            Image<Gray, byte> brigtherMask;

            #region WhiteOnBlack
            brigtherMask = MorphologicalProcessing.MultipleImages(MorphologicalProcessing.CreateBinaryImage(_orgImage, 100), _dustMask).Convert<Gray,byte>();
            patchImage = MorphologicalProcessing.Erode(_orgImage, new Size(3, 3), 10);
            _cleanedImage = MorphologicalProcessing.CombineTwoImages(_orgImage, patchImage, brigtherMask);
            _cleanedImage =_cleanedImage.SmoothBlur(10, 10);
            #endregion WhiteOnBlack

            #region WhiteOnWhite

            #endregion WhiteOnWhite
            brigtherMask.Dispose();
            patchImage.Dispose();
        }

        private void RemoveDarkerDefects()
        {

        }       
    }
}

using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Domain
{
    public class Dust: IDisposable
    {
        private Point[][] _conturMatrix;
        private ImageWrapper<Bgr, byte> _orgImage;
        private ImageWrapper<Gray, byte> _dustMask;   

        public Dust(ImageWrapper<Bgr, byte> orgImage, ImageWrapper<Gray, byte> dustMask, Point[][] conturMatrix)
        {
            if (orgImage != null) 
            {
                _orgImage = orgImage.Copy();
            }
            else
            {
                throw new ArgumentNullException(nameof(orgImage));
            }
            if (dustMask != null)
            {
                _dustMask = dustMask.Copy();
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

        public ImageWrapper<Bgr, byte> RemoveDust()
        {
            ImageWrapper<Bgr, byte> brightSpotsPatchImage;
            ImageWrapper<Gray, byte> brigtherSpotsMask;

            ImageWrapper<Bgr, byte> patchImage = _orgImage.CopyBlank();
            ImageWrapper<Gray, byte> binaryOrgImage = MorphologicalProcessing.CreateBinaryImage(_orgImage, 100);

            #region WhiteOnBlack
            brigtherSpotsMask = MorphologicalProcessing.MultipleImages(binaryOrgImage, _dustMask);
            brightSpotsPatchImage = MorphologicalProcessing.Erode(_orgImage, new Size(3, 3), 10);
            patchImage = MorphologicalProcessing.CombineTwoImages(patchImage, brightSpotsPatchImage, _dustMask /*brigtherSpotsMask*/);
            brightSpotsPatchImage.Dispose();            
            brigtherSpotsMask.Dispose();
            #endregion WhiteOnBlack


            //#region BlackOnWhite
            //Image<Bgr, byte> darkSpotsPatchImage;
            //Image<Gray, byte> darkSpotsMask;
            //binaryOrgImage = MorphologicalProcessing.GenerateBinaryImageNegative(binaryOrgImage);
            //darkSpotsMask = MorphologicalProcessing.MultipleImages(binaryOrgImage, _dustMask);
            //darkSpotsPatchImage = MorphologicalProcessing.Dilate(_orgImage, new Size(3, 3), 10);
            //patchImage = MorphologicalProcessing.CombineTwoImages(patchImage, darkSpotsPatchImage, darkSpotsMask);


            //darkSpotsPatchImage.Dispose();
            //darkSpotsMask.Dispose();
            //#endregion BlackOnWhite

            binaryOrgImage.Dispose();

            ImageWrapper<Bgr, byte> _cleanedImage = _orgImage.CopyBlank();
            _cleanedImage = MorphologicalProcessing.CombineTwoImages(_orgImage, patchImage, _dustMask);
            patchImage.Dispose();
            _cleanedImage = _cleanedImage.SmoothBlur(10, 10);
            _dustMask = MorphologicalProcessing.Dilate(_dustMask, new Size(3, 3), 2);
            _cleanedImage = MorphologicalProcessing.CombineTwoImages(_orgImage, _cleanedImage, _dustMask);

           
            return _cleanedImage;
        }

        public void Dispose()
        {
            _conturMatrix = null;
            _orgImage.Dispose();
            _dustMask.Dispose();
        }
    }
}

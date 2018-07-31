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
        //private Point[][] _conturMatrix;
        private Image<Bgr, byte> _orgImage;
        private Image<Gray, byte> _dustMask;      
        

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
            //if (conturMatrix != null)
            //{
            //    _conturMatrix = conturMatrix;
            //}
            //else
            //{
            //    throw new ArgumentNullException(nameof(conturMatrix));
            //}
        }

        public Image<Bgr, byte> RemoveDust()
        {
            Image<Bgr, byte> brightSpotsPatchImage;
            Image<Gray, byte> brigtherSpotsMask;

            Image<Bgr, byte> darkSpotsPatchImage;
            Image<Gray, byte> darkSpotsMask;

            Image<Bgr, byte> _cleanedImage = _orgImage.CopyBlank(); 
            Image<Bgr, byte> patchImage = _orgImage.CopyBlank();
            Image<Gray, byte> binaryOrgImage = MorphologicalProcessing.CreateBinaryImage(_orgImage, 100);

            #region WhiteOnBlack
            brigtherSpotsMask = MorphologicalProcessing.MultipleImages(binaryOrgImage, _dustMask);
            brightSpotsPatchImage = MorphologicalProcessing.Erode(_orgImage, new Size(3, 3), 10);
            patchImage = MorphologicalProcessing.CombineTwoImages(patchImage, brightSpotsPatchImage, _dustMask /*brigtherSpotsMask*/);            
            brightSpotsPatchImage.Dispose();
            brigtherSpotsMask.Dispose();
            #endregion WhiteOnBlack

            //#region BlackOnWhite
            //binaryOrgImage = MorphologicalProcessing.GenerateBinaryImageNegative(binaryOrgImage);
            //darkSpotsMask = MorphologicalProcessing.MultipleImages(binaryOrgImage, _dustMask);
            //darkSpotsPatchImage = MorphologicalProcessing.Dilate(_orgImage, new Size(3, 3), 10);
            //patchImage = MorphologicalProcessing.CombineTwoImages(patchImage, darkSpotsPatchImage, darkSpotsMask);


            //darkSpotsPatchImage.Dispose();
            //darkSpotsMask.Dispose();
            //#endregion BlackOnWhite

            binaryOrgImage.Dispose();
            _cleanedImage = MorphologicalProcessing.CombineTwoImages(_orgImage, patchImage, _dustMask);
            patchImage.Dispose();
            _cleanedImage = _cleanedImage.SmoothBlur(10, 10);
            _dustMask = MorphologicalProcessing.Dilate(_dustMask, new Size(3, 3), 2);
            _cleanedImage = MorphologicalProcessing.CombineTwoImages(_orgImage, _cleanedImage, _dustMask);          
            
            #region DisposeRegion           
            _orgImage.Dispose();
            _dustMask.Dispose();
            #endregion DisposeRegion

            return _cleanedImage;
        } 
    }
}

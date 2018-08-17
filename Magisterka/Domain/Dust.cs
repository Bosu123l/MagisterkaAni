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
            ImageWrapper<Bgr, byte> patchImage;

            ProgressManager.AddSteps(5);
            using(ImageWrapper<Gray, byte> binaryOrgImage = MorphologicalProcessing.CreateBinaryImage(_orgImage, 100))
            {
                ProgressManager.DoStep();
                #region WhiteOnBlack
                using (ImageWrapper<Bgr, byte> brigtherPatchImage = _orgImage.CopyBlank())
                {
                    ProgressManager.DoStep();
                    using(ImageWrapper<Gray, byte> brigtherSpotsMask = MorphologicalProcessing.MultipleImages(binaryOrgImage, _dustMask))
                    {
                        ProgressManager.DoStep();
                        using (ImageWrapper<Bgr, byte> brightSpotsPatchImage = MorphologicalProcessing.Erode(_orgImage, new Size(3, 3), 10))
                        {
                            ProgressManager.DoStep();
                            patchImage = MorphologicalProcessing.CombineTwoImages(brigtherPatchImage, brightSpotsPatchImage, brigtherSpotsMask);

                           
                        }    
                    }
                }
                #endregion WhiteOnBlack
            }
         

            ImageWrapper<Bgr, byte> _cleanedImage = _orgImage.CopyBlank();
            //_cleanedImage = MorphologicalProcessing.CombineTwoImages(_orgImage, patchImage, _dustMask);
            //patchImage.Dispose();
            //_cleanedImage = _cleanedImage.SmoothBlur(10, 10);
            //_dustMask = MorphologicalProcessing.Dilate(_dustMask, new Size(3, 3), 2);
            //_cleanedImage = MorphologicalProcessing.CombineTwoImages(_orgImage, _cleanedImage, _dustMask);


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

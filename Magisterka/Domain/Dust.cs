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
        private ImageWrapper<Bgr, byte> _cleanedImage;
        private ImageWrapper<Gray, byte> _dustMask;   

        public Dust(ImageWrapper<Bgr, byte> orgImage, ImageWrapper<Gray, byte> dustMask, Point[][] conturMatrix)
        {
            if (orgImage != null) 
            {
                _orgImage = orgImage.Copy();
                _cleanedImage = _orgImage.Copy();
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

        public ImageWrapper<Bgr, byte> RemoveDefect(Point[] defectCountour)
        {
            using (ImageWrapper<Gray, byte> mask = MorphologicalProcessing.CreateMaskFromPoints(_cleanedImage.Convert<Gray, byte>(), new Point[][] { defectCountour }))
            {
                //var countur = new Emgu.CV.Util.VectorOfVectorOfPoint(new Point[][] { defectCountour });
                CvInvoke.Inpaint(_cleanedImage.Image, mask.Image, _cleanedImage.Image, 20, Emgu.CV.CvEnum.InpaintType.Telea);
                //CvInvoke.DrawContours(_cleanedImage.Image, countur, -1, new MCvScalar(255, 0, 255));
            }              

            return _cleanedImage;
        }

        public ImageWrapper<Bgr, byte> RemoveDust()
        {
            ImageWrapper<Bgr, byte> cleanedImage = _orgImage.CopyBlank(); ;

            //_dustMask;

            //import numpy as np
            //import cv2 as cv
            //img = cv.imread('messi_2.jpg')
            //mask = cv.imread('mask2.png', 0)
            //dst = cv.inpaint(img, mask, 3, cv.INPAINT_TELEA)
            //cv.imshow('dst', dst)
            //cv.waitKey(0)
            //cv.destroyAllWindows()
            //https://docs.opencv.org/3.4.1/df/d3d/tutorial_py_inpainting.html
            //https://docs.opencv.org/3.1.0/df/d3d/tutorial_py_inpainting.html
            //https://docs.opencv.org/3.4.1/df/d3d/tutorial_py_inpainting.html
            CvInvoke.Inpaint(_orgImage.Image, _dustMask.Image, cleanedImage.Image, 50, Emgu.CV.CvEnum.InpaintType.Telea);

            //ProgressManager.AddSteps(5);
            //using(ImageWrapper<Gray, byte> binaryOrgImage = MorphologicalProcessing.CreateBinaryImage(_orgImage, 100))
            //{
            //    ProgressManager.DoStep();
            //    #region WhiteOnBlack
            //    using (ImageWrapper<Bgr, byte> brigtherPatchImage = _orgImage.CopyBlank())
            //    {
            //        ProgressManager.DoStep();
            //        using(ImageWrapper<Gray, byte> brigtherSpotsMask = MorphologicalProcessing.MultipleImages(binaryOrgImage, _dustMask))
            //        {
            //            ProgressManager.DoStep();
            //            using (ImageWrapper<Bgr, byte> brightSpotsPatchImage = MorphologicalProcessing.Erode(_orgImage, new Size(3, 3), 10))
            //            {
            //                ProgressManager.DoStep();
            //                patchImage = MorphologicalProcessing.CombineTwoImages(brigtherPatchImage, brightSpotsPatchImage, brigtherSpotsMask);


            //            }    
            //        }
            //    }
            //    #endregion WhiteOnBlack
            //}


            //ImageWrapper<Bgr, byte> _cleanedImage = _orgImage.CopyBlank();
            //_cleanedImage = MorphologicalProcessing.CombineTwoImages(_orgImage, patchImage, _dustMask);
            //patchImage.Dispose();
            //_cleanedImage = _cleanedImage.SmoothBlur(10, 10);
            //_dustMask = MorphologicalProcessing.Dilate(_dustMask, new Size(3, 3), 2);
            //_cleanedImage = MorphologicalProcessing.CombineTwoImages(_orgImage, _cleanedImage, _dustMask);


            return cleanedImage;
        }

        public void Dispose()
        {
            _conturMatrix = null;
            _orgImage.Dispose();
            _dustMask.Dispose();
        }
    }
}

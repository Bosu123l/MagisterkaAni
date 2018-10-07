using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain
{
    public class Dust: IDisposable
    {
        private VectorOfVectorOfPoint _conturMatrix;
        private Image<Bgr, byte> _orgImage;
        private int _kernelSize = 5;

        public Dust(Image<Bgr, byte> orgImage, VectorOfVectorOfPoint conturMatrix)
        {
            if (orgImage != null) 
            {
                _orgImage = orgImage.Copy();
            }
            else
            {
                throw new ArgumentNullException(nameof(orgImage));
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

        public Image<Bgr, byte> DustReductionLeftToRight()
        {
            using (ImageCleaner ic = new ImageCleaner(_orgImage, _conturMatrix, _kernelSize))
            {
                return ic.LeftToRightClearWholeImageByDefects().Copy();
            }
        }

        public Image<Bgr, byte> DustReductionSpiralAveranging()
        {
            using (ImageCleaner ic = new ImageCleaner(_orgImage, _conturMatrix, _kernelSize))
            {
                return ic.SpiralCleanWholeImage().Copy();
            }
        }

        public void Dispose()
        {
            _conturMatrix = null;
            _orgImage.Dispose();

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}

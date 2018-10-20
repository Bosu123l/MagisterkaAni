using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;


namespace Domain
{
    public class Dust: IDisposable
    {
        private VectorOfVectorOfPoint _conturMatrix;
        private Image<Bgr, byte> _orgImage;
        private Image<Gray, byte> _exclFromCleaning;
        private int _kernelSize = Settings.Settings.DustKernelSize;

        public Dust(Image<Bgr, byte> orgImage, VectorOfVectorOfPoint conturMatrix, Image<Gray, byte> exclFromCleaning)
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
            using (ImageCleaner ic = new ImageCleaner(_orgImage, _conturMatrix, _kernelSize, _exclFromCleaning))
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

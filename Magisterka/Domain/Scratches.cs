using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Domain
{
    public class Scratches: IDisposable
    {
        private VectorOfVectorOfPoint _conturMatrix;
        private Image<Bgr, byte> _orgImage;
        private Image<Gray, byte> _exclFromCleaning;
        private int _kernelSize = Settings.Settings.ScratchesKernelSize;
        
        public Scratches(Image<Bgr, byte> orgImage, VectorOfVectorOfPoint conturMatrix, Image<Gray, byte> exclFromCleaning)
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

            _exclFromCleaning = exclFromCleaning;
        }

        public Image<Bgr,byte>RemoveScrates()
        {           
            return InpaintTeleaMethod();
        }

        private Image<Gray, byte>GetMaskOfDefect(VectorOfPoint defect)
        {
            Image<Gray, byte> mask = _orgImage[0].CopyBlank();
            CvInvoke.FillPoly(mask, new VectorOfVectorOfPoint(defect), new MCvScalar(255,255,255), LineType.FourConnected);
            return mask;
        }        

        #region SpiralCleaner
        public Image<Bgr,byte> ScratchesReductionSpiralAveranging()
        {
            using (ImageCleaner ic = new ImageCleaner(_orgImage, _conturMatrix, _kernelSize, _exclFromCleaning))
            {
                return ic.SpiralCleanSingleDefects();
            }
        }
        #endregion SpiralCleaner

        #region Inpaint
        public Image<Bgr, byte> InpaintTeleaMethod()
        {
            return InpaintMethod(_conturMatrix, InpaintType.Telea);
        }

        public Image<Bgr, byte> InpaintNSMethod()
        {
            return InpaintMethod(_conturMatrix, InpaintType.NS);
        }

        private Image<Bgr, byte> InpaintMethod(VectorOfVectorOfPoint defects, InpaintType ip)
        {
            ProgressManager.AddSteps(_conturMatrix.Size);
            for (int i = 0; i < defects.Size; i++)
            {
                InpaintCleaner(defects[i], ip);
                ProgressManager.DoStep();
            }

            return _orgImage;
        }
        private void InpaintCleaner(VectorOfPoint defect, InpaintType ip)
        {
            var mask = GetMaskOfDefect(defect);
            CvInvoke.Inpaint(_orgImage, mask, _orgImage, 1.0, ip);
        }
        #endregion Inpaint

        public void Dispose()
        {
            _conturMatrix = null;
            _orgImage.Dispose();

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}

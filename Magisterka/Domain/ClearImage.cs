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
    public class ImageCleaner : IDisposable
    {
        struct MatElement
        {
            public int _h;
            public int _w;
            public MCvScalar _color;
        }

        private Image<Bgr, byte> _orgImage;
        private VectorOfVectorOfPoint _defects;
        MCvScalar _defectsColor = new MCvScalar(255, 0, 255);
        private int _kernelSize = 9;

        public ImageCleaner(Image<Bgr, byte> orgImage, VectorOfVectorOfPoint defects, int kernel)
        {
            if (orgImage != null)
            {
                _orgImage = orgImage.Copy();
            }
            else
            {
                throw new ArgumentNullException(nameof(orgImage));
            }

            if (defects != null)
            {
                _defects = defects;
            }
            else
            {
                throw new ArgumentNullException(nameof(defects));
            }

            if (kernel < 2)
            {
                _kernelSize = kernel;
            }
            else
            {
                throw new ArgumentNullException(nameof(kernel));
            }
        }       

        #region LeftToRightCleaner
        public Image<Bgr, byte> LeftToRightClearWholeImageByDefects()
        {
            int hSetp = _kernelSize / 2;
            int wStep = _kernelSize / 2;
            int width = _orgImage.Width - _kernelSize;
            int height = _orgImage.Height - _kernelSize;

            CvInvoke.FillPoly(_orgImage, _defects, _defectsColor, LineType.FourConnected);  
            
            for (int h = 0; h < height; h += hSetp)
            {
                for (int w = 0; w < width; w += wStep)
                {
                    List<MatElement> mat = GetMat(h, w);
                    if (mat != null)
                    {
                        RepairMat(mat);
                    }
                }
            }
            return _orgImage;
        }
        #endregion LeftToRightCleaner

        #region SpiralCleaner
        public Image<Bgr, byte> SpiralCleanWholeImage()
        {
            int step = _kernelSize / 2;
            int minH, minW, maxH, maxW;
            int maxImgH, maxImgW;

            maxImgH = (_orgImage.Height - _kernelSize);
            maxImgW = (_orgImage.Width - _kernelSize);

            minH = 0;
            maxH = _orgImage.Height - _kernelSize;

            minW = 0;
            maxW = _orgImage.Width - _kernelSize;

            SpiralClean(minH, minW, maxH, maxW, maxImgH, maxImgW);

            return _orgImage;
        }
        public Image<Bgr, byte> SpiralCleanSingleDefects()
        {
            int step = _kernelSize / 2;
            int minH, minW, maxH, maxW;
            int maxImgH, maxImgW;
            List<Point> defect = new List<Point>();

            for (int i = 0; i < _defects.Size; i++)
            {
                VectorOfPoint defectContur = _defects[i];              

                CvInvoke.FillPoly(_orgImage, new VectorOfVectorOfPoint(defectContur), _defectsColor, LineType.FourConnected);

                for (int j = 0; j < defectContur.Size; j++)
                {
                    defect.Add(defectContur[i]);
                }

                maxImgH = (_orgImage.Height - _kernelSize);
                maxImgW = (_orgImage.Width - _kernelSize);

                minH = defect.Min(dh => dh.Y);
                minH = (minH - step) > 0 ? (minH - step) : 0;

                maxH = defect.Max(dh => dh.Y);
                maxH = (maxH + step + _kernelSize) <= _orgImage.Height ? (maxH + step) : maxImgH;


                minW = defect.Min(dw => dw.X);
                minW = (minW - step) > 0 ? (minW - step) : 0;
                maxW = defect.Max(dw => dw.X);
                maxW = (maxW + step + _kernelSize) <= _orgImage.Width ? (maxW + step) : maxImgW;

                SpiralClean(minH, minW, maxH, maxW, maxImgH, maxImgW);
            }
            return _orgImage;
        }
        public void SpiralClean(int minH, int minW, int maxH, int maxW, int maxImgH, int maxImgW)
        {                                                                   
            int step = _kernelSize / 2;
            List<Point> defect = new List<Point>();

            int h = minH, w = minW;

            while (minH < maxH || minW < maxW)
            {
                for (w = minW; w < maxW; w += step)
                {
                    SpiralCleanMat(h, w);
                }

                w = maxW > 0 ? maxW : 0;

                for (h = minH; h < maxH; h += step)
                {
                    SpiralCleanMat(h, w);
                }

                h = maxH > 0 ? maxH : 0;

                for (; w > minW; w -= step)
                {
                    SpiralCleanMat(h, w);
                }

                w = minW < maxImgW ? minW : maxImgW;

                for (; h > minH; h -= step)
                {
                    SpiralCleanMat(h, w);
                }

                h = minH < maxImgH ? minH : maxImgH;

                minH += step;
                minW += step;

                maxH -= step;
                maxW -= step;
            }
        }
        private void SpiralCleanMat(int h, int w)
        {
            List<MatElement> mat = GetMat(h, w);
            if (mat != null)
            {
                RepairMat(mat);
            }
        }
        #endregion SpiralCleaner

        #region CommonMethods        
        private List<MatElement> GetMat(int min_h, int min_w)
        {
            List<MatElement> mat = new List<MatElement>();
            bool isDefect = false;

            for (int h = 0; h < _kernelSize; h++)
            {
                int H = min_h + h;
                for (int w = 0; w < _kernelSize; w++)
                {
                    int W = min_w + w;

                    if (_orgImage[H, W].MCvScalar.Equals(_defectsColor) && isDefect == false)
                    {
                        isDefect = true;
                    }
                    mat.Add(new MatElement() { _h = H, _w = W, _color = _orgImage[H, W].MCvScalar });
                }
            }

            if (isDefect)
            {
                return mat;
            }
            return null;
        }
        private void RepairMat(List<MatElement> mat)
        {
            var clearMat = mat.Where(x => !x._color.Equals(_defectsColor)).ToList();

            if (clearMat.Count > 0)
            {
                MCvScalar sum = new MCvScalar();
                double V0, V1, V2;

                clearMat.ForEach(x =>
                {
                    sum.V0 += x._color.V0;
                    sum.V1 += x._color.V1;
                    sum.V2 += x._color.V2;
                });

                V0 = sum.V0 / clearMat.Count;
                V1 = sum.V1 / clearMat.Count;
                V2 = sum.V2 / clearMat.Count;

                mat.Where(x => x._color.Equals(_defectsColor)).ToList().ForEach(x =>
                {
                    _orgImage.Data[x._h, x._w, 0] = (byte)V0;
                    _orgImage.Data[x._h, x._w, 1] = (byte)V1;
                    _orgImage.Data[x._h, x._w, 2] = (byte)V2;
                });
            }
        }
        #endregion CommonMethods       
        
        public void Dispose()
        {

        }
    }
}
